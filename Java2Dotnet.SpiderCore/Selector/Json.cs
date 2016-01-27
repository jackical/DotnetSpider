using System.Collections.Generic;
using System.Linq;
using Java2Dotnet.Spider.Core.Utils;
using Newtonsoft.Json;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// Parse json
	/// </summary>
	public class Json : AbstractSelectable
	{
		public Json(string text, string padding = null)
		{
			string json = RemovePadding(text, padding);
			Elements = new List<SelectedNode>() { new SelectedNode() { Type = ResultType.Json, Result = JsonConvert.DeserializeObject(json) } };
		}

		public Json(List<SelectedNode> nodes)
		{
			Elements = nodes;
		}

		public Json(SelectedNode node)
		{
			if (node.Type != ResultType.Json)
			{
				Elements = new List<SelectedNode>() { new SelectedNode() { Type = ResultType.String, Result = node.ToString() } };
			}
			else
			{
				Elements = new List<SelectedNode>() { node };
			}
		}

		/// <summary>
		/// Remove padding for JSONP
		/// </summary>
		/// <param name="text"></param>
		/// <param name="padding"></param>
		/// <returns></returns>
		public string RemovePadding(string text, string padding)
		{
			if (string.IsNullOrEmpty(padding))
			{
				return text;
			}

			XTokenQueue tokenQueue = new XTokenQueue(text);
			tokenQueue.ConsumeWhitespace();
			tokenQueue.Consume(padding);
			tokenQueue.ConsumeWhitespace();
			return tokenQueue.ChompBalancedNotInQuotes('(', ')');
		}

		//public T ToObject<T>()
		//{
		//	if (GetFirstSourceText() == null)
		//	{
		//		return default(T);
		//	}
		//	return JsonConvert.DeserializeObject<T>(GetFirstSourceText());
		//}

		//public List<T> ToList<T>()
		//{
		//	if (GetFirstSourceText() == null)
		//	{
		//		return null;
		//	}
		//	return JsonConvert.DeserializeObject<List<T>>(GetFirstSourceText());
		//}

		public override IList<ISelectable> Nodes()
		{
			//return Elements.Select(element => new Json(element)).Cast<ISelectable>().ToList();
			if (Elements == null || Elements.Count == 0)
			{
				return new List<ISelectable>();
			}

			return Elements.Select(element => new Json(element)).Cast<ISelectable>().ToList();
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
				return new Json(resluts);
			}
			throw new SpiderExceptoin("Selector is null.");
		}

		public override ISelectable SelectList(ISelector selector)
		{
			if (selector != null)
			{
				List<SelectedNode> resluts = new List<SelectedNode>();
				foreach (var selectedNode in Elements)
				{
					resluts.AddRange(selector.SelectList(selectedNode));
				}
				return new Json(resluts);
			}

			throw new SpiderExceptoin("Selector is null.");
		}
	}
}