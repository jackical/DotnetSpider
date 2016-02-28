using Java2Dotnet.Spider.Core.Selector;

namespace Java2Dotnet.Spider.Extension.Model
{
	/// <summary>
	/// The object contains 'ExtractBy' information.
	/// </summary>
	public class Extractor
	{
		public Extractor(ISelector selector, string expression,  bool notNull, int count = int.MaxValue)
		{
			Selector = selector;
			NotNull = notNull;
			Count = count;
			Expression = expression;
		}

		public string Expression { get; }

		public int Count { get; }

		public ISelector Selector { get; }

		public bool NotNull { get; }
	}
}