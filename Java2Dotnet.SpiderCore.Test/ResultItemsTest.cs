﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test
{
	[TestClass]
	public class ResultItemsTest
	{
		[TestMethod]
		public void TestOrderOfEntries()
		{
			ResultItems resultItems = new ResultItems();
			resultItems.AddOrUpdateResultItem("a", "a");
			resultItems.AddOrUpdateResultItem("b", "b");
			resultItems.AddOrUpdateResultItem("c", "c");

			dynamic a = resultItems.GetResultItem("a");
			dynamic b = resultItems.GetResultItem("b");
			dynamic c = resultItems.GetResultItem("c");
			//Assert.AreEqual(a, "a");
			//Assert.AreEqual(b, "b");
			//Assert.AreEqual(c, "c");
		}
	}
}
