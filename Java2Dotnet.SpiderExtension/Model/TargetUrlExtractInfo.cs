using System.Collections.Generic;
using System.Text.RegularExpressions;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Model.Formatter;

namespace Java2Dotnet.Spider.Extension.Model
{
	public class TargetUrlExtractInfo
	{
		public List<Regex> Patterns { get; set; } = new List<Regex>();
		public IObjectFormatter TargetUrlFormatter { get; set; }
		public ISelector TargetUrlRegionSelector { get; set; }
	}
}
