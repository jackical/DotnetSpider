using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.Scheduler;
using Java2Dotnet.Spider.Extension.Utils;
using log4net;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Java2Dotnet.Spider.Extension.Monitor
{
	public class SpiderMonitor
	{
		private static SpiderMonitor _instanse;
		private static readonly object Locker = new object();
		private readonly Dictionary<ISpider, MonitorSpiderListener> _data = new Dictionary<ISpider, MonitorSpiderListener>();

		private SpiderMonitor()
		{
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public SpiderMonitor Register(params Core.Spider[] spiders)
		{
			foreach (Core.Spider spider in spiders)
			{
				if (!_data.ContainsKey(spider))
				{
					MonitorSpiderListener monitorSpiderListener = new MonitorSpiderListener(spider);
					spider.RequestFailedEvent += monitorSpiderListener.OnError;
					spider.RequestSuccessedEvent += monitorSpiderListener.OnSuccess;
					spider.SpiderClosingEvent += monitorSpiderListener.OnClose;
					_data.Add(spider, monitorSpiderListener);
					if (spider.ShowControl)
					{
						Form1 form1 = new Form1(monitorSpiderListener);
						form1.ShowDialog();
					}
				}
			}
			return this;
		}

		public static SpiderMonitor Default
		{
			get
			{
				lock (Locker)
				{
					return _instanse ?? (_instanse = new SpiderMonitor());
				}
			}
		}

		public class MonitorSpiderListener : ISpiderStatus
		{
			protected static readonly ILog Logger = LogManager.GetLogger(typeof(MonitorSpiderListener));

			private readonly AutomicLong _successCount = new AutomicLong(0);
			private readonly AutomicLong _errorCount = new AutomicLong(0);
			private readonly List<string> _errorUrls = new List<string>();
			private readonly Core.Spider _spider;

			public MonitorSpiderListener(Core.Spider spider)
			{
				_spider = spider;

				if (spider.SaveStatusToRedis)
				{
					Task.Factory.StartNew(() =>
					{
						ConnectionMultiplexer redis = RedisProvider.GetProvider();
						IDatabase db = redis.GetDatabase(0);

						while (true)
						{
							try
							{
								if (Closed)
								{
									UpdateStatus(db);
									break;
								}

								UpdateStatus(db);
							}
							catch (Exception)
							{
								// ignored
							}

							Thread.Sleep(3000);
						}
						redis.Close();
					});
				}
			}

			private void UpdateStatus(IDatabase db)
			{
				var status = new
				{
					Name,
					ErrorPageCount,
					LeftPageCount,
					PagePerSecond,
					StartTime,
					EndTime,
					Status,
					SuccessPageCount,
					ThreadCount,
					TotalPageCount,
					AliveThreadCount
				};
				db.HashSet(RedisScheduler.TaskStatus, _spider.Identify, JsonConvert.SerializeObject(status));
			}

			public void OnSuccess(Request request)
			{
				_successCount.Inc();
			}

			public void OnError(Request request)
			{
				_errorUrls.Add(request.Url.ToString());
				_errorCount.Inc();
			}

			public void OnClose()
			{
				Closed = true;
			}

			public long SuccessPageCount => _successCount.Value;

			public long ErrorPageCount => _errorCount.Value;

			public List<string> ErrorPages => _errorUrls;

			public bool Closed { get; set; }

			public string Name => _spider.Identify;

			public long LeftPageCount
			{
				get
				{
					IMonitorableScheduler scheduler = _spider.Scheduler as IMonitorableScheduler;
					if (scheduler != null)
					{
						return scheduler.GetLeftRequestsCount(_spider);
					}
					Logger.Warn("Get leftPageCount fail, try to use a Scheduler implement MonitorableScheduler for monitor count!");
					return -1;
				}
			}

			public long TotalPageCount
			{
				get
				{
					IMonitorableScheduler scheduler = _spider.Scheduler as IMonitorableScheduler;
					if (scheduler != null)
					{
						return scheduler.GetTotalRequestsCount(_spider);
					}
					Logger.Warn("Get totalPageCount fail, try to use a Scheduler implement MonitorableScheduler for monitor count!");
					return -1;
				}
			}

			public string Status => _spider.StatusCode.ToString();

			public int AliveThreadCount => _spider.ThreadAliveCount;

			public int ThreadCount => _spider.ThreadNum;

			public void Start()
			{
				_spider.Run();
			}

			public void Stop()
			{
				_spider.Stop();
			}

			public DateTime StartTime => _spider.StartTime;

			public DateTime EndTime => _spider.FinishedTime == DateTime.MinValue ? DateTime.Now : _spider.FinishedTime;

			public double PagePerSecond
			{
				get
				{
					double runSeconds = (EndTime - StartTime).TotalSeconds;
					if (runSeconds > 0)
					{
						return SuccessPageCount / runSeconds;
					}
					return 0;
				}
			}

			public Core.Spider Spider => _spider;
		}
	}
}