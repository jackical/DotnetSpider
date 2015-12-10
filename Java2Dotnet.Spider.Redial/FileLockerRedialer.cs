using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using Java2Dotnet.Spider.Core.Redial;
using log4net;

namespace Java2Dotnet.Spider.Redial
{
	/// <summary>
	/// 只用于单台电脑单个拨号并仅有自身爬虫
	/// </summary>
	public class FileLockerRedialer : IRedialer
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(FileLockerRedialer));

		/// <summary>
		/// 代码还在测试时大部分在Office里是不能也没有必要拨号（企业网）
		/// 可以测试这个值以跳过拨号以免生产环境中拨号引起调试阻塞
		/// </summary>
		private bool Skip { get; set; }

		private readonly string _lockerFilePath;
		private static FileLockerRedialer _instanse;
		private readonly string _interface;
		private readonly string _user;
		private readonly string _password;
		private static readonly string AtomicActionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DotnetSpdier", "AtomicAction");

		public INetworkValidater NetworkValidater { get; set; }

		public static FileLockerRedialer Default
		{
			get
			{
				if (_instanse == null)
				{
					_instanse = new FileLockerRedialer();
				}
				return _instanse;
			}
		}

		private FileLockerRedialer()
		{
			var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DotnetSpdier");

			DirectoryInfo di = new DirectoryInfo(folder);
			if (!di.Exists)
			{
				di.Create();
			}
			_lockerFilePath = Path.Combine(folder, "redialer.lock");
			var interface1 = ConfigurationManager.AppSettings["redialInterface"];
			_interface = string.IsNullOrEmpty(interface1) ? "宽带连接" : interface1;

			_user = ConfigurationManager.AppSettings["redialUser"];
			_password = ConfigurationManager.AppSettings["redialPassword"];
		}

		public void WaitforRedialFinish()
		{
			if (Skip)
			{
				return;
			}

			if (File.Exists(_lockerFilePath))
			{
				while (true)
				{
					if (!File.Exists(_lockerFilePath))
					{
						break;
					}
					Thread.Sleep(10);
				}
			}
		}

		public void Redial()
		{
			if (Skip)
			{
				return;
			}

			if (File.Exists(_lockerFilePath))
			{
				while (true)
				{
					if (!File.Exists(_lockerFilePath))
					{
						break;
					}

					Thread.Sleep(20);
				}
			}
			else
			{
				Stream stream = null;

				try
				{
					stream = File.Open(_lockerFilePath, FileMode.Create, FileAccess.Write);

					// wait all operation stop.
					Thread.Sleep(5000);
					Logger.Warn("Try to redial network...");

					// 等待数据库操作完成
					while (true)
					{
						if (!Directory.GetFiles(AtomicActionFolder).Any())
						{
							break;
						}

						Thread.Sleep(10);
					}

					RedialInternet();

					Logger.Warn("Redial finished.");
				}
				finally
				{
					stream?.Close();
					File.Delete(_lockerFilePath);
				}
			}
		}

		private void RedialInternet()
		{
			AdslUtil.Connect(_interface, _user, _password);

			Thread.Sleep(2000);

			NetworkValidater?.Wait();
		}

		private class DefalutNetworkValidater : INetworkValidater
		{
			public void Wait()
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
						Thread.Sleep(100);
					}
					catch
					{
					}
				}
			}
		}
	}
}
