using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Bfw.Common;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Helpers;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IQuestionMetadataService questionMetadataService;
        public void StartBrainHoneySession(string username, string password, TimeZoneInfo timeZoneInfo)
        {

            try
            {
                // var config = ConfigurationManager.GetSection("agilixSessionManager") as Configuration.SessionManagerSection;
                var brainHoneyAuthUrl = "http://root.dev.brainhoney.bfwpub.com/BrainHoney/Controls/CredentialsUI.ashx";
                var userDomain = "root";
                var domain = "";
                var BRAIN_HONEY_COOKIE_NAME = "BHAUTH";
                var BRAIN_HONEY_BROWSER_CHECK_COOKIE = "BHBrowserCheck";
                var BRAIN_HONEY_ACTIVE_DOMAIN_COOKIE_NAME = "bhdomain";
                var cookieJar = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);


                //if (!username.Contains("/"))
                //{
                //    username = userDomain + "/" + username;
                //}
                //else
                //{
                //    var parts = username.Split('/');
                //    userDomain = parts[0];
                //}

                var uri = brainHoneyAuthUrl;


                var bhUri = new Uri(uri);
                var server = System.Web.HttpContext.Current.Server;


                var requestData = "action=login&username=" + server.UrlEncode(username) + "&password=" + server.UrlEncode(password);
                if (timeZoneInfo != null && timeZoneInfo.GetAdjustment(DateTime.Now.Year) != null)
                {
                    var adjustment = timeZoneInfo.GetAdjustment(DateTime.Now.Year);

                    requestData += "&standardOffset=" + -1 * timeZoneInfo.BaseUtcOffset.TotalMinutes +
                                   "&daylightOffset=" +
                                   -1 * (adjustment.DaylightDelta.TotalMinutes +
                                    timeZoneInfo.BaseUtcOffset.TotalMinutes) +
                                   "&standardStartTime=" +
                                   server.UrlEncode(adjustment.DaylightTransitionEnd
                                             .GetTransitionInfo(DateTime.Now.Year)
                                             .ToUniversalTime()
                                             .ToString("s") + "Z") +
                                   "&daylightStartTime=" +
                                   server.UrlEncode(adjustment.DaylightTransitionStart
                                             .GetTransitionInfo(DateTime.Now.Year)
                                             .ToUniversalTime()
                                             .ToString("s") + "Z");

                }
                else
                {
                    requestData += "&standardOffset=" + -1 * timeZoneInfo.BaseUtcOffset.TotalMinutes +
                                   "&daylightOffset=" + timeZoneInfo.BaseUtcOffset.TotalMinutes;
                }

                cookieJar.Add(new Cookie("BHBrowserCheck", "1", "/", bhUri.Host));

                var webRequest = (HttpWebRequest)WebRequest.Create(bhUri);

                webRequest.CookieContainer = cookieJar;
                webRequest.Method = "POST";
                webRequest.Accept = "*/*";
                webRequest.ContentLength = requestData.Length;
                webRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";

                var byteData = Encoding.UTF8.GetBytes(requestData);
                using (var postStream = webRequest.GetRequestStream())
                {
                    postStream.Write(byteData, 0, requestData.Length);
                }

                var response = webRequest.GetResponse();
                foreach (Cookie c in cookieJar.GetCookies(webRequest.RequestUri))
                {
                    if (c.Name.ToLowerInvariant() == BRAIN_HONEY_COOKIE_NAME.ToLowerInvariant() ||
                        c.Name.ToLowerInvariant() == BRAIN_HONEY_BROWSER_CHECK_COOKIE.ToLowerInvariant())
                    {
                        var cookie = new HttpCookie(c.Name)
                        {
                            Path = c.Path,
                            Value = c.Value,
                            HttpOnly = c.HttpOnly,
                            Secure = c.Secure,
                            Expires = c.Expires
                        };

                        var activeDomainCookie = new HttpCookie(BRAIN_HONEY_ACTIVE_DOMAIN_COOKIE_NAME)
                        {
                            Path = c.Path,
                            Value = userDomain,
                            HttpOnly = c.HttpOnly,
                            Secure = c.Secure,
                            Expires = c.Expires
                        };

                        if (!string.IsNullOrEmpty(domain))
                        {
                            cookie.Domain = domain;
                            activeDomainCookie.Domain = domain;
                        }

                        System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
                        System.Web.HttpContext.Current.Response.Cookies.Add(activeDomainCookie);
                    }
                }

                response.Close();

            }
            catch (Exception ex)
            {

            }

        }
        public QuestionController(IQuestionManagementService questionManagementService,   IQuestionMetadataService questionMetadataService)
        {
            this.questionManagementService = questionManagementService;
            this.questionMetadataService = questionMetadataService;
        }

        [HttpPost]
        public ActionResult Edit(string questionId, string fieldName, string fieldValue)
        {
            bool success = false;
            if (fieldName.Equals(MetadataFieldNames.Sequence))
            {
                questionManagementService.UpdateQuestionSequence(CourseHelper.CurrentCourse, questionId, int.Parse(fieldValue));
                success = true;
            }
            else
            {
                success = questionManagementService.UpdateQuestionField(questionId, fieldName, fieldValue);
            }
            return JsonCamel(new { isError = !success });
        
        }

        public ActionResult CreateQuestion(int questionType, string bank, string chapter)
        {
            var question = Mapper.Map<Question, QuestionViewModel>(questionManagementService.CreateQuestion(CourseHelper.CurrentCourse, (QuestionType)questionType, bank, chapter));
            question.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), "200117", "AHWDG");
            question.EditorUrl = String.Format(ConfigurationHelper.GetEditorUrlTemplate(), "200117", "AHWDG", "12c19f3103ad4da1b254dd67f17dd1b1");
            return JsonCamel(question);
            
        }

        [HttpPost]
        public ActionResult DuplicateQuestion(string questionId)
        {

            var question = Mapper.Map<Question, QuestionViewModel>(questionManagementService.DuplicateQuestion(CourseHelper.CurrentCourse, questionId));
            question.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), "200117", "AHWDG");
            question.EditorUrl = String.Format(ConfigurationHelper.GetEditorUrlTemplate(), "200117", "AHWDG", "12c19f3103ad4da1b254dd67f17dd1b1");
            return JsonCamel(question);

        }

        public ActionResult GetAvailibleMetadata()
        {
            return JsonCamel(questionMetadataService.GetAvailableFields(CourseHelper.CurrentCourse).Select(MetadataFieldsHelper.Convert).ToList());
        }

        public ActionResult UpdateQuestion(Question question)
        {
            questionManagementService.UpdateQuestion(question);
            return JsonCamel(new { isError = false });
        }

        public ActionResult GetQuestion(string questionId)
        {
            var question = Mapper.Map<Question, QuestionViewModel>(questionManagementService.GetQuestion(questionId));
            question.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), "200117", "AHWDG");
            question.EditorUrl = String.Format(ConfigurationHelper.GetEditorUrlTemplate(), "200117", "AHWDG", "12c19f3103ad4da1b254dd67f17dd1b1");
            StartBrainHoneySession("root/administrator", "Password1", TimeZoneInfo.Local);
            return JsonCamel(question);
        }
	}
}