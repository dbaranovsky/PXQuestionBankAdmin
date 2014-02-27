using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Defined contract for operating on assignments
    /// </summary>
    public interface IAssignmentActions
    {
        /// <summary>
        /// Marks an item as assigned and sets all appropriate assignment settings 
        /// such as due date and max points.
        /// </summary>
        /// <param name="assignItem">The assign item.</param>
        void Assign(AssignedItem assignItem);

        /// <summary>
        /// Makes an item not gradable and also sets all of its assignment
        /// properties back to their defaults.
        /// </summary>
        /// <param name="assignedItem"></param>
        void Unassign(ContentItem assignedItem);

        /// <summary>
        /// Marks or unmarks an assignment as important
        /// </summary>
        /// <param name="assignmentIDs"></param>
        /// <param name="isImportant"></param>
        void AssignmentImportant(List<string> assignmentIDs, bool isImportant);
    }
}
