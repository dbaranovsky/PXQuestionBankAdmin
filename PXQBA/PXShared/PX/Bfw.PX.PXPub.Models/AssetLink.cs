using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents of a link to offsite resources
    /// </summary>
    public class AssetLink : ContentItem
    {
        /// <summary>
        /// Flag to show url in Activity Player or Document Viewer
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show activity player]; otherwise, <c>false</c>.
        /// </value>
        public Boolean showActivityPlayer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetLink"/> class.
        /// </summary>
        public AssetLink()
        {
            Type = "AssetLink";
            showActivityPlayer = false;
            AllowComments = true;
        }
    }
}
