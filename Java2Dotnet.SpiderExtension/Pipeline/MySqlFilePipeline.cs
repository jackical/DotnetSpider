using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class MySqlFilePipeline<T> : IPageModelPipeline<T>
	{
		private readonly Type _type;
		private readonly PropertyInfo[] _propertyInfos;

		public MySqlFilePipeline()
		{
			_type = typeof(T);
			
			_propertyInfos = _type.GetProperties();
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(List<T> data, ISpider spider)
		{
			string filePath = GetDataFilePath(spider, _type.Name);
			foreach (var d in data)
			{
				StringBuilder builder = new StringBuilder();
				foreach (var propertyInfo in _propertyInfos)
				{
					object value = propertyInfo.GetValue(d);
					if (value != null)
					{
						string correntValue = value.ToString().Replace("'", " ").Replace(",", " ");
						builder.Append("'").Append(correntValue).Append("'").Append(",");
					}
					else
					{
						builder.Append("'',");
					}
				}
				builder.Remove(builder.Length - 1, 1);
				builder.Append(Environment.NewLine);

				// 这里需要优化, 这个方法太慢了
				File.AppendAllText(filePath, builder.ToString(), Encoding.UTF8);
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
