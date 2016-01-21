using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Extension.Utils
{
	/// <summary>
	/// Tools for annotation converting. 
	/// </summary>
	public class ExtractorUtils
	{
		public static ISelector GetSelector(BaseExtractBy extractBy)
		{
			string value = extractBy.Expression;
			ISelector selector;
			switch (extractBy.Type)
			{
				case ExtractType.Css:
					selector = new CssSelector(value);
					break;
				case ExtractType.Regex:
					selector = new RegexSelector(value);
					break;
				case ExtractType.XPath:
					selector = GetXpathSelector(value);
					break;
				case ExtractType.JsonPath:
					selector = new JsonPathSelector(value);
					break;
				case ExtractType.Enviroment:
					selector = null;
					break;
				default:
					selector = GetXpathSelector(value);
					break;
			}
			return selector;
		}

		//public static Extractor GetExtractor(PropertyExtractBy extractBy)
		//{
		//	ISelector selector = GetSelector(extractBy);
		//	return new FieldExtractor(selector, extractBy.Expression, extractBy.Source, extractBy.NotNull, extractBy.Count);
		//}

		public static TypeExtractor GetTypeExtractor(TypeExtractBy extractBy)
		{
			ISelector selector = GetSelector(extractBy);
			return new TypeExtractor(selector, extractBy.Expression, extractBy.Source, extractBy.Multi);
		}

		private static ISelector GetXpathSelector(string value)
		{
			ISelector selector = new XPathSelector(value);
			return selector;
		}

		public static IList<ISelector> GetSelectors(PropertyExtractBy[] extractBies)
		{
			IList<ISelector> selectors = new List<ISelector>();
			if (extractBies == null)
			{
				return selectors;
			}
			foreach (PropertyExtractBy extractBy in extractBies)
			{
				selectors.Add(GetSelector(extractBy));
			}
			return selectors;
		}
	}
}