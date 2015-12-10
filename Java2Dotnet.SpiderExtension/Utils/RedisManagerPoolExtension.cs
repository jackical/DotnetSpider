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
				catch (Exception)
				{
					Thread.Sleep(500);
				}
			}
		}
	}
}
