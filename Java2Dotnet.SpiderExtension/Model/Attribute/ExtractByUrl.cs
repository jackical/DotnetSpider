using System;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ExtractByUrl : System.Attribute
	{
		public ExtractByUrl(string value = ".", bool notNull = false)
		{
			Expession = value;
			NotNull = notNull;
		}

		/// <summary>
		/// Extractor expression, only regex can be used
		/// </summary>
		public string Expession { get; set; }

		/// <summary>
		/// Define whether the field can be null.
		/// If set to 'true' and the extractor get no result, the entire class will be discarded.
		/// </summary>
		public bool NotNull { get; set; }
	}
}
