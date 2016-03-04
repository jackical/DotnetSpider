using System.Collections.Generic;
using Java2Dotnet.Spider.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Extension.Pipeline
{
	public class EntityMongoDbPipeline : IEntityPipeline
	{
		public class MongoDbPipelineArgument
		{
			public string Host { get; set; }
			public string Port { get; set; }
			public string Password { get; set; }
		}

		private readonly IMongoCollection<BsonDocument> _collection;

		public EntityMongoDbPipeline(DbSchema schema, JObject argument)
		{
			var argument1 = argument.ToObject<MongoDbPipelineArgument>();

			MongoClient client = new MongoClient(argument1.Host);
			var db = client.GetDatabase(schema.DatabaseName);

			_collection = db.GetCollection<BsonDocument>(schema.TableName);
		}

		public void Process(List<JObject> datas, ISpider spider)
		{
			List<BsonDocument> reslut = new List<BsonDocument>();
			foreach (var data in datas)
			{
				BsonDocument item = BsonDocument.Parse(data.ToString());

				reslut.Add(item);
			}
			_collection.InsertMany(reslut);
		}

		public class Person
		{
			public string Name { get; set; }
		}

		public void Dispose()
		{
		}
	}
}
