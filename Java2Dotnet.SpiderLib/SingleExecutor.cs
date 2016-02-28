using System;
using System.Threading;
using log4net;

namespace Java2Dotnet.Spider.Lib
{
	public class SingleExecutor
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(SingleExecutor));
		private string _locker = "unlock";

		public void Execute(Action action)
		{
			if (_locker == "lock")
			{
				while (true)
				{
					Thread.Sleep(50);
					if (_locker != "lock")
					{
						return;
					}
				}
			}
			else
			{
				try
				{
					_locker = "lock";
					action();
				}
				finally
				{
					_locker = "unlock";
				}
			}
		}
	}
}
