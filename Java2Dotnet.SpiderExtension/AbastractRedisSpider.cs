using System;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Monitor;
using Java2Dotnet.Spider.Extension.Scheduler;
using Java2Dotnet.Spider.Extension.Utils;
using StackExchange.Redis;

namespace Java2Dotnet.Spider.Extension
{
	public abstract class AbastractRedisSpider : IRedisSpider
	{
		private RedisScheduler _scheduler;

		protected RedisScheduler Scheduler
		{
			get
			{
				if (_scheduler == null)
				{
					var redis = ConnectionMultiplexer.Connect($"{RedisHost},password={RedisPassword}");
					_scheduler = new RedisScheduler(redis);
				}
				return _scheduler;
			}
		}

		protected bool IsInited { get; set; }

		private Core.Spider _spider;

		public void Run()
		{
			Prepare();

			_spider?.Run();
		}

		private void Prepare()
		{
			IDatabase db = Scheduler.Redis.GetDatabase(0);
			string key = "locker-" + Name;
			try
			{

				// 取得锁
				Console.WriteLine("Lock: " + key);
				db.LockExtend(key, 0, TimeSpan.FromMinutes(10));

				var lockerValue = db.StringGet(Name);
				bool needInitStartRequest = lockerValue != "init finished";

				Console.WriteLine("Prepare site with paramete: " + needInitStartRequest);

				if (needInitStartRequest)
				{
					PrepareSite();
				}

				Console.WriteLine("Init spider with site.");
				_spider = InitSpider(Site);
				_spider.SaveStatusToRedis = true;
				SpiderMonitor.Instance.Register(_spider);
				_spider.InitComponent();

				if (needInitStartRequest)
				{
					db.StringSet(Name, "init finished");
				}
			}
			catch (Exception e)
			{
				//测试是否操时
			}
			finally
			{
				Console.WriteLine("Release lock.");
				db.LockRelease(key, 0);
			}

		}

		protected abstract void PrepareSite();

		protected abstract Site Site { get; }

		protected abstract Core.Spider InitSpider(Site site);
		public virtual string RedisHost { get; } = "localhost";
		public virtual string RedisPassword { get; } = null;
		public abstract string Name { get; }
	}
}
