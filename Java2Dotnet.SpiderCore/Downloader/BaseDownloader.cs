using System;
using Java2Dotnet.Spider.Redial;
using log4net;

namespace Java2Dotnet.Spider.Core.Downloader
{
	public class BaseDownloader : IDownloader, IDisposable
	{
		public DownloadValidation DownloadValidation;

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
							RedialManager.Default?.Redial();
							throw new RedialException("Download failed and need Redial.");
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
