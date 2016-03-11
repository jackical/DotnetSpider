﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace Java2Dotnet.Spider.Core.Selector
{
	internal class CssHtmlSelector : BaseHtmlSelector
	{
		private readonly string _selectorText;
		private readonly string _attrName;

		public CssHtmlSelector(string selectorText)
		{
			_selectorText = selectorText;
		}

		public CssHtmlSelector(string selectorText, string attrName)
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

		public override dynamic Select(HtmlNode element)
		{
			IList<HtmlNode> elements = element.QuerySelectorAll(_selectorText);
			if (elements != null && elements.Count > 0)
			{
				if (string.IsNullOrEmpty(_attrName))
				{
					return elements[0];
				}
				else
				{
					return elements[0].Attributes[_attrName]?.Value;
				}
			}
			return null;
		}

		public override List<dynamic> SelectList(HtmlNode element)
		{
			return element.QuerySelectorAll(_selectorText).Cast<dynamic>().ToList();
		}

		public override bool HasAttribute()
		{
			return _attrName != null;
		}
	}
}
