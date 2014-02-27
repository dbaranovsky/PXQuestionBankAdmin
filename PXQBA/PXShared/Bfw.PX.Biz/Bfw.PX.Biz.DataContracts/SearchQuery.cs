using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Used to identify fields to include in search.
    /// </summary>
    public enum SearchIncludeType
    {
        All = 0,
        Title,
        Content
    }

    /// <summary>
    /// Used to build up and modify a search, specifying terms to include, exclude and other search criteria.
    /// (See http://gls.agilix.com/Docs/Command/Search)
    /// </summary>
    public class SearchQuery
    {
        /// <summary>
        /// Search for a specific term or phrase.
        /// </summary>
        public string ExactPhrase { get; set; }

        /// <summary>
        /// Include a specific term or phrase.
        /// </summary>
        public string IncludeWords { get; set; }

        /// <summary>
        /// Exclude a specific term or phrase.
        /// </summary>
        public string ExcludeWords { get; set; }

        /// <summary>
        /// Limit search to specific content types.
        /// </summary>
        public string ContentTypes { get; set; }

        /// <summary>
        /// Limit search to specific content types.
        /// </summary>
        public bool IncludeAssigned { get; set; } 

        /// <summary>
        /// Gets or sets the meta categories.
        /// </summary>
        public IEnumerable<string> MetaCategories { get; set; }

        /// <summary>
        /// The index of the first hit to return. 
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// A number between 1 and 25 that is the maximum number of rows to return from the search.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Fields to include in the search.
        /// </summary>
        public string IncludeFields { get; set; }



        /// <summary>
        /// Meta Fields to include in the search (used for facet searching, etc)
        /// </summary>
        public string MetaIncludeFields { get; set; }

        /// <summary>
        /// Class type to search for.
        /// </summary>
        public string ClassType { get; set; }



        /// <summary>
        /// Enable Faceted search
        /// </summary>
        [DataMember]
        public bool IsFaceted { get; set; }

        /// <summary>
        /// Parameters for Faceted Search
        /// </summary>
        public FacetedSearchQuery FacetedQuery { get; set; }

        /// <summary>
        /// Gets or sets the exact query for metadata search.
        /// </summary>
        /// <value>
        /// The exact query.
        /// </value>
        public string ExactQuery { get; set; }

        /// <summary>
        /// Entity id - overrides current context entity id (used to search against product course)
        /// </summary>
        public string EntityId { get; set; }
    }

    public class FacetedSearchQuery
    {
        /// <summary>
        /// List of metadata fields to search
        /// </summary>
        public List<string> Fields { get; set; }

        /// <summary>
        /// Limits number of results per field
        /// Default: no limit
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Indicates the minimum count for facet fields to be included in the response.
        /// Default: 0
        /// </summary>
        public int MinCount { get; set; }
        public FacetedSearchQuery()
        {
            this.Fields = new List<string>();
        }
    }
}
