using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public enum OperateType
	{
		Insert,
		Update
	}

	public sealed class PageModelToDbPipeline : IPageModelPipeline
	{
		private readonly static log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(PageModelToDbPipeline));
		private readonly OperateType _operateType;
		public long TotalCount => _totalCount.Value;
		private readonly ConcurrentDictionary<Type, IDataRepository> _cache = new ConcurrentDictionary<Type, IDataRepository>();

		private readonly string _dbOperateFlagFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DotnetSpdier", "DbOperate");

		public PageModelToDbPipeline(OperateType operateType = OperateType.Insert)
		{
			_operateType = operateType;
			DirectoryInfo di = new DirectoryInfo(_dbOperateFlagFolder);
			if (!di.Exists)
			{
				di.Create();
			}
		}

		private readonly AutomicLong _totalCount = new AutomicLong(0);

		public void Process(Dictionary<Type, List<dynamic>> data, ISpider spider)
		{
			if (data == null)
			{
				return;
			}

			foreach (var pair in data)
			{
				Type type = pair.Key;
				IDataRepository dataRepository = null;

				if (!type.IsGenericType)
				{
					if (_cache.ContainsKey(type))
					{
						dataRepository = _cache[type];
						_totalCount.Inc();
					}
					else
					{
						if (type.GetCustomAttribute(typeof(StoredAs)) != null)
						{
							dataRepository = new DataRepository(type);
							dataRepository.CreateSheme();
							dataRepository.CreateTable();
							_cache.TryAdd(type, dataRepository);
						}
						else
						{
							throw new SpiderExceptoin("Didn't define TableName( StoreAs attribute) in the Type: " + type.FullName);
						}
					}
				}
				else
				{
					IList list = pair.Value;
					if (list.Count > 0)
					{
						type = list[0].GetType();

						if (_cache.ContainsKey(type))
						{
							dataRepository = _cache[type];
						}
						else
						{
							if (type.GetCustomAttribute(typeof(StoredAs)) != null)
							{
								dataRepository = new DataRepository(type);
								dataRepository.CreateSheme();
								dataRepository.CreateTable();
								_cache.TryAdd(type, dataRepository);
							}
							else
							{
								throw new SpiderExceptoin("Didn't define TableName( StoreAs attribute) in the Type: " + type.FullName);
							}
						}

						for (int i = 0; i < list.Count; ++i)
						{
							_totalCount.Inc();
						}
					}
				}
				switch (_operateType)
				{
					case OperateType.Insert:
						{
							Stream stream = null;
							string id = Path.Combine(_dbOperateFlagFolder, Guid.NewGuid().ToString());
							try
							{
								stream = File.Open(id, FileMode.Create, FileAccess.Write);

								for (int i = 0; i < 100; i++)
								{
									try
									{
										dataRepository?.Insert(pair.Value);
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
							finally
							{
								stream?.Close();
								File.Delete(id);
							}
							break;
						}
					case OperateType.Update:
						{
							dataRepository?.Update(pair.Value);
							break;
						}
				}
			}
		}
	}
}
