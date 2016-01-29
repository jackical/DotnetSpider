using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;

namespace Java2Dotnet.Spider.Samples
{
	public class BaseSpiderEntity : SpiderEntity
	{
		[StoredAs("id", StoredAs.ValueType.Long, true)]
		[KeyProperty]
		public override long Id
		{
			get; set;
		}
	}
}
