using System;

namespace Bfw.PXWebAPI.Models
{
    public class GradeItemResult
    {
        /// <summary>
        /// User Id
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// score the user received, with curving applied.
        /// </summary>
        
        public double score { get;  set; }

        /// <summary>
        /// Date the assignment was scored.
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// Status of the grade
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Duration in seconds
        /// </summary>
        public int duration { get; set; }
    }
}
