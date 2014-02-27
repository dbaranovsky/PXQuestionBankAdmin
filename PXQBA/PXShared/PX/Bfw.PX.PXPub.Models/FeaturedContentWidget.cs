using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Model for the FeaturedContentWidget
    /// </summary>
    public class FeaturedContentWidget
    {
        /// <summary>
        /// The list of FeaturedContentItems this widget is to display
        /// </summary>
        /// <value>
        /// The featured content items.
        /// </value>
        public IEnumerable<FeaturedContentItem> FeaturedContentItems { get; set; }
    }
}
