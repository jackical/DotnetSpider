//using System;
//using System.Text;
//using Java2Dotnet.Spider.Core;
//using Java2Dotnet.Spider.Core.Utils;
//using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
//using Java2Dotnet.Spider.Extension.Model;
//using Java2Dotnet.Spider.Extension.Model.Attribute;

//namespace Java2Dotnet.Spider.Samples
//{
//	public class YoukuVipBll
//	{
//		public void RunTask(string op)
//		{
//			switch (op)
//			{
//				case ("VipMovie"):
//					{
//						ModelDatabaseSpider<YoukuVip> ooSpider = new ModelDatabaseSpider<YoukuVip>("youku_viptop_" + DateTime.Now.ToString("yyyyMMddhhmm"),
//							new Site { SleepTime = 1000, Encoding = Encoding.UTF8 });
//						ooSpider.SetEmptySleepTime(15000);
//						ooSpider.SetThreadNum(1);
//						ooSpider.AddStartUrl("http://vip.youku.com/?app=newvip&c=svip&a=getHtml&cid=214673");
//						ooSpider.Run();

//						//EmailUtil.QuerySend("walter@86research.com", "Youku_Vip_Top", "SELECT * FROM youku.vip_top WHERE run_id='" + DateTime.Now.ToString("yyyy-MM-dd") + "'", false);

//						break;
//					}
//				case ("MovieComing"):
//					{
//						ModelDatabaseSpider<YoukuVipMovieComing> ooSpider = new ModelDatabaseSpider<YoukuVipMovieComing>("youku_vipmoviecoming_" + DateTime.Now.ToString("yyyyMMddhhmm"),
//								new Site { SleepTime = 1000, Encoding = Encoding.UTF8 });
//						ooSpider.SetEmptySleepTime(15000);
//						ooSpider.SetThreadNum(1);
//						ooSpider.AddStartUrl("http://vip.youku.com/?app=newvip&c=svip&a=getHtml&cid=212927");
//						ooSpider.Run();

//						//EmailUtil.QuerySend(EmailUtil.DigitMedia + "walter@86research.com;", "Youku_Vip_Movie_Coming", "SELECT * FROM youku.vip_movie_coming WHERE run_id='" + DateTime.Now.ToString("yyyy-MM-dd") + "'", false);

//						break;
//					}
//			}
//		}
//	}

//	[Scheme("youku", "vip_pay_url")]
//	public class PayUrl : BaseSpiderEntity
//	{
//		[PropertyExtractBy(Expression = "url", Type = ExtractType.Enviroment)]
//		[StoredAs("url")]
//		public string Url { get; set; }

//		[StoredAs("run_id", StoredAs.ValueType.Date)]
//		public DateTime RunId => DateTime.Now;
//	}

//	[TypeExtractBy(Expression = "$.result.items[*]", Type = ExtractType.JsonPath, Multi = true)]
//	[Scheme("youku", "vip_top")]
//	public class YoukuVip : BaseSpiderEntity
//	{
//		[StoredAs("rank")]
//		public string Rank { get; set; }

//		[StoredAs("name", StoredAs.ValueType.Varchar, false, 20)]
//		[PropertyExtractBy(Expression = "$.name", Type = ExtractType.JsonPath)]
//		public string Name { get; set; }

//		[PropertyExtractBy(Expression = "$.showweek_all_vv", Type = ExtractType.JsonPath)]
//		[StoredAs("playcount_weekly", StoredAs.ValueType.Varchar, false, 50)]
//		public string PlayCountWeekly { get; set; }

//		[PropertyExtractBy(Expression = "$.videourl", Type = ExtractType.JsonPath)]
//		[StoredAs("url", StoredAs.ValueType.Varchar, false, 100)]
//		public string Url { get; set; }

//		[StoredAs("uuid", StoredAs.ValueType.Varchar, false, 20)]
//		public string Uuid => Encrypt.Md5Encrypt(Name + Url);

//		[StoredAs("run_id", StoredAs.ValueType.Date)]
//		public DateTime RunId => DateTime.Now;
//	}

//	[TypeExtractBy(Expression = "$.result.items[*]", Type = ExtractType.JsonPath, Multi = true)]
//	[Scheme("youku", "vip_movie_coming")]
//	public class YoukuVipMovieComing : BaseSpiderEntity
//	{
//		[StoredAs("name", StoredAs.ValueType.Varchar, false, 20)]
//		[PropertyExtractBy(Expression = "$.name", Type = ExtractType.JsonPath)]
//		public string Name { get; set; }

//		[NonStored]
//		[PropertyExtractBy(Expression = "$.onlinetime", Type = ExtractType.JsonPath)]
//		public string RawOnlineTime { get; set; }

//		[StoredAs("onlinetime", StoredAs.ValueType.Varchar, false, 50)]
//		public string OnlineTime => string.IsNullOrEmpty(RawOnlineTime) ? "即将上映" : RawOnlineTime;

//		[PropertyExtractBy(Expression = "$.videourl", Type = ExtractType.JsonPath)]
//		[StoredAs("url", StoredAs.ValueType.Varchar, false, 100)]
//		public string Url { get; set; }

//		[StoredAs("uuid", StoredAs.ValueType.Varchar, false, 20)]
//		public string Uuid => Encrypt.Md5Encrypt(Name + Url);

//		[StoredAs("run_id", StoredAs.ValueType.Date)]
//		public DateTime RunId => DateTime.Now;
//	}
//}
