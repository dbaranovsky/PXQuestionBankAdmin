using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;


namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class PutQuestionsTest
    {
        private PutQuestions putQuestions;

        [TestInitialize]
        public void TestInitialize()
        {
            this.putQuestions = new PutQuestions();
            putQuestions.Add(new Question() 
            {
                Id = "something",
                EntityId = "something" ,
                InteractionData = "something",
                InteractionType = "something",
                Body = "something"
            });
        }

        [TestMethod]
        public void PutQuestionsTest_Add_Question_Should_Add_Questions_To_The_List()
        {
            putQuestions.Clear();
            putQuestions.Add(new Question() { Id="something"});
            Assert.AreEqual(putQuestions.Questions.Count(), 1);
        }

        [TestMethod]
        public void PutQuestionsTest_Add_Questions_Should_Add_Questions_To_The_List()
        {
            putQuestions.Clear();
            putQuestions.Add(
                    new List<Question> {
                        new Question()  {   Id="1"  },
                        new Question()  {   Id="2"  }
                    });
            Assert.AreEqual(putQuestions.Questions.Count(), 2);
        }

        [TestMethod]
        public void PutQuestionsTest_Clear_Questions_Should_Clear_Questions_From_The_List()
        {
            putQuestions.Clear();
            putQuestions.Add(new Question() { Id = "something" });
            putQuestions.Clear();
            Assert.AreEqual(putQuestions.Questions.Count(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "Cannot create a PutQuestions request if there are not Questions in the Questions collection.")]
        public void PutQuestionsTest_Request_Should_Throw_Exception_If_Questions_Is_Null()
        {
            putQuestions.Questions.RemoveAt(0) ;
            putQuestions.ToRequest();
        }

        [TestMethod]
        public void PutQuestionsTest_Request_DlapRequest_Type_Should_Be_PostRequest()
        {            
            var request = putQuestions.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void PutQuestionsTest_Request_DlapRequest_Mode_Should_Be_BatchRequest()
        {            
            var request = putQuestions.ToRequest();
            Assert.AreEqual(request.Mode, DlapRequestMode.Batch);
        }

        [TestMethod]
        public void PutQuestionsTest_Request_DlapRequest_Parameters_Should_Have_Command_UpdateUsers()
        {            
            var request = putQuestions.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "putquestions");
        }
       

        [TestMethod]
        [ExpectedException(typeof(DlapException), "PutQuestions request failed with response code Error.")]
        public void PutQuestionsTest_ParseResponse_Should_Throw_Exception_If_Response_Code_Is_Not_OK()
        {            
            var dlapResponse = new DlapResponse()
            {
                Code = DlapResponseCode.Error
            };
            putQuestions.ParseResponse(dlapResponse);
        }

        [TestMethod]
        public void PutQuestionsTest_Failures_List_Should_Have_Data_If_Any_Batch_Response_Fails()
        {
            string responseString = "<response code=\"OK\"><responses>" +
                                    "<response code=\"DoesNotExist\" message=\"Entity /109512312328 not found.\">" +
                                    "<detail>GoCourseServer.DataModel.DoesNotExistException:</detail>" +
                                    "</response>  </responses> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));            
            putQuestions.ParseResponse(response);
            Assert.AreEqual(putQuestions.Failures.Count(), 1);
        }

        [TestMethod]
        public void PutQuestionsTest_Failures_Reason_Should_Be_Failure_Code_If_No_Failure_Message_Mentioned()
        {
            string responseString = "<response code=\"OK\"><responses>" +
                                    "<response code=\"DoesNotExist\" >" +
                                    "<detail>GoCourseServer.DataModel.DoesNotExistException:</detail>" +
                                    "</response>  </responses> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            putQuestions.ParseResponse(response);
            Assert.AreEqual(putQuestions.Failures[0].Reason, "DoesNotExist");
        }


    }
}
