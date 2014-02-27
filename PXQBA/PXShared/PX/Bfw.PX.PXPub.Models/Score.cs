using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a score on an assignment
    /// </summary>
    public class Score
    {
        /// <summary>
        /// The grade achieved or number of items correct
        /// </summary>
        /// <value>
        /// The correct.
        /// </value>
        public double Correct { get; set; }

        /// <summary>
        /// The possible grade achievable or number of items in the assignment
        /// </summary>
        /// <value>
        /// The possible.
        /// </value>
        public double Possible { get; set; }

        /// <summary>
        /// The date that the score was applied
        /// </summary>
        /// <value>
        /// The date.
        /// </value>
        public DateTime Date { get; set; }
    }
}
