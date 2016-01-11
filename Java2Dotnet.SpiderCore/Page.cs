using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Object storing extracted result and urls to fetch. 
	/// </summary>
	public class Page
	{
		public const string Images = "580c9065-0f44-47e9-94ea-b172d5a730c0";

		private readonly Request _request;
		private readonly ResultItems _resultItems = new ResultItems();
		private Html _html;
		private Json _json;
		private readonly HashSet<Request> _targetRequests = new HashSet<Request>();

		public Page(Request request)
		{
			_request = request;
			_resultItems.Request = request;
		}

		public bool IsSkip
		{
			set { _resultItems.IsSkip = value; }
		}

		/// <summary>
		/// Store extract results
		/// </summary>
		/// <param name="key"></param>
		/// <param name="field"></param>
		public void AddResultItem(string key, dynamic field)
		{
			_resultItems.AddResultItem(key, field);
		}

		/// <summary>
		/// Get html content of page
		/// </summary>
		/// <returns></returns>
		public Html GetHtml()
		{
			return _html ?? (_html = new Html(RawText, _request.Url));
		}

		/// <summary>
		/// Get json content of page
		/// </summary>
		/// <returns></returns>
		public Json GetJson()
		{
			return _json ?? (_json = new Json(RawText));
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
				string s1 = UrlUtils.CanonicalizeUrl(s, Url);
				_targetRequests.Add(new Request(s1, _request.NextDepth, _request?.Extras));
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
				string s1 = UrlUtils.CanonicalizeUrl(s, Url);
				Request request = new Request(s1, _request.NextDepth, _request?.Extras) { Priority = priority };
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

			requestString = UrlUtils.CanonicalizeUrl(requestString, Url);
			_targetRequests.Add(new Request(requestString, _request.NextDepth, _request?.Extras));
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
		/// Url of current page
		/// </summary>
		/// <returns></returns>
		public string Url { get; set; }

		/// <summary>
		/// Get url of current page
		/// </summary>
		/// <returns></returns>
		public string TargetUrl { get; set; }

		public string Title { get; set; }

		/// <summary>
		/// Get request of current page
		/// </summary>
		/// <returns></returns>
		public Request Request => _request;

		public bool IsNeedCycleRetry { get; set; }

		//public void SetRequest(Request request)
		//{
		//	_request = request;
		//	_resultItems.Request = request;
		//}

		public ResultItems ResultItems => _resultItems;

		public int StatusCode { get; set; }

		public string RawText { get; set; }
 
		public bool MissTargetUrls { get; set; }

		public override string ToString()
		{
			return "Page{" +
					"request=" + _request +
					", resultItems=" + _resultItems +
					", rawText='" + RawText + '\'' +
					", url=" + Url +
					", statusCode=" + StatusCode +
					", targetRequests=" + _targetRequests +
					'}';
		}
	}
}
