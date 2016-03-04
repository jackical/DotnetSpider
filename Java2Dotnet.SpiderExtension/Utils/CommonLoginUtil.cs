using System;
using System.Threading;
using Java2Dotnet.Spider.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Java2Dotnet.Spider.Extension.Utils
{
	public class LoginArguments
	{
		public string Url { get; set; }

		public Selector User { get; set; }

		public Selector Pass { get; set; }

		public Selector Submit { get; set; }

		public class Selector
		{
			public string FindBy { get; set; }
			public string Expression { get; set; }
			public string Value { get; set; }
		}
	}

	public class CommonLoginUtil
	{
		private readonly LoginArguments _loginArguments;

		public CommonLoginUtil(LoginArguments loginArguments)
		{
			_loginArguments = loginArguments;
		}


		public bool Login(RemoteWebDriver webDriver)
		{
			try
			{
				webDriver.Navigate().GoToUrl(_loginArguments.Url);
				var user = FindElement(webDriver, _loginArguments.User);

				user.Clear();
				user.SendKeys(_loginArguments.User.Value);
				Thread.Sleep(1500);
				var pass = FindElement(webDriver, _loginArguments.Pass);
				pass.SendKeys(_loginArguments.Pass.Value);
				Thread.Sleep(1500);
				var submit = FindElement(webDriver, _loginArguments.Submit);
				submit.Click();
				Thread.Sleep(5000);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private IWebElement FindElement(RemoteWebDriver webDriver, LoginArguments.Selector element)
		{
			switch (element.FindBy)
			{
				case "id":
					{
						return webDriver.FindElementById(element.Expression);
					}
				case "xpath":
					{
						return webDriver.FindElementByXPath(element.Expression);
					}
				case "css":
					{
						return webDriver.FindElementByCssSelector(element.Expression);
					}
			}
			throw new SpiderExceptoin("Unsport findy: " + element.FindBy);
		}
	}
}
