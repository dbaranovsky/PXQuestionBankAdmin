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
    public class MenuItem
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
        /// Visibility flag for  
        /// </summary>
        /// 
        [DataMember]
        public bool IsVisible { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// This is the additional sequence to insert additional Menuitem inside menuitem (Second level / higher menu items).
        /// </summary>
        public string FlatSequence { get; set; }

        /// <summary>
        /// This is sub menu of this menu
        /// </summary>
        public List<MenuItem> MenuItems { get; set; }

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
        /// Gets the css class associated with a menu item
        /// </summary>
        [DataMember]
        public string BfwCssClass { get; set; }

        /// <summary>
        /// This list has values from the bfw_display_flags child elements
        /// </summary>
        [DataMember]
        public WidgetDisplayOptions WidgetDisplayOptions { get; set; }

        /// <summary>
        /// list of all the callbacks for the widget
        /// </summary>
        public IDictionary<string, MenuItemCallback> Callbacks {get; set;}

        /// <summary>
        /// item id of the item form which this widget was created
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// place for making item disabled with href = "#"
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>
        [DataMember]
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// The template Id
        /// </summary>
        [DataMember]
        public string BfwMenuCreatedby { get; set; }


        /// <summary>
        /// Determine whether to display on a product course
        /// </summary>
        [DataMember]
        public bool BfwDisplayOnProductCourse { get; set; }

        /// <summary>
        /// Determine if the menu option is selected by default.
        /// </summary>
        [DataMember]
        public bool SelectedByDefault { get; set; }


        /// <summary>
        /// menu item Numeric Menu Seqence
        /// </summary>
        [DataMember]
        public int MenuSequence { get; set; }


        /// <summary>
        /// Constructor
        /// </summary>
        public MenuItem()
        {
            Properties = new Dictionary<string, PropertyValue>();
            this.Callbacks = new Dictionary<string, MenuItemCallback>();
            MenuSequence = -1;
            IsDisabled = false;
        }

        /// <summary>
        /// Overriding ToString, will return ItemID
        /// </summary>
        public override string ToString()
        {
            string message = string.Empty;
            message = string.Format("ItemID: {0}",Id);
            return message;
        }
    }
}
