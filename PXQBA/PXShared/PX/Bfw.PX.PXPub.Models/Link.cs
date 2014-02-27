using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a link to an offsite resource
    /// </summary>
    public class Link : ContentItem
    {
        public Link()
        {
            Type = "Link";
            Hidden = true;
        }

        /// <summary>
        /// Gets or sets the type of the extended link.
        /// </summary>
        /// <value>
        /// The type of the extended link.
        /// </value>
        public string ExtendedLinkType { get; set; }
    }
}
