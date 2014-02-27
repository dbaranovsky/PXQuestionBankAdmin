using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Data necessary to display a breadcrumb trail.
    /// </summary>
    public class Trail
    {
        /// <summary>
        /// The id of the root item of the trail (e.g. PX_TOC)
        /// </summary>
        /// <value>
        /// The root item.
        /// </value>
        public string RootItem { get; set; }

        /// <summary>
        /// This list of levels the trail represents
        /// </summary>
        /// <value>
        /// The levels.
        /// </value>
        public IList<Level> Levels { get; set; }

        /// <summary>
        /// Data necessary to display one level of a breadcrumb trail, a list of sibling
        /// items, along with which one is selected
        /// </summary>
        public class Level
        {
            /// <summary>
            /// Gets or sets the items.
            /// </summary>
            /// <value>
            /// The items.
            /// </value>
            public IList<Breadcrumb> Items { get; set; }
            /// <summary>
            /// Gets or sets the selected.
            /// </summary>
            /// <value>
            /// The selected.
            /// </value>
            public int Selected { get; set; }
        }

        /// <summary>
        /// Data for one 'thing' in the breadcrumb trail.
        /// </summary>
        public class Breadcrumb
        {
            /// <summary>
            /// Display Text.
            /// </summary>
            public string Display;
            /// <summary>
            /// Id.
            /// </summary>
            public string Id;
            /// <summary>
            /// Typess.
            /// </summary>
            public string Type;
        }
    }
}
