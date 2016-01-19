using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class MySqlFilePipeline : IPageModelPipeline
	{
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(Dictionary<Type, List<dynamic>> data, ISpider spider)
		{
			foreach (var pair in data)
			{
				Type type = pair.Key;
				bool isGeneric = typeof(IEnumerable).IsAssignableFrom(type);
				Type actualType = isGeneric ? type.GenericTypeArguments[0] : type;
				foreach (var r in pair.Value)
				{
					string filePath = GetDataFilePath(spider, actualType.Name);

					var properties = actualType.GetProperties();

					StringBuilder builder = new StringBuilder();
					foreach (var propertyInfo in properties)
					{
						builder.Append("'").Append(propertyInfo.GetValue(r).ToString()).Append("'").Append(",");
					}
					builder.Remove(builder.Length - 1, 1);
					builder.Append(Environment.NewLine);

					// 这里需要优化, 这个方法太慢了
					File.AppendAllText(filePath, builder.ToString(), Encoding.UTF8);
				}
			}
		}

		private string GetDataFilePath(ISpider spider, string name)
		{
			string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", spider.Identify);

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return Path.Combine(folderPath, name + ".sql");
		}

		public void Dispose()
		{
		}
	}
}
