using System;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class CreateUsersTest
    {
        private CreateUsers _cmd;
        [TestInitialize]
        public void TestInitialize()
        {
            _cmd = new CreateUsers();
        }

        [TestMethod]
        public void CreateUsersAction_CreateObject_UsersShouldBeInitialized()
        {
            Assert.IsNotNull(_cmd.Users);
        }

        [TestMethod]
        public void CreateUsersAction_AddUser_UsersShouldBePopulated()
        {
            _cmd.Add(new DataContracts.AgilixUser());

            Assert.AreEqual<int>(_cmd.Users.Count, 1);
        }

        [TestMethod]
        public void CreateUsersAction_AddUser_RequestTypeShouldBePost()
        {
            _cmd.Add(new DataContracts.AgilixUser());
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<DlapRequestType>(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void CreateUsersAction_AddUser_RequestModeShouldBeBatch()
        {
            _cmd.Add(new DataContracts.AgilixUser());
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<DlapRequestMode>(request.Mode, DlapRequestMode.Batch);
        }

        [TestMethod]
        public void CreateUsersAction_AddUser_RequestParametersShouldBeInitialized()
        {
            _cmd.Add(new DataContracts.AgilixUser());
            DlapRequest request = _cmd.ToRequest();
            Assert.IsNotNull(request.Parameters);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateUsersAction_GetRequestWithoutUser_ThrowDlapException()
        {
            DlapRequest request = _cmd.ToRequest();
        }

        [TestMethod]
        public void CreateUsersAction_ClearUser_UsersShouldBeEmpty()
        {
            _cmd.Add(new DataContracts.AgilixUser());
            _cmd.Clear();

            Assert.AreEqual<int>(_cmd.Users.Count, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateUsersAction_ParseInvalidResponse_ThrowDlapException()
        {
            _cmd.ParseResponse(new DlapResponse() { Code = DlapResponseCode.Error });
        }

        [TestMethod]
        public void CreateUsersAction_ParseValidResponse_UserObjectWithProperIdCreated()
        {
            string responseString = @"<response code=""OK"">
  <responses>
    <response code=""OK"">
      <user userid=""128810"" />
    </response>
  </responses>
</response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            _cmd.Add(new DataContracts.AgilixUser());
            _cmd.ParseResponse(response);
            Assert.AreEqual(_cmd.Users[0].Id, "128810");
        }
    }
}
