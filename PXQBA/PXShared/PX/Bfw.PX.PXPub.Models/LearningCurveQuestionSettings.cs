// -----------------------------------------------------------------------
// <copyright file="LearningCurveQuestionSettings.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------


namespace Bfw.PX.PXPub.Models
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LearningCurveQuestionSettings
    {
        public string Id { get; set; }
        public string QuizId { get; set; }
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the type of the question.
        /// </summary>
        /// <value>
        /// The type of the question.
        /// </value>
        public string QuestionType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [scramble answers].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [scramble answers]; otherwise, <c>false</c>.
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
