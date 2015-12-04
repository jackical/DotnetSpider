using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Java2Dotnet.Spider.Redial
{
	/// <summary>
	/// VPS 有多根线路, 其中几根是用于稳定远程, 另几根是IP拨号, 所以不能用PING baidu.com这种形式判断是否拨号成功.
	/// </summary>
	public class VpsNetworkValidater : INetworkValidater
	{
		private readonly int _networkCount;

		public VpsNetworkValidater(int networkCount = 2)
		{
			_networkCount = networkCount;
		}

		public void Wait()
		{
			while (true)
			{
				try
				{
					if (GetIp4Count() == _networkCount)
					{
						break;
					}

					Thread.Sleep(100);
				}
				catch
				{
				}
			}
		}

		public static int GetIp4Count()
		{
			string hostName = Dns.GetHostName();
			IPAddress[] addressList = Dns.GetHostAddresses(hostName);
			return addressList.Count(i => i.AddressFamily == AddressFamily.InterNetwork);
		}
	}
}
