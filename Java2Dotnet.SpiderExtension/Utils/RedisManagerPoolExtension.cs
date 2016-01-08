using System;
using System.Threading;
using ServiceStack.Redis;

namespace Java2Dotnet.Spider.Extension.Utils
{
	public static class RedisManagerPoolExtension
	{
		public static IRedisClient GetSafeGetClient(this RedisManagerPool pool)
		{
			while (true)
			{
				try
				{
					return pool.GetClient();
				}
				catch (Exception e)
				{
					Console.WriteLine("Error: Get redis client failed.");
					Thread.Sleep(500);
				}
			}
		}
	}
}
