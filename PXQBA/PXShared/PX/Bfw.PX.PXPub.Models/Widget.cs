using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Xml.Linq;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    [Serializable]
    public class Widget
    {
        /// The unique Id of the item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Id of the resourceentityid the item belongs to.
        /// </summary>
        public string EntityID { get; set; }

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
        public string Abbreviation { get; set; }

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
        /// list of all input helpers for the widget
        /// </summary>
        public IDictionary<string, WidgetInputHelper> InputHelpers { get; set; }

        /// <summary>
        /// item id of the item form which this widget was created
        /// </summary>
        public string Template { get; set; }

        public bool IsMultipleAllowed { get; set; }

        public bool UseProductCourse { get; set; }

        public bool IsCollapseAllowed { get; set; }

        public bool IsTitleHidden { get; set; }

        public bool ListStudents { get; set; }

        /// <summary>
        /// Show the persistent tooltip
        /// </summary>
        public bool IsShowPersistentQtip { get; set; }

        /// <summary>
        /// List of persistent qtip Ids
        /// </summary>
        public List<string> PersistentQtips { get; set; }

        /// <summary>
        /// Gets or sets JSON list of school to be used in AutoComplete
        /// </summary>
        public string SchoolList { get; set; }

        /// <summary>
        /// Set of properties stored in the item.
        /// </summary>

        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// Set of BrainHoney properties stored in the item.
        /// </summary>
        public IDictionary<string, BHProperty> BHProperties { get; set; }

        public Widget()
        {
            Callbacks = new Dictionary<string, WidgetCallback>();
            Properties = new Dictionary<string, PropertyValue>(); 
            BHProperties = new Dictionary<string, BHProperty>(); 
            InputHelpers = new Dictionary<string, WidgetInputHelper>();
        }

        /// <summary>
        /// Makes it easier to get string values from the properties collection used by widgets
        /// </summary>
        /// <param name="key">key in the dictionary for the property</param>
        /// <param name="value">string representation of the value if found. Null otherwise</param>
        /// <returns></returns>
        public bool TryGetProperty<T>(string key, out T val, Func<object, T> convertFunc = null )
        {
            PropertyValue pv;
            val = default(T);
            try
            {
                if (Properties.TryGetValue(key, out pv))
                {
                    if (convertFunc != null)
                        val = convertFunc(pv.Value);
                    else
                        val = (T) pv.Value;
                    return true;
                }
            }
            catch (Exception e)
            {
                //TODO: Add some logging here
            }
            return false;
        }
    }
}
