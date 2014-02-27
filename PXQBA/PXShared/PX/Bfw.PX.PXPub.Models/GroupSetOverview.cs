using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class GroupSetOverview
    {
        /// <summary>
        /// Gets or sets the course title.
        /// </summary>
        /// <value>
        /// The course title.
        /// </value>
        public string CourseTitle { get; set; }

        /// <summary>
        /// Gets or sets the section.
        /// </summary>
        /// <value>
        /// The section.
        /// </value>
        public string Section { get; set; }

        /// <summary>
        /// Gets or sets the student count.
        /// </summary>
        /// <value>
        /// The student count.
        /// </value>
        public int StudentCount { get; set; }

        /// <summary>
        /// Gets or sets the group sets.
        /// </summary>
        /// <value>
        /// The group sets.
        /// </value>
        public IEnumerable<GroupSet> GroupSets { get; set; }
    }
}
