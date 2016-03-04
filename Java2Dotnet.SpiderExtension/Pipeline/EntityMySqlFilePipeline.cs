using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Core.Utils;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class EntityMySqlFilePipeline : FilePersistentBase, IEntityPipeline
	{
		public class MySqlFilePipelineArgument
		{
			public string Directory { get; set; }
		}

		protected readonly DbSchema Schema;
		protected readonly List<Column> Columns;

		public EntityMySqlFilePipeline(DbSchema schema, JObject entityDefine, JObject argument)
		{
			Schema = schema;

			Columns = entityDefine.SelectTokens("$.fields[*]").Select(j => j.ToObject<Column>()).ToList();
			SetPath(argument.ToObject<MySqlFilePipelineArgument>()?.Directory);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Process(List<JObject> datas, ISpider spider)
		{
			FileInfo file = PrepareFile(Schema.TableName);

			using (StreamWriter printWriter = new StreamWriter(file.OpenWrite(), Encoding.UTF8))
			{
				foreach (var entry in datas)
				{
					StringBuilder builder = new StringBuilder();
					foreach (var column in Columns)
					{
						var value = entry.SelectToken($"$.{column}")?.ToString();
						if (!string.IsNullOrEmpty(value))
						{
							string correntValue = value.Replace("'", " ").Replace(",", " ");
							builder.Append("'").Append(correntValue).Append("'").Append(",");
						}
						else
						{
							builder.Append("'',");
						}
					}
					builder.Remove(builder.Length - 1, 1);
					builder.Append(Environment.NewLine);
					printWriter.WriteLine(builder);
				}
			}
		}

		public void Dispose()
		{
		}
	}
}
