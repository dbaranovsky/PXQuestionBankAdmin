using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    public class TableofContentsItem : ContentItem
    {
        /// <summary>
        /// Any children of the item.
        /// </summary>
        public List<TableofContentsItem> Children { get; set; }

        public TableofContentsItem()
        {
            Children = new List<TableofContentsItem>();
        }
    }
}
