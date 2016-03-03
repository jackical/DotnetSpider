using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Selector;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.Model.Formatter;
using Java2Dotnet.Spider.Extension.Utils;

namespace Java2Dotnet.Spider.Extension.Model
{
	/// <summary>
	/// The main internal logic of page model extractor.
	/// </summary>
	public class PageModelExtractor<T> : IPageModelExtractor
	{
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(PageModelExtractor<T>));

		private readonly Type _actualType;
		private List<FieldExtractor> _fieldExtractors;
		private TypeExtractor _typeExtractor;

		private readonly Regex _urlRegex = new Regex(@"((http|https|ftp):(\/\/|\\\\)((\w)+[.]){1，}(net|com|cn|org|cc|tv|[0-9]{1，3})(((\/[\~]*|\\[\~]*)(\w)+)|[.](\w)+)*(((([?](\w)+){1}[=]*))*((\w)+){1}([\&](\w)+[\=](\w)+)*)*)");
		private RequestStoping _requestRequestStoping;

		public PageModelExtractor()
		{
			_actualType = typeof(T);

			Init();
		}

		private FieldExtractor GetAttributeExtractBy(PropertyInfo field)
		{
			FieldExtractor fieldExtractor = null;
			PropertyExtractBy extractBy = (PropertyExtractBy)field.GetCustomAttribute(typeof(PropertyExtractBy));
			if (extractBy != null)
			{
				ISelector selector = ExtractorUtils.GetSelector(extractBy);

				fieldExtractor = new FieldExtractor(field, selector, extractBy.Expression, extractBy.NotNull, field.PropertyType.IsGenericType);
			}

			return fieldExtractor;
		}

		private void Init()
		{
			InitTypeExtractors();

			_fieldExtractors = new List<FieldExtractor>();
			foreach (PropertyInfo field in _actualType.GetProperties())
			{
				FieldExtractor fieldExtractor = GetAttributeExtractBy(field);

				if (fieldExtractor != null)
				{
					CheckFormat(field, fieldExtractor);
					_fieldExtractors.Add(fieldExtractor);
				}
			}
		}

		private void CheckFormat(PropertyInfo field, FieldExtractor fieldExtractor)
		{
			//check custom formatter
			Attribute.Formatter formatter = field.GetCustomAttribute<Attribute.Formatter>();

			IObjectFormatter objectFormatter;
			if (formatter?.FormatterType != null)
			{
				fieldExtractor.ObjectFormatter = (IObjectFormatter)Activator.CreateInstance(formatter.FormatterType);
				if (formatter.UseDefaultFormatter)
				{
					fieldExtractor.ObjectFormatter.NextFormatter = FormatterFactory.GetFormatter(field.PropertyType);
				}
				fieldExtractor.ObjectFormatter.InitParam(formatter.Value);
				return;
			}
			else
			{
				objectFormatter = FormatterFactory.GetFormatter(field.PropertyType);
			}

			if (!fieldExtractor.Multi)
			{
				if (objectFormatter == null)
				{
					throw new Exception("Can't find formatter for field " + field.Name + " of type " + field.PropertyType.Name);
				}
				fieldExtractor.ObjectFormatter = objectFormatter;
			}
			else
			{
				// 注释掉, 已经改为从属性得IsMulti值了, 不可能两边不一至。
				//if (!field.PropertyType.IsGenericType)
				//{
				//	throw new SpiderExceptoin("Field " + field.Name + " must be generice type.");
				//}

				Type[] genericType = field.PropertyType.GetGenericArguments();
				if (genericType.Length != 1)
				{
					throw new SpiderExceptoin("Field " + field.Name + " must be single generice type like List<T> Hashset<T>. Not support Diction<K,V> etc...");
				}

				MethodInfo method = fieldExtractor.Field.PropertyType.GetMethod("Add", genericType);
				if (method == null)
				{
					throw new SpiderExceptoin("Field " + field.Name + " did not contains Add(T t) method.");
				}

				if (objectFormatter == null)
				{
					if (formatter != null)
						throw new SpiderExceptoin("Can't find formatter for field " + field.Name + " of type " + formatter.SubType);
				}
				else
				{
					fieldExtractor.ObjectFormatter = objectFormatter;
				}
			}
		}

		//private FieldExtractor GetAttributeExtractByUrl(PropertyInfo field)
		//{
		//	FieldExtractor fieldExtractor = null;
		//	ExtractByUrl extractByUrl = field.GetCustomAttribute<ExtractByUrl>();
		//	if (extractByUrl != null)
		//	{
		//		string regexPattern = extractByUrl.Expession;
		//		if (string.IsNullOrEmpty(regexPattern.Trim()))
		//		{
		//			regexPattern = ".*";
		//		}

		//		fieldExtractor = new FieldExtractor(field, new RegexSelector(regexPattern), extractByUrl.Expession, ExtractSource.Url, extractByUrl.NotNull, field.PropertyType.IsGenericType);
		//	}
		//	return fieldExtractor;
		//}

		private void InitTypeExtractors()
		{
			var targetUrlAttributes = _actualType.GetCustomAttributes<TargetUrl>().ToList();

			if (targetUrlAttributes.Count > 0)
			{
				foreach (var targetUrlAttribute in targetUrlAttributes)
				{
					var targetUrlExtractInfo = new TargetUrlExtractInfo();

					string[] value = targetUrlAttribute.Value;

					if (value != null)
					{
						foreach (string s in value)
						{
							targetUrlExtractInfo.Patterns.Add(new Regex("(" + s.Replace(".", "\\.").Replace("*", "[^\"'#]*") + ")"));
						}
					}
					else
					{
						targetUrlExtractInfo.Patterns.Add(new Regex("(.*)"));
					}

					targetUrlExtractInfo.TargetUrlRegionSelector = string.IsNullOrEmpty(targetUrlAttribute.SourceRegion) ? null : new XPathSelector(targetUrlAttribute.SourceRegion);

					TargetUrlFormatter formatter = _actualType.GetCustomAttribute<TargetUrlFormatter>();

					if (formatter?.FormatterType != null)
					{
						targetUrlExtractInfo.TargetUrlFormatter = (IObjectFormatter)Activator.CreateInstance(formatter.FormatterType);
						targetUrlExtractInfo.TargetUrlFormatter.InitParam(formatter.Value);
					}

					TargetUrlExtractInfos.Add(targetUrlExtractInfo);
				}
			}

			TypeExtractBy extractByAttribute = _actualType.GetCustomAttribute<TypeExtractBy>();
			if (extractByAttribute != null)
			{
				_typeExtractor = ExtractorUtils.GetTypeExtractor(extractByAttribute);
			}

			_requestRequestStoping = _actualType.GetCustomAttribute<RequestStoping>(true);
		}

		public dynamic Process(Page page)
		{
			bool matched = false;
			if (page.Url != null)
			{
				if (TargetUrlExtractInfos != null)
				{
					if (TargetUrlExtractInfos.Count == 0)
					{
						matched = true;
					}
					else
					{
						foreach (var targetUrlExtractInfo in TargetUrlExtractInfos)
						{
							foreach (Regex targetPattern in targetUrlExtractInfo.Patterns)
							{
								string url = page.Url;
								//check
								if (targetPattern.IsMatch(url))
								{
									matched = true;
								}
								else
								{
									Logger.Warn($"Url {url} is not match your TargetUrl attribute. Cause select 0 element.");
								}
							}
						}
					}
				}
				else
				{
					matched = true;
				}
			}

			if (!matched)
			{
				return null;
			}
			if (_typeExtractor == null)
			{
				return ProcessSingle(page, page.Content);
			}
			else
			{
				if (_typeExtractor.Multi)
				{
					IList<dynamic> list = _typeExtractor.Selector.SelectList(page.Content);
					if (_typeExtractor.Count < int.MaxValue)
					{
						list = list.Take(_typeExtractor.Count).ToList();
					}

					List<T> result = new List<T>();
					foreach (var item in list)
					{
						T obj = ProcessSingle(page, item);
						if (obj != null)
						{
							result.Add(obj);
						}
					}
					return result;
				}
				else
				{
					string select = _typeExtractor.Selector.Select(page.Content);
					if (select == null)
					{
						return null;
					}
					return ProcessSingle(page, select);
				}
			}
		}

		private T ProcessSingle(Page page, string content)
		{
			var instance = Activator.CreateInstance(_actualType);
			foreach (FieldExtractor fieldExtractor in _fieldExtractors)
			{
				if (fieldExtractor.Multi)
				{
					if (fieldExtractor.Selector is EnviromentSelector)
					{
						continue;
					}

					IList<dynamic> value = fieldExtractor.Selector.SelectList(content);

					if ((value == null || value.Count == 0))
					{
						if (fieldExtractor.NotNull)
						{
							return default(T);
						}
					}
					else
					{
						if (fieldExtractor.ObjectFormatter != null)
						{
							IList<dynamic> converted = Convert(value, fieldExtractor.ObjectFormatter);

							dynamic field = fieldExtractor.Field.GetValue(instance) ?? Activator.CreateInstance(fieldExtractor.Field.PropertyType);

							Type[] genericType = fieldExtractor.Field.PropertyType.GetGenericArguments();
							MethodInfo method = fieldExtractor.Field.PropertyType.GetMethod("Add", genericType);

							if (fieldExtractor.Download)
							{
								List<string> urlList = new List<string>();
								foreach (var url in converted)
								{
									// 不需要判断为空, 前面已经判断过了
									if (_urlRegex.IsMatch(url))
									{
										urlList.Add(url);
									}
								}
								page.AddResultItem(Page.Images, urlList);
							}

							foreach (var v in converted)
							{
								method.Invoke(field, new object[] { v });
							}

							fieldExtractor.Field.SetValue(instance, field);
						}
						else
						{
							fieldExtractor.Field.SetValue(instance, value);

							if (fieldExtractor.Download)
							{
								List<string> urlList = new List<string>();
								foreach (var url in value)
								{
									// 不需要判断为空, 前面已经判断过了
									if (_urlRegex.IsMatch(url))
									{
										urlList.Add(url);
									}
								}
								page.AddResultItem(Page.Images, urlList);
							}
						}
					}
				}
				else
				{
					string value;
					EnviromentSelector enviromentSelector;
					if ((enviromentSelector = (fieldExtractor.Selector as EnviromentSelector)) != null)
					{
						value = GetEnviromentValue(enviromentSelector.Field, page);
					}
					else
					{
						value = fieldExtractor.Selector.Select(content);
					}

					if (value == null)
					{
						if (fieldExtractor.NotNull)
						{
							return default(T);
						}
					}
					else
					{
						if (fieldExtractor.ObjectFormatter != null)
						{
							dynamic converted = Convert(value, fieldExtractor.ObjectFormatter);

							if (converted == null && fieldExtractor.NotNull)
							{
								return default(T);
							}

							fieldExtractor.Field.SetValue(instance, converted);
						}
						else
						{
							fieldExtractor.Field.SetValue(instance, value);
						}

						if (fieldExtractor.Download)
						{
							// 不需要判断为空, 前面已经判断过了
							if (_urlRegex.IsMatch(value))
							{
								page.AddResultItem(Page.Images, value);
							}
						}
					}
				}
			}

			if (_requestRequestStoping != null)
			{
				if (_requestRequestStoping.NeedStop(instance) && !page.MissTargetUrls)
				{
					page.MissTargetUrls = true;
				}
			}

			IAfterExtractor afterExtractor = instance as IAfterExtractor;
			afterExtractor?.AfterProcess(page);

			var customize = instance as ICustomize;
			customize?.Customize();

			return (T)instance;
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

		private dynamic Convert(dynamic value, IObjectFormatter objectFormatter)
		{
			try
			{
				object format = objectFormatter.Format(value);
				Logger.DebugFormat("String {0} is converted to {1}", value, format);
				return format;
			}
			catch (Exception e)
			{
				Logger.Error("convert " + value + " to " + objectFormatter.GetType() + " error!", e);
			}
			return null;
		}

		private IList<dynamic> Convert(IList<dynamic> values, IObjectFormatter objectFormatter)
		{
			return values.Select(value => Convert(value, objectFormatter)).ToList();
		}

		public Type ActualType
		{
			get { return _actualType; }
		}

		public List<TargetUrlExtractInfo> TargetUrlExtractInfos { get; } = new List<TargetUrlExtractInfo>();
	}
}