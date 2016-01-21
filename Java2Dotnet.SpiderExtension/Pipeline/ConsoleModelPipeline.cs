using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Newtonsoft.Json;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	/// <summary>
	/// Print page model in con
	/// Usually used in test.
	/// </summary>
	public class ConsoleModelPipeline<T> : IPageModelPipeline<T>
	{
		public void Process(List<T> data, ISpider spider)
		{
			Console.WriteLine(JsonConvert.SerializeObject(data));
		}

		public void Dispose()
		{
		}
	}
}
