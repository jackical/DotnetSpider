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
	public interface IPageModelExtractor
	{
		dynamic Process(Page page);
		Type GetActualType();
		IList<Regex> GetTargetUrlPatterns();
		IObjectFormatter GetTargetUrlFormatter();
		IList<Regex> GetHelpUrlPatterns();
		ISelector GetTargetUrlRegionSelector();
		ISelector GetHelpUrlRegionSelector();
	}

	/// <summary>
	/// The main internal logic of page model extractor.
	/// </summary>
	public class PageModelExtractor<T> : IPageModelExtractor
	{
		private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(typeof(PageModelExtractor<T>));
		private readonly IList<Regex> _targetUrlPatterns = new List<Regex>();
		private ISelector _targetUrlRegionSelector;
		private readonly IList<Regex> _helpUrlPatterns = new List<Regex>();
		private ISelector _helpUrlRegionSelector;
		private readonly Type _actualType;
		private List<FieldExtractor> _fieldExtractors;
		private IObjectFormatter _targetUrlFormatter;
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

				ExtractSource source = extractBy.Source;
				if (extractBy.Type == ExtractType.Enviroment)
				{
					source = ExtractSource.Enviroment;
				}
				if (extractBy.Type == ExtractType.JsonPath)
				{
					source = ExtractSource.Json;
				}

				fieldExtractor = new FieldExtractor(field, selector, extractBy.Expression, source, extractBy.NotNull, field.PropertyType.IsGenericType);
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
				FieldExtractor fieldExtractorTmp = GetAttributeExtractCombo(field);

				if (fieldExtractor == null && fieldExtractorTmp != null)
				{
					fieldExtractor = fieldExtractorTmp;
				}
				fieldExtractorTmp = GetAttributeExtractByUrl(field);
				if (fieldExtractor == null && fieldExtractorTmp != null)
				{
					fieldExtractor = fieldExtractorTmp;
				}
				if (fieldExtractor != null)
				{
					CheckFormat(field, fieldExtractor);
					_fieldExtractors.Add(fieldExtractor);
				}
			}
		}

		private FieldExtractor GetAttributeExtractCombo(PropertyInfo field)
		{
			FieldExtractor fieldExtractor = null;
			ComboExtract comboExtract = field.GetCustomAttribute<ComboExtract>();
			if (comboExtract != null)
			{
				PropertyExtractBy[] extractBies = comboExtract.Value;
				ISelector selector;
				switch (comboExtract.Op)
				{
					case ComboExtract.ExtractOp.And:
						selector = new AndSelector(ExtractorUtils.GetSelectors(extractBies));
						break;
					case ComboExtract.ExtractOp.Or:
						selector = new OrSelector(ExtractorUtils.GetSelectors(extractBies));
						break;
					default:
						selector = new AndSelector(ExtractorUtils.GetSelectors(extractBies));
						break;
				}
				fieldExtractor = new FieldExtractor(field, selector, null, comboExtract.Source,
						comboExtract.NotNull, comboExtract.Multi || field.PropertyType.IsGenericType);
			}

			return fieldExtractor;
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

		private FieldExtractor GetAttributeExtractByUrl(PropertyInfo field)
		{
			FieldExtractor fieldExtractor = null;
			ExtractByUrl extractByUrl = field.GetCustomAttribute<ExtractByUrl>();
			if (extractByUrl != null)
			{
				string regexPattern = extractByUrl.Expession;
				if (string.IsNullOrEmpty(regexPattern.Trim()))
				{
					regexPattern = ".*";
				}

				fieldExtractor = new FieldExtractor(field, new RegexSelector(regexPattern), extractByUrl.Expession, ExtractSource.Url, extractByUrl.NotNull, field.PropertyType.IsGenericType);
			}
			return fieldExtractor;
		}

		private void InitTypeExtractors()
		{
			TargetUrl targetUrlAttribute = _actualType.GetCustomAttribute<TargetUrl>();

			if (targetUrlAttribute == null)
			{
				_targetUrlPatterns.Add(new Regex("(.*)"));
			}
			else
			{
				string[] value = targetUrlAttribute.Value;

				if (value != null)
				{
					foreach (string s in value)
					{
						_targetUrlPatterns.Add(new Regex("(" + s.Replace(".", "\\.").Replace("*", "[^\"'#]*") + ")"));
					}
				}
				else
				{
					_targetUrlPatterns.Add(new Regex("(.*)"));
				}

				if (targetUrlAttribute.ExtractType == ExtractType.XPath)
				{
					_targetUrlRegionSelector = new XPathSelector(string.IsNullOrEmpty(targetUrlAttribute.SourceRegion) ? "." : targetUrlAttribute.SourceRegion);
				}
				else if (targetUrlAttribute.ExtractType == ExtractType.JsonPath)
				{
					_targetUrlRegionSelector = new JsonPathSelector(string.IsNullOrEmpty(targetUrlAttribute.SourceRegion) ? "$." : targetUrlAttribute.SourceRegion);
				}

				TargetUrlFormatter formatter = _actualType.GetCustomAttribute<TargetUrlFormatter>();

				if (formatter?.FormatterType != null)
				{
					_targetUrlFormatter = (IObjectFormatter)Activator.CreateInstance(formatter.FormatterType);
					_targetUrlFormatter.InitParam(formatter.Value);
				}
			}

			HelpUrl helpAnnotation = _actualType.GetCustomAttribute<HelpUrl>();
			if (helpAnnotation != null)
			{
				string[] value = helpAnnotation.Value;
				foreach (string s in value)
				{
					_helpUrlPatterns.Add(new Regex("(" + s.Replace(".", "\\.").Replace("*", "[^\"'#]*") + ")"));
				}
				if (!string.IsNullOrEmpty(helpAnnotation.SourceRegion))
				{
					_helpUrlRegionSelector = new XPathSelector(helpAnnotation.SourceRegion);
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
				foreach (Regex targetPattern in _targetUrlPatterns)
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
			if (!matched)
			{
				return null;
			}
			if (_typeExtractor == null)
			{
				return ProcessSingle(page, null, true);
			}
			else
			{
				if (_typeExtractor.Multi)
				{
					IList<SelectedNode> list = _typeExtractor.Selector.SelectList(new SelectedNode() { Result = page.RawText, Type = ResultType.String });
					if (_typeExtractor.Count < long.MaxValue)
					{
						list = list.Take((int)_typeExtractor.Count).ToList();
					}

					List<T> result = new List<T>();
					foreach (var item in list)
					{
						T obj = ProcessSingle(page, item, false);
						if (obj != null)
						{
							result.Add(obj);

						}
					}
					return result;
				}
				else
				{
					SelectedNode select = _typeExtractor.Selector.Select(new SelectedNode() { Type = ResultType.String, Result = page.RawText });
					return ProcessSingle(page, select, false);
				}
			}
		}

		private T ProcessSingle(Page page, SelectedNode content, bool isEntire)
		{
			var instance = Activator.CreateInstance(_actualType);
			foreach (FieldExtractor fieldExtractor in _fieldExtractors)
			{
				if (fieldExtractor.Multi)
				{
					IList<string> value;
					switch (fieldExtractor.Source)
					{
						case ExtractSource.RawHtml:
							value = page.HtmlDocument.SelectList(fieldExtractor.Selector).Value;
							break;
						case ExtractSource.Html:
							value = isEntire ? page.HtmlDocument.SelectList(fieldExtractor.Selector).Value : fieldExtractor.Selector.SelectList(content)?.ToStringList();
							break;
						case ExtractSource.Json:
							value = isEntire ? page.GetJson().SelectList(fieldExtractor.Selector).Value : fieldExtractor.Selector.SelectList(content)?.ToStringList();
							break;
						case ExtractSource.Url:
							value = fieldExtractor.Selector.SelectList(new SelectedNode() { Result = page.Url, Type = ResultType.String })?.ToStringList();
							break;
						case ExtractSource.Enviroment:
							{
								value = GetEnviromentValue(fieldExtractor.Expression, page).ToString();
								break;
							}
						default:
							value = fieldExtractor.Selector.SelectList(content)?.ToStringList();
							break;
					}
					if ((value == null || value.Count == 0) && fieldExtractor.NotNull)
					{
						return default(T);
					}

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
				else
				{
					string value;

					switch (fieldExtractor.Source)
					{
						case ExtractSource.RawHtml:
							value = page.HtmlDocument.Select(fieldExtractor.Selector).Value;
							break;
						case ExtractSource.Html:
							value = isEntire ? page.HtmlDocument.Select(fieldExtractor.Selector).Value : fieldExtractor.Selector.Select(content)?.ToString();
							break;
						case ExtractSource.Json:
							value = isEntire ? page.GetJson().SelectList(fieldExtractor.Selector).Value : fieldExtractor.Selector.Select(content)?.ToString();
							break;
						case ExtractSource.Url:
							value = fieldExtractor.Selector.Select(new SelectedNode() { Result = page.Url, Type = ResultType.String })?.ToString();
							break;
						case ExtractSource.Enviroment:
							{
								value = GetEnviromentValue(fieldExtractor.Expression, page).ToString();
								break;
							}
						default:
							value = fieldExtractor.Selector.Select(content)?.ToString();
							break;
					}

					if (value == null && fieldExtractor.NotNull)
					{
						return default(T);
					}
					if (fieldExtractor.ObjectFormatter != null)
					{
						//if (!string.IsNullOrEmpty(fieldExtractor.Expresion))
						//{
						//	MemoryStream stream = new MemoryStream();
						//	StreamWriter writer = new StreamWriter(stream);
						//	writer.Write(fieldExtractor.Expresion.EndsWith(";") ? fieldExtractor.Expresion : fieldExtractor.Expresion + ";");
						//	writer.Flush();

						//	// convert stream to string  
						//	stream.Position = 0;
						//	AntlrInputStream input = new AntlrInputStream(stream);

						//	ModifyScriptLexer lexer = new ModifyScriptLexer(input);
						//	CommonTokenStream tokens = new CommonTokenStream(lexer);

						//	ModifyScriptVisitor modifyScriptVisitor = new ModifyScriptVisitor(value, fieldExtractor.Field);
						//	ModifyScriptParser parser = new ModifyScriptParser(tokens);

						//	modifyScriptVisitor.Visit(parser.stats());
						//	value = modifyScriptVisitor.Value;
						//	page.MissTargetUrls = modifyScriptVisitor.NeedStop;
						//}

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

		private dynamic GetEnviromentValue(string expression, Page page)
		{
			if ("url" == expression)
			{
				return page.Url;
			}

			if ("targeturl" == expression)
			{
				return page.TargetUrl;
			}

			return page.Request.GetExtra(expression);
		}

		private dynamic Convert(string value, IObjectFormatter objectFormatter)
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

		private IList<dynamic> Convert(IList<string> values, IObjectFormatter objectFormatter)
		{
			return values.Select(value => Convert(value, objectFormatter)).ToList();
		}

		public Type GetActualType()
		{
			return _actualType;
		}

		public IList<Regex> GetTargetUrlPatterns()
		{
			return _targetUrlPatterns;
		}

		public IObjectFormatter GetTargetUrlFormatter()
		{
			return _targetUrlFormatter;
		}

		public IList<Regex> GetHelpUrlPatterns()
		{
			return _helpUrlPatterns;
		}

		public ISelector GetTargetUrlRegionSelector()
		{
			return _targetUrlRegionSelector;
		}

		public ISelector GetHelpUrlRegionSelector()
		{
			return _helpUrlRegionSelector;
		}
	}
}