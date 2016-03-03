using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class EntityGeneralPipeline : IEntityPipeline
	{
		public class GeneralPipelineArgument
		{
			public string Host { get; set; }
			public string Port { get; set; }
			public string Password { get; set; }
			public string Provider { get; set; }
		}

		private readonly JObject _entityDefine;
		private readonly GeneralPipelineArgument _argument;

		public EntityGeneralPipeline(JObject entityDefine,JObject argument)
		{
			_entityDefine = entityDefine;
			_argument = argument.ToObject<GeneralPipelineArgument>();
		}

		public void Process(List<dynamic> data, ISpider spider)
		{


		}

		public void Dispose()
		{
		}
	}
}
