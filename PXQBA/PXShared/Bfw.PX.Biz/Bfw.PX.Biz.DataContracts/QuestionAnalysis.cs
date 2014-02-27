using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// A summary analysis of an assessment or homework item's questions including question difficulty, 
    /// correlation between question performance and assessment performance, and answer choices.
    /// </summary>
    public class QuestionAnalysis
    {
        /// <summary>
        /// Item ID of the homework or assessment item to analyze.
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// Version (represents how many times the question has been modified)
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the question number.
        /// </summary>
        public string QuestionNumber { get; set; }

        /// <summary>
        /// Number of unique enrollments that had attempts for this summary.
        /// </summary>
        public int Enrollments { get; set; }

        /// <summary>
        /// Number of attempts for this summary.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Scaled average score for this summary [0,1].
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Correlation coefficient [-1,1] between students' question scores
        /// and overall assessment scores, which is calculated using Spearman's rank algorithm.
        /// </summary>
        public float Correlation { get; set; }

        /// <summary>
        /// Gets or sets the correct answer count.
        /// </summary>
        public int CorrectAnswerCount { get; set; }

        /// <summary>
        /// Gets or sets the average time.
        /// </summary>
        public string AverageTime { get; set; }
    }
}
