using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.PXPub.Controllers.Widgets;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Tests.Widgets
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
        private IContentActions contentActions;

        [TestInitialize()]
        public void MyTestInitialize() 
        {
            context = Substitute.For<IBusinessContext>();
            userActions = Substitute.For<IUserActions>();
            courseActions = Substitute.For<ICourseActions>();
            dashboardActions = Substitute.For<IDashboardActions2>();
            domainActions = Substitute.For<IDomainActions>();
            enrollmentActions = Substitute.For<IEnrollmentActions>();
            contentActions = Substitute.For<IContentActions>();
            controller = new DashboardCoursesWidgetController(context, userActions, courseActions, dashboardActions, domainActions, enrollmentActions, contentActions);
        }
        
        [TestMethod]
        public void EditDashboardCourse_DomainName_IsPassedBackToView_SchoolNameUnchanged()
        {
            var domainId = "66159";
            var courseId = "188815";
            var schoolName = "Baruch College CUNY (New York, NY)";
            context.CourseId.Returns(courseId);
            context.CourseIsProductCourse.Returns(true);
            context.CurrentUser.Returns(new UserInfo { Username = "John Doe" });
            var course = new Course { Domain = new Domain { Id = domainId } };
            courseActions.GetCourseByCourseId(courseId).Returns(course);
            courseActions.UpdateCourse(course).Returns(new Course { Id = courseId, Domain = new Domain { Id = domainId, Name = schoolName } });
            domainActions.GetDomain(schoolName).Returns(new Domain { Id = domainId, Name = schoolName });
            var result = controller.EditDashboardCourse(courseId, "Title", "", "", "John Doe", schoolName, "666b07aa-71e5-458a-a070-86d10d7e938d", "Eastern Standard Time", false);
            var courseModel = result.Data as Course;
            Assert.AreEqual(schoolName, courseModel.Domain.Name);
        }

        [TestMethod]
        public void EditDashboardCourse_DomainName_IsPassedBackToView_SchoolNameChanged()
        {
            var domainId = "66159";
            var courseId = "188815";
            var schoolName = "Baruch College CUNY (New York, NY)";
            context.CourseId.Returns(courseId);
            context.CourseIsProductCourse.Returns(true);
            context.CurrentUser.Returns(new UserInfo { Username = "John Doe" });
            var course = new Course { Domain = new Domain { Id = "8849" } };
            courseActions.GetCourseByCourseId(courseId).Returns(course);
            courseActions.UpdateCourse(course).Returns(new Course { Id = courseId, Domain = new Domain { Id = domainId, Name = schoolName } });
            domainActions.GetDomain(schoolName).Returns(new Domain { Id = domainId, Name = schoolName });
            var result = controller.EditDashboardCourse(courseId, "Title", "", "", "John Doe", schoolName, "666b07aa-71e5-458a-a070-86d10d7e938d", "Eastern Standard Time", false);
            var courseModel = result.Data as Course;
            Assert.AreEqual(schoolName, courseModel.Domain.Name);
        }
    }
}
