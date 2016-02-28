using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector.Html
{
	/// <summary>
	/// Selectable text.
	/// </summary>
	public interface ISelectable : IBaseSelectable
	{
		/// <summary>
		/// Select list with xpath
		/// </summary>
		/// <param name="xpath"></param>
		/// <returns></returns>
		ISelectable XPath(string xpath);

		/// <summary>
		/// Select list with css selector
		/// </summary>
		/// <param name="selector"></param>
		/// <returns></returns>
		ISelectable Css(string selector);

		/// <summary>
		/// Select list with css selector
		/// </summary>
		/// <param name="selector"></param>
		/// <param name="attrName"></param>
		/// <returns></returns>
		ISelectable Css(string selector, string attrName);

		/// <summary>
		/// Select smart content with ReadAbility algorithm
		/// </summary>
		/// <returns></returns>
		ISelectable SmartContent();

		/// <summary>
		/// Select all links
		/// </summary>
		/// <returns></returns>
		ISelectable Links();

		/// <summary>
		/// Get all nodes
		/// </summary>
		/// <returns></returns>
		IList<ISelectable> Nodes();
	}
}
