using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Extension.Monitor;
using Java2Dotnet.Spider.Extension.Scheduler;
using Java2Dotnet.Spider.Extension.Utils;
using Java2Dotnet.Spider.Lib;
using Java2Dotnet.Spider.Redial;
using Java2Dotnet.Spider.Redial.RedialManager;
using Java2Dotnet.Spider.Validation;
using StackExchange.Redis;
using System.Configuration;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Extension.Configuration;
using Java2Dotnet.Spider.Extension.Downloader;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;
using Java2Dotnet.Spider.Redial.NetworkValidater;
using Java2Dotnet.Spider.Redial.Redialer;
using Java2Dotnet.Spider.WebDriver;
using log4net;
using Browser = Java2Dotnet.Spider.WebDriver.Browser;
using Site = Java2Dotnet.Spider.Core.Site;

namespace Java2Dotnet.Spider.Scripts
{
	public class ScriptSpider
	{
		private const string InitStatusSetName = "init-status";
		private const string ValidateStatusName = "validate-status";
		private readonly ILog _logger = LogManager.GetLogger(typeof(ScriptSpider));

		private readonly string _validateReportTo;
		private ConnectionMultiplexer _redis;

		//todo
		private List<IValidate> _validations;
		private readonly JsonSpider _jsonSpider;
		private readonly RedisScheduler _scheduler = new RedisScheduler(RedisProvider.GetProvider());

		public string Name { get; }

		public ScriptSpider(JsonSpider spider)
		{
			_jsonSpider = spider;

			_validateReportTo = _jsonSpider.ValidationReportTo;
			Name = _jsonSpider.SpiderName;

			InitEnvoriment();
		}

		private void InitEnvoriment()
		{
			if (DbProviderUtil.Provider == null)
			{
				DbProviderUtil.Provider = new DataProviderManager().LoadDataProvider();
			}
			_redis = RedisProvider.GetProvider();

			if (_jsonSpider.NeedRedial)
			{
				RedialManagerUtils.RedialManager.NetworkValidater = GetNetworValidater(_jsonSpider.NetworkValidater);
				RedialManagerUtils.RedialManager.Redialer = GetRedialer(_jsonSpider.Redialer);
			}
		}

		public void Run(params string[] args)
		{
			Core.Spider spider = null;
			try
			{
				spider = Prepare(args);
				spider?.Run();

				RunAfterSpiderFinished();

				if (!string.IsNullOrEmpty(_validateReportTo))
				{
					DoValidate();
				}
			}
			finally
			{
				_redis.Close();
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
			IDatabase db = _redis.GetDatabase(0);
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

		private Core.Spider Prepare(params string[] args)
		{
			IDatabase db = _redis.GetDatabase(0);
			string key = "locker-" + Name;
			if (args != null && args.Length == 1)
			{
				if (args[0] == "rerun")
				{
					db.HashDelete(InitStatusSetName, Name);
					db.HashDelete(ValidateStatusName, Name);
					db.KeyDelete("set-" + Encrypt.Md5Encrypt(Name));
					db.KeyDelete(key);
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

				var spider = GenerateSpider();

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

		private void PrepareSite()
		{
			_jsonSpider.AdvanceStartUrlsProcess?.Build(_jsonSpider.Site);
		}

		private Core.Spider GenerateSpider()
		{
			Site site = GenerateSite();
			EntityProcessor processor = new EntityProcessor(site);
			foreach (var entity in _jsonSpider.Entities)
			{
				processor.AddEntity(entity);
			}

			EntityGeneralSpider spider = new EntityGeneralSpider(_jsonSpider.SpiderName, processor, _scheduler);

			foreach (var entity in _jsonSpider.Entities)
			{
				string entiyName = entity.SelectToken("$.identity").ToString();
				switch (_jsonSpider.Pipeline.PipelineType)
				{
					case PipelineType.MongoDb:
						{
							spider.AddPipeline(new EntityPipeline(entiyName, new EntityMongoDbPipeline(entity.SelectToken("$.schema").ToObject<DbScheme>(), _jsonSpider.Pipeline.Arguments)));
							break;
						}
					case PipelineType.MySql:
						{
							spider.AddPipeline(new EntityPipeline(entiyName, new EntityGeneralPipeline(entity, _jsonSpider.Pipeline.Arguments)));
							break;
						}
				}
			}
			spider.SetCachedSize(_jsonSpider.CachedSize);
			spider.SetEmptySleepTime(_jsonSpider.EmptySleepTime);
			spider.SetThreadNum(_jsonSpider.ThreadNum);
			spider.Deep = _jsonSpider.Deep;
			spider.SetDownloader(GenerateDownloader());
			return spider;
		}

		private IDownloader GenerateDownloader()
		{
			IDownloader downloader = new HttpClientDownloader();

			switch (_jsonSpider.Downloader.Name)
			{
				case "WebDriverDownloader":
					{
						Browser browser;
						if (Enum.TryParse(_jsonSpider.Downloader.Browser.ToString(), out browser))
						{
							var webDriverDownloader = new WebDriverDownloader(browser);

							if (!string.IsNullOrEmpty(_jsonSpider.Downloader.Login.Name))
							{
								switch (_jsonSpider.Downloader.Login.Name)
								{
									case "common":
										{
											webDriverDownloader.Login = (webdriver) =>
											{
												CommonLoginUtil loginUtil = new CommonLoginUtil(_jsonSpider.Downloader.Login.Arguments.ToObject<LoginArguments>());
												loginUtil.Login(webdriver);
												return true;
											};
											break;
										}
								}
							}

							downloader = webDriverDownloader;
						}

						break;
					}
				case "HttpClientDownloader":
					{
						downloader = new HttpClientDownloader();
						break;
					}
				case "FileDownloader":
					{
						downloader = new FileDownloader();
						break;
					}
			}
			return downloader;
		}

		private Site GenerateSite()
		{
			Site site = new Site();
			site.Accept = _jsonSpider.Site.Accept;
			site.AcceptStatCode = _jsonSpider.Site.AcceptStatCode;
			site.Cookie = _jsonSpider.Site.Cookie;
			site.CycleRetryTimes = _jsonSpider.Site.CycleRetryTimes;
			site.Domain = _jsonSpider.Site.Domain;
			site.Encoding = _jsonSpider.Site.Encoding;
			site.Headers = _jsonSpider.Site.Headers;
			site.IsUseGzip = _jsonSpider.Site.IsUseGzip;
			site.RetryTimes = _jsonSpider.Site.RetryTimes;
			site.Timeout = _jsonSpider.Site.Timeout;
			site.SleepTime = _jsonSpider.Site.SleepTime;
			site.UserAgent = _jsonSpider.Site.UserAgent;

			foreach (var startUrl in _jsonSpider.Site.StartUrls)
			{
				site.AddStartUrl(startUrl);
			}

			if (_jsonSpider.AdvanceStartUrlsProcess != null)
			{

			}

			return site;
		}



		protected void RunAfterSpiderFinished()
		{
		}

		private IRedialer GetRedialer(Redialer redialer)
		{
			switch (redialer)
			{
				case Redialer.Adsl:
					{
						return new AdslRedialer();
					}
				case Redialer.H3C:
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
