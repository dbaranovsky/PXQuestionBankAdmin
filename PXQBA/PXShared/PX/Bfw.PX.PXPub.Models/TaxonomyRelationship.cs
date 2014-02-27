using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Tracks all information about a relationship between two items and the taxonomy that joins them
    /// </summary>
    public class TaxonomyRelationship
    {
        #region Data Members

        /// <summary>
        /// Id of the item pointed to by the taxonomy (Agilix Item Id). If one where to
        /// get a list of all items related to ItemA by taxonomy, then ItemId would be the Id of the item in
        /// the result set, i.e. NOT ItemA.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// Title of the item pointed to by the taxonomy
        /// </summary>
        /// <value>
        /// The item title.
        /// </value>
        public string ItemTitle { get; set; }

        /// <summary>
        /// Id of the item related to ItemId by the taxonomy (Agilix Item Id). If one where to
        /// get a list of all items related to ItemA by taxonomy, then RelatedItemId is the Id of ItemA.
        /// </summary>
        /// <value>
        /// The related item id.
        /// </value>
        public string RelatedItemId { get; set; }

        /// <summary>
        /// Id of the taxonomy that joins ItemId and RelatedItemId (matches id of the taxonomy in the external system, typically
        /// an id from sitebuilder)
        /// </summary>
        /// <value>
        /// The taxonomy id.
        /// </value>
        public string TaxonomyId { get; set; }

        /// <summary>
        /// Id of the scope in which the taxonomy relationship exists (matches id of scope in external system, typically the
        /// agilix entity id)
        /// </summary>
        /// <value>
        /// The scope id.
        /// </value>
        public string ScopeId { get; set; }

        /// <summary>
        /// Title of the taxonomy, e.g. "Early European History"
        /// </summary>
        /// <value>
        /// The taxonomy title.
        /// </value>
        public string TaxonomyTitle { get; set; }

        #endregion
    }
}
