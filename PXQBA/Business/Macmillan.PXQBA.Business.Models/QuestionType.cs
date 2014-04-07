using System.ComponentModel;

namespace Macmillan.PXQBA.Business.Models
{
    public enum QuestionType
    {
        [Description("Multiple Choice")]
        MultipleChoice = 0,
        [Description("Matching")]
        Matching,
        [Description("Short Answer")]
        ShortAnswer,
        [Description("Multiple Answer")]
        MultipleAnswer,
        [Description("Essay")]
        Essay,
        [Description("Advanced Question")]
        Advanced,
        [Description("Graph Exercise")]
        GraphExcepcise,
    }
}
