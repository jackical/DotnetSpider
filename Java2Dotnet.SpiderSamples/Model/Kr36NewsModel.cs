using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Samples.Model
{
	[TargetUrl(new[] { "http://www.36kr.com/p/\\d+.html" })]
	public class Kr36NewsModel
	{
		[PropertyExtractBy(Expression = "//h1[@class='entry-title sep10']")]
		public string Title { get; set; }

		[PropertyExtractBy(Expression = "//div[@class='mainContent sep-10']/tidyText()")]
		public string Content { get; set; }

		//[ExtractByUrl]
		public string Url { get; set; }

		public static void Run()
		{
			Site site = new Site();
			site.AddStartUrl("http://www.36kr.com/");
			//Core.Spider thread = OoSpider.Create(site, new DatabasePipeline(), typeof(Kr36NewsModel)).SetThreadNum(20);
			//ModelSpider<Kr36NewsModel> thread = new ModelSpider<Kr36NewsModel>(Guid.NewGuid().ToString(), site, ModelPipelineType.MySql);
			//thread.SetThreadNum(20);
			//SpiderMonitor spiderMonitor = SpiderMonitor.Instance;
			//spiderMonitor.Register(thread);
			//thread.Run();
		}
	}
}