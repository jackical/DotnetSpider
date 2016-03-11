using System;
using Java2Dotnet.Spider.Core;
using Java2Dotnet.Spider.Extension.Utils;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public class Stopping : System.Attribute
	{
		public string PropertyName { get; set; }

		public Operate Operate { get; set; }

		public string CompareValue { get; set; }

		internal string DataType { get; set; }

		public bool NeedStop(string value)
		{
			if (RegexUtil.StringTypeRegex.IsMatch(DataType))
			{
				if (Operate == Operate.Equal)
				{
					return value == CompareValue;
				}
				else
				{
					return String.CompareOrdinal(value, CompareValue) > 0;
				}
			}

			if (RegexUtil.IntTypeRegex.IsMatch(DataType))
			{
				switch (Operate)
				{
					case Operate.Equal:
						{
							return int.Parse(value) == int.Parse(CompareValue);
						}
					case Operate.Large:
						{
							return int.Parse(value) > int.Parse(CompareValue);
						}
					case Operate.Less:
						{
							return int.Parse(value) < int.Parse(CompareValue);
						}
				}
			}

			if (RegexUtil.BigIntTypeRegex.IsMatch(DataType))
			{
				switch (Operate)
				{
					case Operate.Equal:
						{
							return long.Parse(value) == long.Parse(CompareValue);
						}
					case Operate.Large:
						{
							return long.Parse(value) > long.Parse(CompareValue);
						}
					case Operate.Less:
						{
							return long.Parse(value) < long.Parse(CompareValue);
						}
				}
			}

			if (RegexUtil.FloatTypeRegex.IsMatch(DataType))
			{
				switch (Operate)
				{
					case Operate.Equal:
						{
							return Equals(float.Parse(value), float.Parse(CompareValue));
						}
					case Operate.Large:
						{
							return float.Parse(value) > float.Parse(CompareValue);
						}
					case Operate.Less:
						{
							return float.Parse(value) < float.Parse(CompareValue);
						}
				}
			}

			if (RegexUtil.DoubleTypeRegex.IsMatch(DataType))
			{
				switch (Operate)
				{
					case Operate.Equal:
						{
							return Equals(double.Parse(value), double.Parse(CompareValue));
						}
					case Operate.Large:
						{
							return double.Parse(value) > double.Parse(CompareValue);
						}
					case Operate.Less:
						{
							return double.Parse(value) < double.Parse(CompareValue);
						}
				}
			}
			if (RegexUtil.DateTypeRegex.IsMatch(DataType) || RegexUtil.TimeStampTypeRegex.IsMatch(DataType))
			{
				switch (Operate)
				{
					case Operate.Equal:
						{
							return DateTime.Parse(value) == DateTime.Parse(CompareValue);
						}
					case Operate.Large:
						{
							return DateTime.Parse(value) > DateTime.Parse(CompareValue);
						}
					case Operate.Less:
						{
							return DateTime.Parse(value) < DateTime.Parse(CompareValue);
						}
				}
			}

			if (RegexUtil.TimeStampTypeRegex.IsMatch(DataType) || RegexUtil.DateTypeRegex.IsMatch(DataType))
			{
				switch (Operate)
				{
					case Operate.Equal:
						{
							return DateTime.Parse(value) == DateTime.Parse(CompareValue);
						}
					case Operate.Large:
						{
							return DateTime.Parse(value) > DateTime.Parse(CompareValue);
						}
					case Operate.Less:
						{
							return DateTime.Parse(value) < DateTime.Parse(CompareValue);
						}
				}
			}

			if ("text" == DataType)
			{
				if (Operate == Operate.Equal)
				{
					return value == CompareValue;
				}
				else
				{
					return String.CompareOrdinal(value, CompareValue) > 0;
				}
			}

			throw new SpiderExceptoin("Unsport DataType: " + DataType);
		}
	}

	public enum Operate
	{
		Equal,
		Large,
		Less
	}
}
