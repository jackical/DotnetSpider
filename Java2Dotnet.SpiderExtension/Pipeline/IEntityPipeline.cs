using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public interface IEntityPipeline : IDisposable
	{
		void Process(List<dynamic> data, ISpider spider);
	}
}
