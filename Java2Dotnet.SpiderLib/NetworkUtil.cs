﻿using System.Net.NetworkInformation;
using System.Threading;

namespace Java2Dotnet.Spider.Lib
{
	public class NetworkUtil
	{
		public static void WaitForIntert()
		{
			while (true)
			{
				try
				{
					Ping p = new Ping();//创建Ping对象p
					PingReply pr = p.Send("www.baidu.com", 30000);//向指定IP或者主机名的计算机发送ICMP协议的ping数据包

					if (pr != null && pr.Status == IPStatus.Success)//如果ping成功
					{
						return;
					}
					Thread.Sleep(500);
				}
				catch
				{
					// ignored
				}
			}
		}
	}
}
