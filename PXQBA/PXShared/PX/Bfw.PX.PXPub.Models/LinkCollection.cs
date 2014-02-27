using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a collection of links to offsite resources
    /// </summary>
    public class LinkCollection : ContentItem
    {
        /// <summary>
        /// List of Links in the collection
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        
        public IList<Link> Links { get; set; }

        /// <summary>
        /// Gets or sets the link URL.
        /// </summary>
        /// <value>
        /// The link URL.
        /// </value>
        [RegularExpression(@"(http|https|ftp)://([a-zA-Z0-9\\~\\!\\@\\ \\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?", ErrorMessage = "The specified URL is not valid .")]
        public string linkUrl { get; set; }

        /// <summary>
        /// Initializes an empty description and link list
        /// </summary>
        public LinkCollection()
        {
            Type = "LinkCollection";
            Links = new List<Link>();
        }
    }
}
