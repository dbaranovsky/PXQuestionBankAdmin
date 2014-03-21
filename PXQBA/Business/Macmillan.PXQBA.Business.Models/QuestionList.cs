using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Represent collection of questions 
    /// </summary>
    public class QuestionList
    {
        public QuestionList()
        {
            Questions = new List<Question>();
        }

        /// <summary>
        /// Collection of questions
        /// </summary>
        public IList<Question> Questions { get; set; }

        /// <summary>
        /// All questions count in the data store
        /// </summary>
        public int AllQuestionsAmount { get; set; }
    }
}
