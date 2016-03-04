using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommandLine;
using Java2Dotnet.Spider.Extension.Configuration;
using Newtonsoft.Json;

namespace Java2Dotnet.Spider.Scripts
{
	public class ScriptConsole
	{
		public static void Main(string[] args)
		{
			Core.Spider.PrintInfo();

			//Options param = ParseCommand(args);
			//if (param != null)
			//{
			//	StartSpider(param);
			//}
			string json = File.ReadAllText("mysqlsample.json");
			json = Macros.Replace(json);
			JsonSpider jsonSpider = JsonConvert.DeserializeObject<JsonSpider>(json);
			List<string> errorMessages;
			if (JsonSpiderValidation.Validate(jsonSpider, out errorMessages))
			{
				ScriptSpider spider = new ScriptSpider(jsonSpider);
				spider.Run(args);
			}
			else
			{
				foreach (var errorMessage in errorMessages)
				{
					Console.WriteLine(errorMessage);
				}
			}
			Console.Read();
		}

		private static void StartSpider(Options param)
		{
			ScriptProcessor pageProcessor = ScriptProcessorBuilder.Custom().Language(param.Lang).ScriptFromFile(param.File).Thread(param.Thread).Build();
			pageProcessor.Site.SleepTime = param.Sleep;
			pageProcessor.Site.RetryTimes = 3;
			pageProcessor.Site.AcceptStatCode = new HashSet<int> { 200, 404, 403, 500, 502 };
			Core.Spider spider = Core.Spider.Create(pageProcessor).SetThreadNum(param.Thread);
			spider.ClearPipeline();

			StringBuilder builder = new StringBuilder();
			using (StreamReader sr = new StreamReader(typeof(ScriptConsole).Assembly.GetManifestResourceStream("Java2Dotnet.Spider.Scripts.Resource.js.define.js")))
			{
				string line;

				while ((line = sr.ReadLine()) != null)
				{
					builder.AppendLine(line);
				}
			}

			string script = builder + Environment.NewLine + File.ReadAllText(param.File);

			Jurassic.ScriptEngine engine = new Jurassic.ScriptEngine { EnableExposedClrTypes = true };
			//engine.SetGlobalValue("page", new Page());
			//engine.SetGlobalValue("config", new Site());

			engine.Evaluate(script);

			foreach (string url in param.Urls)
			{
				spider.AddStartUrl(url);
			}
			spider.Run();
		}

		internal class Options
		{
			[Option('l', "language", Required = false, HelpText = "language")]
			public Language Lang { get; set; }

			[Option('t', "thread", Required = false, HelpText = "thread")]
			public int Thread { get; set; }

			[Option('f', "file", Required = true, HelpText = "script file")]
			public string File { get; set; }

			[Option('i', "input", Required = false, HelpText = "input file")]
			public string Input { get; set; }
			[Option('s', "sleep", Required = false, HelpText = "sleep time")]
			public int Sleep { get; set; }

			[Option('u', "url", Required = false, HelpText = "start urls")]
			public IEnumerable<string> Urls { get; set; }

			public Options()
			{
				Lang = Language.Javascript;
				Thread = 1;
				Sleep = 1000;
			}
		}

		private static Options ParseCommand(string[] args)
		{
			try
			{
				var result = Parser.Default.ParseArguments<Options>(new List<string>(args));
				var par = result
					.MapResult(options => options,
					errors =>
					{
						foreach (var error in errors)
						{
							NamedError named = error as NamedError;
							Console.WriteLine(named + ":" + error.ToString());
						}
						return null;
					});
				return par;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}