using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents an possible answer in a multiple choice or matching question.
    /// </summary>
    public class QuestionChoice
    {
        /// <summary>
        /// Id of the choice.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Text of the choice.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Feedback per choice
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// Correct match for this choice (used in matching questions)
        /// </summary>
        public string Answer { get; set; }
    }
}
