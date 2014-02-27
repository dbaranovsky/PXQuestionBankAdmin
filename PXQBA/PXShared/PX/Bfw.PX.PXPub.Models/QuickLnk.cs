using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a simple quick link.
    /// </summary>
    public class QuickLnk
    {
        /// <summary>
        /// A private variable that only the Type
        /// </summary>
        private const string TYPE = "QuickLink";

        /// <summary>
        /// Gets or sets the Title of the quick link
        /// </summary>
        public string LinkTitle { get; set; }

        /// <summary>
        /// Gets or sets the linked item id.
        /// </summary>
        /// <value>
        /// The linked item id.
        /// </value>
        public string LinkedItemId { get; set; }

        /// <summary>
        /// Gets or sets the link URL.
        /// </summary>
        /// <value>
        /// The link URL.
        /// </value>
        public string LinkUrl { get; set; }

    }
}
