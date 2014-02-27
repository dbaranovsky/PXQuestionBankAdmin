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
    public static class HighlightMapper
    {

        /// <summary>
        /// Converts to a Biz Hightlight from a Document view Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static BizDC.Highlight ToHighlight(this DocumentToView model, BizSC.IBusinessContext context)
        {
            if (model.CommentText == "Enter comment here") model.CommentText = "";
            if (model.CommentLink == "http://") model.CommentLink = "";
            var commentLink = String.IsNullOrEmpty(model.CommentLink) ? "" : String.Format(" <a href='{0}'>{0}</a>", model.CommentLink);
            var hlType = (Enum.IsDefined(typeof(BizDC.PxHighlightType), model.HighlightType))
                                       ? (BizDC.PxHighlightType)model.HighlightType
                                       : BizDC.PxHighlightType.GeneralContent;
         
            model.HighlightText = String.IsNullOrEmpty(model.HighlightText) ? "" : model.HighlightText.Trim();

            var highlight = new BizDC.Highlight
            {
                Color = model.HighlightColor,
                CourseId = context.CourseId,
                EnrollmentId = string.IsNullOrEmpty(model.SecondaryId) ? context.EnrollmentId : model.SecondaryId,
                Text = model.HighlightText,
                Start = model.start,
                StartOffset = model.startOffset,
                End = model.end,
                EndOffset = model.endOffset,
                Description = model.HighlightDescription,
                ItemId = model.ItemId,
                ReviewId = model.PeerReviewId,
                HighlightType = hlType,
                Public = model.Shared,
                HighlightId = model.HighlightId,
                UserId =
                    !String.IsNullOrEmpty(model.CommenterId)
                    ? model.CommenterId
                    : model.UserId,
                FirstName = context.CurrentUser.FirstName,
                LastName = context.CurrentUser.LastName,
                Status = model.Locked ? (int)BizDC.HighlightStatus.Locked : 0,
                NoteType =
                    String.IsNullOrEmpty(model.HighlightText)
                        ? BizDC.NoteType.GeneralNote
                        : BizDC.NoteType.HighlightNote,
                Notes = new List<BizDC.Note>
                {
                    new BizDC.Note
                        {
                            Text = model.CommentText + commentLink,
                            Description = model.HighlightDescription,
                            Public = false,
                            Created = DateTime.Now,
                            Modified = DateTime.Now                           
                        }
                },

            };

            return highlight;
        }
    }
}

