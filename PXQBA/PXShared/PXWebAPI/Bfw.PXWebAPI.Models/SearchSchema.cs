using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Defines the search categories that are supported by the course.
    /// </summary>
    public class SearchSchema
    {
        /// <summary>
        /// Collection of search categories supported.
        /// </summary>
        public IEnumerable<SearchCategory> Categories { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchSchema"/> class.
        /// </summary>
        public SearchSchema()
        {
            Categories = new List<SearchCategory>();
        }
    }

    /// <summary>
    /// Representation of a search category in the system.
    /// </summary>
    public class SearchCategory
    {
        /// <summary>
        /// Title of the category.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tag/keyword associated with the category. Content tagged with this tag value will be grouped.
        /// </summary>
        public string MetaTag { get; set; }

        /// <summary>
        /// List of associated item types.
        /// </summary>
        public List<string> AssociatedItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this category is searchable and displayed during search.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this category is searchable; otherwise, <c>false</c>.
        /// </value>
        public Boolean IsSearchable { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCategory"/> class.
        /// </summary>
        public SearchCategory()
        {
            Title = "";
            MetaTag = "";
            AssociatedItems = new List<string>();
            IsSearchable = true;
        }
    }
}
