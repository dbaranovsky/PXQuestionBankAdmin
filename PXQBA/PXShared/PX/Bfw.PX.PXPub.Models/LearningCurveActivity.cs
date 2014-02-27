using System.Collections.Generic;
using System.ComponentModel;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// learning Curve
    /// </summary>
    public class LearningCurveActivity : Quiz
    {
        public LearningCurveActivity()
        {
            this.QuizType = Models.QuizType.LearningCurve;
            this.SubType = "LearningCurveActivity";
            Topics = new List<Quiz>();
        }

        /// <summary>
        /// Auto Calibrate Difficulty
        /// </summary>
        public bool AutoCalibrateDifficulty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [auto target score].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto target score]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoTargetScore { get; set; }

        /// <summary>
        /// Target Score
        /// </summary>
        public string TargetScore { get; set; }

        /// <summary>
        /// Topics
        /// </summary>
        public List<Quiz> Topics { get; set; }

        /// <summary>
        /// Book Id
        /// </summary>
        public string BookId { get; set; }

        /// <summary>
        /// WhoopsRight
        /// </summary>
        [DisplayName("Whoops right")]
        public string WhoopsRight { get; set; }

        /// <summary>
        /// WhoopsWrong
        /// </summary>
        [DisplayName("Whoops wrong")]
        public string WhoopsWrong { get; set; }

        /// <summary>
        /// Description for reference
        /// </summary>
        [DisplayName("eBook Reference Description")]
        public string EbookReferenceDescription { get; set; }

        /// <summary>
        /// Gets or sets the default question id.
        /// Used to see the preview of the question.
        /// </summary>
        /// <value>
        /// The default question id.
        /// </value>
        public string DefaultQuestionId { get; set; }
    }
}
