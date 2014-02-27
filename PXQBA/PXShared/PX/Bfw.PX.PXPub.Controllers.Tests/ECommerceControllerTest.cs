using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web;
using System.Web.Mvc;
using TestHelper;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class ECommerceControllerTest
    {
        private ECommerceController controller;

        private IBusinessContext Context;
        private IContentActions ContentActions;
        private ICourseActions CourseActions;
        private IUserActions UserActions;
        private IEnrollmentActions EnrollmentActions;
        private IDomainActions DomainActions;
        private ICourseHelper CourseHelper;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = Substitute.For<IBusinessContext>();
            ContentActions = Substitute.For<IContentActions>();
            CourseActions = Substitute.For<ICourseActions>();
            UserActions = Substitute.For<IUserActions>();
            EnrollmentActions = Substitute.For<IEnrollmentActions>();
            DomainActions = Substitute.For<IDomainActions>();
            CourseHelper = Substitute.For<ICourseHelper>();

            Context.Course = new Course() {Id = "127411", Isbn13 = "a9903023"};
            Context.ProductCourseId = "52346";
            Context.CurrentUser = new UserInfo() {FirstName = "abc", LastName = "efg", Id="30334", Username = "334456"};
            controller = new ECommerceController(Context, ContentActions, CourseActions, UserActions, EnrollmentActions,
                DomainActions, CourseHelper);
            InitializeControllerContext();
        }

        private void InitializeControllerContext()
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
        }

        [TestMethod]
        public void Enrollment_CourseTypeNotNull()
        {
            var agxCourse = new Bfw.Agilix.DataContracts.Course();
            agxCourse.ParseEntity(Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));

            CourseActions.GetCourseByCourseId("3333").ReturnsForAnyArgs(agxCourse.ToCourse());

            var result = controller.Enroll() as ViewResult;
            
            Assert.IsNotNull(result.ViewData["CourseType"]);
        }

        [TestMethod, TestCategory("LMSIntegration")]
        public void Update_LMSId_Test()
        {
            Context.CurrentUser.Username = "003034";
            UserActions.UpdateUser(new UserInfo() { DomainId = "8842", FirstName = "abc", LastName = "cde", Id = "33445" })
                .ReturnsForAnyArgs(true);

            var testValue = GetValueFromJsonResult<string>(controller.UpdateLMSId("U#00993", "8842"), "message");

            Assert.AreEqual("Update Successful", testValue);
        }

        [TestMethod, TestCategory("LMSIntegration")]
        public void Enrollment_Confirmation_Test()
        {
            Context.EnrollmentId = null;
            CourseActions.GetCourseByCourseId("153135").ReturnsForAnyArgs(new Course());
            EnrollmentActions.CreateEnrollments("8842", "", "", "", "", DateTime.Today.Date,
                DateTime.Today.Date.AddDays(7),"","", true).ReturnsForAnyArgs(new List<Enrollment>(){new Enrollment(){CourseId ="153145"}});
            UserActions.GetUserByReferenceAndDomainId("", "")
                .ReturnsForAnyArgs(new UserInfo() {Id = "33445", FirstName = "", LastName = "3434"});
            string parentid = "";

            CourseActions.GetGenericCourse("", out parentid).ReturnsForAnyArgs(new Course() {});

            var result = controller.EnrollmentConfirmation() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod, TestCategory("LMSIntegration")]
        public void Enrollment_LMS_Redirect_Test()
        {
            CourseActions.GetCourseByCourseId("153135").ReturnsForAnyArgs(new Course());
            EnrollmentActions.CreateEnrollments("8842", "", "", "", "", DateTime.Today.Date,
                DateTime.Today.Date.AddDays(7), "", "", true).ReturnsForAnyArgs(new List<Enrollment>() { new Enrollment() { CourseId = "153145" } });
            UserActions.GetUserByReferenceAndDomainId("", "")
                .ReturnsForAnyArgs(new UserInfo() { Id = "33445", FirstName = "", LastName = "3434" });
            string parentid = "";

            CourseActions.GetGenericCourse("", out parentid).ReturnsForAnyArgs(new Course() { });

            var result = controller.EnrollmentConfirmation() as ViewResult;
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Join_Should_Populate_EnrollmentStatus_Of_Model()
        {
            CourseActions.GetCourseByCourseId("").ReturnsForAnyArgs(new Course()
            {
                ProductCourseId = "121",
                Domain = new Domain() 
                { 
                    Name = "domain"
                }
            });
            UserActions.GetUserByReferenceAndDomainId("", "").ReturnsForAnyArgs(new UserInfo() { Id = "userId" });
            Context.EnrollmentStatus = "4";
            var result = controller.Join() as ViewResult;

            Assert.AreEqual(Bfw.Agilix.DataContracts.EnrollmentStatus.Withdrawn, (result.Model as Bfw.PX.PXPub.Models.EcomerceJoinCourse).EnrollmentStatus);
        }

        /// <summary>
        /// if student enrollment dropped then enrollment status is 4 and should allow re-enrollment
        /// </summary>
        [TestMethod]
        public void EnrollmentConfirmation_Should_Update_Enrollment_If_Status_Withdrawn()
        {
            Context.EntityId.Returns("entityId");
            Context.EnrollmentId = "enrollmentId";
            Context.EnrollmentStatus = "4";
            CourseActions.GetCourseByCourseId("").ReturnsForAnyArgs(new Course());
            EnrollmentActions.GetInactiveEnrollment("", "").ReturnsForAnyArgs(new Enrollment());

            var result = controller.EnrollmentConfirmation();

            EnrollmentActions.Received().UpdateEnrollment(Arg.Is<Enrollment>(o => o.Status.Equals("1")));
        }

        private T GetValueFromJsonResult<T>(JsonResult jsonResult, string propertyName)
        {
            var property =
                jsonResult.Data.GetType().GetProperties()
                .Where(p => string.Compare(p.Name, propertyName) == 0)
                .FirstOrDefault();

            if (null == property)
                throw new ArgumentException("propertyName not found", "propertyName");
            return (T)property.GetValue(jsonResult.Data, null);
        }

    }
}
