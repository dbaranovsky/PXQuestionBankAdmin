using System;
using System.Configuration;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using NSubstitute;
using BizDc = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    /// <summary>
    ///This is a test class for QuizMapperTest and is intended
    ///to contain all QuizMapperTest Unit Tests
    ///</summary>
    [TestClass]
    public class QuizMapperTest
    {
        private IBusinessContext _context;
        private IContentActions _contActions;
        private IQuestionActions _questionActions;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            
            _contActions = Substitute.For<IContentActions>();
            _questionActions = Substitute.For<IQuestionActions>();

            _context = Substitute.For<IBusinessContext>();
            
        }

        [TestCategory("QuizMapper"), TestMethod]
        public void QuizMapperTest_ToBaseQuiz_ShouldNotShowGrade()
        {
            BizDc.ContentItem biz = new BizDc.ContentItem
            {
                Type = "Assessment", AssessmentSettings = new BizDc.AssessmentSettings { DueDate = DateTime.Now.AddDays(1) },
                AssignmentSettings = new AssignmentSettings()
            };
            _context.Course = new BizDc.Course();
            biz.Properties.Add("bfw_IncludeGbbScoreTrigger", new PropertyValue{Value = "2"});
            _contActions.Context.ReturnsForAnyArgs(_context);
            Quiz quiz = new Quiz();
            quiz.ToBaseQuiz(biz, _contActions, _questionActions, false);
            Assert.IsFalse(quiz.ShowGrade);
        }

        [TestCategory("QuizMapper"), TestMethod]
        public void QuizMapperTest_ToBaseQuiz_ShouldShowGradeIfIncludeGbbScoreTriggerIsFalse()
        {
            BizDc.ContentItem biz = new BizDc.ContentItem
            {
                Type = "Assessment",
                AssessmentSettings = new BizDc.AssessmentSettings { DueDate = DateTime.Now.AddDays(1) },
                AssignmentSettings = new AssignmentSettings()
            };
            _context.Course = new BizDc.Course();
            biz.Properties.Add("bfw_IncludeGbbScoreTrigger", new PropertyValue { Value = "1" });
            _contActions.Context.ReturnsForAnyArgs(_context);
            Quiz quiz = new Quiz();
            quiz.ToBaseQuiz(biz, _contActions, _questionActions, false);
            Assert.IsTrue(quiz.ShowGrade);
        }

        [TestCategory("QuizMapper"), TestMethod]
        public void QuizMapperTest_ToBaseQuiz_ShouldShowGradeIfPassDueDate()
        {
            BizDc.ContentItem biz = new BizDc.ContentItem
            {
                Type = "Assessment",
                AssessmentSettings = new BizDc.AssessmentSettings { DueDate = DateTime.Now.AddDays(-1) },
                AssignmentSettings = new AssignmentSettings()
            };
            _context.Course = new BizDc.Course();
            biz.Properties.Add("bfw_IncludeGbbScoreTrigger", new PropertyValue { Value = "2" });
            _contActions.Context.ReturnsForAnyArgs(_context);
            Quiz quiz = new Quiz();
            quiz.ToBaseQuiz(biz, _contActions, _questionActions, false);
            Assert.IsTrue(quiz.ShowGrade);
        }

        /// <summary>
        /// If review settings is set to show question and answer, then we should show review screen
        /// </summary>
        [TestCategory("QuizMapper"), TestMethod]
        public void ToBaseQuiz_IfShowQuestionsAnswers_ShouldShowReviewScreen()
        {
            BizDc.ContentItem biz = new BizDc.ContentItem
            {
                Type = "Assessment",
                AssessmentSettings = new BizDc.AssessmentSettings {ShowQuestionsAnswers = BizDc.ReviewSetting.Each},
                AssignmentSettings = new AssignmentSettings()
            };
            _context.Course = new BizDc.Course();
            biz.Properties.Add("bfw_IncludeGbbScoreTrigger", new PropertyValue { Value = "2" });
            _contActions.Context.ReturnsForAnyArgs(_context);
            Quiz quiz = new Quiz();
            quiz.ToBaseQuiz(biz, _contActions, _questionActions, false);
            Assert.IsTrue(quiz.ShowReviewScreen);
        }

        /// <summary>
        /// If review settings is not set to show question and answer, then we should not show review screen
        /// </summary>
        [TestCategory("QuizMapper"), TestMethod]
        public void ToBaseQuiz_IfNotShowQuestionsAnswers_ShouldNotShowReviewScreen()
        {
            BizDc.ContentItem biz = new BizDc.ContentItem
            {
                Type = "Assessment",
                AssessmentSettings = new BizDc.AssessmentSettings { ShowQuestionsAnswers = BizDc.ReviewSetting.Never },
                AssignmentSettings = new AssignmentSettings()
            };
            _context.Course = new BizDc.Course();
            biz.Properties.Add("bfw_IncludeGbbScoreTrigger", new PropertyValue { Value = "2" });
            _contActions.Context.ReturnsForAnyArgs(_context);
            Quiz quiz = new Quiz();
            quiz.ToBaseQuiz(biz, _contActions, _questionActions, false);
            Assert.IsFalse(quiz.ShowReviewScreen);
        }
    }
}
