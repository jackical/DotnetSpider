using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Pipeline;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class EntityPipeline : CachedPipeline
	{
		private readonly IEntityPipeline _pipeline;
		private readonly string _entityName;

		public EntityPipeline(string entityName, IEntityPipeline pipeline)
		{
			_entityName = entityName;
			_pipeline = pipeline;
		}

		protected override void Process(List<ResultItems> resultItemsList, ISpider spider)
		{
			if (resultItemsList == null || resultItemsList.Count == 0)
			{
				return;
			}

			List<dynamic> list = new List<dynamic>();
			foreach (var resultItems in resultItemsList)
			{
				dynamic data = resultItems.GetResultItem(_entityName);

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
			_pipeline.Process(list, spider);
		}
	}
}
