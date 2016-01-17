using Java2Dotnet.Spider.Extension.Scheduler;
using Java2Dotnet.Spider.Extension.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Java2Dotnet.Spider.Extension.Monitor
{
	public class RedisStatusUpdater : IDisposable
	{
		private readonly ISpiderStatus _spiderStatus;
		private readonly Core.Spider _spider;
		private ConnectionMultiplexer Redis { get; }

		public RedisStatusUpdater(Core.Spider spider, ISpiderStatus spiderStatus)
		{
			_spider = spider;
			_spiderStatus = spiderStatus;

			Redis = RedisProvider.GetProvider();
		}

		public void Run()
		{
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					try
					{
						UpdateStatus();
					}
					catch (Exception)
					{
						// ignored
					}

					Thread.Sleep(1000);
				}
				// ReSharper disable once FunctionNeverReturns
			});
		}

		public void UpdateStatus()
		{
			try
			{
				IDatabase db = Redis.GetDatabase(0);

				var status = new
				{
					_spiderStatus.Name,
					_spiderStatus.ErrorPageCount,
					_spiderStatus.LeftPageCount,
					_spiderStatus.PagePerSecond,
					_spiderStatus.StartTime,
					_spiderStatus.EndTime,
					_spiderStatus.Status,
					_spiderStatus.SuccessPageCount,
					_spiderStatus.ThreadCount,
					_spiderStatus.TotalPageCount,
					_spiderStatus.AliveThreadCount
				};
				db.HashSet(RedisScheduler.TaskStatus, _spider.Identify, JsonConvert.SerializeObject(status));
			}
			catch (Exception)
			{
				// ignored
			}
		}

		public void Dispose()
		{
			Redis?.Dispose();
		}
	}
}
