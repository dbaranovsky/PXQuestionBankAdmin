using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
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

        public BizDC.LaunchPadSettings ToDcItem()
        {
            BizDC.LaunchPadSettings item = new BizDC.LaunchPadSettings();
            item.ShowCollapseUnassigned = this.ShowCollapseUnassigned;
            item.CollapseUnassigned = this.CollapseUnassigned;
            item.CollapseDueLater = this.CollapseDueLater;
            item.CollapsePastDue = this.CollapsePastDue;
            item.CollapseUnassignedItems = this.CollapseUnassignedItems;
            item.SortByDueDate = this.SortByDueDate;
            item.ShowCollapsePastDue = this.ShowCollapsePastDue;
            item.DueLaterDays = this.DueLaterDays;
            item.CategoryName = this.CategoryName;
            item.DisableEditing = this.DisableEditing;
            item.GrayoutPastDueLater = this.GrayoutPastDueLater;
            item.DisableDragAndDrop = this.DisableDragAndDrop;
            item.ShowItemsOnly = this.ShowItemsOnly;
            item.SplitAssigned = this.SplitAssigned;
            item.Properties = this.Properties;

            item.Id = this.Id;
            item.Id = this.Id;
            item.ParentId = this.ParentId;
            item.CourseID = this.EntityID;
            item.Title = this.Title;
            item.Sequence = this.Sequence;
            item.Type = this.Type;

            return item;
        }

        public void ToModelItem(BizDC.LaunchPadSettings dcItem)
        {
            this.ShowCollapseUnassigned = dcItem.ShowCollapseUnassigned;
            this.CollapseUnassigned = dcItem.CollapseUnassigned;
            this.CollapseDueLater = dcItem.CollapseDueLater;
            this.CollapsePastDue = dcItem.CollapsePastDue;
            this.CollapseUnassignedItems = dcItem.CollapseUnassignedItems;
            this.SortByDueDate = dcItem.SortByDueDate;
            this.ShowCollapsePastDue = dcItem.ShowCollapsePastDue;
            this.DueLaterDays = dcItem.DueLaterDays;
            this.CategoryName = dcItem.CategoryName;
            this.DisableEditing = dcItem.DisableEditing;
            this.GrayoutPastDueLater = dcItem.GrayoutPastDueLater;
            this.DisableDragAndDrop = dcItem.DisableDragAndDrop;
            this.ShowItemsOnly = dcItem.ShowItemsOnly;
            this.SplitAssigned = dcItem.SplitAssigned;
            this.Properties = dcItem.Properties;

            this.Id = dcItem.Id;
            this.Id = dcItem.Id;
            this.ParentId = dcItem.ParentId;
            this.EntityID = dcItem.CourseID;
            this.Title = dcItem.Title;
            this.Sequence = dcItem.Sequence;
            this.Type = dcItem.Type;
        }
    }
}
