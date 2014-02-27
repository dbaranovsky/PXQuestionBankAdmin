using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.SSO
{
    /// <summary>
    /// Connects an AgilixUser to a course.
    /// </summary>
    public class AgilixEnrollment
    {
        /// <summary>
        /// Agilix enrollment ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Agilix course ID.
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// ID of the domain in which the course exists.
        /// </summary>
        public string CourseDomainID { get; set; }
    }
}
