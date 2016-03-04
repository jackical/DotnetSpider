using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class EntityJsonFilePipeline : FilePersistentBase, IEntityPipeline
	{
		public class JsonFilePipelineArgument
		{
			public string Directory { get; set; }
		}

		private readonly string _entityName;

		public EntityJsonFilePipeline(JObject entityDefine, JObject argument)
		{
			_entityName = entityDefine.SelectToken("$.identity").ToString();
			SetPath(argument.ToObject<JsonFilePipelineArgument>()?.Directory);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(List<JObject> datas, ISpider spider)
		{
			FileInfo file = PrepareFile(_entityName);

			using (StreamWriter printWriter = new StreamWriter(file.OpenWrite(), Encoding.UTF8))
			{
				foreach (var entry in datas)
				{
					printWriter.WriteLine(entry);
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
