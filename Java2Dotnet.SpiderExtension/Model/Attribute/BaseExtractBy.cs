namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	public class BaseExtractBy : System.Attribute
	{
		private ExtractType _type = ExtractType.XPath;

		/// <summary>
		/// Extractor expression, support XPath, CSS Selector and regex.
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// Extractor type, support XPath, CSS Selector and regex.
		/// </summary>
		public ExtractType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
				//if (_type != value)
				//{
				//	_type = value;
				//	if (value == ExtractType.Enviroment)
				//	{
				//		Source = ExtractSource.Enviroment;
				//	}
				//	if (value == ExtractType.JsonPath)
				//	{
				//		Source = ExtractSource.Json;
				//	}
				//}
			}
		}

		public long Count { get; set; } = long.MaxValue;
	}
}
