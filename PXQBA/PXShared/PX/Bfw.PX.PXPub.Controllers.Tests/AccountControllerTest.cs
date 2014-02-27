using System;
using System.CodeDom;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.IO;
using System.Web.Routing;
using System.Web.Security;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class AccountControllerTest
    {
        private IBusinessContext context;
        private ISessionManager sessionManager;
        private ICacheProvider cacheProvider;
        private ISession session;
        private IUserActions userActions;
        private UserInfo userInfo;

        private AccountController acontroller;

        public AccountControllerTest()
        {
            context = Substitute.For<IBusinessContext>();
            sessionManager = Substitute.For<ISessionManager>();
            cacheProvider = Substitute.For<ICacheProvider>();
            session = Substitute.For<ISession>();
            userActions = Substitute.For<IUserActions>();
            userInfo = new UserInfo() {Id="9001", FirstName ="Shailesh", LastName = "Tripathi", Email = "shailesh.tripathi@macmillan.com", Username = "shailesh.tripathi.instructor@macmillan.com"};

            context.CacheProvider.Returns(cacheProvider);
            sessionManager.CurrentSession.Returns(session);
            context.CurrentUser = userInfo;

            acontroller = new AccountController(context, userActions, cacheProvider);

            InitializeControllerContext();
        }

        [TestMethod]
        public void LoginTest()
        {
            var account = new Account
            {
                Username = "shailesh.tripathi.instructor@macmillan.com",
                Password = "Password1"
            };

            var result = acontroller.Login(account) as ViewResult;

            //testing to see if method got executed. Due to framework not able to set cookie call can not fully succeed 
            Assert.AreEqual(account.Password, "");
        }

        [TestMethod]
        public void AuthenticationScriptsTest()
        {
            var result = acontroller.AuthenticationScripts() as ViewResult;

            Assert.AreNotSame(result.ViewData["rascripts"],"");
            Assert.AreNotSame(result.ViewData["racss"], "");

            //validating method got executed and returns empty view
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void UserAuthenticatedTests()
        {
            var result = acontroller.UserAuthenticated() as ContentResult;

            Assert.AreEqual(result.Content, "true");

        }

        [TestMethod]
        public void CurrentUserIdTest()
        {
            var result = acontroller.CurrentUserId() as ContentResult;
            Assert.IsNotNull(result.Content);
        }

        [TestMethod]
        public void IsInstructorTest()
        {
            var result = acontroller.IsInstructor() as ContentResult;

            Assert.IsNotNull(result.Content);
        }

        /// <summary>
        /// Logout Return URL should not have CourseId as part of URL, if includeCourseId is false
        /// </summary>
        [TestMethod, Ignore]
        public void Logout_ReturnURL_ShouldNotHaveCourseId()
        {
            //Arrange
            const string courseId = "126452";           
            
            //Act
            var result = (RedirectResult) acontroller.Logout();
            var includeCourseId = result.Url.Contains(courseId);

            //Assert
            Assert.AreEqual(includeCourseId, false);
        }

        /// <summary>
        /// Auth(rization) action method should return a URL with a CourseId
        /// </summary>
        [TestMethod, Ignore]
        public void Authorization_ReturnURL_ShouldHaveCourseId()
        {
            //Arrange
            const string courseId = "126452";

            //Act
            var result = (RedirectResult)acontroller.Auth(string.Empty, string.Empty);
            var includeCourseId = result.Url.Contains(courseId);

            //Assert
            Assert.AreEqual(includeCourseId, true);
        }

        /// <summary>
        /// Action should have properly formed path for redirect
        /// </summary>
        [TestMethod]
        public void Login_With_ReturnUrl_Should_Return_MARSPath_With_ReturnUrl()
        {
            context.IsAnonymous.Returns(true);

            var result = acontroller.Login("ReturnUrl");
            var redirectResult = (RedirectResult)result;

            Assert.AreEqual("http://dev.activation.macmillanhighered.com/account/logon?target=http://lcl.worthpublishers.com/xbook/languageofcomp2e/123456/Account/Auth%3FBaseUrl=ReturnUrl", redirectResult.Url);
        }

        private void InitializeControllerContext()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();

            var cookies = new HttpCookieCollection();

            var routeData = new RouteData();
            routeData.Values.Add("course", "languageofcomp2e");
            routeData.Values.Add("section", "xbook");
            routeData.Values.Add("courseid", "123456");
            
            requestContext.RouteData = routeData;

            acontroller.Url = new UrlHelper(requestContext, PopulateRoutes());

            request.Url.Returns(new Uri("http://lcl.worthpublishers.com/launchpad/myers10e/1/Dashboard"));
            httpContext.Request.Returns(request);
            request.Cookies.Returns(cookies);

            HttpContext.Current = new HttpContext(new HttpRequest("", "http://test.com", ""), new HttpResponse(new StringWriter()));

            acontroller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };
            acontroller.ControllerContext.RouteData.Returns(routeData);
        }

        private RouteCollection PopulateRoutes()
        {
            RouteCollection routes = new RouteCollection();

            routes.MapRoute(
                "CourseSectionHome",
                "{section}/{course}/{courseid}",
                new { controller = "Home", action = "Index", __px__routename = "CourseSectionHome" }
            );

            return routes;
        }
    }
}
