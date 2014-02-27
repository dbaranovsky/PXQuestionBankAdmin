using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class ModuleMapper
    {
        /// <summary>
        /// Converts to a Unit from a Biz Content Item
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <param name="children">The children.</param>
        /// <param name="getChildrenGrades">if set to <c>true</c> [get children grades].</param>
        /// <param name="isLoadChildren">if set to <c>true</c> [is load children].</param>
        /// <returns></returns>
        public static PxUnit ToPxUnit(this BizDC.ContentItem biz, BizSC.IContentActions content, string toc = "syllabusfilter",
            IEnumerable<BizDC.ContentItem> children = null, bool isLoadChildren = true)
        {
            var model = new PxUnit();
            model.ToLessonBase(biz, content, toc, children, isLoadChildren: isLoadChildren);
            return model;
        }

        /// <summary>
        /// Automatics the assignment unit.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static AssignmentUnit ToAssignmentUnit(this BizDC.ContentItem biz)
        {
            var assignmentUnit = new AssignmentUnit()
            {
                Title = biz.Title,
                Id = biz.Id,
                CategoryId = biz.UnitGradebookCategory
            };

            return assignmentUnit;
        }

        /// <summary>
        /// Convert to a BizDC.ContentItem from a Unit.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this PxUnit model, string courseId)
        {
            var biz = ((LessonBase)model).ToContentItem(courseId);
            biz.Subtype = "PxUnit";
            biz.ParentId = model.ParentId;
            biz.DefaultCategoryParentId = model.DefaultCategoryParentId;
            return biz;
        }
    }
}
