using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
		private readonly int _maxTaskCount;
		private readonly CancellationTokenSource _cts = new CancellationTokenSource();
		private readonly SynchronizedList<Task> _tasks = new SynchronizedList<Task>();
		private readonly ConcurrentQueue<Task> _cachedTasks = new ConcurrentQueue<Task>();
		private bool _end;
		private readonly int _cachedSize;

		public CountableThreadPool(int threadNum = 5)
		{
			ThreadNum = threadNum;
			//_maxTaskCount = _maxDegreeOfParallelism + threadNum;
			_maxTaskCount = threadNum;

			_cachedSize = _maxTaskCount + 5;

			LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(threadNum);
 
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					if (_end)
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

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					if (_end)
					{
						break;
					}

					while (AliveAndWaitingThreadCount >= _maxTaskCount)
					{
						Thread.Sleep(10);
					}

					Task task;
					if (_cachedTasks.TryDequeue(out task))
					{
						task.Start(lcts);
						_tasks.Add(task);
					}
					Thread.Sleep(10);
				}
			});
		}

		public int ThreadAlive => _tasks.Count(t => t.Status == TaskStatus.Running);

		public int ThreadNum { get; }

		private int AliveAndWaitingThreadCount => _tasks.Count(t => t.Status == TaskStatus.Running || t.Status == TaskStatus.RanToCompletion || t.Status == TaskStatus.WaitingToRun);

		public void Push(Func<object, CancellationTokenSource, int> func, object obj)
		{
			// List中保留比最大线程数多5个
			while (_cachedTasks.Count > _cachedSize)
			{
				Thread.Sleep(10);
			}

			_cachedTasks.Enqueue(new Task(o =>
			{
				CancellationTokenSource cts1 = (CancellationTokenSource)o;
				func.Invoke(obj, cts1);
			}, _cts));
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void WaitToExit()
		{
			while (_tasks.Count() > 0 || _cachedTasks.Count > 0)
			{
				Thread.Sleep(1000);
			}

			_end = true;
		}

		public bool IsShutdown => _cts.IsCancellationRequested;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Shutdown()
		{
			_cts.Cancel();

			while (!_cts.IsCancellationRequested)
			{
				Thread.Sleep(500);
			}
			_end = true;
		}
	}
}