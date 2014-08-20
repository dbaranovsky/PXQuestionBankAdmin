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
