using System;
using System.Web.Mvc;
using System.Linq;
using Bfw.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using TestHelper;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.PXPub.Models;
using Bfw.PX.Biz.Services.Mappers;
using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
using System.Web;
using System.Web.Routing;
using PxWebUser;
using System.Configuration;
using Bfw.Common.Caching;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class DashboardCoursesWidgetControllerTest
    {
        private DashboardCoursesWidgetController controller;

        private IBusinessContext context;
        private IUserActions userActions;
        private ICourseActions courseActions;
        private IDashboardActions2 dashboardActions;
        private IDomainActions domainActions;
        private IEnrollmentActions enrollmentActions;
        private ICacheProvider cacheProvider;
        private UserInfo userInfo;
        private IContentActions contentActions;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            userActions = Substitute.For<IUserActions>();
            courseActions = Substitute.For<ICourseActions>();
            dashboardActions = Substitute.For<IDashboardActions2>();
            domainActions = Substitute.For<IDomainActions>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            cacheProvider = Substitute.For<ICacheProvider>();
            contentActions = Substitute.For<IContentActions>();

            userInfo = new UserInfo();

            userInfo.ReferenceId = "1";
            context.CurrentUser = userInfo;

            var course = new Bfw.Agilix.DataContracts.Course();
            course.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            course.Id = "1";
            context.Course = course.ToCourse();

            context.Product = new Biz.DataContracts.Course() { IsDashboardActive = true };

            var domain = new PX.Biz.DataContracts.Domain() { Id = "8841", Name = "Default" };
            var domains = new List<PX.Biz.DataContracts.Domain>();
            domains.Add(domain);
            context.Domain.Returns(domain);
            domainActions.GetDomain(ConfigurationManager.AppSettings["MainParentDomain"]).Returns(domain);

            enrollmentActions.GetEnrollableDomains(context.CurrentUser.Username, domain.Id).ReturnsForAnyArgs(domains);
            enrollmentActions.GetEntityEnrollmentsAsAdmin("1").ReturnsForAnyArgs(GetEnrollmentList());

            context.CacheProvider.Returns(cacheProvider);

            var terms = new List<Bfw.PX.Biz.DataContracts.CourseAcademicTerm>() { new Bfw.PX.Biz.DataContracts.CourseAcademicTerm() { Id = "Default", Name = "Default" }, new Bfw.PX.Biz.DataContracts.CourseAcademicTerm() { Id = "Summer", Name = "Summer" }, new Bfw.PX.Biz.DataContracts.CourseAcademicTerm() { Id = "Winter", Name = "Winter" } };
            courseActions.ListAcademicTerms().Returns(terms);
            courseActions.ListAcademicTerms("1").ReturnsForAnyArgs(terms);
            courseActions.GetCourseByCourseId("1").Returns(course.ToCourse());

            controller = new DashboardCoursesWidgetController(context, userActions, courseActions, dashboardActions, domainActions, enrollmentActions, contentActions);

            InitializeControllerContext();
        }

        [TestMethod]
        public void CreateBranchConfirmation_Rendered()
        {
            var result = controller.CreateBranchConfirmation();

            Assert.AreEqual(result.GetType().FullName, typeof(ViewResult).FullName);
        }

        [TestMethod]
        public void CreateCourseOption_Rendered()
        {
            var result = controller.CreateCourseOption();

            Assert.AreEqual(result.GetType().FullName, typeof(ViewResult).FullName);
        }

        [TestMethod]
        public void ViewAll_Rendered()
        {
            Bfw.PX.PXPub.Models.Widget model = new Bfw.PX.PXPub.Models.Widget();

            var result = controller.ViewAll(model);

            Assert.AreEqual(result.GetType().FullName, typeof(ViewResult).FullName);
        }

        [TestMethod]
        public void Summary_Rendered()
        {
            var domains = new List<Bfw.PX.Biz.DataContracts.Domain>();
            domains.Add(new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" });
            context.GetRaUserDomains().Returns(domains);
            var agxCourse = new Bfw.Agilix.DataContracts.Course();
            agxCourse.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            Bfw.PX.Biz.DataContracts.DashboardData dashboardData = new Bfw.PX.Biz.DataContracts.DashboardData();
            var course = agxCourse.ToCourse();
            var dashboardItem = new PX.Biz.DataContracts.DashboardItem() { Course = course };
            dashboardItem.Level = "1";
            dashboardData.InstructorCourses.Add(dashboardItem);
            dashboardData.ProgramManagerTemplates.Add(new Biz.DataContracts.DashboardItem());
            dashboardData.PublisherTemplates.Add(new Biz.DataContracts.DashboardItem());
            dashboardActions.GetDashboardData(true).ReturnsForAnyArgs(dashboardData);
            Bfw.PX.PXPub.Models.Widget model = new Bfw.PX.PXPub.Models.Widget();
            model.Properties.Add(new KeyValuePair<string, PropertyValue>("allow_createanotherbranch_column", new PropertyValue() { Value = true }));

            var result = controller.Summary(model);

            Assert.AreEqual(result.GetType().FullName, typeof(ViewResult).FullName);
        }

        [TestMethod]
        public void CreateCourse_Rendered()
        {
            var result = controller.CreateCourse();

            Assert.AreEqual(result.GetType().FullName, typeof(ViewResult).FullName);
        }

        [TestMethod]
        public void DeleteDashboardCourse_Rendered()
        {
            var result = controller.DeleteDashboardCourse("1");

            Assert.AreEqual(result.GetType().FullName, typeof(ViewResult).FullName);
        }

        [TestMethod]
        public void ViewRoster_Rendered()
        {
            var studentId = "1";
            enrollmentActions.GetEntityEnrollments(studentId, UserType.Student).Returns(GetEnrollmentList());

            var result = controller.ViewRoster(studentId);

            Assert.AreEqual(result.GetType().FullName, typeof(ViewResult).FullName);
        }

        [TestMethod]
        public void ActivateDashboardCourse_Returns_FailStatus()
        {
            JsonResult result = controller.ActivateDashboardCourse("0");

            Assert.AreEqual("false", (result.Data as Dictionary<string, string>)["status"].ToLower());
        }

        [TestMethod]
        public void DeactivateDashboardCourse_Returns_FailStatus()
        {
            JsonResult result = controller.DeactivateDashboardCourse("0");

            Assert.AreEqual("false", (result.Data as Dictionary<string, string>)["status"].ToLower());
        }

        [TestMethod]
        public void ActivateDashboardCourse_Returns_CourseInfo()
        {
            cacheProvider.Disabled = true;
            userActions.GetUserByReferenceAndDomainId(string.Empty, string.Empty).ReturnsForAnyArgs(new UserInfo() { Id = "1" });

            JsonResult result = controller.ActivateDashboardCourse("1");

            var dict = (Dictionary<string, string>)result.Data;
            var resultString = string.Join(";", dict.Select(x => x.Key + "=" + x.Value).ToArray());
            Assert.AreEqual("courseid=1;status=True;activationDate=01/02/2013 09:04 AM;url=;school=;instructoremail=", resultString);
        }

        [TestMethod]
        public void DeactivateDashboardCourse_Returns_CompleteStatus()
        {
            cacheProvider.Disabled = true;

            JsonResult result = controller.DeactivateDashboardCourse("1");

            Assert.AreEqual("true", (result.Data as Dictionary<string, string>)["status"].ToLower());
        }

        [TestMethod]
        public void GetCurrentDateTime_Returns_CourseCurrentTime()
        {
            JsonResult result = controller.GetCurrentDateTime("1");
            //expect Pacific time
            var timezone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            var offset =  timezone.GetUtcOffset(DateTime.UtcNow);
            var expectTime = DateTime.UtcNow.AddHours(offset.Hours).ToShortDateString();

            Assert.AreEqual(expectTime, DateTime.Parse(result.Data.ToString()).ToShortDateString());
        }

        [TestMethod]
        public void GetAcademicTermsByDomain_Returns_ListOfTerms()
        {
            domainActions.GetDomainById(ConfigurationManager.AppSettings["BfwUsersDomainId"]).Returns(new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" });
            courseActions.GetAcademicTermsByDomain("1").Returns(new List<Bfw.PX.Biz.DataContracts.CourseAcademicTerm>() { new Bfw.PX.Biz.DataContracts.CourseAcademicTerm() { Id = "3", Name = "Summer" }, new Bfw.PX.Biz.DataContracts.CourseAcademicTerm() { Id = "4", Name = "Winter" } });

            JsonResult result = controller.GetAcademicTermsByDomain();

            var dict = (Bfw.PX.Biz.DataContracts.CourseAcademicTerm[])result.Data;
            var resultString = string.Join(";", dict.Select(x => x.Id + "=" + x.Name).ToArray());
            Assert.AreEqual("3=Summer;4=Winter", resultString);
        }

        [TestMethod]
        public void EditDashboardCourse_Returns_CourseInfo()
        {
            var course = new Bfw.PX.Biz.DataContracts.Course();
            course.Id = "1";
            course.Title = "Test Course";
            course.CourseNumber = "2";
            course.SectionNumber = "3";
            course.InstructorName = "Test Instructor";
            courseActions.UpdateCourse(course).ReturnsForAnyArgs(course);
            var schoolName = "Test School";
            domainActions.GetDomain(schoolName).Returns(new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" });

            JsonResult result = controller.EditDashboardCourse(course.Id, course.Title, course.CourseNumber, course.SectionNumber, course.InstructorName, schoolName, "Summer", "EST", true);

            Assert.AreEqual(course, result.Data);
        }


        [TestMethod, TestCategory("LMSIntegration")]
        public void Edit_Dashboard_Course_LMSId_Test()
        {
            Assert.AreEqual("", "");
        }

        [TestMethod, TestCategory("LMSIntegration")]
        public void Create_Course_From_Dashboard_LMSID_Test()
        {
            Assert.AreEqual("", "");
        }

        [TestMethod]
        public void CreateCourseFromDashboard_Returns_FailStatusIfNoDomain()
        {
            JsonResult result = controller.CreateCourseFromDashboard(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, true);

            Assert.AreEqual("false", (result.Data as Dictionary<string, string>)["status"].ToLower());
        }

        [TestMethod]
        public void CreateCourseFromDashboard_Returns_FailStatusIfNoCourseCreated()
        {
            domainActions.GetOrCreateDomain(string.Empty, string.Empty, string.Empty, userspacePrefix: ConfigurationManager.AppSettings["NewDomainUserspacePrefix"].ToString(), userspaceSuffix: ConfigurationManager.AppSettings["NewDomainUserspaceSuffix"].ToString(), copyResourcesForNewDomain: true).ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" });

            JsonResult result = controller.CreateCourseFromDashboard(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, true);

            Assert.AreEqual("false", (result.Data as Dictionary<string, string>)["status"].ToLower());
        }

        [TestMethod]
        public void CreateCourseFromDashboard_Returns_CourseInfo_New()
        {
            context.Course.SubType = "regular";
            domainActions.GetOrCreateDomain(string.Empty, string.Empty, string.Empty, userspacePrefix: ConfigurationManager.AppSettings["NewDomainUserspacePrefix"].ToString(), userspaceSuffix: ConfigurationManager.AppSettings["NewDomainUserspaceSuffix"].ToString(), copyResourcesForNewDomain: true).ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" });
            courseActions.CreateDerivedCourse(new Bfw.PX.Biz.DataContracts.Course(), "1", "").ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Course());
            courseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { new Bfw.PX.Biz.DataContracts.Course() }).ReturnsForAnyArgs(new List<Bfw.PX.Biz.DataContracts.Course>() { new Bfw.PX.Biz.DataContracts.Course() { Domain = new Biz.DataContracts.Domain() { Id = "8841", Name = "Default" } } });

            JsonResult result = controller.CreateCourseFromDashboard(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "new", true);

            Assert.IsInstanceOfType(result.Data, typeof(Bfw.PX.Biz.DataContracts.Course));
        }

        [TestMethod]
        public void CreateCourseFromDashboard_Returns_CourseInfo_FromDropdown()
        {
            context.Course.SubType = "regular";
            domainActions.GetOrCreateDomain(string.Empty, string.Empty, string.Empty, userspacePrefix: ConfigurationManager.AppSettings["NewDomainUserspacePrefix"].ToString(), userspaceSuffix: ConfigurationManager.AppSettings["NewDomainUserspaceSuffix"].ToString(), copyResourcesForNewDomain: true).ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" });
            courseActions.CreateDerivedCourse(new Bfw.PX.Biz.DataContracts.Course(), "1", "").ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Course());
            courseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { new Bfw.PX.Biz.DataContracts.Course() }).ReturnsForAnyArgs(new List<Bfw.PX.Biz.DataContracts.Course>() { new Bfw.PX.Biz.DataContracts.Course() { Id = "1", Domain = new Biz.DataContracts.Domain() { Id = "8841", Name = "Default" } } });

            JsonResult result = controller.CreateCourseFromDashboard(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "dropdown", true);

            Assert.AreEqual(result.Data, null);
        }

        [TestMethod]
        public void CreateCourseFromDashboard_Returns_CourseInfo_FromCopy()
        {
            context.Course.SubType = "regular";
            domainActions.GetOrCreateDomain(string.Empty, string.Empty, string.Empty, userspacePrefix: ConfigurationManager.AppSettings["NewDomainUserspacePrefix"].ToString(), userspaceSuffix: ConfigurationManager.AppSettings["NewDomainUserspaceSuffix"].ToString(), copyResourcesForNewDomain: true).ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" });
            courseActions.CreateDerivedCourse(new Bfw.PX.Biz.DataContracts.Course(), "1", "").ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Course());
            courseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { new Bfw.PX.Biz.DataContracts.Course() }).ReturnsForAnyArgs(new List<Bfw.PX.Biz.DataContracts.Course>() { new Bfw.PX.Biz.DataContracts.Course() { Id = "1", Domain = new Biz.DataContracts.Domain() { Id = "8841", Name = "Default" } } });

            JsonResult result = controller.CreateCourseFromDashboard(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "copy", true);

            Assert.IsInstanceOfType(result.Data, typeof(Bfw.PX.Biz.DataContracts.Course));
        }

        [TestMethod]
        public void CreateCourseFromDashboard_Should_Not_Copy_Syllabus_File()
        {
            context.Course.SubType = "regular";
            domainActions.GetOrCreateDomain(string.Empty, string.Empty, string.Empty, userspacePrefix: ConfigurationManager.AppSettings["NewDomainUserspacePrefix"].ToString(), userspaceSuffix: ConfigurationManager.AppSettings["NewDomainUserspaceSuffix"].ToString(), copyResourcesForNewDomain: true).ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" });
            var resource = new Resource();
            contentActions.GetResource("", "").ReturnsForAnyArgs(resource);
            var course = new Bfw.PX.Biz.DataContracts.Course() { Id = "2", SyllabusType = "File", Syllabus = "1/file.doc" };
            courseActions.CreateDerivedCourse(new Bfw.PX.Biz.DataContracts.Course(), "1", "").ReturnsForAnyArgs(course);
            courseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { new Bfw.PX.Biz.DataContracts.Course() }).ReturnsForAnyArgs(new List<Bfw.PX.Biz.DataContracts.Course>() { new Bfw.PX.Biz.DataContracts.Course() { Id = "1", Domain = new Biz.DataContracts.Domain() { Id = "8841", Name = "Default" } } });

            controller.CreateCourseFromDashboard(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, "copy", true);

            Assert.IsNull(course.Syllabus);            
        }

        [TestMethod]
        public void DeleteCourse_Returns_FalseIfNoCourseId()
        {
            var result = controller.DeleteCourse(string.Empty);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void DeleteCourse_Returns_FalseIfNoCourseList()
        {
            var result = controller.DeleteCourse(string.Empty + "," + string.Empty);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void DeleteCourse_Returns_FalseIfCourseDoesNotExist()
        {
            var result = controller.DeleteCourse("1");

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void DeleteCourse_Returns_TrueIfCourseExists()
        {
            dashboardActions.DeleteCourse("1").Returns(true);

            var result = controller.DeleteCourse("1");

            Assert.AreEqual(true, result);
        }

        /// <summary>
        /// When make changes to a course domain, it should update its enrollments' userid as well.
        /// </summary>
        [TestCategory("DashboardCoursesWidgetController"), TestMethod]
        public void EditDashboardCourse_ChangeDomain_ExpectEnrollmentUpdatedWithCorrectedUserId()
        {
            var testCourse = new Models.Course { Id = "testCourseId", SectionNumber = "section", CourseNumber = "courseNumbet", Title = "testTitle" };
            var testBizCourse = new Bfw.PX.Biz.DataContracts.Course { Id = "testCourseId", Domain = new Bfw.PX.Biz.DataContracts.Domain { Id = "1111" } };

            context.Course = testBizCourse;

            domainActions.GetDomain(null).ReturnsForAnyArgs(new Biz.DataContracts.Domain { Id = "8841" });
            courseActions.GetCourseByCourseId("testCourseId").Returns(testBizCourse);
            var enrollments = new List<Enrollment> { new Enrollment { User = new UserInfo { ReferenceId = "testReferenceId" } } };
            enrollmentActions.GetAllEntityEnrollmentsAsAdmin("testCourseId").Returns(enrollments);
            userActions.GetUserByReferenceAndDomainId("testReferenceId", "8841").Returns(new UserInfo { Id = "correctedUserId" });
            courseActions.GetCourseByCourseId("").ReturnsForAnyArgs(testBizCourse);
            bool hasCorrectedUser = true;
            bool hasCallUpdateEnrollments = false;
            //Here is the check, all enrollments' user id should be updated to 'correctedUserId'
            //Otherwise, the test will fail.
            enrollmentActions.When(f => f.UpdateEnrollments(Arg.Any<IEnumerable<Enrollment>>())).Do(x =>
            {
                var e = (IEnumerable<Enrollment>)x[0];
                hasCallUpdateEnrollments = true;
                hasCorrectedUser = e.Aggregate(hasCorrectedUser, (current, enrollment) => current && enrollment.User.Id == "correctedUserId");
            });

            controller.EditDashboardCourse("","","","","","8842","","",true);
            Assert.IsTrue(hasCorrectedUser);
            Assert.IsTrue(hasCallUpdateEnrollments);

        }

        private List<Enrollment> GetEnrollmentList()
        {
            var termId = "Summer";
            var enrollments = new List<Enrollment>();
            var course1 = new Bfw.PX.Biz.DataContracts.Course();
            var course2 = new Bfw.PX.Biz.DataContracts.Course();

            course1.Id = "1";
            course1.AcademicTerm = termId;
            course1.CourseSubType = "regular";
            course1.ParentId = "0";
            course1.Domain = new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" };
            enrollments.Add(new Enrollment() { Course = course1, User = new UserInfo() });

            course2.Id = "2";
            course2.AcademicTerm = termId;
            course2.CourseSubType = "regular";
            course2.ParentId = "1";
            course2.Domain = new Bfw.PX.Biz.DataContracts.Domain() { Id = "1", Name = "Default" };
            enrollments.Add(new Enrollment() { Course = course2, User = new UserInfo() });

            return enrollments;
        }

        private void InitializeControllerContext()
        {
            var httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            var requestContext = Substitute.For<RequestContext>();

            var routeData = new RouteData();
            requestContext.RouteData = routeData;

            controller.Url = new UrlHelper(requestContext, PopulateRoutes());

            request.Url.Returns(new Uri("http://lcl.worthpublishers.com/launchpad/myers10e/1/Dashboard"));
            httpContext.Request.Returns(request);

            controller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };
            controller.ControllerContext.RouteData.Returns(routeData);
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
