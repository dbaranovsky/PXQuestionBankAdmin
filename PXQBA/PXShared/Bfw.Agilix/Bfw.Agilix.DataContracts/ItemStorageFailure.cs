using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents an Item that failed to be stored in Agilix and why the failure occured.
    /// </summary>
    [DataContract]
    public class ItemStorageFailure
    {
        /// <summary>
        /// Id of the item that failed to be stored.
        /// </summary>
        [DataMember]
        public string ItemId { get; set; }

        /// <summary>
        /// Id of the entity the item was being stored in.
        /// </summary>
        [DataMember]
        public string EntityId { get; set; }

        /// <summary>
        /// Any message or reason that Agilix couldn't store the item.
        /// </summary>
        [DataMember]
        public string Reason { get; set; }

        /// <summary>
        /// Item that failed. Note that this is NOT included in the actual DataContract sent between services and is
        /// only available if you are using the Bfw.Agilix.DataContracts library directly.
        /// </summary>
        public Item Item { get; set; }
    }
}
