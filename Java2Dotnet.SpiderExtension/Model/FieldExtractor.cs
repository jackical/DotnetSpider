using System.Reflection;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Model.Formatter;

namespace Java2Dotnet.Spider.Extension.Model
{
	/// <summary>
	/// Wrapper of field and extractor.
	/// </summary>
	public class FieldExtractor : Extractor
	{
		public FieldExtractor(PropertyInfo field, ISelector selector, string expression, bool notNull, bool multi, int count = int.MaxValue)
			: base(selector, expression, notNull, count)
		{
			Field = field;
			//Stopper = stopper;
			Multi = multi;
		}

		public PropertyInfo Field { get; private set; }

		public IObjectFormatter ObjectFormatter { get; set; }

		//public Stopper Stopper { get; set; }

		public bool Download { get; set; }

		public bool Multi { get; private set; }
	}
}