using System.Configuration;
using Renci.SshNet;

namespace Java2Dotnet.Spider.Redial
{
	public class SshAdslRedialer : BaseAdslRedialer
	{
		private readonly string _sshhost;
		private readonly string _sshuser;
		private readonly string _sshpass;
		private readonly int _sshport;

		public SshAdslRedialer()
		{
			_sshhost = ConfigurationManager.AppSettings["sshHost"];
			_sshuser = ConfigurationManager.AppSettings["sshUser"];
			_sshpass = ConfigurationManager.AppSettings["sshPassword"];
			var port = ConfigurationManager.AppSettings["sshPort"];
			_sshport = port == null ? 23 : int.Parse(port);
		}

		public SshAdslRedialer(string sshhost, string sshuser, string sshpass, string interface1, string user, string password) : base(interface1, user, password)
		{
			_sshhost = sshhost;
			_sshuser = sshuser;
			_sshpass = sshpass;
		}

		public override void Redial()
		{
			using (var client = new SshClient(_sshhost, _sshport, _sshuser, _sshpass))
			{
				client.RunCommand("");
			}
		}
	}
}
