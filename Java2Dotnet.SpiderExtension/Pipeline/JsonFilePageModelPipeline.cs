using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Newtonsoft.Json;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// Store results objects (page models) to files in JSON format.
	/// Use model.getKey() as file name if the model implements HasKey.
	/// Otherwise use SHA1 as file name.
	/// </summary>
	public class JsonFileModelPipeline<T> : FilePersistentBase, IPageModelPipeline<T>
	{
		private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(JsonFileModelPipeline<T>));
		private readonly Type _type;

		/// <summary>
		/// New JsonFilePageModelPipeline with default path "/data/webmagic/"
		/// </summary>
		public JsonFileModelPipeline()
		{
			SetPath("/data/");
			
			_type = typeof(T);
		}

		public void Process(List<T> data, ISpider spider)
		{
			string path = BasePath + "/" + spider.Identity + "/";
			string filename = path + _type.FullName + ".json";

			foreach (var entry in data)
			{
				try
				{
					FileInfo file = PrepareFile(filename);
					using (StreamWriter printWriter = new StreamWriter(file.OpenWrite(), Encoding.UTF8))
					{
						printWriter.WriteLine(JsonConvert.SerializeObject(entry));
					}
				}
				catch (Exception e)
				{
					_logger.Warn("write file error", e);
					throw;
				}
			}
		}

		public void Dispose()
		{
		}
	}
}