using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper;
using Java2Dotnet.Spider.Extension.Model;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	public sealed class DataRepository<T> : IDataRepository<T> where T : ISpiderEntity
	{
		private ISqlGenerator SqlGenerator { get; }

		public DataRepository()
		{
			if (DbProviderUtil.Provider == null)
			{
				DbProviderUtil.Provider = new DataProviderManager().LoadDataProvider();
			}

			if (DbProviderUtil.Provider.Name == "MySql")
			{
				SqlGenerator = new MySqlGenerator(typeof(T));
			}

			if (SqlGenerator == null)
			{
				throw new SpiderExceptoin("SqlGenerator is null.");
			}
		}

		public void Insert(T instance)
		{
			if (instance == null)
			{
				return;
			}

			var sql = SqlGenerator.GetInsert(false);

			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				conn.Execute(sql, instance, null, 99999, CommandType.Text);
			}
		}

		public void Insert(List<T> instances)
		{
			if (instances == null)
			{
				return;
			}

			var sql = SqlGenerator.GetInsert(false);

			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				conn.Execute(sql, instances, null, 99999, CommandType.Text);
			}
		}

		public void Update(List<T> instances)
		{
			if (instances == null || instances.Count == 0)
			{
				return;
			}
			var sql = SqlGenerator.GetUpdate();

			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				conn.Execute(sql, instances, null, 99999, CommandType.Text);
			}
		}

		public void Update(T instance)
		{
			if (instance == null)
			{
				return;
			}
			var sql = SqlGenerator.GetUpdate();

			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				conn.Execute(sql, instance, null, 99999, CommandType.Text);
			}
		}

		public List<T> GetWhere(object filters)
		{
			if (filters == null)
			{
				return null;
			}

			var sql = SqlGenerator.GetSelect(filters);

			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				return conn.Query<T>(sql, filters, null, false, 99999, CommandType.Text).ToList();
			}
		}

		public int Execute(string sql)
		{
			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				return conn.Execute(sql, null, null, 99999, CommandType.Text);
			}
		}

		public List<T> Query(string sql)
		{
			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				return conn.Query<T>(sql, null, null, false, 99999, CommandType.Text).ToList();
			}
		}

		public void CreateTable()
		{
			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				conn.Execute(SqlGenerator.GetCreateTable(), null, null, 99999, CommandType.Text);
			}
		}

		public void CreateSheme()
		{
			using (IDbConnection conn = DbProviderUtil.Provider.CreateConnection())
			{
				conn.Execute(SqlGenerator.GetCreateSheme(), null, null, 99999, CommandType.Text);
			}
		}
	}
}
