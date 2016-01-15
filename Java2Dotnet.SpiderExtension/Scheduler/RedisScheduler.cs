using System.Collections.Generic;
using System.Threading;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Core.Scheduler.Component;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Lib;
using Java2Dotnet.Spider.Redial;
using Newtonsoft.Json;
using ServiceStack.Redis;
using SafeRedisManagerPool = Java2Dotnet.Spider.Extension.Utils.SafeRedisManagerPool;

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

		public RedisScheduler(string host, string password)
			: this(new SafeRedisManagerPool(new List<string> { host }, new RedisPoolConfig { MaxPoolSize = 100 }, password))
		{
			SafeRedisManagerPool.SetConfig(host, password);
		}

		public override void Init(ISpider spider)
		{
			RedialManagerConfig.RedialManager.AtomicExecutor.Execute("rds-init", () =>
			{
				using (var redis = _pool.GetSafeGetClient())
				{
					redis.AddItemToSortedSet(TaskList, spider.Identify, DateTimeUtil.GetCurrentTimeStamp());
				}
			});
		}

		private RedisScheduler()
		{
			DuplicateRemover = this;
		}

		public void ResetDuplicateCheck(ISpider spider)
		{
			RedialManagerConfig.RedialManager.AtomicExecutor.Execute("rds-reset", () =>
			{
				using (var redis = _pool.GetSafeGetClient())
				{
					redis.Remove(GetSetKey(spider));
				}
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
				using (var redis = _pool.GetSafeGetClient())
				{
					bool isDuplicate = redis.SetContainsItem(GetSetKey(spider), request.Url.ToString());
					if (!isDuplicate)
					{
						redis.AddItemToSet(GetSetKey(spider), request.Url.ToString());
					}
					return isDuplicate;
				}
			});
		}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		protected override void PushWhenNoDuplicate(Request request, ISpider spider)
		{
			SafeExecutor.Execute(30, () =>
			{
				using (var redis = _pool.GetSafeGetClient())
				{
					redis.AddItemToSortedSet(GetQueueKey(spider), request.Url.ToString());

					// û�б�Ҫ�ж��˷�������, ���ﲻ����Ϊ�ա����ٻ���һ���㼶���� Grade
					//if (request.Extras != null && request.Extras.Count > 0)
					//{
					string field = Encrypt.Md5Encrypt(request.Url.ToString());
					string value = JsonConvert.SerializeObject(request);

					redis.SetEntryInHash(ItemPrefix + spider.Identify, field, value);

					var value1 = redis.GetValueFromHash(ItemPrefix + spider.Identify, field);

					// ��֤�����Ƿ����ɹ�
					for (int i = 0; i < 10 && value1 != value; ++i)
					{
						redis.SetEntryInHash(ItemPrefix + spider.Identify, field, value);
						value1 = redis.GetValueFromHash(ItemPrefix + spider.Identify, field);
						Thread.Sleep(150);
					}
					//}
				}
			});
		}

		//[MethodImpl(MethodImplOptions.Synchronized)]
		public override Request Poll(ISpider spider)
		{
			return RedialManagerConfig.RedialManager.AtomicExecutor.Execute("rds-poll", () =>
			{
				return SafeExecutor.Execute(30, () =>
				{
					using (var redis = _pool.GetSafeGetClient())
					{
						string url = redis.PopItemWithLowestScoreFromSortedSet(GetQueueKey(spider));
						if (url == null)
						{
							return null;
						}

						string hashId = ItemPrefix + spider.Identify;
						string field = Encrypt.Md5Encrypt(url);

						string json = null;

						//redis �п���ȡ����ʧ��
						for (int i = 0; i < 10 && string.IsNullOrEmpty(json = redis.GetValueFromHash(hashId, field)); ++i)
						{
							Thread.Sleep(150);
						}

						if (!string.IsNullOrEmpty(json))
						{
							return JsonConvert.DeserializeObject<Request>(json);
						}

						// �ϸ�������˵�����ߵ�����, һ������JSON����,���鿴Push����
						// �Ƿ�Ӧ����Ϊ1����
						Request request = new Request(url, 1, null);
						return request;
					}
				});
			});
		}

		public int GetLeftRequestsCount(ISpider spider)
		{
			return RedialManagerConfig.RedialManager.AtomicExecutor.Execute("rds-getleftcount", () =>
			{
				using (var redis = _pool.GetSafeGetClient())
				{
					long size = redis.GetSortedSetCount(GetQueueKey(spider));
					return (int)size;
				}
			});
		}

		public int GetTotalRequestsCount(ISpider spider)
		{
			return RedialManagerConfig.RedialManager.AtomicExecutor.Execute("rds-gettotalcount", () =>
			{
				using (var redis = _pool.GetSafeGetClient())
				{
					long size = redis.GetSetCount(GetSetKey(spider));

					return (int)size;
				}
			});
		}
	}
}