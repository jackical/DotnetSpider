using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class CollectorModelPipeline<T> : IPageModelPipeline<T>
	{
		private readonly List<T> _collector = new List<T>();

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(List<T> data, ISpider spider)
		{
			_collector.AddRange(data);
		}

		public List<T> GetCollected()
		{
			return _collector;
		}

		public void Dispose()
		{
			_collector.Clear();
		}
	}
}
