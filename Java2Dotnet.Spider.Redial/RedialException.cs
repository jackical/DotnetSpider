using System;
using System.Runtime.Serialization;

namespace Java2Dotnet.Spider.Redial
{
	[Serializable]
	internal class RedialException : Exception
	{
		public RedialException()
		{
		}

		public RedialException(string message) : base(message)
		{
		}

		public RedialException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected RedialException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}