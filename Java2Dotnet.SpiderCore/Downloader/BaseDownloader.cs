using System;
using System.Runtime.CompilerServices;
using Java2Dotnet.Spider.Core.Redial;
using log4net;

namespace Java2Dotnet.Spider.Core.Downloader
{
	public class BaseDownloader : IDownloader, IDisposable
	{
		public DownloadValidation DownloadValidation;
		public IRedialer Redialer { get; set; }

		protected static readonly ILog Logger = LogManager.GetLogger(typeof(BaseDownloader));
		protected int ThreadNum;

		private static readonly object Locker = new object();

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

		protected void ValidatePage(Page page)
		{
			//customer verify
			if (DownloadValidation != null)
			{
				var validatResult = DownloadValidation(page);

				switch (validatResult)
				{
					case DownloadValidationResult.Failed:
						{
							throw new SpiderExceptoin("Customize validate failed.");
						}
					case DownloadValidationResult.FailedAndNeedRedial:
						{
							Redialer?.Redial();
							throw new SpiderExceptoin("Customize validate failed.");
						}
					case DownloadValidationResult.Success:
						{
							break;
						}
				}
			}
		}
	}
}
