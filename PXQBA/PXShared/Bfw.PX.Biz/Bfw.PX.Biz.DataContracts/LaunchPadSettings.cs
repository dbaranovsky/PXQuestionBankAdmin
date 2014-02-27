using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class LaunchPadSettings : Widget
    {
        public bool ShowCollapseUnassigned { get; set; }
        public bool CollapseUnassigned { get; set; }
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
        public bool SplitAssigned { get; set; }
    }
}
