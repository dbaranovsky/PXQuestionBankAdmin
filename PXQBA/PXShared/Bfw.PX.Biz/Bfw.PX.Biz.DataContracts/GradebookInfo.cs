using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a summary of gradebook information for an item.
    /// </summary>
    public class GradebookInfo
    {
        /// <summary>
        /// Id of the item.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// The average score for graded submissions of this item.
        /// </summary>
        public double AverageScore { get; set; }

        /// <summary>
        /// The number of submissions made on this item.
        /// </summary>
        public int TotalSubmissions { get; set; }

        /// <summary>
        /// The number of enrollments.
        /// </summary>
        public int TotalGrades { get; set; }

        /// <summary>
        /// If item is submitted by current user.
        /// </summary>
        public bool IsUserSubmitted { get; set; }

        /// <summary>
        /// If item is graded for current user.
        /// </summary>
        public bool IsUserGraded { get; set; }

        /// <summary>
        /// Score, if item is submitted and graded for current user.
        /// </summary>
        public double LastScore { get; set; }     
    }
}
