using System;
using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class EnviromentSelector : ISelector
	{
		private readonly string _field;

		public EnviromentSelector(string field)
		{
			_field = field;
		}

		public object GetValue(Page page)
		{
			if (_field.ToLower() == "url")
			{
				return page.Url;
			}

			if (_field.ToLower() == "targeturl")
			{
				return page.TargetUrl;
			}

			return page.Request.GetExtra(_field);
		}

		public IList<string> GetValueList(Page page)
		{
			throw new NotImplementedException();
		}

		public IList<SelectedNode> SelectList(SelectedNode text)
		{
			throw new NotImplementedException();
		}

		public SelectedNode Select(List<SelectedNode> text)
		{
			throw new NotImplementedException();
		}

		public List<SelectedNode> SelectList(List<SelectedNode> text)
		{
			throw new NotImplementedException();
		}

		public SelectedNode Select(SelectedNode text)
		{
			throw new NotImplementedException();
		}

		List<SelectedNode> ISelector.SelectList(SelectedNode text)
		{
			throw new NotImplementedException();
		}
	}
}
