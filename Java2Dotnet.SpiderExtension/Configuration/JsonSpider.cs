using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Model;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension.Configuration
{
	public class JsonSpider
	{
		public string SpiderName { get; set; }
		public int ThreadNum { get; set; } = 1;
		public int Deep { get; set; } = int.MaxValue;
		public int EmptySleepTime { get; set; } = 15000;
		public int CachedSize { get; set; }
		public Downloader Downloader { get; set; }
		public Site Site { get; set; }
		public bool NeedRedial { get; set; }
		public NetworkValidater NetworkValidater { get; set; }
		public Redialer Redialer { get; set; }
		public AdvanceStartUrlsProcess AdvanceStartUrlsProcess { get; set; }
		/// <summary>
		/// 对下载完后的数据做处理: Html中截取Json或者Json中截取Html
		/// 如： 
		/// Regex: xxxx
		/// Replace: xxxx,xxxx
		/// 则先使用正则匹配, 再Replace掉无用数据
		/// </summary>
		public List<ISubContenRule> SubHtmlRules { get; set; }
		public List<ITargetUrlFormater> TargetUrlFormaters { get; set; }
		public Pipeline Pipeline { get; set; }

		public List<JObject> Entities { get; set; }
		public string Corporation { get; set; }
		public string ValidationReportTo { get; set; }
	}


	public enum PipelineType
	{
		MongoDb,
		MySql,
		MsSql
	}

	public class Site
	{
		public bool IsUseGzip { get; set; }
		public List<string> StartUrls { get; set; }

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
		public string Domain { get; set; }

		public Dictionary<string, string> Headers { get; set; }

	}

	public class Pipeline
	{
		public PipelineType PipelineType { get; set; }
		public JObject Arguments { get; set; }
	}

	public class RegexSubRule : ISubContenRule
	{
		public string Pattern { get; }

		public RegexSubRule(string pattern)
		{
			Pattern = pattern;
		}

		public string Sub(string content)
		{
			//todo
			return null;
		}
	}

	public interface ISubContenRule
	{
		string Sub(string content);
	}

	public interface ITargetUrlFormater
	{
		string Formate(string url);
	}

	public class AdvanceStartUrlsProcess
	{
		/// <summary>
		/// 数据来源表名, 需要Schema/数据库名
		/// </summary>
		public string TableName { get; set; }

		/// <summary>
		/// 对表的筛选
		/// 如: cdate='2016-03-01', isUsed=true
		/// 代码用户可以直接写SQL的where部分, 普通用户提供界面选择列，大于等于小于，比较值
		/// 这是最终拼接好的where部分, 所以不用List存
		/// </summary>
		public string Filter { get; set; }

		/// <summary>
		/// 用于拼接Url所需要的列
		/// </summary>
		public List<Columns> Columns { get; set; }

		/// <summary>
		/// 拼接Url的方式, 会把Columns对应列的数据传入
		/// https://s.taobao.com/search?q={0},s=0;
		/// </summary>
		public string Formater { get; set; }

		/// <summary>
		/// 替换拼接后的Url的某些值
		/// 如: 数据库中存的数据是 http://list.jd.com/1316-1385-1410.html, 需要拼接成:http://list.jd.com/1316-1385-1410,page=1,JL=6_0_0
		/// </summary>
		public List<string> Replace { get; set; }

		public void Build(Site site)
		{
		}
	}

	public class Columns
	{
		public string Name { get; set; }
		public bool NeedUrlEncoding { get; set; }
		public Encoding UrlEncoding { get; set; }
	}

	/// <summary>
	/// 配置下载器
	/// Http, WebDriver, Fiddler
	/// </summary>
	public class Downloader
	{
		public bool NeedChangeIp { get; set; }
		public string Name { get; set; }
		public Browser Browser { get; set; }

		/// <summary>
		/// user:aaa;password:bbb;interface:ccc
		/// sshhost:192.168.1.1;sshuser:aaa;sshpass:cccc;
		/// </summary>
		public JObject RedialArguments { get; set; }

		/// <summary>
		/// Contains("anti_Spider")
		/// UrlContains("anti_Spider")
		/// </summary>
		public string DownloadValidation { get; set; }

		public Login Login { get; set; }
	}

	public class Login
	{
		public string Name { get; set; }

		/// <summary>
		/// 由LoginInterface值确定这段Json要转换的对象
		/// 如 LoginInterface为Common, 则把此LoginArguments.ToOjbect<CommonLogin>().
		/// CommonLogin{ UserSelector, PasswordSelector, SubmitSelector }
		/// </summary>
		public JObject Arguments { get; set; }
	}

	public enum NetworkValidater
	{
		DefalutNetworkValidater,
		VpsNetworkValidater
	}

	public enum Redialer
	{
		Adsl,
		H3C
	}

	/// <summary>
	/// 所支持的浏览器
	/// </summary>
	public enum Browser
	{
		Firefox,
		Chrome,
		Phantomjs,
		Ie
	}
}
