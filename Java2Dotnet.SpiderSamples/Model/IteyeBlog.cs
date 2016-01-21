using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Samples.Model
{
	[TargetUrl(new[] { "http://*.iteye.com/blog/*" })]
	public class IteyeBlog
	{
		[PropertyExtractBy(Expression = "//title")]
		public string Title { get; set; }

		[PropertyExtractBy(Expression = "div#blog_content", Type = ExtractType.Css)]
		public string Content { get; set; }

		public static void Run()
		{
			var site = new Site();
			site.AddStartUrl("http://flashsword20.iteye.com/blog");
			//OoSpider.Create(site, new QueueDuplicateRemovedScheduler(), new CollectorPageModelPipeline(), typeof(IteyeBlog)).Run();
			//ModelSpider<IteyeBlog> spider = new ModelSpider<IteyeBlog>(Guid.NewGuid().ToString(), site, ModelPipelineType.Collect);
		}
	}
}
