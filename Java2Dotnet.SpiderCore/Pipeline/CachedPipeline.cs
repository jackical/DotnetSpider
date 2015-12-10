using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Java2Dotnet.Spider.Core.Pipeline
{
	public delegate void FlushCachedPipeline(ISpider spider);

	public abstract class CachedPipeline : IPipeline
	{
		protected static readonly ILog Logger = LogManager.GetLogger(typeof(CachedPipeline));

		private readonly List<ResultItems> _cached = new List<ResultItems>();

		public int CachedSize { get; set; } = 1;

		public void Process(ResultItems resultItems, ISpider spider)
		{
			_cached.Add(resultItems);

			if (_cached.Count >= CachedSize)
			{
				ResultItems[] result;
				lock (this)
				{
					result = new ResultItems[_cached.Count];
					_cached.CopyTo(result);
					_cached.Clear();
				}

				// 做成异步
				Process(result.ToList(), spider);
			}
		}

		public void Flush(ISpider spider)
		{
			if (_cached.Count > 0)
			{
				Process(_cached, spider);
				_cached.Clear();
			}
		}

		protected abstract void Process(List<ResultItems> resultItemsList, ISpider spider);
	}
}
