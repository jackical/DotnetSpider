using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Processor;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Model
{
	[TestClass]
	public class ModelPageProcessorTest
	{
		[TargetUrl(new[] { "http://codecraft.us/foo" })]
		public class ModelFoo
		{
			[PropertyExtractBy(Expression = "//div/@foo", NotNull = true)]
			public string Foo { get; set; }
		}

		[TargetUrl(new[] { "." })]
		public class ModelBar
		{
			[PropertyExtractBy(Expression = "//div[2]/@bar", NotNull = true)]
			public string Bar { get; set; }
		}

		[TestMethod]
		public void testMultiModel_should_not_skip_when_match()
		{
			Page page = new Page(new Request("http://codecraft.us/foo", 1, null));
			page.RawText = "<div foo='foo'></div><div bar='bar'></div>";
			page.Url = ("http://codecraft.us/foo");
			ModelPageProcessor modelPageProcessor = new ModelPageProcessor<ModelFoo, ModelBar>(new Site());
			modelPageProcessor.Process(page);
			Assert.IsFalse(page.ResultItems.IsSkip);
		}
	}
}
