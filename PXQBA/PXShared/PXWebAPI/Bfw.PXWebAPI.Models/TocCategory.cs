using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Defines a TocCategory that is available.
    /// </summary>
    public class TocCategory
    {
        /// <summary>
        /// Unique identifier for the category.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Human friendly text that represents the category.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// This is the item ID of the parent of the item this object is attached to.
        /// </summary>
        public string ItemParentId { get; set; }

        /// <summary>
        /// Relative sequence of the item when viewed in this category.
        /// </summary>
        public string Sequence { get; set; }

        /// <summary>
        /// Type of catagory
        /// </summary>
        public string Type { get; set; }
    }
}
