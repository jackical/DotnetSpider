using System;
using System.Collections.Generic;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Downloader;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Core.Selector.Json;
using Java2Dotnet.Spider.Extension;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Model.Formatter;

namespace Java2Dotnet.Spider.Samples
{
	public class TmallGmvSpider : AbstractSpiderTask
	{
		protected override void PrepareSite()
		{
			var list = new List<TmallShop> { new TmallShop() { Brand = "鸿彤彤", Category = "女鞋", Name = "鸿彤彤旗舰店", UserId = "1025369930", Url = "search_shopitem.htm?user_id=1025369930&amp;from=_1_&amp;stype=search" } };

			foreach (var index in list)
			{
				Dictionary<string, object> tmp = index.ToDictionary();
				string keyword = tmp["Name"].ToString();

				string url = "https://s.taobao.com/search?q=" + keyword + "&ie=utf8&sort=sale-desc&bcoffset=-4&ntoffset=-4&p4plefttype=3%2C1&p4pleftnum=1%2C3&filter_tianmao=tmall&s=0";
				Site.AddStartUrl(url, tmp);
			}
		}

		protected override Site Site { get; } = new Site
		{
			SleepTime = 500,
			Encoding = Encoding.UTF8
		};

		protected override Core.Spider InitSpider()
		{
			HttpClientDownloader downloader = new HttpClientDownloader();
			downloader.DownloadValidation = page =>
			{
				string rawText = page.Content;
				if (rawText.Contains("anti_Spider"))
				{
					return DownloadValidationResult.FailedAndNeedRedial;
				}
				else
				{
					return DownloadValidationResult.Success;
				}
			};

			ModelMysqlFileSpider<TmallProductItem> ooSpider = new ModelMysqlFileSpider<TmallProductItem>(Name, Site, Scheduler);
			ooSpider.SetThreadNum(1);
			ooSpider.SetEmptySleepTime(15000);
			ooSpider.SetDownloader(downloader);
			ooSpider.CustomizePage = p =>
			{
				try
				{
					string rawText = p.Content;
					int subBegin = rawText.IndexOf("\"{\\\"bottom\\\":", StringComparison.Ordinal);
					int subEnd = rawText.IndexOf("\"export\":", subBegin, StringComparison.Ordinal) - 3;
					string sub = rawText.Substring(subBegin, subEnd - subBegin + 1).Replace("\\\"", "\"");
					int subSubEnd = sub.LastIndexOf("\"", StringComparison.Ordinal);
					sub = sub.Remove(subSubEnd, 1);
					sub = sub.Remove(0, 1);
					rawText = rawText.Remove(subBegin, subEnd - subBegin + 1);
					rawText = rawText.Insert(subBegin, sub);
					int begin = rawText.IndexOf("{\"pageName\":", StringComparison.Ordinal);
					int end = rawText.IndexOf("g_srp_loadCss();", StringComparison.Ordinal) - 1;
					string newRawText = rawText.Substring(begin, end - begin + 1).Trim().TrimEnd(';');

					p.Content = newRawText;
				}
				catch
				{
					throw new SpiderExceptoin("Rawtext Invalid.");
				}
			};
			ooSpider.SetCustomizeTargetUrls(page =>
			{
				string totalPage = page.HtmlSelectable.Select(new JsonPathSelector("$.mods.pager.data.totalPage")).Value;
				string currentPage = page.HtmlSelectable.Select(new JsonPathSelector("$.mods.pager.data.currentPage")).Value;
				if (totalPage == currentPage)
				{
					return new List<string>();
				}
				else
				{
					string url = page.Url;
					int currentPageNumber = Int32.Parse(currentPage);
					url = url.Replace("s=" + 44 * (currentPageNumber - 1), "s=" + 44 * currentPageNumber);
					return new List<string> { url };
				}
			});
			return ooSpider;
		}

		public override string Name => "Tmall Gmv test " + new DateTime(2016, DateTime.Now.Month, 1).ToString("yyyy_MM_dd");

		[Scheme("taobao", "tmall_shop_gmvorder")]
		public class TmallShop : SpiderEntity
		{
			[StoredAs("id", StoredAs.ValueType.Long, true)]
			[KeyProperty(Identity = true)]
			public override long Id { get; set; }

			[StoredAs("name", StoredAs.ValueType.Varchar, false, 100)]
			public string Name { get; set; }

			[StoredAs("brand", StoredAs.ValueType.Varchar, false, 50)]
			public string Brand { get; set; }

			[StoredAs("category", StoredAs.ValueType.Varchar, false, 50)]
			public string Category { get; set; }

			[StoredAs("uid", StoredAs.ValueType.Varchar, false, 100)]
			public string UserId { get; set; }

			[StoredAs("url", StoredAs.ValueType.Text)]
			public string Url { get; set; }

			[StoredAs("run_id", StoredAs.ValueType.Date)]
			public DateTime RunId { get; set; }
		}

		public class SoldCountFormater : CustomizeFormatter
		{
			protected override dynamic FormatTrimmed(string raw)
			{
				if (raw.EndsWith("万笔"))
				{
					return (long)(float.Parse(raw.Replace("万笔", "")) * 10000);
				}

				if (raw.EndsWith("笔"))
				{
					return long.Parse(raw.Replace("笔", ""));
				}

				if (raw.EndsWith("人收货"))
				{
					return long.Parse(raw.Replace("人收货", ""));
				}

				return long.Parse(raw);
			}
		}

		[Scheme("taobao", "tmall_product_item", SchemeSuffix.FirstDayOfMonth)]
		[TmallGmvItemRequestStoping]
		[TypeExtractBy(Expression = "$.mods.itemlist.data.auctions[*]", Type = ExtractType.JsonPath, Multi = true)]
		public class TmallProductItem : SpiderEntity
		{
			[StoredAs("id", StoredAs.ValueType.Long, true)]
			[KeyProperty(Identity = true)]
			public override long Id { get; set; }

			private static readonly DateTime runId;

			static TmallProductItem()
			{
				DateTime dt = DateTime.Now;
				runId = new DateTime(dt.Year, dt.Month, 1);
			}

			[PropertyExtractBy(Expression = "$.title", Type = ExtractType.JsonPath)]
			[StoredAs("name", StoredAs.ValueType.Varchar, false, 100)]
			public string Name { get; set; }

			[Formatter(typeof(ReplaceFormatter), new[] { "¥", "" }, true)]
			[PropertyExtractBy(Expression = "$.view_price", Type = ExtractType.JsonPath)]
			[StoredAs("price", StoredAs.ValueType.Float)]
			public float Price { get; set; }

			[Formatter(typeof(SoldCountFormater))]
			[PropertyExtractBy(Expression = "$.view_sales", Type = ExtractType.JsonPath)]
			[StoredAs("sold", StoredAs.ValueType.Long)]
			public long Sold { get; set; }

			[StoredAs("url", StoredAs.ValueType.Text)]
			[PropertyExtractBy(Expression = "$.detail_url", Type = ExtractType.JsonPath)]
			public string Url { get; set; }

			[StoredAs("cat_id", StoredAs.ValueType.Text)]
			[PropertyExtractBy(Expression = "$.category", Type = ExtractType.JsonPath)]
			public string CatId { get; set; }

			[PropertyExtractBy(Expression = "Brand", Type = ExtractType.Enviroment)]
			[StoredAs("brand", StoredAs.ValueType.Varchar, false, 50)]
			public string Brand { get; set; }

			[PropertyExtractBy(Expression = "UserId", Type = ExtractType.Enviroment)]
			[StoredAs("uid", StoredAs.ValueType.Varchar, false, 100)]
			public string UserId { get; set; }

			[StoredAs("item_id", StoredAs.ValueType.Varchar, false, 100)]
			[PropertyExtractBy(Expression = "$.nid", Type = ExtractType.JsonPath)]
			public string ItemId { get; set; }

			[StoredAs("run_id", StoredAs.ValueType.Date)]
			public DateTime RunId => runId;

			[StoredAs("cdate", StoredAs.ValueType.Date)]
			public DateTime CDate => DateTime.Now;
		}
	}

	public class TmallGmvItemRequestStoping : RequestStoping
	{
		public override bool NeedStop(dynamic value)
		{
			return value.Sold == 0;
		}
	}
}