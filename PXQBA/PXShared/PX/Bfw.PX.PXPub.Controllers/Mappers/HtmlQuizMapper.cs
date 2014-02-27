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
    public static class HtmlQuizMapper
    {
        public static HtmlQuiz ToHtmlQuiz(this BizDC.ContentItem biz, BizSC.IContentActions content, bool loadChildren = false)
        {
            var model = new HtmlQuiz();
            model.ToBaseContentItem(biz);
            model.ToBaseQuiz(biz, content, null, loadChildren);
            model.IsProductCourse = content.Context.CourseIsProductCourse;
            model.Url = biz.Href;
            model.BHHtmlQuiz = !string.IsNullOrWhiteSpace(biz.ExamTemplate);

            return model;
        }
    }
}
