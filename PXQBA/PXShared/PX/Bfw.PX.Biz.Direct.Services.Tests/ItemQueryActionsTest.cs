using System;
using System.Collections.Generic;
using System.Reflection;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.ServiceContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using Bfw.Agilix.DataContracts;
using TestHelper;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass]
    public class ItemQueryActionsTest
    {
        private IBusinessContext context;
        private ISessionManager sessionManager;
        private ItemQueryActions actions;

        [TestInitialize]
        public void TestInitialize()
        { 
            context = Substitute.For<IBusinessContext>();
            sessionManager = Substitute.For<ISessionManager>();

            actions = new ItemQueryActions();
        }

        [TestMethod]
        public void BuildListChildrenQuery_Returns_QueryString_For_SyncCourse()
        {
            var result = actions.BuildListChildrenQuery("entityId", "parentId", 1, System.Configuration.ConfigurationManager.AppSettings["MyMaterials"], "userId", true);

            Assert.AreEqual("/parent='parentId'", result.Query);
        }

        [TestMethod]
        public void BuildListChildrenQuery_Returns_QueryString_For_USE_AGILIX_PARENT()
        {
            var result = actions.BuildListChildrenQuery("entityId", "parentId", 1, Constants.USE_AGILIX_PARENT, "userId", false);

            Assert.IsNull(result.Query);
        }

        [TestMethod]
        public void BuildListChildrenQuery_Returns_QueryString_For_MyMaterials()
        {
            var result = actions.BuildListChildrenQuery("entityId", "parentId", 1, System.Configuration.ConfigurationManager.AppSettings["MyMaterials"], "userId", false);

            Assert.AreEqual("/bfw_tocs/my_materials@parentid='my_materials_userId'", result.Query);
        }

        [TestMethod]
        public void BuildListChildrenQuery_Returns_QueryString_For_MyQuizes()
        {
            var result = actions.BuildListChildrenQuery("entityId", "parentId", 1, System.Configuration.ConfigurationManager.AppSettings["MyQuizes"], "userId", false);

            Assert.AreEqual("/bfw_tocs/syllabusfilter@parentid<>''AND /type='Assessment'  or /type='Homework'", result.Query);
        }

        [TestMethod]
        public void BuildListChildrenQuery_Returns_QueryString_For_CategoryId()
        {
            var result = actions.BuildListChildrenQuery("entityId", "parentId", 1, "categoryId", "userId", false);

            Assert.AreEqual("/bfw_tocs/categoryId@parentid='parentId'", result.Query);
        }

        [TestMethod]
        public void BuildListChildrenQuery_Returns_QueryString_For_Missing_CategoryId()
        {
            var result = actions.BuildListChildrenQuery("entityId", "parentId", 1, null, "userId", false);

            Assert.AreEqual("/bfw_tocs/bfw_toc_contents@parentid='parentId'", result.Query);
        }

        [TestMethod]
        public void BuildItemSearchQuery_Returns_QueryString_For_Search_Parameters()
        {
            var queryParams = new System.Collections.Generic.Dictionary<string, string>();
            queryParams.Add("paramName1", "paramValue1");
            queryParams.Add("freequery", "freeQueryValue");

            var result = actions.BuildItemSearchQuery("entityId", queryParams, "userId", "or");

            Assert.AreEqual("/paramName1='paramValue1' or freeQueryValue", result.Query);
        }
    }
}
