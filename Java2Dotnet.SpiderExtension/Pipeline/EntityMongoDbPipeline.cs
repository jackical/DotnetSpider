using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Java2Dotnet.Spider.Core;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
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

		private readonly MongoDbPipelineArgument _argument;
		private readonly DbScheme _scheme;

		public EntityMongoDbPipeline(DbScheme scheme, JObject argument)
		{
			_scheme = scheme;
			_argument = argument.ToObject<MongoDbPipelineArgument>();
		}

		public void Process(List<dynamic> datas, ISpider spider)
		{
			MongoClient client = new MongoClient(_argument.Host);
			var db = client.GetDatabase(_scheme.DatabaseName);

			var collection = db.GetCollection<BsonDocument>(_scheme.TableName);


			List<BsonDocument> reslut = new List<BsonDocument>();
			foreach (var data in datas)
			{
				BsonDocument item = BsonDocument.Parse(data.ToString());

				reslut.Add(item);
			}
			collection.InsertMany(reslut);
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
