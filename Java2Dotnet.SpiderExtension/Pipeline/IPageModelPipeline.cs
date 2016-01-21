using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// Implements PageModelPipeline to persistent your page model.
	/// </summary>
	public interface IPageModelPipeline<T> : IDisposable
	{
		void Process(List<T> data, ISpider spider);
	}
}
