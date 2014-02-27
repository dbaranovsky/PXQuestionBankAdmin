using System;
using System.Collections.Generic;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.PX.Biz.DataContracts;
using NSubstitute.Core.Arguments;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.Services.Mappers;
using NSubstitute;
using Microsoft.Practices.ServiceLocation;
using TestHelper;
using System.Linq;
using Bfw.Common.Logging;
using AssignTabSettings = Bfw.PX.Biz.DataContracts.AssignTabSettings;
using Course = Bfw.PX.Biz.DataContracts.Course;
using CourseAssessmentConfiguration = Bfw.PX.Biz.DataContracts.CourseAssessmentConfiguration;
using CourseType = Bfw.PX.Biz.DataContracts.CourseType;
using DashboardSettings = Bfw.PX.Biz.DataContracts.DashboardSettings;
using Domain = Bfw.PX.Biz.DataContracts.Domain;
using LearningObjective = Bfw.PX.Biz.DataContracts.LearningObjective;
using TabSettings = Bfw.PX.Biz.DataContracts.TabSettings;
using TocCategory = Bfw.PX.Biz.DataContracts.TocCategory;

//using Course = Bfw.Agilix.DataContracts.Course;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class CourseActionsTest
    {
        private IBusinessContext context;
        private ISessionManager sessionManager;
        private ISession session;
        private INoteActions noteActions;
        private IContentActions contentActions;
        private IDomainActions domainActions;
        private ICacheProvider cacheProvider;
        private ITraceManager traceManager;

        private CourseActions courseActions;

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IBusinessContext>();
            sessionManager = Substitute.For<ISessionManager>();
            noteActions = Substitute.For<INoteActions>();
            contentActions = Substitute.For<IContentActions>();
            domainActions = Substitute.For<IDomainActions>();
            traceManager = Substitute.For<ITraceManager>();
            session = Substitute.For<ISession>();
            cacheProvider = Substitute.For<ICacheProvider>();
            context.CacheProvider.Returns(cacheProvider);

            context.Tracer.Returns(traceManager);

            context.CurrentUser = new UserInfo() { Id = "1", ReferenceId = "1" };

            var serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<IBusinessContext>().Returns(context);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            sessionManager.CurrentSession = session;
            courseActions = new CourseActions(context, sessionManager, noteActions, contentActions, domainActions);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "Got wrong number of copies (0 when there should be 1)")]
        public void CreateDerivedCourse_Should_Throw_Exception_If_Course_Was_Not_Created()
        {
            Course parentCourse = GetParentCourse();
            var courses = new List<Bfw.Agilix.DataContracts.Course>();
            sessionManager.CurrentSession.WhenForAnyArgs(o => o.ExecuteAsAdmin(Arg.Any<CopyCourses>())).Do(o =>
            {
                var type = o.Arg<CopyCourses>().GetType();
                type.GetProperty("Courses").SetValue(o.Arg<CopyCourses>(), courses, null);
            });

            var result = courseActions.CreateDerivedCourse(parentCourse, "2", "derivative", "1");
        }

        [TestMethod]
        public void CreateDerivedCourse_Should_Create_New_Course_With_Derivative_Option()
        {
            Course parentCourse = GetParentCourse();
            var courses = new List<Bfw.Agilix.DataContracts.Course>() { parentCourse.ToCourse() };
            sessionManager.CurrentSession.When(o => o.ExecuteAsAdmin(Arg.Any<GetCourse>())).Do(o => o.Arg<GetCourse>().Courses = courses);

            var result = courseActions.CreateDerivedCourse(parentCourse, "2", "derivative", "1");
            var calls = sessionManager.CurrentSession.ReceivedCalls();

            Assert.IsTrue(calls.First().GetArguments()[0].GetType().Name.Equals("CopyCourses"));
            Assert.IsTrue(calls.Skip(1).First().GetArguments()[0].GetType().Name.Equals("CreateEnrollment"));
            Assert.IsTrue(calls.Skip(2).First().GetArguments()[0].GetType().Name.Equals("UpdateUsers"));
            Assert.IsTrue(calls.Skip(3).First().GetArguments()[0].GetType().Name.Equals("GetCourse"));
            Assert.IsTrue(ObjectComparer.AreObjectsEqual(parentCourse, result, GetCourseIgnoreProperties()));
        }

        [TestMethod]
        public void CreateDerivedCourse_Should_Create_New_Course_With_Copy_Option()
        {
            Course parentCourse = GetParentCourse();
            var courses = new List<Bfw.Agilix.DataContracts.Course>() { parentCourse.ToCourse() };
            sessionManager.CurrentSession.When(o => o.ExecuteAsAdmin(Arg.Any<GetCourse>())).Do(o => o.Arg<GetCourse>().Courses = courses);

            var result = courseActions.CreateDerivedCourse(parentCourse, "2", "copy");
            var calls = sessionManager.CurrentSession.ReceivedCalls();

            var updateUsers = calls.SingleOrDefault(c => c.GetArguments().FirstOrDefault().GetType().Name.Equals("UpdateUsers")).GetArguments().FirstOrDefault();
            Assert.IsNotNull(updateUsers as UpdateUsers);
            Assert.AreEqual("10808740100", (updateUsers as UpdateUsers).Users.First().GradebookViewFlagsCourse.First().Value);
            Assert.AreEqual("9", (updateUsers as UpdateUsers).Users.First().GradebookSettingsFlagsCourse.First().Value);

            var copyCourses = calls.SingleOrDefault(c => c.GetArguments().FirstOrDefault().GetType().Name.Equals("CopyCourses")).GetArguments().FirstOrDefault();
            Assert.IsNotNull(copyCourses as CopyCourses);

            var createEnrollment = calls.SingleOrDefault(c => c.GetArguments().FirstOrDefault().GetType().Name.Equals("CreateEnrollment")).GetArguments().FirstOrDefault();
            Assert.IsNotNull(createEnrollment as CreateEnrollment);
            
            var getCourse = calls.SingleOrDefault(c => c.GetArguments().FirstOrDefault().GetType().Name.Equals("GetCourse")).GetArguments().FirstOrDefault();
            Assert.IsNotNull(getCourse as GetCourse);

            Assert.IsTrue(ObjectComparer.AreObjectsEqual(parentCourse, result, GetCourseIgnoreProperties()));
        }

        //CreateDerivedCourse needs to find the user of the domain we're creating a course in 
        //And use that user id to create an enrollment
        [TestMethod]
        public void CreateDerivedCourse_Should_Create_Enrollement_With_CourseDomain_User()
        {
            Course parentCourse = GetParentCourse();
            var domainId = "newDomainId";
            var oldDomainId = "oldDomainId";
            //set up current user to be incorrect
            context.CurrentUser.Id = "wrongUserId";
            //UpdateCurrentUser changes user to be correct
            context.When(ctx => ctx.UpdateCurrentUser(domainId)).Do(ci =>
            {
                context.CurrentUser.Id = "rightUserId";
                context.CurrentUser.DomainId = "newDomainId";
            });

            var courses = new List<Bfw.Agilix.DataContracts.Course>() { parentCourse.ToCourse() };
            sessionManager.CurrentSession.When(o => o.ExecuteAsAdmin(Arg.Any<GetCourse>())).Do(o => o.Arg<GetCourse>().Courses = courses);

            //verify that enrollments are called with the correct user id
            var correctUserEnrolled = false;
            sessionManager.CurrentSession.When(o => o.ExecuteAsAdmin(Arg.Is<CreateEnrollment>(ce =>
                ce.Enrollments.TrueForAll(e =>
                    e.User.Id == "rightUserId"))))
               .Do(ci => correctUserEnrolled = true);

            //ACT
            courseActions.CreateDerivedCourse(parentCourse, "newDomainId", "copy");

            //Check if enrollments created
            Assert.IsTrue(correctUserEnrolled);
        }

        //CreateDerivedCourse needs to find the user of the domain we're creating a course in 
        //And use that user id to create an enrollment
        [TestMethod]
        public void EnrollCourses_With_StudentEnrollment()
        {
            Course parentCourse = GetParentCourse();
            var domainId = "newDomainId";
            var oldDomainId = "oldDomainId";
            //set up current user to be incorrect
            context.CurrentUser.Id = "wrongUserId";
            //UpdateCurrentUser changes user to be correct
            context.When(ctx => ctx.UpdateCurrentUser(domainId)).Do(ci =>
            {
                context.CurrentUser.Id = "rightUserId";
                context.CurrentUser.DomainId = "newDomainId";
            });

            var courses = new List<Bfw.Agilix.DataContracts.Course>() { parentCourse.ToCourse() };
            sessionManager.CurrentSession.When(o => o.ExecuteAsAdmin(Arg.Any<GetCourse>())).Do(o => o.Arg<GetCourse>().Courses = courses);

            //verify that enrollments are called with the correct user id
            var correctUserEnrolled = false;
            sessionManager.CurrentSession.When(o => o.ExecuteAsAdmin(Arg.Is<CreateEnrollment>(ce =>
                ce.Enrollments.TrueForAll(e =>
                    e.User.Id == "rightUserId"))))
               .Do(ci => correctUserEnrolled = true);

            sessionManager.CurrentSession.Execute(Arg.Any<DlapCommand>());

            //ACT
            courseActions.EnrollCourses(new List<Course>(){parentCourse}, null, true);

            //Check if enrollments created
            session.Received().ExecuteAsAdmin(Arg.Any<CreateEnrollment>());
            session.Received().ExecuteAsAdmin(Arg.Any<UpdateUsers>());

        }

        //CreateDerivedCourse needs to find the user of the domain we're creating a course in 
        //And use that user id to create an enrollment
        [TestMethod]
        public void EnrollCourses_Without_StudentEnrollment()
        {
            Course parentCourse = GetParentCourse();
            var domainId = "newDomainId";
            var oldDomainId = "oldDomainId";
            //set up current user to be incorrect
            context.CurrentUser.Id = "wrongUserId";
            //UpdateCurrentUser changes user to be correct
            context.When(ctx => ctx.UpdateCurrentUser(domainId)).Do(ci =>
            {
                context.CurrentUser.Id = "rightUserId";
                context.CurrentUser.DomainId = "newDomainId";
            });

            var courses = new List<Bfw.Agilix.DataContracts.Course>() {parentCourse.ToCourse()};
            sessionManager.CurrentSession.When(o => o.ExecuteAsAdmin(Arg.Any<GetCourse>()))
                .Do(o => o.Arg<GetCourse>().Courses = courses);

            //verify that enrollments are called with the correct user id
            var correctUserEnrolled = false;
            sessionManager.CurrentSession.When(o => o.ExecuteAsAdmin(Arg.Is<CreateEnrollment>(ce =>
                ce.Enrollments.TrueForAll(e =>
                    e.User.Id == "rightUserId"))))
                .Do(ci => correctUserEnrolled = true);

            sessionManager.CurrentSession.Execute(Arg.Any<DlapCommand>());

            //ACT
            courseActions.EnrollCourses(new List<Course>() {parentCourse}, null, false);

            //Check if enrollments created
            session.Received().ExecuteAsAdmin(Arg.Any<CreateEnrollment>());
            session.Received().ExecuteAsAdmin(Arg.Any<UpdateUsers>());
        }

        /// <summary>
        /// All courses returned should be of same type
        /// </summary>
        [TestCategory("CourseActionsTest"), TestMethod]
        public void CourseActionsTest_ListCoursesByCourseTypes_ExpectOneCourseReturned()
        {
            const string query = "/meta-bfw_course_type='Eportfolio'";
            var courses = new List<Bfw.Agilix.DataContracts.Course> { new Bfw.Agilix.DataContracts.Course { Id = "testCourse" } };
            sessionManager.CurrentSession.When(s => s.ExecuteAsAdmin(Arg.Any<ListCourses>())).Do(s =>
            {
                s.Arg<ListCourses>().Courses = (s.Arg<ListCourses>().SearchParameters.Query == query) ? courses : null;
            });
            var returnCourses = courseActions.ListCoursesByCourseTypes(new List<CourseType> { CourseType.Eportfolio }).ToList();
            Assert.AreEqual(1, returnCourses.Count());
            Assert.IsTrue(returnCourses.Any(c => c.Id == "testCourse"));
        }

        /// <summary>
        /// All courses returned should be of same type
        /// </summary>
        [TestCategory("CourseActionsTest"), TestMethod]
        public void CourseActionsTest_ListCoursesMatchQuery_ExpectOneCourseReturned()
        {
            const string query = "/meta-bfw_course_type='Eportfolio'";
            var courses = new List<Bfw.Agilix.DataContracts.Course> { new Bfw.Agilix.DataContracts.Course { Id = "testCourse" } };
            sessionManager.CurrentSession.When(s => s.ExecuteAsAdmin(Arg.Any<ListCourses>())).Do(s =>
            {
                s.Arg<ListCourses>().Courses = (s.Arg<ListCourses>().SearchParameters.Query == query) ? courses : null;
            });
            var returnCourses = courseActions.ListCoursesMatchQuery(query).ToList();
            Assert.AreEqual(1, returnCourses.Count());
            Assert.IsTrue(returnCourses.Any(c => c.Id == "testCourse"));
        }

        /// <summary>
        /// All courses returned should be of same type
        /// </summary>
        [TestCategory("CourseActionsTest"), TestMethod]
        public void CourseActionsTest_ListCoursesByCourseTypes_WithMultipleCourseTypes()
        {
            const string query = "/meta-bfw_course_type='Eportfolio' OR /meta-bfw_course_type='PersonalEportfolioDashboard'";
            var courses = new List<Bfw.Agilix.DataContracts.Course> { new Bfw.Agilix.DataContracts.Course { Id = "testCourse" } };
            sessionManager.CurrentSession.When(s => s.ExecuteAsAdmin(Arg.Any<ListCourses>())).Do(s =>
            {
                s.Arg<ListCourses>().Courses = (s.Arg<ListCourses>().SearchParameters.Query == query) ? courses : null;
            });
            var returnCourses = courseActions.ListCoursesByCourseTypes(new List<CourseType> { CourseType.Eportfolio, CourseType.PersonalEportfolioDashboard }).ToList();
            Assert.AreEqual(1, returnCourses.Count());
            Assert.IsTrue(returnCourses.Any(c => c.Id == "testCourse"));
        }

        [TestMethod]
        public void CanListCourses()
        {
            var domainId = "66159";
            var academicTermIds = new List<string> { "2349a155-7a5c-4788-880f-f0b6fcb1ff8f2", "9bf5dc3b-3514-4989-9ed4-767175e94464" };
            context.Domain.Returns(new Domain { Id = domainId });
            var courses = new List<Adc.Course> { new Adc.Course{ Id = "34225" }, new Adc.Course{ Id = "52344" }, new Adc.Course{ Id = "43266" } };
            sessionManager.CurrentSession.When(s => s.ExecuteAsAdmin(Arg.Is<GetCourse>(g => g.SearchParameters.DomainId == domainId))).Do(s =>
            {
                s.Arg<GetCourse>().Courses = courses;
            });
            var resultCourses = courseActions.ListCourses(CourseType.Eportfolio, academicTermIds);
            Assert.AreEqual(3, resultCourses.Count());
        }

        private Course GetParentCourse()
        {
            return new Course()
            {
                Id = "",
                DashboardSettings = new DashboardSettings()
                {
                    DashboardHomePageStart = "",
                    ProgramDashboardHomePageStart = ""
                },
                ProductCourseId = "0",
                bfw_tab_settings = new TabSettings(),
                TinyMCE = new tinyMCE(),
                Categories = new List<TocCategory>(),
                RubricTypes = new List<string>(),
                AssignTabSettings = new AssignTabSettings(),
                SearchSchema = new SearchSchema(),
                FacetSearchSchema = new FacetSearchSchema(),
                AssessmentConfiguration = new CourseAssessmentConfiguration(),
                Metadata = new Dictionary<string, MetadataValue>(),
                ContactInformation = new List<ContactInfo>(),
                Syllabus = string.Empty,
                SyllabusType = "URL",
                EportfolioLearningObjectives = new List<LearningObjective>(),
                DashboardLearningObjectives = new List<LearningObjective>(),
                PublisherTemplateLearningObjectives = new List<LearningObjective>(),
                ProgramLearningObjectives = new List<LearningObjective>(),
                CourseSubType = "regular",
                DerivedCourseId = "-1",
                EnableArgaUrlMapping = true
            };
        }

        private string[] GetCourseIgnoreProperties()
        {
            // Ignoring Id because it's supposed to be different from original Id
            return new string[] { "Id", "GroupSets", "Properties", "ActivatedDate", "LmsIdLabel", "LmsIdPrompt", "GenericCourseId" };
        }
    }
}
