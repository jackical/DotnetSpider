using System;

namespace Java2Dotnet.Spider.Samples
{
	class Program
	{
		static void Main(string[] args)
		{
			TmallGmvSpider spider = new TmallGmvSpider();
			spider.Run();

			Console.Read();
		}
	}
}
