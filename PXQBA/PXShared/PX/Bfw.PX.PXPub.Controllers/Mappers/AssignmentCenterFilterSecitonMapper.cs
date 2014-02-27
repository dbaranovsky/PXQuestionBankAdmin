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
    public static class AssignmentCenterFilterSectionMapper
    {
        /// <summary>
        /// Converts to an Assignment Filter Section from a Biz Content Item.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <param name="context">The context.</param>
        /// <param name="isLoadData">if set to <c>true</c> [is load data].</param>
        /// <param name="courseItems">The course items.</param>
        /// <param name="isLoadSubFilterSections">if set to <c>true</c> [is load sub filter sections].</param>
        /// <returns></returns>
        public static AssignmentCenterFilterSection ToAssignmentCenterFilterSection(this BizDC.ContentItem biz, BizSC.IContentActions content, BizSC.IBusinessContext context, bool isLoadData, List<ContentItem> courseItems, bool isLoadSubFilterSections)
        {
            var model = new AssignmentCenterFilterSection();

            using (context.Tracer.StartTrace("ToAssignmentCenterFilterSection"))
            {
                model.ToBaseContentItem(biz);
                model.Url = biz.Href;
                model.SyllabusType = biz.Properties.ContainsKey("bfw_filter_syllabus_type") == false ? "" : biz.Properties["bfw_filter_syllabus_type"].Value.ToString();
                model.UserAccess = context.AccessLevel;

                model.StartDate = DateTime.MinValue;
                model.DueDate = DateTime.MinValue;

                if (biz.Properties.ContainsKey("bfw_filter_start_date"))
                {
                    DateTime retval = model.StartDate;
                    DateTime.TryParse(biz.Properties["bfw_filter_start_date"].Value.ToString(), out retval);
                    model.StartDate = retval;
                }

                if (biz.Properties.ContainsKey("bfw_filter_end_date"))
                {
                    DateTime retval = model.DueDate;
                    DateTime.TryParse(biz.Properties["bfw_filter_end_date"].Value.ToString(), out retval);
                    model.DueDate = retval;
                }


                model.Description = !String.IsNullOrEmpty(biz.Description) ? biz.Description : model.Title;

                if (isLoadData && !courseItems.IsNullOrEmpty() )
                {
                    var filterItems = courseItems.FindAll(item => (item.Categories.FirstOrDefault(c => c.Id == model.Id || c.ItemParentId == model.Id) != null) || item.SyllabusFilter == model.Id );

                    //filterItems.Where(i => i.HiddenFromStudents == false || context.AccessLevel == BizSC.AccessLevel.Instructor);
                    foreach (var item in filterItems.Where(i => i.HiddenFromStudents == false || context.AccessLevel == BizSC.AccessLevel.Instructor))
                    {
                        item.UserAccess = context.AccessLevel;
                        model.AddChildItems(item);
                    }
                }

                if (isLoadSubFilterSections)
                {
                    foreach (var syllabusItem in content.ListChildren(biz.CourseId, model.Id).Where(c => c.Subtype != null && c.Subtype.Equals("AssignmentCenterFilterSection", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        var filter = syllabusItem.ToAssignmentCenterFilterSection(content, context, isLoadData, courseItems, false);
                        model.ChildrenFilterSections.Add(filter);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Converts to a Biz Content Item from an Assignment Center Filter Section.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        public static BizDC.ContentItem ToContentItem(this AssignmentCenterFilterSection model, string courseId)
        {
            var biz = model.ToBaseContentItem(courseId);
            if (null != model)
            {
                biz.Type = "Folder";
                biz.Subtype = "AssignmentCenterFilterSection";
                biz.ParentId = model.ParentId;

                biz.Properties["bfw_filter_start_date"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.DateTime, Value = model.StartDate };
                biz.Properties["bfw_filter_end_date"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.DateTime, Value = model.DueDate };
                biz.Properties["bfw_filter_syllabus_type"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.SyllabusType };
                biz.Properties["bfw_filter_tocid"] = new BizDC.PropertyValue() { Type = BizDC.PropertyType.String, Value = model.TocId };

                biz.AssignmentSettings = new BizDC.AssignmentSettings()
                {
                    IsAssignable = true,
                    DropBoxType = BizDC.DropBoxType.None,
                    DueDate = model.DueDate,
                    Points = 0,
                    Category = ""
                };

                if (model.Categories.IsNullOrEmpty())
                {
                    biz.DefaultCategoryParentId = biz.ParentId;
                    biz.DefaultCategorySequence = biz.Sequence;
                }
            }

            return biz;
        }
    }
}
