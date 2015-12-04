using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Java2Dotnet.Spider.Lib
{
	public class AtomicExecutor
	{
		private static readonly string AtomicActionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DotnetSpdier", "AtomicAction");
		[DllImport("kernel32.dll")]
		public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
		public const int OfReadwrite = 2;
		public const int OfShareDenyNone = 0x40;
		public static readonly IntPtr HfileError = new IntPtr(-1);

		static AtomicExecutor()
		{
			var di = new DirectoryInfo(AtomicActionFolder);
			if (!di.Exists)
			{
				di.Create();
			}
			Task.Factory.StartNew(() =>
			{
				// 用于删除异常关机残留的文件. 在使用的都是被锁住的, 无法删除
				while (true)
				{
					foreach (var action in Directory.GetFiles(AtomicActionFolder))
					{
						//IntPtr vHandle = _lopen(action, OfReadwrite | OfShareDenyNone);
						//if (vHandle != HfileError)
						//{
						try
						{
							File.Delete(action);
						}
						catch (Exception)
						{
							// ignored
						}

						//}
					}
					Thread.Sleep(1000);
				}
			});
		}

		public static void Execute(string name, Action action)
		{
			Stream stream = null;
			string id = Path.Combine(AtomicActionFolder, name + "-" + Guid.NewGuid());
			try
			{
				stream = File.Open(id, FileMode.Create, FileAccess.Write);

				action();
			}
			finally
			{
				stream?.Close();
				File.Delete(id);
			}
		}

		public static T Execute<T>(string name, Func<T> func)
		{
			Stream stream = null;
			string id = Path.Combine(AtomicActionFolder, name + Guid.NewGuid().ToString());
			try
			{
				stream = File.Open(id, FileMode.Create, FileAccess.Write);

				return func();
			}
			finally
			{
				stream?.Close();
				File.Delete(id);
			}
		}
	}
}
