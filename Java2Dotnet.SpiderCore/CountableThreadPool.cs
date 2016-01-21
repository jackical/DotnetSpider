using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Lib;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Thread pool. 
	/// </summary>
	public class CountableThreadPool
	{
		private readonly SynchronizedList<Task> _tasks = new SynchronizedList<Task>();
		private readonly int _cachedSize;
		private bool _exit;
		private readonly TaskFactory _factory;

		public CountableThreadPool(int threadNum = 5)
		{
			ThreadNum = threadNum;
			_cachedSize = ThreadNum * 2;

			_factory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(ThreadNum));

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					if (_exit)
					{
						break;
					}

					var finishedTasks = _tasks.Where(t => t.IsCompleted).ToList();
					foreach (var finishedTask in finishedTasks)
					{
						_tasks.Remove(finishedTask);
					}

					Thread.Sleep(10);
				}
			});
		}

		public int ThreadAlive
		{
			get { return _tasks.Where(t => t.Status == TaskStatus.Running).Count; }
		}

		public int ThreadNum { get; }

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Push(Func<object, bool> func, object obj)
		{
			if (_exit)
			{
				throw new SpiderExceptoin("Pool is exit.");
			}

			// List�б���������߳�����5��
			while (_tasks.Count() > _cachedSize)
			{
				Thread.Sleep(10);
			}

			Task task = _factory.StartNew((o) => { func(o); }, obj);
			_tasks.Add(task);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void WaitToExit()
		{
			Task.WaitAll(_tasks.GetAll().ToArray());
			_exit = true;
		}
	}
}