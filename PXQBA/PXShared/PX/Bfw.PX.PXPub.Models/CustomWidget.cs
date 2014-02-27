using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all data necessary to render the RSSFeedWidget
    /// </summary>
    public class CustomWidget : ContentItem
    {

        /// <summary>
        /// Contents
        /// </summary>
        /// <value>
        /// The Contents.
        /// </value>
        public string WidgetContents { get; set; }
    }
}
