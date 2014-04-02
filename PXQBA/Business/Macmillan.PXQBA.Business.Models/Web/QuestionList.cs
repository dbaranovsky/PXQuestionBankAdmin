using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models.Web
{
    /// <summary>
    /// Represent collection of questions 
    /// </summary>
    public class QuestionList
    {
        public QuestionList()
        {
            Questions = new List<QuestionMetadata>();
        }

        /// <summary>
        /// Collection of questions
        /// </summary>
        public IList<QuestionMetadata> Questions { get; set; }

        /// <summary>
        /// All questions count in the data store
        /// </summary>
        public int AllQuestionsAmount { get; set; }
    }
}
