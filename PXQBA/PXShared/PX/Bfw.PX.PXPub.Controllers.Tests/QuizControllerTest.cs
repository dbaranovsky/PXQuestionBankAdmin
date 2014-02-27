using System.Collections.Generic;
using Aspose.Pdf;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.ContentTypes;
using Bfw.PX.PXPub.Controllers.Contracts;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;
using System.Web.Mvc;
using Bfw.PX.PXPub.Models;

using NSubstitute;
using NSubstitute.Core;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using Course = Bfw.PX.Biz.DataContracts.Course;
using Domain = Bfw.PX.Biz.DataContracts.Domain;
using Question = Bfw.PX.Biz.DataContracts.Question;
using QuizQuestion = Bfw.PX.Biz.DataContracts.QuizQuestion;
using Grade = Bfw.PX.Biz.DataContracts.Grade;
using System.Web;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    /// <summary>
    ///This is a test class for QuizControllerTest and is intended
    ///to contain all QuizControllerTest Unit Tests
    ///</summary>
    [TestClass]
    public class QuizControllerTest
    {
        private TestContext _testContextInstance;
        private QuizController _controller;
        private IBusinessContext _context;
        private IContentActions _contentActions;
        private IQuestionActions _questionActions;
        private INavigationActions _navigationActions;
        private IGradeActions _gradeActions;
        private IContentHelper _helper;
        private AssignmentCenterHelper _assignmentCenterHelper;
        private IPageActions _pageActions;
        private IQuizHelper _quizHelper;
        private IServiceLocator _serviceLocator;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }

        #region Additional test attributes
        [TestInitialize()]
        public void MyTestInitialize()
        {
            
            _contentActions = Substitute.For<IContentActions>();
            _questionActions = Substitute.For<IQuestionActions>();
            _navigationActions = Substitute.For<INavigationActions>();
            _gradeActions = Substitute.For<IGradeActions>();
            _helper = Substitute.For<IContentHelper>();
            _assignmentCenterHelper = null;
            _pageActions = Substitute.For<IPageActions>();
            _quizHelper = Substitute.For<IQuizHelper>();
            _quizHelper.DefaultHtmlQuizPoints.Returns(1);

            _context = Substitute.For<IBusinessContext>();
            _contentActions.Context.ReturnsForAnyArgs(_context);
            _contentActions.GradableParentId.Returns("Parent");

            _serviceLocator = Substitute.For<IServiceLocator>();
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);

            var context = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            request.UrlReferrer.Returns(new Uri("http://www.google.com"));
            context.Request.Returns(request);

            _controller = new QuizController(_context, _contentActions, _navigationActions, _helper, _quizHelper,
                _assignmentCenterHelper, _pageActions, _questionActions, _gradeActions); // TODO: Initialize to an appropriate value
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = context
            };

        }
        #endregion

        /// <summary>
        ///A test for EditQuestion
        ///</summary>
        [TestCategory("Quiz"), TestMethod]
        public void EditQuestionTest_editQuizQuestionFromDisciplineCourse()
        {

            string questionId = "questionId1";
            string quizId = "quizId1";
            bool isLast = false;
            string disciplineCourseId = "disciplineCourseId";
            string currentCourseId = "currentCourseId";

            var mockQuiz = InitializeContentItem();
            var questions = new List<Question>()
            {
                new Question()
                {
                    Id = questionId,
                    EntityId = currentCourseId,
                    InteractionType = InteractionType.Answer
                }
            };
            _context.EntityId.Returns(currentCourseId);
            _context.Domain.Returns(new Bfw.PX.Biz.DataContracts.Domain()
            {
                CustomQuestionUrls = new Dictionary<string, string>()
            });
            _contentActions.GetContent(currentCourseId, quizId).Returns(mockQuiz);
            _questionActions.GetQuestions(currentCourseId, mockQuiz, true, currentCourseId, null, null)
                .Returns(new QuestionResultSet()
                {
                    Questions = questions
                });
            _quizHelper.UpdateQuizFromQuizQuestions(null, null, null, null).ReturnsForAnyArgs(questions);

            var actionResult = _controller.EditQuestion(questionId, quizId, isLast);

            //ensure question is removed from cache
            _questionActions.ReceivedWithAnyArgs().RemoveQuestionsFromCache(new List<Question>()
            {
                new Question()
                {
                    Id = "questionId1",
                    EntityId = "currentCourseId"
                }
            });

            var viewResult = actionResult as ViewResult;
            Assert.IsInstanceOfType(viewResult.ViewData.Model, typeof(Models.Question));
            var myModel = viewResult.ViewData.Model as Models.Question;
            Assert.AreEqual(myModel.Id, "questionId1");
            Assert.AreEqual(myModel.ItemId, "quizId1");
            Assert.AreEqual(myModel.EntityId, "currentCourseId");
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionFilter_ReturnsListOfMetadata()
        {
            var course = new Bfw.Agilix.DataContracts.Course();
            course.ParseEntity(TestHelper.Helper.GetResponse(TestHelper.Entity.Course, "GenericCourse").Element("course"));
            course.Id = "1";
            _context.Course = course.ToCourse();

            JsonResult result = _controller.QuestionFilter();

            Assert.AreEqual(6, (result.Data.GetType().GetProperty("FilterMetadata").GetValue(result.Data, null) as List<Bfw.PX.PXPub.Models.QuestionFilterMetadata>).Count);
        }

        /// <summary>
        ///A test for SaveQuestionList
        ///</summary>
        [TestCategory("Quiz"), TestMethod, Ignore]
        public void SaveQuestionListTest()
        {
            //QuizQuestions quizQuestions = null; // TODO: Initialize to an appropriate value
            //var actual = controller.SaveQuestionList(quizQuestions);
         
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_GetSingleQuestion_EmptyResultIsReturned_If_QuestionId_Is_Null()
        {
            var singleQuestion = new SingleQuestion()
            {
                QuizId = "Something",
                QuestionId = null
            };
            var result =_controller.GetSingleQuestion(singleQuestion);
            Assert.IsTrue(result is EmptyResult, "An Empty Result should have been returned");
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_GetSingleQuestion_Verify_If_ViewData_Initializes_Correctly()
        {
            var singleQuestion = InitializeSingleQuestion();
            _contentActions.GetContent(singleQuestion.EntityId, singleQuestion.QuizId).Returns(InitializeContentItem());
            _context.Course = new Course()
            {
                Title = "something",
                Id = "something"
            };
            _questionActions.GetQuestion(Arg.Any<string>(), Arg.Any<string>()).Returns(
                new Question()
                {
                    Id = singleQuestion.QuestionId,
                    EntityId = singleQuestion.EntityId,
                    InteractionType = InteractionType.Answer
                });


          
            var result = _controller.GetSingleQuestion(singleQuestion);
            var viewResult = (ViewResult) result;
            var viewData = viewResult.ViewData;
            Assert.AreEqual( "2",(string)viewData["questionNumber"], "ViewData values are not initialized correctly");
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_CreateQuestion_Verify_If_Question_Text_Initializes_Correctly_For_HTS()
        {
            ConfigureCallForTestingQuestionText();

            var result = _controller.CreateQuestion("something","hts",true);
            var viewResult = (ViewResult)result;
            var model = (Bfw.PX.PXPub.Models.Question)viewResult.Model;
            Assert.AreEqual("Untitled Advanced Question", model.Text, "Title is not initialized correctly for HTS Questions");
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_CreateQuestion_Verify_If_Question_Text_Initializes_Correctly_For_Graphing()
        {
            ConfigureCallForTestingQuestionText();

            var result = _controller.CreateQuestion("something", "graph", true);
            var viewResult = (ViewResult)result;
            var model = (Bfw.PX.PXPub.Models.Question)viewResult.Model;
            Assert.AreEqual("Untitled Graphing exercise", model.Text, "Title is not initialized correctly For Graphing Questions");
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_MakeQuizGradableIfSubmitted_WithItemWithoutSubmission_ReturnsFalse()
        {
            var itemId = "itemId";
            _gradeActions.GetGradeList(_context.EntityId, itemId).Returns(new GradeList());

            var result = _controller.MakeQuizGradableIfSubmitted(itemId);
            Assert.IsFalse((bool)result.Data);
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_MakeQuizGradableIfSubmitted_WithItemWithSubmission_ReturnsTrue()
        {
            var itemId = "itemId";
            _gradeActions.GetGradeList(_context.EntityId, itemId).Returns(new GradeList() { Status = "Status" });
            _quizHelper.MakeQuizGradable(itemId, _contentActions, _context).Returns( true );

            var result = _controller.MakeQuizGradableIfSubmitted(itemId);
            Assert.IsTrue((bool)result.Data);
        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_MakeQuizGradable_IfOriginalItemDueDateIsAlreadyMin_DoNothing()
        {
            Quiz quiz = new Quiz { Id = "testQuiz", QuizType  = QuizType.Assessment};
            _context.ImpersonateStudent.Returns(true);
            _context.CurrentUser = new UserInfo {Id = "instructorUserId"};
            _context.Course = new Course {};

            _helper.LoadContentView(quiz.Id, "entityId", ContentViewMode.Preview, "syllabusfilter").Returns(new ContentView {Content = quiz});
            _controller.ShowQuiz(quiz, "entityId", 0);
            _contentActions.DidNotReceiveWithAnyArgs().StoreContent(null);

        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_MakeQuizGradable_IfInstructorItemDueDateIsAlreadyMin_DoNothing()
        {
            Quiz quiz = new Quiz { Id = "testQuiz", QuizType = QuizType.Assessment };
            _context.ImpersonateStudent.Returns(true);
            _context.CurrentUser = new UserInfo { Id = "instructorUserId" };
            _context.Course = new Course { };
            _context.EnrollmentId = "instructorEnrollmentIdForStudentView";
            _contentActions.GetContent("instructorEnrollmentIdForStudentView", "testQuiz").Returns(new ContentItem {AssignmentSettings = new AssignmentSettings{DueDate=DateTime.MinValue} });

            _helper.LoadContentView(quiz.Id, "entityId", ContentViewMode.Preview, "syllabusfilter").Returns(new ContentView { Content = quiz });
            _controller.ShowQuiz(quiz, "entityId", 0);
            _contentActions.DidNotReceiveWithAnyArgs().StoreContent(null);

        }

        [TestCategory("Quiz"), TestMethod]
        public void QuestionController_MakeQuizGradable_IfAssignmentSettingsIsNull_SetDueDateToMin()
        {
            Quiz quiz = new Quiz { Id = "testQuiz", QuizType = QuizType.Assessment, DueDate = DateTime.Now };
            _context.ImpersonateStudent.Returns(true);
            _context.CurrentUser = new UserInfo { Id = "instructorUserId" };
            _context.Course = new Course { };
            _context.EnrollmentId = "instructorEnrollmentIdForStudentView";
            _helper.LoadContentView(quiz.Id, "entityId", ContentViewMode.Preview, "syllabusfilter").Returns(new ContentView { Content = quiz });
            var expectedContentItem = new ContentItem {};
            _contentActions.GetContent("instructorEnrollmentIdForStudentView", "testQuiz").Returns(expectedContentItem);

            _controller.ShowQuiz(quiz, "entityId", 0);
            Assert.IsTrue(expectedContentItem.AssignmentSettings.DueDate == DateTime.MinValue);

        }

        private void ConfigureCallForTestingQuestionText()
        {
            _contentActions.GetContent(Arg.Any<string>(), Arg.Any<string>()).Returns(InitializeContentItem());
            _context.Domain.Returns(new Bfw.PX.Biz.DataContracts.Domain()
            {
                CustomQuestionUrls = new Dictionary<string, string>()
            });
            _context.CurrentUser = new UserInfo
            {
                Id = "someone"
            };

            _questionActions.When(f => f.StoreQuestion(Arg.Any<Biz.DataContracts.Question>())).Do(
                        x =>
                        {
                            //do nothing.
                        }
                );
            _quizHelper.When(f => f.UpdateQuizFromQuizItem(Arg.Any<Quiz>(), _contentActions, _questionActions, _context)).Do(
                      x =>
                      {
                          //do nothing.
                      }
              );
        }

        private SingleQuestion InitializeSingleQuestion()
        {
            var singleQuestion = new SingleQuestion()
            {
                QuizId = "Something",
                QuestionId = "something",
                QuestionNumber = "2",
                AllowDrag = false,
                AllowSelection = false,
                EntityId = "something",
                ExtraClass = "something",
                IsOdd = false,
                IsPoolQuestion = false,
                IsPrimary = false,
                ShowExpand = false,
                ShowAddLink = false,
                ShowPoints = false,
                ShowReused = false,
                IsQuestionOverview = false,
                IsReused = false,
                MainQuizId = string.Empty,
                Mode = QuizBrowserMode.QuestionPicker,
                QuestionEditedType = "something"
            };
            return singleQuestion;
        }

        private ContentItem InitializeContentItem()
        {

            return new ContentItem()
            {
                Id = "quizId1",
                CourseId = "currentCourseId",
                ActualEntityid = "currentCourseId",
                Subtype = "quiz",
                Type = "Assessment",
                QuizQuestions = new List<QuizQuestion>()
                {
                    new QuizQuestion()
                    {
                        EntityId = "disciplineCourseId",
                        QuestionId = "questionId1",
                        Type = "1",
                        Score = "2"
                    }
                }
            };
        }
    }
}
