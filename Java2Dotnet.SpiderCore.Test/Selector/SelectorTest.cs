using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Core.Selector.Html;
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
			HtmlSelectable htmlSelectable = new HtmlSelectable(_html, "");
			List<string> linksWithoutChain = htmlSelectable.Links().Value;
			ISelectable xpath = htmlSelectable.XPath("//div");
			List<string> linksWithChainFirstCall = xpath.Links().Value;
			List<string> linksWithChainSecondCall = xpath.Links().Value;
			Assert.AreEqual(linksWithoutChain.Count, linksWithChainFirstCall.Count);
			Assert.AreEqual(linksWithChainFirstCall.Count, linksWithChainSecondCall.Count);
		}

		[TestMethod]
		public void TestNodes()
		{
			HtmlSelectable htmlSelectable = new HtmlSelectable(_html, "");
			IList<ISelectable> links = htmlSelectable.XPath(".//a/@href").Nodes();
			Assert.AreEqual(links[0].Value, "http://whatever.com/aaa");

			List<string> links1 = htmlSelectable.XPath(".//a/@href").Value;
			Assert.AreEqual(links1[0], "http://whatever.com/aaa");
		}
	}
}
