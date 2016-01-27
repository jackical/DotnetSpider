using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Selectable html.
	/// </summary>
	public class Html : AbstractSelectable
	{
		public Html(string text, Uri url)
		{
			try
			{
				HtmlDocument document = new HtmlDocument();
				document.OptionAutoCloseOnEnd = true;
				document.LoadHtml(text);

				if (url != null)
				{
					FixAllRelativeHrefs(document, url.ToString());
				}

				Elements=new List<SelectedNode>() { new SelectedNode() { Type = ResultType.Node, Result = document.DocumentNode } };
			}
			catch (Exception e)
			{
				Logger.Warn("parse document error ", e);
			}
		}

		public Html(string text, string url = null) : this(text, string.IsNullOrEmpty(url) ? null : new Uri(url))
		{
		}

		public Html(SelectedNode node)
		{
			if (node.Type != ResultType.Node)
			{
				HtmlDocument document = new HtmlDocument();
				document.OptionAutoCloseOnEnd = true;
				document.LoadHtml(node.Result.ToString());

				Elements = new List<SelectedNode>() { new SelectedNode() { Type = ResultType.Node, Result = document.DocumentNode } };
			}
			else
			{
				//Document = new List<SelectedNode>() { new SelectedNode() { Type = ResultType.Node, Result = document.DocumentNode } };
				Elements.Add(node);
			}
		}

		public Html(List<SelectedNode> nodes)
		{
			Elements = nodes;
		}

		public override ISelectable Select(ISelector selector)
		{
			if (selector != null)
			{
				List<SelectedNode> resluts = new List<SelectedNode>();
				foreach (var selectedNode in Elements)
				{
					resluts.Add(selector.Select(selectedNode));
				}
				return new Html(resluts);
			}
			throw new SpiderExceptoin("Selector is null.");
		}

		public override ISelectable SelectList(ISelector selector)
		{
			//var elementSelector = selector as IElementSelector;
			if (selector != null)
			{
				List<SelectedNode> resluts = new List<SelectedNode>();
				foreach (var selectedNode in Elements)
				{
					resluts.AddRange(selector.SelectList(selectedNode));
				}
				return new Html(resluts);
			}
			//return selector?.SelectList(GetFirstSourceText());
			throw new SpiderExceptoin("Selector is null.");
		}

		public override IList<ISelectable> Nodes()
		{
			return Elements.Select(element => new Html(element)).Cast<ISelectable>().ToList();
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
