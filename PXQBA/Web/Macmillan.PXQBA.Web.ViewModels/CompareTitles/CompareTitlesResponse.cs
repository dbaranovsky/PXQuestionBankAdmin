using System.Collections;
using System.Collections.Generic;

namespace Macmillan.PXQBA.Web.ViewModels.CompareTitles
{
    /// <summary>
    /// Response that is returned to comparison list
    /// </summary>
    public class CompareTitlesResponse
    {
        /// <summary>
        /// Questions that get into comparison
        /// </summary>
        public IList<ComparedQuestionViewModel> Questions { get; set; }

        /// <summary>
        /// Page number requested
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Total pages count
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Question card layout for first course
        /// </summary>
        public string FirstCourseQuestionCardLayout { get; set; }

        /// <summary>
        /// Question card layout for second course
        /// </summary>
        public string SecondCourseQuestionCardLayout { get; set; }

        /// <summary>
        /// Shows if questions have the same repository
        /// </summary>
        public bool OneQuestionRepositrory { get; set; }
    }
}
