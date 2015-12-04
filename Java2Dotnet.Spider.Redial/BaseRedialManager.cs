﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using Java2Dotnet.Spider.Lib.Redial;
using log4net;

namespace Java2Dotnet.Spider.Redial
{
	public abstract class BaseRedialManager : IRedialManager
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(BaseRedialManager));
		private static readonly string AtomicActionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DotnetSpdier", "AtomicAction");

		public abstract void WaitforRedialFinish();
		public abstract RedialResult Redial();

		public INetworkValidater NetworkValidater { get; set; } = new DefalutNetworkValidater();
		public IRedialer Redialer { get; set; }=new AdslRedialer();

		/// <summary>
		/// 代码还在测试时大部分在Office里是不能也没有必要拨号（企业网）
		/// 可以测试这个值以跳过拨号以免生产环境中拨号引起调试阻塞
		/// </summary>
		public bool Skip { get; set; } = false;

		protected void RedialInternet()
		{
			Redialer?.Redial();

			NetworkValidater?.Wait();
		}

		protected void WaitAtomicAction()
		{
			// 等待数据库等操作完成
			while (true)
			{
				if (!Directory.GetFiles(AtomicActionFolder).Any())
				{
					break;
				}

				Thread.Sleep(50);
			}
		}
	}
}
