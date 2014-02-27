using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents an attempt against a question.
    /// </summary>
    public class SubmissionAttempt
    {
        /// <summary>
        /// ID of the question that was attempted
        /// </summary>
        public string QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the part id.
        /// </summary>
        /// <value>
        /// The part id.
        /// </value>
        public string PartId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [to continue].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [to continue]; otherwise, <c>false</c>.
        /// </value>
        public bool ToContinue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [start page].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [start page]; otherwise, <c>false</c>.
        /// </value>
        public bool StartPage { get; set; }

        /// <summary>
        /// Gets or sets the last save.
        /// </summary>
        /// <value>
        /// The last save.
        /// </value>
        public string LastSave { get; set; }

        /// <summary>
        /// Gets or sets the seconds spent.
        /// </summary>
        /// <value>
        /// The seconds spent.
        /// </value>
        public string SecondsSpent { get; set; }

    }
}
