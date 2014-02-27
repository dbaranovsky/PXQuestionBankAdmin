using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class NoteSearchMapper
    {
        /// <summary>
        /// Converts to a Biz Note Search from a Document view Model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static BizDC.NoteSearch ToNoteSearch(this DocumentToView model, BizSC.IBusinessContext context)
        {
            var noteSearch = new BizDC.NoteSearch
            {
                CourseId = context.Course.Id,
                EnrollmentId = model.SecondaryId,
                ItemId = model.ItemId,
                NoteId = model.NoteId,
                ReviewId = model.PeerReviewId,
                UserId = model.UserId,
                CurrentUserId = context.CurrentUser.Id,
                NoteType = (int)BizDC.NoteType.None,
                HighlightType = (BizDC.PxHighlightType)model.HighlightType,
                NotePublic = model.Shared
            };

            return noteSearch;
        }

    }
}
