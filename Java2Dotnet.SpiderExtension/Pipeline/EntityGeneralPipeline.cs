using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Java2Dotnet.Spider.Core;
using Newtonsoft.Json.Linq;
using Dapper;
using Java2Dotnet.Spider.Extension.DbSupport;
using Java2Dotnet.Spider.Extension.Utils;
using Java2Dotnet.Spider.Lib;
using PropertyAttributes = System.Reflection.PropertyAttributes;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public abstract class EntityGeneralPipeline : IEntityPipeline
	{
		public class Column
		{
			public string Name { get; set; }
			public string DataType { get; set; }

			public override string ToString()
			{
				return $"{Name},{DataType}";
			}
		}

		protected string ConnectString { get; set; }
		protected readonly List<Column> Columns;

		protected abstract IDbConnection CreateConnection();
		protected abstract string GetInsertSql();
		protected abstract string GetCreateTableSql();
		protected abstract string GetCreateSchemaSql();
		protected readonly Schema Schema;

		protected readonly Type Type;

		protected List<List<string>> Indexs { get; set; } = new List<List<string>>();
		protected List<List<string>> Uniques { get; set; } = new List<List<string>>();
		protected List<string> Primary { get; set; }
		protected string AutoIncrement { get; set; }

		protected abstract string ConvertToDbType(string datatype);

		protected EntityGeneralPipeline(Schema schema, JObject entityDefine, string connectString)
		{
			ConnectString = connectString;

			Schema = GenerateSchema(schema);
			Columns = entityDefine.SelectTokens("$.Fields[*]").Select(j => j.ToObject<Column>()).ToList();

			Primary = entityDefine.SelectToken("$.Primary").ToObject<List<string>>();
			AutoIncrement = entityDefine.SelectToken("$.AutoIncrement")?.ToString();
			foreach (var index in entityDefine.SelectTokens("$.Indexs[*]"))
			{
				Indexs.Add(index.ToObject<List<string>>());
			}

			foreach (var index in entityDefine.SelectTokens("$.Uniques[*]"))
			{
				Uniques.Add(index.ToObject<List<string>>());
			}

			Type = GenerateType(schema, Columns);
		}

		private Schema GenerateSchema(Schema schema)
		{
			switch (schema.Suffix)
			{
				case TableSuffix.FirstDayOfThisMonth:
					{
						schema.TableName += "_" + DateTimeUtil.FirstDayofThisMonth.ToString("yyyy_MM_dd");
						break;
					}
				case TableSuffix.Monday:
					{
						schema.TableName += "_" + DateTimeUtil.FirstDayofThisWeek.ToString("yyyy_MM_dd");
						break;
					}
			}
			return schema;
		}

		public virtual void Initialize()
		{
			using (IDbConnection conn = CreateConnection())
			{
				conn.Execute(GetCreateSchemaSql());
				conn.Execute(GetCreateTableSql());
			}
		}

		public void Process(List<JObject> datas, ISpider spider)
		{
			using (var conn = CreateConnection())
			{
				conn.Execute(GetInsertSql(), datas.Select(j => j.ToObject(Type)));
			}
		}

		public void Dispose()
		{
		}

		private Type GenerateType(Schema schema, List<Column> columns)
		{
			AppDomain currentAppDomain = AppDomain.CurrentDomain;
			AssemblyName assyName = new AssemblyName("DotnetSpiderAss_" + schema.TableName);

			AssemblyBuilder assyBuilder = currentAppDomain.DefineDynamicAssembly(assyName, AssemblyBuilderAccess.Run);

			ModuleBuilder modBuilder = assyBuilder.DefineDynamicModule("DotnetSpiderMod_" + schema.TableName);

			TypeBuilder typeBuilder = modBuilder.DefineType("type_" + schema.TableName, TypeAttributes.Class | TypeAttributes.Public);

			foreach (var column in columns)
			{
				AddProperty(typeBuilder, column.Name, Convert(column.DataType.ToLower()));
			}

			return (typeBuilder.CreateType());
		}

		private void AddProperty(TypeBuilder tb, string name, Type type)
		{
			var property = tb.DefineProperty(name, PropertyAttributes.HasDefault, type, null);

			FieldBuilder field = tb.DefineField($"_{name}", type, FieldAttributes.Private);

			MethodAttributes getOrSetAttribute = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

			MethodBuilder getAccessor = tb.DefineMethod($"get_{name}", getOrSetAttribute, type, Type.EmptyTypes);

			ILGenerator getIl = getAccessor.GetILGenerator();
			getIl.Emit(OpCodes.Ldarg_0);
			getIl.Emit(OpCodes.Ldfld, field);
			getIl.Emit(OpCodes.Ret);

			MethodBuilder setAccessor = tb.DefineMethod($"set_{name}", getOrSetAttribute, null, new[] { type });

			ILGenerator setIl = setAccessor.GetILGenerator();
			setIl.Emit(OpCodes.Ldarg_0);
			setIl.Emit(OpCodes.Ldarg_1);
			setIl.Emit(OpCodes.Stfld, field);
			setIl.Emit(OpCodes.Ret);

			property.SetGetMethod(getAccessor);
			property.SetSetMethod(setAccessor);
		}

		private Type Convert(string datatype)
		{
			if (RegexUtil.StringTypeRegex.IsMatch(datatype))
			{
				return typeof(string);
			}

			if (RegexUtil.IntTypeRegex.IsMatch(datatype))
			{
				return typeof(int);
			}

			if (RegexUtil.BigIntTypeRegex.IsMatch(datatype))
			{
				return typeof(long);
			}

			if (RegexUtil.FloatTypeRegex.IsMatch(datatype))
			{
				return typeof(float);
			}

			if (RegexUtil.DoubleTypeRegex.IsMatch(datatype))
			{
				return typeof(double);
			}
			if (RegexUtil.DateTypeRegex.IsMatch(datatype) || RegexUtil.TimeStampTypeRegex.IsMatch(datatype))
			{
				return typeof(DateTime);
			}

			if (RegexUtil.TimeStampTypeRegex.IsMatch(datatype) || RegexUtil.DateTypeRegex.IsMatch(datatype))
			{
				return typeof(DateTime);
			}

			if ("text" == datatype)
			{
				return typeof(string);
			}

			throw new SpiderExceptoin("Unsport datatype: " + datatype);
		}
	}
}

