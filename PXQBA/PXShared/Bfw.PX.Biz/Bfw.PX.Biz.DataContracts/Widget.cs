using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
    [System.Serializable]
    [DataContract]
    public class Widget
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
        /// This list has values from the bfw_display_flags child elements
        /// </summary>
        [DataMember]
        public WidgetDisplayOptions WidgetDisplayOptions { get; set; }

        /// <summary>
        /// list of all the callbacks for the widget
        /// </summary>
        [DataMember]
        public IDictionary<string, WidgetCallback> Callbacks { get; set; }

        /// <summary>
        /// list of all input helpers for the widget
        /// </summary>
        [DataMember]
        public IDictionary<string, WidgetInputHelper> InputHelpers { get; set; }

        /// <summary>
        /// item id of the item form which this widget was created
        /// </summary>
        [DataMember]
        public string Template { get; set; }

        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>
        [DataMember]
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>
        [DataMember]
        public IDictionary<string, BHProperty> BHProperties { get; set; }

        /// <summary>
        /// If set to true, items in this TOC will be loaded from the product course rather than the derivative
        /// This allows us to have a fresh copy of the TOC w/o any instructor modifications
        /// </summary>
        [DataMember]
        public bool UseProductCourse
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_use_product_course", false);
            }
        }

        [DataMember]
        public bool IsMultipleAllowed
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_ismultipleallowed", false);
            }
        }

        [DataMember]
        public bool IsCollapseAllowed
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_iscollapseallowed", true);
            }
        }

        [DataMember]
        public bool IsTitleHidden
        {
            get
            {
                return Properties.GetPropertyValue<bool>("bfw_istitlehidden", false);
            }
        }

        [DataMember]
        public bool ListStudents
        {
            get { return Properties.GetPropertyValue<bool>("bfw_liststudents", true); }
        }

        /// <summary>
        /// Show the persistent tooltip
        /// </summary>
        [DataMember]
        public bool IsShowPersistentQtip
        {
            get { return Properties.GetPropertyValue<bool>("bfw_show_persistent_qtip", false); }
        }

        /// <summary>
        /// List of persistent qtip Ids
        /// </summary>
        [DataMember]
        public List<string> PersistentQtips
        {
            get
            {
                var qtips = Properties.GetPropertyValue<string>("bfw_qtips", string.Empty);
                var qtipList = new List<string>();
                if (!string.IsNullOrEmpty(qtips))
                {
                    qtipList = new List<string>(qtips.Split(','));
                }

                return qtipList;
            }
        }
        /// <summary>
        /// Element that stores instructor console settings
        /// </summary>
        public InstructorConsoleSettings InstructorConsoleSettings { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Widget()
        {
            Properties = new Dictionary<string, PropertyValue>();
            BHProperties = new Dictionary<string, BHProperty>();
            Callbacks = new Dictionary<string, WidgetCallback>();
            InputHelpers = new Dictionary<string, WidgetInputHelper>();
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

    [Serializable]
    [DataContract]
    public class BHProperty
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public PropertyType Type { get; set; }

        [DataMember]
        public IEnumerable<object> Values { get; set; }

        [DataMember]
        public Dictionary<string, BHParams> Parameters { get; set; }
    }

    [Serializable]
    [DataContract]
    public class BHParams
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public PropertyType Type { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public IEnumerable<object> Values { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Rel { get; set; }
    }
}