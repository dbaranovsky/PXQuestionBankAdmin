using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.SSO
{
    /// <summary>
    /// Represents an insitution in the Onyx system.
    /// </summary>
    public class OnyxInstitution
    {
        /// <summary>
        /// The institution's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Insitution's ID.  This corresponds to the DLAP external ref ID.
        /// </summary>
        public string Id { get; set; }
    }
}
