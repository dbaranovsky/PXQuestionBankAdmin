
using System;
using System.Collections.Generic;
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

        public static List<string> GetQuestionRelatedResources(Question question)
        {
            return new List<string>();
        }
    }
}
