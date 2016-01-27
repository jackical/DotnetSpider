using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Java2Dotnet.Spider.Core.Selector
{
	public abstract class AbstractSelectable : ISelectable
	{
		protected static readonly ILog Logger = LogManager.GetLogger("AbstractSelectable");

		public List<SelectedNode> Elements { get; protected set; } = new List<SelectedNode>();

		public abstract ISelectable Select(ISelector selector);
		public abstract ISelectable SelectList(ISelector selector);
		public abstract IList<ISelectable> Nodes();

		public ISelectable Css(string selector)
		{
			CssSelector cssSelector = Selectors.Css(selector);
			return Select(cssSelector);
		}

		public ISelectable Css(string selector, string attrName)
		{
			var cssSelector = Selectors.Css(selector, attrName);
			return Select(cssSelector);
		}

		public ISelectable SmartContent()
		{
			SmartContentSelector smartContentSelector = Selectors.SmartContent();
			return Select(smartContentSelector);
		}

		public ISelectable Links()
		{
			return XPath(".//a/@href");
		}

		public ISelectable XPath(string xpath)
		{
			XPathSelector xpathSelector = Selectors.XPath(xpath);
			return SelectList(xpathSelector);
		}

		public ISelectable Regex(string regex)
		{
			RegexSelector regexSelector = Selectors.Regex(regex);
			return Select(regexSelector);
		}

		public ISelectable Regex(string regex, int group)
		{
			RegexSelector regexSelector = Selectors.Regex(regex, group);
			return Select(regexSelector);
		}

		public ISelectable JsonPath(string jsonPath)
		{
			JsonPathSelector jsonPathSelector = new JsonPathSelector(jsonPath);
			return SelectList(jsonPathSelector);
		}

		public dynamic Value
		{
			get
			{
				if (Elements == null || Elements.Count == 0)
				{
					return null;
				}

				if (Elements.Count == 1)
				{
					return Elements[0].ToString();
				}

				return Elements.Select(selectedNode => selectedNode.ToString()).ToList();
			}
		}
	}
}