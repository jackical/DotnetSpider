using System.Reflection;
using Java2Dotnet.Spider.Extension.Model;
using Java2Dotnet.Spider.Extension.Model.Attribute;
using Java2Dotnet.Spider.Extension.ORM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Extension.Test
{
	[TestClass]
	public class SpiderEntityTest
	{
		public class Entity1 : BaseEntity
		{
		}

		[Indexes(Index = new[] { "name" }, Primary = "name")]
		public class Entity2 : BaseEntity
		{
			[StoredAs("name", StoredAs.ValueType.String)]
			public string Name { get; set; }
		}

		[TestMethod]
		public void Test1()
		{
			var indexes = typeof(Entity1).GetCustomAttribute<Indexes>();
			Assert.AreEqual(indexes.AutoIncrement, "id");
			Assert.AreEqual(indexes.Primary, "id");

			var indexes1 = typeof(Entity2).GetCustomAttribute<Indexes>(true);

			Assert.AreEqual(indexes1.AutoIncrement, null);
			Assert.AreEqual(indexes1.Primary, "name");
		}
	}
}
