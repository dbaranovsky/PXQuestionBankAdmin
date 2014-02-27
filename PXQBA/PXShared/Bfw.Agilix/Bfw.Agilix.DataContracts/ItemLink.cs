using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents an item link in Agilix.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ItemLink : IDlapEntityParser
    {
        /// <summary>
        /// ID of the course that contains Id. If Id is empty, this is the derivative course ID that links to the entityid specified in the request.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// ID of the item that links to itemid; or empty string ("") if you did not specify an itemid.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        public void ParseEntity(XElement element)
        {
            var entityId = element.Attribute(ElStrings.Entityid);
            var id = element.Attribute(ElStrings.Id);

            if (entityId != null)
            {
                EntityId = entityId.Value;
            }
            if (id != null)
            {
                Id = id.Value;
            }
        }
    }
}
