using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Java2Dotnet.Spider.Extension.Model
{
	public interface ISpiderEntity
	{
	}

	public static class SpiderEntityExtensions
	{
		public static Dictionary<string, object> ToDictionary(this ISpiderEntity entity)
		{
			var properties = entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
			return properties.ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(entity));
		}
	}

	public abstract class SpiderEntity : ICustomize, ISpiderEntity
	{
		public abstract long Id { get; set; }

		public virtual void Customize()
		{
		}
	}

	public abstract class SpiderEntityUseStringKey : ICustomize, ISpiderEntity
	{
		public string Id { get; set; }

		public virtual void Customize()
		{
		}
	}
}
