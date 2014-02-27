using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizDC = Bfw.PX.Biz.DataContracts;
using System.Web.Mvc;
namespace Bfw.PX.QuestionEditor.Controllers.Mappers
{
    public static class HTSModelMapper
    {


        /// Maps ContentItem to Activity
        /// </summary>
        /// <param name="biz">Content Item</param>
        /// <param name="content">Context Service</param>
        /*public static Models.HTSModel ToHtsModel(System.Web.HttpRequestBase request, UrlHelper Url)
        {
            var model = new Models.HTSModel();

           
            //model.HTSData = 
            model.LoadUrl = Url.Action("LoadHtsData", "HTS", new { questionId = model.QuestionId, entityId = model.EntityId, enrollmentId = model.EnrollmentId, quizId = model.QuizId, playerUrl = model.PlayerUrl, convert = model.IsConvertMode });
            

            return model;
        }*/
    }
}
