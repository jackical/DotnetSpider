using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Java2Dotnet.Spider.Core.Scheduler
{
	/// <summary>
	/// Basic Scheduler implementation. 
	/// Store urls to fetch in LinkedBlockingQueue and remove duplicate urls by HashMap.
	/// </summary>
	//check:
	//[Synchronization]
	public class QueueDuplicateRemovedScheduler : DuplicateRemovedScheduler, IMonitorableScheduler
	{
		//check:
		private readonly Queue<Request> _queue = new Queue<Request>();

		[MethodImpl(MethodImplOptions.Synchronized)]
		protected override void PushWhenNoDuplicate(Request request, ISpider spider)
		{
			_queue.Enqueue(request);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public override Request Poll(ISpider spider)
		{
			return _queue.Count > 0 ? _queue.Dequeue() : null;
		}

		public int GetLeftRequestsCount(ISpider spider)
		{
			return _queue.Count;
		}

		public int GetTotalRequestsCount(ISpider spider)
		{
			return DuplicateRemover.GetTotalRequestsCount(spider);
		}
	}
}