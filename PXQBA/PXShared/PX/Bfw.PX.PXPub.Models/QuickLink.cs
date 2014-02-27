using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a quick link.
    /// </summary>
    public class QuickLink : ContentItem
    {
        /// <summary>
        /// A private variable that only the Type
        /// </summary>
        private const string TYPE = "QuickLink";

        /// <summary>
        /// Initializes a new instance of the <see cref="QuickLink"/> class.
        /// </summary>
        public QuickLink()
        {
            Type = TYPE;
        }

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

        /// <summary>
        /// Gets or sets the toc item.
        /// </summary>
        /// <value>
        /// The toc item.
        /// </value>
        public IEnumerable<TocItem> TocItem { get; set; }
    }
}
