using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class SelectedNode
	{
		/// <summary>
		/// Type of Result denpend by SelectorType.
		/// </summary>
		public dynamic Result { get; set; }
		public ResultType Type { get; set; }

		public override string ToString()
		{
			switch (Type)
			{
				case ResultType.Node:
					return Result?.InnerHtml;
				case ResultType.String:
				case ResultType.Json:
					return Result?.ToString();
			}

			throw new SpiderExceptoin($"Not support selector type: {Type}");
		}
	}

	public static class SelectNodeExtension
	{
		public static List<string> ToStringList(this IEnumerable<SelectedNode> nodes)
		{
			return nodes.Select(i => i.ToString()).ToList();
		}

		public static string CombineToString(this IEnumerable<SelectedNode> nodes)
		{
			StringBuilder builder = new StringBuilder();
			foreach (var selectedNode in nodes)
			{
				builder.Append(selectedNode);
			}
			return builder.ToString();
		}
	}
}
