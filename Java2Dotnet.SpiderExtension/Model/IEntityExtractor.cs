using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Model
{
	public interface IEntityExtractor
	{
		dynamic Process(Page page);
		string EntityName { get; }
		List<TargetUrlExtractInfo> TargetUrlExtractInfos { get; }
	}
}
