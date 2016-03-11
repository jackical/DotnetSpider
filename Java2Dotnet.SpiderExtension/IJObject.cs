using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension
{
	public interface IJObject
	{
	}

	public static class JsonObjectExtension
	{
		public static JObject ToJObject(this IJObject jsonObj)
		{
			return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(jsonObj)) as JObject;
		}
	}
}
