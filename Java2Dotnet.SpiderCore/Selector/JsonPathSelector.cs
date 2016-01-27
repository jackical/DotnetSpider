using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Core.Selector
{
	/// <summary>
	/// JsonPath selector. 
	/// Used to extract content from JSON. 
	/// </summary>
	public class JsonPathSelector : ISelector
	{
		private readonly string _jsonPathStr;

		public JsonPathSelector(string jsonPathStr)
		{
			_jsonPathStr = jsonPathStr;
		}

		public SelectedNode Select(SelectedNode text)
		{
			IList<SelectedNode> result = SelectList(text);
			if (result.Count > 0)
			{
				return result[0];
			}
			return null;
		}

		public SelectedNode Select(string text)
		{
			IList<SelectedNode> result = SelectList(new SelectedNode() { Type = ResultType.String, Result = text });
			if (result.Count > 0)
			{
				return result[0];
			}
			return null;
		}

		public IList<SelectedNode> SelectList(string text)
		{
			return SelectList(new SelectedNode() { Type = ResultType.String, Result = text });
		}

		public List<SelectedNode> SelectList(SelectedNode text)
		{
			if (text != null)
			{
				if (text.Type == ResultType.Json)
				{
					var jToken = text.Result as JToken;
					if (jToken != null)
					{
						var items = jToken.SelectTokens(_jsonPathStr);

						if (items == null)
						{
							return new List<SelectedNode>();
						}

						return items.Select(item => new SelectedNode { Type = ResultType.Json, Result = item }).ToList();
					}
					throw new SpiderExceptoin("SelectedNode is not a JToken");
				}
				else
				{
					List<SelectedNode> list = new List<SelectedNode>();
					JObject o = (JObject)JsonConvert.DeserializeObject(text.ToString());
					var items = o.SelectTokens(_jsonPathStr).ToList();

					list.AddRange(items.Select(item => new SelectedNode { Type = ResultType.Json, Result = item }));

					return list;
				}

			}
			else
			{
				return new List<SelectedNode>();
			}
		}
	}
}