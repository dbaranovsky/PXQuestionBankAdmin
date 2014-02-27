using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using Bfw.PXWebAPI.Models.Response;
using NSubstitute;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Controllers;
using Bfw.PXWebAPI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.IO;
using System.Web.SessionState;
using System.Reflection;
using System.Web.Script.Serialization;
using NSubstitute.Core.Arguments;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PXWebAPI.Helpers.Context;
using Microsoft.Practices.ServiceLocation;
using Enrollment = Bfw.PX.Biz.DataContracts.Enrollment;

namespace Bfw.PXWebAPI.Tests.Controllers
{
    /// <summary>
    /// TODO - Revist as part of pending work to get the full end-to-end Integration tests working.
    /// TODO - This test will need to be re-located to the correct integration.Test folder once agreed on target location of all Integration Tests.
    /// </summary>
    [TestClass]
    public class UserControllerTest
    {
        private UserController controller;

        private ISessionManager sessionManager;
        private ISession session;
        private PX.Biz.ServiceContracts.IBusinessContext context;
        private IApiEnrollmentActions enrollmentActions;
        private IApiCourseActions courseActions;
        private IApiUserActions userActions;
        private IApiDomainActions domainActions;
        private IHttpContextAdapter httpContextAdapter;
        private HttpContext httpContext;
        private ICacheProvider cacheProvider;
        private IServiceLocator serviceLocator;
        private ICourseActions pxCourseActions;
        private IEnrollmentActions pxEnrollmentActions;

        [TestInitialize]
        public void TestInitialize()
        {
            sessionManager = Substitute.For<ISessionManager>();
            session = Substitute.For<ISession>();
            sessionManager.CurrentSession.Returns(session);
            context = Substitute.For<PX.Biz.ServiceContracts.IBusinessContext>();
            enrollmentActions = Substitute.For<IApiEnrollmentActions>();
            courseActions = Substitute.For<IApiCourseActions>();
            userActions = Substitute.For<IApiUserActions>();
            domainActions = Substitute.For<IApiDomainActions>();
            httpContextAdapter = Substitute.For<IHttpContextAdapter>();
            cacheProvider = Substitute.For<ICacheProvider>();
            context.CacheProvider.Returns(cacheProvider);
            serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<IBusinessContext>().Returns(context);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            pxCourseActions = Substitute.For<ICourseActions>();
            pxEnrollmentActions = Substitute.For<IEnrollmentActions>();

            controller = new UserController(sessionManager, context, enrollmentActions, courseActions, userActions, domainActions, httpContextAdapter, pxCourseActions, pxEnrollmentActions);
            
            InitializeControllerContext();
        }

        private void InitializeControllerContext()
        {
            var httpRequest = new HttpRequest("", "http://url/", "");
            var stringWriter = new StringWriter();
            var httpResponce = new HttpResponse(stringWriter);
            httpContext = new HttpContext(httpRequest, httpResponce);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null).Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;
        }

        [TestMethod, Ignore]
        public void CheckAndCreateUser_Should_Create_User()
        {
        }

        [TestMethod, Ignore]
        public void Details_Should_Return_User()
        {
            //Arrange

            //Act
            var user = controller.Details("135651");  //This test fails because we're using Fakes/NSubstitute - instead of a real Integration Test.

            //Assert
        }

        [TestMethod, TestCategory("Enrollment")]
        public void DropUserEnrollment_Should_Drop_Enrollment()
        {
            //Arrange
            var rauserid = "123";
            var courseid = "456";
            var domainid = "35";
            var agilixUserid = "888";

            var domain = new Adc.Domain { Id = domainid };
            var course = new Bfw.Agilix.DataContracts.Course();
            course.Domain = domain;
            course.Id = courseid;
            courseActions.GetCourseByCourseId(courseid).Returns(course);

            var users = new List<Adc.AgilixUser> { new Adc.AgilixUser { Id = agilixUserid } };
            userActions.GetUsers(rauserid, domainid, "").Returns(users);

            var enrollment = new Adc.Enrollment
            {
                Id = "1",
                PercentGraded = 0.25,
                Reference = "asdf",
                Schema = "def",
                Status = "1",
                Course = new Adc.Course { Id = courseid },
                User = new Adc.AgilixUser
                {
                    Id = agilixUserid,
                    FirstName = "First",
                    LastName = "Last",
                    Reference = rauserid,
                    LastLogin = DateTime.UtcNow,
                    Email = "FirstLast@fake.com"
                },
                Domain = domain
            };
            enrollmentActions.GetEnrollment(agilixUserid, courseid).Returns(enrollment);

            enrollmentActions.DropEnrollment(null).ReturnsForAnyArgs(true);

            //Act
            var result = controller.DropUserEnrollment(rauserid, courseid);

            //Assert
            Assert.IsTrue(result.results);
        }

        [TestMethod, TestCategory("Enrollment")]
        public void CreateUserEnrollment_Should_Create_Enrollment_As_Instructor()
        {
            //Arrange
            var rauserid = "123";
            var courseid = "456";
            var domainid = "35";
            var agilixUserid = "888";

            var domain = new Adc.Domain { Id = domainid };
            var course = new Bfw.Agilix.DataContracts.Course();
            course.Domain = domain;
            course.Id = courseid;
            pxCourseActions.GetCourseByCourseId(courseid).ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Course(){});

            userActions.GetUsers("123").ReturnsForAnyArgs(new List<Adc.AgilixUser>() { new Adc.AgilixUser()
            {
                FirstName   = "FirstName",
                LastName = "LastName",
                Email = "myemail@test.com",
                UserName = "123456",
                Reference = "654321",
                Domain = new Adc.Domain()
                {
                    
                }}
            } );

            var users = new List<Adc.AgilixUser> { new Adc.AgilixUser { Id = agilixUserid } };
            controller.CheckandCreateUserEnrollment(rauserid, course.Id, domainid, true);

            //Assert
            session.Received().Execute(Arg.Any<Bfw.Agilix.Commands.GetUserEnrollmentList>());

            pxCourseActions.ReceivedWithAnyArgs()
                .EnrollCourses(Arg.Any<List<PX.Biz.DataContracts.Course>>());
        }

        [TestMethod, TestCategory("Enrollment")]
        public void CreateUserEnrollment_Should_Create_Enrollment_As_Student()
        {
            //Arrange
            var rauserid = "123";
            var courseid = "456";
            var domainid = "35";
            var agilixUserid = "888";

            var domain = new Adc.Domain { Id = domainid };
            var course = new Bfw.Agilix.DataContracts.Course();
            course.Domain = domain;
            course.Id = courseid;
            pxCourseActions.GetCourseByCourseId(courseid).ReturnsForAnyArgs(new Bfw.PX.Biz.DataContracts.Course() { });

            userActions.GetUsers("123").ReturnsForAnyArgs(new List<Adc.AgilixUser>() { new Adc.AgilixUser()
            {
                FirstName   = "FirstName",
                LastName = "LastName",
                Email = "myemail@test.com",
                UserName = "123456",
                Reference = "654321",
                Domain = new Adc.Domain()
                {
                    
                }}
            });

            var users = new List<Adc.AgilixUser> { new Adc.AgilixUser { Id = agilixUserid } };
            controller.CheckandCreateUserEnrollment(rauserid, course.Id, domainid, false);

            //Assert
            session.Received().Execute(Arg.Any<Bfw.Agilix.Commands.GetUserEnrollmentList>());
            pxEnrollmentActions.ReceivedWithAnyArgs()
                .CreateEnrollments("2336","1235", "4458",null,"", DateTime.Now.Date,DateTime.Now.AddDays(365), null,"",false);
        }

        [TestMethod]
        public void Update_Should_Return_False_If_Failed()
        {
            //Added to fix cache clearing 
            userActions.GetUsersFromAllDomains("userid", true)
                .ReturnsForAnyArgs(new List<Adc.AgilixUser>()
                {
                    new Adc.AgilixUser()
                    {
                        Id = "userId",
                        UserName = "22334",
                        Domain = new Adc.Domain() {Id = "99125", Name = "test"}
                    }
                });

            userActions.UpdateUsers("userId", true, new EditUser() { FirstName = "Family", LastName = "Guy", Email = "family@guy.com" }).ReturnsForAnyArgs(false);

            var result = controller.Update("userId", true, new EditUser() { FirstName = "Family", LastName = "Guy", Email = "family@guy.com" });

            Assert.IsFalse(result.results);
        }

        [TestMethod]
        public void Update_Should_Return_True_If_Success()
        {
            //Added to fix cache clearing 
            userActions.GetUsersFromAllDomains("userid", true)
                .ReturnsForAnyArgs(new List<Adc.AgilixUser>()
                {
                    new Adc.AgilixUser()
                    {
                        Id = "userId",
                        UserName = "22334",
                        Domain = new Adc.Domain() {Id = "99125", Name = "test"}
                    }
                });

            userActions.UpdateUsers("userId", true, new EditUser() { FirstName = "Family", LastName = "Guy", Email = "family@guy.com" }).ReturnsForAnyArgs(true);

            var result = controller.Update("userId", true, new EditUser() { FirstName = "Family", LastName = "Guy", Email = "family@guy.com" });

            Assert.IsTrue(result.results);
        }
    }
}
