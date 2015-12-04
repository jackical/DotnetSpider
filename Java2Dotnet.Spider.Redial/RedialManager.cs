using Java2Dotnet.Spider.Lib.Redial;

namespace Java2Dotnet.Spider.Redial
{
	public static class RedialManager
	{
		public static IRedialManager Default = FileLockerRedialManager.Default;
	}
}
