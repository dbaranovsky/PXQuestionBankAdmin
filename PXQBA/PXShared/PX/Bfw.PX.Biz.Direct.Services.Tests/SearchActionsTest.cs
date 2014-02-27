using System.Linq;
using System.Xml.Linq;
using Bfw.PX.Biz.Direct.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.PX.Biz.DataContracts;
using System.Collections.Generic;
using NSubstitute;

using TestHelper;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    
    
    /// <summary>
    ///This is a test class for SearchActionsTest and is intended
    ///to contain all SearchActionsTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SearchActionsTest
    {


        private TestContext testContextInstance;
        private IBusinessContext _ctx;
        private ISessionManager _sessionManager;
        private IContentActions _contentActions;
        private IQuestionActions _questionActions;
        private SearchActions controller;


        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
       
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestInitialize()]
        public void SearchActionsTestInitialize()
        {

            _ctx = Substitute.For<IBusinessContext>();
            _sessionManager = Substitute.For<ISessionManager>();
            _contentActions = Substitute.For<IContentActions>();
            _questionActions = Substitute.For<IQuestionActions>();

            _ctx.CurrentUser = new UserInfo()
            {
                Id = "user1"
            };


            System.Configuration.ConfigurationManager.AppSettings["FaceplateSearchAgainstProductCourse"] = "true";

            controller = new SearchActions(_ctx, _sessionManager, _contentActions, _questionActions);

        }
        /// <summary>
        ///A test for DoQuestionSearch
        ///</summary>
        [TestMethod()]
        public void DoQuestionSearchTest()
        {
            SearchQuery query = new SearchQuery()
            {
                IncludeWords = "search query",
                EntityId = "6712",
                Start = 0,
                Rows = 10
            };
            string entityId = "6712";
            int numfound = 0;
            int numfoundExpected = 2265;
            IEnumerable<Question> expected = new List<Question>()
            {
                new Question()
                {
                    Id = "F7F3EFAFE235257B5E50C44354160036",
                    EntityId = "6712",
                    InteractionType = InteractionType.Choice,
                    Points = 1.0,
                    Body = "Convergent thinking is to a single correct answer as _________________ is to multiple answers.",
                    Title = "Convergent thinking is to a single correct answer as _________________ is to multiple answers.",
                    Choices = new List<QuestionChoice>()
                    {
                        new QuestionChoice(){Text = "social thinking", Id="unknown"},
                        new QuestionChoice(){Text = "divergent thinking", Id="unknown"},
                        new QuestionChoice(){Text = "emergent thinking", Id="unknown"},
                        new QuestionChoice(){Text = "practical thinking", Id="unknown"}
                    }
                },
                 new Question()
                {
                    Id = "6404822A0A8612473FDDCF85F9D405E4",
                    EntityId = "6712",
                    InteractionType = InteractionType.Choice,
                    Points = 1.2,
                    Body = "Thinking on the margin means that the individual thinks about the additional costs or benefits of doing something.",
                    Choices = new List<QuestionChoice>()
                    {
                        new QuestionChoice(){Text = "True", Id="unknown"},
                        new QuestionChoice(){Text = "False", Id="unknown"},
                    },
                    Title = "376. (TB) Thinking on the margin means that the individual thinks about the additional costs or benefits of do...",
                    SearchableMetaData = new Dictionary<string, string>()
                    {
                        {"title", "376. (TB) Thinking on the margin means that the individual thinks about the additional costs or benefits of do..."},
                        {"exercisenumber_dlap_l", "376"},
                        {"exercisenumber_dlap_d", "376.0"},
                        {"exercisenumber", "376"},

                    }
                }
            };


            XDocument searchResults = TestHelper.Helper.GetXDocument("QuestionSearchResults");

            _sessionManager.CurrentSession.When(s => s.ExecuteAsAdmin(
                Arg.Is<Agilix.Commands.Search>(cmd => cmd.SearchParameters.Query.Contains("search query"))))
                .Do(c => c.Arg<Agilix.Commands.Search>().SearchResults = searchResults);

            IEnumerable<Question> actual = controller.DoQuestionSearch(query, entityId, out numfound);

            Assert.AreEqual(numfoundExpected, numfound);
            
            for(int i=0; i<actual.Count(); i++)
            {
                var actualQuestion = actual.ToArray()[i];
                var expectedQuestion = expected.ToArray()[i];
                Assert.IsTrue(ObjectComparer.AreObjectsEqual(actualQuestion, expectedQuestion));    
            }
            
        }
    }
}
