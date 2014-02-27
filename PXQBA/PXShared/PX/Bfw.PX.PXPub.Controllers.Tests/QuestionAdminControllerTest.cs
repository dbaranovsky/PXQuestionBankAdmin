using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Bfw.Common;
    using Bfw.Common.Collections;
    using Bfw.PX.Biz.ServiceContracts;
    using Bfw.PX.PXPub.Controllers.Mappers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using BizDC = Bfw.PX.Biz.DataContracts;
    using NSubstitute;

    /// <summary>
    /// WIP for testing Timeout condition for QBA and other PX issues.
    /// </summary>
    [TestClass]
    public class QuestionAdminControllerTest
    {
        #region "Local Variables"
        private QuestionAdminController _controller;
        private IBusinessContext _context;
        private IQuestionAdminActions _questionAdminActions;
        private ISearchActions _searchActions;
        private IContentActions _contentActions;
        private IQuestionActions questionActions;

        private INavigationActions _navActions;
        private IAssignmentActions _assignmentActions;
        private IGradeActions _gradeActions;
        private IResourceMapActions _resourceMapActions;        
        private INoteActions _noteActions;
        private IUserActivitiesActions _userActivitiesActions;

        private ContentHelper _contentHelper;

        private string mainEntityId { get; set; }
        private Dictionary<string, object> parameters { get; set; }

        private Dictionary<string, string> questionType
        {
            get
            {
                var settingString = "choice:Multiple Choice|text:Short Answer|essay:Essay|match:Matching|answer:Multiple Answer|HTS:Advanced Question|FMA_GRAPH:Graph Exercise";
                return settingString.Split('|').Select(type => type.Split(':')).ToDictionary(parts => parts[0], parts => parts[1]); 
            }
        }

        private readonly string BRAIN_HONEY_BROWSER_CHECK_COOKIE = "BHBrowserCheck";
        private readonly string BRAIN_HONEY_ACTIVE_DOMAIN_COOKIE_NAME = "bhdomain";
        private readonly string BRAIN_HONEY_COOKIE_NAME = "BHAUTH";

        enum EnvironmentTesting
        {
            DEV = 1,
            QA = 2,
            PR =3,
            PROD =4
        }

        private string trustHeaderUsername = string.Empty;

        #endregion

        public string TrustHeaderKey { protected get; set; }
        public string TrustHeaderUsername
        {
            get
            {
                return trustHeaderUsername;
            }
            set
            {
                int id = -1;
                if (int.TryParse(value, out id))
                {
                    trustHeaderUsername = value;
                }
                else
                {
                    var parts = value.Split('/');
                    trustHeaderUsername = string.Format("//{0}//{1}", parts[0], parts[1]);
                }
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _questionAdminActions = Substitute.For<IQuestionAdminActions>();
            _searchActions = Substitute.For<ISearchActions>();
            _contentActions = Substitute.For<IContentActions>();
            questionActions = Substitute.For<IQuestionActions>();
            
            _navActions = Substitute.For<INavigationActions>();
            _assignmentActions = Substitute.For<IAssignmentActions>(); 
            _gradeActions = Substitute.For<IGradeActions>();            
            _resourceMapActions = Substitute.For<IResourceMapActions>();            
            _noteActions = Substitute.For<INoteActions>();
            _userActivitiesActions = Substitute.For<IUserActivitiesActions>();
            //contentHelper = Substitute.For<ContentHelper>();

            _contentHelper = new ContentHelper(_context,_navActions,_contentActions,_assignmentActions,_gradeActions, _resourceMapActions,_noteActions,_userActivitiesActions);

            _controller = new QuestionAdminController(_context, _questionAdminActions, _searchActions, _contentActions, questionActions, _contentHelper);
            InitializeControllerContext();
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void MetaData_Information_For_ExcerciseNumber_Should_Be_Empty_If_Provided_Null()
        {
            var questionToUpdate = new BizDC.Question();
            var questionMetaData = new QuestionMetadata
            {
                ExcerciseNo = null
            };

            _controller.UpdateQuestion(questionMetaData, questionToUpdate);
            Assert.AreEqual(string.Empty, questionToUpdate.ExcerciseNo, "Exercise number not geting updated correctly.");
        }


        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void MetaData_Information_For_ExcerciseNumber_Updates_Correctly_If_Provided()
        {
            const string providedExcerciseNo = "Test";
            var questionToUpdate = new BizDC.Question();
            var questionMetaData = new QuestionMetadata
            {
                ExcerciseNo = providedExcerciseNo
            };

            _controller.UpdateQuestion(questionMetaData, questionToUpdate);
            Assert.AreEqual(providedExcerciseNo,questionToUpdate.ExcerciseNo,"Exercise number not geting updated correctly.");
        }

        [TestMethod]
        public void Dummy_Question_InQuestionEditorTab_If_QuestionIs_Empty_Returns_Error()
        {
            var questionId = "questionId";
            var mockQuizQuestionId = "mockquizquestionId";
            string changedQuestionId;
            var quizId = "quizId";

            var returnQuizId = questionActions.StoreMockQuiz(
                sourceEntityId: mainEntityId,
                destinationEntityId: mainEntityId,
                sourceQuestionId: questionId,
                mockQuizId: quizId,
                mockQuestionId: mockQuizQuestionId,
                changedQuestionId: out changedQuestionId).ReturnsForAnyArgs(quizId);

            questionActions.GetQuestion(mainEntityId, changedQuestionId).ReturnsForAnyArgs(a => null);

            var result = _controller.QuestionEditorTab("questionId", "quizId") as JsonResult;

            Assert.AreEqual("{ Status = error }", result.Data.ToString());
        }


        [TestMethod]
        public void Dummy_Question_InQuestionEditorTab_If_Question_Is_Not_Null()
        {
            var questionId = "questionId";
            var mockQuizQuestionId = "mockquizquestionId";
            string changedQuestionId;
            var quizId = "quizId";

            var returnQuizId = questionActions.StoreMockQuiz(
                sourceEntityId: mainEntityId,
                destinationEntityId: mainEntityId,
                sourceQuestionId: questionId,
                mockQuizId: quizId,
                mockQuestionId: mockQuizQuestionId,
                changedQuestionId: out changedQuestionId).ReturnsForAnyArgs(quizId);

            questionActions.GetQuestion(mainEntityId, changedQuestionId).ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Question
            {
                EntityId = mainEntityId,
                Id = questionId,
                CustomUrl = "choice"
            });

            var result = _controller.QuestionEditorTab("questionId", "quizId") as ViewResult;
            var qm = result.Model as QuestionEditor;

            Assert.AreEqual(mainEntityId, qm.EntityId);
        }


        #region "TestActualEnvironmentExecution With Dummy data"

        [TestMethod, Ignore]
        public void Check_Dev_Environment()
        {
            this.Check_BrainHoney_Component_Return_Success(EnvironmentTesting.DEV);
        }

        [TestMethod, Ignore]
        public void Check_QA_Environment()
        {
            this.Check_BrainHoney_Component_Return_Success(EnvironmentTesting.QA);
        }

        [TestMethod, Ignore]
        public void Check_PR_Environment()
        {
            this.Check_BrainHoney_Component_Return_Success(EnvironmentTesting.PR);
        }

        private void Check_BrainHoney_Component_Return_Success(EnvironmentTesting testing)
        {
            // Arrange
            string userdomain = "bfwproducts";
            string userAgent =
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.95 Safari/537.36";
            string brainhoneyAuthUrl;
            var cookieCollection = new CookieCollection();
            string domainName = string.Empty;
            if (testing == EnvironmentTesting.DEV)
            {
                domainName = this.SetDevEnvironment(userdomain, out brainhoneyAuthUrl, out cookieCollection);
            }
            if (testing == EnvironmentTesting.QA)
            {
                domainName = this.SetQAEnvironment(userdomain, out brainhoneyAuthUrl, out cookieCollection);
            }
            if (testing == EnvironmentTesting.PR)
            {
                domainName = this.SetPREnvironment(userdomain, out brainhoneyAuthUrl, out cookieCollection);
            }


            var uriBuilder = new UriBuilder
            {
                Scheme = "http",
                Host = domainName,
                Path = "/brainhoney/component/QuestionEditor",
                Query = this.BuildQuery()
            };

            HttpWebRequest webRequest = null;
            HttpWebResponse response = null;
            int timeout = 600000; // Set to 10 min.
            webRequest = WebRequest.Create(uriBuilder.Uri.ToString()) as HttpWebRequest;

            webRequest.UserAgent = userAgent;
            webRequest.ReadWriteTimeout = timeout;
            webRequest.AllowAutoRedirect = false;
            //webRequest.TransferEncoding = "gzip,deflate,sdch";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Proxy = null;

            webRequest.CookieContainer = new CookieContainer();
            webRequest.CookieContainer.Add(cookieCollection);

            var stopwatch = new Stopwatch();
            webRequest.Method = "GET";

            // Act

            stopwatch.Start();
            response = (HttpWebResponse)webRequest.GetResponse();


            Stream webData = response.GetResponseStream();

            stopwatch.Stop();
            Debug.WriteLine("Time Taken to run this command {0}", stopwatch.Elapsed.ToString());

            response.Close();
            webData.Dispose();


            // Assert
            Assert.AreEqual(System.Net.HttpStatusCode.OK, response.StatusCode, "Failed to get response in required timeout of 10 minutes");

        }

        #endregion

        #region "Private Methods"

        private string SetDevEnvironment(string userdomain, out string brainhoneyAuthUrl, out CookieCollection cookieCollection)
        {
            var enrollmentId = "127786";
            var itemId = "LOR_econportal__stoneecon2__master_QUIZ_428D17645886688F39373ADE1CAB0006";
            var questionId = "428D17645886688F39373ADE1CAB0010";
            var referer = @"http://dev.worthpublishers.com/launchpad/stoneecon2/57704/QuestionAdmin";
            var domainName = "dev.worthpublishers.com";
            brainhoneyAuthUrl = "http://{1}.bhdev.worthpublishers.com/BrainHoney/Controls/CredentialsUI.ashx";

            StartBrainhoneySession("powercoder@bfwpub.com", "fakepassword", brainhoneyAuthUrl, userdomain, domainName, TimeZoneInfo.Local, out cookieCollection);

            this.CreateUriParameters(enrollmentId, itemId, questionId, referer);
            return domainName;
        }
        
        private string SetQAEnvironment(string userdomain, out string brainhoneyAuthUrl, out CookieCollection cookieCollection)
        {
            var enrollmentId = "125509";
            var itemId = "LOR_statsportal__bps6e__master_QUIZ_D98FEDFA93744B9A82768AF1DB148820";
            var questionId = "09AE9F44D821497CA8251C35D9F43095";
            var referer = @"http://qa.whfreeman.com/launchpad/bps6e/61233/QuestionAdmin";
            var domainName = "qa.whfreeman.com";
            brainhoneyAuthUrl = "http://{1}.bhqa.whfreeman.com/BrainHoney/Controls/CredentialsUI.ashx";

            StartBrainhoneySession("powercoder@bfwpub.com", "fakepassword", brainhoneyAuthUrl, userdomain, domainName, TimeZoneInfo.Local, out cookieCollection);

            this.CreateUriParameters(enrollmentId, itemId, questionId, referer);
            return domainName;
        }

        private string SetPREnvironment(string userdomain, out string brainhoneyAuthUrl, out CookieCollection cookieCollection)
        {
            var enrollmentId = "83514";
            var itemId = "LOR_statsportal__bps6e__master_QUIZ_CF97394A4636424584AE723E1BB90FB1";
            var questionId = "F32FFB5D793A47D7ABCD1781AB7D2B7D";
            var referer = @"http://pr.whfreeman.com/launchpad/bps6e/56605/QuestionAdmin";
            var domainName = "pr.whfreeman.com";
            brainhoneyAuthUrl = "http://{1}.bhpr.whfreeman.com/BrainHoney/Controls/CredentialsUI.ashx";

            StartBrainhoneySession("instructor3@bfwpub.com", "fakepassword", brainhoneyAuthUrl, userdomain, domainName, TimeZoneInfo.Local, out cookieCollection);

            this.CreateUriParameters(enrollmentId, itemId, questionId, referer);
            return domainName;
        }

        private void CreateUriParameters(string enrollmentId, string itemId, string questionId, string referer)
        {
            this.parameters = new Dictionary<string, object>
            {
                { "Id", "quizeditorcomponent" },
                { "EnrollmentId", enrollmentId },
                { "ItemId", itemId },
                { "QuestionId", questionId },
                { "ShowAdvanced", "False" },
                { "ShowSave", "False" },
                { "ShowCancel", "False" },
                { "ShowProperties", "False" },
                { "ShowFeedback", "True" },
                { "showBeforeUnloadPrompts", "false" },
                { "xdm_e", referer },
                { "xdm_c", "default6685" },
                { "xdm_p", "4" }
            };
        }

        private void StartBrainhoneySession(string username, string password, string brainHoneyAuthUrl, string userDomain, string domainName, TimeZoneInfo timeZoneInfo, out CookieCollection cookieCollection)
        {
            cookieCollection = new CookieCollection();
            try
            {
                //var brainHoneyAuthUrl = "http://{1}.bhdev.whfreeman.com/BrainHoney/Controls/CredentialsUI.ashx";
                //var userDomain = "pxmigration";
                //var domain = "";
                var cookieJar = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);

                {
                    if (!username.Contains("/"))
                    {
                        username = userDomain + "/" + username;
                    }
                    else
                    {
                        var parts = username.Split('/');
                        userDomain = parts[0];
                    }

                    var uri = brainHoneyAuthUrl.Replace("{1}", userDomain);

                    var bhUri = new Uri(uri);
                    //var server = HttpContext.Current.Server;


                    var requestData = "action=login&username=" + HttpUtility.UrlEncode(username) + "&password=" + HttpUtility.UrlEncode(password);
                    if (timeZoneInfo != null && timeZoneInfo.GetAdjustment(DateTime.Now.Year) != null)
                    {
                        var adjustment = timeZoneInfo.GetAdjustment(DateTime.Now.Year);

                        requestData += "&standardOffset=" + -1 * timeZoneInfo.BaseUtcOffset.TotalMinutes +
                                       "&daylightOffset=" +
                                       -1 * (adjustment.DaylightDelta.TotalMinutes +
                                        timeZoneInfo.BaseUtcOffset.TotalMinutes) +
                                       "&standardStartTime=" +
                                       HttpUtility.UrlEncode(adjustment.DaylightTransitionEnd
                                                 .GetTransitionInfo(DateTime.Now.Year)
                                                 .ToUniversalTime()
                                                 .ToString("s") + "Z") +
                                       "&daylightStartTime=" +
                                       HttpUtility.UrlEncode(adjustment.DaylightTransitionStart
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
                        var cookieName = c.Name;
                        if (cookieName.ToLowerInvariant() == BRAIN_HONEY_COOKIE_NAME.ToLowerInvariant() ||
                            cookieName.ToLowerInvariant() == BRAIN_HONEY_BROWSER_CHECK_COOKIE.ToLowerInvariant())
                        {
                            var cookie = new System.Net.Cookie()
                            {
                                Name = cookieName,
                                Path = c.Path,
                                Value = c.Value,
                                HttpOnly = c.HttpOnly,
                                Secure = c.Secure,
                                Expires = c.Expires
                            };

                            var activeDomainCookie = new System.Net.Cookie()
                            {
                                Name = BRAIN_HONEY_ACTIVE_DOMAIN_COOKIE_NAME,
                                Path = c.Path,
                                Value = userDomain,
                                HttpOnly = c.HttpOnly,
                                Secure = c.Secure,
                                Expires = c.Expires
                            };

                            if (!string.IsNullOrEmpty(domainName))
                            {
                                cookie.Domain = domainName;
                                activeDomainCookie.Domain = domainName;
                            }

                            cookieCollection.Add(cookie);
                            cookieCollection.Add(activeDomainCookie);
                            //HttpContext.Current.Response.Cookies.Add(cookie);
                            //HttpContext.Current.Response.Cookies.Add(activeDomainCookie);
                        }
                    }

                    /*FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, "6668572", DateTime.Now, DateTime.Now.AddMinutes(30), true, null, FormsAuthentication.FormsCookiePath);
                    string encTicket = FormsAuthentication.Encrypt(ticket);
                    System.Web.HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                    if (!string.IsNullOrEmpty(ticket.Name)) //ticket.Expired && 
                    {
                        //webRequest.CookieContainer.Add(webRequest.RequestUri, new Cookie() { Name = ".ASPXAUTH", Value = userId });
                        //webRequest.CookieContainer.Add(webRequest.RequestUri, new Cookie() { Name = ".ASPXAUTH", Value = webRequest. });
                        try
                        {
                            System.Web.Security.FormsAuthentication.SetAuthCookie(ticket.Name, false, "/");
                        }
                        catch (Exception exception)
                        { }
                        var cookie1 = System.Web.HttpContext.Current.Response.Cookies["pxauth"];

                        var newCookie = new System.Net.Cookie()
                        {
                            Name = "pxauth",
                            Path = "/"
                        };
                        if (cookie1 != null)
                        {
                            newCookie.Value = cookie1.Value;
                            newCookie.Expires = cookie1.Expires;
                        }

                        if (!string.IsNullOrEmpty(domainName))
                        {
                            newCookie.Domain = domainName;
                        }
                        //System.Web.HttpContext.Current.Request.Cookies.Set(newCookie);
                        cookieCollection.Add(newCookie);
                    }
                    

                    response.Close();*/
                }
            }
            catch (Exception exception)
            {

            }
        }

        private string BuildQuery()
        {
            string query = string.Empty;

            if (!parameters.IsNullOrEmpty())
            {
                query = parameters.Fold("&", p => string.Format("{0}={1}", HttpUtility.UrlEncode(p.Key), HttpUtility.UrlEncode(p.Value.ToString())));
            }

            return query;
        }

        private void AddTrustHeader(HttpWebRequest request)
        {
            var date = System.Xml.XmlConvert.ToString(DateTime.UtcNow, System.Xml.XmlDateTimeSerializationMode.Utc);
            var hash = ComputeHash(TrustHeaderUsername, date, TrustHeaderKey);
            var headerValue = string.Format("userid={0}&timestamp={1}&hash={2}", HttpUtility.UrlEncode(TrustHeaderUsername), HttpUtility.UrlEncode(date), HttpUtility.UrlEncode(hash));

            request.Headers.Add("DlapUserId", headerValue);
        }

        private string ComputeHash(string user, string date, string key)
        {
            var encoder = new UTF8Encoding(false);
            var hashData = string.Format("{0}{1}", user, date);
            var utf8Bytes = encoder.GetBytes(hashData);
            var sha1 = System.Security.Cryptography.HMACSHA1.Create();

            sha1.Key = encoder.GetBytes(key);
            var hash = sha1.ComputeHash(utf8Bytes);

            return Convert.ToBase64String(hash);
        }

        private void InitializeControllerContext()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();

            var routeData = new RouteData();
            requestContext.RouteData = routeData;
            var routeCollection = this.PopulateRoutes();

            _controller.Url = new UrlHelper(requestContext, routeCollection);

            request.Url.Returns(new Uri("http://lcl.whfreeman.com/launchpad/bps6e/1/QuestionAdmin"));
            httpContext.Request.Returns(request);

            mainEntityId = "disciplineEntityId";
            _controller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };
            _controller.ControllerContext.RouteData.Returns(routeData);
        }

        private RouteCollection PopulateRoutes()
        {
            RouteCollection routes = new RouteCollection();

            routes.MapRoute(
                "QuestionAdminEditor",
                "{section}/{course}/{courseid}/QuestionAdmin/Editor/{id}",
                new { controller = "QuestionAdmin", action = "QuestionEditor", id = UrlParameter.Optional, __px__routename = "QuestionAdminEditor" }
            );

           /* routes.MapRoute(
                "QuestionAdminNewQuestion",
                "{section}/{course}/{courseid}/QuestionAdmin/NewQuestion/{questionType}/{quizId}",
                new { controller = "QuestionAdmin", action = "AddNewQuestion", questionType = UrlParameter.Optional, quizId = UrlParameter.Optional, __px__routename = "QuestionAdminNewQuestion" }
            );*/
            
            routes.MapRoute(
                "CourseSectionHome",
                "{section}/{course}/{courseid}",
                new { controller = "Home", action = "Index", __px__routename = "CourseSectionHome" }
            );

            return routes;
        }

        #endregion
    }
}
