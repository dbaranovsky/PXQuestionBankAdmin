using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Collection of content items that are related to another content item, grouped by category.  Right now
    /// specific to xbook.
    /// </summary>
    public class RelatedItems 
    {
        public string Category { get; set; }
        public string PreviewHash { get; set; }
        public IList<RelatedItem> Items { get; set; }
    }

    /// <summary>
    /// Class to wrap a content item with any properties that may be associated with a related content item
    /// </summary>
    public class RelatedItem
    {
        /// <summary>
        /// The content item 
        /// </summary>
        public ContentItem Item { get; set; }
        /// <summary>
        /// The hash to be used to get a preview of the content item
        /// </summary>
        public string PreviewHash { get; set; }

        /// <summary>
        /// Creates a new related item
        /// </summary>
        /// <param name="previewhash">The hash to be used to get a preview of the content item</param>
        public RelatedItem(ContentItem item, string previewhash)
        {
            Item = item;
            PreviewHash = previewhash;
        }
    }
}
