using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.SSO
{
    /// <summary>
    /// Information about an Agilix user linked to an SSO user and their entitlements.
    /// </summary>
    public class AgilixAccount
    {
        /// <summary>
        /// Agilix user ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// ID of the domain in which the user exists.
        /// </summary>
        public string DomainID { get; set; }

        /// <summary>
        /// Prefix of the domain.
        /// </summary>
        public string Userspace { get; set; }

        /// <summary>
        /// List of enrollments the user has.
        /// </summary>
        public IEnumerable<AgilixEnrollment> Enrollments { get; set; }
    }
}
