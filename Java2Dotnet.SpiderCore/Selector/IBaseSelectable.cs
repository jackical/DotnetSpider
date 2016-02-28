using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Java2Dotnet.Spider.Core.Selector
{
	public interface IBaseSelectable
	{
		/// <summary>
		/// Select list with regex, default group is group 1
		/// </summary>
		/// <param name="regex"></param>
		/// <returns></returns>
		IBaseSelectable Regex(string regex);

		/// <summary>
		/// Select list with regex
		/// </summary>
		/// <param name="regex"></param>
		/// <param name="group"></param>
		/// <returns></returns>
		IBaseSelectable Regex(string regex, int group);

		///// <summary>
		///// Replace with regex
		///// </summary>
		///// <param name="regex"></param>
		///// <param name="replacement"></param>
		///// <returns></returns>
		//ISelectable Replace(string regex, string replacement);

		/// <summary>
		/// Single string result
		/// </summary>
		dynamic Value { get; }

		///// <summary>
		///// If result exist for select
		///// </summary>
		///// <returns></returns>
		////bool Exist();

		/// <summary>
		/// Extract by custom selector
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		IBaseSelectable Select(ISelector selector);

		/// <summary>
		/// Extract by custom selector
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		IBaseSelectable SelectList(ISelector selector);
	}
}
