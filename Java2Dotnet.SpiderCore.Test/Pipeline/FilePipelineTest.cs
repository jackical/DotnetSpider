﻿using System;
using System.Collections.Generic;
using Java2Dotnet.Spider.Core.Pipeline;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Java2Dotnet.Spider.Core.Test.Pipeline
{
	[TestClass]
	public class FilePipelineTest
	{
		private ResultItems _resultItems;
		private ISpider _spider;

		[TestInitialize]
		public void Before()
		{
			_resultItems = new ResultItems();
			_resultItems.AddOrUpdateResultItem("content", "webmagic 爬虫工具");
			Request request = new Request("http://www.baidu.com", 1, null);
			_resultItems.Request = request;
			_spider = new TestSpider();

		}

		private class TestSpider : ISpider
		{
			public string Identify => Guid.NewGuid().ToString();

			public Site Site => null;
			public void Start()
			{
			}

			public void Run()
			{
			}

			public void Stop()
			{
			}

			public Dictionary<string, dynamic> Settings { get; } = new Dictionary<string, dynamic>();

			public void Dispose()
			{
			}
		};

		[TestMethod]
		public void TestProcess()
		{
			FilePipeline filePipeline = new FilePipeline();
			filePipeline.Process(_resultItems, _spider);
		}
	}
}
