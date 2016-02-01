using System;
using System.Collections.Generic;
using System.Configuration;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Monitor;
using Java2Dotnet.Spider.Extension.Scheduler;
using StackExchange.Redis;
using Java2Dotnet.Spider.Redial;
using Java2Dotnet.Spider.Redial.RedialManager;
using Java2Dotnet.Spider.Extension.Utils;
using System.Threading;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Lib;
using Java2Dotnet.Spider.Validation;
using log4net;

namespace Java2Dotnet.Spider.Extension
{
	public abstract class AbstractSpiderTask : ISpiderTask
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(AbstractSpiderTask));

		private RedisScheduler _scheduler;
		private const string InitStatusSetName = "init-status";
		private const string ValidateStatusName = "validate-status";
		private readonly string _validateReportTo = ConfigurationManager.AppSettings["validationReportTo"];
		private readonly ConnectionMultiplexer _redis;

		protected List<IValidate> Validations;
		protected RedisScheduler Scheduler => _scheduler ?? (_scheduler = new RedisScheduler(RedisProvider.GetProvider()));

		public Core.Spider Spider { get; private set; }

		protected AbstractSpiderTask()
		{
			Core.Spider.PrintInfo();

			if (DbProviderUtil.Provider == null)
			{
				DbProviderUtil.Provider = new DataProviderManager().LoadDataProvider();
			}
			_redis = RedisProvider.GetProvider();
		}

		public void Run(params string[] args)
		{
			try
			{
				if (!string.IsNullOrEmpty(_validateReportTo))
				{
					Validations = InitValidation();

					CheckValidations();
				}

				Spider = Prepare(args);
				Spider?.Run();

				if (!string.IsNullOrEmpty(_validateReportTo))
				{
					DoValidate();
				}
			}
			finally
			{
				_redis.Close();
				Spider?.Dispose();
			}
		}

		private void CheckValidations()
		{
			if (Validations != null && Validations.Count > 0)
			{
				foreach (var validation in Validations)
				{
					validation.CheckArguments();
				}
			}
		}

		protected virtual List<IValidate> InitValidation()
		{
			return null;
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

					if (Validations != null && Validations.Count > 0)
					{
						MailBodyBuilder builder = new MailBodyBuilder(Name, ConfigurationManager.AppSettings["corporation"]);
						foreach (var validation in Validations)
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
				Logger.Error(e.Message, e);
			}
			finally
			{
				Console.WriteLine("Release locker.");
				db.LockRelease(key, 0);
			}
		}

		private Core.Spider Prepare(params string[] args)
		{
			switch (AtomicType)
			{
				case AtomicType.Zookeeper:
					{
						RedialManagerUtils.RedialManager = ZookeeperRedialManager.Default;
						break;
					}
				case AtomicType.File:
					{
						RedialManagerUtils.RedialManager = FileLockerRedialManager.Default;
						break;
					}
				case AtomicType.Null:
					{
						RedialManagerUtils.RedialManager = null;
						break;
					}
			}

			IDatabase db = _redis.GetDatabase(0);

			if (args != null && args.Length == 1)
			{
				if (args[0] == "rerun")
				{
					db.HashDelete(InitStatusSetName, Name);
					db.HashDelete(ValidateStatusName, Name);
					db.KeyDelete("set-" + Encrypt.Md5Encrypt(Name));
				}
			}

			string key = "locker-" + Name;
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

				var spider = InitSpider();

				if (spider.Identify != Name || spider.Scheduler != Scheduler || !Equals(spider.Site, Site))
				{
					throw new SpiderExceptoin("AbstractRedisSpider should only use default Name, Site, Scheduler.");
				}

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
				Logger.Error(e.Message, e);
				return null;
			}
			finally
			{
				Console.WriteLine("Release locker.");
				db.LockRelease(key, 0);
			}
		}

		protected abstract void PrepareSite();
		protected virtual Site Site { get; } = new Site();
		protected abstract Core.Spider InitSpider();

		public abstract string Name { get; }

		public virtual AtomicType AtomicType { get; } = AtomicType.Null;
	}
}
