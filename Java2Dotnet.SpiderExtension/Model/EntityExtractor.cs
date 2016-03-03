using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Configuration;
using Java2Dotnet.Spider.Lib;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension.Model
{
	public class EntityExtractor : IEntityExtractor
	{
		private readonly JObject _entityDefine;

		public EntityExtractor(string entityName, JObject entityDefine)
		{
			_entityDefine = entityDefine;
			TargetUrlExtractInfos = GenerateTargetUrlExtractInfos(entityDefine);
			EntityName = entityName;
		}

		private List<TargetUrlExtractInfo> GenerateTargetUrlExtractInfos(JObject entityDefine)
		{
			List<TargetUrlExtractInfo> results = new List<TargetUrlExtractInfo>();
			var targetUrlTokens = entityDefine.SelectTokens("$.targeturls[*]");
			foreach (var targetUrlToken in targetUrlTokens)
			{
				var patterns = targetUrlToken.SelectToken("$.values").ToObject<HashSet<string>>();
				var sourceregionToken = targetUrlToken.SelectToken("$.sourceregion");
				results.Add(new TargetUrlExtractInfo()
				{
					Patterns = patterns == null || patterns.Count == 0 ? new List<Regex>() { new Regex("(.*)") } : patterns.Select(p => new Regex(p)).ToList(),
					TargetUrlRegionSelector = sourceregionToken == null || !sourceregionToken.HasValues ? null : Selectors.XPath(sourceregionToken.ToString())
				});
			}

			return results;
		}

		public dynamic Process(Page page)
		{
			bool isMulti = _entityDefine.SelectToken("$.multi").ToObject<bool>();
			ISelector selector = GetSelector(_entityDefine.SelectToken("$.selector").ToObject<ExtractType>(), _entityDefine.SelectToken("$.expression").ToString());

			if (isMulti)
			{
				var list = page.Selectable.SelectList(selector).Nodes();
				var countToken = _entityDefine.SelectToken("$.count");
				if (countToken != null)
				{
					int count = countToken.ToObject<int>();
					list = list.Take(count).ToList();
				}

				List<dynamic> result = new List<dynamic>();
				foreach (var item in list)
				{
					dynamic obj = ProcessSingle(page, item, _entityDefine);
					if (obj != null)
					{
						result.Add(obj);
					}
				}
				return result;
			}
			else
			{
				var select = page.Selectable.Select(selector);
				if (select == null)
				{
					return null;
				}
				return ProcessSingle(page, select, _entityDefine);
			}
		}

		private string GetEnviromentValue(string field, Page page)
		{
			if (field.ToLower() == "url")
			{
				return page.Url;
			}

			if (field.ToLower() == "targeturl")
			{
				return page.TargetUrl;
			}

			return page.Request.GetExtra(field);
		}

		private JObject ProcessSingle(Page page, ISelectable item, JToken entityDefine)
		{
			JObject dataItem = new JObject();

			foreach (var field in entityDefine.SelectTokens("$.fields[*]"))
			{
				var datatype = field.SelectToken("$.datatype");
				bool isEntity = VerifyIfEntity(datatype);

				var multiToken = field.SelectToken("$.multi");
				bool isMulti = multiToken?.ToObject<bool>() ?? false;

				ISelector selector = GetSelector(field.SelectToken("$.selector").ToObject<ExtractType>(),
						field.SelectToken("$.expression").ToString());

				string propertyName = field.SelectToken("$.name").ToString();

				if (!isEntity)
				{
					if (selector is EnviromentSelector)
					{
						var enviromentSelector = selector as EnviromentSelector;
						dataItem.Add(propertyName, GetEnviromentValue(enviromentSelector.Field, page));
					}
					else
					{
						if (isMulti)
						{
							var propertyValues = item.SelectList(selector).Value;
							var countToken = _entityDefine.SelectToken("$.count");
							if (countToken != null)
							{
								int count = countToken.ToObject<int>();
								propertyValues = propertyValues.Take(count).ToList();
							}
							dataItem.Add(propertyName, new JArray(propertyValues));
						}
						else
						{
							dataItem.Add(propertyName, new JValue(item.Select(selector).Value));
						}
					}

				}
				else
				{
					if (isMulti)
					{
						var propertyValues = item.SelectList(selector).Nodes();
						var countToken = _entityDefine.SelectToken("$.count");
						if (countToken != null)
						{
							int count = countToken.ToObject<int>();
							propertyValues = propertyValues.Take(count).ToList();
						}

						List<JObject> result = new List<JObject>();
						foreach (var entity in propertyValues)
						{
							JObject obj = ProcessSingle(page, entity, datatype);
							if (obj != null)
							{
								result.Add(obj);
							}
						}
						dataItem.Add(propertyName, new JArray(result));
					}
					else
					{
						var select = item.Select(selector);
						if (select == null)
						{
							return null;
						}
						var perpertyValue = ProcessSingle(page, select, datatype);
						dataItem.Add(propertyName, new JObject(perpertyValue));
					}
				}
			}

			return dataItem;
		}

		private bool VerifyIfEntity(JToken datatype)
		{
			return datatype.Type == JTokenType.Object;
		}

		/// <summary>
		/// 注意: 只有在Html页面中才能取得目标链接, 如果是Json数据, 一般是不会出现目标页面(下一页...）
		/// </summary>
		public List<TargetUrlExtractInfo> TargetUrlExtractInfos { get; }
		public string EntityName { get; }

		private static ISelector GetSelector(ExtractType selector, string expression)
		{
			switch (selector)
			{
				case ExtractType.Css:
					{
						return new CssHtmlSelector(expression);
					}
				case ExtractType.Enviroment:
					{
						return new EnviromentSelector(expression);
					}
				case ExtractType.JsonPath:
					{
						return new JsonPathSelector(expression);
					}
				case ExtractType.Regex:
					{
						return new RegexSelector(expression);
					}
				case ExtractType.XPath:
					{
						return new XPathSelector(expression);
					}
			}
			throw new SpiderExceptoin("Not support selector: " + selector);
		}
	}
}
