using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Samples
{
	[TypeExtractBy(Expression = "//*[@id=\"tab_top50\"]/div[1]/ul/li", Count = 15)]
	[Scheme("aiqiyi", "movies")]
	public class Walter : SpiderEntity
	{
		[StoredAs("id", StoredAs.ValueType.Long, true)]
		[KeyProperty(Identity = true)]
		public override long Id { get; set; }

		public static void RunTask()
		{
			//ModelSpider<Walter> ooSpider = OoSpider.Create("aiqiyi_movies_" + DateTime.Now.ToLocalTime(), new Site { SleepTime = 1000, Encoding = Encoding.UTF8 }, new RedisScheduler("localhost", null), new DatabasePipeline(), typeof(List<Walter>));
			//ModelSpider<List<Walter>> ooSpider = new ModelSpider<List<Walter>>("aiqiyi_movies_" + DateTime.Now.ToLocalTime(), new Site { SleepTime = 1000, Encoding = Encoding.UTF8 }, ModelPipelineType.MySql);
			//ooSpider.SetEmptySleepTime(15000);
			//ooSpider.SetThreadNum(10);
			//ooSpider.AddStartUrl("http://top.iqiyi.com/dianshiju.html#");
			//ooSpider.Run();
		}

		[StoredAs("rank", StoredAs.ValueType.Varchar, false, 20)]
		[PropertyExtractBy(Expression = "li/em")]
		public string Rank { get; set; }

		[StoredAs("name", StoredAs.ValueType.Varchar, false, 20)]
		[PropertyExtractBy(Expression = "/li/a[1]/@title")]
		public string Name { get; set; }

		[StoredAs("url", StoredAs.ValueType.Varchar, false, 100)]
		[PropertyExtractBy(Expression = "/li/a[1]/@href")]
		public string Url { get; set; }

		[PropertyExtractBy(Expression = "/li/span[1]/a")]
		public List<string> Label1 { get; set; }

		[StoredAs("tag", StoredAs.ValueType.Varchar, false, 500)]
		public string Tag => string.Join("|", Label1);

		//[StoredAs("label_2", StoredAs.ValueType.Varchar, false, 20)]
		//[PropertyExtractBy(Expression = "/li/span[1]/a[2]")]
		//public string Label_2 { get; set; }

		//[StoredAs("label_3", StoredAs.ValueType.Varchar, false, 20)]
		//[PropertyExtractBy(Expression = "/li/span[1]/a[3]")]
		//public string Label_3 { get; set; }

		[StoredAs("uuid", StoredAs.ValueType.Varchar, false, 20)]
		public string Uuid => Encrypt.Md5Encrypt(Name + Url);

		[StoredAs("cdate", StoredAs.ValueType.Date)]
		public DateTime CDate => DateTime.Now;
	}
}
