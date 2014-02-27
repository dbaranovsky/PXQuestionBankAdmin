using System.Collections.Generic;
using System.Linq;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using System.Web.Mvc;
using NSubstitute;
using Course = Bfw.PX.Biz.DataContracts.Course;
using Question = Bfw.PX.Biz.DataContracts.Question;
using SearchQuery = Bfw.PX.Biz.DataContracts.SearchQuery;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    
    
    /// <summary>
    ///This is a test class for QuizEditControllerTest and is intended
    ///to contain all QuizEditControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QuizEditControllerTest
    {


        private TestContext testContextInstance;
        private IBusinessContext _context;
        private INavigationActions _navigationActions;
        private IContentActions _contentActions;
        private ISearchActions _searchActions;
        private IQuestionActions _questionActions;
        private QuizEditController quizEditController;

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

        [TestInitialize()]
        public void QuizEditControllerInitialize()
        {
            _context = Substitute.For<IBusinessContext>();
            _navigationActions = Substitute.For<INavigationActions>();
            _contentActions = Substitute.For<IContentActions>();
            _questionActions = Substitute.For<IQuestionActions>();
            _searchActions = Substitute.For<ISearchActions>();
            quizEditController = new QuizEditController(_context, _navigationActions, _contentActions, _searchActions, _questionActions);
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


        /// <summary>
        ///A test for ExpandSection
        ///</summary>
        [TestMethod()]
        public void ExpandSection_Can_Render_SearchResults()
        {
          
            string id = string.Empty; 
            string category = string.Empty; 
            string startIndex = "0"; 
            string lastIndex = "20"; 
            var mode = QuizBrowserMode.Quiz; 
            string mainQuizId = string.Empty;
            string query = "this is my query";

            _context.EntityId.Returns("currentEntityId");
            _context.Course.Returns(new Course()
            {
                Id="currentEntityId"
            });

            _questionActions.GetQuestionRepositoryCourse("currentEntityId").Returns("disciplineCourseId");
            int numFound = 0;
            _searchActions.DoQuestionSearch(Arg.Any<SearchQuery>(), "disciplineCourseId", out numFound).Returns(arg =>
            {
                arg[2] = 1;//set numFound
                return new List<Question>()
                {
                    new Question()
                    {
                        Id = "questionId",
                        EntityId = "disciplineCourseId",
                        Body = "body",
                        InteractionType = InteractionType.Choice
                    }
                };
            });

            var result = quizEditController.ExpandSection(id, category, startIndex, lastIndex, mode, mainQuizId, query);
            var view = result as ViewResult;
            Assert.IsNotNull(view);
            
            var model = view.Model as QuizSearchResults;
            Assert.IsNotNull(model);

            var expectedQuery = new Models.SearchQuery()
            {
                IncludeWords = query,
                Start = 0,
                Rows = 20
            };
            Assert.AreEqual(expectedQuery.IncludeWords, model.Query.IncludeWords);
            Assert.AreEqual(expectedQuery.Start, model.Query.Start);
            Assert.AreEqual(expectedQuery.Rows, model.Query.Rows);

            var expectedQuiz = new Quiz()
            {
                Questions = new List<Models.Question>()
                {
                    new Models.Question()
                    {
                        Id = "questionId",
                        EntityId = "disciplineCourseId",
                        Text = "body",
                        Type = "MC"
                    }
                },
                ShowReused = true,
                QuizPaging = new Paging()
                {
                    StartIndex = "0",
                    LastIndex = "20",
                    TotalCount = 1
                }
            };
            Assert.AreEqual(expectedQuiz.Questions[0].Id, model.Quiz.Questions[0].Id);
            Assert.AreEqual(expectedQuiz.Questions[0].EntityId, model.Quiz.Questions[0].EntityId);
            Assert.AreEqual(expectedQuiz.Questions[0].Text, model.Quiz.Questions[0].Text);
            Assert.AreEqual(expectedQuiz.Questions[0].Type, model.Quiz.Questions[0].Type);

            Assert.AreEqual(expectedQuiz.ShowReused, model.Quiz.ShowReused);

            Assert.AreEqual(expectedQuiz.QuizPaging.StartIndex, model.Quiz.QuizPaging.StartIndex);
            Assert.AreEqual(expectedQuiz.QuizPaging.LastIndex, model.Quiz.QuizPaging.LastIndex);
            Assert.AreEqual(expectedQuiz.QuizPaging.TotalCount, model.Quiz.QuizPaging.TotalCount);



        }

        /// <summary>
        /// If you are in quiz "TestBank", then the questions in testbank should not have "TestBank" appeared in Used Elsewhere
        ///</summary>
        [TestCategory("QuizEditController"),TestMethod(), Ignore]
        public void SameQuizId_ShouldNotAppearInUsedElsewhere()
        {

            //const string quizId = "testBank";
            //string category = string.Empty;
            //const string startIndex = "0";
            //const string lastIndex = "20";
            //const string entityId = "currentEntityId";
            //const QuizBrowserMode mode = QuizBrowserMode.Quiz;
            //string mainQuizId = string.Empty;
            //string query = string.Empty;
            //var question = new Question
            //{
            //    InteractionType = InteractionType.Choice,
            //    SearchableMetaData = new Dictionary<string, string>()
            //};
            //question.SearchableMetaData.Add("usedin", "ab,testBank,b,c");
            //var questionResultSet = new QuestionResultSet
            //{
            //    Questions = new List<Question>
            //    {
            //        question
            //    }
            //};
            //_context.EntityId.Returns(entityId);
            //_context.Course.Returns(new Course()
            //{
            //    Id = entityId
            //});

            //_navigationActions.LoadNavigation(entityId, quizId, category)
            //    .Returns(new Biz.DataContracts.NavigationItem { Type = "quiz", Id = "PX_LOR" });

            //_questionActions.GetQuestions(entityId, "PX_LOR", startIndex, lastIndex).Returns(questionResultSet);
            //var result = quizEditController.ExpandSection(quizId, category, startIndex, lastIndex, mode, mainQuizId, query);
            //var view = result as ViewResult;
            //Assert.IsNotNull(view);

            //var model = view.Model as Quiz;
            //Assert.IsNotNull(model);
            //var firstQuestion = model.Questions.First();
            //Assert.IsNotNull(firstQuestion);
            //Assert.IsTrue(firstQuestion.SearchableMetaData.ContainsKey("usedin"));
            //Assert.IsTrue(firstQuestion.SearchableMetaData["usedin"] == "ab,b,c");

        }


        [TestCategory("QuizEditController"), TestMethod()]
        public void Verify_If_All_Questions_Are_Shown_For_Question_Banks()
        {
            const string quizId = "testBank";
            var category = string.Empty;
            const string startIndex = "0";
            const string lastIndex = "20";
            const string entityId = "currentEntityId";
            const QuizBrowserMode mode = QuizBrowserMode.Quiz;
            string mainQuizId = string.Empty;
            string query = string.Empty;
            var questionResultSet = GetQuestionResultSetWithQuestionBanks();
           
            _context.EntityId.Returns(entityId);
            _context.Course.Returns(new Course()
            {
                Id = entityId
            });

            _navigationActions.LoadNavigation(entityId, quizId, category)
                .Returns(new Biz.DataContracts.NavigationItem { Type = "quiz", Id = "PX_LOR" });

            _questionActions.GetQuestions(entityId, "PX_LOR", startIndex, lastIndex).Returns(questionResultSet);
            var result = quizEditController.ExpandSection(quizId, category, startIndex, lastIndex, mode, mainQuizId, query);

            var view = result as ViewResult;
            Assert.IsNotNull(view);
            var model = view.Model as Quiz;
            Assert.IsNotNull(model);

            Assert.AreEqual(4, model.Questions.Count());
        }

        private QuestionResultSet GetQuestionResultSetWithQuestionBanks()
        {
            var questionBank1 = new Question
            {
                InteractionType = InteractionType.Bank,
                SearchableMetaData = new Dictionary<string, string>(),
                Questions = new List<Question>()
                {
                    new Question(){Id = "1"},
                    new Question(){Id = "2"}
                }
            };
            var questionBank2 = new Question
            {
                InteractionType = InteractionType.Bank,
                SearchableMetaData = new Dictionary<string, string>(),
                Questions = new List<Question>()
                {
                    new Question(){Id = "3"},
                    new Question(){Id = "4"}
                }
            };
            var questionResultSet = new QuestionResultSet
            {
                Questions = new List<Question>
                {
                    questionBank1,
                    questionBank2
                }
            };
            return questionResultSet;
        }


    }
}
