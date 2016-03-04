using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Java2Dotnet.Spider.Core;
using Newtonsoft.Json.Linq;
using Dapper;
using PropertyAttributes = System.Reflection.PropertyAttributes;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public abstract class EntityGeneralPipeline : IEntityPipeline
	{
		public class GeneralPipelineArgument
		{
			public string ConnectString { get; set; }
		}
		protected readonly GeneralPipelineArgument Argument;
		protected readonly List<Column> Columns;

		protected abstract IDbConnection CreateConnection();
		protected abstract string GetInsertSql();
		protected abstract string GetCreateTableSql();
		protected abstract string GetCreateSchemaSql();
		protected readonly DbSchema Schema;

		protected readonly Type Type;

		protected static Regex StringTypeRegex = new Regex(@"string\(\d+\)");
		protected static Regex IntTypeRegex = new Regex(@"[int\(\d+\)|int]");
		protected static Regex BigIntTypeRegex = new Regex(@"[bigint\(\d+\)|bigint]");
		protected static Regex FloatTypeRegex = new Regex(@"[float\(\d+\)|float]");
		protected static Regex DoubleTypeRegex = new Regex(@"[double\(\d+\)|double]");
		protected static Regex NumRegex = new Regex(@"\d+");

		protected List<List<string>> Indexs { get; set; } = new List<List<string>>();
		protected List<List<string>> Uniques { get; set; } = new List<List<string>>();

		protected abstract string ConvertToDbType(string datatype);

		protected EntityGeneralPipeline(DbSchema schema, JObject entityDefine, JObject argument)
		{
			Schema = schema;
			Columns = entityDefine.SelectTokens("$.fields[*]").Select(j => j.ToObject<Column>()).ToList();

			foreach (var index in entityDefine.SelectTokens("$.indexs[*]"))
			{
				Indexs.Add(index.ToObject<List<string>>());
			}

			foreach (var index in entityDefine.SelectTokens("$.uniques[*]"))
			{
				Uniques.Add(index.ToObject<List<string>>());
			}

			Argument = argument.ToObject<GeneralPipelineArgument>();
			Type = GenerateType(schema, Columns);
		}

		public void Initialize()
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

		private Type GenerateType(DbSchema schema, List<Column> columns)
		{
			AppDomain currentAppDomain = AppDomain.CurrentDomain;
			AssemblyName assyName = new AssemblyName("DotnetSpiderAss_" + schema.TableName);

			AssemblyBuilder assyBuilder = currentAppDomain.DefineDynamicAssembly(assyName, AssemblyBuilderAccess.Run);

			ModuleBuilder modBuilder = assyBuilder.DefineDynamicModule("DotnetSpiderMod_" + schema.TableName);

			TypeBuilder typeBuilder = modBuilder.DefineType("type_" + schema.TableName, TypeAttributes.Class | TypeAttributes.Public);

			foreach (var column in columns)
			{
				AddProperty(typeBuilder, column.Name, Convert(column.DataType));
			}

			AddProperty(typeBuilder, "id", typeof(long));

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
			if (StringTypeRegex.IsMatch(datatype))
			{
				return typeof(string);
			}

			if (IntTypeRegex.IsMatch(datatype))
			{
				return typeof(int);
			}

			if (BigIntTypeRegex.IsMatch(datatype))
			{
				return typeof(long);
			}

			if (FloatTypeRegex.IsMatch(datatype))
			{
				return typeof(float);
			}

			if (DoubleTypeRegex.IsMatch(datatype))
			{
				return typeof(double);
			}

			if ("text" == datatype)
			{
				return typeof(string);
			}

			throw new SpiderExceptoin("Unsport datatype: " + datatype);
		}
	}
}

