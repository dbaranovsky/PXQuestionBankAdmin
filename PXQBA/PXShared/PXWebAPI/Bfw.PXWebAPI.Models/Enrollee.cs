using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    public class Enrollee
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user reference id.
        /// </summary>
        /// <value>
        /// The user reference id.
        /// </value>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the last login.
        /// </summary>
        /// <value>
        /// The last login.
        /// </value>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Gets or sets the course complete percentage.
        /// </summary>
        /// <value>
        /// The course complete percentage.
        /// </value>
        public int CourseCompletePercentage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is instructor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is instructor; otherwise, <c>false</c>.
        /// </value>
        public string UserRole { get; set; }
    }
}
