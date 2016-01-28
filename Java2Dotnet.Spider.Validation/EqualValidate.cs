using System;
using System.Data;
using System.Web;

namespace Java2Dotnet.Spider.Validation
{
	public class EqualValidate : AbstractValidate
	{
		public EqualValidate(IDbConnection conn, string sql, string arguments, string description, ValidateLevel level = ValidateLevel.Error) : base(conn, sql, arguments, description, level)
		{
		}

		public override ValidateResult Validate()
		{
			try
			{
				string value = GetValue();
				return new ValidateResult
				{
					IsPass = value == Arguments,
					Arguments = Arguments,
					Description = Description,
					Sql = Sql,
					ActualValue = value
				};
			}
			catch (Exception e)
			{
				return new ValidateResult
				{
					Message = HttpUtility.HtmlAttributeEncode(e.Message).Replace('\"', '\0'),
					IsPass = false,
					Arguments = Arguments,
					Description = Description,
					Sql = Sql
				};
			}
		}

		public override string ToString()
		{
			return $"CompareTo: {Arguments}";
		}
	}
}
