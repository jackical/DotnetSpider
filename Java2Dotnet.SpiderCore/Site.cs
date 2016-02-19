using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Java2Dotnet.Spider.Core.Proxy;

namespace Java2Dotnet.Spider.Core
{
	/// <summary>
	/// Object contains setting for crawler.
	/// </summary>
	public class Site
	{
		//private readonly Dictionary<string, string> _cookies = new Dictionary<string, string>();

		private readonly List<Request> _startRequests = new List<Request>();
		private Dictionary<string, string> _headers;
		private ProxyPool _httpProxyPool = new ProxyPool();
		private string _domain;

		public Dictionary<string, string> Headers
		{
			get { return _headers ?? (_headers = new Dictionary<string, string>()); }
			set { _headers = value; }
		}

		/// <summary>
		/// User agent
		/// </summary>
		public string UserAgent { get; set; }

		/// <summary>
		/// User agent
		/// </summary>
		public string Accept { get; set; }

		/// <summary>
		/// Set the domain of site.
		/// </summary>
		/// <returns></returns>
		public string Domain
		{
			get
			{
				if (_domain == null && StartRequests != null && StartRequests.Count > 0)
				{
					_domain = StartRequests[0].Url.Host;
				}
				return _domain;
			}
			set
			{
				_domain = value;
			}
		}

		/// <summary>
		/// Set charset of page manually. 
		/// When charset is not set or set to null, it can be auto detected by Http header.
		/// </summary>
		public Encoding Encoding { get; set; } = Encoding.Default;

		/// <summary>
		/// Set or Get timeout for downloader in ms
		/// </summary>
		public int Timeout { get; set; } = 5000;

		/// <summary>
		/// Get or Set acceptStatCode. 
		/// When status code of http response is in acceptStatCodes, it will be processed. 
		/// {200} by default. 
		/// It is not necessarily to be set.
		/// </summary>
		public HashSet<int> AcceptStatCode { get; set; } = new HashSet<int> { 200 };

		public List<Request> StartRequests => _startRequests;

		/// <summary>
		/// Set the interval between the processing of two pages. 
		/// Time unit is micro seconds. 
		/// </summary>
		public int SleepTime { get; set; } = 500;

		/// <summary>
		/// Get or Set retry times immediately when download fail, 5 by default.
		/// </summary>
		/// <returns></returns>
		public int RetryTimes { get; set; } = 5;

		/// <summary>
		/// When cycleRetryTimes is more than 0, it will add back to scheduler and try download again. 
		/// </summary>
		public int CycleRetryTimes { get; set; } = 20;

		public string Cookie { get; set; }

		/// <summary>
		/// Set or Get up httpProxy for this site
		/// </summary>
		public string HttpProxy { get; set; }

		/// <summary>
		/// Whether use gzip.  
		/// Default is true, you can set it to false to disable gzip.
		/// </summary>
		public bool IsUseGzip { get; set; }

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void ClearStartRequests()
		{
			_startRequests.Clear();
			GC.Collect();
		}

		/// <summary>
		/// Add a url to start request. 
		/// </summary>
		/// <param name="startUrl"></param>
		/// <returns></returns>
		public Site AddStartUrl(string startUrl)
		{
			return AddStartRequest(new Request(startUrl, 1, null));
		}

		/// <summary>
		/// Add a url to start request. 
		/// </summary>
		/// <param name="startUrl"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public Site AddStartUrl(string startUrl, IDictionary<string, object> data)
		{
			return AddStartRequest(new Request(startUrl, 1, data));
		}

		public Site AddStartUrls(IList<string> startUrls)
		{
			foreach (var url in startUrls)
			{
				AddStartUrl(url);
			}

			return this;
		}

		public Site AddStartUrls(IDictionary<string, IDictionary<string, object>> startUrls)
		{
			foreach (var entry in startUrls)
			{
				AddStartUrl(entry.Key, entry.Value);
			}

			return this;
		}

		/// <summary>
		/// Add a request.
		/// </summary>
		/// <param name="startRequest"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public Site AddStartRequest(Request startRequest)
		{
			_startRequests.Add(startRequest);
			if (Domain == null)
			{
				Domain = startRequest.Url.Host;
			}
			return this;
		}

		/// <summary>
		/// Put an Http header for downloader. 
		/// </summary>
		public Site AddHeader(string key, string value)
		{
			if (Headers.ContainsKey(key))
			{
				Headers[key] = value;
			}
			else
			{
				Headers.Add(key, value);
			}
			return this;
		}

		public override bool Equals(object o)
		{
			if (this == o) return true;
			if (o == null || GetType() != o.GetType()) return false;

			Site site = (Site)o;

			if (CycleRetryTimes != site.CycleRetryTimes) return false;
			if (RetryTimes != site.RetryTimes) return false;
			if (SleepTime != site.SleepTime) return false;
			if (Timeout != site.Timeout) return false;
			if (!AcceptStatCode?.Equals(site.AcceptStatCode) ?? site.AcceptStatCode != null)
				return false;
			if (!Encoding?.Equals(site.Encoding) ?? site.Encoding != null) return false;
			if (!Cookie?.Equals(site.Cookie) ?? site.Cookie != null)
				return false;
			if (!Domain?.Equals(site.Domain) ?? site.Domain != null) return false;
			if (!_headers?.Equals(site._headers) ?? site._headers != null) return false;
			if (!_startRequests?.Equals(site._startRequests) ?? site._startRequests != null)
				return false;
			if (!UserAgent?.Equals(site.UserAgent) ?? site.UserAgent != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			int result = Domain?.GetHashCode() ?? 0;
			result = 31 * result + (UserAgent?.GetHashCode() ?? 0);
			result = 31 * result + (Cookie?.GetHashCode() ?? 0);
			result = 31 * result + (Encoding?.GetHashCode() ?? 0);
			result = 31 * result + (_startRequests?.GetHashCode() ?? 0);
			result = 31 * result + SleepTime;
			result = 31 * result + RetryTimes;
			result = 31 * result + CycleRetryTimes;
			result = 31 * result + Timeout;
			result = 31 * result + (AcceptStatCode?.GetHashCode() ?? 0);
			result = 31 * result + (_headers?.GetHashCode() ?? 0);
			return result;
		}

		public override string ToString()
		{
			return "Site{" +
					"domain='" + Domain + '\'' +
					", userAgent='" + UserAgent + '\'' +
					", cookies=" + Cookie +
					", charset='" + Encoding + '\'' +
					", startRequests=" + _startRequests +
					", sleepTime=" + SleepTime +
					", retryTimes=" + RetryTimes +
					", cycleRetryTimes=" + CycleRetryTimes +
					", timeOut=" + Timeout +
					", acceptStatCode=" + AcceptStatCode +
					", headers=" + _headers +
					'}';
		}

		/// <summary>
		/// add http proxy , string[0]:ip, string[1]:port
		/// </summary>
		/// <param name="httpProxyList"></param>
		/// <returns></returns>
		public Site AddHttpProxies(List<string[]> httpProxyList)
		{
			_httpProxyPool = new ProxyPool(httpProxyList);
			return this;
		}

		public bool HttpProxyPoolEnable => _httpProxyPool.Enable;

		public HttpHost GetHttpProxyFromPool()
		{
			return _httpProxyPool.GetProxy();
		}

		public void ReturnHttpProxyToPool(HttpHost proxy, int statusCode)
		{
			_httpProxyPool.ReturnProxy(proxy, statusCode);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reuseInterval">Re use interval time</param>
		/// <returns></returns>
		public Site SetProxyReuseInterval(int reuseInterval)
		{
			_httpProxyPool.SetReuseInterval(reuseInterval);
			return this;
		}
	}
}