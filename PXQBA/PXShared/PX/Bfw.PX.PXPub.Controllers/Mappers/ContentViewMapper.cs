// -----------------------------------------------------------------------
// <copyright file="ContentViewMapper.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class ContentViewMapper
    {
        /// <summary>
        /// To the content view.
        /// </summary>
        /// <param name="displayItem">The display item.</param>
        /// <param name="model">The model.</param>
        /// <param name="firstItemId">The first item id.</param>
        public static void ToContentView(this DisplayItem displayItem, ContentView model, IContentHelper contentHelper, string firstItemId)
        {
            if (model != null)
            {
                var context = ServiceLocator.Current.GetInstance<BizSC.IBusinessContext>();

                model.Content.NoteId = displayItem.CommentId;
                model.GroupId = displayItem.GroupId;

                if (!displayItem.ReadOnly.HasValue && context.AccessLevel == BizSC.AccessLevel.Student)
                {
                    model.Content.ReadOnly = true;
                }

                if (displayItem.ReadOnly.HasValue && displayItem.ReadOnly.Value && model.Content != null)
                {
                    model.Content.ReadOnly = true;
                    model.AllowedModes = ContentViewMode.None;
                }

                model.IncludeDiscussion = (displayItem.IncludeDiscussion.HasValue && displayItem.IncludeDiscussion.Value);

                model.IncludeNavigation = displayItem.IncludeNavigation.HasValue ? displayItem.IncludeNavigation.Value : true;

                if (displayItem.Mode == ContentViewMode.Edit && displayItem.IsBeingEdited.HasValue)
                {
                    model.Content.IsBeingEdited = displayItem.IsBeingEdited.Value;
                }

                if (displayItem.IncludeToc.HasValue && displayItem.IncludeToc.Value)
                {
                    model.TableOfContents = contentHelper.LoadToc(context.EntityId, firstItemId);
                }

                if (displayItem.Category.IsNullOrEmpty() && model.Content.UserAccess == AccessLevel.Student)
                {
                    displayItem.Category = "enrollment_" + model.Content.EnrollmentId;
                }


            }
        }
    }
}
