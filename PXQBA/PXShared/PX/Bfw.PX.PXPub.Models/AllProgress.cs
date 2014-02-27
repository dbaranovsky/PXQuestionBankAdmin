using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// The data necessary for the progress view all view to display.
    /// </summary>
    public class AllProgress
    {
        /// <summary>
        /// The Enrollment whose progress we are looking at.
        /// </summary>
        /// <value>
        /// The enrollment id.
        /// </value>
        public string EnrollmentId { get; set; }
    }
}
