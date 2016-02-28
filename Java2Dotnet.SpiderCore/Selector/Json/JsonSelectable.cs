using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core.Selector.Json
{
	/// <summary>
	/// Parse json
	/// </summary>
	public class JsonSelectable : BaseSelectable
	{
		public JsonSelectable(string text, string padding)
		{
			string json = string.IsNullOrEmpty(padding) ? text : RemovePadding(text, padding);
			Elements = new List<string> { json };
		}

		public JsonSelectable(string text) : this(text, null)
		{
		}

		public JsonSelectable(List<string> nodes)
		{
			Elements = nodes;
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

		public ISelectable JsonPath(string jsonPath)
		{
			JsonPathSelector jsonPathSelector = new JsonPathSelector(jsonPath);
			return SelectList(jsonPathSelector) as ISelectable;
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
				return new JsonSelectable(resluts);
			}
			throw new SpiderExceptoin("Selector is null.");
		}

		public override IBaseSelectable SelectList(ISelector selector)
		{
			if (selector != null)
			{
				List<string> resluts = new List<string>();
				foreach (var selectedNode in Elements)
				{
					resluts.AddRange(selector.SelectList(selectedNode));
				}
				return new JsonSelectable(resluts);
			}

			throw new SpiderExceptoin("Selector is null.");
		}
	}
}