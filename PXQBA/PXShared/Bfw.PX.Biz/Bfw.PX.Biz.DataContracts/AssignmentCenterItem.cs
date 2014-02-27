using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common;

namespace Bfw.PX.Biz.DataContracts
{
    [Serializable]
    public class AssignmentCenterItem
    {
        /// <summary>
        /// Unique Id of the node.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Unique Id of the node's parent. Null or empty if the node has no parent.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Unique Id of the node's previous parent. Null or empty if the node has no parent.
        /// </summary>
        public string PreviousParentId { get; set; }

        /// <summary>
        /// Date the assignment is supposed to start, if any.
        /// </summary>
        public DateTime StartDate { get; set; }

        public DateTimeWithZone StartDateTZ { get; set; }

        /// <summary>
        /// Date the assignment is supposed to end, if any.
        /// </summary>
        public DateTime EndDate { get; set; }

        public DateTimeWithZone EndDateTZ { get; set; }

        /// <summary>
        /// Points possible for the node item, if any.
        /// </summary>
        public double Points { get; set; }

        /// <summary>
        /// Default points possible for item, if any.
        /// </summary>
        public double DefaultPoints { get; set; }

        /// <summary>
        /// Relative order of this item under its parent.
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Order in which this item should be displayed relative to other items within assigned category.
        /// </summary>
        public string CategorySequence { get; set; }

        /// <summary>
        /// Gradebook Category
        /// </summary>
        public string GradebookCategory { get; set; }

        /// <summary>
        /// Unit Gradebook Category
        /// </summary>
        public string UnitGradebookCategory { get; set; }

        /// <summary>
        /// the submission grade action
        /// </summary>
        public SubmissionGradeAction SubmissionGradeAction { get; set; }

        /// <summary>
        /// is this item manually set
        /// </summary>
        public bool WasDueDateManuallySet { get; set; }

        /// <summary>
        /// True if the item is assigned (i.e. EndDate != DateTime.MinValue)
        /// </summary>
        public bool IsAssigned
        {
            get
            {
                return EndDate.ToShortDateString() != DateTime.MinValue.ToShortDateString();
            }
        }

        public List<Container> Containers { get; set; }
        public List<Container> SubContainerIds { get; set; }

        /// <summary>
        /// Set of custom fields stored in the item.
        /// </summary>
        public IDictionary<string, string> CustomFields { get; set; }

        public AssignmentCenterItem()
        {
            Containers = new List<Container>();
            SubContainerIds = new List<Container>();
        }
    }
}
