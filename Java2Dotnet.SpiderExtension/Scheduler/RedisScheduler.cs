using System.Threading;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Scheduler.Component;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Lib;
using Java2Dotnet.Spider.Redial;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;

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
		}

		public override void Init(ISpider spider)
		{
			RedialManagerUtils.Execute("rds-init", () =>
			{
				IDatabase db = Redis.GetDatabase(0);
				// redis.AddItemToSortedSet(TaskList, spider.Identify, DateTimeUtil.GetCurrentTimeStamp());
				db.SortedSetAdd(TaskList, spider.Identify, DateTimeUtil.GetCurrentTimeStamp());
			});
		}

		private RedisScheduler()
		{
			DuplicateRemover = this;
		}

		public void ResetDuplicateCheck(ISpider spider)
		{
			RedialManagerUtils.Execute("rds-reset", () =>
			{
				IDatabase db = Redis.GetDatabase(0);
				db.KeyDelete(GetSetKey(spider));
			});
		}

		private string GetSetKey(ISpider spider)
		{
			return SetPrefix + Encrypt.Md5Encrypt(spider.Identify);
		}

		private string GetQueueKey(ISpider spider)
		{
			return QueuePrefix + Encrypt.Md5Encrypt(spider.Identify);
		}

		public bool IsDuplicate(Request request, ISpider spider)
		{
			return SafeExecutor.Execute(30, () =>
			{
				IDatabase db = Redis.GetDatabase(0);
				bool isDuplicate = db.SetContains(GetSetKey(spider), request.Url.ToString());
				if (!isDuplicate)
				{
					db.SetAdd(GetSetKey(spider), request.Url.ToString());
				}
				return isDuplicate;
			});
		}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		protected override void PushWhenNoDuplicate(Request request, ISpider spider)
		{
			SafeExecutor.Execute(30, () =>
			{
				IDatabase db = Redis.GetDatabase(0);
				db.SetAdd(GetQueueKey(spider), request.Url.ToString());

				// 没有必要判断浪费性能了, 这里不可能为空。最少会有一个层级数据 Grade
				//if (request.Extras != null && request.Extras.Count > 0)
				//{
				string field = Encrypt.Md5Encrypt(request.Url.ToString());
				string value = JsonConvert.SerializeObject(request);

				db.HashSet(ItemPrefix + spider.Identify, field, value);
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
				IDatabase db = Redis.GetDatabase(0);
				long size = db.SetLength(GetQueueKey(spider));
				return (int)size;
			});
		}

		public int GetTotalRequestsCount(ISpider spider)
		{
			return RedialManagerUtils.Execute("rds-gettotalcount", () =>
			{
				IDatabase db = Redis.GetDatabase(0);
				long size = db.SetLength(GetSetKey(spider));

				return (int)size;
			});
		}

		public override void Dispose()
		{
			Redis?.Dispose();
		}

		private Request DoPoll(ISpider spider)
		{
			IDatabase db = Redis.GetDatabase(0);
			return SafeExecutor.Execute(30, () =>
			{
				//string url = redis.PopItemWithLowestScoreFromSortedSet(GetQueueKey(spider));

				var value = db.SetPop(GetQueueKey(spider));
				if (!value.HasValue)
				{
					return null;
				}
				string url = value.ToString();
				string hashId = ItemPrefix + spider.Identify;
				string field = Encrypt.Md5Encrypt(url);

				string json = null;

				//redis 有可能取数据失败
				for (int i = 0; i < 10 && string.IsNullOrEmpty(json = db.HashGet(hashId, field)); ++i)
				{
					Thread.Sleep(150);
				}

				if (!string.IsNullOrEmpty(json))
				{
					var result = JsonConvert.DeserializeObject<Request>(json);
					db.HashDelete(hashId, field);
					return result;
				}

				// 严格意义上说不会走到这里, 一定会有JSON数据,详情看Push方法
				// 是否应该设为1级？
				Request request = new Request(url, 1, null);
				return request;
			});
		}
	}
}