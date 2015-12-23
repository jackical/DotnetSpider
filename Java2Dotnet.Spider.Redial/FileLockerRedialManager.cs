using System;
using System.IO;
using System.Threading;

namespace Java2Dotnet.Spider.Redial
{
	/// <summary>
	/// 用于单台电脑
	/// </summary>
	public class FileLockerRedialManager : BaseRedialManager
	{
		private readonly string _lockerFilePath;
		private static FileLockerRedialManager _instanse;

		public static FileLockerRedialManager Default
		{
			get
			{
				if (_instanse == null)
				{
					_instanse = new FileLockerRedialManager();
				}
				return _instanse;
			}
		}

		private FileLockerRedialManager()
		{
			var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DotnetSpdier");

			DirectoryInfo di = new DirectoryInfo(folder);
			if (!di.Exists)
			{
				di.Create();
			}
			_lockerFilePath = Path.Combine(folder, "redialer.lock");
		}

		public override void WaitforRedialFinish()
		{
			if (Skip)
			{
				return;
			}

			if (File.Exists(_lockerFilePath))
			{
				while (true)
				{
					Thread.Sleep(50);
					if (!File.Exists(_lockerFilePath))
					{
						break;
					}
				}
			}
		}

		public override RedialResult Redial()
		{
			if (Skip)
			{
				return RedialResult.Skip;
			}

			if (File.Exists(_lockerFilePath))
			{
				while (true)
				{
					Thread.Sleep(50);
					if (!File.Exists(_lockerFilePath))
					{
						return RedialResult.OtherRedialed;
					}
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

					Logger.Warn("Wait atomic action to finish...");

					// 等待数据库等操作完成
					WaitAtomicAction();

					Logger.Warn("Try to redial network...");

					RedialInternet();

					Logger.Warn("Redial finished.");
					return RedialResult.Sucess;
				}
				catch (Exception)
				{
					return RedialResult.Failed;
				}
				finally
				{
					stream?.Close();
					File.Delete(_lockerFilePath);
				}
			}
		}
	}
}
