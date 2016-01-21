using System;

namespace Java2Dotnet.Spider.Extension.Model
{
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class Stopper : System.Attribute
	{
		public abstract bool NeedStop(dynamic value);
	}
}
