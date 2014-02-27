using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class QuestionChoice
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
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
