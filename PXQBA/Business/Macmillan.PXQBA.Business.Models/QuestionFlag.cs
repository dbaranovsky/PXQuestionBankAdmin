using System.ComponentModel;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question flag
    /// </summary>
    public enum QuestionFlag
    {
        [Description("Not Flagged")]
        NotFlagged,
        [Description("Flagged")]
        Flagged
    }
}
