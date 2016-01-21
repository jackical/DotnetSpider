using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Processor;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Formatter;

namespace Java2Dotnet.Spider.Extension.Processor
{
	/// <summary>
	/// The extension to PageProcessor for page model extractor.
	/// </summary>
	public class ModelPageProcessor : IPageProcessor
	{
		protected readonly IList<IPageModelExtractor> PageModelExtractorList = new List<IPageModelExtractor>();

		public Func<Page, IList<string>> GetCustomizeTargetUrls;

		protected ModelPageProcessor(Site site)
		{
			Site = site;
		}

		public virtual void Process(Page page)
		{
			foreach (IPageModelExtractor pageModelExtractor in PageModelExtractorList)
			{
				ExtractLinks(page, pageModelExtractor.GetHelpUrlRegionSelector(), pageModelExtractor.GetHelpUrlPatterns());

				dynamic process = pageModelExtractor.Process(page);
				if (process == null || (process is IEnumerable && !((IEnumerable)process).GetEnumerator().MoveNext()))
				{
					continue;
				}
				PostProcessPageModel(process);
				page.AddResultItem(pageModelExtractor.GetActualType().FullName, process);

				if (GetCustomizeTargetUrls == null)
				{
					ExtractLinks(page, pageModelExtractor.GetTargetUrlRegionSelector(), pageModelExtractor.GetTargetUrlPatterns(), pageModelExtractor.GetTargetUrlFormatter());
				}
				else
				{
					page.AddTargetRequests(GetCustomizeTargetUrls(page));
				}
			}
			if (page.ResultItems.Results.Count == 0)
			{
				page.ResultItems.IsSkip = true;
			}
		}

		/// <summary>
		/// 如果找不到则不返回URL, 不然返回的URL太多
		/// </summary>
		/// <param name="page"></param>
		/// <param name="urlRegionSelector"></param>
		/// <param name="urlPatterns"></param>
		/// <param name="formatter"></param>
		private void ExtractLinks(Page page, ISelector urlRegionSelector, IList<Regex> urlPatterns, IObjectFormatter formatter = null)
		{
			var links = urlRegionSelector == null ? new List<string>() : page.HtmlDocument.SelectList(urlRegionSelector).Links().GetAll();

			// check: 仔细考虑是放在前面, 还是在后面做 formatter, 我倾向于在前面. 对targetUrl做formatter则表示Start Url也应该是要符合这个规则的。
			if (formatter != null)
			{
				List<string> tmp = new List<string>();
				foreach (var link in links)
				{
					tmp.Add(formatter.Format(link));
				}
				links = tmp;
			}

			if (urlPatterns == null || urlPatterns.Count == 0)
			{
				page.AddTargetRequests(links);
				return;
			}

			foreach (Regex targetUrlPattern in urlPatterns)
			{
				foreach (string link in links)
				{
					if (targetUrlPattern.IsMatch(link))
					{
						page.AddTargetRequest(new Request(link, page.Request.NextDepth, page.Request.Extras));
					}
				}
			}
		}

		protected virtual void PostProcessPageModel(dynamic obj)
		{
		}

		public Site Site { get; protected set; }
	}

	public class ModelPageProcessor<T> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T>());
		}
	}

	public class ModelPageProcessor<T1, T2> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T1>());
			PageModelExtractorList.Add(new PageModelExtractor<T2>());
		}
	}

	public class ModelPageProcessor<T1, T2, T3> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T1>());
			PageModelExtractorList.Add(new PageModelExtractor<T2>());
			PageModelExtractorList.Add(new PageModelExtractor<T3>());
		}
	}

	public class ModelPageProcessor<T1, T2, T3, T4> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T1>());
			PageModelExtractorList.Add(new PageModelExtractor<T2>());
			PageModelExtractorList.Add(new PageModelExtractor<T3>());
			PageModelExtractorList.Add(new PageModelExtractor<T4>());
		}
	}

	public class ModelPageProcessor<T1, T2, T3, T4, T5> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T1>());
			PageModelExtractorList.Add(new PageModelExtractor<T2>());
			PageModelExtractorList.Add(new PageModelExtractor<T3>());
			PageModelExtractorList.Add(new PageModelExtractor<T4>());
			PageModelExtractorList.Add(new PageModelExtractor<T5>());
		}
	}

	public class ModelPageProcessor<T1, T2, T3, T4, T5, T6> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T1>());
			PageModelExtractorList.Add(new PageModelExtractor<T2>());
			PageModelExtractorList.Add(new PageModelExtractor<T3>());
			PageModelExtractorList.Add(new PageModelExtractor<T4>());
			PageModelExtractorList.Add(new PageModelExtractor<T5>());
			PageModelExtractorList.Add(new PageModelExtractor<T6>());
		}
	}

	public class ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T1>());
			PageModelExtractorList.Add(new PageModelExtractor<T2>());
			PageModelExtractorList.Add(new PageModelExtractor<T3>());
			PageModelExtractorList.Add(new PageModelExtractor<T4>());
			PageModelExtractorList.Add(new PageModelExtractor<T5>());
			PageModelExtractorList.Add(new PageModelExtractor<T6>());
			PageModelExtractorList.Add(new PageModelExtractor<T7>());
		}
	}

	public class ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T1>());
			PageModelExtractorList.Add(new PageModelExtractor<T2>());
			PageModelExtractorList.Add(new PageModelExtractor<T3>());
			PageModelExtractorList.Add(new PageModelExtractor<T4>());
			PageModelExtractorList.Add(new PageModelExtractor<T5>());
			PageModelExtractorList.Add(new PageModelExtractor<T6>());
			PageModelExtractorList.Add(new PageModelExtractor<T7>());
			PageModelExtractorList.Add(new PageModelExtractor<T8>());
		}
	}

	public class ModelPageProcessor<T1, T2, T3, T4, T5, T6, T7, T8, T9> : ModelPageProcessor
	{
		public ModelPageProcessor(Site site) : base(site)
		{
			PageModelExtractorList.Add(new PageModelExtractor<T1>());
			PageModelExtractorList.Add(new PageModelExtractor<T2>());
			PageModelExtractorList.Add(new PageModelExtractor<T3>());
			PageModelExtractorList.Add(new PageModelExtractor<T4>());
			PageModelExtractorList.Add(new PageModelExtractor<T5>());
			PageModelExtractorList.Add(new PageModelExtractor<T6>());
			PageModelExtractorList.Add(new PageModelExtractor<T7>());
			PageModelExtractorList.Add(new PageModelExtractor<T8>());
			PageModelExtractorList.Add(new PageModelExtractor<T9>());
		}
	}
}