using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Contexts;
using Bfw.Agilix.Dlap;
using Bfw.Common.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using TestHelper;

using Bfw.Agilix.Commands;
using Adc = Bfw.Agilix.DataContracts;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
   

    [TestClass]
    public class UserActionsTest
    {
        private UserActions actions;

        private IBusinessContext context;
        private ISessionManager sessionManager;
        private ICourseActions courseActions;
        private IDomainActions domainActions;
        private IContentActions contentActions;
        private INoteActions noteActions;

        private ISession session;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            sessionManager = Substitute.For<ISessionManager>();
            domainActions = Substitute.For<IDomainActions>();
            contentActions = Substitute.For<IContentActions>();

            courseActions = Substitute.For<ICourseActions>();
            noteActions = Substitute.For<INoteActions>();
            
            session = Substitute.For<ISession>();
            sessionManager.CurrentSession.Returns(session);

            context.Course = new Course() { Id = "productCourse", CourseType = "FACEPLATE" };
            

            this.actions = new UserActions(context, sessionManager, contentActions, domainActions);
        }

        /// <summary>
        /// Check that that context product course id is used when finding courses by instructor
        /// </summary>
        [TestMethod]
        public void FindCoursesByInstructor_IsProductCourse()
        {
            context.ProductCourseId = "productCourse";
            context.CourseIsProductCourse.Returns(true);

         
            var dataEnrollments = new GetUserEnrollmentList()
            {
                Enrollments = new List<Adc.Enrollment>()
                {
                    new Adc.Enrollment()
                    {
                        Course =  new Adc.Course()
                        {
                            Id = "inst1Course",
                            Title = "course1Title"
                        },
                        Domain = new Adc.Domain()
                        {
                            Id = "domain1"
                        },
                        Flags = DlapRights.SubmitFinalGrade
                    }
                }
            };

            var expectedQueryFilter = String.Format("/meta-product-course-id='{0}", "productCourse");
            var expectedQueryFilterUsed = false;
            session.When(s => 
                s.ExecuteAsAdmin(Arg.Any<GetUserEnrollmentList>()))
                .Do(s =>
                {
                    s.Arg<GetUserEnrollmentList>().Enrollments = dataEnrollments.Enrollments;
                    if(s.Arg<GetUserEnrollmentList>().SearchParameters.Query.Contains(expectedQueryFilter))
                    {
                        expectedQueryFilterUsed = true;
                    }
                });

            var courseList = actions.FindCoursesByInstructor("FACEPLATE", "inst1", "domain1", "term1", "");

            Assert.IsTrue(!courseList.IsNullOrEmpty());

            foreach (var course in courseList)
            {
                Assert.AreEqual(course.Id, "inst1Course");
                Assert.AreEqual(course.Title, "course1Title");
                Assert.IsTrue(expectedQueryFilterUsed);
            }
            
        }

        [TestMethod]
        public void Pxmigration_ShouldNotBeIn_InstructorList()
        {
            var domainId = "66159";
            var dataCourses = new GetCourse()
            {
                Courses = new List<Adc.Course>()
                {
                    new Adc.Course { Id = "132133", Domain = new Adc.Domain { Id = domainId } },
                    new Adc.Course { Id = "187455", Domain = new Adc.Domain { Id = domainId } },
                    new Adc.Course { Id = "245212", Domain = new Adc.Domain { Id = domainId } }
                }
            };
            session.When(s => s.ExecuteAsAdmin(Arg.Any<GetCourse>())).Do(s => { s.Arg<GetCourse>().Courses = dataCourses.Courses; });
            session.When(s => s.ExecuteAsAdmin(Arg.Any<Batch>())).Do(s =>
            {
                var cmd1 = s.Arg<Batch>().CommandAs<GetEntityEnrollmentList>(0);
                var cmd2 = s.Arg<Batch>().CommandAs<GetEntityEnrollmentList>(1);
                var cmd3 = s.Arg<Batch>().CommandAs<GetEntityEnrollmentList>(2);
                cmd1.Enrollments = new List<Adc.Enrollment> { new Adc.Enrollment{ User = new Adc.AgilixUser{ Id = actions.PxMigrationUserId }, Flags = DlapRights.SubmitFinalGrade },
                                                              new Adc.Enrollment{ User = new Adc.AgilixUser{ Id = "648789" }, Flags = DlapRights.SubmitFinalGrade } 
                                                            };
                cmd2.Enrollments = new List<Adc.Enrollment> { new Adc.Enrollment{ User = new Adc.AgilixUser{ Id = actions.PxMigrationUserId }, Flags = DlapRights.SubmitFinalGrade },
                                                              new Adc.Enrollment{ User = new Adc.AgilixUser{ Id = "384173" }, Flags = DlapRights.SubmitFinalGrade } 
                                                            };
                cmd3.Enrollments = new List<Adc.Enrollment> { new Adc.Enrollment{ User = new Adc.AgilixUser{ Id = actions.PxMigrationUserId }, Flags = DlapRights.SubmitFinalGrade },
                                                              new Adc.Enrollment{ User = new Adc.AgilixUser{ Id = "679254" }, Flags = DlapRights.SubmitFinalGrade } 
                                                            };
            });
            List<UserInfo> users = actions.ListInstructorsForDomain("FACEPLATE", domainId, "5349a155-7a5c-4788-880f-f0b6fcb1ff8f").ToList();
            Assert.AreEqual(null, users.SingleOrDefault(u => u.Id == actions.PxMigrationUserId));
        }
    }
}
