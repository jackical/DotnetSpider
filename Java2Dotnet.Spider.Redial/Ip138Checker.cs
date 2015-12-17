using System.Net;
using System.Text.RegularExpressions;

namespace Java2Dotnet.Spider.Redial
{
	public class Ip138Checker : IIpChecker
	{
		private static readonly Regex IpAddressRegex = new Regex(@"^\[((?:(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d)))\.){3}(?:25[0-5]|2[0-4]\d|((1\d{2})|([1-9]?\d))))\]$");

		public string GetIp()
		{
			WebClient client = new WebClient();
			string html = client.DownloadString("www.ip138.com");
			var match = IpAddressRegex.Match(html);
			return match.Value;
		}
	}
}
