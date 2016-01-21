using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper.Attributes;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Lib;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public class MongoDbRepository<T> : IDataRepository<T> where T : SpiderEntityUseStringKey
	{
		private readonly IMongoCollection<T> _collection;

		public MongoDbRepository()
		{
			Type type = typeof(T);

			var schemeAttribute = type.GetCustomAttribute<Scheme>();
			if (schemeAttribute == null)
			{
				throw new SpiderExceptoin("MongoDb pipeline need Scheme attribute to the data class.");
			}
			if (string.IsNullOrEmpty(schemeAttribute.Value))
			{
				throw new SpiderExceptoin("Value of Scheme attribute can't be null/empty in MongoDb pipeline.");
			}
			string tmpCollectionName = string.IsNullOrEmpty(schemeAttribute.TableName) ? type.Name : schemeAttribute.TableName;
			string collectionName = $"{tmpCollectionName}{GetSuffix(schemeAttribute.Suffix)}";

			MongoClient client = new MongoClient(ConfigurationManager.AppSettings["mongoconnectstring"]);
			var db = client.GetDatabase(schemeAttribute.Value);
			_collection = db.GetCollection<T>(collectionName);
		}

		private string GetSuffix(SchemeSuffix suffixType)
		{
			switch (suffixType)
			{
				case SchemeSuffix.Empty:
					return "";
				case SchemeSuffix.FirstDayOfMonth:
					return "_" + DateTimeUtil.FirstDayofThisMonth.ToString("yyyy_MM_dd");
				case SchemeSuffix.ThisMonday:
					return "_" + DateTimeUtil.FirstDayofThisWeek.ToString("yyyy_MM_dd");
				case SchemeSuffix.Today:
					return "_" + DateTime.Now.ToString("yyyy_MM_dd");
			}

			return "";
		}

		public void CreateSheme()
		{
		}

		public void CreateTable()
		{
		}

		public void Insert(T instance)
		{
			_collection.InsertOne(instance);
		}

		public void Update(T instance)
		{
			BsonDocument bd = instance.ToBsonDocument();
			_collection.FindOneAndUpdate(x => x.Id == instance.Id, new BsonDocumentUpdateDefinition<T>(bd));
		}

		public void Insert(List<T> instances)
		{
			_collection.InsertMany(instances);
		}

		public void Update(List<T> instances)
		{
			foreach (var instance in instances)
			{
				Update(instance);
			}
		}

		public int Execute(string commandOrSql)
		{
			return -1;
		}
	}
}
