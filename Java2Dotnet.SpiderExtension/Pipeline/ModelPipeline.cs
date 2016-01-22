using System;
using System.Collections;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// The extension to Pipeline for page model extractor.
	/// </summary>
	public class ModelPipeline<T> : CachedPipeline
	{
		public IPageModelPipeline<T> PageModelPipeline { get; }
		private readonly Type _actuallyType;

		public ModelPipeline(IPageModelPipeline<T> pageModelPipeline)
		{
			_actuallyType = typeof(T);

			PageModelPipeline = pageModelPipeline;
		}

		protected override void Process(List<ResultItems> resultItemsList, ISpider spider)
		{
			if (resultItemsList == null || resultItemsList.Count == 0)
			{
				return;
			}

			List<T> list = new List<T>();
			foreach (var resultItems in resultItemsList)
			{
				dynamic data = resultItems.GetResultItem(_actuallyType.FullName);

				if (data != null)
				{
					if (data is IEnumerable)
					{
						list.AddRange(data);
					}
					else
					{
						list.Add(data);
					}
				}
			}
			PageModelPipeline.Process(list, spider);
		}
	}
}