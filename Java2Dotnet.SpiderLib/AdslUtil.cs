using System.Diagnostics;

namespace Java2Dotnet.Spider.Lib
{
	public class AdslUtil
	{
		public void Connect(string connectionName, string user, string pass)
		{
			Disconnect(connectionName);
			string arg = $"rasdial \"{connectionName}\" {user} {pass}";
			InvokeCmd(arg);
		}

		public void Disconnect(string connectionName)
		{
			string arg = $"rasdial \"{connectionName}\" /disconnect";
			InvokeCmd(arg);
		}

		public static string InvokeCmd(string cmdArgs)
		{
			Process p = new Process
			{
				StartInfo =
				{
					FileName = "cmd.exe",
					UseShellExecute = false,
					RedirectStandardInput = true,
					RedirectStandardOutput = true,
					RedirectStandardError = true,
					CreateNoWindow = true
				}
			};
			p.Start();
			p.StandardInput.WriteLine(cmdArgs);
			p.StandardInput.WriteLine("exit");
			return p.StandardOutput.ReadToEnd();
		}
	}
}
