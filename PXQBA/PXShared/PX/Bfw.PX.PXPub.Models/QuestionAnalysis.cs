using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class QuestionAnalysis
    {
        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        /// <value>
        /// The question id.
        /// </value>
        public string QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the question number.
        /// </summary>
        /// <value>
        /// The question number.
        /// </value>
        public string QuestionNumber { get; set; }

        /// <summary>
        /// Gets or sets the enrollments.
        /// </summary>
        /// <value>
        /// The enrollments.
        /// </value>
        public int Enrollments { get; set; }

        /// <summary>
        /// Gets or sets the attempts.
        /// </summary>
        /// <value>
        /// The attempts.
        /// </value>
        public int Attempts { get; set; }

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public float Score { get; set; }

        /// <summary>
        /// Gets or sets the correlation.
        /// </summary>
        /// <value>
        /// The correlation.
        /// </value>
        public float Correlation { get; set; }

        /// <summary>
        /// Gets or sets the correct answer count.
        /// </summary>
        /// <value>
        /// The correct answer count.
        /// </value>
        public int CorrectAnswerCount { get; set; }

        /// <summary>
        /// Gets or sets the average time.
        /// </summary>
        /// <value>
        /// The average time.
        /// </value>
        public string AverageTime { get; set; }

        /// <summary>
        /// Gets the percent correct.
        /// </summary>
        public float PercentCorrect
        {
            get
            {                
                if (Attempts > 0)
                    return (CorrectAnswerCount * 100) / Attempts;
                else
                    return 0;
            }
        }
    }
}
