using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Bfw.Common;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a node in the AssignmentCenter navigation.
    /// </summary>
    /// 
    [DataContract]
    public class AssignmentCenterItem
    {
        /// <summary>
        /// Unique Id of the node.
        /// </summary>
        /// 
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Unique Id of the node's parent. Null or empty if the node has no parent.
        /// </summary>
        /// 
        [DataMember(Name = "parentId")]
        public string ParentId { get; set; }

        /// <summary>
        /// Unique Id of the node's previous parent. used when doing a move.
        /// </summary>
        /// 
        [DataMember(Name = "previousparentid")]
        public string PreviousParentId { get; set; }

        /// <summary>
        /// Date the assignment is supposed to start, if any.
        /// </summary>
        /// 
        private DateTime _startDate;

        [DataMember(Name = "startDate")]
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                if (StartDateTZ != null)
                {
                    StartDateTZ.LocalTime = value;
                }
            }
        }

        public DateTimeWithZone StartDateTZ { get; set; }

        /// <summary>
        /// Date the assignment is supposed to end, if any.
        /// </summary>
        /// 
        private DateTime _endDate;

        [DataMember(Name = "endDate")]
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                if (EndDateTZ != null)
                {
                    EndDateTZ.LocalTime = value;
                }
            }
        }

        public DateTimeWithZone EndDateTZ { get; set; }

        /// <summary>
        /// Points possible for item, if any.
        /// </summary>
        [DataMember(Name = "points")]
        public double? Points { get; set; }


        /// <summary>
        /// Default points for item, if any.
        /// </summary>
        [DataMember(Name = "defaultpoints")]
        public double? DefaultPoints { get; set; }

        /// <summary>
        /// Relative order of this item under its parent.
        /// </summary>
        /// 
        [DataMember(Name = "sequence")]
        public string Sequence { get; set; }

        /// <summary>
        /// Expansion state of the item.
        /// Values: open, closed, barren
        /// </summary>
        [DataMember(Name = "state")]
        public string State { get; set; }

        /// <summary>
        /// Title of the item.
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Raw Title of the item.
        /// </summary>
        [DataMember(Name = "rawtitle")]
        public string RawTitle { get; set; }

        /// <summary>
        /// True if student can view, false otherwise.
        /// </summary>
        [DataMember(Name = "isvisibletostudents")]
        public bool isVisibleToStudents { get; set; }

        /// <summary>
        /// User Acces level
        /// </summary>
        [DataMember(Name = "accessLevel")]
        public string AccessLevel { get; set; }

        /// <summary>
        /// Any children that the node has.
        /// </summary>
        /// 
        [DataMember(Name = "children")]
        public List<AssignmentCenterItem> Children { get; set; }

        /// <summary>
        ///represents the date mode
        /// </summary>
        [DataMember(Name = "datemode")]
        public string DateMode { get; set; }


        /// <summary>
        /// the level of this item
        /// </summary>
        [DataMember(Name = "level")]
        public string Level { get; set; }

        /// <summary>
        /// the level of this item
        /// </summary>
        [DataMember(Name = "gradebookcategory")]
        public string GradebookCategory { get; set; }

        /// <summary>
        /// Order in which this item should be displayed relative to other items within assigned category.
        /// </summary>
        [DataMember(Name = "categorysequence")]
        public string CategorySequence { get; set; }

        /// <summary>
        /// PX Unit Gradebook Category
        /// </summary>
        [DataMember(Name = "unitgradebookcategory")]
        public string UnitGradebookCategory { get; set; }

        /// <summary>
        /// the type of this itme
        /// </summary>
        [DataMember(Name = "type")]
        public string Type { get; set; }

        /// <summary>
        /// the submission grade action
        /// </summary>
        [DataMember(Name = "submissiongradeaction")]
        public SubmissionGradeAction SubmissionGradeAction { get; set; }

        /// <summary>
        /// is this item manually set
        /// </summary>
        [DataMember(Name = "wasduedatemanuallyset")]
        public bool WasDueDateManuallySet { get; set; }

        /// <summary>
        /// This member is not part of the datacontract. It only exists to
        /// make it easier to navigate the tree when an item is searched for.
        /// </summary>
        public AssignmentCenterItem Parent { get; set; }

        public List<Container> Containers { get; set; }
        public List<Container> SubContainerIds { get; set;  }

        /// <summary>
        /// Set of custom fields stored in the item.
        /// </summary>
        public IDictionary<string, string> CustomFields { get; set; }

        public AssignmentCenterItem()
        {
            State = "barren";
            CustomFields = new Dictionary<string, string>();
            Containers = new List<Container>();
            SubContainerIds = new List<Container>();
        }

        /// <summary>
        /// Gets the container value for the specified toc
        /// </summary>
        /// <param name="toc">The TOC to get the container value for</param>
        /// <returns></returns>
        public string GetContainer(string toc = "syllabusfilter")
        {
            var container = this.Containers.FirstOrDefault(c => c.Toc == toc);
            if (container == null)
                return null;
            else
                return container.Value;
        }

        /// <summary>
        /// Sets the container value for the specified TOC
        /// </summary>
        /// <param name="toc">The TOC to set the container value for. If the TOC doesn't exist, it will be created</param>
        /// <param name="val">The value to set for the container</param>
        public void SetContainer(string val, string toc = "syllabusfilter")
        {
            var container = this.Containers.FirstOrDefault(c => c.Toc == toc);
            if (container != null)
            {
                container.Value = val;
            }
            else
            {
                this.Containers.Add(new Container(toc, val, "exact"));
            }
        }

        /// <summary>
        /// Gets the container value for the specified toc
        /// </summary>
        /// <param name="toc">The TOC to get the container value for</param>
        /// <returns></returns>
        public string GetSubContainer(string toc = "syllabusfilter")
        {
            var subcontainer = this.SubContainerIds.FirstOrDefault(c => c.Toc == toc);
            if (subcontainer == null)
                return null;
            else
                return subcontainer.Value;
        }

        /// <summary>
        /// Sets the container value for the specified TOC
        /// </summary>
        /// <param name="toc">The TOC to set the container value for. If the TOC doesn't exist, it will be created</param>
        /// <param name="val">The value to set for the container</param>
        public void SetSubContainer(string val, string toc = "syllabusfilter")
        {
            var subcontainer = this.SubContainerIds.FirstOrDefault(c => c.Toc == toc);
            if (subcontainer != null)
            {
                subcontainer.Value = val;
            }
            else
            {
                this.SubContainerIds.Add(new Container(toc, val, "exact"));
            }
        }
    }
}
