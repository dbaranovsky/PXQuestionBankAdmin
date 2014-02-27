using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a content item that could not be found
    /// </summary>
    public class Content404 : ContentItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Content404"/> class.
        /// </summary>
        public Content404()
        {
            Type = "Content404";
        }
    }
}
