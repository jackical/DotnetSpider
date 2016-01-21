using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core;

namespace Java2Dotnet.Spider.Extension.Monitor
{
	public interface ISpiderStatus
	{
		string Name { get; }

		string Status { get; }

		int AliveThreadCount { get; }

		int ThreadCount { get; }

		long TotalPageCount { get; }

		long LeftPageCount { get; }

		long SuccessPageCount { get; }

		long ErrorPageCount { get; }

		List<string> ErrorPages { get; }

		void Start();

		void Stop();

		DateTime StartTime { get; }

		DateTime EndTime { get; }

		double PagePerSecond { get; }

		 Core.Spider Spider { get; }
	}
}
