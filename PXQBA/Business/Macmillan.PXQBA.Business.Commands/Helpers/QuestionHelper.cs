
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Bfw.Common.Collections;
using Macmillan.PXQBA.Business.Models;
using Question = Bfw.Agilix.DataContracts.Question;

namespace Macmillan.PXQBA.Business.Commands.Helpers
{
    public static class QuestionHelper
    {
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

        public static List<string> GetQuestionRelatedResources(string questionBody)
        {
            MatchCollection matchList = Regex.Matches(questionBody, @"src=""\[~\](.*?)""", RegexOptions.IgnoreCase);
            return  matchList.Cast<Match>().Select(match => match.Groups.Cast<Capture>().Skip(1).First().Value).Distinct().ToList();
        }
    }
}
