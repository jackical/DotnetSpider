using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Core.Selector.Html;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test
{
	[TestClass]
	public class HtmlTest
	{
		[TestMethod]
		public void TestRegexSelector()
		{
			HtmlSelectable htmlSelectable = new HtmlSelectable("aaaaaaab", "");
			//        Assert.assertEquals("abbabbab", (selectable.regex("(.*)").replace("aa(a)", "$1bb").toString()));
			string value = htmlSelectable.Regex("(.*)").Value;
			Assert.AreEqual("aaaaaaab", value);

		}
	}
}
