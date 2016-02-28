using Java2Dotnet.Spider.Core.Selector;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Selector
{
	[TestClass]
	public class JsonTest
	{
		private string _text = "{\"name\":\"json\"}";

		[TestMethod]
		public void Test01()
		{
			//string name = new Selectable(_text,"").JsonPath("$.name").Value;
			//Assert.AreEqual(name, "json");
		}
	}
}
