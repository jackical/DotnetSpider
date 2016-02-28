using System.Collections.Generic;
using HtmlAgilityPack;

namespace Java2Dotnet.Spider.Core.Selector.Html
{
	public abstract class BaseSelector : ISelector
	{
		public virtual string Select(string text)
		{
			if (text != null)
			{
				HtmlDocument document = new HtmlDocument();
				document.OptionAutoCloseOnEnd = true;
				document.LoadHtml(text);
				return Select(document.DocumentNode);
			}
			return null;
		}

		public virtual List<string> SelectList(string text)
		{
			if (text != null)
			{
				HtmlDocument document = new HtmlDocument();
				document.OptionAutoCloseOnEnd = true;
				document.LoadHtml(text);
				return SelectList(document.DocumentNode);
			}
			else
			{
				return new List<string>();
			}
		}

		public abstract bool HasAttribute();
		public abstract string Select(HtmlNode element);
		public abstract List<string> SelectList(HtmlNode element);
	}
}