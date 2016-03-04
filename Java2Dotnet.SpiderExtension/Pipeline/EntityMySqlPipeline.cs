using System.Data;
using System.Linq;
using System.Text;
using Java2Dotnet.Spider.Core;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class EntityMySqlPipeline : EntityGeneralPipeline
	{
		public EntityMySqlPipeline(DbSchema schema, JObject entityDefine, JObject argument) : base(schema, entityDefine, argument)
		{
		}

		protected override IDbConnection CreateConnection()
		{
			return new MySqlConnection(Argument.ConnectString);
		}

		protected override string GetInsertSql()
		{
			string columNames = string.Join(", ", Columns.Select(p => $"{p.Name}"));
			string values = string.Join(", ", Columns.Select(p => $"@{p.Name}"));

			var sqlBuilder = new StringBuilder();
			sqlBuilder.AppendFormat("INSERT IGNORE INTO `{0}`.`{1}` {2} {3};",
				Schema.DatabaseName,
				Schema.TableName,
				string.IsNullOrEmpty(columNames) ? string.Empty : $"({columNames})",
				string.IsNullOrEmpty(values) ? string.Empty : $" VALUES ({values})");

			return sqlBuilder.ToString();
		}

		protected override string GetCreateTableSql()
		{
			StringBuilder builder = new StringBuilder($"CREATE TABLE IF NOT EXISTS  `{Schema.DatabaseName}`.`{Schema.TableName}`  (");

			string columNames = string.Join(", ", Columns.Select(p => $"`{p.Name}` {ConvertToDbType(p.DataType)}"));
			builder.Append(columNames.Substring(0, columNames.Length));
			builder.Append(", `id` bigint AUTO_INCREMENT");
			builder.Append(", PRIMARY KEY(`id`)");

			foreach (var index in Indexs)
			{
				string name = string.Join("_", index.Select(c => c));
				string indexColumNames = string.Join(", ", index.Select(c => $"`{c}`"));
				builder.Append($", KEY `index_{name}` ({indexColumNames.Substring(0, indexColumNames.Length)})");
			}

			foreach (var unique in Uniques)
			{
				string name = string.Join("_", unique.Select(c => c));
				string uniqueColumNames = string.Join(", ", unique.Select(c => $"`{c}`"));
				builder.Append($", UNIQUE KEY `unique_{name}` ({uniqueColumNames.Substring(0, uniqueColumNames.Length)})");
			}


			builder.Append(") ENGINE=InnoDB  DEFAULT CHARSET=utf8");
			return builder.ToString();
		}

		protected override string GetCreateSchemaSql()
		{
			return $"CREATE SCHEMA IF NOT EXISTS `{Schema.DatabaseName}` DEFAULT CHARACTER SET utf8mb4 ;";
		}

		protected override string ConvertToDbType(string datatype)
		{
			int length = 0;
			var match = NumRegex.Match(datatype);
			length = match.Length == 0 ? 0 : int.Parse(match.Value);

			if (StringTypeRegex.IsMatch(datatype))
			{
				return length == 0 ? "varchar(100)" : $"varchar({length})";
			}

			if (IntTypeRegex.IsMatch(datatype))
			{
				return length == 0 ? "int(11)" : $"int({length})";
			}

			if (BigIntTypeRegex.IsMatch(datatype))
			{
				return length == 0 ? "int(11)" : $"int({length})";
			}

			if (FloatTypeRegex.IsMatch(datatype))
			{
				return length == 0 ? "float(11)" : $"float({length})";
			}

			if (DoubleTypeRegex.IsMatch(datatype))
			{
				return length == 0 ? "double(11)" : $"double({length})";
			}

			if ("text" == datatype)
			{
				return "text";
			}

			throw new SpiderExceptoin("Unsport datatype: " + datatype);
		}
	}
}
