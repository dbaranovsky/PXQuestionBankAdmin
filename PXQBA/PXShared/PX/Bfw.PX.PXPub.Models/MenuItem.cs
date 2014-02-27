using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Linq;
using System.Runtime.Serialization;

using Bfw.PX.Biz.DataContracts;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Models
{
    [DataContract]
    public class MenuItem
    {
        /// The unique Id of the item.
        /// </summary>
        /// 
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Id of the resourceentityid the item belongs to.
        /// </summary>
        /// 
        [DataMember]
        public string CourseID { get; set; }

        /// <summary>
        /// Id of the parent content item, if any exist.
        /// </summary>
        /// 
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// The primary type of the item (typically the Agilix type).
        /// </summary>
        /// 
         [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Target to place item in (typically the Agilix type).
        /// </summary>
        /// 
         [DataMember]
        public string Target { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        /// 
         [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// This is the additional sequence to insert additional Menuitem inside menuitem (Second level / higher menu items).
        /// </summary>
        /// 
         [DataMember]
        public string FlatSequence { get; set; }

        /// <summary>
        /// This is sub menu of this menu
        /// </summary>
        /// 
         [DataMember]
        public List<MenuItem> MenuItems { get; set; }

        /// <summary>
        /// The title of the item.
        /// </summary>
        /// 
         [DataMember]
        public string Title { get; set; }

        /// <summary>
        /// The Abbreviation of the item.
        /// </summary>
        /// 
         [DataMember]
        public string Abbreviation { get; set; }

        /// <summary>
        /// The bfw type of the item.
        /// </summary>
        /// 
         [DataMember]
        public string BfwType { get; set; }

        /// <summary>
        /// The bfw sub type of the item.
        /// </summary>
        /// 
         [DataMember]
        public string BfwSubType { get; set; }

        /// <summary>
        /// Gets the css class associated with a menu item
        /// </summary>
        /// 
         [DataMember]
        public string BfwCssClass { get; set; }

        /// <summary>
        /// BfwMenuCreatedby
        /// </summary>
        /// 
         [DataMember]
        public string BfwMenuCreatedby { get; set; }

        /// <summary>
        /// This list has values from the bfw_display_flags child elements
        /// </summary>
        /// 
         [DataMember]
        public WidgetDisplayOptions WidgetDisplayOptions { get; set; }

        /// <summary>
        /// list of all the callbacks for the widget
        /// </summary>
        /// 
         [DataMember]
        public IDictionary<string, MenuItemCallback> Callbacks { get; set; }

        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>
        /// 
         [DataMember]
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// UI Mode
        /// </summary>
        /// 
         [DataMember]
        public string Controller { get; set; }

        /// <summary>
        /// UI Mode
        /// </summary>
        /// 
         [DataMember]
        public string Action { get; set; }

        /// <summary>
        /// call back params
        /// </summary>
        /// 
         [DataMember]
        public IDictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        /// 
         [DataMember]
        public string Url { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        /// 
         [DataMember]
        public string ClickUrl { get; set; }

        /// <summary>
        /// place holder for minSequence
        /// </summary>
        /// 
         [DataMember]
        public string minSequence { get; set; }

        /// <summary>
        /// place holder for maxSequence
        /// </summary>
        /// 
         [DataMember]
        public string maxSequence { get; set; }

        /// <summary>
        /// place for the currently selected menu item
        /// </summary>
        /// 
         [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// place for making item disabled with href = "#"
        /// </summary>
        /// 
         [DataMember]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Visibility flag for  
        /// </summary>
        /// 
         [DataMember]
        public bool IsHidden { get; set; }

        /// <summary>
        /// Display this menu Item on a course or not
        /// </summary>
        /// 
         [DataMember]
        public bool BfwDisplayOnProductCourse { get; set; }

        
        /// <summary>
        /// Determine if the menu option is selected by default.
        /// </summary>
        [DataMember]
        public bool SelectedByDefault { get; set; }

        /// <summary>
        /// Place holder for the Content Id selected from in the View
        /// </summary>
        /// 
         [DataMember]
        public string ContentItemId { get; set; }

        /// <summary>
        /// Can be seen be students
        /// </summary>
        /// 
         [DataMember]
        public bool VisibleByStudent { get; set; }

        /// <summary>
        /// Can be seen be Instructors
        /// </summary>
        /// 
         [DataMember]
        public bool VisibleByInstructor { get; set; }
        

        /// <summary>
        /// menu item Numeric Menu Seqence
        /// </summary>
        /// 
         [DataMember]
        public int MenuSequence { get; set; }

        public MenuItemCallback OnClick()
        {
            MenuItemCallback cb = new MenuItemCallback();
            if (Callbacks.ContainsKey("click"))
            {
                cb = Callbacks["click"];
            }

            return cb;
        }

        public static RouteValueDictionary createRouteValueDictionary(IDictionary<string, string> localParameters, string menuid)
        {
            RouteValueDictionary dict = null;
            if (localParameters != null)
            {
                dict = new RouteValueDictionary();
                foreach (var item in localParameters)
                {
                    dict.Add(item.Key, item.Value);
                }
                if (!dict.ContainsKey("menuitemid"))
                {
                    dict.Add("menuitemid", menuid);
                }
            }
            return dict;
        }

        public MenuItem()
        {
            Callbacks = new Dictionary<string, MenuItemCallback>();
            Properties = new Dictionary<string, PropertyValue>();
            BfwMenuCreatedby = String.Empty;
            WidgetDisplayOptions = new WidgetDisplayOptions();
            IsDisabled = false;
        }
    }
}
