using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.SSO
{
    /// <summary>
    /// Represents a Single-Sign-On instance.
    /// </summary>
    public class SSOData
    {
        /// <summary>
        /// Globally unique user ID. If null, the user is not authenticated.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The DLAP trust authentication value. Ignore if UserId is null or empty.
        /// </summary>
        public string DlapAuth { get; set; }

        /// <summary>
        /// The BrainHoney authentication value. Ignore if UserId is null or empty.
        /// </summary>
        public string BrainHoneyAuth { get; set; }

        /// <summary>
        /// Information about the user's central account and any secondary system information.
        /// </summary>
        public SSOUser User { get; set; }

        /// <summary>
        /// Value set by Novell if user session exists.
        /// </summary>
        public string AuthSession { get; set; }

        /// <summary>
        /// True if it is determined that the client should be redirected to the protected resource.
        /// </summary>
        public bool IsProtected { get; set; }

        /// <summary>
        /// Whether or not the business context should switch to a protected URL.
        /// </summary>
        public bool SwitchToProtected { get; set; }

        /// <summary>
        /// True if it is determined that the client should be redirected to the unprotected resource
        /// </summary>
        public bool SwitchToUnprotected { get; set; }
    }
}
