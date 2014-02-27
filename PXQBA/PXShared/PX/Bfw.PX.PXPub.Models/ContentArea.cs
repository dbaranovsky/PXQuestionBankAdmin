using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Holds information necessary for displaying content in the Px Content Area Widget
    /// </summary>
    public class ContentArea
    {
        public ContentItem Content { get; set; }
        public XbookContentOptions HeaderOptions { get; set; }

        public ContentArea()
        {
            
        }

        public ContentArea(ContentItem item, XbookContentOptions options)
        {
            Content = item;
            HeaderOptions = options;
        }
    }
}
