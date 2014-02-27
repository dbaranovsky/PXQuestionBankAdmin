using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// A ContentItem representing a folder
    /// </summary>
    public class Folder : ContentItem
    {
        /// <summary>
        /// The folder's child child folders.  This may or may not be populated depending on the intended usage.
        /// </summary>
        /// <value>
        /// The folders.
        /// </value>
        public IList<TocItem> Folders { get; set; }

        /// <summary>
        /// The folder's child child content items.  This may or may not be populated depending on the intended usage.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public IList<TocItem> Items { get; set; }

        /// <summary>
        /// The folder's theme. Null if there is no theme selected.
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// The folder's display image. Null if there is no image selected.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Folder"/> class.
        /// </summary>
        public Folder()
        {
            Type = "Folder";
            TrackMinutesSpent = false;
        }
    }
}
