using System;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class CreateGroupsTest
    {
        private CreateGroups _cmd;

        [TestInitialize]
        public void Setup()
        {
            _cmd = new CreateGroups();
        }

        [TestMethod]
        public void CreateGroupsAction_Initialize_GroupsNotNull()
        {
            Assert.IsNotNull(_cmd.Groups);
        }

        [TestMethod]
        public void CreateGroupsAction_AddGroup_GroupsHasCountOfOne()
        {
            _cmd.Add(new Group());
            Assert.AreEqual<int>(_cmd.Groups.Count, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateGroupsAction_CreateDlapRequestWithoutUser_ThrowDlapException()
        {
            _cmd.ToRequest();
        }

        [TestMethod]
        public void CreateGroupsAction_CreateDlapRequest_RequestTypeIsPost()
        {
            _cmd.Add(new Group());
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<DlapRequestType>(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void CreateGroupsAction_CreateDlapRequest_RequestModeIsBatch()
        {
            _cmd.Add(new Group());
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<DlapRequestMode>(request.Mode, DlapRequestMode.Batch);
        }

        [TestMethod]
        public void CreateGroupsAction_CreateDlapRequest_ParametersInitialized()
        {
            _cmd.Add(new Group());
            DlapRequest request = _cmd.ToRequest();
            Assert.IsNotNull(request.Parameters);
        }

        [TestMethod]
        public void CreateGroupsAction_CreateDlapRequest_ParametersHasCmd()
        {
            _cmd.Add(new Group());
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<string>(request.Parameters["cmd"].ToString(), "creategroups");
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateGroupsAction_ParseResponseWithErrorCode_ThrowsDlapException()
        {
            _cmd.ParseResponse(new DlapResponse() { Code = DlapResponseCode.Error });
        }

        [TestMethod]
        public void CreateGroupsAction_ParseValidResponse_ObjectPopularedWithCorrectGroupId()
        {
            string responseString = @"<response code=""OK"">
  <responses>
    <response code=""OK"">
      <group groupid=""204235""/>
    </response>
  </responses>
</response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            _cmd.Add(new DataContracts.Group());
            _cmd.ParseResponse(response);
            Assert.AreEqual(_cmd.GroupId, "204235");
        }
    }
}
