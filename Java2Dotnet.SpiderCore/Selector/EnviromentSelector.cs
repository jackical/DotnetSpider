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

			//Page page = text.Data as Page;
			//if (page != null)
			//{
			//	if (_field.ToLower() == "url")
			//	{
			//		return new SelectedNode() { Result = page.Url, Type = ResultType.String, Data = text.Data };
			//	}

			//	if (_field.ToLower() == "targeturl")
			//	{
			//		return new SelectedNode() { Result = page.TargetUrl, Type = ResultType.String, Data = text.Data };
			//	}

			//	return new SelectedNode() { Result = page.Request.GetExtra(_field), Type = ResultType.String, Data = text.Data };
			//}
			//else
			//{
			//	throw new SpiderExceptoin("EnviromentSelector need Page data. Please pass the page object to SelectNode.Data");
			//}
		}

		public List<string> SelectList(string text)
		{
			throw new SpiderExceptoin("EnviromentSelector does not support SelectList method now.");
		}
	}
}
