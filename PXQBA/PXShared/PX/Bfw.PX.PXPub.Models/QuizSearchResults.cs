using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class QuizSearchResults
    {
        /// <summary>
        /// Gets or sets the results.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public SearchQuery Query { set; get; }

        /// <summary>
        /// Gets or sets the quiz.
        /// </summary>
        /// <value>
        /// The quiz.
        /// </value>
        public Quiz Quiz { get; set; }       
    }
}
