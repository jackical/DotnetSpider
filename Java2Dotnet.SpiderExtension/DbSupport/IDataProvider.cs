using System.Data;
using Java2Dotnet.Spider.Extension.DbSupport.Dapper;

namespace Java2Dotnet.Spider.Extension.DbSupport
{
	/// <summary>
	/// Data provider interface
	/// </summary>
	public interface IDataProvider
	{
		/// <summary>
		/// Create a connection
		/// </summary>
		/// <returns></returns>
		IDbConnection CreateConnection();

		string Name { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		string GetHost();
	}
}
