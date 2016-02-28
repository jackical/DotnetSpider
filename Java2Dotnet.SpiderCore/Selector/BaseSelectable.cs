using System.Collections.Generic;
using System.Linq;
using log4net;

namespace Java2Dotnet.Spider.Core.Selector
{
	public abstract class BaseSelectable : IBaseSelectable
	{
		protected static readonly ILog Logger = LogManager.GetLogger("BaseSelectable");

		public List<string> Elements { get; set; }

		public IBaseSelectable Regex(string regex)
		{
			RegexSelector regexSelector = Selectors.Regex(regex);
			return Select(regexSelector);
		}

		public IBaseSelectable Regex(string regex, int group)
		{
			RegexSelector regexSelector = Selectors.Regex(regex, group);
			return Select(regexSelector);
		}

		public dynamic Value
		{
			get
			{
				if (Elements == null || Elements.Count == 0)
				{
					return null;
				}

				if (Elements.Count == 1)
				{
					return Elements[0];
				}

				return Elements.Select(selectedNode => selectedNode.ToString()).ToList();
			}
		}

		public abstract IBaseSelectable Select(ISelector selector);
 
		public abstract IBaseSelectable SelectList(ISelector selector);
	}
}
