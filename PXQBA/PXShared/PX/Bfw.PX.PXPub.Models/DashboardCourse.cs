using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class DashboardCourse
    {
        /// <summary>
        /// Course id.
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Course title
        /// </summary>
        public string CourseTitle { get; set; }

        /// <summary>
        /// Status of the course.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Owner of the course
        /// </summary>
        public string OwnerName { get; set; }

        /// <summary>
        /// Course Owners First Name
        /// </summary>
        public string OwnerFirstName { get; set; }

        /// <summary>
        /// Course Owners Last Name
        /// </summary>
        public string OwnerLastName { get; set; }

        /// <summary>
        /// Course Owner's email address
        /// </summary>
        public string OwnerEmail { get; set; }

        /// <summary>
        /// Owner id of the course
        /// </summary>
        public string OwnerId { get; set; }

        /// <summary>
        /// Students enrolled for eportfolio course
        /// Derived eportfolios courses for template courses
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public DashboardCourse()
        {

        }
    }
}
