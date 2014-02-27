using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Mhe.GrantAccess.DAL.Models;
using Mhe.GrantAccess.DAL.Contracts;
using Mhe.GrantAccess.DataContracts;
using Mhe.GrantAccess.ServiceContracts;
using Mhe.GrantAccess.Services;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;

using NSubstitute;

namespace GrantAccess.Tests
{
    [TestClass]
    public class AccessServiceTests
    {
        [TestMethod]
        public void GrantAdminToolAccess()
        {
            // arrange            
            var userStore = Substitute.For<IUserStore>();
            var rightsStore = Substitute.For<IRightsStore>();
            var courseStore = Substitute.For<ICourseStore>();
            var accessService = new AccessService(userStore, rightsStore, courseStore);

            var rights = Access.AdminSandbox;
            var userName = "joe@test.com";
            var courseId = "1234";

            userStore.UserReferenceId(userName).Returns("456");            
            
            // act
            var result = accessService.Grant(
                access: rights, 
                to: userName, 
                forCourse: courseId,
                inDomain: "6650"
            );

            // assert
            Assert.AreEqual(false, result.Error);
        }
    }
}
