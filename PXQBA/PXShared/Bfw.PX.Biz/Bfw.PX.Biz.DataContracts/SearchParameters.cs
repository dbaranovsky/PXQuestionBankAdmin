using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Entity that stores search parameters.
    /// (See http://gls.agilix.com/Docs/Command/Search).
    /// </summary>
    [DataContract]
    public class SearchParameters
    {
        /// <summary>
        /// A vertical-bar separated list of entity IDs by which to filter search results. 
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// The query terms to search for. In its simplest form, query is a space-separated list of terms such as hello dolly. 
        /// </summary>
        [DataMember]
        public string Query { get; set; }

        /// <summary>
        /// An optional, vertical-bar separated list of field names to return from the query.
        /// </summary>
        [DataMember]
        public string Fields { get; set; }

        /// <summary>
        /// Specify true to retrieve highlighted hit terms in the results. The default is false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hl; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Hl { get; set; }

        /// <summary>
        /// When hl is true, contains the vertical bar-separated list of fields in which to highlight hit terms.
        /// </summary>
        [DataMember]
        public string HlFields { get; set; }

        /// <summary>
        /// Enable Faceted search
        /// </summary>
        [DataMember]
        public bool Facet { get; set; }

        /// <summary>
        /// When facet is true, contains the vertical bar-separated list of fields to facet. 
        /// Search iterates over each indexed term for each listed field and generates a 
        /// facet count using that term as the constraint. 
        /// This parameter is ignored if facet is false.
        /// </summary>
        [DataMember]
        public string FacetFields { get; set; }

        /// <summary>
        /// Maximum number of constraint counts to be returned for the facet fields
        /// A negative value means unlimited. Default is 100.
        /// </summary>
        [DataMember]
        public FacetParam<int> FacetLimit { get; set; }

        /// <summary>
        /// Inicidates what algorithm to use when faceting a field 
        /// Possible values: fc, enum
        /// Default: fc
        /// </summary>
        [DataMember]
        public FacetParam<string> FacetMethod { get; set; }

        /// <summary>
        /// Indicates the minimum count for facet fields to be included in the response.
        /// Default is 0.
        /// </summary>
        public FacetParam<int> FacetMinCount { get; set; }

        /// <summary>
        /// When true, in addition to term-based constraints of a facet field, 
        /// a count of all matching results that have no value for the field should also be computed.
        /// Default is false.
        /// </summary>
        public FacetParam<bool> FacetMissing { get; set; }

        /// <summary>
        /// Indicates an offset into the list of constraints to allow paging. The default is 0. 
        /// </summary>
        public FacetParam<int> FacetOffset { get; set; }


        /// <summary>
        /// Determines the ordering of the facet field constraints.
        ///     count - sort the constraints by count (highest count first)
        ///     index - sort constraints by index order. For terms in the ascii range, this is alphabetical.
        /// The default is count if facet.limit is greater than 0, index otherwise.
        /// </summary>
        public FacetParam<string> FacetSort { get; set; }

        /// <summary>
        /// Maximum number of result rows to return (1 - 25), defaults to 10.
        /// </summary>
        /// <summary>
        /// A number between 1 and 25 that is the maximum number of rows to return from the search. The default is 10.
        /// </summary>
        [DataMember]
        public int Rows { get; set; }

        /// <summary>
        /// The index of the first hit to return. The default is 0.
        /// </summary>
        [DataMember]
        public int Start { get; set; }

    }


    public class FacetParam<T>
    {
        /// <summary>
        /// (Optional) Field name to apply parameter to in the form: f.[fieldname].facet.[METHOD]
        /// </summary>
        public string FieldNamePrefix { get; set; }

        /// <summary>
        /// Value of parameter
        /// </summary>
        public T Value { get; set; }
        /// <summary>
        /// Default Constructor
        /// </summary>
        public FacetParam()
        {
            FieldNamePrefix = "";
        }
    }
}