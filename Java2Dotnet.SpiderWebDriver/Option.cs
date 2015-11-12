namespace Java2Dotnet.Spider.WebDriver
{
	public class Option
	{
		public static Option Default = new Option();

		public bool LoadImage { get; set; } = true;

		public bool AlwaysLoadNoFocusLibrary { get; set; } = true;

		public bool LoadFlashPlayer { get; set; } = true;

		public string Proxy { get; set; }

		public string ProxyAuthentication { get; set; }
	}
}
