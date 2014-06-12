
using System;
using Bfw.Common.Collections;
using Macmillan.PXQBA.Business.Models;

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
    }
}
