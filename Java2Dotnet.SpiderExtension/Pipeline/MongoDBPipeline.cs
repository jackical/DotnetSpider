using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Extension.Model;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class MongoDbPipeline<T> : IPageModelPipeline<T> where T : SpiderEntityUseStringKey
	{
		//private readonly static log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(MongoDbPipeline<T>));
		public long TotalCount => _totalCount.Value;
		private readonly AutomicLong _totalCount = new AutomicLong(0);
		private readonly IDataRepository<T> _repository;

		public MongoDbPipeline()
		{
			_repository = new MongoDbRepository<T>();
		}

		public void Dispose()
		{
		}

		public void Process(List<T> data, ISpider spider)
		{
			if (data == null || data.Count == 0)
			{
				return;
			}

			if (data.Count > 0)
			{
				for (int i = 0; i < data.Count; ++i)
				{
					_totalCount.Inc();
				}
			}

			_repository.Insert(data);
		}
	}
}
