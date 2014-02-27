using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents an ActivitiesWidget that can be viewed by user.
    /// </summary>
    public class ActivitiesWidget
    {
        /// <summary>
        /// Dictionary of ContentItems grouped by meta-topic.
        /// </summary>
        public Dictionary<string, List<Activity>> GroupedActivities { get; set; }

        /// <summary>
        /// Dictionary of ContentItems grouped by meta-topic.
        /// </summary>
        public bool isSortable { get; set; }

        /// <summary>
        /// Dictionary of ContentItems grouped by meta-topic.
        /// </summary>
        public string Userspace { get; set; }

        /// <summary>
        /// Dictionary of ContentItems grouped by meta-topic.
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Dictionary of ContentItems grouped by meta-topic.
        /// </summary>
        public string CourseUrl { get; set; }

        /// <summary>
        /// Gets or sets the user access.
        /// </summary>
        /// <value>
        /// The user access.
        /// </value>
        public Bfw.PX.Biz.ServiceContracts.AccessLevel UserAccess { get; set; }

        /// <summary>
        /// Constructor for ActivitiesWidget model. Initializes GroupedActivities dictionary.
        /// </summary>
        public ActivitiesWidget()
        {
            GroupedActivities = new Dictionary<string, List<Activity>>();
        }
    }
}
