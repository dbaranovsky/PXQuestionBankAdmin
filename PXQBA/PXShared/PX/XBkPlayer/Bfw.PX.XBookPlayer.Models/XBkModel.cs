using System.Collections.Generic;
using Bfw.PX.XBkPlayer.Biz.DataContracts;
using System.Web.Mvc;

namespace Bfw.PX.XBkPlayer.Models
{
    public class XBkModel
    {
        public string SaveUrl { get; set; }

        public string ServerUrl { get; set; }

        public string LoadUrl { get; set; }

        public string PlayerUrl { get; set; }

        public string XBkJsonResult { get; set; }

        public bool InputError { get; set; }

        public string QuestionId { get; set; }

        public string EntityId { get; set; }

        public string QuizId { get; set; }

        public string EnrollmentId { get; set; }

        public XBkPlayerData XBkPlayerData { get; set; }

        public XBkModel(System.Web.HttpRequestBase request)
        {
            UrlHelper url = new UrlHelper(request.RequestContext);

            this.QuestionId = string.IsNullOrEmpty(request.Params["questionId"]) ? "" : request.Params["questionId"].ToString();
            this.EntityId = string.IsNullOrEmpty(request.Params["entityId"]) ? "" : request.Params["entityId"].ToString();
            this.EnrollmentId = string.IsNullOrEmpty(request.Params["enrollmentId"]) ? "" : request.Params["enrollmentId"].ToString();
            this.QuizId = string.IsNullOrEmpty(request.Params["quizId"]) ? "" : request.Params["quizId"].ToString();
            this.InputError = string.IsNullOrEmpty(this.QuestionId) || string.IsNullOrEmpty(this.EntityId);

            this.SaveUrl = url.Action("SaveXBkData", "XBP");
            this.ServerUrl = url.Content("~");

            this.LoadUrl = url.Action("LoadXBkPlayer", "XBP", new
            {
                questionId = this.QuestionId,
                entityId = this.EntityId,
                enrollmentId = this.EnrollmentId,
                quizId = this.QuizId,
                playerUrl = this.PlayerUrl
            });
        }
    }
}
