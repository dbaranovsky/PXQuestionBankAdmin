using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// View model for the homework/quiz settings tab
    /// </summary>
    public class AssessmentSettings
    {
        /// <summary>
        /// The ID of the item these settings belong to.
        /// </summary>
        public string AssessmentId { get; set; }

        /// <summary>
        /// The ID of the entity (course or group) these settings are for.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// Whether these settings apply to a course or not.
        /// </summary>
        public bool EntityIdIsCourseId { get; set; }

        /// <summary>
        /// The type of assessment (homework or quiz).
        /// </summary>
        public AssessmentType AssessmentType { get; set; }

        [Display(Name = "Number of Attempts")]
        [Range(-1, 10)]
        public NumberOfAttempts NumberOfAttempts { get; set; }

        [Display(Name = "Scored Attempt")]
        public SubmissionGradeAction SubmissionGradeAction { get; set; }

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

        [Display(Name = "Show question score after...")]
        public ReviewSetting? ShowScoreAfter { get; set; }

        [Display(Name = "Show questions and student answers after...")]
        public ReviewSetting? ShowQuestionsAnswers { get; set; }

        [Display(Name = "Show whether answers were right/wrong after...")]
        public ReviewSetting? ShowRightWrong { get; set; }

        [Display(Name = "Show correct answers after...")]
        public ReviewSetting? ShowAnswers { get; set; }

        [Display(Name = "Show feedbacks and grader remarks after...")]
        public ReviewSetting? ShowFeedbackAndRemarks { get; set; }

        [Display(Name = "Show solutions after...")]
        public ReviewSetting? ShowSolutions { get; set; }

        [Display(Name = "Allow students to e-mail instructor concerning results")]
        public bool StudentsCanEmailInstructors { get; set; }

        [Display(Name = "Scored Attempt")]
        public GradeRule GradeRule { get; set; }

        /// <summary>
        /// The Target score for a learning curve activity
        /// </summary>
        [Display(Name = "Target Score")]
        public string LearningCurveTargetScore { get; set; }

        /// <summary>
        /// The Target score for a learning curve activity
        /// </summary>
        [Display(Name = "Auto Target Score")]
        public bool AutoTargetScore { get; set; }

        /// <summary>
        /// Auto calibrate leaving curve difficulty
        /// </summary>
        [Display(Name = "Auto Calibrate Difficulty")]
        public bool AutoCalibrateDifficulty { get; set; }
    }

    /// <summary>
    /// Assessment review settings
    /// </summary>
    public enum ReviewSetting
    {
        Each = 0,
        DueDate = 1,
        Never = 2,
        Second = 3,
        Final = 4
    }

    public enum AssessmentType
    {
        Quiz,
        Homework,
        LearningCurve,
        HtmlQuiz
    }

    public class NumberOfAttempts
    {
        public int? Attempts { get; set; }
    }
}
