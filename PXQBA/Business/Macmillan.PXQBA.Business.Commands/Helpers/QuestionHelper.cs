
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bfw.Common.Collections;
using Macmillan.PXQBA.Business.Models;
using Question = Bfw.Agilix.DataContracts.Question;

namespace Macmillan.PXQBA.Business.Commands.Helpers
{
    /// <summary>
    /// Helper that handles non-trivial question properties that should be somehow calculated
    /// </summary>
    public static class QuestionHelper
    {
        /// <summary>
        /// Checks if the question is draft
        /// </summary>
        /// <param name="question">Question to check</param>
        /// <returns>If question is draft</returns>
        public static bool IsDraft(this Bfw.Agilix.DataContracts.Question question)
        {
            var draftValue = question.MetadataElements.GetValue(MetadataFieldNames.DraftFrom, null);

            if (draftValue != null)
            {
                if (!String.IsNullOrEmpty(draftValue.Value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Using a particular template returns the list of all the resources that question body contains
        /// </summary>
        /// <param name="questionBody"></param>
        /// <returns></returns>
        public static List<string> GetQuestionRelatedResources(string questionBody)
        {
            MatchCollection matchList = Regex.Matches(questionBody, @"src=""\[~\](.*?)""", RegexOptions.IgnoreCase);
            return  matchList.Cast<Match>().Select(match => match.Groups.Cast<Capture>().Skip(1).First().Value).Distinct().ToList();
        }
    }
}
