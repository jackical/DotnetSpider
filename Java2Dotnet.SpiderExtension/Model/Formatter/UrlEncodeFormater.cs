using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Java2Dotnet.Spider.Extension.Model.Formatter
{
	[AttributeUsage(AttributeTargets.Property)]
	public class UrlEncodeFormater : Formatter
	{
		public override string Name { get; internal set; } = "UrlEncodeFormater";

		public string Encoding { get; set; }

		public override string Formate(string value)
		{
			return HttpUtility.UrlEncode(value, System.Text.Encoding.GetEncoding(Encoding));
		}
	}
}
