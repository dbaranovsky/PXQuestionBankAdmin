using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class VerticalMenu
    {
        /// <summary>
        /// Gets or sets the nav items.
        /// </summary>
        /// <value>
        /// The nav items.
        /// </value>
        public System.Collections.Generic.IList<Bfw.PX.PXPub.Models.NavigationItem> NavItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is instructor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is instructor; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstructor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is anonymous.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is anonymous; otherwise, <c>false</c>.
        /// </value>
        public bool IsAnonymous { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is student.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is student; otherwise, <c>false</c>.
        /// </value>
        public bool IsStudent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is product course.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is product course; otherwise, <c>false</c>.
        /// </value>
        public bool IsProductCourse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show assignment link].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show assignment link]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowAssignmentLink { get; set; }
    }
}
