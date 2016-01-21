using System;
using System.Diagnostics;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;

namespace Java2Dotnet.Spider.Samples
{
	////*[@id="list-job-id"]/div[15]/ul
	//[TargetUrl(new[] { "zpdianhuaxiaoshou/o[0-9]+/" }, "//*[@id=\"list-job-id\"]/div[15]/ul")]

	//////*[@id="list-job-id"]/div[9]/dl[1]
	[TypeExtractBy(Expression = "//*[@id=\"list-job-id\"]/div[9]/dl", Multi = true)]
	[Scheme("ganji", "post")]
	public class Ganji : SpiderEntityUseStringKey
	{
		public static void RunTask()
		{
			Stopwatch watch = Stopwatch.StartNew();
			watch.Start();
			ModelCollectorSpider<Ganji> ooSpider = new ModelCollectorSpider<Ganji>("ganji", new Site { SleepTime = 1000, Encoding = Encoding.UTF8 });
			ooSpider.SetEmptySleepTime(15000);
			ooSpider.SetThreadNum(1);
			ooSpider.SetCachedSize(1);

			for (int i = 1; i <= 50; ++i)
			{
				ooSpider.AddStartUrl($"http://sh.ganji.com/zpdianhuaxiaoshou/o{i}/");
			}
			ooSpider.Run();
			watch.Stop();
			Console.WriteLine("Need time: " + watch.ElapsedMilliseconds);
		}

		[StoredAs("title", StoredAs.ValueType.Varchar, false, 100)]
		////*[@id="list-job-id"]/div[9]/dl[1]/dt/a
		[PropertyExtractBy(Expression = "/dl/dt/a")]
		public string Title { get; set; }

		[StoredAs("company", StoredAs.ValueType.Varchar, false, 100)]
		[PropertyExtractBy(Expression = "/dl/dd[1]/a")]
		public string Company { get; set; }

		[StoredAs("ishr", StoredAs.ValueType.Varchar, false, 5)]
		[PropertyExtractBy(Expression = "/dl/dd[1]/span/@class")]
		public string IsHr { get; set; }

		[StoredAs("bangbang", StoredAs.ValueType.Varchar, false, 20)]
		[PropertyExtractBy(Expression = "/dl/dd[1]/span")]
		public string BangBang { get; set; }

		[StoredAs("third", StoredAs.ValueType.Varchar, false, 20)]
		[PropertyExtractBy(Expression = "/dl/dd[1]/i/@title")]
		public string Third { get; set; }

		[StoredAs("corpmail", StoredAs.ValueType.Varchar, false, 20)]
		[PropertyExtractBy(Expression = "/dl/dd[1]/span[1]/@title")]
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
