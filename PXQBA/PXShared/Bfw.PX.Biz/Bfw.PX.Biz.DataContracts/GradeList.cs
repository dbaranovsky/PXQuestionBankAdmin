using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
    [Serializable]
    public class GradeList
    {
        /// <summary>
        /// Status of the grade.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// version of the last teacher resposne.
        /// </summary>
        public string Responseversion { get; set; }

        /// <summary>
        /// Duration of time spent on the material.
        /// </summary>
        public string Seconds { get; set; }

        /// <summary>
        /// Date of last submission.
        /// </summary>
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// Last submitted version.
        /// </summary>
        public string Submittedversion { get; set; }
    }
}
