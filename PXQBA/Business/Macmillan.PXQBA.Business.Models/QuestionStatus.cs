using System.ComponentModel;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question status
    /// </summary>
    public enum QuestionStatus
    {
        [Description("Available to instructors")]
        AvailableToInstructors,
        [Description("In progress")]
        InProgress,
        [Description("Deleted")]
        Deleted
    }
}