using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Models
{
    public class QuestionStaticMetadata
    {
        public string Title { get; set; }
        public string Chapter { get; set; }
        public string Bank { get; set; }
        public int Sequence { get; set; }

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
    }
}