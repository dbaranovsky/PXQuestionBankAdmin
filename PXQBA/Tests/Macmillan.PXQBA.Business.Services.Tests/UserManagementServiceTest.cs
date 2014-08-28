using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services.Automapper;
using Macmillan.PXQBA.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class UserManagementServiceTest
    {
 
        private AutomapperConfigurator automapperConfigurator;
        private IUserNotificationOperation userNotificationOperation;
        private IRoleOperation roleOperation;
        private IModelProfileService modelProfileService;

        private IUserManagementService userManagementService;

        [TestInitialize]
        public void TestInitialize()
        {

            userNotificationOperation = Substitute.For<IUserNotificationOperation>();
            roleOperation = Substitute.For<IRoleOperation>();
            modelProfileService = Substitute.For<IModelProfileService>();

            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();

            userManagementService = new UserManagementService(userNotificationOperation, roleOperation);
        }


        [TestMethod]
        public void GetRole_roleIdIsNull_NewRole()
        {
            const string courseId = "123";
            int? roleId = null;

            var result = userManagementService.GetRole(courseId, roleId);

            Assert.IsTrue(result.Name == "New Role");
            Assert.IsTrue(result.Id == 0);
        }

        [TestMethod]
        public void GetRole_roleIdIs0_NewRole()
        {
            const string courseId = "123";
            int? roleId = 0;

            var result = userManagementService.GetRole(courseId, roleId);

            Assert.IsTrue(result.Name == "New Role");
            Assert.IsTrue(result.Id == 0);
        }

        [TestMethod]
        public void GetNotShownNotification_NoParams_NotShownNotification()
        {
            userNotificationOperation.GetNotShownNotification().Returns(new List<UserNotShownNotification>()
                                                                        {
                                                                            new UserNotShownNotification(),
                                                                            new UserNotShownNotification()
                                                                        });

           Assert.IsTrue(userManagementService.GetNotShownNotification().Count() == 2);
        }
        
        [TestMethod]
        public void UpdateUserRoles_CorrectUserToUpdate_SuccessRun()
        {
            roleOperation.UpdateUserRoles(Arg.Do<QBAUser>(x => Assert.IsTrue(x.FullName == "Test")));
            userManagementService.UpdateUserRoles(new QBAUser()
                                                  {
                                                      FullName = "Test"
                                                  });
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UpdateUserRoles_NotCorrecttUserToUpdate_FailWithNullReference()
        {
            roleOperation.UpdateUserRoles(Arg.Do<QBAUser>(x => { throw new NullReferenceException(); }));
            userManagementService.UpdateUserRoles(new QBAUser());
        }

        [TestMethod]
        public void DeleteRole_CorrectIdToDelete_SuccessRun()
        {
            roleOperation.DeleteRole(Arg.Do<int>(x => Assert.IsTrue(x == 1)));
            userManagementService.DeleteRole(1);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void DeleteRole_NotCorrectIdToDelete_FailWithNullReference()
        {
            roleOperation.DeleteRole(Arg.Do<int>(x => { throw new NullReferenceException(); }));
            userManagementService.DeleteRole(1);
        }


        [TestMethod]
        public void GetUser_CorrectId_SuccessRun()
        {
            roleOperation.GetUserWithRoles(Arg.Do<string>(x => Assert.IsTrue(x == "23"))).Returns(new QBAUser()
                                                                                                  {
                                                                                                      FullName = "Test"
                                                                                                  });
            Assert.IsTrue( userManagementService.GetUser("23").FullName=="Test");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetUser_NotCorrectId_FailWithNullReference()
        {
            roleOperation.GetUserWithRoles(Arg.Do<string>(x => { throw new NullReferenceException(); }));
            userManagementService.GetUser("23");
        }


        
        [TestMethod]
        public void GetUserCapabilities_CorrectCourseId_SuccessRun()
        {
            roleOperation.GetUserCapabilities(Arg.Do<string>(x => Assert.IsTrue(x == "23"))).Returns(new List<Capability>()
                                                                                                     {
                                                                                                         new Capability()
                                                                                                     });
            Assert.IsTrue( userManagementService.GetUserCapabilities("23").Count()== 1);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetUserCapabilities_NotCorrectCourseId_FailWithNullReference()
        {
            roleOperation.GetUserCapabilities(Arg.Do<string>(x => { throw new NullReferenceException(); }));
            userManagementService.GetUserCapabilities("23");
        }
        

        [TestMethod]
        public void GetUsers_StartAndRecordCounts_CollectionOfQBAUsers()
        {
            roleOperation.GetQBAUsers(Arg.Do<int>(x => Assert.IsTrue(x == 1)), Arg.Do<int>(x => Assert.IsTrue(x == 6))).Returns(new PagedCollection<QBAUser>()
                                                                             {
                                                                                 CollectionPage = new List<QBAUser>()
                                                                                                  {
                                                                                                      new QBAUser(),
                                                                                                      new QBAUser()
                                                                                                  }
                                                                             });
            var users =  userManagementService.GetUsers(1, 6);
            Assert.IsTrue( users.CollectionPage.Count() == 2);
        }
        
      


        [TestMethod]
        public void UpdateRole_CorrectIdToUpdate_SuccessRun()
        {
            roleOperation.UpdateRolesCapabilities(Arg.Do<string>(x => Assert.IsTrue(x == "12")), Arg.Do<IEnumerable<Role>>( x=> Assert.IsTrue( x.First().Name=="test")));
            userManagementService.UpdateRole("12", new Role()
                                                   {
                                                       Name = "test"
                                                   });
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UpdateRole_NotCorrectIdToUpdate_FailWithNullReference()
        {
            roleOperation.UpdateRolesCapabilities(Arg.Do<string>(x => { throw new NullReferenceException(); }), Arg.Do<IEnumerable<Role>>(x => Assert.IsTrue(x.First().Name == "test")));
            userManagementService.UpdateRole("12", new Role()
            {
                Name = "test"
            });
        } 




        [TestMethod]
        public void CreateNotShownNotification_NotificationToCreate_CorrectParamsPassed()
        {
            userNotificationOperation.CreateNotShownNotification(Arg.Do<NotificationType>(x => Assert.IsTrue(x == NotificationType.PublishChangesMadeWithinDraft)));
            userManagementService.CreateNotShownNotification(NotificationType.PublishChangesMadeWithinDraft);
        }



        [TestMethod]
        public void GetRole_roleIdIs1_GetRoleWithCapabilitiesInvoked()
        {
            const string courseId = "123";
            int? roleId = 1;
            const string testRole = "TestRole";

            roleOperation.GetRoleWithCapabilities(Arg.Is<int>(a => a == 1)).Returns(new Role() {Id = 1, Name = testRole });

            var result = userManagementService.GetRole(courseId, roleId);

            Assert.IsTrue(result.Name == testRole);
            Assert.IsTrue(result.Id == 1);
        }


        [TestMethod]
        public void GetRolesForCourse_CourseId_UpdateRolesCapabilitiesForPredefinedRoles()
        {
            bool updateRolesCapabilitiesSuccess = false;
            const string courseId = "123";

            roleOperation.When(r => r.UpdateRolesCapabilities(Arg.Is<string>(a => a == courseId),
                                                             Arg.Is<IEnumerable<Role>>(a => 
                                                             IsRolesEqual(a,PredefinedRoleHelper.GetPredefinedRoles()))))
                                                            .Do(x=> { updateRolesCapabilitiesSuccess = true; });
                                                          

            userManagementService.GetRolesForCourse(courseId);

            Assert.IsTrue(updateRolesCapabilitiesSuccess);
        }

        private bool IsRolesEqual(IEnumerable<Role> roles1, IEnumerable<Role> roles2)
        {
            var roles1Array = roles1.ToArray();
            var roles2Array = roles2.ToArray();
            return !roles1Array.Where((t, i) => t.Name != roles2Array[i].Name).Any();
        }
    }
}
