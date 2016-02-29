using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class EnviromentSelector : ISelector
	{
		public string Field { get; private set; }

		public EnviromentSelector(string field)
		{
			Field = field;
		}

		public string Select(string text)
		{
			throw new SpiderExceptoin("EnviromentSelector does not support SelectList method now.");
		}

		public List<string> SelectList(string text)
		{
			throw new SpiderExceptoin("EnviromentSelector does not support SelectList method now.");
		}
	}
}
