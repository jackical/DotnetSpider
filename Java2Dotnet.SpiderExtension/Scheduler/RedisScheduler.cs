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

		public ConnectionMultiplexer Redis { get; }

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
				_db.SortedSetAdd(TaskList, spider.Identity, DateTimeUtil.GetCurrentTimeStamp());
			});
		}

		public override void ResetDuplicateCheck(ISpider spider)
		{
			RedialManagerUtils.Execute("rds-reset", () =>
			{
				_db.KeyDelete(GetSetKey(spider.Identity));
			});
		}

		public static string GetSetKey(string identity)
		{
			return SetPrefix + Encrypt.Md5Encrypt(identity);
		}

		public static string GetQueueKey(string identity)
		{
			return QueuePrefix + Encrypt.Md5Encrypt(identity);
		}

		public bool IsDuplicate(Request request, ISpider spider)
		{
			return SafeExecutor.Execute(30, () =>
			{
				string key = GetSetKey(spider.Identity);
				bool isDuplicate = _db.SetContains(key, request.Identity);
				if (!isDuplicate)
				{
					_db.SetAdd(key, request.Identity);
				}
				return isDuplicate;
			});
		}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		protected override void PushWhenNoDuplicate(Request request, ISpider spider)
		{
			SafeExecutor.Execute(30, () =>
			{
				_db.ListRightPush(GetQueueKey(spider.Identity), request.Identity);
				string field = request.Identity;
				string value = JsonConvert.SerializeObject(request);

				_db.HashSet(ItemPrefix + spider.Identity, field, value);
			});
		}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		public override Request Poll(ISpider spider)
		{
			return RedialManagerUtils.Execute("rds-poll", () => DoPoll(spider));
		}

		public int GetLeftRequestsCount(ISpider spider)
		{
			return RedialManagerUtils.Execute("rds-getleftcount", () =>
			{
				long size = _db.ListLength(GetQueueKey(spider.Identity));
				return (int)size;
			});
		}

		public int GetTotalRequestsCount(ISpider spider)
		{
			return RedialManagerUtils.Execute("rds-gettotalcount", () =>
			{
				long size = _db.SetLength(GetSetKey(spider.Identity));

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
				var value = _db.ListRightPop(GetQueueKey(spider.Identity));
				if (!value.HasValue)
				{
					return null;
				}
				string field = value.ToString();
				string hashId = ItemPrefix + spider.Identity;

				string json = null;

				//redis �п���ȡ����ʧ��
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

				// �ϸ�������˵�����ߵ�����, һ������JSON����,���鿴Push����
				// �Ƿ�Ӧ����Ϊ1����

				return null;
			});
		}
	}
}