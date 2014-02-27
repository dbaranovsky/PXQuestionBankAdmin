using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using Bfw.Common.Collections;

using Microsoft.Practices.ServiceLocation;

using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class AssignmentCenterItemMapper
    {
        public static BizDC.AssignmentCenterItem ToAssignmentCenterItem(this AssignmentCenterItem model)
        {
            List<Biz.DataContracts.Container> containers = null;
            List<Biz.DataContracts.Container> subContainerIds = null;
            if (model.Containers != null)
            {
                containers = GetContainers(model.Containers);
            }
            if (model.SubContainerIds != null)
            {
                subContainerIds = GetContainers(model.SubContainerIds);
            }
            return new BizDC.AssignmentCenterItem
            {
                Id = model.Id,
                ParentId = model.ParentId,
                Sequence = model.Sequence,
                CategorySequence = model.CategorySequence,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                StartDateTZ = model.StartDateTZ,
                EndDateTZ = model.EndDateTZ,
                Points = (model.Points)?? 0,
                DefaultPoints = (model.DefaultPoints)?? 0,
                GradebookCategory = model.GradebookCategory,
                PreviousParentId = model.PreviousParentId,
                WasDueDateManuallySet = model.WasDueDateManuallySet,
                Containers = containers,
                CustomFields = model.CustomFields,
                SubContainerIds = subContainerIds,
                SubmissionGradeAction = (BizDC.SubmissionGradeAction)model.SubmissionGradeAction
            };
        }

        public static AssignmentCenterItem ToAssignmentCenterItem(this ContentItem item, string toc = "syllabusfilter")
        {
            var isUnit = item.Type.ToLowerInvariant() == "pxunit";
            var model = new AssignmentCenterItem
            {
                Id = item.Id,
                ParentId = item.GetSyllabusFilterFromCategory(toc),
                Sequence = item.Sequence,
                CategorySequence = item.CategorySequence,
                EndDate = item.DueDateTZ.ServerTime, //when converting content item to assignment center item, assume that the time has been converted to server time (as that's how JSON arrives to the server)
                EndDateTZ = item.DueDateTZ,
                StartDateTZ = item.StartDateTZ,
                State = isUnit ? "closed" : "barren",
                Title = item.Title,
                WasDueDateManuallySet = item.WasDueDateManuallySet,
                DefaultPoints = item.DefaultPoints,
                Points = item.MaxPoints,
                Containers = item.Containers,
                SubContainerIds = item.SubContainerIds
            };

            if (!String.IsNullOrEmpty(toc))
            {
                model.ParentId = item.GetSyllabusFilterFromCategory(toc);
            }

            if (item.StartDateTZ != null)
            {
                model.StartDate = item.StartDateTZ.ServerTime;
            }

            if (isUnit)
            {
                var unit = item as PxUnit;
                var children = unit.GetAssociatedItems().Map(c => c.ToAssignmentCenterItem(toc)).ToList();

                model.Children = children;
            }

            return model;
        }

        private static List<Container> GetContainers(List<Bfw.PX.Biz.DataContracts.Container> containers)
        {
            List<Container> containerList = new List<Container>();
            foreach (var container in containers)
            {
                containerList.Add(new Container(container.Toc, container.Value, container.DlapType));
            }
            return containerList;
        }

        private static List<Biz.DataContracts.Container> GetContainers(List<Container> list)
        {
            List<Biz.DataContracts.Container> containerList = new List<Biz.DataContracts.Container>();
            foreach (var container in list)
            {
                containerList.Add(new Biz.DataContracts.Container(container.Toc, container.Value, container.DlapType));
            }
            return containerList;
        }
    }
}
