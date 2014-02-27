using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class Paging
    {
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        public string StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the last index.
        /// </summary>
        /// <value>
        /// The last index.
        /// </value>
        public string LastIndex { get; set; }
    }
}
