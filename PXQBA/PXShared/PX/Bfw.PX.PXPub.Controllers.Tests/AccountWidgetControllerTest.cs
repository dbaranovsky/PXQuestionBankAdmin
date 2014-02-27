using System.Collections.Generic;
using System.Configuration;
using System.Web.Routing;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using TestHelper;
using Bfw.PX.PXPub.Controllers.Contracts;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class AccountWidgetControllerTest
    {
        private AccountWidgetController controller;

        private IBusinessContext Context;
        private IUserActions UserActions;
        private IEnrollmentActions EnrollmentActions;
        private IPageActions PageActions;
        private ICourseActions CourseActions;        
        private ICourseHelper CourseHelper;
        private ICacheProvider CacheProvider;
        private IUrlHelperWrapper urlHelper;
        private IServiceLocator serviceLocator;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = Substitute.For<IBusinessContext>();
            PageActions = Substitute.For<IPageActions>();
            CourseActions = Substitute.For<ICourseActions>();
            UserActions = Substitute.For<IUserActions>();
            EnrollmentActions = Substitute.For<IEnrollmentActions>();            
            CacheProvider = Substitute.For<ICacheProvider>();
            CourseHelper = Substitute.For<ICourseHelper>();
            urlHelper = Substitute.For<IUrlHelperWrapper>();
            serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<IUrlHelperWrapper>().Returns(urlHelper);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            var agxCourse = new Bfw.Agilix.DataContracts.Course();
            agxCourse.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));

            Context.Course = agxCourse.ToCourse();
            Context.Course.SubType = "abc";
            CourseActions.GetCourseByCourseId("9989").ReturnsForAnyArgs(agxCourse.ToCourse());

            Context.CurrentUser = new UserInfo() { FirstName = "abc", LastName = "efg", ReferenceId = "11223" };

            controller = new AccountWidgetController(Context, UserActions, PageActions, CourseActions, EnrollmentActions, CourseHelper, CacheProvider);

            InitiazlieControllerContext();
        }

        private void InitiazlieControllerContext()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();

            var routeData = new RouteData();
            requestContext.RouteData = routeData;

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
                RequestContext = requestContext
            };

            controller.ControllerContext.RouteData.Returns(routeData);

            var urlHelper = new UrlHelper(requestContext, PopulateRoutes());
            controller.Url = urlHelper;
        }

        [TestMethod]
        public void ListCoursesProperlyHandlesCourseSectionTypeOfMedia()
        {
            var courses = new List<BizDC.Course>();
            courses.Add(new BizDC.Course
            {
                CourseType = "XBOOK",
                CourseSectionType = "media",
                CourseSubType = "regular",
                Id = "161978",
                SubType = "bedhandbook9e",
                Title = "Media title"
            });
            courses.Add(new BizDC.Course
            {
                CourseType = "XBOOK",
                CourseSectionType = "media",
                CourseSubType = "instructor_dashboard",
                Id = "133534",
                SubType = "bedhandbook9e",
                Title = "Dashboard"
            });

            CourseHelper.ListCourses("", "", false, false, "").ReturnsForAnyArgs(courses);
            urlHelper.RouteUrl("CourseSectionHome", Arg.Any<object>()).Returns("/media/bedhandbook9e/161978");
            Context.AccessLevel = AccessLevel.Instructor;
            Context.Course.CourseSectionType = "media";
            Context.CourseId = "161978";
            Context.Course.ProductCourseId = "133534";
            Context.Course.CourseType = Models.CourseType.XBOOK.ToString();
            
            ActionResult result = controller.ListCoursesForDropDown();
            var results = (List<SelectListItem>)((JsonResult)(result)).Data;
            var numberOfMedia = results.Count(r => r.Value.StartsWith("/media/"));

            Assert.AreEqual(2, numberOfMedia);
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

        [TestCategory("AccountWidgetController"),TestMethod]
        public void Summary_IfCourseTypeIsLearningCurve_ExpectAccountActionListHasCourses()
        {
            // Arrange
            Context.Course.CourseType = BizDC.CourseType.LearningCurve.ToString();
            ConfigurationManager.AppSettings["LearningCurveStudentHelpUrl"] = "demo";
            Context.AccessLevel = BizSC.AccessLevel.Student;
            Context.Product = new BizDC.Course();
            var model = new AccountWidget() { Account = new Account { DisplayName = "abc efg" } };
            var expected = new List<SelectListItem>
                {
                    new SelectListItem() { Selected = true, Text = model.Account.DisplayName, Value = "user" },
                    new SelectListItem() {Selected = false, Text = "Manage Profile", Value = "profile" },
                    new SelectListItem() { Text = "--------------------------------------", Value = "disabled"},
                    new SelectListItem() {Selected = false, Text = "learning curve course1", Value = "" },
                };
            CourseHelper.ListCourses(null, null, false, false, null)
                .ReturnsForAnyArgs(new List<Bfw.PX.Biz.DataContracts.Course>
                {
                    new Bfw.PX.Biz.DataContracts.Course{CourseSubType = "regular", SubType="LearningCurve", CourseSectionType = "", Id="course1", Title="learning curve course1"}
                });
            // Act
            var result = controller.Summary(new Bfw.PX.PXPub.Models.Widget());
            var accountActionsList = (List<SelectListItem>)((System.Web.Mvc.ViewResultBase)(result)).ViewData["accountActionsList"];

            // Assert
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.IsTrue(ObjectComparer.AreObjectsEqual(expected[i], accountActionsList[i]));

            }

        }

        [TestCategory("AccountWidgetController"), TestMethod]
        public void Verify_Summary_Properly_Sets_AccountActionsList_For_Not_CourseTypeLearningCurve()
        {
            // Arrange
            Context.Course.CourseType = "Not" + BizDC.CourseType.LearningCurve.ToString();
            Context.AccessLevel = BizSC.AccessLevel.Student;
            Context.Product = new BizDC.Course();
            var model = new AccountWidget() { Account = new Account { DisplayName = "abc efg" } };
            var expected = new List<SelectListItem>
                    {
                        new SelectListItem() { Selected = true, Text = model.Account.DisplayName, Value = "user" },
                        new SelectListItem() { Selected = false, Text = "Manage Profile", Value = "profile" }
                    };

            // Act
            var result = controller.Summary(new Bfw.PX.PXPub.Models.Widget());
            var accountActionsList = (List<SelectListItem>)((System.Web.Mvc.ViewResultBase)(result)).ViewData["accountActionsList"];

            // Assert
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(expected.First(), accountActionsList.First()));
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(expected.Skip(1).First(), accountActionsList.Skip(1).First()));
        }

        [TestCategory("AccountWidgetController"),TestMethod]
        public void Verify_Summary_Properly_Sets_AccountActionsList_For_Launchpad_Student()
        {
            // Arrange
            Context.Course.CourseType = "FACEPLATE";
            Context.AccessLevel = BizSC.AccessLevel.Student;
            Context.Product = new BizDC.Course();
            var model = new AccountWidget() { Account = new Account { DisplayName = "abc efg" } };
            var expected = new List<SelectListItem>
                    {
                        new SelectListItem() { Selected = true, Text = model.Account.DisplayName, Value = "user" },
                        new SelectListItem() { Selected = false, Text = "Switch course enrollment", Value = "switchenrollment" },
                        new SelectListItem() { Selected = false, Text = "Manage Profile", Value = "profile" }
                    };

            // Act
            var result = controller.Summary(new Bfw.PX.PXPub.Models.Widget());
            var accountActionsList = (List<SelectListItem>)((System.Web.Mvc.ViewResultBase)(result)).ViewData["accountActionsList"];

            // Assert
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(expected.First(), accountActionsList.First()));
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(expected.Skip(1).First(), accountActionsList.Skip(1).First()));
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(expected.Skip(2).First(), accountActionsList.Skip(2).First()));
        }

        [TestCategory("AccountWidgetController"), TestMethod]
        public void Summary_IfCourseIsLearningCurve_UserIsInstructor_ExpectAddCreateCourseLinkIsFalse()
        {
            // Arrange
            Context.Course.CourseType = BizDC.CourseType.LearningCurve.ToString();
            ConfigurationManager.AppSettings["LearningCurveInstructorHelpUrl"] = "demo";

            Context.AccessLevel = BizSC.AccessLevel.Instructor;
            Context.Product = new BizDC.Course();
            var model = new AccountWidget() { Account = new Account { DisplayName = "abc efg" } };

            // Act
            var result = controller.Summary(new Bfw.PX.PXPub.Models.Widget());
            var addcreateCourseLink =((System.Web.Mvc.ViewResultBase)(result)).ViewData["AddCreateCourseLink"];

            // Assert
            Assert.AreEqual("False", addcreateCourseLink.ToString());

        }

        [TestCategory("AccountWidgetController"), TestMethod]
        public void Summary_IfCourseIsLearningCurve_UserIsStudent_ExpectAddCreateCourseLinkIsFalse()
        {
            // Arrange
            Context.Course.CourseType = BizDC.CourseType.LearningCurve.ToString();
            ConfigurationManager.AppSettings["LearningCurveStudentHelpUrl"] = "demo";

            Context.AccessLevel = BizSC.AccessLevel.Student;
            Context.Product = new BizDC.Course();
            var model = new AccountWidget() { Account = new Account { DisplayName = "abc efg" } };


            // Act
            var result = controller.Summary(new Bfw.PX.PXPub.Models.Widget());
            var addcreateCourseLink = ((System.Web.Mvc.ViewResultBase)(result)).ViewData["AddCreateCourseLink"];

            // Assert
            Assert.AreEqual("False", addcreateCourseLink.ToString());

        }

        [TestCategory("AccountWidgetController"), TestMethod]
        public void Summary_IfCourseIsNotLearningCurve_UserIsInstructor_ExpectAddCreateCourseLinkIsTrue()
        {
            // Arrange
            Context.Course.CourseType = "Not" + BizDC.CourseType.LearningCurve.ToString();

            Context.AccessLevel = BizSC.AccessLevel.Instructor;
            Context.Product = new BizDC.Course();
            var model = new AccountWidget() { Account = new Account { DisplayName = "abc efg" } };


            // Act
            var result = controller.Summary(new Bfw.PX.PXPub.Models.Widget());
            var addcreateCourseLink = ((System.Web.Mvc.ViewResultBase)(result)).ViewData["AddCreateCourseLink"];

            // Assert
            Assert.AreEqual("True", addcreateCourseLink.ToString());

        }

        [TestCategory("AccountWidgetController"), TestMethod]
        public void Summary_IfCourseIsNotLearningCurve_UserIsStudent_ExpectAddCreateCourseLinkIsFalse()
        {
            // Arrange
            Context.Course.CourseType = "Not" + BizDC.CourseType.LearningCurve.ToString();

            Context.AccessLevel = BizSC.AccessLevel.Student;
            Context.Product = new BizDC.Course();
            var model = new AccountWidget() { Account = new Account { DisplayName = "abc efg" } };


            // Act
            var result = controller.Summary(new Bfw.PX.PXPub.Models.Widget());
            var addcreateCourseLink = ((System.Web.Mvc.ViewResultBase)(result)).ViewData["AddCreateCourseLink"];

            // Assert
            Assert.AreEqual("False", addcreateCourseLink.ToString());

        }
    }
}
