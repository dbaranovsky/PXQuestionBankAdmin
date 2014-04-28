using System.Collections.Generic;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.ViewModels.Filter;

namespace Macmillan.PXQBA.Web.ViewModels
{
    /// <summary>
    /// Represent collection of question for react.js controls
    /// </summary>
    public class QuestionListDataResponse
    {
        /// <summary>
        /// Filtration
        /// </summary>
        public IEnumerable<FilterFieldDescriptor> Filter { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// List of questions
        /// </summary>
        public IEnumerable<QuestionMetadata> QuestionList { get; set; }

        /// <summary>
        /// Total pages for current query
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Field Ordering
        /// </summary>
        public QuestionOrder Order { get; set; }

        public IEnumerable<QuestionFieldViewModel> Columns { get; set; }

        public IEnumerable<QuestionFieldViewModel> AllAvailableColumns { get; set; }

        public string QuestionCardLayout { get; set; }

        public string ProductTitle { get; set; }
    }
}