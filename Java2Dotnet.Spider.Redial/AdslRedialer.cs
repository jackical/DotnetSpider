using System;

namespace Java2Dotnet.Spider.Redial
{
	public class AdslRedialer : BaseAdslRedialer
	{
		public AdslRedialer()
		{
		}

		public AdslRedialer(string interface1, string user, string password) : base(interface1, user, password)
		{
		}

		public override void Redial()
		{
			Console.WriteLine($"Try to redial: {Interface} {User} {Password}");
			AdslUtil.Connect(Interface, User, Password);
		}
	}
}
