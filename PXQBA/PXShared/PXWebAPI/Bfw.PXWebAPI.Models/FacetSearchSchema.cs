using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Defines the faceted search categories that are supported by the course.
    /// </summary>
    public class FacetSearchSchema
    {
        /// <summary>
        /// Collection of search categories supported.
        /// </summary>
        public IEnumerable<FacetSearchCategory> Categories { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacetSearchSchema"/> class.
        /// </summary>
        public FacetSearchSchema()
        {
            Categories = new List<FacetSearchCategory>();
        }
    }

    /// <summary>
    /// Representation of a search category in the system.
    /// </summary>
    public class FacetSearchCategory
    {
        /// <summary>
        /// Title of the category.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Meta-tagd associated with the category. Content tagged with this tag value will be grouped.
        /// </summary>
        public string MetaDataKey { get; set; }

        /// <summary>
        /// Default facet to use when browsing by facet
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FacetSearchCategory"/> class.
        /// </summary>
        public FacetSearchCategory()
        {
            Title = "";
            MetaDataKey = "";
        }
    }
}
