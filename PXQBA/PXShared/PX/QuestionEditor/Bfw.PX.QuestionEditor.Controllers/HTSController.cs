using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using Bfw.PX.QuestionEditor.Models;
using System.IO;
using System.Text;

using HtmlAgilityPack;
using BizDC = Bfw.PX.QuestionEditor.Biz.DataContracts;
using PxBizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.QuestionEditor.Biz.Services;

using Bfw.Common;
using Bfw.Common.Mvc;
using Mono.Options;
using System.Configuration;
using Microsoft.Practices.ServiceLocation;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml.Linq;


namespace QuestionEditor.Controllers
{
    /// <summary>
    /// Handles all actions related to HTS Editor component.
    /// </summary>
    public class HTSController : Controller
    {
        ///// <summary>
        ///// Access to an IHTSServices implementation.
        ///// </summary>
        ///// <value>
        ///// The content actions.
        ///// </value>
        protected BizSC.IHTSServices HtsServices { get; set; }

        public HTSController(BizSC.IHTSServices htsServices)
        {
            HtsServices = htsServices;
        }

        //public ActionResult Index()
        //{            
        //    HTSModel model = new HTSModel(Request, Url);

        //    model.htsData =HtsServices.GetHtsData(model.EntityId, model.QuestionId, model.PlayerUrl, model.IsConvertMode);
        //    model.htsData.RawXML = HttpUtility.HtmlEncode(model.htsData.RawXML);
        //    model.HTSJsonResult = new JavaScriptSerializer().Serialize(model);
            
        //    ViewData.Model = model;
        //    return View();
        //}
        public ActionResult Index()
        {
            HTSModel model = new HTSModel(Request, Url);

            if (model.IsDebugMode)
            {
                //entityId = "49698";
                //questionId = "e37cdeae-b2db-45da-9444-c683080ab921";
                model.EntityId = "47254";
                model.QuestionId = "43ecc430-b326-4488-9edd-06201d4440e5";
                model.PlayerUrl = "http://dev.px.bfwpub.com/PxHTS/PxPlayer.ashx";
                model.EquationImageUrl = model.PlayerUrl.ToLowerInvariant().Replace("pxplayer.ashx", "geteq.ashx");
            }            

            model.HTSJsonResult = new JavaScriptSerializer().Serialize(model);
            ViewData.Model = model;
            return View();
        }

        public ActionResult LoadHtsData()
        {
            string questionId, entityId, enrollmentId, playerUrl, formulaEditorUrl;
            //TODO: remove temporary Ids

            entityId = "";
            enrollmentId = "";
            questionId = "";
            playerUrl = "";

            if (Request.QueryString != null)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["entityId"]))
                    entityId = Request.QueryString["entityId"].ToString();

                if (!string.IsNullOrEmpty(Request.QueryString["enrollmentId"]))
                    enrollmentId = Request.QueryString["enrollmentId"].ToString();

                if (!string.IsNullOrEmpty(Request.QueryString["questionId"]))
                    questionId = Request.QueryString["questionId"].ToString();

                if (!string.IsNullOrEmpty(Request.QueryString["playerUrl"]))
                {
                    playerUrl = Request.QueryString["playerUrl"].ToString();
                }

                formulaEditorUrl = playerUrl.ToLowerInvariant().Replace("pxplayer.ashx", "geteq.ashx");

                var question = HtsServices.GetQuestion(entityId, questionId);

                var htsData = new BizDC.HTSData()
                {
                    EntityId = entityId,
                    EnrollmentId = enrollmentId,
                    QuestionId = questionId,                    
                    PlayerUrl = playerUrl,
                    FormulaEditorUrl = formulaEditorUrl,
                    QuestionTitle = question.Body.ToString()
                };
                try
                {
                    string convert = Request.QueryString["convert"];
                    if ((!string.IsNullOrWhiteSpace(convert) && Convert.ToBoolean(Request.QueryString["convert"])) && (string.IsNullOrEmpty(question.CustomUrl) || !question.CustomUrl.Equals("HTS")))
                    {
                        htsData.LoadQuestionXml(question.QuestionXml);
                        htsData.Solution = string.Empty;
                    }
                    else
                    {
                        /*if (!string.IsNullOrEmpty(question.InteractionData))
                        {
                            question.InteractionData = Regex.Replace(question.InteractionData, "&amp;", "&");
                        }*/
                        var interactionData = question.InteractionData;
                        htsData.LoadXML(interactionData);
                    }
                }
                catch (Exception e)
                {
                    htsData.ContentWithInvalidXml = "true";
                }
                //var jsonHtsData = JsonHelper.Serialize(htsData);
                //return Content(jsonHtsData, "application/json; charset=utf-8");

                var jsonResult = Json(htsData);
                var json = new JavaScriptSerializer().Serialize(jsonResult.Data);
                return Content(json, "application/json; charset=utf-8");
            }

            return Content("");
        }
        

        [ValidateInput(false)]
        public ActionResult PreviewHtsDataOld(BizDC.HTSData htsData)
        {
            string sResponse = "";

            if (htsData.Variables != null)
            {
                foreach (var variable in htsData.Variables)
                {
                    foreach (var cnstrnt in variable.Constraints)
                    {
                        cnstrnt.Inclusions = HttpUtility.HtmlEncode(cnstrnt.Inclusions);
                    }
                }
            }
            var interactionData = htsData.ToXML();
            //interactionData = Server.HtmlDecode(interactionData);
            try
            {
                var sXml = "<info id=\"0\" mode=\"Active\"><question schema=\"2\" partial=\"false\"><answer /><body></body><customurl>HTS</customurl><interaction type=\"custom\"><data><![CDATA["
                    + interactionData + "]]></data></interaction></question></info>";
                byte[] buffer = Encoding.UTF8.GetBytes(sXml);

                HttpWebRequest WebReq = null;
                //if (Request.UserHostAddress == "::1")
                //{
                //    WebReq = (HttpWebRequest)WebRequest.Create("http://localhost:50109/PxHTS/PxPlayer.ashx");            
                //}
                //else
                //{
                    WebReq = (HttpWebRequest)WebRequest.Create(htsData.PlayerUrl);
                //}              
                

                WebReq.Method = "POST";
                WebReq.ContentLength = buffer.Length;
                Stream ReqStream = WebReq.GetRequestStream();
                ReqStream.Write(buffer, 0, buffer.Length);
                ReqStream.Close();
                WebResponse WebRes = WebReq.GetResponse();
                Stream ResStream = WebRes.GetResponseStream();
                StreamReader ResReader = new StreamReader(ResStream);
                sResponse = ResReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                sResponse = ex.ToString();
            }

            try
            {
                XDocument myXml = XDocument.Parse(sResponse);
                sResponse = myXml.Element("custom").Element("body").Value; // CDATA property loaded as text.
                sResponse = sResponse.Replace("$QUESTIONID$", htsData.QuestionId); // just use placeholder value.

                if (sResponse.Length == 0)
                {
                    sResponse = "<br /><div id=\"info.divId\">The question does not contain any data.</div>";
                }
                else
                {
                    sResponse = "<br /><div id=\"info.divId\">Generating preview...</div>" + sResponse;
                }
                sResponse = sResponse.Replace("getElementById(info.divId)", "getElementById('info.divId')");
                sResponse = sResponse.Replace("info.divId", "info.divId" + htsData.QuestionId);
                //sResponse = sResponse.Replace("[~]", "/BrainHoney/Resource/" + htsData.EntityId);
            }
            catch (Exception ex2)
            {
                sResponse = "There was an error: " + ex2.ToString();
            }

            return Content(sResponse);
        }

        [ValidateInput(false)]
        public ActionResult PreviewHtsData(BizDC.HTSData htsData)
        {
            string sResponse = "";
            string quizId = "";

            //if (htsData.Variables != null)
            //{
            //    foreach (var variable in htsData.Variables)
            //    {
            //        foreach (var cnstrnt in variable.Constraints)
            //        {
            //            cnstrnt.Inclusions = HttpUtility.HtmlEncode(cnstrnt.Inclusions);
            //        }
            //    }
            //}

            if (!string.IsNullOrEmpty(htsData.PendingXML))
            {
                var pendingData = new BizDC.HTSData()
                {
                    EntityId = htsData.EntityId,
                    QuestionId = htsData.QuestionId,
                    QuizId = htsData.QuizId,
                    FormulaEditorUrl = htsData.FormulaEditorUrl,
                    Variables = htsData.Variables
                };

                try
                {
                    pendingData.LoadXML(htsData.PendingXML);
                }
                catch (Exception e)
                {
                    List<BizDC.Step> errorSteps = new List<BizDC.Step>();
                    BizDC.Step errorStep = new BizDC.Step() ;
                    errorStep.Question = string.Format("<p>Raw xml mode failed! {0} </p>", HttpUtility.HtmlEncode(e.Message));
                    errorSteps.Add(errorStep);
                    pendingData.Steps = errorSteps;

                }

                var interactionData = pendingData.ToXML();
                interactionData = Server.HtmlDecode(interactionData);

                htsData = pendingData;
            }

            try
            {

                quizId = HtsServices.StoreQuizPreview(htsData);
            }
            catch (Exception ex2)
            {
                sResponse = "There was an error: " + ex2.ToString();
            }

            return Content(quizId);
        }

        [ValidateInput(false)]
        public ActionResult SaveHtsData(BizDC.HTSData htsData)
        {
            try
            {
                PxBizDC.Question question = null;
                
                var interactionData = htsData.ToXML();
                interactionData = HttpUtility.HtmlDecode(interactionData);

                if (!string.IsNullOrEmpty(htsData.QuestionId))
                {
                    question = HtsServices.GetQuestion(htsData.EntityId, htsData.QuestionId);
                    question.InteractionData = interactionData;
                    question.Title = htsData.QuestionTitle;
                   
                    if (QuestionTitleToBeChanged(question))
                    {
                        question.Body = (string.IsNullOrEmpty(htsData.QuestionTitle)) ? "Untitled Advanced Question" : htsData.QuestionTitle;
                       
                    }
                    
                    question.CustomUrl = "HTS";
                    question.InteractionType = PxBizDC.InteractionType.Custom;
                }

                HtsServices.StoreQuestion(question);
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(500, string.Format("Save failed! {0} stack:{1}", e.Message, e.StackTrace));
            }

            return Json(new { message = "Question has been saved successfully!" });
        }

        [ValidateInput(false)]
        public ActionResult SaveHtsXml(BizDC.HTSData htsData)
        {
            ActionResult result = null;
            try
            {
                /*var pendingData = new BizDC.HTSData()
                {
                    EntityId = htsData.EntityId,
                    QuestionId = htsData.QuestionId,
                    QuizId = htsData.QuizId,
                    FormulaEditorUrl = htsData.FormulaEditorUrl,
                    Variables = htsData.Variables
                };

                pendingData.LoadXML(htsData.PendingXML);

                var interactionData = pendingData.ToXML();
                interactionData = HttpUtility.HtmlDecode(interactionData);*/

                BizDC.HTSData pendingData;
                string interactionData;
                UpdateXMLData(htsData, out pendingData, out interactionData);
                htsData = pendingData;

                PxBizDC.Question question = null;
                question = HtsServices.GetQuestion(pendingData.EntityId, pendingData.QuestionId);
                question.InteractionData = interactionData;
                if (QuestionTitleToBeChanged(question))
                {
                    question.Body = (string.IsNullOrEmpty(pendingData.QuestionTitle)) ? "Untitled Advanced Question" : pendingData.QuestionTitle;
                }                
                question.CustomUrl = "HTS";
                HtsServices.StoreQuestion(question);

                var jsonResult = Json(htsData);
                var json = new JavaScriptSerializer().Serialize(jsonResult.Data);
                result = Content(json, "application/json; charset=utf-8");                
            }
            catch (Exception e)
            {
                result = new HttpStatusCodeResult(500, string.Format("Raw xml mode failed! {0} stack:{1}", e.Message, e.StackTrace));
            }

            return result;
        }

        [ValidateInput(false)]
        public ActionResult SaveHtsInvalidXml(BizDC.HTSData htsData)
        {
            ActionResult result = null;
            try
            {
                var pendingData = new BizDC.HTSData()
                {
                    EntityId = htsData.EntityId,
                    QuestionId = htsData.QuestionId,
                    QuizId = htsData.QuizId,
                    FormulaEditorUrl = htsData.FormulaEditorUrl,
                    Variables = htsData.Variables
                };

                /*pendingData.LoadXML(htsData.PendingXML);

                var interactionData = pendingData.ToXML();*/
                var interactionData = htsData.PendingXML;

                PxBizDC.Question question = null;
                question = HtsServices.GetQuestion(pendingData.EntityId, pendingData.QuestionId);
                question.InteractionData = interactionData;
                if (QuestionTitleToBeChanged(question))
                {
                    question.Body = (string.IsNullOrEmpty(pendingData.QuestionTitle)) ? "Untitled Advanced Question" : pendingData.QuestionTitle;
                }
                question.CustomUrl = "HTS";
                HtsServices.StoreQuestion(question);

                var jsonResult = Json(pendingData);
                var json = new JavaScriptSerializer().Serialize(jsonResult.Data);
                result = Content(json, "application/json; charset=utf-8");
            }
            catch (Exception e)
            {
                result = new HttpStatusCodeResult(500, string.Format("Raw xml mode failed! {0} stack:{1}", e.Message, e.StackTrace));
            }

            return result;
        }

        private static bool QuestionTitleToBeChanged(PxBizDC.Question question)
        {
            return string.IsNullOrEmpty(question.Body) || question.Body.Equals("Untitled Advanced Question", StringComparison.OrdinalIgnoreCase) || question.Body.Equals("click here and start typing", StringComparison.OrdinalIgnoreCase);
        }

        [ValidateInput(false)]
        public ActionResult UpdateXML(BizDC.HTSData originalData)
        {
            ActionResult result = null;
            try
            {
                BizDC.HTSData pendingData;
                string interactionData;
                UpdateXMLData(originalData, out pendingData, out interactionData);
                result = Content(interactionData);

                var jsonResult = Json(pendingData);
                var json = new JavaScriptSerializer().Serialize(jsonResult.Data);
                return Content(json, "application/json; charset=utf-8");
            }
            catch (Exception e)
            {
                result = new HttpStatusCodeResult(500, string.Format("Raw xml mode failed! {0} stack:{1}", e.Message, e.StackTrace));
            }

            return result;
        }

        private void UpdateXMLData(BizDC.HTSData originalData, out BizDC.HTSData pendingData, out string interactionData)
        {
            pendingData = new BizDC.HTSData()
            {
                EntityId = originalData.EntityId,
                QuestionId = originalData.QuestionId,
                QuizId = originalData.QuizId,
                FormulaEditorUrl = originalData.FormulaEditorUrl,
                Variables = originalData.Variables
            };

            originalData.PendingXML = UpdateVariableName(originalData); // Server.HtmlDecode(UpdateVariableName(originalData)); 

            /*if (!string.IsNullOrEmpty(originalData.PendingXML))
            {
                originalData.PendingXML = Regex.Replace(originalData.PendingXML, "&amp;", "&");
            }*/
            pendingData.LoadXML(originalData.PendingXML); // Regex.Replace(originalData.PendingXML, "&(?!amp;|#)", "&amp;"));

            interactionData = pendingData.ToXML();
            interactionData = Server.HtmlDecode(interactionData);
        }

        private string UpdateVariableName(BizDC.HTSData data)
        {
            string sReturnXML = data.PendingXML;
            if (data.Variables != null)
            {
                foreach (var variable in data.Variables)
                {
                    if (variable.Name != variable.OldName && !string.IsNullOrEmpty(variable.OldName))
                    {
                        Regex exp = new Regex(@", ?");
                        var searchWord = @"(\b" + exp.Replace(variable.OldName, @"|") + @"\b)";
                        sReturnXML = Regex.Replace(sReturnXML, searchWord, variable.Name);
                    }
                }
            }

            return sReturnXML;
        }

        [ValidateInput(false)]
        public ActionResult LoadRawXML(BizDC.HTSData htsData)
        {
            ActionResult result = null;
            try
            {
                var interactionData = htsData.ToXML();
                interactionData = Server.HtmlDecode(interactionData);
                result = Content(interactionData);
            }
            catch (Exception e)
            {
                result = new HttpStatusCodeResult(500, string.Format("Raw xml mode failed! {0} stack:{1}", e.Message, e.StackTrace));
            }

            return result;
        }

        [ValidateInput(false)]
        public ActionResult Reload(BizDC.HTSData htsData)
        {
            ActionResult result = null;
            try
            {
                htsData.PendingXML = Server.HtmlDecode(htsData.ToXML());
                return UpdateXML(htsData);
            }
            catch (Exception e)
            {
                result = new HttpStatusCodeResult(500, string.Format("Fail to process Hts Data! {0} stack:{1}", e.Message, e.StackTrace));
            }

            return result;
        }

        [ValidateInput(false)]
        public ActionResult CheckQuestionIsSaved(BizDC.HTSData htsData)
        {
            ActionResult result = null;
            try
            {
                string questionIsSaved = "false";
                BizDC.HTSData pendingData;
                string interactionData;

                if (string.IsNullOrEmpty(htsData.ViewAsXml))
                {
                    interactionData = htsData.ToXML();
                    interactionData = HttpUtility.HtmlDecode(interactionData);
                }
                else
                {
                    UpdateXMLData(htsData, out pendingData, out interactionData);
                }

                interactionData = Regex.Replace(interactionData, @"<p>\s{1,}</p>", "<p />");

                PxBizDC.Question question = null;
                question = HtsServices.GetQuestion(htsData.EntityId, htsData.QuestionId);
                var questionInteractionData = Regex.Replace(question.InteractionData, @"<p>\s{1,}</p>", "<p />");
                if (string.Equals(questionInteractionData, interactionData, StringComparison.InvariantCultureIgnoreCase))
                {
                    questionIsSaved = "true";
                }

                result = Content(questionIsSaved);
            }
            catch (Exception e)
            {
                result = new HttpStatusCodeResult(500, string.Format("Some Error while checking question is saved! {0} stack:{1}", e.Message, e.StackTrace));
            }

            return result;
        }
    }
}
