using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Java2Dotnet.Spider.Redial
{
	public static class AtomicRedialExecutor
	{
		public static IRedialManager RedialManager = FileLockerRedialManager.Default;

		private static readonly string AtomicActionFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DotnetSpdier", "AtomicAction");

		static AtomicRedialExecutor()
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
				// ReSharper disable once FunctionNeverReturns
			});
		}

		public static void Execute(string name, Action action)
		{
			RedialManager.WaitforRedialFinish();
			Stream stream = null;
			string id = Path.Combine(AtomicActionFolder, name + "-" + Guid.NewGuid().ToString("N"));
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

		public static void Execute(string name, Action<object> action, object obj)
		{
			RedialManager.WaitforRedialFinish();
			Stream stream = null;
			string id = Path.Combine(AtomicActionFolder, name + "-" + Guid.NewGuid().ToString("N"));
			try
			{
				stream = File.Open(id, FileMode.Create, FileAccess.Write);

				action(obj);
			}
			finally
			{
				stream?.Close();
				File.Delete(id);
			}
		}

		public static T Execute<T>(string name, Func<object, T> func, object obj)
		{
			RedialManager.WaitforRedialFinish();
			Stream stream = null;
			string id = Path.Combine(AtomicActionFolder, name + "-" + Guid.NewGuid().ToString("N"));
			try
			{
				stream = File.Open(id, FileMode.Create, FileAccess.Write);

				return func(obj);
			}
			finally
			{
				stream?.Close();
				File.Delete(id);
			}
		}

		public static T Execute<T>(string name, Func<T> func)
		{
			RedialManager.WaitforRedialFinish();
			Stream stream = null;
			string id = Path.Combine(AtomicActionFolder, name + "-" + Guid.NewGuid().ToString("N"));
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
