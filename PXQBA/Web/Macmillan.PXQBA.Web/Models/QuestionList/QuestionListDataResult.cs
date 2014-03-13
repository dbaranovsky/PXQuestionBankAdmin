using System.Collections.Generic;
using System.Linq;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Web.Models.QuestionList
{
    /// <summary>
    /// Represent collection of question for react.js controls
    /// </summary>
    public class QuestionListDataResult
    {
        /// <summary>
        /// Current page number
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// List of questions
        /// </summary>
        public IEnumerable<Question> Data { get; set; }

        /// <summary>
        /// Total pages for current query
        /// </summary>
        public int TotalPages { get; set; } 
    }
}