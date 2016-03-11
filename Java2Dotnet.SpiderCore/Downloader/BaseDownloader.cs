using System;
using Java2Dotnet.Spider.Lib;
using Java2Dotnet.Spider.Redial;
using log4net;

namespace Java2Dotnet.Spider.Core.Downloader
{
	public class BaseDownloader : IDownloader, IDisposable
	{
		public DownloadValidation DownloadValidation { get; set; }
		public int ThreadNum { set; get; }

		protected static readonly ILog Logger = LogManager.GetLogger(typeof(BaseDownloader));
		protected SingleExecutor SingleExecutor = new SingleExecutor();

		public Action CustomizeCookie;

		public virtual Page Download(Request request, ISpider spider)
		{
			return null;
		}

		public virtual void Dispose()
		{
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
							if (RedialManagerUtils.RedialManager == null)
							{
								throw new SpiderExceptoin("RedialManager is null.");
							}

							RedialManagerUtils.RedialManager?.Redial();
							throw new RedialException("Download failed and Redial already.");
						}
					case DownloadValidationResult.Success:
						{
							break;
						}
					case DownloadValidationResult.FailedAndNeedUpdateCookie:
						{
							SingleExecutor.Execute(() =>
							{
								CustomizeCookie?.Invoke();
							});
							throw new SpiderExceptoin("Cookie validate failed.");
						}
					case DownloadValidationResult.Miss:
						{
							page.IsSkip = true;
							break;
						}
				}
			}
		}
	}
}
