using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Extension.Model
{
	/// <summary>
	/// Wrapper of type and extractor.
	/// </summary>
	public class TypeExtractor : Extractor
	{
		public TypeExtractor(ISelector selector, string expression, ExtractSource source, bool multi, long count = long.MaxValue, RequestStoping requestStoping=null)
			: base(selector, expression, source, false, count)
		{
			Multi = multi;
			RequestStoping = requestStoping;
		}

		public bool Multi { get; private set; }

		public RequestStoping RequestStoping { get; private set; }
	}
}
