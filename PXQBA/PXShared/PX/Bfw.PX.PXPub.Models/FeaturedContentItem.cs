using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{

    /// <summary>
    /// Contains data necessary to display a featured content item
    /// </summary>
    public class FeaturedContentItem
    {
        /// <summary>
        /// The item's title
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// The item's ID
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Link to the item's image
        /// </summary>
        /// <value>
        /// The image URL.
        /// </value>
        public Uri ImageUrl { get; set; }
    }
}
