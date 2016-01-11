using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Object storing extracted result and urls to fetch. 
	/// Not thread safe 
	/// Main method：                                                
	/// {@link #GetUrl()} get url of current page                   
	/// {@link #GetHtml()}  get content of current page                  
	/// {@link #PutField(String, Object)}  save extracted result             
	/// {@link #GetResultItems()} get extract results to be used in {@link us.codecraft.webmagic.pipeline.Pipeline} 
	/// {@link #AddTargetRequests(java.util.List)} {@link #addTargetRequest(String)} add urls to fetch                  
	/// </summary>
	public class Page
	{
		public const string Images = "580c9065-0f44-47e9-94ea-b172d5a730c0";
		private readonly Request _request;

		private readonly ResultItems _resultItems = new ResultItems();

		private Html _html;

		private Json _json;

		private string _rawText;

		private string _url;

		private string _targetUrl;

		private int _statusCode;

		private bool _needCycleRetry;

		private readonly HashSet<Request> _targetRequests = new HashSet<Request>();

		public Page(Request request)
		{
			_request = request;
			_resultItems.Request = request;
		}

		public Page SetSkip(bool skip)
		{
			_resultItems.IsSkip = skip;
			return this;
		}

		/// <summary>
		/// Store extract results
		/// </summary>
		/// <param name="key"></param>
		/// <param name="field"></param>
		public void PutField(string key, dynamic field)
		{
			_resultItems.Put(key, field);
		}

		/// <summary>
		/// Get html content of page
		/// </summary>
		/// <returns></returns>
		public Html GetHtml()
		{
			return _html ?? (_html = new Html(_rawText, _request.Url));
		}

		/// <summary>
		/// Get json content of page
		/// </summary>
		/// <returns></returns>
		public Json GetJson()
		{
			return _json ?? (_json = new Json(_rawText));
		}

		public void SetHtml(Html html)
		{
			_html = html;
		}

		public HashSet<Request> GetTargetRequests()
		{
			return _targetRequests;
		}

		/// <summary>
		/// Add urls to fetch
		/// </summary>
		/// <param name="requests"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddTargetRequests(IList<string> requests)
		{
			foreach (string s in requests)
			{
				if (string.IsNullOrEmpty(s) || s.Equals("#") || s.StartsWith("javascript:"))
				{
					continue;
				}
				string s1 = UrlUtils.CanonicalizeUrl(s, _url.ToString());
				_targetRequests.Add(new Request(s1, _request.NextDepth(), _request?.Extras));
			}
		}

		/// <summary>
		/// Add urls to fetch
		/// </summary>
		/// <param name="requests"></param>
		/// <param name="priority"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddTargetRequests(IList<string> requests, int priority)
		{
			foreach (string s in requests)
			{
				if (string.IsNullOrEmpty(s) || s.Equals("#") || s.StartsWith("javascript:"))
				{
					continue;
				}
				string s1 = UrlUtils.CanonicalizeUrl(s, _url.ToString());
				Request request = new Request(s1, _request.NextDepth(), _request?.Extras) { Priority = priority };
				_targetRequests.Add(request);
			}
		}


		/// <summary>
		/// Add url to fetch
		/// </summary>
		/// <param name="requestString"></param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddTargetRequest(string requestString)
		{
			if (string.IsNullOrEmpty(requestString) || requestString.Equals("#"))
			{
				return;
			}

			requestString = UrlUtils.CanonicalizeUrl(requestString, _url.ToString());
			_targetRequests.Add(new Request(requestString, _request.NextDepth(), _request?.Extras));
		}

		/// <summary>
		/// Add requests to fetch
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddTargetRequest(Request request)
		{
			_targetRequests.Add(request);
		}

		/// <summary>
		/// Get url of current page
		/// </summary>
		/// <returns></returns>
		public string GetUrl()
		{
			return _url;
		}

		/// <summary>
		/// Get url of current page
		/// </summary>
		/// <returns></returns>
		public string GetTargetUrl()
		{
			return _targetUrl;
		}

		public void SetUrl(string url)
		{
			_url = url;
		}

		public string Title { get; set; }

		public void SetTargetUrl(string url)
		{
			_targetUrl = url;
		}

		/// <summary>
		/// Get request of current page
		/// </summary>
		/// <returns></returns>
		public Request GetRequest()
		{
			return _request;
		}

		public bool IsNeedCycleRetry()
		{
			return _needCycleRetry;
		}

		public void SetNeedCycleRetry(bool needCycleRetry)
		{
			_needCycleRetry = needCycleRetry;
		}

		//public void SetRequest(Request request)
		//{
		//	_request = request;
		//	_resultItems.Request = request;
		//}

		public ResultItems GetResultItems()
		{
			return _resultItems;
		}

		public int GetStatusCode()
		{
			return _statusCode;
		}

		public void SetStatusCode(int statusCode)
		{
			_statusCode = statusCode;
		}

		public string GetRawText()
		{
			return _rawText;
		}

		public Page SetRawText(string rawText)
		{
			_rawText = rawText;
			return this;
		}

		public bool MissTargetUrls { get; set; }

		public override string ToString()
		{
			return "Page{" +
					"request=" + _request +
					", resultItems=" + _resultItems +
					", rawText='" + _rawText + '\'' +
					", url=" + _url +
					", statusCode=" + _statusCode +
					", targetRequests=" + _targetRequests +
					'}';
		}
	}
}
