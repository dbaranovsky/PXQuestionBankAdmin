using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class QuestionPreviewHelper
    {
        private const string CQScriptString = "if(typeof CQ ==='undefined')CQ = window.parent.CQ; CQ.questionInfoList['{0}'] = {{ divId: '{1}', version: '{2}', mode: '{3}', question: {{ body: '{4}', data: {5}}}, response: {{ pointspossible: '{6}', pointsassigned: '{7}'}} }}";
        private const string customXmlPattern = "<info id=\"0\" mode=\"{0}\"><question schema=\"2\" partial=\"false\"><answer /><body></body><customurl>HTS</customurl><interaction type=\"custom\"><data><![CDATA[{1}]]></data></interaction></question></info>";
        private static readonly Regex regexConverter = new Regex(@"\s+");

        /// <summary>
        /// Gets question html preview
        /// </summary>
        /// <param name="question">Question to render preview for</param>
        /// <returns>Preview string</returns>
        public static string GetQuestionHtmlPreview(Question question)
        {

            var html = new StringBuilder(String.Format(@"<div class=""{0}"">", question.CustomUrl == QuestionTypeHelper.GraphType ? "question-preview  graph" : "question-preview"));
            html.AppendFormat(@"<div class=""question-body"">{0}</div>", question.CustomUrl == QuestionTypeHelper.GraphType && string.IsNullOrEmpty(question.Body)? question.Title : question.Body);


            switch (question.InteractionType)
            {
                case "custom":
                    html.Append(RenderCustomQuestion(question));
                    break;

                case "answer":
                    html.Append(RenderMultiAnswer(question.AnswerList, question.Choices));
                    break;

                case "choice":
                    html.Append(RenderMultiChoice(question.Choices, question.Answer));
                    break;

                case "text":
                    html.Append(RenderShortAnswer(question.AnswerList, question.Answer));
                    break;

                case "match":
                    html.Append(RenderMatching(question.Choices));
                    break;

            }

            html.Append("</div>");

            return  html.ToString();    
        }

        /// <summary>
        /// Updates image urls if any in question preview
        /// </summary>
        /// <param name="html">Question preview</param>
        /// <param name="resourceCourseId">Resource course id</param>
        /// <param name="interactionType">Question interaction type</param>
        /// <returns></returns>
        public static string UpdateImagesUrls(string html,string resourceCourseId, string interactionType)
        {
            return interactionType == "custom" ? html : html.Replace(@"src=""[~]/", string.Format(@"src=""{0}/{1}/[~]/", "/brainhoney/Resource", resourceCourseId));
        }


        private static string RenderMatching(IEnumerable<QuestionChoice> choices)
        {
            var html = new StringBuilder();
            if (choices != null && choices.Any())
            {

                html.Append("<ul>");
                foreach (var choice in choices)
                {
                    html.Append("<li>");
                    html.AppendFormat(@"<span class=""option-text"">{0} = {1}</span >", choice.Text, choice.Answer);
                    html.Append("</li>");
                }
                html.Append("</ul>");
            }
            else
            {
                html.Append("<p>This question has no options.</p>");
            }
            return html.ToString();
        }

        private static string RenderShortAnswer(IEnumerable<string> answerList, string answer)
        {
            var html = new StringBuilder();
            if (answerList != null && answerList.Count() > 1)
            {
                html.Append("<b>Correct answers:</b>");

            }
            else
            {
                html.Append("<b>Correct answer:</b>");
                html.AppendFormat("<p>{0}</p>", string.IsNullOrEmpty(answer) ? "No answer provided" : answer);
            }

            return html.ToString();
        }

        private static string RenderMultiChoice(IEnumerable<QuestionChoice> choices, string answer)
        {
            var html = new StringBuilder();
            if (choices != null && choices.Any())
            {
                string answerValue = !string.IsNullOrEmpty(answer) ? regexConverter.Replace(answer, "").ToLower() : "No answer provided";
                html.Append("<ul>");
                foreach (var choice in choices)
                {
                    string choiceValue = regexConverter.Replace(choice.Text, "").ToLower();
                    string chioceId = regexConverter.Replace(choice.Id, "").ToLower();

                    html.Append("<li>");

                    // Condition 'answerValue == choiceValue' leads to incorrect work. It is not clear what purpose it was added to the code.

                    //html.AppendFormat(@"<input disabled=""disabled"" type=""radio"" {0} />", (answerValue == choiceValue || answerValue == chioceId) ? "checked='checked'" : "");
                  
                    html.AppendFormat(@"<input disabled=""disabled"" type=""radio"" {0} />",  answerValue == chioceId ? "checked='checked'" : "");
                    html.AppendFormat(@"<span class=""option-text"">{0}</span >", choice.Text);
                    html.Append("</li>");
                }
                html.Append("</ul>");
            }
            else
            {
                html.Append("<p>This question has no options.</p>");
            }
            return html.ToString();
        }

        private static string RenderMultiAnswer(IEnumerable<string> answerList, IEnumerable<QuestionChoice> choices)
        {
            var html = new StringBuilder();
            if (choices != null && choices.Any())
            {
               
                html.Append("<ul>");
                foreach (var choice in choices)
                {
                    string cid = regexConverter.Replace(choice.Id, "");
                    string answerChecked = answerList != null && answerList.Contains(cid) ? "checked=checked" : "";

                    html.Append("<li>");
                    html.AppendFormat(@"<input disabled=""disabled"" type=""checkbox"" {0} />", answerChecked);
                    html.AppendFormat(@"<span class=""option-text"">{0}</span >", choice.Text);
                    html.Append("</li>");
                }
                html.Append("</ul>");
            }
            else
            {
                html.Append("<p>This question has no options.</p>");
            }
            return html.ToString();
        }

        private static string RenderCustomQuestion(Question question)
        {

            if (String.IsNullOrEmpty(question.InteractionData))
            {
                //TODO: handle empty interaction data
                return "Body is empty, no preview available";
            }


            if (question.CustomUrl == QuestionTypeHelper.HTSType)
            {
                return GetHTSQuestionPreview(ConfigurationHelper.GetHTSConverterUrl(), question.InteractionData,
                    question.Id);
            }
            return GetFmaGraphQuestionPreview(ConfigurationHelper.GetFmaGraphConverterUrl(), question.InteractionData,
                question.Id);
        }

        /// <summary>
        /// Get HTS Question Preview
        /// </summary>
        /// <param name="sUrl">BrainHoney Player URL</param>
        /// <param name="sXml">Interaction Data</param>
        /// <param name="questionId">Question id</param>
        /// <returns></returns>
        private static string GetHTSQuestionPreview(string sUrl, string sXml, string questionId)
        {
            bool isBlankQuestion = string.IsNullOrEmpty(sXml);
            string sResponse = "";
            sXml = isBlankQuestion ? sXml : HtmlXmlHelper.CleanupHtmlString(sXml);
            string sQuestionData = sXml;
            questionId = questionId.Replace("-", "_");
           
                try
                {
                    sXml = String.Format(customXmlPattern, "PrintKey", sXml);
                    byte[] buffer = Encoding.UTF8.GetBytes(sXml);

                    var webReq = WebRequest.Create(sUrl);

                    webReq.Method = "POST";
                    webReq.ContentLength = buffer.Length;
                    Stream reqStream = webReq.GetRequestStream();
                    reqStream.Write(buffer, 0, buffer.Length);
                    reqStream.Close();
                    using (WebResponse webRes = webReq.GetResponse())
                    {
                        using (var resReader = new StreamReader(webRes.GetResponseStream()))
                        {
                            sResponse = resReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    sResponse = ex.ToString();
                }

                try
                {
                    //parse xml response and grab custom question service markup to inject
                    XDocument myXml = XDocument.Parse(sResponse);
                    sResponse = myXml.Element("custom").Element("body").Value; // CDATA property loaded as text.

                    //replace token strings with actual question IDs, this allows for unique JS events
                    sResponse = sResponse.Replace("$QUESTIONID$", questionId); // just use placeholder value.

                    //add questionInfo object to client side array
                    string divId = "custom_preview_" + questionId;
                    string version = "1";
                    string mode = "Review";
                    string body = "";
                    var ser = new JavaScriptSerializer();
                    string data = ser.Serialize(sQuestionData);
                    string pointsPossible = "1";
                    string pointsAssigned = "NaN";

                    string cqScript = string.Format(
                        CQScriptString,
                        questionId,
                        divId,
                        version,
                        mode,
                        body,
                        data,
                        pointsPossible,
                        pointsAssigned);

                    cqScript = string.Format("<script type='text/javascript'>{0}</script>", cqScript);

                    sResponse = cqScript + sResponse;

                    //=== required if rendering question in iframe          
                    string cqPatch = "<script type='text/javascript' src='/Scripts/customQuestions/CQ.js' />";
                    sResponse = cqPatch + sResponse;
                    //===

                    //CQ questionIds are assumed to be numeric, PX allows alphanumeric so fix script.
                    string questionIdNumeric = "(" + questionId;
                    string questionIdAlpha = "('" + questionId + "'";
                    sResponse = sResponse.Replace(questionIdNumeric, questionIdAlpha);

                    //if question data exist start render else inject a default message
                    if (sResponse.Length == 0 || isBlankQuestion)
                    {
                        sResponse = "<br /><div id=\"info.divId\">The question does not contain any data.</div>";
                    }
                    else
                    {
                        sResponse = "<br /><div id=\"info.divId\">Generating preview...</div>" + sResponse;
                    }
                    sResponse = sResponse.Replace("getElementById(info.divId)", "getElementById('info.divId')");
                    sResponse = sResponse.Replace("info.divId", "info.divId" + questionId);

                    //sResponse = sResponse.Replace("[~]", "/BrainHoney/Resource/" + Context.EntityId);
                }
                catch (Exception ex2)
                {
                    sResponse = "There was an error: " + ex2.ToString();
                }
            
            return sResponse;

        }

        /// <summary>
        /// Get Graph Question Preview
        /// </summary>
        /// <param name="sUrl"></param>
        /// <param name="sXml">Interaction Data</param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        private static string GetFmaGraphQuestionPreview(string sUrl, string sXml, string questionId)
        {
            string sResponse = "";
            string sQuestionData = sXml;
            //for test purpose: version history preview failing, when preview on list collapsed
            questionId = Guid.NewGuid().ToString();
                try
                {
                    sXml = String.Format(customXmlPattern, "Active", sXml); 
                    byte[] buffer = Encoding.UTF8.GetBytes(sXml);

                    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(sUrl);

                    webReq.Method = "POST";
                    webReq.ContentLength = buffer.Length;
                    Stream reqStream = webReq.GetRequestStream();
                    reqStream.Write(buffer, 0, buffer.Length);
                    reqStream.Close();

                    using (WebResponse webRes = webReq.GetResponse())
                    {
                        using (StreamReader resReader = new StreamReader(webRes.GetResponseStream()))
                        {
                            sResponse = resReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    sResponse = ex.ToString();
                }

                try
                {
                    questionId = questionId.Replace("-", "_");
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    XDocument myXml = XDocument.Parse(sResponse);
                    sResponse = myXml.Element("custom").Element("body").Value; // CDATA property loaded as text.
                    sResponse = sResponse.Replace("$QUESTIONID$", questionId); // just use placeholder value.

                    string divId = "custom_preview_" + questionId;
                    string version = "1";
                    string mode = "Active";
                    string body = "";
                    string data = ser.Serialize(sQuestionData);
                    string pointsPossible = "1";
                    string pointsAssigned = "NaN";

                    string cqScript = string.Format(
                        CQScriptString,
                        questionId,
                        divId,
                        version,
                        mode,
                        body,
                        data,
                        pointsPossible,
                        pointsAssigned);

                    cqScript = string.Format("<script type='text/javascript'>{0}</script>", cqScript);

                    sResponse = cqScript + sResponse;

                    //CQ questionIds are assumed to be numeric, PX allows alphanumeric so fix script.
                    string questionIdNumeric = "(" + questionId;
                    string questionIdAlpha = "('" + questionId + "'";
                    sResponse = sResponse.Replace(questionIdNumeric, questionIdAlpha);

                    if (sResponse.Length == 0)
                    {
                        sResponse = "<br /><div id=\"questionInfo.divId\">The question does not contain any data.</div>";
                    }
                    else
                    {
                        sResponse = "<br /><div id=\"questionInfo.divId\">Generating graph preview...</div>" + sResponse;
                    }
                    sResponse = sResponse.Replace("getElementById(questionInfo.divId)", "getElementById('questionInfo.divId')");
                    sResponse = sResponse.Replace("questionInfo.divId", "questionInfo.divId" + questionId);
                    sResponse = sResponse.Replace("saveMethod" + questionId, "saveMethod");
                    //sResponse = sResponse.Replace("[~]", "/BrainHoney/Resource/" + entityId);
                }
                catch (Exception ex2)
                {
                    sResponse = "There was an error: " + ex2;
                }

            
            return sResponse;
        }


        /// <summary>
        /// Gets graph question editor
        /// </summary>
        /// <param name="customXML"></param>
        /// <param name="questionId"></param>
        /// <param name="questionType"></param>
        /// <returns></returns>
        public static string GetGraphEditor(string customXML, string questionId, string questionType)
        {

            if (string.IsNullOrEmpty(questionType) || questionType != QuestionTypeHelper.GraphType)
            {
                return null;
            }

            string sResponse = "";
            customXML = RemoveCData(HttpUtility.HtmlDecode(customXML));
            //customXML = HttpUtility.HtmlEncode(customXML);

            string sQuestionData = customXML;


            //TODO: implement staff
            string editorURL = ConfigurationHelper.GetFmaGraphEditorUrl();

                try
                {
                    customXML = String.Format(customXmlPattern, "Review", customXML);
                    byte[] buffer = Encoding.UTF8.GetBytes(customXML);

                    HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(editorURL);

                    WebReq.Method = "POST";
                    WebReq.ContentLength = buffer.Length;
                    Stream ReqStream = WebReq.GetRequestStream();
                    ReqStream.Write(buffer, 0, buffer.Length);
                    ReqStream.Close();
                    using (WebResponse WebRes = WebReq.GetResponse())
                    {
                        using (StreamReader ResReader = new StreamReader(WebRes.GetResponseStream()))
                        {
                            sResponse = ResReader.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    sResponse = ex.ToString();
                }

                try
                {
                    JavaScriptSerializer ser = new JavaScriptSerializer();
                    XDocument myXml = XDocument.Parse(sResponse);

                    questionId = questionId.Replace("-", "_");

                    sResponse = myXml.Element("custom").Element("body").Value; // CDATA property loaded as text.
                    sResponse = sResponse.Replace("$QUESTIONID$", questionId); // just use placeholder value.

                    string divId = "custom_preview_" + questionId;
                    string version = "1";
                    string mode = "editing";
                    string body = "";
                    string data = ser.Serialize(sQuestionData);
                    string pointsPossible = "1";
                    string pointsAssigned = "NaN";


                    string cqScript = string.Format(
                        CQScriptString,
                        questionId,
                        divId,
                        version,
                        mode,
                        body,
                        data,
                        pointsPossible,
                        pointsAssigned);

                    cqScript = string.Format("<script type='text/javascript'>{0}</script>", cqScript);

                    sResponse = cqScript + sResponse;

                    //CQ questionIds are assumed to be numeric, PX allows alphanumeric so fix script.
                    string questionIdNumeric = "(" + questionId;
                    string questionIdAlpha = "('" + questionId + "'";
                    sResponse = sResponse.Replace(questionIdNumeric, questionIdAlpha);

                    if (sResponse.Length == 0)
                    {
                        sResponse = "<br /><div id=\"questionInfo.divId\">The question does not contain any data.</div>";
                    }
                    else
                    {
                        sResponse = "<br /><div id=\"questionInfo.divId\">Generating graph preview...</div>" + sResponse;
                    }
                    sResponse = sResponse.Replace("getElementById(questionInfo.divId)", "getElementById('questionInfo.divId')");
                    sResponse = sResponse.Replace("questionInfo.divId", "questionInfo.divId" + questionId + "_edit");

                    //sResponse = sResponse.Replace("[~]", "/BrainHoney/Resource/" + entityId);
                }
                catch (Exception ex2)
                {
                    sResponse = "There was an error: " + ex2;
                }

            

            return sResponse;
        }



        /// <summary>
        /// Gets question editor url
        /// </summary>
        /// <param name="questionType">Question type</param>
        /// <param name="questionId">Question id</param>
        /// <param name="entityId">Repository course id</param>
        /// <param name="quizId">Quiz id</param>
        /// <returns>Editor url</returns>
        public static string GetEditorUrl(string questionType, string questionId, string entityId, string quizId)
        {
            return questionType == QuestionTypeHelper.HTSType ? 
                    String.Format(ConfigurationHelper.GetHTSEditorUrlTemplate(), questionId, quizId, entityId) : 
                    String.Format(ConfigurationHelper.GetEditorUrlTemplate(), entityId, quizId, questionId);
        }

        private static string RemoveCData(string sXML)
        {
            string outputXML = "";

            if (!string.IsNullOrEmpty(sXML))
            {
                XDocument doc = XDocument.Parse(sXML);
                var cDataNodes = doc.DescendantNodes().OfType<XCData>().ToArray();
                foreach (var cDataNode in cDataNodes)
                    cDataNode.ReplaceWith(new XText(cDataNode));

                outputXML = doc.ToString();
            }

            return outputXML;
        }
    }
}
