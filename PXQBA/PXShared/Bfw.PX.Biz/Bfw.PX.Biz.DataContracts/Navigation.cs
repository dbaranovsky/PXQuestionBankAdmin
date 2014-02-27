using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a top-level navigational construct.
    /// </summary>
    [DataContract]
    public class Navigation
    {
        /// <summary>
        /// Name of the navigation element.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// Any items that are directly under the root of the hierarchical structure.
        /// </summary>
        [DataMember]
        public List<NavigationItem> Children { get; set; }
    }
}
