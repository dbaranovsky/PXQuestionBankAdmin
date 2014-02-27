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
	public class GetQuestionsAdminTest
	{
		private GetQuestionsAdmin getQuestionsAdmin;

		[TestInitialize]
		public void TestInitialize()
		{
			getQuestionsAdmin = new GetQuestionsAdmin();
			getQuestionsAdmin.SearchParameters = new QuestionAdminSearch {
				EntityId = "something",
				QuestionIds = new List<string> { "QuestionId1", "QuestionId2" },
				QuestionsFilter = @"query=/interaction@type='choice'",
			};
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Invalid search parameters for generating GetQuestionsAdmin search request.")]
		public void ToRequest_SearchParametersIsNul_ErrorThrown()
		{
			//Assign
			getQuestionsAdmin.SearchParameters = null;
			//Act
			getQuestionsAdmin.ToRequest();
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Invalid search parameters for generating GetQuestionsAdmin search request.")]
		public void ToRequest_EntityIdIsNull_ExceptionThrown()
		{
			//Assign
			TestInitialize();
			getQuestionsAdmin.SearchParameters.EntityId = null;
			//Act
			getQuestionsAdmin.ToRequest();
		}

		[TestMethod]
		public void ToRequest_CheckTheCommand_GetQuestionList()
		{
			//Assign
			TestInitialize();
			//Act
			var dlapRequest = getQuestionsAdmin.ToRequest();
			//Assert
			Assert.AreEqual("GetQuestionList", dlapRequest.Parameters["cmd"]);
		}

		[TestMethod]
		public void ToRequest_ShouldHaveQuestionIds_HasQuestionIds()
		{
			//Assign
			var testQuestionIds = getQuestionsAdmin.SearchParameters.QuestionIds.Fold("|");
			//Act
			var dlapRequest = getQuestionsAdmin.ToRequest();
			//Assert
			Assert.AreEqual(testQuestionIds, dlapRequest.Parameters["questionid"]);
		}

		[TestMethod]
		public void ToRequest_ShouldHaveQuestionsFilter_HasQuestionsFilter()
		{
			//Assign
			var testQuestionFilter = getQuestionsAdmin.SearchParameters.QuestionsFilter;
			//Act
			var dlapRequest = getQuestionsAdmin.ToRequest();
			//Assert
			Assert.AreEqual(testQuestionFilter, dlapRequest.Parameters["query"]);
		}

		[TestMethod]
		public void ToRequest_ShouldHaveCount_HasCount()
		{
			//Assign
			getQuestionsAdmin.SearchParameters.Count = 5;
			//Act
			var dlapRequest = getQuestionsAdmin.ToRequest();
			//Assign
			Assert.AreEqual(getQuestionsAdmin.SearchParameters.Count, dlapRequest.Parameters["count"]);
		}

		[TestMethod]
		public void ToRequest_ShouldHaveVersion_HasVersion()
		{
			//Assign
			getQuestionsAdmin.SearchParameters.version = true;
			//Act
			var dlapRequest = getQuestionsAdmin.ToRequest();
			//Assign
			Assert.AreEqual(getQuestionsAdmin.SearchParameters.version, dlapRequest.Parameters["allversions"]);
		}

		[TestMethod]
		[ExpectedException(typeof(DlapAuthenticationException), "GetQuestions command failed with code AccessDenied")]
		public void ParseResponse_AccessDenied_ExceptionThrown()
		{
			//Assign
			var dlapResponse = new DlapResponse { Code = DlapResponseCode.AccessDenied };
			//Act
			getQuestionsAdmin.ParseResponse(dlapResponse);
		}

		[TestMethod]
		[ExpectedException(typeof(DlapException))]
		public void ParseResponse_ResponseNotOK_ExceptionThrown()
		{
			//Assign
			var dlapResponse = new DlapResponse { Code = DlapResponseCode.BadRequest };
			//Act
			getQuestionsAdmin.ParseResponse(dlapResponse);
		}

		[TestMethod]
		public void ParseResponse_ShouldHaveQuestions_HasQuestions()
		{
			//Assign
			var questionsXml = Helper.GetContent(Entity.Questions);
			var dlapResonse = new DlapResponse { ResponseXml = XDocument.Parse(questionsXml) };
			//Act
			getQuestionsAdmin.ParseResponse(dlapResonse);
			var questionsCount = getQuestionsAdmin.Questions.Count();
			//Assert
			Assert.AreEqual(2, questionsCount);
		}


	}
}
