using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    [Serializable]
    public class QuizQuestion
    {
        public string QuizId { get; set; }
        public string QuestionId { get; set; }
        public string EntityId { get; set; }
        public bool IsBank { get; set; }
        public int UseCount { get; set; }
        /// <summary>
        /// specifies that the bank is new - create a content item for it
        /// </summary>
        public bool IsNew { get; set; }
        public bool IsEmpty { get; set; }
        public string MainQuizId { get; set; }

        public List<QuizQuestion> BankQuestions { get; set; }
 
        public QuizQuestion()
        {
            BankQuestions = new List<QuizQuestion>();
        }
    }

    /// <summary>
    /// Used to model changes in order for list of questions of a quiz
    /// </summary>
    [Serializable]
    public class QuizQuestions
    {
        /// <summary>
        /// Gets or sets the quiz id.
        /// </summary>
        /// <value>
        /// The quiz id.
        /// </value>
        public string QuizId { get; set; }

        /// <summary>
        /// A comma-delimited list of:
        /// questionId|entityId
        /// </summary>
        /// <value>
        /// The question ids.
        /// </value>
        public string QuestionIds { get; set; }
        
        /// <summary>
        /// Serialized list of questions including questions inside banks
        /// </summary>
        public List<QuizQuestion> Questions { get; set; }


        /// <summary>
        /// Quiz id of the main quiz being saved 
        /// Used when saving pools to keep track of the parent of the pool
        /// </summary>
        public string MainQuizId { get; set; }
    }
}
