using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Mvc;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.Common.Patterns.Unity;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PXWebAPI.Helpers;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.DTO;
using Bfw.PXWebAPI.Models.Response;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Adc = Bfw.Agilix.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;
using Bsc = Bfw.PX.Biz.ServiceContracts;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PXWebAPI.Controllers.Tests
{
    [TestClass]
    public class CourseControllerTest
    {
        private ISession _currentSession;
        private CourseController _courseController;
        private Bsc.IUserActions _userActions;
        private IApiCourseActions _courseActions;
        private IApiDomainActions _domainActions;
        private Bsc.IContentActions _contentActions;
        private IApiEnrollmentActions _enrollmentActions;
        private Bsc.IEnrollmentActions _pxEnrollmentActions;
        private string _dummyTitle = "Test Course for CourseControllerTest";
        private string _productCourseId = "109554";
        private Bdc.UserInfo _user;
        private Bdc.UserInfo _user2;
        private List<CourseAcademicTerm> _terms;
        private DomainDto _createdDomainDto;
        private Adc.Domain _createdDomain;
        private const string DOMAIN_NAME = "Test Domain for CourseControllerTest";
        private const string REFERENCE = "8888889";

        #region Helper Methods

        public CourseControllerTest()
        {
            var dummyObject = new ThreadSessionManager(null, null);
                //to force MsTest to copy Bfw.Agilix.Dlap.Components to output directory
            ConfigureServiceLocator();
            InitializeSessionManager();
            _courseController = ServiceLocator.Current.GetInstance<CourseController>();
            _userActions = ServiceLocator.Current.GetInstance<Bsc.IUserActions>();
            _courseActions = ServiceLocator.Current.GetInstance<IApiCourseActions>();
            _domainActions = ServiceLocator.Current.GetInstance<IApiDomainActions>();
            _contentActions = ServiceLocator.Current.GetInstance<Bsc.IContentActions>();
            _enrollmentActions = ServiceLocator.Current.GetInstance<IApiEnrollmentActions>();
            _pxEnrollmentActions = ServiceLocator.Current.GetInstance<Bsc.IEnrollmentActions>();
        }

        private void ConfigureServiceLocator()
        {
            var locator = new Bfw.Common.Patterns.Unity.UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);

            var container = locator.Container;
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            DependencyResolver.SetResolver(new UnityDependencyResolver(locator.Container));
        }


        private void InitializeSessionManager()
        {
            var userName = ConfigurationManager.AppSettings["DlapUserName"];
            var password = ConfigurationManager.AppSettings["DlapUserPassword"];
            var userId = ConfigurationManager.AppSettings["DlapUserId"];
            var sessionManager = ServiceLocator.Current.GetInstance<ISessionManager>();
            _currentSession = sessionManager.StartNewSession(userName, password, false, userId);
            sessionManager.CurrentSession = _currentSession;
        }

        [TestInitialize]
        public void InitializeTest()
        {
            var domainDeletedId = CreateDomain();
            CreateAcademicTerms();

            if (!String.IsNullOrWhiteSpace(domainDeletedId))
            {
                var users = _userActions.ListUsersLike(
                    new Bdc.UserInfo
                    {
                        DomainId = domainDeletedId
                    });
                if (users != null)
                {
                    DeleteUsers(users.ToArray());
                }
            }
            _user = CreateUser();
        }
        [TestCleanup]
        public void CleanUp()
        {
            DeleteUsers(_user);
            DeleteAcademicTerms(_createdDomain.Id);
            DeleteDomain(_createdDomainDto);
        }

        private Bdc.Course CreateCourse()
        {
            var courses = new List<Bdc.Course> 
            {
                new Bdc.Course { 
                    Title = _dummyTitle,
                    Domain = new Bdc.Domain { Id = _createdDomain.Id }, 
                    DashboardSettings = new Bdc.DashboardSettings(), 
                    ProductCourseId = _productCourseId,
                    AcademicTerm = _terms.First().Id,
                    InstructorName = "Test Instructor"
                }
            };
            var cmd = new CreateCourses();
            cmd.Add(courses.Map(c => c.ToCourse()).ToList());
            _currentSession.ExecuteAsAdmin(cmd);
            return BizEntityExtensions.ToCourse(cmd.Courses.First());
        }

        private void DeleteCourse(Bdc.Course course)
        {
            course.DashboardSettings = new Bdc.DashboardSettings();
            List<Bdc.Course> courses = new List<Bdc.Course> { course };
            var cmd = new DeleteCourses();
            cmd.Add(courses.Map(c => c.ToCourse()));
            _currentSession.ExecuteAsAdmin(cmd);
        }

        private Bdc.UserInfo CreateUser(string username = "henryford", string password="modelt",string firstname="Henry", string lastname="Ford", string email="henry@test.com")
        {
            Bdc.UserInfo userInfo = _userActions.CreateUser(username, password,
                "", "", firstname, lastname, email, _createdDomainDto.Id, DOMAIN_NAME, REFERENCE);
            return userInfo;
        }
        private void DeleteUsers(params Bdc.UserInfo[] users)
        {
            if (users != null)
            {
                _userActions.DeleteUsers(users);
            }
        }
        private void DeleteUsers(params Models.User[] users)
        {
            if (users != null)
            {
                var list = new List<Bdc.UserInfo>();
                foreach (var usr in users)
                {
                    var userInfo = new Bdc.UserInfo
                    {
                        DomainId = usr.DomainId,
                        Email = usr.Email,
                        FirstName = usr.FirstName,
                        Id = usr.Id,
                        LastName = usr.LastName,
                        ReferenceId = usr.ReferenceId,
                        Username = usr.Username
                    };
                    list.Add(userInfo);
                }
                _userActions.DeleteUsers(list.ToArray());
            }
        }

        private string CreateDomain()
        {
            Adc.Domain domainDeleted = null;
            var cmd1 = new GetDomainList
            {
                SearchParameters = new Adc.Domain
                    {
                        Name = DOMAIN_NAME,
                        Reference = REFERENCE
                    }
            };
            _currentSession.ExecuteAsAdmin(cmd1);
            if (!cmd1.Domains.IsNullOrEmpty())
            {
                domainDeleted = cmd1.Domains.FirstOrDefault();
                var cmd = new DeleteDomain { Domain = domainDeleted };
                _currentSession.ExecuteAsAdmin(cmd);
            }

            _createdDomainDto = EntityExtensions.ToDomainDto(_domainActions.CreateDomain(DOMAIN_NAME, REFERENCE));
            _createdDomain = _domainActions.GetDomainById(_createdDomainDto.Id);

            return (domainDeleted != null) ? domainDeleted.Id : "";
        }
        private void DeleteDomain(DomainDto domain)
        {
            if (domain != null)
            {
                var cmd = new DeleteDomain { Domain = _createdDomain };
                _currentSession.ExecuteAsAdmin(cmd);
            }
        }

        private void CreateAcademicTerms()
        {
            var resources = new List<Bdc.Resource>();
            Bdc.Resource resource = null;
            var sb = new StringBuilder();
            sb.Append("<academic_terms>");
            sb.AppendFormat("<academic_term id=\"{0}\" start=\"{1}\" end=\"{2}\">Test Semester for CourseControllerTest CreateAcademicTerms</academic_term>",
                Guid.NewGuid(), DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.AddMonths(1).ToString("yyyy-MM-dd"));
            sb.Append("</academic_terms>");

            resource = new Bdc.Resource
            {
                EntityId = _createdDomainDto.Id,
                Url = "PX/academicterms.xml",
                Status = Bdc.ResourceStatus.Normal
            };

            var stream = resource.GetStream();
            stream.Seek(0, SeekOrigin.Begin);
            var buffer = Encoding.ASCII.GetBytes(sb.ToString());
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            resources.Add(resource);
            _contentActions.StoreResources(resources);

            _terms = _courseController.GetTerms(_createdDomain.Id).ToList();
        }
        private void DeleteAcademicTerms(string entityId)
        {
            var resources = new List<Bdc.Resource>();
            var resource = new Bdc.Resource
            {
                EntityId = entityId,
                Url = "PX/academicterms.xml",
            };
            resources.Add(resource);
            _contentActions.RemoveResources(resources);
            _terms = null;
        }
        #endregion Helper Methods

        [TestMethod]
        public void GetDetails()
        {
            Bdc.Course course = null;
            try
            {
                course = CreateCourse();
                CourseResponse response = _courseController.Details(course.Id);
                var results = response.results;
                Assert.AreEqual(course.Title, results.Title);
            }
            finally
            {
                DeleteCourse(course);
            }
        }

        [TestMethod]
        public void GetDetails_Returns_AllCourseInstructors()
        {
            Bdc.Course course = null;
            try
            {
                _user2 = CreateUser("abc", "abc123", "test", "test2", "test.test2@macmillan.com");
                course = CreateCourse();
                _courseActions.CreateUserEnrollment(_user.Id, course.Id, _createdDomain.Id, true);
                _courseActions.CreateUserEnrollment(_user2.Id, course.Id, _createdDomain.Id, true);

                CourseResponse response = _courseController.Details(course.Id);
                var results = response.results;
                Assert.IsTrue(results.Instructors.Count() > 1);
            }
            finally
            {
                DeleteCourse(course);
                DeleteUsers(_user2);
            }
        }

        [TestMethod]
        public void GetTerms_Returns_AtLeastOneTerm()
        {
            Assert.IsTrue(_terms.Any());  //Since Terms are just labels - make sure that at least one is returned.
        }

        [TestMethod]
        public void GetInstructors_Returns_Instructors()
        {
            Bdc.Course course = null;
            Adc.Enrollment enrollment = null;
            List<User> instructors = null;
            try
            {
                course = CreateCourse();
                enrollment = _courseActions.CreateUserEnrollment(_user.Id, course.Id, _createdDomain.Id, true);
                instructors = _courseController.GetInstructors(_createdDomain.Id, _terms.First().Id).ToList();
                var instructor = instructors.First();
                Assert.AreEqual(_user.Id, instructor.Id);
                Assert.AreEqual(_user.Email, instructor.Email);
                Assert.AreEqual(_user.FormattedName, instructor.FormattedName);
            }
            finally
            {
                _enrollmentActions.DropEnrollment(EntityExtensions.ToEnrollment(enrollment));
                DeleteCourse(course);
            }
        }

        [TestMethod]
        public void GetInstructorCourseList_Returns_CourseList()
        {
            Bdc.Course course = null;
            Adc.Enrollment enrollment = null;
            try
            {
                course = CreateCourse();
                enrollment = _courseActions.CreateUserEnrollment(_user.Id, course.Id, _createdDomain.Id, true);
                var courseList = _courseController.GetInstructorCourseList(_createdDomain.Id, _terms.First().Id, _user.Id);
                var course1 = courseList.First();

                Assert.AreEqual(course.Id, course1.Id);
                Assert.AreEqual(course.Title, course1.Title);
            }
            finally
            {
                _enrollmentActions.DropEnrollment(EntityExtensions.ToEnrollment(enrollment));
                DeleteCourse(course);
            }
        }

    }
}
