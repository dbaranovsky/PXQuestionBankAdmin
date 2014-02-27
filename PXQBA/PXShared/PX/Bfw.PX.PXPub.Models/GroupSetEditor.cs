using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class GroupSetEditor
    {
        /// <summary>
        /// Gets or sets the enrollment id.
        /// </summary>
        /// <value>
        /// The enrollment id.
        /// </value>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Gets or sets the type of the group set edit.
        /// </summary>
        /// <value>
        /// The type of the group set edit.
        /// </value>
        public EditType GroupSetEditType { get; set; }

        /// <summary>
        /// Gets or sets the group set id.
        /// </summary>
        /// <value>
        /// The group set id.
        /// </value>
        public int GroupSetId { get; set; }

        /// <summary>
        /// Enum for Edit Type
        /// </summary>
        public enum EditType
        {
            Create,
            Edit,
            Clone
        }
    }
}
