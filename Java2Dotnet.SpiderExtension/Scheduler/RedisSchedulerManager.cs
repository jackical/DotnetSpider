using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Java2Dotnet.Spider.Extension.Scheduler
{
	public class RedisSchedulerManager : ISchedulerManager
	{
		private ConnectionMultiplexer redis;

		public RedisSchedulerManager(string host, string password)
		{
			redis = ConnectionMultiplexer.Connect(new ConfigurationOptions()
			{
				ServiceName = host,
				Password = password,
				ConnectTimeout = 5000,
				KeepAlive = 8
			});
		}

		public IDictionary<string, double> GetTaskList(int startIndex, int count)
		{
			//IDatabase db = redis.GetDatabase(0);
			//Dictionary<string, double> tmp = new Dictionary<string, double>();
			// foreach (var entry in db.SortedSetRangeByScore(RedisScheduler.TaskList, startIndex, startIndex + count))
			//{
			//	tmp.Add(entry.k)
			//}
			return null;
		}

		public void RemoveTask(string taskIdentify)
		{
			//using (var redis = _pool.GetSafeGetClient())
			//{
			//	//string json = redis?.GetValueFromHash(RedisScheduler.TaskStatus, taskIdentify);
			//	//if (!string.IsNullOrEmpty(json))
			//	{
			//		redis.Remove(GetQueueKey(taskIdentify));
			//		redis.Remove(GetSetKey(taskIdentify));
			//		redis.RemoveEntryFromHash(RedisScheduler.TaskStatus, taskIdentify);
			//		redis.Remove(RedisScheduler.ItemPrefix + taskIdentify);
			//		redis.Remove(taskIdentify);
			//		redis.RemoveItemFromSortedSet(RedisScheduler.TaskList, taskIdentify);
			//	}
			//}
		}

		private string GetSetKey(string identify)
		{
			return RedisScheduler.SetPrefix + Encrypt.Md5Encrypt(identify);
		}

		private string GetQueueKey(string identify)
		{
			return RedisScheduler.QueuePrefix + Encrypt.Md5Encrypt(identify);
		}

		public SpiderStatus GetTaskStatus(string taskIdentify)
		{
			//using (var redis = _pool.GetSafeGetClient())
			//{
			//	string json = redis?.GetValueFromHash(RedisScheduler.TaskStatus, taskIdentify);
			//	if (!string.IsNullOrEmpty(json))
			//	{
			//		return JsonConvert.DeserializeObject<SpiderStatus>(json);
			//	}
			//}
			return new SpiderStatus();
		}

		public void ClearDb()
		{
			//using (var redis = _pool.GetSafeGetClient())
			//{
			//	redis.FlushDb();
			//}
		}
	}
}
