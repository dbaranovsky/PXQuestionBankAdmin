using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
namespace Bfw.PX.QuestionEditor.Models
{
    /// <summary>
    /// Class for the HTS Model
    /// </summary>
    public class HTSModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HTSModel"/> class.
        /// </summary>
        public HTSModel(System.Web.HttpRequestBase request, UrlHelper Url)
        {
            this.IsDebugMode = string.IsNullOrEmpty(request.Params["debug"]) ? false : true;
            this.IsConvertMode = string.IsNullOrEmpty(request.Params["convert"]) ? false : true;
            this.QuestionId = string.IsNullOrEmpty(request.Params["questionId"]) ? "" : request.Params["questionId"].ToString();
            this.EntityId = string.IsNullOrEmpty(request.Params["entityId"]) ? "" : request.Params["entityId"].ToString();
            this.UserId = string.IsNullOrEmpty(request.Params["userId"]) ? "" : request.Params["userId"].ToString();
            this.QuizId = string.IsNullOrEmpty(request.Params["quizId"]) ? "" : request.Params["quizId"].ToString();
            this.PlayerUrl = string.IsNullOrEmpty(request.Params["playerUrl"]) ? "" : request.Params["playerUrl"].ToString();

            this.EquationImageUrl = this.PlayerUrl.Replace("PxPlayer.ashx", "geteq.ashx");
            
            this.InvalidParams = string.IsNullOrEmpty(this.QuestionId) || string.IsNullOrEmpty(this.EntityId) || string.IsNullOrEmpty(this.PlayerUrl);
            this.SaveUrl = Url.Action("SaveHtsData", "HTS");
            this.SaveXmlUrl = Url.Action("SaveHtsXml", "HTS");
            this.PreviewUrl = Url.Action("PreviewHtsData", "HTS");

            this.ServerUrl = Url.Content("~");
            this.EditorUrl = Url.Action("Index", "HTS");
            this.XmlEditorUrl = Url.Action("LoadRawXML", "HTS");
            this.UpdateXMLUrl = Url.Action("UpdateXML", "HTS");
            this.ReloadUrl = Url.Action("Reload", "HTS");
    
            this.LoadUrl = Url.Action("LoadHtsData", "HTS", new { questionId = this.QuestionId, entityId = this.EntityId, userId = this.UserId, quizId = this.QuizId, playerUrl = this.PlayerUrl, convert = this.IsConvertMode });
            
        }

        /// <summary>
        /// Flag set to true if in debug mode
        /// </summary>
        public bool IsDebugMode { get; set; }

        /// <summary>
        /// Flag set to true if in Convert Mode
        /// </summary>
        public bool IsConvertMode { get; set; }

        /// <summary>
        /// Gets or sets the question id.
        /// </summary>
        /// <value>
        /// The question id.
        /// </value>
        public string QuestionId { get; set; }

        /// <summary>
        /// Gets or sets the Entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        /// <value>
        /// The user id.
        /// </value>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the quiz id.
        /// </summary>
        /// <value>
        /// The quiz id.
        /// </value>
        public string QuizId { get; set; }

        /// <summary>
        /// Gets or sets the player url
        /// </summary>
        /// <value>
        /// The player url
        /// </value>
        public string PlayerUrl { get; set; }

        /// <summary>
        /// Gets or sets the Equation Image url
        /// </summary>
        /// <value>
        /// The Equation Image url
        /// </value>
        public string EquationImageUrl { get; set; }

        /// <summary>
        /// Gets or sets Invaild Params
        /// </summary>
        /// <value>
        /// The Invaild Params
        /// </value>
        public bool InvalidParams { get; set; }

        /// <summary>
        /// Gets or sets Load Url
        /// </summary>
        /// <value>
        /// The Load Url
        /// </value>
        public string LoadUrl { get; set; }

        /// <summary>
        /// Gets or sets Save Url
        /// </summary>
        /// <value>
        /// The Save Url
        /// </value>
        public string SaveUrl { get; set; }

        /// <summary>
        /// Gets or sets Save Xml Url
        /// </summary>
        /// <value>
        /// The Save Xml Url
        /// </value>
        public string SaveXmlUrl { get; set; }

        /// <summary>
        /// Gets or sets Preview Url
        /// </summary>
        /// <value>
        /// The Preview Url
        /// </value>
        public string PreviewUrl { get; set; }

        /// <summary>
        /// Gets or sets Server Url
        /// </summary>
        /// <value>
        /// The Server Url
        /// </value>
        public string ServerUrl { get; set; }

        /// <summary>
        /// Gets or sets Editor Url
        /// </summary>
        /// <value>
        /// The Editor Url
        /// </value>
        public string EditorUrl { get; set; }

        /// <summary>
        /// Gets or sets Xml Editor Url
        /// </summary>
        /// <value>
        /// The Xml Editor Url
        /// </value>
        public string XmlEditorUrl { get; set; }

        /// <summary>
        /// Gets or sets Updated Xml Url
        /// </summary>
        /// <value>
        /// The Update Xml Url
        /// </value>
        public string UpdateXMLUrl { get; set; }

        /// <summary>
        /// Gets or sets Reload Url
        /// </summary>
        /// <value>
        /// The Reload Url
        /// </value>
        public string ReloadUrl { get; set; }

        /// <summary>
        /// Sets HTS Json Result
        /// </summary>
        /// <value>
        /// The HTS Json result
        /// </value>
        public string HTSJsonResult { get; set; }

    }
}
