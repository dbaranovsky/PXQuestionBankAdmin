// -----------------------------------------------------------------------
// <copyright file="RubricGrade.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.PX.Biz.DataContracts
{
[System.Serializable]

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RubricGrade
    {
        /// <summary>
        /// Gets or sets the rubric rule id.
        /// </summary>
        /// <value>
        /// The rubric rule id.
        /// </value>
        public string RubricRuleId { get; set; }

        /// <summary>
        /// Gets or sets the enrollment id.
        /// </summary>
        /// <value>
        /// The enrollment id.
        /// </value>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the achieved.
        /// </summary>
        /// <value>
        /// The achieved.
        /// </value>
        public double Achieved { get; set; }

        /// <summary>
        /// Gets or sets the possible.
        /// </summary>
        /// <value>
        /// The possible.
        /// </value>
        public double Possible { get; set; }
    }
}
