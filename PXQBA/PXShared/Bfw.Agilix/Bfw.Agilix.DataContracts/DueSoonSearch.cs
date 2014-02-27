using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters for searching for due soon items in DLAP.
    /// </summary>
    [DataContract]
    public class DueSoonSearch
    {
        /// <summary>
        /// User Id (will select assigments across all courses)
        /// </summary>
        [DataMember]
        public string UserId { get; set; }

        /// <summary>
        /// Student Enrollment Id 
        /// </summary>
        [DataMember]
        public string EnrollmentId { get; set; }

        /// <summary>
        /// When true, returns items that are due soon even if already completed. The default is false.
        /// </summary>
        [DataMember]
        public bool ShowCompleted { get; set; }

        /// <summary>
        /// When true, returns items that are due soon even if past due. The default is false.
        /// </summary>
        [DataMember]
        public bool ShowPastDue { get; set; }

        /// <summary>
        /// Filters the list of items by due date. The due date must fall between now and the number of days in the future. The default is 14.
        /// </summary>
        [DataMember]
        public int Days { get; set; }

        /// <summary>
        /// The course time difference between GMT and local time, in minutes.
        /// </summary>
        [DataMember]
        public int UtcOffSet { get; set; }

        /// <summary>
        /// Initializes new instance 
        /// </summary>
        public DueSoonSearch()
        {
            ShowCompleted = false;
            ShowPastDue = false;
            Days = 14;
        }
    }
}
