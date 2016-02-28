using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Model
{
	public interface IPageModelExtractor
	{
		dynamic Process(Page page);
		Type ActualType { get; }
		List<TargetUrlExtractInfo> TargetUrlExtractInfos { get; }
	}
}
