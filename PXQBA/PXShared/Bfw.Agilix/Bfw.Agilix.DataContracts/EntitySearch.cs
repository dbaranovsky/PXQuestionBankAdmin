using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters for a Course search
    /// </summary>
    public class EntitySearch
    {
        /// <summary>
        /// The ID of the course to search for
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// The ID of the section to search for
        /// </summary>
        public string SectionId { get; set; }

        /// <summary>
        /// Filters out enrollments by userid
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Filters out enrollments by enrollmentid
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// If set to true, will return all enrollments, even the ones that are invalid.
        /// </summary>
        public bool AllStatus { get; set; }

        /// <summary>
        /// DLAP Right flags
        /// </summary>
        public string Flags { get; set; }

        /// <summary>
        /// XPath syntax based query where / is assumed to be the data element of the course.
        /// </summary>
        public string Query { get; set; }
    }
}
