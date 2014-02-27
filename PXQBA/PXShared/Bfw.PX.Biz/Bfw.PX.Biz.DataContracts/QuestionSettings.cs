using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents all settings that can be changed for a Question in a
    /// Quiz.
    /// </summary>
    public class QuestionSettings
    {
        public string Id { get; set; }
        public string QuizId { get; set; }
        public string EntityId { get; set; }
        public int? AttemptLimit { get; set; }
        public int? TimeLimit { get; set; }
        public double? Points { get; set; }
        public int? Score { get; set; }
        public ReviewSettings ReviewSettings { get; set; }
    }

    public class ReviewSettings
    {
        public ReviewSetting ShowScoreAfter { get; set; }
        public ReviewSetting ShowQuestionsAnswers { get; set; }
        public ReviewSetting ShowRightWrong { get; set; }
        public ReviewSetting ShowAnswers { get; set; }
        public ReviewSetting ShowFeedbackAndRemarks { get; set; }
        public ReviewSetting ShowSolutions { get; set; }
    }
}
