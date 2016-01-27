using System.Collections.Generic;
using HtmlAgilityPack;

namespace Java2Dotnet.Spider.Core.Selector
{
	public abstract class BaseElementSelector : ISelector
	{
		public virtual SelectedNode Select(SelectedNode text)
		{
			if (text != null)
			{
				if (text.Type == ResultType.Node)
				{
					return Select(text.Result as HtmlNode);
				}
				else
				{
					HtmlDocument document = new HtmlDocument();
					document.OptionAutoCloseOnEnd = true;
					document.LoadHtml(text.ToString());
					return Select(document.DocumentNode);
				}
			}
			return null;
		}

		public virtual List<SelectedNode> SelectList(SelectedNode text)
		{
			if (text != null)
			{
				if (text.Type == ResultType.Node)
				{
					return SelectList(text.Result as HtmlNode);
				}
				else
				{
					HtmlDocument document = new HtmlDocument();
					document.OptionAutoCloseOnEnd = true;
					document.LoadHtml(text.ToString());
					return SelectList(document.DocumentNode);
				}
			}
			else
			{
				return new List<SelectedNode>();
			}
		}

		public abstract bool HasAttribute();
		public abstract SelectedNode Select(HtmlNode element);
		public abstract List<SelectedNode> SelectList(HtmlNode element);
	}
}