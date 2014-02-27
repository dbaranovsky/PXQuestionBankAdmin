using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Xml.Linq;
using System.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class UpdateUsersTest
    {
        private UpdateUsers updateUsers;

        [TestInitialize]
        public void TestInitialize()
        {
            this.updateUsers = new UpdateUsers();
        }

        [TestMethod]
        public void UpdateUsersTest_Clear_Users_Should_Clear_Users_From_The_List()
        {
            updateUsers.Clear();
            updateUsers.Add(new AgilixUser() { Id = "something" });
            updateUsers.Clear();
            Assert.AreEqual(updateUsers.Users.Count(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Cannot update a UpdateUsers request if there are no user in the Users collection.")]
        public void UpdateUsersTest_Request_Should_Throw_Exception_If_Users_Is_Null()
        {            
            updateUsers.ToRequest();
        }

        [TestMethod]
        public void UpdateUsersTest_Request_DlapRequest_Type_Should_Be_PostRequest()
        {
            updateUsers.Add(new AgilixUser() { Id = "something" });
            var request = updateUsers.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void UpdateUsersTest_Request_DlapRequest_Mode_Should_Be_BatchRequest()
        {
            updateUsers.Add(new AgilixUser() { Id = "something" });
            var request = updateUsers.ToRequest();
            Assert.AreEqual(request.Mode, DlapRequestMode.Batch);
        }

        [TestMethod]
        public void UpdateUsersTest_Request_DlapRequest_Parameters_Should_Have_Command_UpdateUsers()
        {
            updateUsers.Add(new AgilixUser() { Id = "something" });
            var request = updateUsers.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "updateusers");
        }
        

        [TestMethod]
        [ExpectedException(typeof(DlapException), "UpdateUsers request failed with response code Error.")]
        public void UpdateUsersTest_ParseResponse_Should_Throw_Exception_If_Response_Code_Is_Not_OK()
        {
            updateUsers.Add(new AgilixUser() { Id = "something" });
            var dlapResponse = new DlapResponse()
            {
                Code = DlapResponseCode.Error
            };
            updateUsers.ParseResponse(dlapResponse);
        }

        [TestMethod]
        public void UpdateUsersTest_UpdateResult_Should_Be_False_If_Any_Batch_Response_Fails()
        {
            string responseString = "<response code=\"OK\"><responses>" +
                                    "<response code=\"DoesNotExist\" message=\"Entity /109512312328 not found.\">" +
                                    "<detail>GoCourseServer.DataModel.DoesNotExistException:</detail>" +
                                    "</response>  </responses> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            updateUsers.Add(new AgilixUser() { Id = "something" });
            updateUsers.ParseResponse(response);
            Assert.IsFalse(updateUsers.UpdateResult);

        }

        [TestMethod]
        public void UpdateUsersTest_UpdateResult_Should_Be_True_If_No_Batch_Response_Fails()
        {
            string responseString = "<response code=\"OK\"><responses>" +
                                    "<response code=\"OK\" message=\"Entity /109512312328 found.\">" +                                    
                                    "</response>  </responses> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            updateUsers.Add(new AgilixUser() { Id = "something" });
            updateUsers.ParseResponse(response);
            Assert.IsTrue(updateUsers.UpdateResult);

        }
       
    }
}
