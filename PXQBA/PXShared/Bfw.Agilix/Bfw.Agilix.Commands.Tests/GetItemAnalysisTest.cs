using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Collections.Generic;
using Bfw.Common.Collections;


namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetItemAnalysisTest
    {
        private GetItemAnalysis itemAnalysis;

        [TestInitialize]
        public void TestInitialize()
        {
            this.itemAnalysis = new GetItemAnalysis();
            this.itemAnalysis.SearchParameters = new ItemAnalysisSearch()
            {
                ItemId = "something",
                EntityId = "something"
            };
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Invalid parameters for generating item analysis search request.")]
        public void GetItemAnalysisTest_Request_Should_Throw_Exception_If_EntityId_Is_Null()
        {
            itemAnalysis.SearchParameters.EntityId = null;
            itemAnalysis.ToRequest();
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Invalid parameters for generating item analysis search request.")]
        public void GetItemAnalysisTest_Request_Should_Throw_Exception_If_ItemId_Is_Null()
        {
            itemAnalysis.SearchParameters.ItemId = null;
            itemAnalysis.ToRequest();
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        public void GetItemAnalysisTest_Request_DlapRequest_Should_Be_GetRequest()
        {
            var request = itemAnalysis.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        public void GetItemAnalysisTest_Request_DlapRequest_GroupId_Should_Be_Initialized()
        {
            itemAnalysis.SearchParameters.GroupId = "test";
            var request = itemAnalysis.ToRequest();
            Assert.AreEqual(request.Parameters["groupid"], itemAnalysis.SearchParameters.GroupId);
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        public void GetItemAnalysisTest_Request_DlapRequest_EnrollmentId_Should_Be_Initialized()
        {
            itemAnalysis.SearchParameters.EnrollmentId = "test";
            var request = itemAnalysis.ToRequest();
            Assert.AreEqual(request.Parameters["enrollmentid"], itemAnalysis.SearchParameters.EnrollmentId);
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        public void GetItemAnalysisTest_Request_DlapRequest_Summary_Should_Be_Initialized()
        {
            itemAnalysis.SearchParameters.Summary = "test";
            var request = itemAnalysis.ToRequest();
            Assert.AreEqual(request.Parameters["summary"], itemAnalysis.SearchParameters.Summary);
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        public void GetItemAnalysisTest_Request_DlapRequest_SetId_Should_Be_Initialized()
        {
            itemAnalysis.SearchParameters.SetId = "test";
            var request = itemAnalysis.ToRequest();
            Assert.AreEqual(request.Parameters["setid"], itemAnalysis.SearchParameters.SetId);
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        public void GetItemAnalysisTest_Response_ItemAnalysis_Should_Be_Initialized()
        {
            var dlapResponse = new DlapResponse();
            itemAnalysis.ParseResponse(dlapResponse);
            Assert.IsNotNull(itemAnalysis.ItemAnalysis);
        }

        [TestCategory("Eportfolio"), TestCategory("EportfolioRubricsReport"), TestMethod]
        public void GetItemAnalysisTest_Response_ItemAnalysis_List_Count_Should_Be_Zero_If_Response_XML_Is_Null()
        {
            var dlapResponse = new DlapResponse();
            itemAnalysis.ParseResponse(dlapResponse);
            Assert.AreEqual( ((List<ItemAnalysisDetail>)itemAnalysis.ItemAnalysis).Count,0);
        }
    }
}
