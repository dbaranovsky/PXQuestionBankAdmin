using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.DataContracts.Test
{
    [TestClass]
    public class GetUsersTest : BaseTest
    {
        #region Before and After each Test methods

        private const String
            path = @"GetUsersTest\",
            outputDir = "GetUsersTest";

        [TestInitialize]
        public void BeforeEachTest()
        {
        }

        [TestCleanup]
        public void AfterEachTest()
        {
        }

        #endregion

        #region Test Methods

        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void GetUserUsingIdWithOutResults()
        {
            var cmd = new GetUsers() { SearchParameters = new UserSearch() { Id = "124" } };

            try
            {
                ProcessCommand(cmd, getFileLocator("SingleUserRequestedNoUserFound"));
                Assert.Fail("Should have throw an exception");
            }
            catch (Exception e)
            {
                Assert.AreEqual(true,
                    e is DlapException &&
                    e.Message == "GetUsers command failed with code BadRequest", "Unexpected exception");
            }
        }

        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void GetUserUsingIdWithOneResult()
        {
            var cmd = new GetUsers() { SearchParameters = new UserSearch() { Id = "1" } };

            ProcessCommand(cmd, getFileLocator("SingleUserRequested"));
            var returnedUsers = cmd.Users;

            Assert.IsNotNull(returnedUsers, "Users Object");
            Assert.AreEqual<int>(1, returnedUsers.Count, "Number of users returned");

            var expected = new AgilixUser()
            {
                Id = "2",
                Reference = "234",
                Email = "test@macmillan.com",
                FirstName = "System",
                LastName = "Administrator",
                LastLogin = DateTime.Parse("2012-06-03T22:10:34.733Z"),
                Credentials = new Credentials()
                {
                    Username = "administrator",
                    UserSpace = "userSpace"
                },
                Domain = new Domain()
                {
                    Id = "1",
                    Name = "domainName"
                },
                GlobalId = new Guid("65015e20-f278-408a-807f-7b5be65736b7")
            };

            expected.Assert_AreEqual(returnedUsers.First(), null);
        }


        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void SearchForUsersWithOneResult()
        {
            var cmd = new GetUsers() { SearchParameters = new UserSearch() { DomainId = "1" } };

            ProcessCommand(cmd, getFileLocator("SingleUserResult"));
            var returnedUsers = cmd.Users;

            Assert.IsNotNull(returnedUsers, "Users Object");
            Assert.AreEqual<int>(1, returnedUsers.Count, "Number of users returned");

            var expected = new AgilixUser()
            {
                Id = "2",
                Reference = "234",
                Email = "test@macmillan.com",
                FirstName = "System",
                LastName = "Administrator",
                LastLogin = DateTime.Parse("2012-06-03T22:10:34.733Z"),
                Credentials = new Credentials()
                {
                    Username = "administrator",
                    UserSpace = "userSpace"
                },
                Domain = new Domain()
                {
                    Id = "1",
                    Name = "domainName"
                },
                GlobalId = new Guid("65015e20-f278-408a-807f-7b5be65736b7")
            };

            expected.Assert_AreEqual(returnedUsers.First(), null);
        }

        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void SearchForUsersWithTwoResult()
        {
            var cmd = new GetUsers() { SearchParameters = new UserSearch() { DomainId = "2" } };

            ProcessCommand(cmd, getFileLocator("TwoUserResult"));
            var returnedUsers = cmd.Users;

            Assert.IsNotNull(returnedUsers, "Users Object");
            Assert.AreEqual<int>(2, returnedUsers.Count, "Number of users returned");

            var expected = new AgilixUser()
            {
                Id = "1",
                Reference = "2",
                Email = "test@macmillan.com",
                FirstName = "System",
                LastName = "Administrator",
                LastLogin = DateTime.Parse("2012-06-03T22:10:34.733Z"),
                Credentials = new Credentials()
                {
                    Username = "administrator",
                    UserSpace = "userSpace"
                },
                Domain = new Domain()
                {
                    Id = "3",
                    Name = "domainName"
                },
                GlobalId = new Guid("65015e20-f278-408a-807f-7b5be65736b7")
            };

            expected.Assert_AreEqual(returnedUsers[0], null);

            var expected2 = new AgilixUser()
            {
                Id = "2",
                Reference = "3",
                Email = "test2@macmillan.com",
                FirstName = "System2",
                LastName = "Administrator2",
                LastLogin = DateTime.Parse("2012-06-03T22:10:34.730Z"),
                Credentials = new Credentials()
                {
                    Username = "administrator2",
                    UserSpace = "userSpace2"
                },
                Domain = new Domain()
                {
                    Id = "4",
                    Name = "domainName2"
                },
                GlobalId = new Guid("65015e20-f278-408a-807f-7b5be65736b8")
            };

            expected2.Assert_AreEqual(returnedUsers[1], null);
        }

        [TestMethod]
        [DeploymentItem(path, outputDir)]
        public void SearchForUsersUsingDomainWithNoResults()
        {
            var cmd = new GetUsers() { SearchParameters = new UserSearch() { DomainId = "88" } };

            ProcessCommand(cmd, getFileLocator("NoUsersFound"));
            var returnedUsers = cmd.Users;

            Assert.IsNotNull(returnedUsers, "Users Object");
            Assert.AreEqual<int>(0, returnedUsers.Count, "Number of users returned");
        }

        #endregion
    }
}
