using System.Collections.Generic;

namespace Java2Dotnet.Spider.Core.Selector.Json
{
	/// <summary>
	/// Selectable text.
	/// </summary>
	public interface ISelectable : IBaseSelectable
	{
		ISelectable JsonPath(string path);
	}
}
