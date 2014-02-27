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
using System.Linq;

namespace Bfw.Agilix.Commands.Tests
{
	[TestClass]
	public class GetQuestionsTest
	{
		private GetQuestions getQuestions;

		[TestInitialize]
		public void TestInitialize()
		{
			getQuestions = new GetQuestions();
			getQuestions.SearchParameters = new QuestionSearch
			{
				EntityId = "something",
				Query = "something",
				QuestionIds = new List<string> { "question1", "question2" }
			};
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Invalid search parameters for generating question search request.")]
		public void ToRequest_SearchParameterIsNull_ExceptionThrown()
		{
			//Assign
			getQuestions.SearchParameters = null;
			//Act
			getQuestions.ToRequest();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Invalid search parameters for generating question search request.")]
		public void ToRequest_EntityIdIsNull_ExceptionThrown()
		{
			//Assign
			TestInitialize();
			getQuestions.SearchParameters.EntityId = null;
			//Assign
			getQuestions.SearchParameters = null;
			//Act
			getQuestions.ToRequest();
		}

		[TestMethod]
		public void ToRequest_CheckCommand_ItIsGetQuestionList()
		{
			//Assign
			TestInitialize();
			//Act
			var dlapRequest = getQuestions.ToRequest();
			//Asssert
			Assert.AreEqual("GetQuestionList", dlapRequest.Parameters["cmd"]);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Invalid search parameters for generating question search request.")]
		public void ToRequestAsync_SearchParameterIsNull_ExceptionThrown()
		{
			//Assign
			getQuestions.SearchParameters = null;
			//Act
			getQuestions.ToRequestAsync();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Invalid search parameters for generating question search request.")]
		public void ToRequestAsync_EntityIdIsNull_ExceptionThrown()
		{
			//Assign
			TestInitialize();
			getQuestions.SearchParameters.EntityId = null;
			//Act
			getQuestions.ToRequestAsync();
		}

		[TestMethod]
		public void ToRequestAsync_SearchParameterHasQuery_RequestHasQuery()
		{
			//Assign
			getQuestions.SearchParameters.Query = "testQuery";
			//Act
			var dlapRequest = getQuestions.ToRequestAsync().FirstOrDefault();
			//Assert
			Assert.AreEqual("testQuery", dlapRequest.Parameters["query"]);
		}

		[TestMethod]
		public void ToRequestAsync_SearchParameterHasQuestionIds_RequestHasQuestionIds()
		{
			//Assign
			getQuestions.SearchParameters.Query = null;
			//Act
			var dlapRequest = getQuestions.ToRequestAsync().FirstOrDefault();
			var testQuestionIds = string.Format("{0}|{1}", "question1", "question2");
			//Assert
			Assert.AreEqual(testQuestionIds, dlapRequest.Parameters["questionid"]);
		}

		[TestMethod]
		public void ToRequestAsync_NoQueryQuestionId_RequestIsNull()
		{
			//Assign
			getQuestions.SearchParameters.Query = null;
			getQuestions.SearchParameters.QuestionIds = null;
			//Act
			var dlapRequest = getQuestions.ToRequestAsync();
			//Assert
			Assert.IsNull(dlapRequest);
		}

		[TestMethod]
		[ExpectedException(typeof(DlapAuthenticationException), "GetQuestions command failed with code AccessDenied")]
		public void ParseResponse_AccessDenied_ExceptionThrown()
		{
			//Assign
			var dlapResponse = new DlapResponse { Code = DlapResponseCode.AccessDenied };
			//Act
			getQuestions.ParseResponse(dlapResponse);
		}

		[TestMethod]
		[ExpectedException(typeof(DlapException))]
		public void ParseResponse_ResponseNotOK_ExceptionThrown()
		{
			//Assign
			var dlapResponse = new DlapResponse { Code = DlapResponseCode.BadRequest };
			//Act
			getQuestions.ParseResponse(dlapResponse);
		}

		[TestMethod]
		public void ParseResponse_ShouldHaveQuestions_HasQuestions()
		{
			//Assign
			var questionsXml = Helper.GetContent(Entity.Questions);
			var dlapResonse = new DlapResponse { ResponseXml = XDocument.Parse(questionsXml) };
			//Act
			getQuestions.ParseResponse(dlapResonse);
			var questionsCount = getQuestions.Questions.Count();
			//Assert
			Assert.AreEqual(2, questionsCount);
		}

		[TestMethod]
		public void ParseResponseAsync_ShouldHaveQuestions_HasQuestions()
		{
			//Assign
			var questionsXml = Helper.GetContent(Entity.Questions);
			var dlapResonse = new DlapResponse { ResponseXml = XDocument.Parse(questionsXml) };

			//Act
			getQuestions.ParseResponseAsync(new List<DlapResponse> { dlapResonse });
			var questionsCount = getQuestions.Questions.Count();
			//Assert
			Assert.AreEqual(2, questionsCount);
		}

        [TestMethod]        
        public void ParseEntity_Question_Without_QuestionVersion_Attribute_Parse_Ok()
        {
            //Assign
            var questionsXml = Helper.GetContent(Entity.QuestionWithoutQuestionVersion);
            var responseXml = XDocument.Parse(questionsXml);
            var questionElements = responseXml.Element("responses").Element("response").Elements("question");

            //Act
            Question question = new Question();
            question.ParseEntity(questionElements.First());

            //Assert
            Assert.IsNotNull(question);
            Assert.AreEqual("12345", question.EntityId);
        }

        [TestMethod]
        public void ParseEntity_Question_Without_ResourceEntityId_And_With_EntityId_Parse_Ok()
        {
            //Assign
            var questionsXml = Helper.GetContent(Entity.QuestionWithoutResourceEntityId);
            var responseXml = XDocument.Parse(questionsXml);
            var questionElements = responseXml.Element("responses").Element("response").Elements("question");
            
            //Act
            Question q = new Question();
            q.ParseEntity(questionElements.First());
            
            //Assert
            Assert.AreEqual("12345", q.EntityId);
        }
		
	}
}
