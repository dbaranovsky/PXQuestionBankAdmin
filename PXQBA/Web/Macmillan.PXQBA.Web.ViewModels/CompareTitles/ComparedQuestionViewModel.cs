using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.ViewModels.CompareTitles
{
    /// <summary>
    /// View model for question in comparison list
    /// </summary>
    public class ComparedQuestionViewModel
    {
        /// <summary>
        /// Shows if question belongs to one of the courses or is shared between 2 courses in comparison
        /// </summary>
        public CompareLocationType CompareLocation { get; set; }

        /// <summary>
        /// Question metadata field values
        /// </summary>
        public QuestionMetadata QuestionMetadata { get; set; }
    }
}
