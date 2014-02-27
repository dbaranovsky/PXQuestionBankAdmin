using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents data about a user's account
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Name displayed accross the site for this user
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [Required(ErrorMessage="Username is required")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the RA_Email.
        /// </summary>
        /// <value>
        /// The RA_Email.
        /// </value>
        [DisplayName("User Name")]
        public string RA_Email { get; set; }

        /// <summary>
        /// Gets or sets the RA_password.
        /// </summary>
        /// <value>
        /// The RA_password.
        /// </value>
        [DisplayName("Password")]
        public string RA_Password { get; set; }
    }
}
