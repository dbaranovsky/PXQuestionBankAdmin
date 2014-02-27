// -----------------------------------------------------------------------
// <copyright file="LearningCurveQuestionSettings.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.PX.Biz.DataContracts
{
[System.Serializable]

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LearningCurveQuestionSettings
    {
        /// <summary>
        /// The Id of the quiz id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the question ID.
        /// </summary>
        /// <value>
        /// The question ID.
        /// </value>
        public string QuestionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [never scramble answers].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [never scramble answers]; otherwise, <c>false</c>.
        /// </value>
        public bool NeverScrambleAnswers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [primary question].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [primary question]; otherwise, <c>false</c>.
        /// </value>
        public bool PrimaryQuestion { get; set; }

        /// <summary>
        /// Gets or sets the difficulty level.
        /// </summary>
        /// <value>
        /// The difficulty level.
        /// </value>
        public string DifficultyLevel { get; set; }
    }
}
