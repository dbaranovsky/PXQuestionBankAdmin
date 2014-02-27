using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.PXPub.Controllers.Mappers;
using NSubstitute;
using TestHelper;

namespace Bfw.PXWebAPI.Tests
{
    [TestClass]
    public class IApiCourseActionsTest
    {
        private ISessionManager sessionManager;
        private IBusinessContext context;
        private ICourseActions courseActions;
        private IItemQueryActions itemQueryActions;
        private ICacheProvider cacheProvider;

        private ApiCourseActions apiCourseActions;

        [TestInitialize]
        public void TestInitialize()
        { 
            sessionManager = Substitute.For<ISessionManager>();
            context = Substitute.For<IBusinessContext>();
            courseActions = Substitute.For<ICourseActions>();
            itemQueryActions = Substitute.For<IItemQueryActions>();
            cacheProvider = Substitute.For<ICacheProvider>();

            apiCourseActions = new ApiCourseActions(sessionManager, context, courseActions, itemQueryActions, cacheProvider);
        }

        [TestMethod]
        public void AddCopyDerivedCourse_Should_Create_New_Course_With_Same_Data()
        { 
            Course parentCourse  = new Course() 
            { 
                Id = "0", Title = "title", CourseType="LearningCurve", DashboardSettings = new DashboardSettings(), CourseOwner = "2",
                CourseNumber = "courseNumber",
                InstructorName = "instructorName",
                CourseTimeZone = "Eastern Standard Time",
                SectionNumber = "sectionNumber",
                CourseProductName = "course"
            };
            Course derivedCourse = new Course() { Id = "00", DashboardSettings = new DashboardSettings(), CourseProductName = "course", CourseOwner = "2" };
            var domainId = "1";
            UserInfo user = new UserInfo() { Id = "2", ReferenceId = "3" };
            courseActions.CreateDerivedCourse(parentCourse, domainId, "Derivative", user.Id).Returns(derivedCourse);
            courseActions.UpdateCourse(derivedCourse).Returns(derivedCourse);

            Bfw.PX.PXPub.Models.Course result = apiCourseActions.AddCopyDerivedCourse(parentCourse, domainId, "Derivative", user, "title", "LearningCurve", "", "courseNumber", "instructorName", "Eastern Standard Time", "sectionNumber");

            Assert.IsTrue(ObjectComparer.AreObjectsEqual(parentCourse, result.ToCourse(), "Id", "Properties", "ActivatedDate"));
        }
    }
}
