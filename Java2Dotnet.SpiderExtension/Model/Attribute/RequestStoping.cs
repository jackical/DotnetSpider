using System;

namespace Java2Dotnet.Spider.Extension.Model.Attribute
{
	[AttributeUsage(AttributeTargets.Class)]
	public abstract class RequestStoping : System.Attribute
	{
		public abstract bool NeedStop(dynamic value);
	}
}
