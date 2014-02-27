using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents a navigation item business object.
    /// </summary>
    [DataContract]
    public class NavigationItem
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the navigation item.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent ID.
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        /// The order in which this item should be displayed relative to other items with the same parent.
        /// </summary>
        [DataMember]
        public string Sequence { get; set; }

        /// <summary>
        /// Gets or sets a navigation item type.
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Visibility flag for  
        /// </summary>
        [DataMember]
        public bool IsVisible { get; set; }
        /// <summary>
        /// Gets or sets the navigation item description.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Collection of child navigation items if any exist.
        /// </summary>
        [DataMember]
        public List<NavigationItem> Children { get; set; }

        /// <summary>
        /// Collection of TOC categories the navigation item falls under.
        /// </summary>
        [DataMember]
        public IEnumerable<TocCategory> Categories { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NavigationItem"/> is highlighted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if highlighted; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Highlilghted
        {
            get { return Properties.GetPropertyValue<bool>("highlighted", false); }
            set { Properties.SetPropertyValue("highlighted", PropertyType.Boolean, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="NavigationItem"/> is hidden.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the navigation item visibility settings xml.
        /// </summary>
        [DataMember]
        public string Visibility
        {
            get { return Properties.GetPropertyValue<string>("bfw_visibility", "<bfw_visibility />"); }
            set { Properties.SetPropertyValue("bfw_visibility", PropertyType.String, value); }
        }

        /// <summary>
        /// Gets or sets the parent table of contents filter ID.
        /// </summary>
        /// <value>
        /// The toc ID.
        /// </value>
        [DataMember]
        public string TocId
        {
            get { return Properties.GetPropertyValue<string>("bfw_tocid", ""); }
            set { Properties.SetPropertyValue("bfw_tocid", PropertyType.String, value); }
        }

        /// <summary>
        /// Gets or sets the display title.
        /// </summary>
        [DataMember]
        public string DisplayTitle
        {
            get { return Properties.GetPropertyValue<string>("bfw_displaytitle", ""); }
            set { Properties.SetPropertyValue("bfw_displaytitle", PropertyType.String, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation item title should be displayed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is display title; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsDisplayTitle
        {
            get { return Properties.GetPropertyValue<bool>("bfw_isdisplaytitle", false); }
            set { Properties.SetPropertyValue("bfw_isdisplaytitle", PropertyType.Boolean, value); }
        }

        /// <summary>
        /// Gets or sets the title URL.
        /// </summary>
        [DataMember]
        public string TitleURL
        {
            get { return Properties.GetPropertyValue<string>("bfw_titleurl", ""); }
            set { Properties.SetPropertyValue("bfw_titleurl", PropertyType.String, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item supports adding child links.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance supports adding child items; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsSupportAddLink
        {
            get { return Properties.GetPropertyValue<bool>("bfw_issupportaddlink", false); }
            set { Properties.SetPropertyValue("bfw_issupportaddlink", PropertyType.Boolean, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is supports adding child menus.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance supports adding child menus; otherwise, <c>false</c>.
        /// </value>
        [DataMember]
        public bool IsSupportAddMenu
        {
            get { return Properties.GetPropertyValue<bool>("bfw_issupportaddmenu", false); }
            set { Properties.SetPropertyValue("bfw_issupportaddmenu", PropertyType.Boolean, value); }
        }

        /// <summary>
        /// If the navigation item represents an item that has a due date assigned, the date is available here.
        /// Otherwise this property is null.
        /// </summary>
        [DataMember]
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Gets or sets the max points.
        /// </summary>
        [DataMember]
        public Double? MaxPoints { get; set; }

        /// <summary>
        /// Collection of child links, if exist.
        /// </summary>
        [DataMember]
        public List<ContentItem> Links { get; set; }


        private string _dataString;

        [System.Runtime.Serialization.OnSerializing]
        private void OnSerializing(System.Runtime.Serialization.StreamingContext context)
        {
            if (_data != null)
            {
                _dataString = _data.ToString();
            }
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (!string.IsNullOrEmpty(_dataString))
            {
                _data = XElement.Parse(_dataString);
            }
        }

        [NonSerialized]
        private XElement _data;

        /// <summary>
        /// XML item data read from agilix item retrieval.
        /// </summary>
        public XElement Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        /// <summary>
        /// Properties collection for any metadata attached to a navigation item.
        /// </summary>
        [DataMember]
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// Properties collection for any metadata attached to a navigation item.
        /// </summary>
        [DataMember]
        public bool IsStudentCreated { get; set; }

        /// <summary>
        /// Course type to which the item is locked by.
        /// </summary>
        [DataMember]
        public string LockedCourseType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class.
        /// </summary>
        public NavigationItem()
        {
            Properties = new Dictionary<string, PropertyValue>();
        }
    }
}
