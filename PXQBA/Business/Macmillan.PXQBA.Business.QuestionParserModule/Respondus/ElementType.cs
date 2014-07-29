using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macmillan.PXQBA.Business.QuestionParserModule.Respondus
{
    /// <summary>
    /// Type of the element in Respondus file
    /// </summary>
    internal enum ElementType
    {
        QuestionTitle,
        Feedback,
        GeneralFeedback,
        CorrectAnswer,
        CorrectAnswers,
        None,
        MatchingChoice
    }
}
