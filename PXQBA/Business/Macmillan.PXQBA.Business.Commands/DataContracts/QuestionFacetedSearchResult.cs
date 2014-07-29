namespace Macmillan.PXQBA.Business.Commands.DataContracts
{
    /// <summary>
    /// Represents the entity that is returned by SOLR faceted search
    /// </summary>
    public class QuestionFacetedSearchResult
    {
        /// <summary>
        /// Field name that is used for faceted search
        /// </summary>
        public string FacetedFieldValue { get; set; }

        /// <summary>
        /// Number of questions per facet
        /// </summary>
        public int FacetedCount { get; set; }
    }
}