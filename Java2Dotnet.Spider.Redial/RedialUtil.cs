using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core.Redial;
using ServiceStack.Redis;

namespace Java2Dotnet.Spider.Redial
{
	internal enum RedialStatus
	{
		Dialing,
		Running
	}

	public class RedialUtil : IRedialer, IDisposable
	{
		/// <summary>
		/// 代码还在测试时大部分在Office里是不能也没有必要拨号（企业网）
		/// 可以测试这个值以跳过拨号以免生产环境中拨号引起调试阻塞
		/// </summary>
		public static bool Skip { get; set; }

		private readonly IRedisClient _redisClient;
		private const string RedialStatusKey = "REDIAL_STATUS";
		public static string RedisHost = "localhost";
		private readonly string _interface;
		private readonly string _user;
		private readonly string _password;

		public RedialUtil()
		{
			try
			{
				var redisHost = ConfigurationManager.ConnectionStrings["redisServer"];
				if (redisHost != null)
				{
					RedisHost = redisHost.ConnectionString;
				}

				_interface = ConfigurationManager.AppSettings["redialInterface"];
				_user = ConfigurationManager.AppSettings["redialUser"];
				_password = ConfigurationManager.AppSettings["redialPassword"];
				var pool = new RedisManagerPool(new List<string> { RedisHost }, new RedisPoolConfig { MaxPoolSize = 100 });
				_redisClient = pool.GetClient();
			}
			catch
			{
				throw new RedialException("ConectString: redisServer is not exist.");
			}

			var redialSetting = _redisClient.GetValue(RedialStatusKey);
			if (redialSetting == null)
			{
				redialSetting = RedialStatus.Running.ToString();
				_redisClient.SetValue(RedialStatusKey, redialSetting);
			}
		}

		public void WaitforRedialFinish()
		{
			if (Skip)
			{
				return;
			}

			var redialSetting = _redisClient.GetValue(RedialStatusKey);
			RedialStatus status = (RedialStatus)Enum.Parse(typeof(RedialStatus), redialSetting);
			switch (status)
			{
				case RedialStatus.Dialing:
					{
						while (true)
						{
							Thread.Sleep(500);
							var redialSetting1 = _redisClient.GetValue(RedialStatusKey);
							if (redialSetting1 == RedialStatus.Running.ToString())
							{
								break;
							}
						}
						break;
					}
				case RedialStatus.Running:
					{
						return;
					}
			}
		}

		public void Redial()
		{
			if (Skip)
			{
				return;
			}

			var redialSetting = _redisClient.GetValue(RedialStatusKey);
			RedialStatus status = (RedialStatus)Enum.Parse(typeof(RedialStatus), redialSetting);
			switch (status)
			{
				case RedialStatus.Dialing:
					{
						while (true)
						{
							Thread.Sleep(500);
							var redialSetting1 = _redisClient.GetValue(RedialStatusKey);
							if (redialSetting1 == RedialStatus.Running.ToString())
							{
								break;
							}
						}
						break;
					}
				case RedialStatus.Running:
					{
						redialSetting = RedialStatus.Dialing.ToString();
						_redisClient.SetValue(RedialStatusKey, redialSetting);

						RedialInternet();

						redialSetting = RedialStatus.Running.ToString();
						_redisClient.SetValue(RedialStatusKey, redialSetting);
						break;
					}
			}
		}

		private void RedialInternet()
		{
			AdslUtil.Connect(_interface, _user, _password);

			Thread.Sleep(1500);
			WaitForConnection();
		}

		private static void WaitForConnection()
		{
			while (true)
			{
				try
				{
					Ping p = new Ping();//创建Ping对象p
					PingReply pr = p.Send("www.baidu.com", 30000);//向指定IP或者主机名的计算机发送ICMP协议的ping数据包

					if (pr.Status == IPStatus.Success)//如果ping成功
					{
						return;
					}
					Thread.Sleep(500);
				}
				catch
				{
				}
			}
		}

		public void Dispose()
		{
			_redisClient?.Dispose();
		}
	}
}
