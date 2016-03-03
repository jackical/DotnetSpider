using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Core.Processor;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Extension.Pipeline;

namespace Java2Dotnet.Spider.Extension.Model
{
	public class EntityGeneralSpider : BaseModelSpider
	{
		public EntityGeneralSpider(string identify, IPageProcessor pageProcessor, IScheduler scheduler) : base(identify, pageProcessor, scheduler)
		{
		}
	}
}
