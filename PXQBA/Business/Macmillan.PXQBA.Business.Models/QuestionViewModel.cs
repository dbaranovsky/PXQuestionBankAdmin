using System.Collections.Generic;
using System.Linq;

namespace Macmillan.PXQBA.Business.Models
{
    /// <summary>
    /// Question DTO
    /// </summary>
    public class QuestionViewModel
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public string Chapter { get; set; }
        public string Bank { get; set; }
        public int Sequence { get; set; }
        public QuestionType Type { get; set; }
        public string Preview { get; set; }
        public QuestionStatus Status { get; set; }

        public IEnumerable<string> Keywords { get; set; }

        /// <summary>
        /// Excercise Number for the  question.
        /// </summary>
        public string ExcerciseNo { get; set; }

        /// <summary>
        /// Difficulty for the  question.
        /// </summary>
        public string Difficulty { get; set; }

        /// <summary>
        /// Guidance for the  question.
        /// </summary>
        public string Guidance { get; set; }

        /// <summary>
        /// Question particular version
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Congnitive Level for the  question.
        /// </summary>
        public string CognitiveLevel { get; set; }

        /// <summary>
        /// Suggested Use for the  question.
        /// </summary>
        public IEnumerable<string> SuggestedUse { get; set; }

        /// <summary>
        /// Learning objectives
        /// </summary>
        public IEnumerable<LearningObjective> LearningObjectives { get; set; }

        public bool IsDuplicateOfSharedQuestion
        {
            get
            {
                return !string.IsNullOrEmpty(QuestionIdDuplicateFrom);
            }
        }

        public string QuestionIdDuplicateFrom { get; set; }

        public bool IsShared
        {
            get
            {
                return ProductCourses != null && ProductCourses.Count() > 1;
            }
        }

        public IEnumerable<string> ProductCourses
        {
            get; set;
        }

        public string EntityId { get; set; }

        public string QuizId { get; set; }
        public string ActionPlayerUrl { get; set; }
        public string EditorUrl { get; set; }
        public Question SharedMetadata { get; set; }

    }
}
