using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Represents a page layout in its entirety. This includes any extra
    /// CSS files, javascript dependencies, and a list of widgets that need to be
    /// rendered.
    /// </summary>
    public class LayoutConfiguration
    {
        /// <summary>
        /// Contains all zones and the list of widgets to render in each.
        /// </summary>
        /// <value>
        /// The zones.
        /// </value>
        protected Dictionary<string, List<WidgetConfiguration>> Zones { get; set; }

        /// <summary>
        /// Used as the page title
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is activated.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is activated; otherwise, <c>false</c>.
        /// </value>
        public bool IsActivated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is allowed to create course.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is allowed to create course; otherwise, <c>false</c>.
        /// </value>
        public bool IsAllowedToCreateCourse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is allowed to activate.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is allowed to activate; otherwise, <c>false</c>.
        /// </value>
        public bool IsAllowedToActivate { get; set; }

        /// <summary>
        /// Gets or sets the type of the course.
        /// </summary>
        /// <value>
        /// The type of the course.
        /// </value>
        public string CourseType { get; set; }

        public PageDefinition PageDefinitions { get; set; }


        /// <summary>
        /// The Course
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Builds a default LayoutConfiguration with an empty set of zones.
        /// </summary>
        public LayoutConfiguration()
        {
            Zones = new Dictionary<string, List<WidgetConfiguration>>();
        }

        /// <summary>
        /// Provides access to an ordered list of widgets for the specified zone.
        /// </summary>
        /// <param name="zone">name of the zone to get widgets from</param>
        /// <returns>ordered set of widgets in the requested zone</returns>
        public IOrderedEnumerable<WidgetConfiguration> this[string zone]
        {
            get
            {
                IOrderedEnumerable<WidgetConfiguration> result = null;

                if (Zones.ContainsKey(zone))
                {
                    result = Zones[zone].OrderBy(w => w.Order);
                }

                return result;
            }
        }

        /// <summary>
        /// Adds a widget to the specified zone.
        /// </summary>
        /// <param name="zone">zone to add the widget to</param>
        /// <param name="widget">widget to add to the zone</param>
        public void AddWidget(string zone, WidgetConfiguration widget)
        {
            if (!Zones.ContainsKey(zone))
                Zones[zone] = new List<WidgetConfiguration>();

            Zones[zone].Add(widget);
        }

        /// <summary>
        /// LMS Id of user
        /// </summary>
        public string LMSId { get; set; }
    }
}
