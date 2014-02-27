using System;
using System.Collections.Generic;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a Quiz
    /// </summary>
    public class Quiz : ContentItem
    {
        /// <summary>
        /// Text to describe the policies/settings of the quiz
        /// </summary>
        /// <value>
        /// The policy description.
        /// </value>
        public IList<String> PolicyDescription { get; set; }

        /// <summary>
        /// Gets or sets the questions.
        /// </summary>
        /// <value>
        /// The questions.
        /// </value>
        public IList<Question> Questions { get; set; }

        /// <summary>
        /// Enum for the Display Type
        /// </summary>
        public enum DisplayType
        {
            Instructor,
            Student,
            Anonymous
        }

        /// <summary>
        /// Gets or sets the display.
        /// </summary>
        /// <value>
        /// The display.
        /// </value>
        public DisplayType Display { get; set; }

        /// <summary>
        /// Gets or sets the submissions.
        /// </summary>
        /// <value>
        /// The submissions.
        /// </value>
        public IEnumerable<Submission> Submissions { get; set; }

        /// <summary>
        /// Gets or sets the question types.
        /// </summary>
        /// <value>
        /// The question types.
        /// </value>
        public Dictionary<string, string> QuestionTypes { get; set; }

        /// <summary>
        /// Gets or sets the type of the quiz.
        /// </summary>
        /// <value>
        /// The type of the quiz.
        /// </value>
        public QuizType QuizType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is product course.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is product course; otherwise, <c>false</c>.
        /// </value>
        public bool IsProductCourse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is LC.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is LC; otherwise, <c>false</c>.
        /// </value>
        public bool IsLC { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show reused].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show reused]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowReused { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether [show content view].
        /// If it is true then display anoter grid on the quiz page.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show content view]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowContentView { get; set; }

        /// <summary>
        /// Gets or sets the attempt limit.
        /// </summary>
        /// <value>
        /// The attempt limit.
        /// </value>
        public int AttemptLimit { get; set; }

        /// <summary>
        /// Gets or sets the time limit.
        /// </summary>
        /// <value>
        /// The time limit.
        /// </value>
        public int TimeLimit { get; set; }

        /// <summary>
        /// Gets or sets the scored attempt.
        /// </summary>
        /// <value>
        /// The scored attempt.
        /// </value>
        public string SubmissionGradeAction { get; set; }

        /// <summary>
        /// Gets or sets the specific question ID to load.
        /// </summary>
        /// <value>
        /// The scored attempt.
        /// </value>
        public string QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the quiz paging.
        /// </summary>
        /// <value>
        /// The quiz paging.
        /// </value>
        public Paging QuizPaging { get; set; }

        /// <summary>
        /// Set of attempts for each question in a homework
        /// </summary>
        /// <value>
        /// The question attempts.
        /// </value>
        public IDictionary<string, IList<QuestionAttempt>> QuestionAttempts { get; set; }

        /// <summary>
        /// Set of attempts for each question in a homework
        /// </summary>
        /// <value>
        /// The question attempts.
        /// </value>
        public IDictionary<string, SubmissionAttempt> SubmissionAttempts { get; set; }

        /// <summary>
        /// Stores state of the SubType property.
        /// </summary>
        private string subType = "quiz";
        /// <summary>
        /// Override ContentItem.SubType to set the value to either quiz or homework based on
        /// the value of QuizType. If QuizType == Assessment, then SubType == "quiz". If 
        /// QuizType == Homework then SubType == "homework".
        /// </summary>
        public override string SubType
        {
            get
            {
                switch (QuizType)
                {
                    case Models.QuizType.Homework:
                        subType = "homework";
                        break;
                    case Models.QuizType.LearningCurve:
                        SubType = "learningcurve";
                        break;
                    case Models.QuizType.HtmlQuiz:
                        subType = "htmlquiz";
                        break;
                }

                return subType;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is quiz submission saved.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is quiz submission saved; otherwise, <c>false</c>.
        /// </value>
        public bool IsQuizSubmissionSaved { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quiz"/> class.
        /// </summary>
        public Quiz()
        {
            Type = "Quiz";
            TrackMinutesSpent = false;
            QuestionTypes = new Dictionary<string, string>();
            Questions = new List<Question>();
        }

        /// <summary>
        /// Whether the student can stop taking the quiz and continue it later, without it being automatically submitted.
        /// </summary>
        public bool AllowSaveAndContinue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow re submission].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow re submission]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowResubmission { get; set; }

        /// <summary>
        /// Whether to show review screen when quiz is submitted.
        /// </summary>
        public bool ShowReviewScreen { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RelatedContent RelatedContent;

        /// <summary>
        /// Allow to show grade to user
        /// </summary>
        public bool ShowGrade { get; set; }

        /// <summary>
        /// Get/Set the exam template for the quiz. Currently only used by htmlquiz
        /// </summary>
        public string ExamTemplate { get; set; }

        /// <summary>
        /// Get the name of the quiz type.
        /// </summary>
        /// <param name="quizType">Type of the quiz.</param>
        /// <returns></returns>
        public string GetQuizTypeName(QuizType quizType)
        {
            if (quizType == QuizType.Assessment)
            {
                return "Quiz";
            }
            return quizType.ToString();
        }
    }
}
