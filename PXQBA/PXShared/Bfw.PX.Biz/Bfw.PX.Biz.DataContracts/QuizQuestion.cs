using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a question as it relates to a quiz.  I.e., it doesn't contain
    /// the question text or other data, just the information that is managed in the
    /// join to the quiz.
    /// </summary>
    public class QuizQuestion
    {
        public string QuestionId { get; set; }
        public string EntityId { get; set; }
        public string Type { get; set; }
        public string Score { get; set; }
        public string Count { get; set; }
        public ReviewSettings ReviewSettings { get; set; }
    }
}
