using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Redial;
using log4net;
using ServiceStack.Redis;

namespace Java2Dotnet.Spider.Redial
{
	internal enum RedialStatus
	{
		Dialing,
		Running
	}

	public class Redialer : IRedialer, IDisposable
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(Redialer));

		/// <summary>
		/// 代码还在测试时大部分在Office里是不能也没有必要拨号（企业网）
		/// 可以测试这个值以跳过拨号以免生产环境中拨号引起调试阻塞
		/// </summary>
		private bool Skip { get; set; }

		private static Redialer _instanse;
		private readonly RedisManagerPool _pool;
		private const string RedialStatusKey = "REDIAL_STATUS";
		public static string RedisHost = "localhost";
		private readonly string _interface;
		private readonly string _user;
		private readonly string _password;

		private Redialer()
		{
			try
			{
				var tmpRedisHost = ConfigurationManager.AppSettings["redialRedisServer"];
				if (!string.IsNullOrEmpty(tmpRedisHost))
				{
					RedisHost = tmpRedisHost;
				}
				var interface1 = ConfigurationManager.AppSettings["redialInterface"];
				_interface = string.IsNullOrEmpty(interface1) ? "宽带连接" : interface1;

				_user = ConfigurationManager.AppSettings["redialUser"];
				_password = ConfigurationManager.AppSettings["redialPassword"];
				_pool = new RedisManagerPool(new List<string> { RedisHost }, new RedisPoolConfig { MaxPoolSize = 100 });
			}
			catch (Exception e)
			{
				throw new RedialException("Redial init failed.", e);
			}


			var redialSetting = GetRedialStatus();
			if (redialSetting == null)
			{
				redialSetting = RedialStatus.Running.ToString();
				SetRedialStatus(redialSetting);
			}

		}

		public static Redialer Default
		{
			get
			{
				if (_instanse == null)
				{
					_instanse = new Redialer();
				}
				return _instanse;
			}
		}

		public void WaitforRedialFinish()
		{
			if (Skip)
			{
				return;
			}

			var redialSetting = GetRedialStatus();
			RedialStatus status = (RedialStatus)Enum.Parse(typeof(RedialStatus), redialSetting);
			switch (status)
			{
				case RedialStatus.Dialing:
					{
						while (true)
						{
							Thread.Sleep(500);
							var redialSetting1 = GetRedialStatus();
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

		private string GetRedialStatus()
		{
			using (var redisClient = _pool.GetClient())
			{
				return redisClient.GetValue(RedialStatusKey);
			}
		}

		private void SetRedialStatus(string value)
		{
			using (var redisClient = _pool.GetClient())
			{
				redisClient.SetValue(RedialStatusKey, value);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Redial()
		{
			if (Skip)
			{
				return;
			}
			Logger.Warn("Try to redial network...");
			var redialSetting = GetRedialStatus();
			RedialStatus status = (RedialStatus)Enum.Parse(typeof(RedialStatus), redialSetting);
			switch (status)
			{
				case RedialStatus.Dialing:
					{
						while (true)
						{
							Thread.Sleep(500);
							var redialSetting1 = GetRedialStatus();
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
						SetRedialStatus(redialSetting);

						RedialInternet();

						redialSetting = RedialStatus.Running.ToString();
						SetRedialStatus(redialSetting);
						break;
					}
			}
			Logger.Warn("Redial finished.");
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
			_pool?.Dispose();
		}
	}
}
