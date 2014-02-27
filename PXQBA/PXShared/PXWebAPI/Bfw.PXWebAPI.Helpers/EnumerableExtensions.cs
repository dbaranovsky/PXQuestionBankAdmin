using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Bfw.PXWebAPI.Helpers
{
	/// <summary>
	/// EnumerableExtensions
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// MatchesWildcard
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="expression"></param>
		/// <param name="pattern"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static IEnumerable<T> MatchesWildcard<T>(this IEnumerable<T> sequence, Func<T, string> expression, string pattern)
		{
			var regEx = WildcardToRegex(pattern);

			return sequence.Where(item => Regex.IsMatch(expression(item), regEx));
		}

		/// <summary>
		/// WildcardToRegex
		/// </summary>
		/// <param name="wildcard"></param>
		/// <returns></returns>
		public static string WildcardToRegex(string wildcard)
		{
			var sb = new StringBuilder(wildcard.Length + 8);

			sb.Append("^");

			for (int i = 0; i < wildcard.Length; i++)
			{
				char c = wildcard[i];
				switch (c)
				{
					case '*':
						sb.Append(".*");
						break;
					case '?':
						sb.Append(".");
						break;
					case '\\':
						if (i < wildcard.Length - 1)
							sb.Append(Regex.Escape(wildcard[++i].ToString()));
						break;
					default:
						sb.Append(Regex.Escape(wildcard[i].ToString()));
						break;
				}
			}
			sb.Append("$");
			return sb.ToString();
		}
	}

}
