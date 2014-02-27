using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Services.Mappers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestHelper;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class BrowseMoreResourcesWidgetControllerTest
    {
        private BrowseMoreResourcesWidgetController controller;

        private IBusinessContext context;
        private IContentActions contentActions;
        private ISearchActions searchActions;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            contentActions = Substitute.For<IContentActions>();
            searchActions = Substitute.For<ISearchActions>();

            var course = new Bfw.Agilix.DataContracts.Course();
            course.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            course.Id = "1";
            context.Course = course.ToCourse();

            controller = new BrowseMoreResourcesWidgetController(context, contentActions, searchActions);

            InitializeControllerContext();
        }

        [TestMethod]
        public void FacePlateBrowseResourcesStringSearch_Title_Should_Not_Have_Junk()
        {
            var includeWords = "Instructor%27s Resources";
            var start = 0;
            var rows = 100;

            SearchQuery query = new SearchQuery();
            query.IncludeWords = HttpUtility.UrlDecode(includeWords);
            query.Start = start;
            query.Rows = rows;
            query.ClassType = string.Empty;

            searchActions.DoSearch(query, null, true).ReturnsForAnyArgs(new SearchResultSet());

            var result = controller.FacePlateBrowseResourcesStringSearch(includeWords, start, rows);
            var viewResult = result as ViewResult;

            Assert.AreEqual("Search Results for Instructor's Resources", viewResult.ViewData["Title"].ToString());
        }

        private void InitializeControllerContext()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();

            var routeData = new RouteData();
            requestContext.RouteData = routeData;

            request.Url.Returns(new Uri("http://lcl.worthpublishers.com/launchpad/myers10e/1/Dashboard"));
            httpContext.Request.Returns(request);

            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };
            controller.ControllerContext.RouteData.Returns(routeData);

            var cookies = new HttpCookieCollection();
            cookies.Add(new HttpCookie(context.Course.Id + "_currentChapterId", "1"));
            controller.Request.Cookies.ReturnsForAnyArgs(cookies);
        }
    }
}
