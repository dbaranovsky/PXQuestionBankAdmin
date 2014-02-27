using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Collections.Generic;
using Bfw.Common.Collections;
using TestHelper;
using System.Xml.Linq;
using System.IO;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetMessagesTest
    {
        private GetMessages getMessages;

        [TestInitialize]
        public void TestInitialize()
        {
            getMessages = new GetMessages();
            getMessages.SearchParameters = new MessageSearch()
            {
                EntityId = "something",
                DiscussionId = "something"
            };
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "GetMessages requires an enrollment id")]
        public void ToRequest_EntityIdIsNull_ExceptionThrown()
        {
            //Assign
            getMessages.SearchParameters.EntityId = null;
            //Act
            getMessages.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "GetMessages requires a discussion item id")]
        public void ToRequest_DiscussionIdIsNull_ExceptionThrown()
        {
            //Assign
            getMessages.SearchParameters.DiscussionId = null;
            //Act
            getMessages.ToRequest();
        }

        [TestMethod]
        public void ToRequest_MessageIdHasValue_CmdIsGetMessage()
        {
            //Assign
            getMessages.SearchParameters.MessageId = "messageValue";
            //Act
            var request = getMessages.ToRequest();
            //Assert
            Assert.AreEqual(request.Parameters["cmd"], "getmessage");
        }

		[TestMethod]
		public void ParseStreamResponse_Stream_ShouldHaveMessages_HasMessages()
		{
			//Assign
			var dlapResponse = new DlapResponse { ResponseStream = new MemoryStream() };
			//Act
			getMessages.ParseResponse(dlapResponse);
			//Assert
			Assert.IsNotNull(getMessages.Messages);
		}

		[TestMethod]
		public void ParseStreamResponse_XML_ShouldHaveMessage_HasMessage()
		{
			//Assign
			var messages = XDocument.Parse(Helper.GetContent(Entity.Messages));
			var dlapResponse = new DlapResponse { ResponseXml = messages };
			//Act
			getMessages.ParseResponse(dlapResponse);
			var messageCount = getMessages.Messages.Count;
			//Assert
			Assert.AreEqual(1, messageCount);
		}


    }
}
