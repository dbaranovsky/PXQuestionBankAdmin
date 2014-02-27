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
    public static class TocItemMapper
    {
        /// <summary>
        /// Determine whether or not this item was assigned to Assignment Center [Biz]
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static bool IsAssociatedWithAssignmentCenter(this IEnumerable<BizDC.TocCategory> categories)
        {
            if (categories.IsNullOrEmpty())
                return false;

            return
                categories.ToList()
                    .Exists(
                        i => i.Id.ToLowerInvariant().Contains("syllabus") || i.Id.ToLowerInvariant().Contains("module_"));
        }

        /// <summary>
        /// Determine whether or not this item was assigned to Assignment Center [model]
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static bool IsAssociatedWithAssignmentCenter(this IEnumerable<TocCategory> categories)
        {
            return
                categories.ToList()
                    .Exists(
                        i => i.Id.ToLowerInvariant().Contains("syllabus") || i.Id.ToLowerInvariant().Contains("module_"));
        }

        /// <summary>
        /// Converts to a Toc Item from a Biz Navigation Item.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static TocItem ToTocItem(this BizDC.NavigationItem biz)
        {
            return ToTocItem(biz, string.Empty, false);
        }

        /// <summary>
        /// Maps a NavigationItem business object tot a TocItem model.
        /// </summary>
        /// <param name="biz">The NavigationItem business object.</param>
        /// <param name="activeItemId">The active item id.</param>
        /// <param name="showControls">if set to <c>true</c> [show controls].</param>
        /// <returns>
        /// TocItem model.
        /// </returns>
        public static TocItem ToTocItem(this BizDC.NavigationItem biz, string activeItemId, bool showControls)
        {
            return ToTocItem(biz, activeItemId, showControls, null, null);
        }

        /// <summary>
        /// Maps a NavigationItem business object tot a TocItem model.
        /// </summary>
        /// <param name="biz">The NavigationItem business object.</param>
        /// <param name="activeItemId">The active item id.</param>
        /// <param name="showControls">if set to <c>true</c> [show controls].</param>
        /// <returns>
        /// TocItem model.
        /// </returns>
        public static TocItem ToTocItem(this BizDC.NavigationItem biz, string activeItemId, bool showControls,
            IEnumerable<BizDC.ContentItem> items, IEnumerable<BizDC.ItemUpdate> updates)
        {
            return ToTocItem(biz, activeItemId, showControls, items, updates, null);
        }

        /// <summary>
        /// Maps a NavigationItem business object tot a TocItem model.
        /// </summary>
        /// <param name="biz">The NavigationItem business object.</param>
        /// <param name="activeItemId">The active item id.</param>
        /// <param name="showControls">if set to <c>true</c> [show controls].</param>
        /// <returns>
        /// TocItem model.
        /// </returns>
        public static TocItem ToTocItem(this BizDC.NavigationItem biz, string activeItemId, bool showControls,
            IEnumerable<BizDC.ContentItem> items, IEnumerable<BizDC.ItemUpdate> updates, BizSC.IBusinessContext context)
        {
            TocItem model = null;

            if (null != biz)
            {
                var children = new List<TocItem>();
                foreach (BizDC.NavigationItem child in biz.Children)
                {
                    children.Add(child.ToTocItem(activeItemId, showControls, items, updates, context));
                }


                model = new TocItem(biz.Name, biz.Id, biz.Type, biz.Description, children);
                model.ParentId = biz.ParentId;
                model.IsContent = (biz.Type != "Folder" && biz.Type != "None");
                model.IsActive = (!string.IsNullOrEmpty(activeItemId) &&
                                  model.Id.ToLowerInvariant() == activeItemId.ToLowerInvariant());
                model.IsHiddenFromStudents = biz.Hidden;
                model.ShowControls = showControls;
                model.Sequence = biz.Sequence;
                model.IsStudentCreated = biz.IsStudentCreated;

                model.IsPartOfAssignmentCenter = biz.Categories.IsAssociatedWithAssignmentCenter();

                if (biz.Type != null)
                {
                    model.ItemType = biz.Type.ToLower();
                }

                if (biz.DueDate.HasValue && biz.DueDate.Value.Year != DateTime.MinValue.Year)
                {
                    model.IsAssigned = true;
                    model.DueDate = biz.DueDate.Value;

                    if (biz.MaxPoints.HasValue)
                    {
                        model.MaxPoints = biz.MaxPoints.Value;
                    }
                }

                if (context != null)
                {
                    model.IsLocked = !string.IsNullOrEmpty(biz.LockedCourseType) &&
                                     biz.LockedCourseType != context.Course.CourseType;
                }
            }

            return model;
        }
    }
}
