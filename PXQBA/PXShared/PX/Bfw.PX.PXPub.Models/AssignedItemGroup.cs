using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// A grouping of assignments.
    /// </summary>
    public class AssignedItemGroup
    {
        /// <summary>
        /// Title for the group.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }
        /// <summary>
        /// Set of assignments in the group ordered by due date
        /// </summary>
        /// <value>
        /// The assignments.
        /// </value>
        public List<AssignedItem> Assignments { get; protected set; }

        /// <summary>
        /// Content to be displayed when no assignments are present in the group
        /// </summary>
        public string DefaultDisplay { get; set; }

        /// <summary>
        /// The first assignment due date to be displayed in the collapsed mode
        /// </summary>
        public string FirstDefaultDate { get; set; }

        /// <summary>
        /// The count of assignements due
        /// </summary>
        public int CountOfAssignments { get; set; }

        /// <summary>
        /// Constructs an assignment group given a set of assignments
        /// </summary>
        /// <param name="assignments">The assignments.</param>
        public AssignedItemGroup(IEnumerable<AssignedItem> assignments)
        {
            if (!assignments.IsNullOrEmpty())
            {
                Assignments = assignments.OrderBy(a => a.DueDate).ToList();
            }
        }

        public AssignedItemGroup()
        {

        }
    }
}
