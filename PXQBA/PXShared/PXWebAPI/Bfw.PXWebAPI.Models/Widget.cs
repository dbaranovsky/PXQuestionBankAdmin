using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    public class Widget
    {
        /// The unique Id of the item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Id of the resourceentityid the item belongs to.
        /// </summary>
        public string CourseID { get; set; }

        /// <summary>
        /// Id of the parent content item, if any exist.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// The primary type of the item (typically the Agilix type).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        public string Sequence { get; set; }


        /// <summary>
        /// The title of the item.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The Abbreviation of the item.
        /// </summary>
        [DataMember]

        /// <summary>
        /// The bfw type of the item.
        /// </summary>
        public string BfwType { get; set; }

        /// <summary>
        /// The bfw sub type of the item.
        /// </summary>
        public string BfwSubType { get; set; }

        /// <summary>
        /// This list has values from the bfw_display_flags child elements
        /// </summary>
        public WidgetDisplayOptions WidgetDisplayOptions { get; set; }

        /// <summary>
        /// list of all the callbacks for the widget
        /// </summary>
        public IDictionary<string, WidgetCallback> Callbacks { get; set; }

        /// <summary>
        /// item id of the item form which this widget was created
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>
        public IDictionary<string,PropertyValue> Properties { get; set; }

        public bool IsMultipleAllowed
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_ismultipleallowed", false);
            }
        }

        public bool IsCollapseAllowed
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_iscollapseallowed", true);
            }
        }

        public bool IsTitleHidden
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_istitlehidden", false);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Widget()
        {
            Properties = new Dictionary<string,PropertyValue>();
            this.Callbacks = new Dictionary<string, WidgetCallback>();
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
