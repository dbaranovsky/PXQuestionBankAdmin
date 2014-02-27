using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a widget with a navigable view of content as well as 
    /// a pane for displaying a selected piece of content
    /// </summary>
    public class ContentWidget
    {
        /// <summary>
        /// The item to be displayed in the view pane, null if there is no content to display
        /// </summary>
        /// <value>
        /// The content item.
        /// </value>
        public ContentItem ContentItem { get; set; }

        /// <summary>
        /// True if on a product course
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is product course; otherwise, <c>false</c>.
        /// </value>
        public bool IsProductCourse { get; set; }

        /// <summary>
        /// The item to be displayed in the view pane, null if there is no content to display
        /// </summary>
        /// <value>
        /// The content items.
        /// </value>
        public List<ContentItem> ContentItems { get; set; }

        /// <summary>
        /// Available Toc Categories to filter by
        /// </summary>
        /// <value>
        /// The categories.
        /// </value>
        public IList<TocCategory> Categories { get; set; }

        /// <summary>
        /// Represents the table of contents in the navigation pane
        /// </summary>
        /// <value>
        /// The table of contents.
        /// </value>
        public IEnumerable<TocItem> TableOfContents { get; set; }

        /// <summary>
        /// True if this model is being used as an ebbok browser, false otherwise.
        /// </summary>
        public bool IsEbookBrowser { get; set; }

        /// <summary>
        /// Ebook Items
        /// </summary>
        public List<Ebook> EbookItems { get; set; }



    }
}
