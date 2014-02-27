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
	public class GetQuestionAnalysisTest
	{
		private GetQuestionAnalysis getQuestionAnalysis;

		[TestInitialize]
		public void TestInitialize()
		{
			getQuestionAnalysis = new GetQuestionAnalysis { EntityId = "something", ItemId = "something" };
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException), "Invalid parameters for generating question analysis search request.")]
		public void ToRequest_EntityIsNull_ExceptionThrown()
		{
			//Assign
			getQuestionAnalysis.EntityId = null;
			//Act
			getQuestionAnalysis.ToRequest();
		}

		[TestMethod]
		public void ToRequest_CheckCommand_GetQuestionAnalysis()
		{
			//Assign
			TestInitialize();
			//Act
			var dlapRequest = getQuestionAnalysis.ToRequest();
			//Assert
			Assert.AreEqual("GetQuestionAnalysis", dlapRequest.Parameters["cmd"]);
		}

		[TestMethod]
		public void ParseResponse_ShouldHaveQuestionAnalysis_HasQuestionAnalysis()
		{
			//Assign
			var questionAnalysis = Helper.GetContent(Entity.QuestionAnalysis);
			var dlapResponse = new DlapResponse { ResponseXml = XDocument.Parse(questionAnalysis) };
			//Act
			getQuestionAnalysis.ParseResponse(dlapResponse);
			//Assert
			Assert.AreEqual(1, getQuestionAnalysis.QuestionAnalysis.Count());
		}

	}
}
