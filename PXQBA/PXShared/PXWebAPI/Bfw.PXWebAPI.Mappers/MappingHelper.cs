using System;
using System.Linq;

namespace Bfw.PXWebAPI.Mappers
{
	static class MappingHelper
	{

		/// <summary>
		/// Giving the first N words of a statement.
		/// </summary>
		/// <param name="givenString">The given string.</param>
		/// <param name="totalWords">The total words.</param>
		/// <returns></returns>
		public static string FirstNWords(this string givenString, int totalWords)
		{
			if (!string.IsNullOrEmpty(givenString))
			{
				var words = givenString.Split(' ');
				var newWord = string.Empty;

				for (var i = 0; ( i < totalWords && words.Length > i ); i++)
				{
					newWord += words[i] + " ";
				}
				return newWord;
			}
			return string.Empty;
		}


		/// <summary>
		/// This function is to replace no of specified characters with the replacement
		/// Source String.
		/// </summary>
		/// <param name="sourceString">The source string.</param>
		/// <param name="charList">The char list.</param>
		/// <param name="replacement">The replacement.</param>
		/// <returns></returns>
		public static string Translate(string sourceString, char[] charList, string replacement)
		{
			if (!String.IsNullOrEmpty(sourceString))
			{
				sourceString = charList.Aggregate(sourceString, (current, c) => current.Replace(c.ToString(), replacement));
			}
			return sourceString;
		}
	}
}
