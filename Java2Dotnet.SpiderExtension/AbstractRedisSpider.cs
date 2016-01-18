using System;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Monitor;
using Java2Dotnet.Spider.Extension.Scheduler;
using StackExchange.Redis;
using Java2Dotnet.Spider.Redial;
using Java2Dotnet.Spider.Redial.RedialManager;
using Java2Dotnet.Spider.Extension.Utils;

namespace Java2Dotnet.Spider.Extension
{
	public abstract class AbstractRedisSpider : IRedisSpider
	{
		private RedisScheduler _scheduler;

		protected RedisScheduler Scheduler
		{
			get
			{
				if (_scheduler == null)
				{
					_scheduler = new RedisScheduler(RedisProvider.GetProvider());
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

		public abstract string Name { get; }
		public virtual AtomicType AtomicType { get; } = AtomicType.Zookeeper;
	}
}
