﻿using Java2Dotnet.Spider.Core.Selector;
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
			[ExtractBy(Value = "//div/@foo", NotNull = true)]
			public string Foo { get; set; }
		}

		[TargetUrl(new[] { "http://codecraft.us/bar" })]
		public class ModelBar
		{
			[ExtractBy(Value = "//div/@bar", NotNull = true)]
			public string Bar { get; set; }
		}

		[TestMethod]
		public void testMultiModel_should_not_skip_when_match()
		{
			Page page = new Page(new Request("http://codecraft.us/foo", 1, null));
			page.RawText = "<div foo='foo'></div>";
			page.Url = ("http://codecraft.us/foo");
			ModelPageProcessor modelPageProcessor = ModelPageProcessor.Create(null, typeof(ModelFoo), typeof(ModelBar));
			modelPageProcessor.Process(page);
			Assert.IsFalse(page.ResultItems.IsSkip);
		}
	}
}
