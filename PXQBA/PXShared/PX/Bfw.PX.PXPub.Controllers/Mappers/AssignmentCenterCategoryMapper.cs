using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class AssignmentCenterCategoryMapper
    {
        public static BizDC.AssignmentCenterCategory ToAssignmentCenterCategory(this AssignmentCenterCategory model)
        {
            var biz = new BizDC.AssignmentCenterCategory
            {
                Id = model.Id,
                StartDate = model.StartDate,
                EndDate = model.EndDate
            };

            return biz;
        }

        public static AssignmentCenterCategory ToAssignmentCenterCategory(this AssignmentCenterFilterSection filter)
        {
            var model = new AssignmentCenterCategory
            {
                Id = filter.Id,
                Title = filter.Title,
                Items = new List<AssignmentCenterItem>(),
                StartDate = filter.StartDate,
                EndDate = filter.DueDate
            };

            return model;
        }
    }
}
