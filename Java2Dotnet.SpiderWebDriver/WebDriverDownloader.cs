using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Selector;
using OpenQA.Selenium;
using Java2Dotnet.Spider.Lib;

namespace Java2Dotnet.Spider.WebDriver
{
	public class WebDriverDownloader : BaseDownloader
	{
		private volatile WebDriverPool _webDriverPool;
		private readonly int _webDriverWaitTime;
		private readonly Browser _browser;
		private readonly Option _option;
		private static bool _isLogined;

		public Func<IWebDriver, bool> LoginFunc;
		public Func<string, string> UrlFormatFunc;
		public Func<IWebDriver, bool> AfterNavigateFunc;

		public WebDriverDownloader(Browser browser = Browser.Phantomjs, int webDriverWaitTime = 200, Option option = null)
		{
			_option = option ?? new Option();
			_webDriverWaitTime = webDriverWaitTime;
			_browser = browser;

			if (browser == Browser.Firefox)
			{
				Task.Factory.StartNew(() =>
				{
					while (true)
					{
						IntPtr maindHwnd = WindowsFormUtil.FindWindow(null, "plugin-container.exe - 应用程序错误");
						if (maindHwnd != IntPtr.Zero)
						{
							WindowsFormUtil.SendMessage(maindHwnd, WindowsFormUtil.WM_CLOSE, 0, 0);
						}
						Thread.Sleep(500);
					}
					// ReSharper disable once FunctionNeverReturns
				});
			}
		}

		public override Page Download(Request request, ISpider spider)
		{
			CheckInit();

			WebDriverItem driverService = null;

			try
			{
				driverService = _webDriverPool.Get();

				lock (this)
				{
					Site site = spider.Site;
					if (!_isLogined && LoginFunc != null)
					{
						_isLogined = LoginFunc.Invoke(driverService.WebDriver);
						if (!_isLogined)
						{
							throw new SpiderExceptoin("Login failed. Please check your login codes.");
						}
					}

					IOptions manage = driverService.WebDriver.Manage();
					if (site.GetCookies() != null && site.GetCookies().Count > 0)
					{
						foreach (KeyValuePair<String, String> cookieEntry in site.GetCookies())
						{
							Cookie cookie = new Cookie(cookieEntry.Key, cookieEntry.Value);
							manage.Cookies.AddCookie(cookie);
						}
					}
					//else
					//{
					//	manage.Cookies.DeleteAllCookies();
					//}
				}

				//Logger.Info("Downloading page " + request.Url);

				//中文乱码URL
				Uri uri = new Uri(request.Url);
				string query = uri.Query;
				string realUrl = uri.Scheme + "://" + uri.DnsSafeHost + uri.AbsolutePath + (string.IsNullOrEmpty(query) ? ""
					: ("?" + HttpUtility.UrlPathEncode(uri.Query.Substring(1, uri.Query.Length - 1))));

				if (UrlFormatFunc != null)
				{
					realUrl = UrlFormatFunc(realUrl);
				}

				// 统一拨号换IP
				Redialer?.WaitforRedialFinish();

				driverService.WebDriver.Navigate().GoToUrl(realUrl);

				Thread.Sleep(_webDriverWaitTime);

				AfterNavigateFunc?.Invoke(driverService.WebDriver);

				Page page = new Page(request);
				page.SetRawText(driverService.WebDriver.PageSource);
				page.SetUrl(new PlainText(request.Url));
				page.SetTargetUrl(new PlainText(driverService.WebDriver.Url));
				page.Title = driverService.WebDriver.Title;

				ValidatePage(page);

				// 结束后要置空, 这个值存到Redis会导置无限循环跑单个任务
				request.PutExtra(Request.CycleTriedTimes, null);

				return page;
			}
			finally
			{
				_webDriverPool.ReturnToPool(driverService);
			}
		}

		public override void Dispose()
		{
			_webDriverPool?.CloseAll();
		}

		private void CheckInit()
		{
			if (_webDriverPool == null)
			{
				lock (this)
				{
					_webDriverPool = new WebDriverPool(_browser, ThreadNum, _option);
				}
			}
		}
	}
}