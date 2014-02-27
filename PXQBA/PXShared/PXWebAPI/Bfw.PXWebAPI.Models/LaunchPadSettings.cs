using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    public class LaunchPadSettings : Widget
    {
        public bool ShowCollapseAll { get; set; }
        public bool CollapseAll { get; set; }
        public bool CollapsePastDue { get; set; }
        public bool CollapseDueLater { get; set; }
        public bool CollapseUnassignedItems { get; set; }
        public bool SortByDueDate { get; set; }
        public bool ShowCollapsePastDue { get; set; }
        public string DueLaterDays { get; set; }
        public string CategoryName { get; set; }
        public bool DisableEditing { get; set; }
        public bool GrayoutPastDueLater { get; set; }
        public bool DisableDragAndDrop { get; set; }
        public string ShowItemsOnly { get; set; }
    }
}
