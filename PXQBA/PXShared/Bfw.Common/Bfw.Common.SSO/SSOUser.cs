using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.SSO
{
    /// <summary>
    /// Information about a user's central account.
    /// </summary>
    public class SSOUser
    {
        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// User's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// User's customer ID from the central system.
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// Set of user accounts the SSO user has in the Agilix System.
        /// </summary>
        public IEnumerable<AgilixAccount> AgilixUsers { get; set; }

        /// <summary>
        /// Set of user accounts the SSO user has in the Agilix System.
        /// </summary>
        public IEnumerable<OnyxInstitution> Institutions { get; set; }
    }
}
