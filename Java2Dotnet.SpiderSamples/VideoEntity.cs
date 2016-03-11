using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.ORM;

namespace Java2Dotnet.Spider.Samples
{
	[TypeExtractBy(Expression = "//*[@id=\"listofficial\"]/div[1]/div")]
	[Schema("youku", "video3")]
	public class VideoEntity
	{
		[StoredAs("id", StoredAs.ValueType.Int)]
		public int Id { get; set; }
		[StoredAs("Url", StoredAs.ValueType.Text)]
		[PropertyExtractBy(Expression = "/div[1]/div[1]/div[2]/a/@href")]
		public string Url { get; set; }
		[StoredAs("Name", StoredAs.ValueType.String, 100)]
		[PropertyExtractBy(Expression = "/div[1]/div[1]/div[4]/div[1]/a/@title")]
		public string Name { get; set; }
		[StoredAs("Count", StoredAs.ValueType.String, 12)]
		[PropertyExtractBy(Expression = "/div[1]/div[1]/div[4]/div[3]/span")]
		public string Count { get; set; }
	}
}
