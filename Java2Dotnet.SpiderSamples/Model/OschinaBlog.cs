using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Samples.Model
{
	[TargetUrl(new[] { "http://my.oschina.net/flashsword/blog" })]
	public class OschinaBlog
	{
		[PropertyExtractBy(Expression = "//title/text()")]
		public string Title { get; set; }

		[PropertyExtractBy(Expression = "div.BlogContent", Type = ExtractType.Css)]
		public string Content { get; set; }

		[PropertyExtractBy(Expression = "//div[@class='BlogTags']/a/text()")]
		public HashSet<string> Tags { get; set; }

		//[PropertyExtractBy(Expression = "//div[@class='BlogStat']/regex('\\d+-\\d+-\\d+\\s+\\d+:\\d+')")]
		//public DateTime Date { get; set; }

		public static void Run()
		{
			Site site = new Site
			{
				UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_8_5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36"
			};
			site.AddStartUrl("http://my.oschina.net/flashsword/blog");
			site.SleepTime = 0;
			site.RetryTimes = 3;

			//OoSpider.Create(site, new DatabasePipeline(), typeof(OschinaBlog)).SetThreadNum(1).Run();
			//ModelSpider<OschinaBlog> spider = new ModelSpider<OschinaBlog>(Guid.NewGuid().ToString(), site, ModelPipelineType.Collect);
		}
	}
}
