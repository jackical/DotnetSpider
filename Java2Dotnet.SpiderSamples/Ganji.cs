using System;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension;

namespace Java2Dotnet.Spider.Samples
{
	public class GanjiPostSpiderTaskTask : AbstractSpiderTask
	{
		public override string Name => "Ganji Post Dayly " + DateTime.Now.ToString("yyyy_MM_dd");

		protected override Site Site { get; } = new Site { SleepTime = 1000, Encoding = Encoding.UTF8 };

		protected override Core.Spider InitSpider()
		{
			ModelDatabaseSpider<Ganji> spider = new ModelDatabaseSpider<Ganji>(Name, Site, Scheduler);
			spider.SetEmptySleepTime(15000);
			spider.SetThreadNum(1);
			spider.SetCachedSize(1);

			return spider;
		}

		protected override void PrepareSite()
		{
			for (int i = 1; i <= 50; ++i)
			{
				Site.AddStartUrl($"http://sh.ganji.com/zpdianhuaxiaoshou/o{i}/");
			}
		}
	}

	[TypeExtractBy(Expression = "//*[@id=\"list-job-id\"]/div[9]/dl", Multi = true)]
	[Scheme("ganji", "post")]
	public class Ganji : BaseSpiderEntity
	{
		[StoredAs("title", StoredAs.ValueType.Varchar, false, 100)]
		////*[@id="list-job-id"]/div[9]/dl[1]/dt/a
		[PropertyExtractBy(Expression = "./dt/a")]
		public string Title { get; set; }

		[StoredAs("company", StoredAs.ValueType.Varchar, false, 100)]
		[PropertyExtractBy(Expression = "./dd[1]/a")]
		public string Company { get; set; }

		[StoredAs("ishr", StoredAs.ValueType.Varchar, false, 5)]
		[PropertyExtractBy(Expression = "./dd[1]/span/@class")]
		public string IsHr { get; set; }

		[StoredAs("bangbang", StoredAs.ValueType.Varchar, false, 20)]
		[PropertyExtractBy(Expression = "./dd[1]/span")]
		public string BangBang { get; set; }

		[StoredAs("third", StoredAs.ValueType.Varchar, false, 20)]
		[PropertyExtractBy(Expression = "./dd[1]/i/@title")]
		public string Third { get; set; }

		[StoredAs("corpmail", StoredAs.ValueType.Varchar, false, 20)]
		[PropertyExtractBy(Expression = "./dd[1]/span[1]/@title")]
		public string CorpMail { get; set; }

		[StoredAs("uuid", StoredAs.ValueType.Varchar, false, 20)]
		public string Uuid => Encrypt.Md5Encrypt(Title + Company);

		[PropertyExtractBy(Expression = "url", Type = ExtractType.Enviroment)]
		[StoredAs("url", StoredAs.ValueType.Text)]
		public string Url { get; set; }

		[StoredAs("cdate", StoredAs.ValueType.Date)]
		public DateTime CDate => DateTime.Now;
	}
}
