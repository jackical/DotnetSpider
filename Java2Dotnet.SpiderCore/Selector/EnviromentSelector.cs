﻿using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector
{
	internal class EnviromentSelector : ISelector
	{
		public string Field { get; private set; }

		public EnviromentSelector(string field)
		{
			Field = field;
		}

		public dynamic Select(dynamic text)
		{
			throw new SpiderExceptoin("EnviromentSelector does not support SelectList method now.");
		}

		public List<dynamic> SelectList(dynamic text)
		{
			throw new SpiderExceptoin("EnviromentSelector does not support SelectList method now.");
		}
	}
}
