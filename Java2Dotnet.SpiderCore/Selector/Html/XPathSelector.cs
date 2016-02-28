﻿using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Java2Dotnet.Spider.Core.Selector.Html
{
	public class XPathSelector : BaseSelector
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

		public override string Select(HtmlAgilityPack.HtmlNode element)
		{
			var node = element.SelectSingleNode(_xpath);
			if (node != null)
			{
				return HasAttribute() ? (node.Attributes.Contains(_attribute) ? node.Attributes[_attribute].Value?.Trim() : null) : node.OuterHtml?.Trim();
			}
			return null;
		}

		public override List<string> SelectList(HtmlAgilityPack.HtmlNode element)
		{
			List<string> result = new List<string>();
			var nodes = element.SelectNodes(_xpath);
			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					if (!HasAttribute())
					{
						result.Add(node.OuterHtml);
					}
					else
					{
						var attr = node.Attributes[_attribute];
						if (attr != null)
						{
							result.Add(attr.Value?.Trim());
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
