using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// As defined by http://dev.dlap.bfwpub.com/Docs/Command/Search.
    /// </summary>
    [DataContract]
    public class SolrSearchParameters
    {
        /// <summary>
        /// Id of the entity to restrict resutls to.
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// SOLR query to execute.
        /// </summary>
        [DataMember]
        public string Query { get; set; }

        /// <summary>
        /// Fields to return in resutls.
        /// </summary>
        [DataMember]
        public string Fields { get; set; }

        /// <summary>
        /// True if highlighed hits should be retured.
        /// </summary>
        [DataMember]
        public bool Hl { get; set; }

        /// <summary>
        /// Fields that should be highlighted if <see cref="Hl" /> is true.
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
        [DataMember]
        public int Rows { get; set; }

        /// <summary>
        /// Start index of the first hit item. Default is 0.
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
