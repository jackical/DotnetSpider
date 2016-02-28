using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Java2Dotnet.Spider.Core.Selector.Html
{
	public class CssSelector : BaseSelector
	{
		private readonly string _selectorText;
		private readonly string _attrName;

		public CssSelector(string selectorText)
		{
			_selectorText = selectorText;
		}

		public CssSelector(string selectorText, string attrName)
		{
			_selectorText = selectorText;
			_attrName = attrName;
		}

		protected string GetText(HtmlNode element)
		{
			StringBuilder accum = new StringBuilder();
			foreach (var node in element.ChildNodes)
			{
				if (node is HtmlTextNode)
				{
					accum.Append(node.InnerText);
				}
			}
			return accum.ToString();
		}

		public override string Select(HtmlNode element)
		{
			IList<HtmlNode> elements = element.QuerySelectorAll(_selectorText);
			if (elements != null && elements.Count > 0)
			{
				return GetValue(elements[0]);
			}
			return null;
		}

		public override List<string> SelectList(HtmlNode element)
		{
			return element.QuerySelectorAll(_selectorText).Select(GetValue).ToList();
		}

		public override bool HasAttribute()
		{
			return _attrName != null;
		}

		private string GetValue(HtmlNode element)
		{
			if (_attrName == null)
			{
				return element.OuterHtml;
			}
			else if ("innerhtml".Equals(_attrName.ToLower()))
			{
				return element.InnerHtml;
			}
			else if ("text".Equals(_attrName.ToLower()))
			{
				return GetText(element);
			}
			//check
			//else if ("allText".equalsIgnoreCase(attrName)) {
			//	return element.text();
			//} 
			else
			{
				return element.Attributes[_attrName].Value;
			}
		}
	}
}
