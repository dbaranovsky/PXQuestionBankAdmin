using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents data about a user's RA account
    /// </summary>
    public class RALogin
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RALogin"/> class.
        /// </summary>
        public RALogin()
        {
        }

        /// <summary>
        /// Gets or sets the R a_ email.
        /// </summary>
        /// <value>
        /// The R a_ email.
        /// </value>
        [DisplayName("User Name")]
        public string RA_Email { get; set; }

        /// <summary>
        /// Gets or sets the R a_ password.
        /// </summary>
        /// <value>
        /// The R a_ password.
        /// </value>
        [DisplayName("Password")]
        public string RA_Password { get; set; }

        /// <summary>
        /// Gets or sets the type of the R a_ user.
        /// </summary>
        /// <value>
        /// The type of the R a_ user.
        /// </value>
        public string RA_UserType { get; set; }

        /// <summary>
        /// Gets or sets the course id.
        /// </summary>
        /// <value>
        /// The course id.
        /// </value>
        public string CourseId { get; set; }
    }
}
