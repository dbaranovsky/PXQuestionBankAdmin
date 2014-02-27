using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    [TestClass]
    public class CourseControllerTest
    {
        private IBusinessContext _context ;

        private ICourseActions _courseActions ;

        private IDomainActions _domainActions ;

        private ITaskActions _taskActions ;

        private IEnrollmentActions _enrollmentActions ;

        private IUserActions _userActions ;


        private IAutoEnrollmentActions _autoEnrollmentActions ;
        

        private ICacheProvider _cacheProvider;
        private CourseController _controller;
        public CourseControllerTest()
        {
            _context = Substitute.For<IBusinessContext>();
            _courseActions = Substitute.For<ICourseActions>();
            _domainActions = Substitute.For<IDomainActions>();
            _taskActions = Substitute.For<ITaskActions>();
            _enrollmentActions = Substitute.For<IEnrollmentActions>();
            _userActions = Substitute.For<IUserActions>();
            _autoEnrollmentActions = Substitute.For<IAutoEnrollmentActions>();
            _cacheProvider = Substitute.For<ICacheProvider>();

            _context.CacheProvider.Returns(_cacheProvider);
            _context.CurrentUser = new UserInfo();
            var httpContext = Substitute.For<HttpContextBase>();
            var requestContext = Substitute.For<RequestContext>();
            
            _controller = new CourseController(_context, _courseActions, _taskActions, _autoEnrollmentActions, _enrollmentActions, _domainActions, _userActions);
            _controller.ControllerContext = new ControllerContext() { HttpContext = httpContext, RequestContext = requestContext };
            var formData = new System.Collections.Specialized.NameValueCollection();
            formData.Add("PossibleDomains", "8841");
            formData.Add("SelectedDerivativeDomain","TestDomain");
            _controller.HttpContext.Request.Form.Returns(formData);
        }

        /// <summary>
        /// When make changes to a course domain, it should update its enrollments' userid as well.
        /// </summary>
        [TestCategory("CourseController"), TestMethod]
        public void UpdateCourse_ChangeDomain_ExpectEnrollmentUpdatedWithCorrectedUserId()
        {
            var testCourse = new Models.Course { Id = "testCourseId", SectionNumber = "section", CourseNumber = "courseNumbet", Title = "testTitle"};
            var testBizCourse = new Course {Id="testCourseId", Domain = new Domain{Id ="1111"}};
            
            _context.Course = testBizCourse;
            
            _domainActions.GetOrCreateDomain(null, null, null, null, null).ReturnsForAnyArgs(new Biz.DataContracts.Domain { Id="8841"});
            _courseActions.GetCourseByCourseId("testCourseId").Returns(testBizCourse);
            var enrollments = new List<Enrollment> { new Enrollment { User = new UserInfo { ReferenceId  = "testReferenceId"} } };
            _enrollmentActions.GetAllEntityEnrollmentsAsAdmin("testCourseId").Returns(enrollments);
            _userActions.GetUserByReferenceAndDomainId("testReferenceId", "8841").Returns(new UserInfo{Id = "correctedUserId"});
            bool hasCorrectedUser = true;
            //Here is the check, all enrollments' user id should be updated to 'correctedUserId'
            //Otherwise, the test will fail.
            _enrollmentActions.When(f => f.UpdateEnrollments(Arg.Any<IEnumerable<Enrollment>>())).Do(x =>
            {
                var e = (IEnumerable<Enrollment>)x[0];
                hasCorrectedUser = e.Aggregate(hasCorrectedUser, (current, enrollment) => current && enrollment.User.Id == "correctedUserId");
            });

            _controller.UpdateCourse(testCourse, "edit");
            Assert.IsTrue(hasCorrectedUser);
        }

        /// <summary>
        /// Calling GetContextCourse returns LmsIdRequired defaulted to false.
        /// </summary>
        [TestCategory("LMSIntegration"), TestMethod]
        public void GetContextCourse_Returns_Defaulted_LMSIdRequired()
        {
            //Arrange
            var testCourse = new Models.Course { Id = "testCourseId", SectionNumber = "section", CourseNumber = "courseNumbet", Title = "testTitle" };
            var testBizCourse = new Course { Id = "testCourseId", Domain = new Domain { Id = "1111" } };
            _context.Course = testBizCourse;

            //Act
            JsonResult jsonResult = _controller.GetContextCourse();
            var serializer = new JavaScriptSerializer();
            var result = serializer.Deserialize<dynamic>(serializer.Serialize(jsonResult.Data));
            var property =
                jsonResult.Data.GetType()
                    .GetProperties()
                    .Where(p => string.Compare(p.Name, "LmsIdRequired") == 0)
                    .FirstOrDefault();
            var returnValue = (bool) property.GetValue(jsonResult.Data, null);

            //Assert
            Assert.IsFalse(returnValue);
        }

    }
}
