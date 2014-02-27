using System.Configuration;
using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.DataContracts;
using System.Collections.Generic;
using NSubstitute;
using Course = Bfw.PX.Biz.DataContracts.Course;
using Domain = Bfw.Agilix.DataContracts.Domain;
using TestHelper;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    
    
    /// <summary>
    ///This is a test class for EnrollmentActionsTest and is intended
    ///to contain all EnrollmentActionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EnrollmentActionsTest
    {


        private TestContext testContextInstance;
        private IBusinessContext context;
        private ISessionManager _sessionManager;
        private ISession session;
        private INoteActions _noteActions;
        private IUserActions _userActions;
        private IContentActions _contentActions;
        private EnrollmentActions enrollmentActions;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        
        
        [TestInitialize()]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            _sessionManager = Substitute.For<ISessionManager>();
            _contentActions = Substitute.For<IContentActions>();
            _userActions = Substitute.For<IUserActions>();
            _noteActions = Substitute.For<INoteActions>();
            session = Substitute.For<ISession>();
            _sessionManager.CurrentSession.Returns(session);
            context.Course = new Course() { Id = "productCourse", CourseType = "FACEPLATE" };

            enrollmentActions = new EnrollmentActions(context, _sessionManager, _noteActions, _userActions, _contentActions);
        }
   
        #endregion


        /// <summary>
        ///ListEnrollments should return enrollments without courses and count
        ///</summary>
        [TestMethod()]
        public void ListEnrollments_ShouldGetAgilixUsers_ReturnEnrollments()
        {
           
            var result = LoadEnrollments(false, false);
            Assert.IsFalse(result.IsNullOrEmpty());
            Assert.AreEqual("course1", result.FirstOrDefault().CourseId);
            Assert.AreEqual("enrollment1", result.FirstOrDefault().Id);
        }
        /// <summary>
        ///ListEnrollments should return enrollments with courses when load courses = true
        ///</summary>
        [TestMethod()]
        public void ListEnrollments_ShouldGetAgilixUsers_ReturnCourses()
        {
             session.When(s => s.ExecuteAsAdmin(Arg.Is<Batch>(b => b.Commands.FirstOrDefault() is GetCourse)))
                 .Do(ci =>
                 {
                     var getCourseCmd = ci.Arg<Batch>().Commands.FirstOrDefault() as GetCourse;
                     if (getCourseCmd.SearchParameters.CourseId == "course1")
                     {
                         getCourseCmd.Courses = new List<Agilix.DataContracts.Course>()
                         {
                             new Agilix.DataContracts.Course()
                             {
                                 Id = "course1",
                                 Title = "Course 1 Title"
                             }
                         };
                     }
                 });
            var result = LoadEnrollments(true, false);
            Assert.IsFalse(result.IsNullOrEmpty());
            Assert.AreEqual(result.FirstOrDefault().Course.Id, "course1");
            Assert.AreEqual(result.FirstOrDefault().Course.Title, "Course 1 Title");
        }

        /// <summary>
        ///ListEnrollments should return enrollments with courses when load courses = true
        ///</summary>
        [TestMethod()]
        public void ListEnrollments_ShouldGetAgilixUsers_ReturnEnrollmentCount()
        {
            session.When(s => s.ExecuteAsAdmin(Arg.Is<Batch>(b => b.Commands.FirstOrDefault() is GetCourse)))
                .Do(ci =>
                {
                    var getCourseCmd = ci.Arg<Batch>().Commands.FirstOrDefault() as GetCourse;
                    if (getCourseCmd.SearchParameters.CourseId == "course1")
                    {
                        getCourseCmd.Courses = new List<Agilix.DataContracts.Course>()
                         {
                             new Agilix.DataContracts.Course()
                             {
                                 Id = "course1",
                                 Title = "Course 1 Title"
                             }
                         };
                    }
                });

            session.When(s => s.ExecuteAsAdmin(Arg.Is<Batch>(b => b.Commands.FirstOrDefault() is GetEntityEnrollmentList)))
                .Do(ci =>
                {
                    var getCourseCmd = ci.Arg<Batch>().Commands.FirstOrDefault() as GetEntityEnrollmentList;
                    if (getCourseCmd.SearchParameters.CourseId == "course1")
                    {
                        getCourseCmd.Enrollments = new List<Agilix.DataContracts.Enrollment>()
                         {
                            new Agilix.DataContracts.Enrollment() {Id="studentEnrollment1", CourseId = "course1", Flags = DlapRights.Participate, Domain = new Domain(){Id="domainId"}},
                            new Agilix.DataContracts.Enrollment() {Id="studentEnrollment2", CourseId = "course1", Flags = DlapRights.Participate, Domain = new Domain(){Id="domainId"}},
                            new Agilix.DataContracts.Enrollment() {Id="studentEnrollment3", CourseId = "course1", Flags = DlapRights.Participate, Domain = new Domain(){Id="domainId"}},
                            new Agilix.DataContracts.Enrollment() {Id="instructorEnrollment4", CourseId = "course1", Flags = DlapRights.SubmitFinalGrade, Domain = new Domain(){Id="domainId"}}
                         };
                    }
                });
            var result = LoadEnrollments(true, true);
            Assert.IsFalse(result.IsNullOrEmpty());
            Assert.AreEqual(result.FirstOrDefault().Course.StudentEnrollmentCount, 3);
        }

        /// <summary>
        /// Should return enrollments with item and category (totals) grades
        /// </summary>
        [TestMethod()]
        public void GetEntityEnrollmentsWithGrades_Should_Return_Enrollments_Grades()
        {
            var grade = new Agilix.DataContracts.Grade();
            grade.GetType().GetProperty("Item").SetValue(grade, new Agilix.DataContracts.Item(), null);
            var grades = new List<Agilix.DataContracts.Grade>()
            {
                grade
            };
            var catgrades = new List<Agilix.DataContracts.CategoryGrade>()
            {
                new Agilix.DataContracts.CategoryGrade()
            };
            var enrollment = new Agilix.DataContracts.Enrollment()
            {
                Domain = new Domain() 
                {
                    Id = "x"
                }
            };
            enrollment.GetType().GetProperty("ItemGrades").SetValue(enrollment, grades, null);
            enrollment.GetType().GetProperty("CategoryGrades").SetValue(enrollment, catgrades, null);
            var enrollments = new List<Agilix.DataContracts.Enrollment>();
            enrollments.Add(enrollment);

            session.WhenForAnyArgs(o => o.Execute(Arg.Any<GetGrades>())).Do(o =>
            {
                o.Arg<GetGrades>().GetType().GetProperty("Enrollments").SetValue(o.Arg<GetGrades>(), enrollments, null); ;
            });

            var result = enrollmentActions.GetEntityEnrollmentsWithGrades("1");

            Assert.IsTrue(result.First().ItemGrades.Any());
            Assert.IsTrue(result.First().CategoryGrades.Any());
        }

        /// <summary>
        ///ListEnrollments should return no enrollments for new RA users/users that dont have an agilix user account
        ///</summary>
        [TestMethod()]
        public void ListEnrollments_ForNewUsers_ShouldReturnNoEnrollments()
        {
            string referenceId = "user_ref1";
            string productCourseId = "prodCourse";
         
            _userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.Username == referenceId)).Returns(ci => null);
            
            var result = enrollmentActions.ListEnrollments(referenceId, productCourseId);
            Assert.AreEqual(0, result.Count);
        }
        private List<Enrollment> LoadEnrollments(bool loadCourses, bool getEnrollmentCount)
        {
            string referenceId = "user_ref1";
            string productCourseId = "prodCourse";
            var users = new List<UserInfo>()
            {
                new UserInfo()
                {
                    Id = "user1",
                    ReferenceId = referenceId
                }
            };
            _userActions.ListUsersLike(Arg.Is<UserInfo>(u => u.Username == referenceId)).Returns(users);
            session.When(s => s.ExecuteAsAdmin(Arg.Is<Batch>(b => b.Commands.FirstOrDefault() is GetUserEnrollmentList)))
                .Do(ci =>
                {
                    var getEnrollmentCmd = ci.Arg<Batch>().Commands.FirstOrDefault() as GetUserEnrollmentList;
                    if (getEnrollmentCmd.SearchParameters.UserId == "user1")
                        getEnrollmentCmd.Enrollments = new List<Agilix.DataContracts.Enrollment>()
                        {
                            new Agilix.DataContracts.Enrollment()
                            {
                                CourseId = "course1",
                                Course = new Agilix.DataContracts.Course(){Id="course1"},
                                Id = "enrollment1",
                                Domain = new Domain(){Id = "8841"}
                            }
                        };
                });

            var result = enrollmentActions.ListEnrollments(referenceId, productCourseId, loadCourses, getEnrollmentCount);
            return result;
        }
    }
}
