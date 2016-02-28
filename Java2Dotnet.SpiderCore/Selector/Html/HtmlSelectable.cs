using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core.Selector.Html
{
	public class HtmlSelectable : BaseSelectable, ISelectable
	{
		public ISelectable Css(string selector)
		{
			CssSelector cssSelector = Selectors.Css(selector);
			return Select(cssSelector) as ISelectable;
		}

		public ISelectable Css(string selector, string attrName)
		{
			var cssSelector = Selectors.Css(selector, attrName);
			return Select(cssSelector) as ISelectable;
		}

		public ISelectable SmartContent()
		{
			SmartContentSelector smartContentSelector = Selectors.SmartContent();
			return Select(smartContentSelector) as ISelectable;
		}

		public ISelectable Links()
		{
			return XPath(".//a/@href");
		}

		public ISelectable XPath(string xpath)
		{
			XPathSelector xpathSelector = Selectors.XPath(xpath);
			return SelectList(xpathSelector) as ISelectable;
		}

		public HtmlSelectable(string text, string url)
		{
			HtmlDocument document = new HtmlDocument();
			document.OptionAutoCloseOnEnd = true;
			document.LoadHtml(text);

			if (!string.IsNullOrEmpty(url))
			{
				FixAllRelativeHrefs(document, url);
			}

			Elements = new List<string> { document.DocumentNode.OuterHtml };
		}

		public HtmlSelectable(string text) : this(text, null)
		{
		}

		public HtmlSelectable(List<string> nodes)
		{
			Elements = nodes;
		}

		public override IBaseSelectable Select(ISelector selector)
		{
			if (selector != null)
			{
				List<string> resluts = new List<string>();
				foreach (var selectedNode in Elements)
				{
					resluts.Add(selector.Select(selectedNode));
				}
				return new HtmlSelectable(resluts);
			}
			throw new SpiderExceptoin("Selector is null.");
		}

		public override IBaseSelectable SelectList(ISelector selector)
		{
			//var elementSelector = selector as IElementSelector;
			if (selector != null)
			{
				List<string> resluts = new List<string>();
				foreach (var selectedNode in Elements)
				{
					resluts.AddRange(selector.SelectList(selectedNode));
				}
				return new HtmlSelectable(resluts);
			}
			//return selector?.SelectList(GetFirstSourceText());
			throw new SpiderExceptoin("Selector is null.");
		}

		public virtual IList<ISelectable> Nodes()
		{
			return Elements.Select(element => new HtmlSelectable(element)).Cast<ISelectable>().ToList();
		}

		private void FixAllRelativeHrefs(HtmlDocument document, string url)
		{
			var nodes = document.DocumentNode.SelectNodes("//a[not(starts-with(@href,'http') or starts-with(@href,'https'))]");
			if (nodes != null)
			{
				foreach (var node in nodes)
				{
					if (node.Attributes["href"] != null)
					{
						node.Attributes["href"].Value = UrlUtils.CanonicalizeUrl(node.Attributes["href"].Value, url);
					}
				}
			}
		}
	}
}