using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;

namespace Macmillan.PXQBA.Common.Helpers
{
    public static class CustomQuestionHelper
    {
        private const string CQScriptString = "if(typeof CQ ==='undefined')CQ = window.parent.CQ; CQ.questionInfoList['{0}'] = {{ divId: '{1}', version: '{2}', mode: '{3}', question: {{ body: '{4}', data: {5}}}, response: {{ pointspossible: '{6}', pointsassigned: '{7}'}} }}";
                                                                                    
        public static string GetQuestionHtmlPreview(Question question)
        {

         
            if (String.IsNullOrEmpty(question.InteractionData))
            {
               //TODO: handle empty interaction data
                return "Something gone wrong";
            }

            if (question.InteractionType.ToLower() != "custom")
            {
                return "Not Custom Qeestion: to be implemented..";
            }

            if (question.CustomUrl == "HTS")
            {
                return GetHTSQuestionPreview(ConfigurationHelper.GetHTSConverterUrl(), question.InteractionData,
                    question.Id);
            }
            return GetFmaGraphQuestionPreview(ConfigurationHelper.GetHTSConverterUrl(), question.InteractionData,
                question.Id);
        }

        /// <summary>
        /// Get HTS Question Preview
        /// </summary>
        /// <param name="sUrl">BrainHoney Player URL</param>
        /// <param name="sXml">Interaction Data</param>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public static string GetHTSQuestionPreview(string sUrl, string sXml, string questionId)
        {
            bool isBlankQuestion = string.IsNullOrEmpty(sXml);
            string sResponse = "";
            sXml = isBlankQuestion ? sXml : HtmlXmlHelper.CleanupHtmlString(sXml);
            string sQuestionData = sXml;
            questionId = questionId.Replace("-", "_");
           
                try
                {
                    sXml = "<info id=\"0\" mode=\"PrintKey\"><question schema=\"2\" partial=\"false\"><answer /><body></body><customurl>HTS</customurl><interaction type=\"custom\"><data><![CDATA[" + sXml + "]]></data></interaction></question></info>";
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
                    string cqPatch = "<script type='text/javascript' src='/Scripts/quiz/CQ.js' />";
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
        public static string GetFmaGraphQuestionPreview(string sUrl, string sXml, string questionId)
        {
            string sResponse = "";
            string sQuestionData = sXml;
         
                try
                {
                    sXml = "<info id=\"0\" mode=\"Active\"><question schema=\"2\" partial=\"false\"><answer /><body></body><customurl>HTS</customurl><interaction type=\"custom\"><data><![CDATA["
                        + sXml + "]]></data></interaction></question></info>";
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

                    //sResponse = sResponse.Replace("[~]", "/BrainHoney/Resource/" + entityId);
                }
                catch (Exception ex2)
                {
                    sResponse = "There was an error: " + ex2;
                }

            
            return sResponse;
        }


        public static string GetGraphEditor(string customXML, string questionId, string questionType)
        {
            string sResponse = "";
            customXML = RemoveCData(HttpUtility.HtmlDecode(customXML));
            //customXML = HttpUtility.HtmlEncode(customXML);

            string sQuestionData = customXML;


            //TODO: implement staff
            string editorURL = ConfigurationHelper.GetFmaGraphEditorUrl();

                try
                {
                    customXML = "<info id=\"0\" mode=\"Review\"><question schema=\"2\" partial=\"false\"><answer /><body></body><customurl>HTS</customurl><interaction type=\"custom\"><data><![CDATA["
                        + customXML + "]]></data></interaction></question></info>";
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
                    sResponse = "There was an error: " + ex2.ToString();
                }

            

            return sResponse;
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
