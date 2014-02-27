using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    [DataContract]
    public class Menu
    {
        /// The unique Id of the item.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Id of the resourceentityid the item belongs to.
        /// </summary>
        [DataMember]
        public string CourseID { get; set; }

        /// <summary>
        /// Id of the parent content item, if any exist.
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// The primary type of the item (typically the Agilix type).
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// Count of all items and children items in menu
        /// </summary>
        public int FlatCount { get; set; }

        /// <summary>
        /// The title of the item.
        /// </summary>
        [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The Abbreviation of the item.
        /// </summary>
        [DataMember]
        public string Abbreviation { get; set; }

        /// <summary>
        /// The bfw type of the item.
        /// </summary>
        [DataMember]
        public string BfwType { get; set; }

        /// <summary>
        /// The bfw sub type of the item.
        /// </summary>
        [DataMember]
        public string BfwSubType { get; set; }

        /// <summary>
        /// The Menu items to display
        /// </summary>
        public List<MenuItem> MenuItems { get; set; }

        /// <summary>
        /// The template items
        /// </summary>
        public List<MenuItem> MenuItemTemplates { get; set; }


        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>
        [DataMember]
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// The bfw Toc Id.
        /// </summary>
        [DataMember]
        public string BfwTocId { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public Menu()
        {
            Properties = new Dictionary<string, PropertyValue>();
            MenuItems = new List<MenuItem>();
            MenuItemTemplates = new List<MenuItem>();
        }

        /// <summary>
        /// Overriding ToString, will return ItemID
        /// </summary>
        public override string ToString()
        {
            string message = string.Empty;
            message = string.Format("ItemID: {0}", Id);
            return message;
        }
    }
}
