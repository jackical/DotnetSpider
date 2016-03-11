﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.Configuration;
using Java2Dotnet.Spider.Extension.Downloader;
using Java2Dotnet.Spider.Extension.Downloader.WebDriver;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Monitor;
using Java2Dotnet.Spider.Extension.ORM;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;
using Java2Dotnet.Spider.Extension.Utils;
using Java2Dotnet.Spider.Lib;
using Java2Dotnet.Spider.Redial;
using Java2Dotnet.Spider.Redial.NetworkValidater;
using Java2Dotnet.Spider.Redial.Redialer;
using Java2Dotnet.Spider.Redial.RedialManager;
using Java2Dotnet.Spider.Validation;
using log4net;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace Java2Dotnet.Spider.Extension
{
	public class ScriptSpider
	{
		private const string InitStatusSetName = "init-status";
		private const string ValidateStatusName = "validate-status";
		private readonly ILog _logger = LogManager.GetLogger(typeof(ScriptSpider));

		private readonly string _validateReportTo;

		private readonly List<IValidate> _validations = new List<IValidate>();
		private readonly SpiderContext _spiderContext;

		public string Name { get; }

		public ScriptSpider(SpiderContext spiderContext)
		{
			_spiderContext = spiderContext;

			_validateReportTo = _spiderContext.ValidationReportTo;
			if (!string.IsNullOrEmpty(_validateReportTo))
			{
				CheckValidations();
			}

			Name = _spiderContext.SpiderName;

			InitEnvoriment();
		}

		private void InitEnvoriment()
		{
			if (_spiderContext.NeedRedial)
			{
				RedialManagerUtils.RedialManager = ZookeeperRedialManager.Default;
				RedialManagerUtils.RedialManager.NetworkValidater = GetNetworValidater(_spiderContext.NetworkValidater);
				RedialManagerUtils.RedialManager.Redialer = GetRedialer(_spiderContext.Redialer);
			}
		}

		public void Run(params string[] args)
		{
			Core.Spider spider = null;
			try
			{
				spider = PrepareSpider(args);
				spider?.Run();

				RunAfterSpiderFinished();

				if (!string.IsNullOrEmpty(_validateReportTo))
				{
					DoValidate();
				}
			}
			finally
			{
				spider?.Dispose();
			}
		}

		private void CheckValidations()
		{
			if (_validations != null && _validations.Count > 0)
			{
				foreach (var validation in _validations)
				{
					validation.CheckArguments();
				}
			}
		}

		private void DoValidate()
		{
			var redis = RedisProvider.GetProvider();
			IDatabase db = redis.GetDatabase(0);

			string key = "locker-validate-" + Name;
			try
			{
				Console.WriteLine($"Lock: {key} to keep only one validate process.");
				while (!db.LockTake(key, 0, TimeSpan.FromMinutes(10)))
				{
					Thread.Sleep(1000);
				}

				var lockerValue = db.HashGet(ValidateStatusName, Name).ToString();
				bool needInitStartRequest = lockerValue != "validate finished";

				if (needInitStartRequest)
				{
					Console.WriteLine("Start validate ...");

					if (_validations != null && _validations.Count > 0)
					{
						MailBodyBuilder builder = new MailBodyBuilder(Name, ConfigurationManager.AppSettings["corporation"]);
						foreach (var validation in _validations)
						{
							builder.AddValidateResult(validation.Validate());
						}
						string mailBody = builder.Build();
						EmailUtil.Send($"{Name} " + "validation report", _validateReportTo, mailBody);
					}
				}
				else
				{
					Console.WriteLine("No need to validate on this process because other process did.");
				}

				if (needInitStartRequest)
				{
					db.HashSet(ValidateStatusName, Name, "validate finished");
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				_logger.Error(e.Message, e);
			}
			finally
			{
				Console.WriteLine("Release locker.");
				db.LockRelease(key, 0);
			}
		}

		private Core.Spider PrepareSpider(params string[] args)
		{
			var schedulerType = _spiderContext.Scheduler.SelectToken("$.Type").ToObject<Configuration.Scheduler.Types>();

			switch (schedulerType)
			{
				case Configuration.Scheduler.Types.Queue:
					{
						var schedulerConfig = _spiderContext.Scheduler.ToObject<QueueScheduler>();
						PrepareSite();
						var spider = GenerateSpider(schedulerConfig.GetScheduler());
						spider.InitComponent();
						return spider;

					}
				case Configuration.Scheduler.Types.Redis:
					{
						var schedulerConfig = _spiderContext.Scheduler.ToObject<Configuration.RedisScheduler>();
						var scheduler = (Scheduler.RedisScheduler)schedulerConfig.GetScheduler();

						IDatabase db = scheduler.Redis.GetDatabase(0);
						string key = "locker-" + Name;
						if (args != null && args.Length == 1)
						{
							if (args[0] == "rerun")
							{
								db.KeyDelete(Scheduler.RedisScheduler.GetQueueKey(Name));
								db.KeyDelete(Scheduler.RedisScheduler.GetSetKey(Name));
								db.HashDelete(Scheduler.RedisScheduler.TaskStatus, Name);
								db.KeyDelete(Scheduler.RedisScheduler.ItemPrefix + Name);
								db.KeyDelete(Name);
								db.KeyDelete(key);
								db.SortedSetRemove(Scheduler.RedisScheduler.TaskList, Name);
								db.HashDelete("init-status", Name);
								db.HashDelete("validate-status", Name);
								db.KeyDelete("set-" + Encrypt.Md5Encrypt(Name));
							}
						}

						try
						{
							Console.WriteLine($"Lock: {key} to keep only one prepare process.");
							while (!db.LockTake(key, 0, TimeSpan.FromMinutes(10)))
							{
								Thread.Sleep(1000);
							}

							var lockerValue = db.HashGet(InitStatusSetName, Name);
							bool needInitStartRequest = lockerValue != "init finished";

							if (needInitStartRequest)
							{
								Console.WriteLine("Preparing site...");

								PrepareSite();
							}
							else
							{
								Console.WriteLine("No need to prepare site because other process did it.");
							}

							Console.WriteLine("Start creating Spider...");

							var spider = GenerateSpider(scheduler);

							spider.SaveStatusToRedis = true;
							SpiderMonitor.Default.Register(spider);
							spider.InitComponent();

							if (needInitStartRequest)
							{
								db.HashSet(InitStatusSetName, Name, "init finished");
							}

							Console.WriteLine("Creating Spider finished.");

							return spider;
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
							_logger.Error(e.Message, e);
							return null;
						}
						finally
						{
							Console.WriteLine("Release locker.");
							db.LockRelease(key, 0);
						}
					}
			}

			throw new SpiderExceptoin("Prepare spider failed.");
		}

		private void PrepareSite()
		{
			var type = _spiderContext.PrepareStartUrls.SelectToken("$.Type").ToObject<PrepareStartUrls.Types>();
			PrepareStartUrls prepareStartUrls = null;
			switch (type)
			{
				case PrepareStartUrls.Types.GeneralDb:
					{
						prepareStartUrls = _spiderContext.PrepareStartUrls.ToObject<GeneralDbPrepareStartUrls>();
						break;
					}
				case PrepareStartUrls.Types.Cycle:
					{
						prepareStartUrls = _spiderContext.PrepareStartUrls.ToObject<CyclePrepareStartUrls>();
						break;
					}
			}

			prepareStartUrls?.Build(_spiderContext.Site);
		}

		private Core.Spider GenerateSpider(IScheduler scheduler)
		{
			Site site = _spiderContext.Site;
			EntityProcessor processor = new EntityProcessor(site);
			foreach (var entity in _spiderContext.Entities)
			{
				processor.AddEntity(entity);
			}

			EntityGeneralSpider spider = new EntityGeneralSpider(_spiderContext.SpiderName, processor, scheduler);

			foreach (var entity in _spiderContext.Entities)
			{
				string entiyName = entity.SelectToken("$.Identity")?.ToString();
				var pipelineType = _spiderContext.Pipeline.SelectToken("$.Type").ToObject<Configuration.Pipeline.Types>();
				var schema = entity.SelectToken("$.Schema")?.ToObject<Schema>();

				switch (pipelineType)
				{
					case Configuration.Pipeline.Types.MongoDb:
						{
							var mongoDbPipelineConfig = _spiderContext.Pipeline.ToObject<MongoDbPipeline>();
							spider.AddPipeline(new EntityPipeline(entiyName, mongoDbPipelineConfig.GetPipeline(schema, entity)));

							break;
						}
					case Configuration.Pipeline.Types.MySql:
						{
							var mysqlPipelineConfig = _spiderContext.Pipeline.ToObject<MysqlPipeline>();
							spider.AddPipeline(new EntityPipeline(entiyName, mysqlPipelineConfig.GetPipeline(schema, entity)));
							break;
						}
					case Configuration.Pipeline.Types.MySqlFile:
						{
							var mysqlFilePipelineConfig = _spiderContext.Pipeline.ToObject<MysqlFilePipeline>();

							spider.AddPipeline(new EntityPipeline(entiyName, mysqlFilePipelineConfig.GetPipeline(schema, entity)));
							break;
						}
				}
			}
			spider.SetCachedSize(_spiderContext.CachedSize);
			spider.SetEmptySleepTime(_spiderContext.EmptySleepTime);
			spider.SetThreadNum(_spiderContext.ThreadNum);
			spider.Deep = _spiderContext.Deep;
			spider.SetDownloader(GenerateDownloader());

			if (_spiderContext.CustomizePage != null)
			{
				var customizePageType = _spiderContext.CustomizePage.SelectToken("$.Type").ToObject<CustomizePage.Types>();
				switch (customizePageType)
				{
					case CustomizePage.Types.Sub:
						{
							var customizePage = _spiderContext.CustomizePage.ToObject<SubCustomizePage>();
							spider.CustomizePage = customizePage.Customize;
							break;
						}
				}
			}

			if (_spiderContext.CustomizeTargetUrls != null)
			{
				var customizeTargetUrlsType = _spiderContext.CustomizeTargetUrls.SelectToken("$.Type").ToObject<CustomizeTargetUrls.Types>();
				switch (customizeTargetUrlsType)
				{
					case CustomizeTargetUrls.Types.IncreasePageNumber:
						{
							var customizeTargetUrls = _spiderContext.CustomizeTargetUrls.ToObject<IncreasePageNumberCustomizeTargetUrls>();
							spider.SetCustomizeTargetUrls(customizeTargetUrls.Customize);
							break;
						}
				}
			}

			return spider;
		}

		private IDownloader GenerateDownloader()
		{
			IDownloader downloader = new HttpClientDownloader();

			if (_spiderContext.Downloader != null)
			{
				var downloaderType = _spiderContext.Downloader.SelectToken("$.Type").ToObject<Configuration.Downloader.Types>();
				switch (downloaderType)
				{
					case Configuration.Downloader.Types.WebDriverDownloader:
						{
							var webDriverDownloaderConfig = _spiderContext.Downloader.ToObject<Configuration.WebDriverDownloader>();
							Browser browser;
							if (Enum.TryParse(webDriverDownloaderConfig.Browser.ToString(), out browser))
							{
								var webDriverDownloader = new Downloader.WebDriver.WebDriverDownloader(browser);

								var loginerType = webDriverDownloaderConfig.Login.SelectToken("$.Type")?.ToObject<Loginer.Types>();

								switch (loginerType)
								{
									case Loginer.Types.Common:
										{
											var login = webDriverDownloaderConfig.Login.ToObject<CommonLoginer>();
											webDriverDownloader.Login = login.Login;
											break;
										}
								}

								downloader = webDriverDownloader;
							}

							break;
						}
					case Configuration.Downloader.Types.HttpClientDownloader:
						{
							downloader = new HttpClientDownloader();
							break;
						}
					case Configuration.Downloader.Types.FileDownloader:
						{
							downloader = new FileDownloader();
							break;
						}
				}

				var downloadValidationType = _spiderContext.Downloader.SelectToken("$.DownloadValidation.Type")?.ToObject<Configuration.DownloadValidation.Types>();

				if (downloadValidationType != null)
				{
					switch (downloadValidationType)
					{
						case Configuration.DownloadValidation.Types.Contains:
							{
								var validation = _spiderContext.Downloader.SelectToken("$.DownloadValidation").ToObject<ContainsDownloadValidation>();
								downloader.DownloadValidation = validation.Validate;
								break;
							}
						default:
							{
								throw new SpiderExceptoin("Unspodrt validation type: " + downloadValidationType);
							}
					}
				}
			}
			return downloader;
		}

		protected void RunAfterSpiderFinished()
		{
		}

		private IRedialer GetRedialer(JObject redialer)
		{
			var type = redialer.SelectToken("$.Type").ToObject<Redialer.Types>();
			switch (type)
			{
				case Redialer.Types.Adsl:
					{
						var adslRedialerConfig = redialer.ToObject<Configuration.AdslRedialer>();
						return adslRedialerConfig.GetRedialer();

					}
				case Redialer.Types.H3C:
					{
						return new H3CSshAdslRedialer();
					}
			}
			return null;
		}

		private INetworkValidater GetNetworValidater(NetworkValidater networkValidater)
		{
			switch (networkValidater)
			{
				case NetworkValidater.VpsNetworkValidater:
					{
						return new VpsNetworkValidater();
					}
				case NetworkValidater.DefalutNetworkValidater:
					{
						return new DefalutNetworkValidater();
					}
			}
			return null;
		}
	}
}
