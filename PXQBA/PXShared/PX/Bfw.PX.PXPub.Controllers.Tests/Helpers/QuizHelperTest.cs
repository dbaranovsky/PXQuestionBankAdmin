using System.Collections.Generic;
using System.Linq;

using Bfw.PX.Biz.ServiceContracts;
using DC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Models;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

using Question = Bfw.PX.Biz.DataContracts.Question;

namespace Bfw.PX.PXPub.Controllers.Tests.Helpers
{
    
    /// <summary>
    ///This is a test class for QuizHelperTest and is intended
    ///to contain all QuizHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class QuizHelperTest
    {
        private string entityid = "currentCourseId";
        private TestContext testContextInstance;
        private IContentActions _contentActions;
        private IQuestionActions _questionActions;
        private IBusinessContext _context;
        private IQuizHelper _helper;

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

        [TestInitialize]
        public void MyTestInitialize()
        {
            _contentActions = Substitute.For<IContentActions>();
            _questionActions = Substitute.For<IQuestionActions>();
            _context = Substitute.For<IBusinessContext>();
            _context.EntityId.Returns("currentCourseId");
            _helper = new QuizHelper();
        }

        /// <summary>
        /// A test for UpdateQuizFromQuizQuestions
        /// Test whether the function moves questions from the current course to the discipline course
        ///</summary>
        [TestCategory("Quiz"), TestMethod]
        public void QuizHelperTest_UpdateQuizFromQuizQuestionsTest_moveQuestionsFromDisciplineCourse()
        {
            var target = new QuizHelper();
            var mockQuestion = new QuizQuestion()
            {
                EntityId = "discplineCourseId",
                IsBank = false,
                IsEmpty = false,
                IsNew = false,
                QuestionId = "questionId1",
                QuizId = "quizId1",
                MainQuizId = "quizId1",
                UseCount = 0
            };
            var quizQuestions = new QuizQuestions()
            {
                MainQuizId = "quizId1",
                QuestionIds = string.Empty,
                Questions = new List<QuizQuestion>()
                {
                    mockQuestion
                }
            };
            var retrievedQuestions = new List<Biz.DataContracts.Question>()
            {
                new Biz.DataContracts.Question()
                {
                    Id = "questionId1",
                    EntityId = "discplineCourseId"
                }
            };
           

            var storedQuestions = new List<Biz.DataContracts.Question>()
            {
                new Biz.DataContracts.Question()
                {
                    Id = "questionId1",
                    EntityId = "currentCourseId"
                }
            };
            _questionActions.GetQuestions("discplineCourseId", Arg.Is<IEnumerable<string>>(x => x.Any() && x.FirstOrDefault() == "questionId1"))
               .Returns(ci => retrievedQuestions);
            bool storedQuestionsCalled = false;
            _questionActions.When(c => c.StoreQuestions(Arg.Is<IList<Biz.DataContracts.Question>>(x => x.Any()
                                                                                          && x.FirstOrDefault().EntityId ==
                                                                                          "currentCourseId" && x.FirstOrDefault().Id ==
                                                                                          "questionId1")))
                            .Do(c => storedQuestionsCalled = true);
            _questionActions.GetQuestions("currentCourseId", Arg.Is<IEnumerable<string>>(x => x.Any() && x.FirstOrDefault() == "questionId1"))
                .Returns(callInfo => storedQuestionsCalled ? storedQuestions: new List<Question>());

            var actual = target.UpdateQuizFromQuizQuestions(quizQuestions, _contentActions, _questionActions, _context);

            Assert.IsTrue(storedQuestionsCalled);

            Assert.IsTrue(actual.Count > 0);
            var actualQuestion = actual.FirstOrDefault();
            Assert.AreEqual(actualQuestion.Id, "questionId1");
            Assert.AreEqual(actualQuestion.EntityId, "currentCourseId");
        }

        /// <summary>
        /// When pass empty question list to UpdateQuizFromQuizQuestions(), 
        /// it should call ContentActions.updateQuestionList() and return an empty list
        ///</summary>
        [TestCategory("Quiz"), TestMethod]
        public void QuizHelperTest_UpdateQuizFromQuizQuestions_PassEmptyQuestionList_ExpectCalledUpdateQuizFromQuizQuestions_ExpectReturnEmptyList()
        {
            var helper = new QuizHelper();
            var quizQuestions = new QuizQuestions()
            {
                MainQuizId = "quizId1",
                QuestionIds = string.Empty,
                Questions = null
            };
            var updateQuestionListCalled = false;
            _questionActions.When(
                f => f.UpdateQuestionList("currentCourseId", null, Arg.Any<IList<Question>>(), true, "quizId1"))
                .Do(x => updateQuestionListCalled = true);

            var result = helper.UpdateQuizFromQuizQuestions(quizQuestions, _contentActions, _questionActions, _context);

            //Should not Return any question
            Assert.IsFalse(result.Any());
            //Should call ContentActions.updateQuestionList()
            Assert.IsTrue(updateQuestionListCalled);
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuizHelperTest_MakeQuizGradable_WithBadItemId_ReturnsFalseJson()
        {
            _contentActions.GetContent(Arg.Any<string>(), Arg.Any<string>()).Returns(x => null);
            var result = _helper.MakeQuizGradable("itemId", _contentActions, _context);
            Assert.IsFalse(result);
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuizHelperTest_MakeQuizGradable_WithItemId_SetsGradableAndWeight_ReturnsTrueJson()
        {
            var defaultPts = 20;
            var entityid = "entityid";
            var itemid = "itemid";
            var item = new DC.ContentItem()
            {
                AssignmentSettings = new DC.AssignmentSettings(),
                DefaultPoints = defaultPts
            };

            _context.EntityId.Returns(entityid);
            _contentActions.GetContent(entityid, itemid).Returns(item);
            var result = _helper.MakeQuizGradable(itemid, _contentActions, _context);

            Assert.IsTrue(result);
            Assert.AreEqual(_contentActions.GradableParentId, item.ParentId);
            Assert.IsTrue(item.AssignmentSettings.IsAssignable);
            Assert.AreEqual(defaultPts, item.AssignmentSettings.Points);
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuizHelperTest_MakeQuizGradable_WithNoDefaultPoints_WeightDefaultsToQuizHelperDefault()
        {
            var defaultPts = 0;
            var entityid = "entityid";
            var itemid = "itemid";
            var item = new DC.ContentItem()
            {
                AssignmentSettings = new DC.AssignmentSettings(),
                DefaultPoints = defaultPts
            };

            _context.EntityId.Returns(entityid);
            _contentActions.GetContent(entityid, itemid).Returns(item);
            var result = _helper.MakeQuizGradable(itemid, _contentActions, _context);

            Assert.IsTrue(result);
            Assert.AreEqual(_contentActions.GradableParentId, item.ParentId);
            Assert.IsTrue(item.AssignmentSettings.IsAssignable);
            Assert.AreEqual(_helper.DefaultHtmlQuizPoints, item.AssignmentSettings.Points);
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuizHelperTest_MakeQuizGradable_WithItemId_WithExistingWeight_WeightRemainsTheSame()
        {
            var existingPts = 90;
            var defaultPts = 20;
            var entityid = "entityid";
            var itemid = "itemid";
            var item = new DC.ContentItem()
            {
                AssignmentSettings = new DC.AssignmentSettings()
                {
                    Points = existingPts
                },
                DefaultPoints = defaultPts
            };

            _context.EntityId.Returns(entityid);
            _contentActions.GetContent(entityid, itemid).Returns(item);
            var result = _helper.MakeQuizGradable(itemid, _contentActions, _context);

            Assert.IsTrue(result);
            Assert.AreEqual(_contentActions.GradableParentId, item.ParentId);
            Assert.IsTrue(item.AssignmentSettings.IsAssignable);
            Assert.AreEqual(existingPts, item.AssignmentSettings.Points);
        }

        /// <summary>
        ///A test for UpdateQuizFromQuizItem
        ///</summary>
        [TestMethod, Ignore]
        public void UpdateQuizFromQuizItemTest()
        {
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
