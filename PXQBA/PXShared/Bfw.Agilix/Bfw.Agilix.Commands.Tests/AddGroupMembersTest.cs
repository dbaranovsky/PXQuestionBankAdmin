using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using TestHelper;
namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class AddGroupMembersTest
    {
        private AddGroupMembers GroupMember;

        [TestInitialize]
        public void TestInitialize()
        {
            this.GroupMember = new AddGroupMembers();
        }
        [TestMethod]
        public void AddGroupMembers_Request_Initialized()
        {
            this.GroupMember.GroupMember = new GroupMember {EnrollmentId= "10128", GroupId="190918" };
            var request = this.GroupMember.ToRequest();
            Assert.AreEqual("addgroupmembers", request.Parameters["cmd"]);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void AddGroupMembers_Response_Should_Throw_Exception_If_Response_Not_OK()
        {
            this.GroupMember.ParseResponse(new DlapResponse() { Code = DlapResponseCode.Error });
        }
    }
}
