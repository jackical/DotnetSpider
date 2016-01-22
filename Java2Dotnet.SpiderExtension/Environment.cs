using System;
using System.IO;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension
{
	public static class SpiderEnvironment
	{
		public static string GetDataFilePath(ISpider spider, string name)
		{
			string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", spider.Identify);

			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}

			return Path.Combine(folderPath, name + ".sql");
		}
	}
}
