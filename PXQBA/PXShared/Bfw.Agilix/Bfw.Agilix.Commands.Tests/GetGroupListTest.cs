using System;
using System.Linq;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetGroupListTest
    {
        private GetGroupList groups;

        [TestInitialize]
        public void TestInitialize()
        {
            this.groups = new GetGroupList();            
        }

        [TestMethod]
        public void Request_Should_Be_Initialized()
        {
            var request = this.groups.ToRequest();

            Assert.AreEqual("GetGroupList", request.Parameters["cmd"]);
        }

        [TestMethod]
        public void Response_Should_Have_Groups()
        {
            var response = new DlapResponse();
            response.ResponseXml = Helper.GetResponse(Entity.GroupList, "GenericGroupList");

            this.groups.ParseResponse(response);

            Assert.IsTrue(this.groups.Groups.Count > 0);
        }

        [TestMethod]
        public void Response_Groups_Should_Have_Enrollments()
        {
            var response = new DlapResponse();
            response.ResponseXml = Helper.GetResponse(Entity.GroupList, "GroupListWithEnrollments");

            this.groups.ParseResponse(response);

            Assert.IsTrue((from s in groups.Groups select s.MemberEnrollments).Count() > 0);
        }

    }
}
