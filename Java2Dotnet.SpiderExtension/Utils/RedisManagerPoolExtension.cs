using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
