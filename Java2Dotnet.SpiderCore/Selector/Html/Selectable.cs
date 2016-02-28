//using System.Collections.Generic;
//using System.Linq;
//using HtmlAgilityPack;
//using Java2Dotnet.Spider.Core.Utils;

//namespace Java2Dotnet.Spider.Core.Selector.Html
//{
//	/// <summary>
//	/// Selectable.
//	/// </summary>
//	public class HtmlSelectable : AbstractSelectable
//	{
//		public HtmlSelectable(string text, string url)
//		{
//			HtmlDocument document = new HtmlDocument();
//			document.OptionAutoCloseOnEnd = true;
//			document.LoadHtml(text);

//			if (!string.IsNullOrEmpty(url))
//			{
//				FixAllRelativeHrefs(document, url);
//			}

//			Elements = new List<string> { document.DocumentNode.OuterHtml };
//		}

//		public HtmlSelectable(string node)
//		{
//			if (Elements == null)
//			{
//				Elements = new List<string> { node };
//			}
//			else
//			{
//				//Document = new List<SelectedNode>() { new SelectedNode() { Type = ResultType.Node, Result = document.DocumentNode } };
//				Elements.Add(node);
//			}
//		}

//		public HtmlSelectable(List<string> nodes)
//		{
//			Elements = nodes;
//		}

//		public override ISelectable Select(ISelector selector)
//		{
//			if (selector != null)
//			{
//				List<string> resluts = new List<string>();
//				foreach (var selectedNode in Elements)
//				{
//					resluts.Add(selector.Select(selectedNode));
//				}
//				return new HtmlSelectable(resluts);
//			}
//			throw new SpiderExceptoin("Selector is null.");
//		}

//		public override ISelectable SelectList(ISelector selector)
//		{
//			//var elementSelector = selector as IElementSelector;
//			if (selector != null)
//			{
//				List<string> resluts = new List<string>();
//				foreach (var selectedNode in Elements)
//				{
//					resluts.AddRange(selector.SelectList(selectedNode));
//				}
//				return new HtmlSelectable(resluts);
//			}
//			//return selector?.SelectList(GetFirstSourceText());
//			throw new SpiderExceptoin("Selector is null.");
//		}

//		public override IList<ISelectable> Nodes()
//		{
//			return Elements.Select(element => new HtmlSelectable(element)).Cast<ISelectable>().ToList();
//		}

//		private void FixAllRelativeHrefs(HtmlDocument document, string url)
//		{
//			var nodes = document.DocumentNode.SelectNodes("//a[not(starts-with(@href,'http') or starts-with(@href,'https'))]");
//			if (nodes != null)
//			{
//				foreach (var node in nodes)
//				{
//					if (node.Attributes["href"] != null)
//					{
//						node.Attributes["href"].Value = UrlUtils.CanonicalizeUrl(node.Attributes["href"].Value, url);
//					}
//				}
//			}
//		}
//	}
//}
