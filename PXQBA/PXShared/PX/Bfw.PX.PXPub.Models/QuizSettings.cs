using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// View model for the quiz settings tab
    /// </summary>
    public class QuizSettings
    {
        [Display(Name = "Number of Attempts")]
        [Range(0, 10)]
        public NumberOfAttempts NumberOfAttempts { get; set; }

        [Display(Name = "Scored Attempt")]
        public ScoredAttempt ScoredAttempt { get; set; }

        [Display(Name = "Time Limit")]
        public int TimeLimit { get; set; }

        [Display(Name = "Question Delivery")]
        public QuestionDelivery QuestionDelivery { get; set; }

        [Display(Name = "Allow save and continue")]
        public bool AllowSaveAndContinue { get; set; }
        [Display(Name = "Auto-submit saved assessments and partially completed assessments at due time")]
        public bool AutoSubmitAssessments { get; set; }

        [Display(Name = "Randomize question order")]
        public bool RandomizeQuestionOrder { get; set; }
        [Display(Name = "Randomize answer order within questions")]
        public bool RandomizeAnswerOrder { get; set; }

        [Display(Name = "Allow students to view hints for questions that have them")]
        public bool AllowViewHints { get; set; }
        public int HintSubstractPercentage { get; set; }

        [Display(Name = "Show overall score after")]
        public QuizReviewSetting ShowScoreAfter { get; set; }
        [Display(Name = "Show questions, student answers, and points possible after...")]
        public QuizReviewSetting ShowQuestionsAnswersPoints { get; set; }
        [Display(Name = "Show whether answers were right/wrong after...")]
        public QuizReviewSetting ShowRightWrong { get; set; }
        [Display(Name = "Show correct answers after...")]
        public QuizReviewSetting ShowAnswers { get; set; }
        [Display(Name = "Show feedbacks and grader remarks after...")]
        public QuizReviewSetting ShowFeedbackAndRemarks { get; set; }
        [Display(Name = "Show solutions after...")]
        public QuizReviewSetting ShowSolutions { get; set; }

        public bool StudentsCanEmailInstructors { get; set; }
    }

    /// <summary>
    /// Quiz review settings options
    /// </summary>
    public enum QuizReviewSetting
    {
        Each    = 0,
        DueDate = 1,
        Never   = 2
    }

    public class NumberOfAttempts
    {
        public int Attempts { get; set; }
    }
}