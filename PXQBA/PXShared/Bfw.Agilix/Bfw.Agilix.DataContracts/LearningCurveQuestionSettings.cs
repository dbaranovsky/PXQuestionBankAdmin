// -----------------------------------------------------------------------
// <copyright file="LearningCurveQuestionSettings.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LearningCurveQuestionSettings : IDlapEntityParser
    {
        /// <summary>
        /// The Id of the quiz id
        /// </summary>
        public string Id { get; set; }

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

        /// <summary>
        /// Parse and objective XML element and populate this object's state.
        /// </summary>
        /// <param name="root"></param>
        public void ParseEntity(XElement root)
        {
            if (root.Name != ElStrings.meta_lc_question_setting.LocalName)
            {
                throw new DlapEntityFormatException(string.Format("Expected root element to be 'meta_lc_question_setting', but got '{0}' instead", root.Name));
            }

            var guid = root.Attribute(ElStrings.Id);
            if (guid == null)
            {
                throw new DlapEntityFormatException("Required attribute 'guid' is missing from 'item' element");
            }
            else
            {
                Id = guid.Value;
            }

            var neverScrambleAnswers = root.Attribute(ElStrings.never_scramble_answers);

            if (neverScrambleAnswers != null)
            {
                NeverScrambleAnswers = neverScrambleAnswers.Value.Trim().ToLower() == "true" ? true : false;
            }

            var primaryQuestion = root.Attribute(ElStrings.primary_question);
            if (primaryQuestion != null)
            {
                PrimaryQuestion = primaryQuestion.Value.Trim().ToLower() == "true" ? true : false;
            }

            var difficultyLevel = root.Attribute(ElStrings.difficulty_level);
            if (difficultyLevel != null)
            {
                DifficultyLevel = difficultyLevel.Value;
            }
        }
    }
}
