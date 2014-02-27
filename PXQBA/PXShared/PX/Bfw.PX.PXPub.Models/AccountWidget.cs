using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents the view data for the Account widget
    /// </summary>
    public class AccountWidget
    {
        /// <summary>
        /// Account that is being displayed in the widget
        /// </summary>
        /// <value>
        /// The account.
        /// </value>
        public Account Account { get; set; }

        /// <summary>
        /// True if the account has logged in, false otherwise
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is authenticated; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// An enum to represent the current state of student view.
        /// </summary>
        public enum StudentViewStates
        {
            /// <summary>
            /// The current user is not an instructor; student view does not apply.
            /// </summary>
            NotInstructor,

            /// <summary>
            /// The current mode is instructor (normal) view.
            /// </summary>
            InstructorView,

            /// <summary>
            /// The current mode is student view.
            /// </summary>
            StudentView,

            /// <summary>
            /// The current mode implies that the link should not be shown at all (e.g., we're looking at a product course).
            /// </summary>
            HideLink
        }

        /// <summary>
        /// The current status of student view.
        /// </summary>
        public StudentViewStates StudentViewStatus { get; set; }

        /// <summary>
        /// Gets or sets the login URL.
        /// </summary>
        /// <value>
        /// The login URL.
        /// </value>
        public string LoginUrl { get; set; }

        /// <summary>
        /// Gets or sets the logout URL.
        /// </summary>
        /// <value>
        /// The logout URL.
        /// </value>
        public string LogoutUrl { get; set; }

        /// <summary>
        /// Gets or sets dashboard course id for user having dashboard as instructor for current Product course.
        /// </summary>
        public string InstructorDashboardCourseId { get; set; }

        /// <summary>
        /// Flag to see if instructor dashboard is active or not
        /// </summary>
        public bool IsInstructorDashboardActive { get; set; }

    }
}
