using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a relationship between an item and a resource.
    /// </summary>
    public class ResourceMap
    {
        /// <summary>
        /// ID of the associated item.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Unique ID of the resource-map relation.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Collection of associated item IDs.
        /// </summary>
        public List<string> AssociatedItems { get; set; }

        /// <summary>
        /// Resource map type.
        /// </summary>
        public string MapType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceMap"/> class.
        /// </summary>
        public ResourceMap()
        {
            Id = "";
            ItemId = "";
            AssociatedItems = new List<string>();
            MapType = "";
            Description = "";
        }
    }
}
