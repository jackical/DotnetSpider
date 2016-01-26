using System;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Monitor;
using Java2Dotnet.Spider.Extension.Scheduler;
using StackExchange.Redis;
using Java2Dotnet.Spider.Redial;
using Java2Dotnet.Spider.Redial.RedialManager;
using Java2Dotnet.Spider.Extension.Utils;
using System.Threading;
using log4net;

namespace Java2Dotnet.Spider.Extension
{
	public abstract class AbstractRedisSpider : IRedisSpider
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(AbstractRedisSpider));

		protected AbstractRedisSpider()
		{
			Core.Spider.PrintInfo();
		}

		private RedisScheduler _scheduler;

		protected RedisScheduler Scheduler => _scheduler ?? (_scheduler = new RedisScheduler(RedisProvider.GetProvider()));

		protected bool IsInited { get; set; }

		public void Run()
		{
			Core.Spider spider = null;
			try
			{
				spider = Prepare();
				spider?.Run();
			}
			finally
			{
				spider?.Dispose();
			}
		}

		private Core.Spider Prepare()
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

			IDatabase db = Scheduler.Redis.GetDatabase(0);
			string key = "locker-" + Name;
			try
			{
				Console.WriteLine($"Lock: {key} to keep only one prepare process.");
				while (!db.LockTake(key, 0, TimeSpan.FromMinutes(10)))
				{
					Thread.Sleep(1000);
				}

				var lockerValue = db.StringGet(Name);
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
					db.StringSet(Name, "init finished");
				}

				Console.WriteLine("Creating Spider finished.");

				return spider;
			}
			catch (Exception e)
			{
				Logger.Error(e.Message, e);
				return null;
			}
			finally
			{
				Console.WriteLine("Release lock.");
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
