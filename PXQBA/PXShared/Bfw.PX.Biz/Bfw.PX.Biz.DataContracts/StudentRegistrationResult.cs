using System;
using System.Collections.Generic;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents registration data from RA authentication.
    /// </summary>
    public class StudentRegistrationResult
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the firstname.
        /// </summary>
        public string Firstname { get; set; }

        /// <summary>
        /// Gets or sets the lastname.
        /// </summary>
        public string Lastname { get; set; }

        /// <summary>
        /// Gets or sets the password hint.
        /// </summary>
        public string PasswordHint { get; set; }

        /// <summary>
        /// Gets or sets the mail preference.
        /// </summary>
        public string MailPreference { get; set; }

        /// <summary>
        /// Gets or sets the opt in email.
        /// </summary>
        public string OptInEmail { get; set; }

        /// <summary>
        /// Gets or sets the instructor email.
        /// </summary>
        public string InstructorEmail { get; set; }

        /// <summary>
        /// Gets or sets the level of access.
        /// </summary>
        public string LevelOfAccess { get; set; }
    }
}