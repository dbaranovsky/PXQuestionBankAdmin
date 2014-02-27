using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represent the root of a ContentTreeWidget
    /// </summary>
    public class TreeWidgetRoot
    {
        /// <summary>
        /// The top level items contained within the tree widget (usually PxUnit)
        /// </summary>
        public List<TreeWidgetViewItem> Items { get; set; }

        /// <summary>
        /// Setting/Configuration options associated with the TreeWidget
        /// </summary>
        public TreeWidgetSettings Settings { get; set; }

        public TreeWidgetRoot()
        {
            Items = new List<TreeWidgetViewItem>();
        }
    }
}
