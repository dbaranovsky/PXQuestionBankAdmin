using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Runtime.Serialization;

using Bfw.Agilix.Dlap;
using Bfw.Common.Collections;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters for searching for items in DLAP.
    /// </summary>
    [DataContract]
    public class ItemSearch
    {
        /// <summary>
        /// Id of the item to get. Set to empty or null if you want all items in the entity.
        /// </summary>
        [DataMember]
        public string ItemId { get; set; }

        /// <summary>
        /// Id of the entity for the item(s) you are interested in.
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// Do not search student item
        /// </summary>
        [DataMember]
        public bool ExcludeStudentItem { get; set; }

        /// <summary>
        /// Depth of child items to return. Defaults to zero.
        /// </summary>
        [DataMember]
        public int Depth { get; set; }

        /// <summary>
        /// XPath syntax based query where / is assumed to be the data element of the
        /// item.
        /// </summary>
        [DataMember]
        public string Query { get; set; }

        /// <summary>
        /// Gets the list of item and its descesndents based on the item id
        /// </summary>
        [DataMember]
        public string AncestorId { get; set; }

        /// <summary>
        /// Determines whether to apply resource-based overrides for students when looking for items
        /// Used in Eportfolio
        /// </summary>
        [DataMember] 
        public static bool ApplyStudentItems =
            !ConfigurationManager.AppSettings["ApplyStudentItemsEnabled"].IsNullOrEmpty()
            && Boolean.Parse(ConfigurationManager.AppSettings["ApplyStudentItemsEnabled"]);

        /// <summary>
        /// If set to a value other than "none" the results will only contain items
        /// of the given type. Note that if this value is set along with Depth, then 
        /// a linear list of results will be returned, i.e. no hierarchy information
        /// will be maintained.
        /// </summary>
        [DataMember]
        public DlapItemType Type { get; set; }
    }
}
