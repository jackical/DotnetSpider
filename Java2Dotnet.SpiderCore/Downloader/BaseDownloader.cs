using System;
using Java2Dotnet.Spider.Core.Redial;
using log4net;

namespace Java2Dotnet.Spider.Core.Downloader
{
	public class BaseDownloader : IDownloader, IDisposable
	{
		public DownloadVerify DownloadVerifyEvent;
		public IRedialer Redialer;

		protected static readonly ILog Logger = LogManager.GetLogger(typeof(BaseDownloader));
		protected int ThreadNum;

		public virtual Page Download(Request request, ISpider spider)
		{
			return null;
		}

		protected virtual void OnSuccess(Request request)
		{
		}

		public virtual void Dispose()
		{
		}

		public void SetThreadNum(int threadNum)
		{
			ThreadNum = threadNum;
		}
	}
}
