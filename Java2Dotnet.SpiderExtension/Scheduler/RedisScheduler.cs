using System.Threading;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Scheduler.Component;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Lib;
using Java2Dotnet.Spider.Redial;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Java2Dotnet.Spider.Extension.Scheduler
{
	/// <summary>
	/// Use Redis as url scheduler for distributed crawlers.
	/// </summary>
	public class RedisScheduler : DuplicateRemovedScheduler, IMonitorableScheduler, IDuplicateRemover
	{
		public static readonly string QueuePrefix = "queue-";
		public static readonly string TaskStatus = "task-status";
		public static readonly string SetPrefix = "set-";
		public static readonly string TaskList = "task";
		public static readonly string ItemPrefix = "item-";
		private ConnectionMultiplexer Redis { get; }
		private readonly IDatabase _db;

		public RedisScheduler(string host, string password = null, int port = 6379) : this(ConnectionMultiplexer.Connect(new ConfigurationOptions()
		{
			ServiceName = host,
			Password = password,
			ConnectTimeout = 5000,
			KeepAlive = 8,
			EndPoints =
				{
					{ host, port }
				}
		}))
		{
		}

		public RedisScheduler(ConnectionMultiplexer redis) : this()
		{
			Redis = redis;
			_db = Redis.GetDatabase(0);
		}

		private RedisScheduler()
		{
			DuplicateRemover = this;
		}

		public override void Init(ISpider spider)
		{
			RedialManagerUtils.Execute("rds-init", () =>
			{
				// redis.AddItemToSortedSet(TaskList, spider.Identify, DateTimeUtil.GetCurrentTimeStamp());
				_db.SortedSetAdd(TaskList, spider.Identity, DateTimeUtil.GetCurrentTimeStamp());
			});
		}

		public void ResetDuplicateCheck(ISpider spider)
		{
			RedialManagerUtils.Execute("rds-reset", () =>
			{
				_db.KeyDelete(GetSetKey(spider));
			});
		}

		private string GetSetKey(ISpider spider)
		{
			return SetPrefix + Encrypt.Md5Encrypt(spider.Identity);
		}

		private string GetQueueKey(ISpider spider)
		{
			return QueuePrefix + Encrypt.Md5Encrypt(spider.Identity);
		}

		public bool IsDuplicate(Request request, ISpider spider)
		{
			return SafeExecutor.Execute(30, () =>
			{
				bool isDuplicate = _db.SetContains(GetSetKey(spider), request.Identity);
				if (!isDuplicate)
				{
					_db.SetAdd(GetSetKey(spider), request.Identity);
				}
				return isDuplicate;
			});
		}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		protected override void PushWhenNoDuplicate(Request request, ISpider spider)
		{
			SafeExecutor.Execute(30, () =>
			{
				_db.SetAdd(GetQueueKey(spider), request.Identity);

				// 没有必要判断浪费性能了, 这里不可能为空。最少会有一个层级数据 Grade
				//if (request.Extras != null && request.Extras.Count > 0)
				//{
				string field = request.Identity;
				string value = JsonConvert.SerializeObject(request);

				_db.HashSet(ItemPrefix + spider.Identity, field, value);
			});
		}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		public override Request Poll(ISpider spider)
		{
			return RedialManagerUtils.Execute("rds-poll", () =>
			{
				return DoPoll(spider);
			});
		}

		public int GetLeftRequestsCount(ISpider spider)
		{
			return RedialManagerUtils.Execute("rds-getleftcount", () =>
			{
				long size = _db.SetLength(GetQueueKey(spider));
				return (int)size;
			});
		}

		public int GetTotalRequestsCount(ISpider spider)
		{
			return RedialManagerUtils.Execute("rds-gettotalcount", () =>
			{
				long size = _db.SetLength(GetSetKey(spider));

				return (int)size;
			});
		}

		public override void Dispose()
		{
			Redis?.Dispose();
		}

		private Request DoPoll(ISpider spider)
		{
			return SafeExecutor.Execute(30, () =>
			{
				//string url = redis.PopItemWithLowestScoreFromSortedSet(GetQueueKey(spider));

				var value = _db.SetPop(GetQueueKey(spider));
				if (!value.HasValue)
				{
					return null;
				}
				string field = value.ToString();
				string hashId = ItemPrefix + spider.Identity;

				string json = null;

				//redis 有可能取数据失败
				for (int i = 0; i < 10 && string.IsNullOrEmpty(json = _db.HashGet(hashId, field)); ++i)
				{
					Thread.Sleep(150);
				}

				if (!string.IsNullOrEmpty(json))
				{
					var result = JsonConvert.DeserializeObject<Request>(json);
					_db.HashDelete(hashId, field);
					return result;
				}

				// 严格意义上说不会走到这里, 一定会有JSON数据,详情看Push方法
				// 是否应该设为1级？

				return null;
			});
		}
	}
}