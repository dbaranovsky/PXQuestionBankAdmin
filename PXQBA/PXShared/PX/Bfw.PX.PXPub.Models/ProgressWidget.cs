using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class ProgressWidget
    {
        /// <summary>
        /// The overall grade to date; could be a percentage or a letter grade or something else
        /// </summary>
        /// <value>
        /// The overall grade.
        /// </value>
        public string OverallGrade { get; set; }

        /// <summary>
        /// The number of assignments that are graded out of the number of assignments that are gradable
        /// </summary>
        /// <value>
        /// The percent graded.
        /// </value>
        public double PercentGraded { get; set; }

        /// <summary>
        /// The number of assignments that have been submitted or graded
        /// </summary>
        /// <value>
        /// The complete.
        /// </value>
        public int Complete { get; set; }

        /// <summary>
        /// The number of assignemnts that are due today or prior
        /// </summary>
        /// <value>
        /// The due.
        /// </value>
        public int Due { get; set; }
    }
}
