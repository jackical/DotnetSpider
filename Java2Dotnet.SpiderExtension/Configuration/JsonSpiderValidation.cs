using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension.Configuration
{
	public class JsonSpiderValidation
	{
		public static bool Validate(JsonSpider spider, out List<string> messages)
		{
			bool correct = true;
			messages = new List<string>();

			// 1. 脚本未能正确序列化
			if (spider == null)
			{
				correct = false;
				messages.Add("Error 001: Script can't deserialize to a JsonSpider object.");
			}
			else
			{
				if (spider.Entities == null || spider.Entities.Count == 0)
				{
					messages.Add($"Error 002: Didn't define any data entity.");
				}
				else
				{
					// 2. 如果数据实体有嵌套或者属性是List, 则Pipeline只能选择mongodb.
					foreach (var entity in spider.Entities)
					{
						var fieldTokens = entity.SelectTokens("$.fields[*]")?.ToList();
						string entityName = entity.SelectToken("$.identity")?.ToString();

						if (string.IsNullOrEmpty(entityName))
						{
							if (correct)
							{
								correct = false;
							}
							messages.Add($"Error 003: Entity: {entityName} is null.");
						}

						if (fieldTokens == null || fieldTokens.Count == 0)
						{
							if (correct)
							{
								correct = false;
							}

							messages.Add($"Error 003: Entity: {entityName} has no field.");
						}

						if (fieldTokens != null)
						{
							foreach (var fieldToken in fieldTokens)
							{
								string fieldName = fieldToken.SelectToken("$.name")?.ToString();
								if (string.IsNullOrEmpty(fieldName))
								{
									messages.Add($"Error 003: Entity: {entityName}, Field index: {fieldTokens.IndexOf(fieldToken)} has no name.");
								}

								var dataTypeToken = fieldToken.SelectToken("$.datatype");
								if (dataTypeToken == null)
								{
									if (correct)
									{
										correct = false;
									}
									messages.Add($"Error 003: Entity: {entityName}, Field index: {fieldTokens.IndexOf(fieldToken)} has no datatype.");
								}
								else
								{
									if (dataTypeToken.Type != JTokenType.String && spider.Pipeline.PipelineType != PipelineType.MongoDb)
									{
										if (correct)
										{
											correct = false;
										}
										messages.Add($"Error 003: Entity: {entityName}, Field index: {fieldTokens.IndexOf(fieldToken)} is a class, when data entity is a loop type, pipeline should be mongodb only.");
									}
								}
							}
						}
					}
				}
			}



			return correct;
		}
	}
}
