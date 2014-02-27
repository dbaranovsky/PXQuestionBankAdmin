using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents an attempt against a question.
    /// </summary>
    public class QuestionAttempt
    {
        /// <summary>
        /// ID of the question that was attempted
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// answer the user submitted for this attempt
        /// </summary>
        public string AttemptAnswer { get; set; }

        /// <summary>
        /// points attributed for this question
        /// </summary>
        /// <value>
        /// The calculated points.
        /// </value>
        public string PointsComputed { get; set; }

        /// <summary>
        /// points available for this question
        /// </summary>
        /// <value>
        /// The available points.
        /// </value>
        public string PointsPossible { get; set; }

        /// <summary>
        /// ID of the attempt
        /// </summary>
        /// <value>
        /// The attempt id.
        /// </value>
        public string PartId { get; set; }

        /// <summary>
        /// ID of the version
        /// </summary>
        /// <value>
        /// The attempt version.
        /// </value>
        public string AttemptVersion { get; set; }

    }
}
