using System.Linq;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class QuestionNoteMapper
    {

        /// <summary>
        /// Map a QuestionNote model to a QuestionNote business object.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.QuestionNote ToQuestionNote(this QuestionNote model)
        {
            var biz = new BizDC.QuestionNote
                          {
                              Id = model.Id,
                              QuestionId = model.QuestionId,
                              CourseId = model.CourseId,
                              Text = model.Text,                              
                              Created = model.Created,
                              UserId = model.UserId,
                              FirstName = model.FirstName,
                              LastName = model.LastName,
                              AttachPath = model.AttachPath
                          };
            return biz;
        }

        /// <summary>
        /// Map a QuestionNote business object to a QuestionNote model.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <returns></returns>
        public static QuestionNote ToQuestionNote(this BizDC.QuestionNote biz)
        {
            var model = new QuestionNote
                            {
                                Id = biz.Id,
                                QuestionId = biz.QuestionId,
                                CourseId = biz.CourseId,
                                Text = biz.Text,
                                Created = biz.Created,
                                UserId = biz.UserId,
                                FirstName = biz.FirstName,
                                LastName = biz.LastName,
                                AttachPath = biz.AttachPath
                            };
            return model;
        }
    }
}
