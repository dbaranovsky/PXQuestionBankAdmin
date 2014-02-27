using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    public class QuestionSettings
    {
        public string Id { get; set; }
        public string QuizId { get; set; }
        public string EntityId { get; set; }

        [Display(Name="Number of attempts")]
        [Required()]
        public NumberOfAttempts AttemptLimit { get; set; }

        [Display(Name = "Points possible")]
        [Required()]
        public double? Points { get; set; }

        [Display(Name = "Time limit")]
        [Required()]
        public int? TimeLimit { get; set; }

        [Display(Name = "Show question score after...")]
        [Required()]
        public ReviewSetting? ShowScoreAfter { get; set; }

        [Display(Name = "Show questions and student answers after...")]
        [Required()]
        public ReviewSetting? ShowQuestionsAnswers { get; set; }

        [Display(Name = "Show whether answers were right/wrong after...")]
        [Required()]
        public ReviewSetting? ShowRightWrong { get; set; }

        [Display(Name = "Show correct answers after...")]
        [Required()]
        public ReviewSetting? ShowAnswers { get; set; }

        [Display(Name = "Show feedbacks and grader remarks after...")]
        [Required()]
        public ReviewSetting? ShowFeedbackAndRemarks { get; set; }

        [Display(Name = "Show solutions after...")]
        [Required()]
        public ReviewSetting? ShowSolutions { get; set; }

        public QuestionSettings()
        {
            ShowScoreAfter = ShowQuestionsAnswers = ShowRightWrong = ShowAnswers = ShowFeedbackAndRemarks = ShowSolutions = ReviewSetting.Each;
            AttemptLimit = new NumberOfAttempts();
            Points = TimeLimit = 0;
        }
    }
}
