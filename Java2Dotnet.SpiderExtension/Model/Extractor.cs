using Java2Dotnet.Spider.Core.Selector;

namespace Java2Dotnet.Spider.Extension.Model
{
	/// <summary>
	/// The object contains 'ExtractBy' information.
	/// </summary>
	public class Extractor
	{
		public Extractor(ISelector selector, string expression, ExtractSource source, bool notNull, long count = long.MaxValue)
		{
			Selector = selector;
			Source = source;
			NotNull = notNull;
			Count = count;
			Expression = expression;
		}

		public string Expression { get; }

		public long Count { get; }

		public ISelector Selector { get; }

		public ExtractSource Source { get; }

		public bool NotNull { get; }
	}
}