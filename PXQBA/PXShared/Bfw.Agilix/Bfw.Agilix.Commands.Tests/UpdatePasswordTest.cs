using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Xml.Linq;
using System.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class UpdatePasswordTest
    {
        private UpdatePassword updatePassword;

        [TestInitialize]
        public void TestInitialize()
        {
            this.updatePassword = new UpdatePassword();
        }
        
        [TestMethod]
        public void UpdatePasswordTest_Request_Type_Should_Be_Post()
        {
            updatePassword.UserId = "99999";
            var request = updatePassword.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void UpdatePasswordTest_Request_DlapRequest_Attribute_Should_Have_Command_UpdatePassword()
        {
            updatePassword.UserId = "99999";
            var request = updatePassword.ToRequest();
            Assert.AreEqual(request.Attributes["cmd"], "updatepassword");
        }

        [TestMethod]
        public void UpdatePasswordTest_Request_Userid_Attribute_Should_Have_Same_UserId()
        {
            updatePassword.UserId = "99999";
            var request = updatePassword.ToRequest();
            Assert.AreEqual(request.Attributes["userid"], "99999");
        }

        [TestMethod]
        public void UpdatePasswordTest_Request_Password_Attribute_Should_Have_Same_Password()
        {
            updatePassword.UserId = "99999";
            updatePassword.Password = "fakepassword";
            var request = updatePassword.ToRequest();
            Assert.AreEqual(request.Attributes["password"], "fakepassword");
        }

        [TestMethod]
        public void UpdatePasswordTest_ParseResponse_Should_Set_Success_To_True_If_Response_Code_Is_OK()
        {         
            updatePassword.UserId = "99999";
            var dlapResponse = new DlapResponse()
            {
                Code = DlapResponseCode.OK
            };
            updatePassword.ParseResponse(dlapResponse);
            Assert.AreEqual(updatePassword.Success, true);
        }

        [TestMethod]
        public void UpdatePasswordTest_ParseResponse_Should_Set_Success_To_False_If_Response_Code_Is_Error()
        {
            updatePassword.UserId = "99999";
            var dlapResponse = new DlapResponse()
            {
                Code = DlapResponseCode.Error
            };
            updatePassword.ParseResponse(dlapResponse);
            Assert.AreEqual(updatePassword.Success, false);
        }
        
    }
}
