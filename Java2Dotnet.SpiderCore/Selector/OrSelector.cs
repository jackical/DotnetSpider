using System.Collections.Generic;
using System.Linq;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class OrSelector : ISelector
	{
		private readonly IList<ISelector> _selectors = new List<ISelector>();

		public OrSelector(params ISelector[] selectors)
		{
			foreach (ISelector selector in selectors)
			{
				_selectors.Add(selector);
			}
		}

		public OrSelector(IList<ISelector> selectors)
		{
			_selectors = selectors;
		}

		public SelectedNode Select(SelectedNode text)
		{
			return _selectors.Select(selector => selector.Select(text)).FirstOrDefault(result => result != null);
		}

		public List<SelectedNode> SelectList(SelectedNode text)
		{
			List<SelectedNode> results = new List<SelectedNode>();
			foreach (ISelector selector in _selectors)
			{
				IList<SelectedNode> strings = selector.SelectList(text);
				results.AddRange(strings);
			}
			return results;
		}
	}
}
