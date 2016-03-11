//using System;
//using System.Collections.Generic;
//using System.Text;
//using Java2Dotnet.Spider.Core;
//using Java2Dotnet.Spider.Core.Utils;
//using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
//using Java2Dotnet.Spider.Extension.Model;
//using Java2Dotnet.Spider.Extension.Model.Attribute;

//namespace Java2Dotnet.Spider.Samples.Samples
//{
//	[TypeExtractBy(Expression = "//*[@id=\"tab_top50\"]/div[1]/ul/li", Count = 1)]
//	[Scheme("Test", "SingleTest")]
//	public class SingleSample
//	{
//		public static void RunTask()
//		{
//			ModelMysqlFileSpider<SingleSample> spider = new ModelMysqlFileSpider<SingleSample>("aiqiyi_movies_" + DateTime.Now.ToLocalTime(), new Site { SleepTime = 1000, Encoding = Encoding.UTF8 });

//			spider.SetEmptySleepTime(15000);
//			spider.SetThreadNum(1);
//			spider.AddStartUrl("http://top.iqiyi.com/dianshiju.html#");
//			spider.Run();
//		}

//		[StoredAs("rank", StoredAs.ValueType.Varchar, false, 20)]
//		[PropertyExtractBy(Expression = "li/em")]
//		public string Rank { get; set; }

//		[StoredAs("name", StoredAs.ValueType.Varchar, false, 20)]
//		[PropertyExtractBy(Expression = "/li/a[1]/@title")]
//		public string Name { get; set; }

//		[StoredAs("url", StoredAs.ValueType.Varchar, false, 100)]
//		[PropertyExtractBy(Expression = "/li/a[1]/@href")]
//		public string Url { get; set; }

//		[PropertyExtractBy(Expression = "/li/span[1]/a")]
//		public List<string> Label_1 { get; set; }

//		[StoredAs("tag", StoredAs.ValueType.Varchar, false, 500)]
//		public string Tag
//		{
//			get
//			{
//				return string.Join("|", Label_1);
//			}
//		}
//		//[StoredAs("label_2", StoredAs.ValueType.Varchar, false, 20)]
//		//[PropertyExtractBy(Expression = "/li/span[1]/a[2]")]
//		//public string Label_2 { get; set; }

//		//[StoredAs("label_3", StoredAs.ValueType.Varchar, false, 20)]
//		//[PropertyExtractBy(Expression = "/li/span[1]/a[3]")]
//		//public string Label_3 { get; set; }

//		[StoredAs("uuid", StoredAs.ValueType.Varchar, false, 20)]
//		public string Uuid => Encrypt.Md5Encrypt(Name + Url);

//		[StoredAs("cdate", StoredAs.ValueType.Date)]
//		public DateTime CDate => DateTime.Now;
//	}
//}
