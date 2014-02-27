using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a question choice business object. 
    /// (See http://gls.agilix.com/Docs/Schema/Question).
    /// Represents an possible answer in a multiple choice or matching question.
    /// </summary>
    public class QuestionChoice
    {
        /// <summary>
        /// ID for this choice. 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Choice string that can include HTML.
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
