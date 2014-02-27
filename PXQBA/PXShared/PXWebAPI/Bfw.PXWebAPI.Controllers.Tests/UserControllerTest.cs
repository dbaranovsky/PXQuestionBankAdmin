using System;
using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Bfw.Common.Patterns.Unity;
using System.Web.Mvc;
using System.Configuration;
using Bfw.Agilix.Dlap.Session;
using Bdc = Bfw.PX.Biz.DataContracts;
using Bsc = Bfw.PX.Biz.ServiceContracts;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.Response;
using Bfw.Agilix.Dlap.Components.Session;
using System.Collections.Generic;
using Bfw.Agilix.Commands;
using Bfw.Common.Collections;
using Bfw.PXWebAPI.Helpers;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PXWebAPI.Helpers.Services;

namespace Bfw.PXWebAPI.Controllers.Tests
{
    [TestClass]
    public class UserControllerTest
    {
        private Bdc.UserInfo user;
        private UserController userController;
        private Bsc.IUserActions userActions;
        private ISession currentSession;
        private IApiCourseActions courseActions;
        private string productCourseId = "109554";
        private string domainId = "66159";
        private string staticRaId = "6669870";

        public UserControllerTest()
        {
            var dummyObject = new ThreadSessionManager(null, null); //to force MsTest to copy Bfw.Agilix.Dlap.Components to output directory
            ConfigureServiceLocator();
            InitializeSessionManager();
            userController = ServiceLocator.Current.GetInstance<UserController>();
            userActions = ServiceLocator.Current.GetInstance<Bsc.IUserActions>();
            courseActions = ServiceLocator.Current.GetInstance<IApiCourseActions>();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            // attempt to delete here in case the cleanup does not run
            var users = userActions.ListUsersLike(new Bdc.UserInfo() { Username = staticRaId });
            if (users != null)
            {
                DeleteUsers(users.ToArray());
            }
            user = CreateUser();
        }

        [TestMethod]
        public void CheckandCreateUser_NewUser()
        {
            UserResponse response = null;
            Models.User usr = null;
            string rauserid = "9999123";
            try
            {
                response = userController.CheckandCreateUser(rauserid, "8841", "testing@test.com", "Thomas", "Edison", "");
                usr = response.results;
                Assert.IsNotNull(usr);
                Assert.AreEqual("Thomas", usr.FirstName);
                Assert.AreEqual("Edison", usr.LastName);
                Assert.AreEqual("8841", usr.DomainId);
                Assert.AreEqual("testing@test.com", usr.Email);
                Assert.AreEqual(rauserid, usr.Username);
            }
            finally
            {
                DeleteUsers(usr);
            }
        }

        [TestMethod]
        public void CheckandCreateUser_ExistingUser()
        {
            UserResponse response = null;
            Bfw.PXWebAPI.Models.User usr = null;
            Bdc.UserInfo user2 = CreateUser();
            Assert.IsNotNull(user2);
            try
            {
                response = userController.CheckandCreateUser(user2.Username, user2.DomainId, user2.Email, user2.FirstName, user2.LastName, user2.DomainName);
                usr = response.results;
                Assert.IsNotNull(usr);
                Assert.AreEqual(user2.FirstName, usr.FirstName);
                Assert.AreEqual(user2.LastName, usr.LastName);
                Assert.AreEqual(user2.DomainId, usr.DomainId);
                Assert.AreEqual(user2.Email, usr.Email);
                Assert.AreEqual(user2.Username, usr.Username);
            }
            finally
            {
                DeleteUsers(user2);
            }
        }

        [TestMethod]
        public void UpdateFirstAndLastName_WithUserId_PasswordAndEmailShouldNotChange()
        {
            var editUser = new EditUser { FirstName = "George", LastName = "Bush" };
            var response = userController.Update(user.Id, false, editUser) as BoolResponse;
            Assert.IsTrue(response.results);
            //if login is successful then the password did not change
            var loggedInUser = userActions.Login(String.Format("bfwusers/{0}", user.Username), user.Password);
            Assert.AreNotEqual(loggedInUser.FirstName, user.FirstName);
            Assert.AreNotEqual(loggedInUser.LastName, user.LastName);
            Assert.AreEqual(loggedInUser.Email, user.Email);
            //check again for email change because .... just because
            var user2 = userActions.GetUser(user.Id);
            Assert.AreEqual(user.Email, user2.Email);
        }

        [TestMethod]
        public void UpdateFirstAndLastName_WithRAId_PasswordAndEmailShouldNotChange()
        {
            var editUser = new EditUser { FirstName = "Frank", LastName = "Zappa" };
            var response = userController.Update(user.Username, true, editUser) as BoolResponse;
            Assert.IsTrue(response.results);
            var loggedInUser = userActions.Login(String.Format("bfwusers/{0}", user.Username), user.Password);
            Assert.AreNotEqual(loggedInUser.FirstName, user.FirstName);
            Assert.AreNotEqual(loggedInUser.LastName, user.LastName);
            Assert.AreEqual(loggedInUser.Email, user.Email);
            //check again for email change because .... just because
            var user2 = userActions.GetUser(user.Id);
            Assert.AreEqual(user.Email, user2.Email);
        }

        [TestMethod]
        public void GetUserById()
        {
            var userResponse = userController.PxUserDetails(user.Id);
            var retrievedUser = userResponse.results;
            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual(user.Id, retrievedUser.Id);
            Assert.AreEqual(user.Email, retrievedUser.Email);
            Assert.AreEqual(user.FirstName, retrievedUser.FirstName);
            Assert.AreEqual(user.LastName, retrievedUser.LastName);
            Assert.AreEqual(user.DomainId, retrievedUser.DomainId);
        }

        [TestMethod]
        public void GetDetails_ByUserId()
        {
            var userList = userController.Details(user.Id);
            Assert.AreEqual(1, userList.results.Count);
            var retrievedUser = userList.results.First();
            Assert.IsNotNull(retrievedUser);
            Assert.AreEqual(user.Id, retrievedUser.Id);
            Assert.AreEqual(user.Email, retrievedUser.Email);
            Assert.AreEqual(user.FirstName, retrievedUser.FirstName);
            Assert.AreEqual(user.LastName, retrievedUser.LastName);
            Assert.AreEqual(user.DomainId, retrievedUser.DomainId);
        }

        [TestMethod]
        public void GetDetails_ByRAId()
        {
            Bdc.UserInfo user2 = null;
            try
            {
                //create another user in a different domain
                user2 = CreateUser(domainId);
                var response = userController.Details(user.Username, true);
                var list = response.results;
                Assert.AreEqual(2, list.Count);
                var usr1 = list[0];
                var usr2 = list[1];
                Assert.IsTrue(usr1.Username == usr2.Username);
                Assert.IsTrue(usr1.Email == usr2.Email);
                Assert.IsNotNull(list.FirstOrDefault(u => u.DomainId == domainId));
                Assert.IsNotNull(list.FirstOrDefault(u => u.DomainId == user.DomainId));
            }
            finally
            {
                DeleteUsers(user2);
            }
        }

        [TestMethod]
        public void GetCoursesForInstructor()
        {
            Bdc.Course course = null;
            try
            {
                course = CreateCourse();
                courseActions.CreateUserEnrollment(user.Id, course.Id, domainId, true);
                var response = userController.GetCoursesForInstructor(user.Id);
                Assert.AreEqual(1, response.results.Count);
                var retrievedCourse = response.results.First();
                Assert.AreEqual(course.Id, retrievedCourse.Id);
            }
            finally
            {
                DeleteCourse(course);
            }
        }

        [TestMethod]
        public void CheckandCreateUserEnrollment_EnrollmentDoesNotExist()
        {
            Bdc.Course course = null;
            try
            {
                course = CreateCourse();
                var response = userController.CheckandCreateUserEnrollment(user.Id, course.Id, domainId);
                Assert.IsNotNull(response.results);
                var newEnrollment = response.results;
                Assert.AreEqual(course.Id, newEnrollment.CourseId);
                Assert.AreEqual(domainId, newEnrollment.DomainId);
                Assert.AreEqual(user.Id, newEnrollment.User.Id);
            }
            finally
            {
                DeleteCourse(course);
            }
        }

        [TestMethod]
        public void CheckandCreateUserEnrollment_EnrollmentExists()
        {
            Bdc.Course course = null;
            try
            {
                course = CreateCourse();
                var enrollmentId = courseActions.CreateUserEnrollment(user.Id, course.Id, domainId).Id;
                var response = userController.CheckandCreateUserEnrollment(user.Id, course.Id, domainId);
                Assert.IsNotNull(response.results);
                var enrollment = response.results;
                Assert.AreEqual(enrollmentId, enrollment.Id);
            }
            finally
            {
                DeleteCourse(course);
            }
        }

        [TestMethod]
        public void DropUserEnrollment()
        {
            Bdc.Course course = null;
            var user2 = CreateUser(domainId);
            try
            {
                course = CreateCourse();
                var enrollmentId = courseActions.CreateUserEnrollment(user2.Id, course.Id, domainId);
                var response = userController.DropUserEnrollment(user2.Username, course.Id);
                Assert.IsNotNull(response.results);
                var result = response.results;
                Assert.AreEqual(true, result);
            }
            finally
            {
                DeleteCourse(course);
                DeleteUsers(user2);
            }
        }

        [TestMethod]
        public void NewStudentEnrollment()
        {
            //create a new RA user - student account
            //email must be in the format of UnitTest.Student10000@Macmillan.com 
            var coreServices = new CoreServices();
            string errorCode = "";
            string newRaUserId = "";
            var random = new Random();
            
            do
            {
                var email = GenerateRaUserEmail(random);
                var response = coreServices.RegisterUser(email, "Password1", "Same old", "Bill", "Clinton");
                newRaUserId = response.UserIdInfo.Id;
                errorCode = response.Error.Code;
            }

            while (errorCode == "-100");
            Assert.IsNotNull(newRaUserId);
            //redeem access code
            var accessId = ConfigurationManager.AppSettings["RA_AccessID"];
            var responseCheck = coreServices.CheckUserAssignment(newRaUserId, accessId);
            Assert.AreEqual("0", responseCheck.Error.Code);
            //create a brainhoney user
            Bdc.UserInfo user2 = null;
            try
            {
                //create another user with a different RA Id
                user2 = CreateUser(domainId, newRaUserId);
                Assert.IsNotNull(user2);
                Assert.AreEqual(newRaUserId, user2.Username);
                Assert.AreEqual(domainId, user2.DomainId);
                Bdc.Course course = null;
                try
                {
                    course = CreateCourse();

                    var response = userController.CheckandCreateUserEnrollment(user2.Id, course.Id, domainId);
                    Assert.IsNotNull(response.results);
                    var newEnrollment = response.results;
                    Assert.AreEqual(course.Id, newEnrollment.CourseId);
                    Assert.AreEqual(domainId, newEnrollment.DomainId);
                    Assert.AreEqual(user2.Id, newEnrollment.User.Id);
                }
                finally
                {
                    DeleteCourse(course);
                }
            }
            finally
            {
                DeleteUsers(user2);
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            DeleteUsers(user);
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
            currentSession = sessionManager.StartNewSession(userName, password, false, userId);
            sessionManager.CurrentSession = currentSession;
        }

        private Bdc.UserInfo CreateUser(string domainId = null, string raUserId = null)
        {
            var username = raUserId ?? staticRaId;
            var password = "Password1";
            var firstName = "Louis";
            var lastName = "Schlouis";
            var email = "lschlouis@gmail.com";
            return userActions.CreateUser(username, password, "", "", firstName, lastName, email, domainId ?? "8841", "", "");
        }

        private void DeleteUsers(params Bdc.UserInfo[] users)
        {
            if (users != null)
            {
                userActions.DeleteUsers(users);
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
                userActions.DeleteUsers(list.ToArray());
            }
        }

        private Bdc.Course CreateCourse()
        {
            var courses = new List<Bdc.Course> { new Bdc.Course { Title = "DummyTestCourse", 
                                                 Domain = new Bdc.Domain { Id = domainId }, 
                                                 DashboardSettings = new Bdc.DashboardSettings(), 
                                                 ProductCourseId = productCourseId }
                                                };
            var cmd = new CreateCourses();
            cmd.Add(courses.Map(c => c.ToCourse()).ToList());
            currentSession.ExecuteAsAdmin(cmd);
            var result = cmd.Entity.Map(c => c.ToCourse()).ToList();
            return result.First();
        }

        private void DeleteCourse(Bdc.Course course)
        {
            course.DashboardSettings = new Bdc.DashboardSettings();
            List<Bdc.Course> courses = new List<Bdc.Course> { course };
            var cmd = new DeleteCourses();
            cmd.Add(courses.Map(c => c.ToCourse()));
            currentSession.ExecuteAsAdmin(cmd);
        }

        private string GenerateRaUserEmail(Random random)
        {
            var integer = random.Next(1, 1000000);
            return String.Format("UnitTest.Student{0}@Macmillan.com", integer);
        }
    }
}
