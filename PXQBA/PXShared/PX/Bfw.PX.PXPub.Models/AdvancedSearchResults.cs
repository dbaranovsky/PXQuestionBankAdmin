using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Models
{
    public class AdvancedSearchResults
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicSearchResults"/> class.
        /// </summary>
        public AdvancedSearchResults()
        {
            IsIndexed = true;
           
        }

        /// <summary>
        /// Gets or sets the Search Result Object.
        /// </summary>
        /// <value>
        /// The results.
        /// </value>
        public SearchResults Results { set; get; }
        /// <summary>
        /// Gets or sets the Search Querys.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public SearchQuery Query { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is indexed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is indexed; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsIndexed { set; get; }

    }
}
