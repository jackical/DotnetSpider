﻿using System.Collections.Generic;
using Java2Dotnet.Spider.Extension;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Model.Formatter;

namespace Java2Dotnet.Spider.Samples.Model
{
	[TargetUrl(new[] { "http://news.163.com/\\d+/\\d+/\\d+/\\w+*.html" })]
	public class News163 : IMultiPageModel
	{
		//[ExtractByUrl("http://news\\.163\\.com/\\d+/\\d+/\\d+/([^_]*).*\\.html")]
		public string PageKey { get; set; }

		//[ExtractByUrl(Expession = "http://news\\.163\\.com/\\d+/\\d+/\\d+/\\w+_(\\d+)\\.html", NotNull = false)]
		public string Page { get; set; }

		////div[@class=\"ep-pages\"]/a[substring(@href,string-length(@href)-5)='.html']

		[Formatter(typeof(RegexFormater), new[] { "http://news\\.163\\.com/\\d+/\\d+/\\d+/\\w+_(\\d+)\\.html", "1" })]
		[PropertyExtractBy(Expression = "//div[@class=\"ep-pages\"]/a/@href", NotNull = false)]
		public HashSet<string> OtherPage { get; set; }

		[PropertyExtractBy(Expression = "//h1[@id=\"h1title\"]/text()")]
		public string Title { get; set; }

		[PropertyExtractBy(Expression = "//div[@id=\"epContentLeft\"]")]
		public string Content { get; set; }

		public string GetPageKey()
		{
			return PageKey;
		}

		public string GetPage()
		{
			if (Page == null)
			{
				return "1";
			}
			return Page;
		}

		public ICollection<string> GetOtherPages()
		{
			return OtherPage;
		}

		public IMultiPageModel Combine(IMultiPageModel multiPageModel)
		{
			News163 news163 = new News163 { Title = Title };
			News163 pagedModel1 = (News163)multiPageModel;
			news163.Content = Content + pagedModel1.Content;
			return news163;
		}

		public override string ToString()
		{
			return "News163{" +
					"content='" + Content + '\'' +
					", title='" + Title + '\'' +
					", otherPage=" + OtherPage +
					'}';
		}

		public static void Run()
		{
			//ModelSpider<News163> spider = new ModelSpider<News163>("aiqiyi_movies_" + DateTime.Now.ToLocalTime(), new Site { SleepTime = 1000, Encoding = Encoding.UTF8 }, ModelPipelineType.Collect);
			//spider.AddStartUrl("http://news.163.com/13/0802/05/958I1E330001124J_2.html");
			//spider.AddPipeline(new MultiPagePipeline());
			//spider.Run();
			//OoSpider.Create(new Site(), new RedisScheduler("localhost", ""), new CollectorPageModelPipeline(), typeof(List<News163>)).AddStartUrl("http://news.163.com/13/0802/05/958I1E330001124J_2.html")
			//	.AddPipeline(new MultiPagePipeline())
			//	.AddPipeline(new ConsolePipeline())
			//	.Run();
		}
	}
}
