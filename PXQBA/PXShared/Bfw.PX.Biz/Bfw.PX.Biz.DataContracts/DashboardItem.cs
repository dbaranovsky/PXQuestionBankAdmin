using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class DashboardItem
    {
        /// <summary>
        /// Course.
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Course id.
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Gets or sets domain id.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Gets or sets domain name.
        /// </summary>
        public string DomainName { get; set; }

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
        /// Owner of the course Reference Id in RA
        /// </summary>
        public string OwnerReferenceId { get; set; }

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
        /// Level 
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// List of shared notes
        /// </summary>
        public IDictionary<string,string> Notes { get; set; }

        /// <summary>
        /// List of shared users
        /// </summary>
        public IDictionary<string, string> Users { get; set; }

        /// <summary>
        /// Comma sepearated list of instructors, with whom the eportfolio has been shared
        /// </summary>
        public string SharedInstructors { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public DashboardItem()
        {
            Notes = new Dictionary<string, string>();
            Users = new Dictionary<string, string>();
        }

    }
}
