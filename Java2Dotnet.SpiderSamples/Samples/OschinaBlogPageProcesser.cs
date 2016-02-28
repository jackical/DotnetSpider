using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Processor;

namespace Java2Dotnet.Spider.Samples.Samples
{
	public class OschinaBlogPageProcesser : IPageProcessor
	{
		public Site Site { get; } = new Site { Domain = "my.oschina.net" }.AddStartUrl("http://my.oschina.net/flashsword/blog");

		public void Process(Page page)
		{
			IList<String> links = page.HtmlSelectable.Links().Regex("http://my\\.oschina\\.net/flashsword/blog/\\d+").Value;
			page.AddTargetRequests(links);
			page.AddResultItem("title", page.HtmlSelectable.XPath("//div[@class='BlogEntity']/div[@class='BlogTitle']/h1/text()").ToString());
			page.AddResultItem("content", page.HtmlSelectable.XPath("//div[@class='BlogContent']/tidyText()").ToString());
			page.AddResultItem("tags", page.HtmlSelectable.XPath("//div[@class='BlogTags']/a/text()").Value);
			page.AddResultItem("artical", page.HtmlSelectable.XPath("//*[@Class='Blog']/div[1]/div/h2/a").ToString());
		}
	}
}
