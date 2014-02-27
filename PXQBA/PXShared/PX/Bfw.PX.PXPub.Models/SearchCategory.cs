using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Bfw.PX.PXPub.Models
{
    [Serializable]
    public class SearchCategory
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the associated items.
        /// </summary>
        /// <value>
        /// The associated items.
        /// </value>
        public string AssociatedItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is searchable.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is searchable; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsSearchable { get; set; }
    }
}
