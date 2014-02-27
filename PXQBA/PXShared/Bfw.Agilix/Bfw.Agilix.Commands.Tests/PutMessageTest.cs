using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class PutMessageTest
    {
        private PutMessage putMessage;

        [TestInitialize]
        public void TestInitialize()
        {
            this.putMessage = new PutMessage();
            this.putMessage.EntityId = "something";
            this.putMessage.DiscussionId = "something";
            var message = new Message()
            {
                Id = null
            };
            this.putMessage.Message = message;
            this.putMessage.Message.EnrollmentId = "something";
            this.putMessage.Message.Author = "something";

        }

        [TestMethod]
        public void PutMessageTest_Request_Message_Version_Should_Be_Zero_If_Message_Id_Is_Null()
        {          
            var request = putMessage.ToRequest();
            Assert.AreEqual(request.Parameters["version"], "0");
        }

        [TestMethod]
        public void PutMessageTest_Request_DlapRequest_Type_Should_Be_PostRequest()
        {
            var request = putMessage.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void PutMessageTest_Request_DlapRequest_EntityId_Should_Be_Initialized()
        {
            var request = putMessage.ToRequest();
            Assert.AreEqual(request.Parameters["entityid"], putMessage.EntityId);
        }

        [TestMethod]
        public void PutMessageTest_Request_DlapRequest_DiscussionId_Should_Be_Initialized()
        {
            var request = putMessage.ToRequest();
            Assert.AreEqual(request.Parameters["itemid"], putMessage.DiscussionId);
        }

        [TestMethod]
        public void PutMessageTest_Request_DlapRequest_MessageId_Should_Be_Initialized()
        {
            putMessage.Message.Id = "thisistheid";
            putMessage.Message.Version = "something";
            var request = putMessage.ToRequest();
            Assert.AreEqual(request.Parameters["messageid"], putMessage.Message.Id);
        }

        [TestMethod]
        public void PutMessageTest_Request_DlapRequest_Should_Be_PutMessage()
        {           
            var request = putMessage.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "putmessage");
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "PutMessage failed with code Error.")]
        public void PutMessageTest_ParseResponse_Should_Throw_Exception_If_Response_Code_Is_Not_OK()
        {
            var dlapResponse = new DlapResponse()
            {
                Code = DlapResponseCode.Error
            };
            putMessage.ParseResponse(dlapResponse);
        }


        [TestMethod]        
        public void PutMessageTest_ParseResponse_Message_Version_Should_Initialize_With_The_Version_Passed()
        {
            var dlapResponse = new DlapResponse()
            {
                ResponseXml = new System.Xml.Linq.XDocument()
            };
            putMessage.ParseResponse(dlapResponse);
            Assert.AreEqual(putMessage.Message.Version, null);
        }
    }
}
