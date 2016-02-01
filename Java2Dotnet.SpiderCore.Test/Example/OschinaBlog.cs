using System;
using System.Collections.Generic;
using System.Text;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Example
{
	[TargetUrl(new[] { "http://my.oschina.net/flashsword/blog" })]
	public class OschinaBlog
	{
		[PropertyExtractBy(Expression = "//title/text()")]
		public string Title { get; set; }

		[PropertyExtractBy(Expression = "div.BlogContent", Type = ExtractType.Css)]
		public string Content { get; set; }

		[PropertyExtractBy(Expression = "//div[@class='BlogTags']/a/text()")]
		public List<string> Tags { get; set; }
	}

	[TestClass]
	public class OschinaBlogTest
	{
		[TestMethod]
		public void TestOschinaBlog()
		{
			//results will be saved to "/data/webmagic/" in json format
			//OoSpider.Create(new Site(), new JsonFilePageModelPipeline("/data/webmagic/"), typeof(OschinaBlog)).AddStartUrl("http://my.oschina.net/flashsword/blog").Run();
			ModelMysqlFileSpider<OschinaBlog> spider = new ModelMysqlFileSpider<OschinaBlog>(Guid.NewGuid().ToString(), new Site()
			{
				Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
				//Referer = "http://my.oschina.net/flashsword/blog",
				Headers = new Dictionary<string, string>()
				{
					{ "Upgrade-Insecure-Requests","1" }
				},
				Encoding = Encoding.UTF8
			});
			spider.AddStartUrl("http://my.oschina.net/flashsword/blog");
			spider.Run();
		}
	}
}