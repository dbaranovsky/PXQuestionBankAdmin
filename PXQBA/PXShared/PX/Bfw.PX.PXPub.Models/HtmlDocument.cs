using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// An Html document authored by a user
    /// </summary>
    public class HtmlDocument : ContentItem
    {
        /// <summary>
        /// Stores the HTML of the content item
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        [System.ComponentModel.DisplayName("Body")]
        public string Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlDocument"/> class.
        /// </summary>
        public HtmlDocument()
        {
            Type = "HtmlDocument";
            AllowComments = true;
        }
        
    }
}
