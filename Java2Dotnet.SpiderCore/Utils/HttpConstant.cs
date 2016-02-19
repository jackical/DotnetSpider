namespace Java2Dotnet.Spider.Core.Utils
{
	/// <summary>
	/// Some constants of Http protocal.
	/// </summary>
	public static class HttpConstant
	{
		public static class Method
		{
			public const  string Get = "GET";

			public const string Head = "HEAD";

			public const string Post = "POST";

			public const string Put = "PUT";

			public const string Delete = "DELETE";

			public const string Trace = "TRACE";

			public const string Connect = "CONNECT";
		}

		public static class Header
		{
			public const string Referer = "Referer";
			public const string UserAgent = "User-Agent";
		}
	}
}
