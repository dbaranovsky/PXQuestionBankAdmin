using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common;
using Bfw.Common.Collections;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Contains all data required to display an assignment widget
    /// </summary>
    public class DashBoardWidget
    {
        /// <summary>
        /// The items to show for My Eportfolio Templates
        /// </summary>
        public List<DashBoardWidgetDataLine> MyTemplates
        {
            get;
            set;
        }

        /// <summary>
        /// The items to show for Publisher Templates
        /// </summary>
        public List<DashBoardWidgetDataLine> PublisherTemplates
        {
            get;
            set;
        }

        /// <summary>
        /// The items to show for Program Manager Templates
        /// </summary>
        public List<DashBoardWidgetDataLine> ProgramManagerTemplates
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor to use current date as reference date
        /// </summary>
        public DashBoardWidget()  {            
            MyTemplates = new List<DashBoardWidgetDataLine>();
            PublisherTemplates = new List<DashBoardWidgetDataLine>();
            ProgramManagerTemplates = new List<DashBoardWidgetDataLine>();
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance has data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has data; otherwise, <c>false</c>.
        /// </value>
        public bool HasData { get; set; }
    }


    public class DashBoardWidgetDataLine
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int Count { get; set; }
        public bool IsAllowDelete { get; set; }
        public string URL { get; set; }
        public string Id { get; set; }
        public string Parent { get; set; }

        /// <summary>
        /// Comma sepearated list of instructors, with whom the eportfolio has been shared
        /// </summary>
        public string SharedInstructors { get; set; }
    }
}
