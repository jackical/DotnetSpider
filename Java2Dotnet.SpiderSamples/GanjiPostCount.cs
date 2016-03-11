//using System;
//using System.Text;
//using Java2Dotnet.Spider.Core;
//using Java2Dotnet.Spider.Extension.Model;

//namespace Java2Dotnet.Spider.Samples
//{
//	class GanjiPostCount
//	{
//		public static void RunTask()
//		{
//			ModelMysqlFileSpider<Ganji> ooSpider = new ModelMysqlFileSpider<Ganji>("ganji" + DateTime.Now.ToLocalTime(), new Site { SleepTime = 1000, Encoding = Encoding.UTF8 });
//			ooSpider.SetThreadNum(1);
//			Request request = new Request("http://mobds.ganji.com/datashare/", 1, null);
//			request.Method = "POST";
//			ooSpider.AddRequest(request);
//			ooSpider.Run();
//		}
//	}
//}
