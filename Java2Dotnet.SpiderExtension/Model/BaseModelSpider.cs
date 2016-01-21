using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;
using Java2Dotnet.Spider.Core.Processor;
using Java2Dotnet.Spider.Core.Scheduler;
using Java2Dotnet.Spider.Extension.Pipeline;
using Java2Dotnet.Spider.Extension.Processor;

namespace Java2Dotnet.Spider.Extension.Model
{
	public class BaseModelSpider : Core.Spider
	{
		public const string PipelineMode = "PipelineMode";

		protected BaseModelSpider(string identify, IPageProcessor pageProcessor, IScheduler scheduler) : base(identify, pageProcessor, scheduler)
		{
		}

		public void SetCustomizeTargetUrls(Func<Page, IList<string>> getCustomizeTargetUrls)
		{
			((ModelPageProcessor)PageProcessor).GetCustomizeTargetUrls = getCustomizeTargetUrls;
		}

		public void SetCachedSize(int count)
		{
			foreach (var pipeline in Pipelines)
			{
				var cachedPipeline = pipeline as CachedPipeline;
				if (cachedPipeline != null)
				{
					cachedPipeline.CachedSize = count;
				}
			}
		}

		public void SetPipelineModel(PipelineModel model)
		{
			Settings.Add(PipelineMode, model);
		}
	}
}
