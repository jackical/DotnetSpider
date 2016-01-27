using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Selector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Selector
{
	[TestClass]
	public class SelectorTest
	{
		private string _html = "<div><a href='http://whatever.com/aaa'></a></div><div><a href='http://whatever.com/bbb'></a></div>";

		[TestMethod]
		public void TestChain()
		{
			Html selectable = new Html(_html, "");
			List<string> linksWithoutChain = selectable.Links().Value;
			ISelectable xpath = selectable.XPath("//div");
			List<string> linksWithChainFirstCall = xpath.Links().Value;
			List<string> linksWithChainSecondCall = xpath.Links().Value;
			Assert.AreEqual(linksWithoutChain.Count, linksWithChainFirstCall.Count);
			Assert.AreEqual(linksWithChainFirstCall.Count, linksWithChainSecondCall.Count);
		}

		[TestMethod]
		public void TestNodes()
		{
			Html selectable = new Html(_html, "");
			IList<ISelectable> links = selectable.XPath(".//a/@href").Nodes();
			Assert.AreEqual(links[0].Value, "http://whatever.com/aaa");

			List<string> links1 = selectable.XPath(".//a/@href").Value;
			Assert.AreEqual(links1[0], "http://whatever.com/aaa");
		}
	}
}
