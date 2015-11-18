using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test
{
	[TestClass]
	public class CountableThreadPoolTests
	{
		[TestMethod]
		public void CountableThreadPoolTest()
		{
			/*构建一个threadPool*/
			var threadPool = new CountableThreadPool();
			for (int i = 0; i <= 10; i++)
			{
				threadPool.Execute((obj, cts) =>
				{
					Thread.Sleep(1000 * 30);
					return 1;
				}, "");
			}
			Thread.Sleep(1000 * 10);
			Assert.AreEqual(threadPool.GetThreadAlive(), 5);
			Thread.Sleep(1000 * 60);
			Assert.IsTrue(threadPool.GetThreadAlive() == 0);
			threadPool.WaitToEnd();
			threadPool.Shutdown();
		}
	}
}
