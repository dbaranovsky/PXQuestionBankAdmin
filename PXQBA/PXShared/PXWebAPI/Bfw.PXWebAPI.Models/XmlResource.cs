using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Represents a binary resource of some type.
    /// Wrapped in an XML construct including metadata about the content stored.
    /// </summary>
    public class XmlResource : Resource
    {
        /// <summary>
        /// Stores the HTML of the content item.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the word count.
        /// </summary>
        public int WordCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlResource"/> class.
        /// </summary>
        public XmlResource()
        {
            ContentType = "xml-res";
            Extension = "pxres";
        }
    }
}
