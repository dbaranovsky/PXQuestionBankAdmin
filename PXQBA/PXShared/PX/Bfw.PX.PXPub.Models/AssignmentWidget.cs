using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Bfw.Common;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    [DataContract]
    public class CalendarAssignment
    {
        [DataMember(Name = "id")]
        public string id {get;set;}
        [DataMember(Name = "itemid")]
        public string itemid {get;set;}
        [DataMember(Name = "entityid")]
        public string entityid { get; set; }
        [DataMember(Name = "start")]
        public DateTime start {get;set;}
        [DataMember(Name = "originalstart")]
        public DateTime originalstart { get; set; }
        [DataMember(Name = "title")]
        public string title {get;set;}
        [DataMember(Name = "points")]
        public double points {get; set;}
        [DataMember(Name = "editLink")]
        public string editLink {get;set;}
        [DataMember(Name = "openLink")]
        public string openLink {get;set;}
        [DataMember(Name = "type")]
        public string type {get;set;}
        [DataMember(Name = "adjustedGroups")]
        public string adjustedGroups { get; set; }                                 
    }

    /// <summary>
    /// Contains all data required to display an assignment widget
    /// </summary>

    public class AssignmentWidget
    {
        /// <summary>
        /// Gets or sets the groups.
        /// </summary>
        /// <value>
        /// The groups.
        /// </value>
        public List<AssignedItemGroup> Groups
        {
            get;
            set;
        }

        /// <summary>
        /// Holds all of the assignments to be shown by the widget
        /// </summary>
        /// <value>
        /// All.
        /// </value>
        public IOrderedEnumerable<AssignedItem> All
        {
            get;
            set;
        }

        /// <summary>
        /// The date for which to calculate derived properties (e.g. ThisWeek)
        /// </summary>
        /// <value>
        /// The reference date.
        /// </value>
        public DateTime ReferenceDate
        {
            get;
            protected set;
        }

        /// <summary>
        /// Default constructor to use current date as reference date
        /// </summary>
        public AssignmentWidget()
            : this(DateTime.Now.GetCourseDateTime())
        {
        }

        /// <summary>
        /// Constructor to allow for an arbitrary date to be used as reference date
        /// </summary>
        /// <param name="referenceDate">The reference date.</param>
        public AssignmentWidget(DateTime referenceDate)
        {
            ReferenceDate = referenceDate;
            Groups = new List<AssignedItemGroup>();
            HasData = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has data; otherwise, <c>false</c>.
        /// </value>
        public bool HasData { get; set; }
    }
}
