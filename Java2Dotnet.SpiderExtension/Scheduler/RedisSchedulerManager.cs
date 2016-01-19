using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.Utils;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Java2Dotnet.Spider.Extension.Scheduler
{
	public class RedisSchedulerManager : ISchedulerManager
	{
		private readonly ConnectionMultiplexer _redis;

		public RedisSchedulerManager(string host, string password = null, int port = 6379)
		{
			_redis = ConnectionMultiplexer.Connect(new ConfigurationOptions()
			{
				ServiceName = host,
				Password = password,
				ConnectTimeout = 5000,
				KeepAlive = 8,
				EndPoints =
				{
					{ host, port }
				}
			});
		}

		public RedisSchedulerManager()
		{
			_redis = RedisProvider.GetProvider();
		}

		public IDictionary<string, double> GetTaskList(int startIndex, int count)
		{
			IDatabase db = _redis.GetDatabase(0);
			Dictionary<string, double> tmp = new Dictionary<string, double>();
			foreach (var entry in db.SortedSetRangeByRank(RedisScheduler.TaskList, startIndex, startIndex + count))
			{
				tmp.Add(entry.ToString(), 0d);
			}
			return tmp;
		}

		public void RemoveTask(string taskIdentify)
		{
			IDatabase db = _redis.GetDatabase(0);

			db.KeyDelete(GetQueueKey(taskIdentify));
			db.KeyDelete(GetSetKey(taskIdentify));
			db.HashDelete(RedisScheduler.TaskStatus, taskIdentify);
			db.KeyDelete(RedisScheduler.ItemPrefix + taskIdentify);
			db.KeyDelete(taskIdentify);
			db.SortedSetRemove(RedisScheduler.TaskList, taskIdentify);
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
			IDatabase db = _redis.GetDatabase(0);
			string json = db.HashGet(RedisScheduler.TaskStatus, taskIdentify);
			if (!string.IsNullOrEmpty(json))
			{
				return JsonConvert.DeserializeObject<SpiderStatus>(json);
			}

			return new SpiderStatus();
		}

		public void ClearDb()
		{
			IServer server = _redis.GetServer(_redis.GetEndPoints()[0]);
			server.FlushDatabase();
		}
	}
}
