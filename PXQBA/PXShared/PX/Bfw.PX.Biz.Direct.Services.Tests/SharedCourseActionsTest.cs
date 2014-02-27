using System;
using System.Linq;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using NSubstitute;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.DataContracts;
using System.Collections.Generic;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class SharedCourseActionsTest
    {
        #region Properties

        private IBusinessContext _context;
        private IEnrollmentActions _enrollmentActions;
        private ICourseActions _courseActions;
        private IUserActions _userActions;
        private IDatabaseManager _dbManager;

        private ISharedCourseActions _sharedCourseActions;
        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _enrollmentActions = Substitute.For<IEnrollmentActions>();
            _courseActions = Substitute.For<ICourseActions>();
            _userActions = Substitute.For<IUserActions>();
            _dbManager = Substitute.For<IDatabaseManager>();
            _sharedCourseActions = new SharedCourseActions(_context, _enrollmentActions, _courseActions, _userActions, _dbManager);
        }

        [TestCategory("SharedCourseActions"), TestMethod]
        public void GetSharedCourses()
        {
            //Create data for testing
            var record1 = new DatabaseRecord();
            record1["UserId"] = "userId1";
            record1["CourseId"] = record1["SharedCourseId"] = "course1";
            record1["Note"] = "randomNote";
            record1["AnonymousName"] = "";

            var record2 = new DatabaseRecord();
            record2["UserId"] = "instructor1";
            record2["CourseId"] = record2["SharedCourseId"] = "course1";
            record2["IsActive"] = "true";

            var record3 = new DatabaseRecord();
            record3["ItemId"] = "randomItem";
            record3["CourseId"] = record3["SharedCourseId"] = "course1";
            record3["IsActive"] = "true";
            var getSharedCourseDefinitionBySharingIdRecords = new List<DatabaseRecord> { record1 };
            var getSharedCourseDefinitionUsersRecords = new List<DatabaseRecord> { record2 };
            var getSharedCourseDefinitionItemsRecords = new List<DatabaseRecord> { record3 };

            // Mock return values of database query
            _dbManager.Query("GetSharedCourseDefinitionBySharingId @0", "userId1").Returns(getSharedCourseDefinitionBySharingIdRecords);
            _dbManager.Query("GetSharedCourseDefinition @0, @1", "course1", "userId1").Returns(getSharedCourseDefinitionBySharingIdRecords);
            _dbManager.Query("GetSharedCourseDefinitionUsers @0, @1", "course1", "userId1").Returns(getSharedCourseDefinitionUsersRecords);
            _dbManager.Query("GetSharedCourseDefinitionItems @0, @1", "course1", "userId1")
                .Returns(getSharedCourseDefinitionItemsRecords);
            // Mock methods in CourseActions & UserActions
            var courses = new List<Course>();
            var users = new List<UserInfo>();
            _courseActions.GetCoursesByCourseIds(
                Arg.Do<IEnumerable<string>>(x => x.ToList().ForEach(s => courses.Add(new Course { Id = s, CourseOwner = "instructor2" }))))
                .Returns(courses);
            _userActions.ListUsers(
                Arg.Do<IEnumerable<string>>(x => x.ToList().ForEach(s => users.Add(new UserInfo { Id = s, FirstName = "test", LastName = s}))))
                .Returns(users);
            var dashboardItems = _sharedCourseActions.getSharedCourses("userId1", true);
            //Ensure we got one result back
            Assert.IsFalse(dashboardItems.IsNullOrEmpty());
            Assert.IsTrue(dashboardItems.Count() == 1);
            var dashboardItem = dashboardItems.FirstOrDefault();
            Assert.IsTrue(dashboardItem != null);
            //Ensure the data we got back is expected
            Assert.AreEqual("course1", dashboardItem.CourseId);
            Assert.AreEqual("test instructor2", dashboardItem.OwnerName);
            //Ensure the user data we got back is expected
            Assert.IsTrue(dashboardItem.Users.Count == 1);
            Assert.IsTrue(dashboardItem.Users.ContainsKey("userId1"));
            Assert.IsTrue(dashboardItem.Users["userId1"] == "test userId1");
            //Ensure the note data we got back is expected
            Assert.IsTrue(dashboardItem.Notes.Count == 1);
            Assert.IsTrue(dashboardItem.Notes.ContainsKey("userId1"));
            Assert.IsTrue(dashboardItem.Notes["userId1"] == "randomNote");

        }
    }
}
