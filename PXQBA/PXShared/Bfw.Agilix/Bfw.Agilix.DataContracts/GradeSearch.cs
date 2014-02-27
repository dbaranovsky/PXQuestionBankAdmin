using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters for performing a grade search
    /// </summary>
    [DataContract]
    public class GradeSearch
    {
        /// <summary>
        /// The Enrollment for which to find grades.  At least one of
        /// EnrollmentId, EntityId, or UserId must be set to perform a search.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// The Entity for which to find grades.  At least one of
        /// EnrollmentId, EntityId, or UserId must be set to perform a search.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// The User for which to find grades.  This will find all of the
        /// user's grades for all of their enrollments.  At least one of
        /// EnrollmentId, EntityId, or UserId must be set to perform a search.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// A list of ItemIds for which to return grades (within the enrollment,
        /// entity, or user specified by other parameters).  If this is null
        /// then all items will be returned.  If it is an empty list then no item
        /// data will be returned.
        /// </summary>
        public IEnumerable<string> ItemIds { get; set; }

        /// <summary>
        /// Command that was requested
        /// </summary>
        public string CommandRequested { get; set; }

    }
}
