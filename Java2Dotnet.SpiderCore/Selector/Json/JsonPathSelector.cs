using System.Collections.Generic;
using System.Linq;
using Java2Dotnet.Spider.Core.Selector.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Core.Selector.Json
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

		public string Select(string text)
		{
			IList<string> result = SelectList(text);
			if (result.Count > 0)
			{
				return result[0];
			}
			return null;
		}

		public List<string> SelectList(string text)
		{
			if (text != null)
			{
				List<string> list = new List<string>();
				JObject o = JsonConvert.DeserializeObject(text) as JObject;

				if (o != null)
				{
					var items = o.SelectTokens(_jsonPathStr).ToList();
					list.AddRange(items.Select(i => i.ToString()));
				}
				else
				{
					JArray a = JsonConvert.DeserializeObject(text) as JArray;
					var items = a.SelectTokens(_jsonPathStr).ToList();
					list.AddRange(items.Select(i => i.ToString()));
				}
				return list;
			}
			else
			{
				return new List<string>();
			}
		}
	}
}