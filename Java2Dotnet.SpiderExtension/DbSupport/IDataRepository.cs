using System.Collections.Generic;
using Java2Dotnet.Spider.Extension.Model;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public interface IDataRepository<T> where T : ISpiderEntity
	{
		void CreateSheme();

		void CreateTable();

		void Insert(T instance);

		void Update(T instance);

		void Insert(List<T> instances);

		void Update(List<T> instances);

		int Execute(string commandOrSql);
	}
}
