using System;

namespace Java2Dotnet.Spider.Samples
{
	class Program
	{
		static void Main(string[] args)
		{
			GanjiPostSpider spider = new GanjiPostSpider();
			spider.Run(args);

			Console.Read();
		}
	}
}
