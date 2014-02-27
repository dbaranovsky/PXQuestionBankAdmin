using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class Zone
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
        public string Abbreviation { get; set; }

        /// <summary>
        /// list of widgets in currently zone
        /// </summary>
        public List<Widget> Widgets { get; set; }

        /// <summary>
        /// get a comma separated list of template ids of widgets which are allowed in the zone
        /// </summary>
        public string AllowedWidgetList { get; set; }

        public IList<AllowedWidget> AllowedWidgets { get; set; }
        
        public PageDefinition DefaultPage { get; set; }

        public bool IsSupportHide { get; set; }

        public Zone()
        {
           Widgets = new List<Widget>();
           AllowedWidgets = new List<AllowedWidget>();
        }
    }
}
