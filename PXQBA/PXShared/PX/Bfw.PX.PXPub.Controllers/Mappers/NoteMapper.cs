using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class NoteMapper
    {
        /// <summary>
        /// Converts to a Biz.Note from a Biz Content Item
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.Note ToNote(this Note model)
        {
            return new BizDC.Note()
            {
                Id = model.NoteId,
                EntityId = model.EntityId,
                Title = model.Title,
                Text = model.Text,
                CreatedBy = model.CreatedBy,
                Created = model.CreatedDate,
                Sequence = model.Sequence
            };
        }

        /// <summary>
        /// Converts to a Note from a Biz Content Item
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public static Note ToNote(this BizDC.Note biz, BizSC.IBusinessContext context)
        {
            var model = new Note()
            {
                NoteId = string.IsNullOrEmpty(biz.Id) ? biz.NoteId.ToString() : biz.Id,
                EntityId = biz.EntityId,
                Title = biz.Title,
                Text = biz.Text,
                Description = biz.Description,
                ShortText = biz.Text.FirstNWords(10),
                CreatedBy = biz.CreatedBy,
                CreatedDate = biz.Created,
                ModifiedDate = biz.Modified,
                Sequence = biz.Sequence,
                EnrollmentId = biz.EnrollmentId,
                ItemId = biz.ItemId,
                ReviewId = biz.ReviewId,
                UserId = biz.UserId,
                FirstName = biz.FirstName,
                LastName = biz.LastName,
                IsPublic = biz.Public,
                HighlightType = biz.HighlightType,
                Status = (BizDC.HighlightStatus)biz.Status
            };

            model.IsUserNote = (context.CurrentUser != null) && model.UserId == context.CurrentUser.Id;
            return model;
        }
    }
}
