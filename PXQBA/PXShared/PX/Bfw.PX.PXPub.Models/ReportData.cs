using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    public class ReportData
    {
        /// <summary>
        /// Gets or sets the students.
        /// </summary>
        /// <value>
        /// The students.
        /// </value>
        public List<Student> Students { get; set; }

        /// <summary>
        /// Gets or sets the group list.
        /// </summary>
        /// <value>
        /// The group list.
        /// </value>
        public List<Group> GroupList { get; set; }

        /// <summary>
        /// Gets the start date.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return DateTime.Now.GetCourseDateTime();
            }
        }

        /// <summary>
        /// Gets the end date.
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return DateTime.Now.GetCourseDateTime();
            }
        }

    }
}
