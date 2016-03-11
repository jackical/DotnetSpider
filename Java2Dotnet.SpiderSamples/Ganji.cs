using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension;
using Java2Dotnet.Spider.Extension.Configuration;
using Java2Dotnet.Spider.Extension.DbSupport;

namespace Java2Dotnet.Spider.Samples
{
	public class GanjiPostSpider : BaseTask
	{
		protected override SpiderContext CreateSpiderContext()
		{
			return new SpiderContext
			{
				SpiderName = "Ganji Post Dayly " + DateTime.Now.ToString("yyyy_MM_dd"),
				Site = new Site { SleepTime = 1000, EncodingName = "UTF-8" },
				Pipeline = new MysqlPipeline()
				{
					ConnectString = "Database='mysql';Data Source= localhost;User ID=root;Password=1qazZAQ!;Port=3306"
				}.ToJObject(),
				PrepareStartUrls = new CyclePrepareStartUrls
				{
					FormateString = "http://sh.ganji.com/zpdianhuaxiaoshou/o{0}/",
					From = 1,
					To = 50
				}.ToJObject()
			};
		}

		public override HashSet<Type> EntiTypes => new HashSet<Type>() { typeof(Ganji) };
	}

	[TypeExtractBy(Expression = "//*[@id=\"list-job-id\"]/div[9]/dl", Multi = true)]
	[Schema("ganji", "post")]
	public class Ganji : BaseEntity
	{
		[StoredAs("title", StoredAs.ValueType.String, 100)]
		[PropertyExtractBy(Expression = "./dt/a")]
		public string Title { get; set; }

		[StoredAs("company", StoredAs.ValueType.String, 100)]
		[PropertyExtractBy(Expression = "./dd[1]/a")]
		public string Company { get; set; }

		[StoredAs("ishr", StoredAs.ValueType.String, 5)]
		[PropertyExtractBy(Expression = "./dd[1]/span/@class")]
		public string IsHr { get; set; }

		[StoredAs("bangbang", StoredAs.ValueType.String, 20)]
		[PropertyExtractBy(Expression = "./dd[1]/span")]
		public string BangBang { get; set; }

		[StoredAs("third", StoredAs.ValueType.String, 20)]
		[PropertyExtractBy(Expression = "./dd[1]/i/@title")]
		public string Third { get; set; }

		[StoredAs("corpmail", StoredAs.ValueType.String, 20)]
		[PropertyExtractBy(Expression = "./dd[1]/span[1]/@title")]
		public string CorpMail { get; set; }

		[StoredAs("uuid", StoredAs.ValueType.String, 20)]
		public string Uuid => Encrypt.Md5Encrypt(Title + Company);

		[PropertyExtractBy(Expression = "url", Type = ExtractType.Enviroment)]
		[StoredAs("url", StoredAs.ValueType.Text)]
		public string Url { get; set; }

		[PropertyExtractBy(Expression = "Now", Type = ExtractType.Enviroment)]
		[StoredAs("cdate", StoredAs.ValueType.Date)]
		public DateTime CDate { get; set; }
	}
}
