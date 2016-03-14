using System;
using System.Collections.Generic;
using System.Threading;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Redial;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public sealed class DatabasePipeline<T> : IPageModelPipeline<T> where T : ISpiderEntity
	{
		private readonly static log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(DatabasePipeline<T>));
		private readonly IDataRepository<T> _dataRepository;

		public long TotalCount => _totalCount.Value;

		public DatabasePipeline()
		{
			_dataRepository = new DataRepository<T>();
			_dataRepository.CreateSheme();
			_dataRepository.CreateTable();
		}

		private readonly AutomicLong _totalCount = new AutomicLong(0);

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

			SaveOrUpate(data, spider);
		}

		public void Dispose()
		{
		}

		private void SaveOrUpate(List<T> data, ISpider spider)
		{
			PipelineModel model = PipelineModel.Insert;

			if (spider.Settings.ContainsKey("PipelineModel"))
			{
				model = spider.Settings["PipelineModel"];
			}
			switch (model)
			{
				case PipelineModel.Insert:
					{
						RedialManagerUtils.Execute("db-insert", () =>
						{
							Insert(data);
						});

						break;
					}
				case PipelineModel.Update:
					{
						RedialManagerUtils.Execute("db-update", () =>
						{
							_dataRepository?.Update(data);
						});

						break;
					}
			}
		}

		private void Insert(List<T> data)
		{
			for (int i = 0; i < 100; i++)
			{
				try
				{
					_dataRepository?.Insert(data);
					break;
				}
				catch (Exception e)
				{
					Logger.Warn($"Try to save data to DB failed. Times: {i + 1}", e);
					Thread.Sleep(2000);
					// ignored
				}
			}
		}
	}
}
