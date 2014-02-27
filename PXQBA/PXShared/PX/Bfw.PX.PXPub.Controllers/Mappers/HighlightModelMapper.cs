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
    public static class HighlightModelMapper
    {

        /// <summary>
        /// Converts to a Hight from a Biz Hightlight.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static HighlightModel ToHighlight(this BizDC.Highlight biz, BizSC.IBusinessContext context)
        {
            var model = new HighlightModel
                            {
                                Text = biz.Text,
                                Description = biz.Text,
                                HighlightType = biz.HighlightType,
                                Public = biz.Public,
                                NoteType = biz.NoteType,
                                Status = (BizDC.HighlightStatus) biz.Status,
                                UserId = biz.UserId,
                                ItemId = biz.ItemId,
                                EnrollmentId = biz.EnrollmentId,
                                ReviewId = biz.ReviewId,
                                FirstName = biz.FirstName,
                                LastName = biz.LastName,
                                Color = biz.Color,
                                IsInstructor = context.AccessLevel == BizSC.AccessLevel.Instructor,
                                HighlightId = biz.HighlightId.ToString(),
                                IsUserHighlight = (context.CurrentUser != null) && biz.UserId == context.CurrentUser.Id,
                                AllowShareComments = context.Course.AllowCommentSharing
                            };

            if (!biz.Notes.IsNullOrEmpty())
            {
                model.Notes = biz.Notes.Map(n => n.ToNote(context)).OrderBy(n=>n.CreatedDate).ToList();
            }

            return model;
        }
    }
}