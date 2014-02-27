namespace Bfw.Agilix.Commands.Tests
{
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Bfw.Agilix.Dlap;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass, Ignore]
    // ReSharper disable InconsistentNaming
    public class DeleteQuestionsTest
    {
        [TestMethod]
        [ExpectedException(typeof(DlapException), "Cannot create a DeleteQuestions request if there are not items in the Items collection")]
        public void DeleteQuestion_Should_ThrowException_If_QuestionsCollection_IsNull()
        {
            // Arrange
            var deleteQuestions = new DeleteQuestions();

            // Act
            deleteQuestions.ToRequest();
        }

        [TestMethod]
        public void CommandName_Is_DeleteQuestions_For_This_Command()
        {
            // Arrange
            XDocument expectedResult = XDocument.Parse(@"<requests><question entityid=""entityid"" questionid=""qid1"" /><question entityid=""entityid"" questionid=""qid2"" /></requests>");
            var deleteQuestions = new DeleteQuestions
            {
                Questions =
                    new List<XElement>{ 
                        (new XElement("question", new XAttribute("entityid", "entityid"), new XAttribute("questionid", "qid1"))),
                        (new XElement("question", new XAttribute("entityid", "entityid"), new XAttribute("questionid", "qid2")))
                    }
            };

            // Act
            var request = deleteQuestions.ToRequest();
            var actualResult = request.GetXmlRequestBody();

            // Assert
            Assert.AreEqual(expectedResult.ToString(), actualResult.ToString(), "Failed to create DLAP request for DELETE questions");
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Delete Questions response failed - without response code OK")]
        public void Parse_DeleteQuestions_With_Incorrect_ResponseCode_Throws_Error()
        {
            // Arrange
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.AccessDenied };

            // Act
            (new DeleteQuestions()).ParseResponse(dlapResponse);
        }

        [TestMethod]
        public void Parse_QuestionDelete_Response_With_Some_Failures()
        {
            // Arrage
            string responseXml =
                @"<response code=""OK""><responses><response code=""OK"" /><response code=""BadRequest"" message=""The specified entity id (id) is not valid! Parameter 'entityid'.""><detail>GoCourseServer.BadRequestException: The specified entity id (id) is not valid! Parameter 'entityid'.at GoCourseServer.EntityRef.TryParse(DlapContext context, Char entityType, XPathNavigator node, String parameterName)at GoCourseServer.EntityRef.Parse(DlapContext context, Char entityType, XPathNavigator node, String parameterName)at GoCourseServer.QuestionRequestHandler.DeleteQuestions(DlapContext context)agent='Firefox23.0' url='http://qa.dlap.bfwpub.com/Dlap.ashx?deletequestions' site='dlap' method='POST' referer='http://qa.dlap.bfwpub.com/CallMethod.aspx' process='8592' userid='2'</detail></response><response code=""AccessDenied"" message=""Access Denied: userId='2'""><detail>GoCourseServer.AccessDeniedException: Access Denied: userId='2'at GoCourseServer.QuestionRequestHandler.DeleteQuestions(DlapContext context)agent='Firefox23.0' url='http://qa.dlap.bfwpub.com/Dlap.ashx?deletequestions' site='dlap' method='POST' referer='http://qa.dlap.bfwpub.com/CallMethod.aspx' process='8592' userid='2'</detail></response></responses></response>";
            XDocument expectedResult = XDocument.Parse(responseXml);
            var deleteQuestion = new DeleteQuestions();


            // Act
            var dlapResponse = new DlapResponse(expectedResult);
            deleteQuestion.ParseResponse(dlapResponse);

            // Assert
            Assert.AreEqual(2, deleteQuestion.Failures.Count, "Failed to parse response for Delete Question Command.");
        }

    }
    // ReSharper restore InconsistentNaming
}