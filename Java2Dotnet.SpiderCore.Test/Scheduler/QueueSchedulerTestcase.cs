﻿using System.Threading.Tasks;
using Java2Dotnet.Spider.Core.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Scheduler
{
	[TestClass]
	public class QueueSchedulerTestcase
	{
		[TestMethod]
		public void QueueSchedulerPushPollSynchronized()
		{
			QueueDuplicateRemovedScheduler scheduler = new QueueDuplicateRemovedScheduler();
			ISpider spider = new DefaultSpider("test", new Site());

			Parallel.For(0, 1000, new ParallelOptions() { MaxDegreeOfParallelism = 30 }, i =>
			{
				scheduler.Push(new Request("http://www.a.com", 1, null), spider);
				scheduler.Push(new Request("http://www.a.com", 1, null), spider);
				scheduler.Push(new Request("http://www.a.com", 1, null), spider);

				scheduler.Push(new Request("http://www.b.com", 1, null), spider);

				scheduler.Push(new Request($"http://www.{i.ToString()}.com", 1, null), spider);
			});

			Parallel.For(0, 1000, new ParallelOptions() { MaxDegreeOfParallelism = 30 }, i =>
			{
				scheduler.Poll(spider);
			});

			int left = scheduler.GetLeftRequestsCount(spider);
			int total = scheduler.GetTotalRequestsCount(spider);

			Assert.AreEqual(left, 2);
			Assert.AreEqual(total, 1002);
		}

		[TestMethod]
		public void QueueSchedulerPush()
		{
			QueueDuplicateRemovedScheduler scheduler = new QueueDuplicateRemovedScheduler();
			ISpider spider = new DefaultSpider("test", new Site());
			scheduler.Push(new Request("http://www.a.com", 1, null), spider);
			scheduler.Push(new Request("http://www.a.com", 1, null), spider);
			scheduler.Push(new Request("http://www.a.com", 1, null), spider);

			scheduler.Push(new Request("http://www.b.com", 1, null), spider);
			int left = scheduler.GetLeftRequestsCount(spider);
			int total = scheduler.GetTotalRequestsCount(spider);

			Assert.AreEqual(left, 2);
			Assert.AreEqual(total, 2);
		}


		[TestMethod]
		public void QueueSchedulerPoll()
		{
			QueueDuplicateRemovedScheduler scheduler = new QueueDuplicateRemovedScheduler();
			ISpider spider = new DefaultSpider("test", new Site());
			scheduler.Push(new Request("http://www.a.com", 1, null), spider);
			scheduler.Push(new Request("http://www.a.com", 1, null), spider);
			scheduler.Push(new Request("http://www.a.com", 1, null), spider);

			scheduler.Push(new Request("http://www.b.com", 1, null), spider);

			var request = scheduler.Poll(spider);
			Assert.AreEqual(request.Url, "http://www.a.com");

			int left = scheduler.GetLeftRequestsCount(spider);
			int total = scheduler.GetTotalRequestsCount(spider);

			Assert.AreEqual(left, 1);
			Assert.AreEqual(total, 2);
		}
	}
}
