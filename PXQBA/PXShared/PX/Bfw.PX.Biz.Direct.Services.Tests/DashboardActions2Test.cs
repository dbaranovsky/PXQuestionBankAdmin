using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using TestHelper;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class DashboardActions2Test
    {
        private DashboardActions2 actions;

        private IBusinessContext context;
        private ISessionManager sessionManager;
        private ICourseActions courseActions;
        private IEnrollmentActions enrollmentActions;
        private ISharedCourseActions sharedCourseActions;
        private ICacheProvider cacheProvider;
        private ISession session;
        private UserInfo userInfo;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            sessionManager = Substitute.For<ISessionManager>();
            courseActions = Substitute.For<ICourseActions>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            sharedCourseActions = Substitute.For<ISharedCourseActions>();
            cacheProvider = Substitute.For<ICacheProvider>();
            session = Substitute.For<ISession>();
            userInfo = new UserInfo();

            context.CacheProvider.Returns(cacheProvider);            
            sessionManager.CurrentSession.Returns(session);
            context.CurrentUser = userInfo;

            context.Course = new Course() { Id = "1", CourseType = "LearningCurve" };
            courseActions.ListAcademicTerms().Returns(new List<CourseAcademicTerm>() { new CourseAcademicTerm() { Id = "Summer", Name = "Summer" }, new CourseAcademicTerm() { Id = "Winter", Name = "Winter" } });

            this.actions = new DashboardActions2(context, sessionManager, courseActions, enrollmentActions, sharedCourseActions);
        }

        [TestMethod]
        public void DeleteCourse_InvalidCourseId()
        {
            var courseId = "-1";
            GetCourseById(courseId);
            sessionManager.CurrentSession = null;

            var result = this.actions.DeleteCourse(courseId);
            
            Assert.AreEqual(result, false);
        }

        [TestMethod]
        public void DeleteCourse_ValidCourseId()
        {
            var courseId = "1";
            GetCourseById(courseId);

            var result = this.actions.DeleteCourse(courseId);

            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void GetDashboardCoursesSpecificToProduct_Returns_ListOfCourses()
        {
            var referenceId = "1";
            var productId = "2";
            var termId = "Summer";
            enrollmentActions.ListEnrollments(referenceId, productId).Returns(GetEnrollmentList());

            var courses = this.actions.GetDashboardCoursesSpecificToProduct(referenceId, productId, termId);

            Assert.AreEqual(courses.Count > 0, true);
        }

        [TestMethod]
        public void GetDashboardData_Returns_ListOfCourses()
        {
            PrepareRequiredDashboardData();

            var courses = this.actions.GetDashboardData(false).InstructorCourses;

            Assert.AreEqual(courses.Count > 0, true);
        }

        [TestMethod]
        public void GetDashboardData_Returns_ListOfCourses_WithSort()
        {
            PrepareRequiredDashboardData();

            var courses = this.actions.GetDashboardData(true).InstructorCourses;

            Assert.AreEqual(courses.Count > 0, true);
        }



        private void PrepareRequiredDashboardData()
        {
            var domains = new List<Domain>();
            domains.Add(new Domain() { Id = "1", Name = "Default" });
            context.GetRaUserDomains().Returns(domains);

            enrollmentActions.ListEnrollments(context.CurrentUser.Username, context.ProductCourseId, true, true).Returns(GetEnrollmentList()); 
        }

        private void GetCourseById(string courseId)
        {
            var agxCourse = new Bfw.Agilix.DataContracts.Course();
            agxCourse.ParseEntity(TestHelper.Helper.GetResponse(Entity.Course, "GenericCourse").Element("course"));
            courseActions.GetCourseByCourseId(courseId).Returns(agxCourse.ToCourse());
        }

        private List<Enrollment> GetEnrollmentList() 
        {
            var termId = "Summer";
            var enrollments = new List<Enrollment>();
            var course1 = new Course();
            var course2 = new Course();

            course1.Id = "1";
            course1.AcademicTerm = termId;
            course1.CourseSubType = "regular";
            course1.ParentId = "0";
            course1.Domain = new Domain() { Id = "1", Name = "Default" };
            enrollments.Add(new Enrollment() { Course = course1 });

            course2.Id = "2";            
            course2.AcademicTerm = termId;
            course2.CourseSubType = "regular";
            course2.ParentId = "1";
            course2.Domain = new Domain() { Id = "1", Name = "Default" }; 
            enrollments.Add(new Enrollment() { Course = course2 });

            return enrollments;
        }
    }
}
