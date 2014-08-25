using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Common.JqGridHelper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Business.Services.Automapper;
using Macmillan.PXQBA.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Macmillan.PXQBA.Business.Models.Question;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class TemporaryQuestionOperationTest
    {
        private IContext context;
        private IProductCourseOperation productCourseOperation;
        private IUserOperation userOperation;
        private INoteCommands noteCommands;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;
        private IQuestionCommands questionCommands;
        private ITemporaryQuestionOperation temporaryQuestionOperation;
        private string productCourseId = "12";

        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IContext>();
            context.CurrentUser = new UserInfo {Id = "633478"};

            questionCommands = Substitute.For<IQuestionCommands>();
            questionCommands.SetSequence(Arg.Any<string>(), Arg.Do<Question>(q => q.ProductCourseSections.First().Sequence = "1"));
          

            productCourseOperation = Substitute.For<IProductCourseOperation>();
            temporaryQuestionOperation = new TemporaryQuestionOperation(context, questionCommands);

        
            modelProfileService = new ModelProfileService(productCourseOperation, questionCommands, userOperation, noteCommands);
            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();
        }
        
        [TestMethod]
        public void CreateQuestion_QuestionToCreate_ProperlyCreatedQuestion()
        {
            var question = new Models.Question()
                           {
                               Id = "1",
                               ProductCourseSections = new List<QuestionMetadataSection>()
                                                       {
                                                           new QuestionMetadataSection()
                                                       }
                           };
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(items =>
            {
                items.Items = new List<Item>(){new Item()};
            }));


            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetRawItem>(items =>
                                                                                    {
                                                                                        items.ItemDocument =
                                                                                             XDocument.Parse(tempQuizXml);
                                                                                    }));
            var result = temporaryQuestionOperation.CreateQuestion("12", question);

            Assert.IsTrue(result.QuizId.StartsWith("QBA_temp_quiz"));
            Assert.IsTrue(result.ModifiedBy == context.CurrentUser.Id);
            Assert.IsTrue(result.Id != "1");
            Assert.IsTrue(result.ProductCourseSections.First().Sequence == "1");
            Assert.IsTrue(result.EntityId == ConfigurationHelper.GetTemporaryCourseId());
        }

        [TestMethod]
        public void DeleteTemporaryQuestionWithQuiz_CorrectParams_SuccessRun()
        {
            temporaryQuestionOperation.DeleteTemporaryQuestionWithQuiz("quiestinId");
        }

         [TestMethod]
        public void CopyQuestionToSourceCourse_QuestionToCopy_ProperlyCopiedQuestion()
         {
             var questionAgilix = new Bfw.Agilix.DataContracts.Question()
                                  {
                                      InteractionType = "2",
                                      QuestionXml = @"<img src=""[~]/brainhoney/resources/img.jpg""></img>"
                                  };
             questionCommands.GetAgilixQuestion(null, null).ReturnsForAnyArgs(questionAgilix);
             var result = temporaryQuestionOperation.CopyQuestionToSourceCourse("12", "12d");
             Assert.IsTrue(result.EntityId == "12");
             Assert.IsTrue(result.Id == "12d");
         }

       

         [TestMethod]
        public void CopyQuestionToTemporaryCourse_QuestionToCopy_ProperlyCopiedQuestion()
        {
            var questionAgilix = new Bfw.Agilix.DataContracts.Question()
            {
                InteractionType = "2",
                QuestionXml = @"<img src=""[~]/brainhoney/resources/img.jpg""></img>"
            };
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(items =>
            {
                items.Items = new List<Item>() { new Item() };
            }));


            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetRawItem>(items =>
            {
                items.ItemDocument =
                     XDocument.Parse(tempQuizXml);
            }));
            questionCommands.GetAgilixQuestion(null, null).ReturnsForAnyArgs(questionAgilix);
            var result = temporaryQuestionOperation.CopyQuestionToTemporaryCourse("12", "12d");
            Assert.IsTrue(result.EntityId == ConfigurationHelper.GetTemporaryCourseId());
            Assert.IsTrue(result.Id == string.Format("QBA_temp_question_{0}_12d", context.CurrentUser.Id));
            Assert.IsTrue(result.QuizId == string.Format("QBA_temp_quiz_{0}_12d", context.CurrentUser.Id));

        }


         [TestMethod]
         public void CopyQuestionToTemporaryCourse_QuestionVesrionToCopy_ProperlyCopiedQuestion()
         {
             var questionAgilix = new Bfw.Agilix.DataContracts.Question()
             {
                 InteractionType = "2",
                 QuestionXml = ""
             };
             var counter = 0;
             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(items =>
             {


                 items.Items = counter == 0 ? new List<Item>() : new List<Item> { new Item() };
                 counter++;
             }));

             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetRawItem>(items =>
             {
                 items.ItemDocument =
                      XDocument.Parse(tempQuizXml);
             }));


             questionCommands.GetAgilixQuestion(null, null).ReturnsForAnyArgs(questionAgilix);
             var result = temporaryQuestionOperation.CopyQuestionToTemporaryCourse("12", "12d", "3");
             Assert.IsTrue(result.EntityId == ConfigurationHelper.GetTemporaryCourseId());
             Assert.IsTrue(result.Id == string.Format("QBA_temp_ver_question_{0}_12d", context.CurrentUser.Id));
             Assert.IsTrue(result.QuizId == string.Format("QBA_temp_ver_quiz_{0}_12d", context.CurrentUser.Id));

         }

         [TestMethod]
         public void CopyQuestionToTemporaryCourse_NotExistingQuestionVesrionToCopy_ProperlyCopiedQuestion()
         {
             var questionAgilix = new Bfw.Agilix.DataContracts.Question()
             {
                 InteractionType = "2",
                 QuestionXml = "",
                 Id = "2",
                 EntityId = "12"
             };
             var counter = 0;
             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(items =>
             {


                 items.Items = counter == 0 ? new List<Item>() : new List<Item> { new Item() };
                 counter++;
             }));

             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetRawItem>(items =>
             {
                 items.ItemDocument =
                      XDocument.Parse(tempQuizXml);
             }));


             questionCommands.GetAgilixQuestion(Arg.Any<string>(), Arg.Any<string>(), "3").Returns((Bfw.Agilix.DataContracts.Question)null);
             questionCommands.GetAgilixQuestion(Arg.Any<string>(), Arg.Any<string>()).Returns(questionAgilix);

             var result = temporaryQuestionOperation.CopyQuestionToTemporaryCourse("12", "12d", "3");
             Assert.IsTrue(result.EntityId == "12");
             Assert.IsTrue(result.Id =="2");
             Assert.IsTrue(result.QuizId == string.Format("QBA_temp_ver_quiz_{0}_12d", context.CurrentUser.Id));

         }

         [TestMethod]
         [ExpectedException(typeof(Exception))]
         public void CopyQuestionToTemporaryCourse_InvalidConfiguration_FailedRun()
         {
             var questionAgilix = new Bfw.Agilix.DataContracts.Question()
             {
                 InteractionType = "2",
                 QuestionXml = ""
             };
             var counter = 0;
             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(items =>
             {

                 items.Items = counter == 0 ? new List<Item>() : new List<Item> { null };
                 counter++;
             }));

             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<PutItems>(items =>
             {
                 items.Items.Clear();
                 items.Items.Add(null);
             }));



             questionCommands.GetAgilixQuestion(Arg.Any<string>(), Arg.Any<string>()).Returns(questionAgilix);

             temporaryQuestionOperation.CopyQuestionToTemporaryCourse("12", "12d");


         }

        private const string tempQuizXml = @"<item id=""QBA_temp_quiz_185781_8656bc24-e4d6-4cce-b826-038f8b644537"" version=""4"" resourceentityid=""200117,D26"" actualentityid=""200117"" creationdate=""2014-08-12T12:20:55.047Z"" creationby=""7"" modifieddate=""2014-08-20T11:25:51.3240905Z"" modifiedby=""7"">
  <data>
    <type>Assessment</type>
 
    <timetocomplete>PT0S</timetocomplete>
    <category>0</category>
    <period inherit=""true"">0</period>
    <groupsetid>-1</groupsetid>
    <projectgroupsetid>-1</projectgroupsetid>
    <duedate>0001-01-01T00:00:00Z</duedate>
    <gradereleasedate>2014-08-20T11:25:51.3247757Z</gradereleasedate>
    <gradeentry>1</gradeentry>
    <inputtable inherit=""true"">0</inputtable>
    <completiontrigger>0</completiontrigger>
    <examflags>6144</examflags>
    <attemptlimit>0</attemptlimit>
    <examreviewrules>
      <rule setting=""Question"" condition=""true"" />
      <rule setting=""Answer"" condition=""true"" />
      <rule setting=""Possible"" condition=""true"" />
      <rule setting=""CorrectQuestion"" condition=""true"" />
      <rule setting=""CorrectChoice"" condition=""true"" />
      <rule setting=""Feedback"" condition=""true"" />
      <rule setting=""Feedback-GROUP"" condition=""true"" />
    </examreviewrules>
    <IsMarkAsCompleteChecked>false</IsMarkAsCompleteChecked>
    <meta-bfw_assigned>false</meta-bfw_assigned>
    <allowviewhints>false</allowviewhints>
    <percentsubstracthint>0</percentsubstracthint>
    <allowstudentsemailinstructors>false</allowstudentsemailinstructors>
  </data>
</item>";

        private Course GetTestCourse()
        {
            return new Course()
                   {
                       QuestionRepositoryCourseId = "1525",
                       ProductCourseId = "12",
                       FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                          {
                                              new CourseMetadataFieldDescriptor()
                                              {
                                                  Name = "bank",
                                                  CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                              {
                                                                                  new CourseMetadataFieldValue()
                                                                                  {
                                                                                      Text = "Test"
                                                                                  }
                                                                              }
                                              },

                                              new CourseMetadataFieldDescriptor()
                                              {
                                                  Name = "field",
                                                  CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                              {
                                                                                  new CourseMetadataFieldValue()
                                                                                  {
                                                                                      Text = "value"
                                                                                  }


                                                                              }
                                              }
                                          }
                   };
        }

    }
}
