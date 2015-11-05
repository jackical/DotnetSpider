using Microsoft.VisualStudio.TestTools.UnitTesting;
using Java2Dotnet.Spider.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Java2Dotnet.Spider.Extension.Test
{
    [TestClass()]
    public class CountableThreadPoolTests
    {
        [TestMethod()]
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
