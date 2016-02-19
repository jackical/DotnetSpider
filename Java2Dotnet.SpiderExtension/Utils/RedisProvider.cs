using Java2Dotnet.Spider.Core;
using StackExchange.Redis;
using System;
using System.Configuration;

namespace Java2Dotnet.Spider.Extension.Utils
{
	public class RedisProvider
	{
		public static ConnectionMultiplexer GetProvider()
		{
			try
			{
				string host = ConfigurationManager.AppSettings["redisServer"];
				var password = ConfigurationManager.AppSettings["redisPassword"];
				int port = string.IsNullOrEmpty(ConfigurationManager.AppSettings["redisPort"]) ? 6379 : int.Parse(ConfigurationManager.AppSettings["redisPort"]);

				return ConnectionMultiplexer.Connect(new ConfigurationOptions()
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
			catch (Exception e)
			{
				throw new SpiderExceptoin("Can't init redis provider: " + e);
			}
		}
	}
}
