using System;

namespace Java2Dotnet.Spider.Lib
{
	public static class StringExtension
	{
		public static string GetRandomString(this string value, int count)
		{
			int number;
			string checkCode = String.Empty; //存放随机码的字符串   

			Random random = new Random();

			for (int i = 0; i < count; i++) //产生4位校验码   
			{
				number = random.Next();
				number = number%36;
				if (number < 10)
				{
					number += 48; //数字0-9编码在48-57   
				}
				else
				{
					number += 55; //字母A-Z编码在65-90   
				}

				checkCode += ((char) number).ToString();
			}
			return checkCode;
		}
	}
}
