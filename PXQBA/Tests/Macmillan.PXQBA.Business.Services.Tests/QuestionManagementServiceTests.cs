using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class QuestionManagementServiceTests
    {
        private IQuestionManagementService questionManagementService;

        private IQuestionCommands questionCommands;
        private ITemporaryQuestionOperation temporaryQuestionOperation;
        private IProductCourseManagementService productCourseManagementService;
        private IKeywordOperation keywordOperation;

        [TestInitialize]
        public void TestInitialize()
        {
            questionCommands = Substitute.For<IQuestionCommands>();
            temporaryQuestionOperation = Substitute.For<ITemporaryQuestionOperation>();
            productCourseManagementService = Substitute.For<IProductCourseManagementService>();
            keywordOperation = Substitute.For<IKeywordOperation>();

            questionManagementService = new QuestionManagementService(questionCommands, temporaryQuestionOperation, productCourseManagementService, keywordOperation);
        }

 


        [TestMethod]
        public void PublishToTitle_AnyParameters_TransferNotSuccessfulFromQuestionCommands()
        {
          Course course = new Course();
          string[] questionIds = new[] {"1", "2"};
          var result = questionManagementService.PublishToTitle(questionIds, 1, "bank 1", "chapter 1", course);

          Assert.IsFalse(result);
        }

        [TestMethod]
        public void PublishToTitle_AnyParameters_TransferSuccessfulSuccessfulFromQuestionCommands()
        {
            questionCommands.UpdateQuestions(Arg.Any<IEnumerable<Question>>(), Arg.Any<string>()).Returns(true);

            Course course = new Course();
            string[] questionIds = new[] { "1", "2" };
            var result = questionManagementService.PublishToTitle(questionIds, 1, "bank 1", "chapter 1", course);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void PublishToTitle_AnyParameters_NewProductCourseSectionAdded()
        {
            Course course = new Course()
                            {
                                ProductCourseId = "123",
                                FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                            };

            string[] questionIds = new[] { "1", "2" };
            int newProductCourseId = 1100;
            List<Question> questions = new List<Question>
                                       {
                                           new Question()
                                           {
                                               ProductCourseSections = new List<QuestionMetadataSection>()
                                                                       {
                                                                           new QuestionMetadataSection()
                                                                           {
                                                                               ProductCourseId = course.ProductCourseId
                                                                           }
                                                                       }
                                           },
                                           new Question()
                                           {
                                               ProductCourseSections = new List<QuestionMetadataSection>()
                                                                       {
                                                                           new QuestionMetadataSection()
                                                                           {
                                                                               ProductCourseId = course.ProductCourseId
                                                                           }
                                                                       }
                                           }
                                       };

            questionCommands.UpdateQuestions(Arg.Any<IEnumerable<Question>>(), Arg.Any<string>()).Returns(true);
            questionCommands.GetQuestions(Arg.Any<string>(), Arg.Any<string[]>()).Returns(questions);

            var result = questionManagementService.PublishToTitle(questionIds, newProductCourseId, "bank 1", "chapter 1", course);

            Assert.IsTrue(result);
            Assert.IsTrue(questions[0].ProductCourseSections.Count(q => q.ProductCourseId == newProductCourseId.ToString()) == 1);
            Assert.IsTrue(questions[1].ProductCourseSections.Count(q => q.ProductCourseId == newProductCourseId.ToString()) == 1);
        }
    }
}
