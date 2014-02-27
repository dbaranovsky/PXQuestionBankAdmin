using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class Helper
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
                string[] words = givenString.Split(' ');
                string newWord = string.Empty;

                for (int i = 0; (i < totalWords && words.Length > i); i++)
                {
                    newWord += words[i] + " ";
                }
                return newWord;
            }
            return string.Empty;
        }

        /// <summary>
        /// Dummies the notes.
        /// </summary>
        /// <returns></returns>
        public static List<Note> DummyNotes()
        {
            var notes = new List<Note>();
            string text = "Do mapping post-it notes for logical connections count? IF yes or not?";
            notes.Add(new Note { NoteId = "1", Text = text, ShortText = text.FirstNWords(10), Sequence = "1", EntityId = "1", Title = "ABC" });
            notes.Add(new Note { NoteId = "2", Text = text, ShortText = text.FirstNWords(10), Sequence = "2", EntityId = "2", Title = "DEF" });
            notes.Add(new Note { NoteId = "3", Text = text, ShortText = text.FirstNWords(10), Sequence = "3", EntityId = "3", Title = "GHI" });
            notes.Add(new Note { NoteId = "4", Text = text, ShortText = text.FirstNWords(10), Sequence = "4", EntityId = "4", Title = "JKL" });
            notes.Add(new Note { NoteId = "5", Text = text, ShortText = text.FirstNWords(10), Sequence = "5", EntityId = "5", Title = "MNO" });

            return notes;
        }

        /// <summary>
        /// This function is to replace no of specified charecters with the replacement
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
                foreach (char c in charList)
                {
                    sourceString = sourceString.Replace(c.ToString(), replacement);
                }
            }
            return sourceString;
        }
    }
}
