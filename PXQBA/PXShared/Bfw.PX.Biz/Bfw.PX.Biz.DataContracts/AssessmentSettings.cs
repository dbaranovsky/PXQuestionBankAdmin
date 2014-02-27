using System;
using System.Runtime.Serialization;
using Bfw.Common;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents assessment settings for a content item.
    /// </summary>
    [DataContract]
    public class AssessmentSettings
    {
        /// <summary>
        /// The number of attempts the student may submit. The value 0 means that the user may use an unlimited amount of attempts.
        /// </summary>
        /// <value>
        /// The attempt limit.
        /// </value>
        [DataMember]
        public int AttemptLimit { get; set; }

        ///// <summary>
        ///// The scored attempt level
        ///// </summary>
        [DataMember]
        public SubmissionGradeAction SubmissionGradeAction { get; set; }


        /// <summary>
        /// The GradeRule
        /// </summary>
        [DataMember]
        public GradeRule GradeRule { get; set; }

        /// <summary>
        /// Gets or sets the time limit.
        /// </summary>
        /// <value>
        /// The time limit.
        /// </value>
        [DataMember]
        public int TimeLimit { get; set; }

        /// <summary>
        /// The question delivery mode
        /// </summary>
        public QuestionDelivery QuestionDelivery { get; set; }

        /// <summary>
        /// Determines the assessment behavior, such as question randomization and feedback.
        /// </summary>
        /// <value>
        /// Any value of <see cref="ExamFlags" />.
        /// </value>
        [DataMember]
        public ExamFlags ExamFlags { get; set; }

        /// <summary>
        /// Gets or sets the default score.
        /// </summary>
        /// <value>
        /// The default score.
        /// </value>
        [DataMember]
        public double DefaultScore { get; set; }

        /// <summary>
        /// Gets or sets the type of the quiz.
        /// </summary>
        /// <value>
        /// The type of the quiz.
        /// </value>
        [DataMember]
        public string QuizType { get; set; }

        /// <summary>
        /// Gets or sets the due date.
        /// </summary>
        /// <value>
        /// The due date.
        /// </value>
        [DataMember]
        public DateTime DueDate { get; set; }

        [DataMember]
        public DateTimeWithZone DueDateTZ { get; set; }

        /// <summary>
        /// Gets or sets the number of questions per page.
        /// </summary>
        /// <value>
        /// The number of questions per page.
        /// </value>
        [DataMember]
        public int QuestionsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the allow save and continue flag
        /// </summary>
        [DataMember]
        public bool AllowSaveAndContinue { get; set; }

        /// <summary>
        /// Gets or sets the allow save and continue flag
        /// </summary>
        [DataMember]
        public bool AutoSubmitAssessments { get; set; }

        /// <summary>
        /// Gets or sets the allow randomize questions option
        /// </summary>
        [DataMember]
        public bool RandomizeQuestionOrder { get; set; }

        /// <summary>
        /// Gets or sets the allow randomize answers option
        /// </summary>
        [DataMember]
        public bool RandomizeAnswerOrder { get; set; }

        /// <summary>
        /// Gets or sets the allow randomize answers option
        /// </summary>
        [DataMember]
        public bool AllowViewHints { get; set; }

        /// <summary>
        /// Gets or sets the allow randomize answers option
        /// </summary>
        [DataMember]
        public int PercentSubstractHint { get; set; }

        public ReviewSetting ShowScoreAfter { get; set; }
        public ReviewSetting ShowQuestionsAnswers { get; set; }
        public ReviewSetting ShowRightWrong { get; set; }
        public ReviewSetting ShowAnswers { get; set; }
        public ReviewSetting ShowFeedbackAndRemarks { get; set; }
        public ReviewSetting ShowSolutions { get; set; }

        public bool StudentsCanEmailInstructors { get; set; }

        /// <summary>
        /// The Target score for a learning curve activity
        /// </summary>
        public string LearningCurveTargetScore { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto target score].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto target score]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoTargetScore { get; set; }

        /// <summary>
        /// Auto calibrate leaving curve difficulty
        /// </summary>
        public bool AutoCalibrateDifficulty { get; set; }

    }
}
