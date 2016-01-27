using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Java2Dotnet.Spider.Core.Utils;

namespace Java2Dotnet.Spider.Core.Selector
{
	public class RegexSelector : ISelector
	{
		private readonly string _regexStr;
		private readonly Regex _regex;
		private readonly int _group;

		public RegexSelector(string regexStr, int group)
		{
			if (string.IsNullOrEmpty(regexStr))
			{
				throw new ArgumentException("regex must not be empty");
			}
			// Check bracket for regex group. Add default group 1 if there is no group.
			// Only check if there exists the valid left parenthesis, leave regexp validation for Pattern.
			if (StringUtils.CountMatches(regexStr, "(") - StringUtils.CountMatches(regexStr, "\\(") ==
					StringUtils.CountMatches(regexStr, "(?:") - StringUtils.CountMatches(regexStr, "\\(?:"))
			{
				regexStr = "(" + regexStr + ")";
			}
			_regexStr = regexStr;
			//check: regex = Pattern.compile(regexStr, Pattern.DOTALL | Pattern.CASE_INSENSITIVE);
			_regex = new Regex(regexStr, RegexOptions.IgnoreCase);
			_group = group;
		}

		public RegexSelector(string regexStr)
			: this(regexStr, 1)
		{
		}

		public SelectedNode Select(SelectedNode text)
		{
			return new SelectedNode { Type = ResultType.String, Result = SelectGroup(text).Get(_group) };
		}

		public SelectedNode Select(string text)
		{
			return new SelectedNode { Type = ResultType.String, Result = SelectGroup(new SelectedNode() { Type = ResultType.String, Result = text }).Get(_group) };
		}

		public List<SelectedNode> SelectList(SelectedNode text)
		{
			IList<RegexResult> results = SelectGroupList(text.ToString());
			return results.Select(result => new SelectedNode { Type = ResultType.String, Result = result.Get(_group) }).ToList();
		}

		private RegexResult SelectGroup(SelectedNode text)
		{
			var match = _regex.Match(text.ToString());
			return new RegexResult(_regex.ToString(), (from Group g in match.Groups select g.Value).ToList());
		}

		private List<RegexResult> SelectGroupList(string text)
		{
			List<RegexResult> resultList = new List<RegexResult>();

			var matches = _regex.Matches(text);
			if (matches.Count > 0)
			{
				foreach (Match m in matches)
				{
					resultList.Add(new RegexResult(_regex.ToString(), (from Group @group in m.Groups select @group.Value).ToList()));
				}
			}

			return resultList;
		}

		public override string ToString()
		{
			return _regexStr;
		}
	}
}
