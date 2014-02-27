using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// View model for the homework settings tab
    /// </summary>
    public class HomeworkSettings
    {
        [Display(Name = "Number of Attempts")]
        [Range(1, 3)]
        public int NumberOfAttempts { get; set; }

        [Display(Name = "Scored Attempt")]
        public ScoredAttempt ScoredAttempt { get; set; }

        [Display(Name = "Time Limit")]
        public int TimeLimit { get; set; }

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
        public HomeworkReviewSetting ShowScoreAfter { get; set; }
        [Display(Name = "Show questions, student answers, and points possible after...")]
        public HomeworkReviewSetting ShowQuestionsAnswersPoints { get; set; }
        [Display(Name = "Show whether answers were right/wrong after...")]
        public HomeworkReviewSetting ShowRightWrong { get; set; }
        [Display(Name = "Show correct answers after...")]
        public HomeworkReviewSetting ShowAnswers { get; set; }
        [Display(Name = "Show feedbacks and grader remarks after...")]
        public HomeworkReviewSetting ShowFeedbackAndRemarks { get; set; }
        [Display(Name = "Show solutions after...")]
        public HomeworkReviewSetting ShowSolutions { get; set; }

        public bool StudentsCanEmailInstructors { get; set; }
    }

    /// <summary>
    /// Homework review settings options
    /// </summary>
    public enum HomeworkReviewSetting
    {
        Each,
        Second,
        Final,
        DueDate,
        Never
    }
}