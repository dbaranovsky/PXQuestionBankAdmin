using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class NavigationItem : ContentItem
    {
        /// <summary>
        /// Whether this item should be highlighted or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if highlighted; otherwise, <c>false</c>.
        /// </value>
        public bool Highlighted { get; set; }

        /// <summary>
        /// Any subelements of this menu item
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<NavigationItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public List<ContentItem> Children { get; set; }

        /// <summary>
        /// Gets or sets the toc id.
        /// </summary>
        /// <value>
        /// The toc id.
        /// </value>
        public string TocId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is top level.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is top level; otherwise, <c>false</c>.
        /// </value>
        public bool IsTopLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is fn E.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is fn E; otherwise, <c>false</c>.
        /// </value>
        public bool IsFnE { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is display title.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is display title; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisplayTitle { get; set; }

        /// <summary>
        /// Gets or sets the display title.
        /// </summary>
        /// <value>
        /// The display title.
        /// </value>
        public string DisplayTitle { get; set; }

        /// <summary>
        /// Gets or sets the title URL.
        /// </summary>
        /// <value>
        /// The title URL.
        /// </value>
        public string TitleURL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is support add link.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is support add link; otherwise, <c>false</c>.
        /// </value>
        public bool IsSupportAddLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is support add menu.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is support add menu; otherwise, <c>false</c>.
        /// </value>
        public bool IsSupportAddMenu { get; set; }

        /// <summary>
        /// Gets or sets the type of the extended link.
        /// </summary>
        /// <value>
        /// The type of the extended link.
        /// </value>
        public string ExtendedLinkType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class.
        /// </summary>
        public NavigationItem()
        {
            Type = "Folder";
            Items = new List<NavigationItem>();
            Children = new List<ContentItem>();
            IsTopLevel = false;
            IsActive = true;
            IsDisplayTitle = false;
        }
    }
}
