using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class XPathSelector : BaseElementSelector
	{
		private readonly string _xpath;
		private static readonly Regex AttributeXPathRegex = new Regex(@"@[\w\s-]+", RegexOptions.RightToLeft | RegexOptions.IgnoreCase);
		private readonly string _attribute;

		public XPathSelector(string xpathStr)
		{
			_xpath = xpathStr;
			//if (!string.IsNullOrEmpty(this.xpath))
			//{
			Match match = AttributeXPathRegex.Match(_xpath);
			if (!string.IsNullOrEmpty(match.Value) && _xpath.EndsWith(match.Value))
			{
				_attribute = match.Value.Replace("@", "");
				_xpath = _xpath.Replace("/" + match.Value, "");
			}
			//}
		}

		public override SelectedNode Select(HtmlAgilityPack.HtmlNode element)
		{
			var node = element.SelectSingleNode(_xpath);
			if (node != null)
			{
				return HasAttribute()
					? new SelectedNode
					{
						Result = node.Attributes.Contains(_attribute) ? node.Attributes[_attribute].Value?.Trim() : null,
						Type = ResultType.String
					}
					: new SelectedNode { Type = ResultType.Node, Result = node };
			}
			return null;
		}

		public override List<SelectedNode> SelectList(HtmlAgilityPack.HtmlNode element)
		{
			List<SelectedNode> result = new List<SelectedNode>();
			var nodes = element.SelectNodes(_xpath);
			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					if (!HasAttribute())
					{
						result.Add(new SelectedNode() { Type = ResultType.Node, Result = node });
					}
					else
					{
						var attr = node.Attributes[_attribute];
						if (attr != null)
						{
							result.Add(new SelectedNode() { Type = ResultType.String, Result = attr.Value?.Trim() });
						}
					}
				}
			}
			return result;
		}

		public override bool HasAttribute()
		{
			return !string.IsNullOrEmpty(_attribute);
		}
	}
}
