using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;

namespace Bfw.Common
{
    /// <summary>
    /// Static helper methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Helper method to count the amount of words in a source string.
        /// </summary>
        /// <param name="strInput">The input.</param>
        /// <returns>The number of words separated by whitespace.</returns>
        public static int WordCount(this String strInput)
        {
            MatchCollection collection = Regex.Matches(strInput, @"[\S]+");
            return collection.Count;
        }

        /// <summary>
        /// Helper method to truncate a string and append an ellipsis if maxlength reached.
        /// </summary>
        /// <param name="sourceText">The source text.</param>
        /// <param name="ellipsis">The ellipsis text.</param>
        /// <param name="min">The min length.</param>
        /// <param name="max">The max length.</param>
        /// <returns></returns>
        public static string Truncate(this String sourceText, string ellipsis, int min, int max)
        {
            if (String.IsNullOrEmpty(sourceText))
            {
                return String.Empty;
            }

            // If text is shorter than preview length.
            if (sourceText.Length <= max)
            {
                return sourceText; // @RETURN break out early if too short.
            }

            // Grab the char at the last position allowed.
            char cutOffChar = sourceText[max];
            int lastPosition = max;

            // While the last char isn't a space, cut back until we hit a space or minimum.
            while (cutOffChar != ' ' && lastPosition > min)
            {
                lastPosition--;
                cutOffChar = sourceText[lastPosition];
            }

            // Crop text and add some dots.
            string outText = sourceText.Substring(0, lastPosition);
            outText += ellipsis;
            return outText;
        }

        /// <summary>
        /// Remove invalid search terms (whitespace, boolean logic).
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static String ToSearchString(this String input)
        {
            String cleanString = String.Empty;
            String invalidTerms = "and or not";
            String invalidChars = "$ % ^ & * ! + : { } \"";

            if (!String.IsNullOrEmpty(input))
            {
                var invChars = invalidChars.Split(' ').ToList();
                foreach (String iChar in invChars)
                {
                    input = input.Replace(iChar, " ");
                }
            }
            if (!String.IsNullOrEmpty(input))
            {
                var words = input.Split(' ').ToList();
                foreach (String word in words)
                {
                    if (!invalidTerms.Contains(word.ToLower()))
                    {
                        cleanString += word + ' ';
                    }
                }
            }
            return cleanString;
        }

        /// <summary>
        /// Reverses the string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static String ReverseString(this String input)
        {
            char[] arr = input.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// Converts UTF8 string to Unicode.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UTF8ToUnicode(this string input)
        {

            return Encoding.Unicode.GetString(

                                Encoding.Convert(Encoding.UTF8,

                                                    Encoding.Unicode,

                                                    Encoding.UTF8.GetBytes(

                                                                input)));

        }
    }
}
