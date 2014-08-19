using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Xml.Serialization;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.QuestionParserModule;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Business.QuestionParserModule.QML;
using Macmillan.PXQBA.Business.QuestionParserModule.QTI;
using Macmillan.PXQBA.Business.Services.Automapper;
using Macmillan.PXQBA.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Options;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    [DeploymentItem("FilesForValidation")]
    public class QuestionManagementServiceTests
    {
        private IQuestionManagementService questionManagementService;

        private IQuestionCommands questionCommands;
        private ITemporaryQuestionOperation temporaryQuestionOperation;
        private IProductCourseManagementService productCourseManagementService;
        private IKeywordOperation keywordOperation;
        private IParsedFileOperation parsedFileOperation;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;

        [TestInitialize]

        public void TestInitialize()
        {
            modelProfileService = Substitute.For<IModelProfileService>();
            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();

            QuestionParserProvider.AddParser(new QTIQuestionParser());

            questionCommands = Substitute.For<IQuestionCommands>();
            questionCommands.GetQuestionList(string.Empty, string.Empty, null, null, 0, 0)
                .ReturnsForAnyArgs(GetQuestions());
            questionCommands.GetComparedQuestionList(null, null, null, 0, 0).ReturnsForAnyArgs(GetComparedQuestions());

            temporaryQuestionOperation = Substitute.For<ITemporaryQuestionOperation>();
            productCourseManagementService = Substitute.For<IProductCourseManagementService>();
            keywordOperation = Substitute.For<IKeywordOperation>();
            SetSubstituteForParsedFileOperations();
            questionManagementService = new QuestionManagementService(questionCommands, temporaryQuestionOperation,
                productCourseManagementService, keywordOperation, parsedFileOperation);
        }




        [TestMethod]
        public void GetQuestionList_AnyParameters_ListOfQuestion()
        {
            var result = questionManagementService.GetQuestionList(new Course(), new List<FilterFieldDescriptor>(),
                new SortCriterion(), 1, 5);
            Assert.IsTrue(result.TotalItems == 2);
        }


        [TestMethod]
        public void GetComparedQuestionList_AnyParameters_ListOfComparesQuestion()
        {
            var result = questionManagementService.GetComparedQuestionList(null, null, null, 0, 0);
            Assert.IsTrue(result.TotalItems == 2);
        }

        [TestMethod]
        public void CreateQuestion_ChapterAndBank_SettedQuestionTemplate()
        {

            temporaryQuestionOperation.CreateQuestion(Arg.Any<string>(), Arg.Any<Question>())
                .Returns(x => (Question) x[1]);
            var result = questionManagementService.CreateQuestion(GetTestCourse(), "HTS", "bank", "chapter");
            Assert.IsTrue(result.ProductCourseSections.First().Bank == "bank");
            Assert.IsTrue(result.ProductCourseSections.First().Chapter == "chapter");
            Assert.IsTrue(result.ProductCourseSections.First().Title == "New Question");
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void CreateQuestion_AnyIncorrectParams_Exception()
        {
            temporaryQuestionOperation.CreateQuestion(Arg.Any<string>(), Arg.Any<Question>())
                .Returns(x => { throw new Exception(); }
                );
           questionManagementService.CreateQuestion(GetTestCourse(), "HTS", "bank", "chapter");     
        }

        [TestMethod]
        public void DuplicateQuestion_QuestionWithoutServiceFieldsOneProductCourse_CorrectlySettedDuplicateCopy()
        {
            var course = GetTestCourse();
            questionCommands.CreateQuestion(Arg.Any<string>(), Arg.Any<Question>()).Returns(x => (Question) x[1]);
            questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>()).Returns(new Question
                                                                                       {
                                                                                           Id = "15",
                                                                                           DuplicateFromShared = "3411",
                                                                                           ProductCourseSections = new List <QuestionMetadataSection>
                                                                                               {
                                                                                                   new QuestionMetadataSection
                                                                                                   {
                                                                                                       ProductCourseId = course.ProductCourseId,
                                                                                                   }
                                                                                               }
                                                                                       });
            var question = questionManagementService.DuplicateQuestion(course, "QuestionId");
            Assert.IsTrue(question.DuplicateFromShared == string.Empty);
            Assert.IsTrue(question.DuplicateFrom == "QuestionId");
            Assert.IsTrue(question.Status == ((int) QuestionStatus.InProgress).ToString());
        }

        [TestMethod]
        public void DuplicateQuestion_QuestionWithServiceFieldsTwoProductCourse_CorrectlySettedDuplicateCopy()
        {
            var course = GetTestCourse();
            questionCommands.CreateQuestion(Arg.Any<string>(), Arg.Any<Question>()).Returns(x => (Question) x[1]);
            questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>()).Returns(new Question()
                                                                                       {
                                                                                           Id = "15",
                                                                                           DuplicateFromShared = "3411",
                                                                                           ProductCourseSections = new List
                                                                                               <QuestionMetadataSection>
                                                                                               {
                                                                                                   new QuestionMetadataSection
                                                                                                   {
                                                                                                       ProductCourseId = course.ProductCourseId,
                                                                                                   },
                                                                                                   new QuestionMetadataSection
                                                                                                   {
                                                                                                       ProductCourseId = "55"
                                                                                                   }
                                                                                               }
                                                                                       });
            questionManagementService.DuplicateQuestion(course, "QuestionId");
            var question = questionManagementService.DuplicateQuestion(course, "QuestionId");
            Assert.IsTrue(question.DuplicateFromShared == string.Empty);
            Assert.IsTrue(question.DuplicateFrom == "QuestionId");
            Assert.IsTrue(question.Status == ((int) QuestionStatus.InProgress).ToString());
            Assert.IsTrue(question.ProductCourseSections.Count == 1);
            Assert.IsTrue(question.ProductCourseSections.First().ProductCourseId == course.ProductCourseId);
        }

        [TestMethod]
        public void UpdateQuestion_QuestionWithNewKeyWords_ProperlySettedQuestion()
        {
            var course = GetTestCourse();
            course.FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                      {
                                          new CourseMetadataFieldDescriptor()
                                          {
                                              Name = "keyword",
                                              CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                          {
                                                                              new CourseMetadataFieldValue()
                                                                              {
                                                                                  Text = "val1"
                                                                              },
                                                                               new CourseMetadataFieldValue()
                                                                              {
                                                                                  Text = "val2"
                                                                              }
                                                                          },
                                             Type = MetadataFieldType.Keywords
                                          }
                                      };

            temporaryQuestionOperation.CopyQuestionToSourceCourse(Arg.Any<string>(), Arg.Any<string>())
                .Returns(new Question()
                         {
                             Id = "14",
                             ProductCourseSections = new List<QuestionMetadataSection>
                                                                                         {
                                                                                             new QuestionMetadataSection
                                                                                             {
                                                                                                 ProductCourseId = course.ProductCourseId,
                                                                                                 DynamicValues = new Dictionary<string, List<string>>()
                                                                                                                 {
                                                                                                                     {"keyword", new List<string>()
                                                                                                                                 {
                                                                                                                                     "val1",
                                                                                                                                     "val3"
                                                                                                                                 }}
                                                                                                                 }
                                                                                             }
                                                                                         }
                         
                         });
            var restult = questionManagementService.UpdateQuestion(course, "15", new Question()
                                                                                 {
                                                                                     Id = "16"
                                                                                    
                                                                                });
            keywordOperation.AddKeywords(Arg.Any<string>(), 
                                         Arg.Any<string>(), 
                                         Arg.Do<IEnumerable<string>>(manuallyAddedValues => 
                                                                        Assert.IsTrue(manuallyAddedValues.Contains("val3")&& manuallyAddedValues.Count() == 1))
                                         );
            Assert.IsTrue(restult.Id == "14");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void UpdateQuestion_IncorrectParametrs_Exception()
        {
            questionCommands.UpdateQuestion(null, Arg.Any<string>()).ReturnsForAnyArgs(x => { throw new Exception(); });
            questionManagementService.UpdateQuestion(new Course(), "14", new Question());

        }

        [TestMethod]
        public void UpdateQuestionField_AnyCorrectParametrs_True()
        {
            
            questionCommands.UpdateQuestionField(null, null, null, null, null, null).ReturnsForAnyArgs(true);
            Assert.IsTrue(questionManagementService.UpdateQuestionField(new Course() {ProductCourseId = "23"}, null,
                null, null, new List<Capability>()));
        }

        [TestMethod]
        public void GetVersionHistory_IdWithNoHistory_EmptyResult()
        {
            var course = GetTestCourse();
            questionCommands.GetVersionHistory(course.QuestionRepositoryCourseId, Arg.Any<string>()).Returns(new List<Question>());
            Assert.IsFalse(questionManagementService.GetVersionHistory(course, "").Any());
        }

        [TestMethod]
        public void GetVersionHistory_NotDraftQuestionId_TwoVersions()
        {
            var course = GetTestCourse();
            var originalQuestion = new Question()
            {
                Id = "15"
            };
            questionCommands.GetVersionHistory(course.QuestionRepositoryCourseId, originalQuestion.Id).Returns(new List<Question>()
                                                                                                               {
                                                                                                                   new Question(){},
                                                                                                                   new Question(){}
                                                                                                               });

            Assert.IsTrue(questionManagementService.GetVersionHistory(course, originalQuestion.Id).Count() == 2);

        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetVersionHistory_IncorrectParametrs_Exception()
        {
            questionCommands.GetVersionHistory(Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(x => { throw new Exception(); });
            var course = GetTestCourse();
            questionManagementService.GetVersionHistory(course, string.Empty).Returns(new List<Question>());

        }

        [TestMethod]
        public void GetVersrions_DraftQuestionId_VersionsOfDraftAndOriginal()
        {
            var originalQuestion = new Question()
            {
                DuplicateFromShared = "14",
                Id = "15"
            };
            var draftQuestion = new Question()
            {
                DuplicateFromShared = "88989",
                DraftFrom = originalQuestion.Id,
                Id = "324"
            };
            var course = GetTestCourse();
            questionCommands.GetVersionHistory(course.QuestionRepositoryCourseId, originalQuestion.Id).Returns(new List<Question>()
                                                                                                               {
                                                                                                                   new Question()
                                                                                                                {
                                                                                                                   ModifiedDate = DateTime.Now.AddDays(-2)
                                                                                                                },
                                                                                                                 new Question()
                                                                                                                {
                                                                                                                  ModifiedDate = DateTime.Now.AddDays(-3)
                                                                                                                },
                                                                                                               });
            questionCommands.GetVersionHistory(course.QuestionRepositoryCourseId, draftQuestion.Id).Returns(new List<Question>()
                                                                                                            {
                                                                                                                new Question()
                                                                                                                {
                                                                                                                    DraftFrom = originalQuestion.Id,
                                                                                                                    ModifiedDate = DateTime.Now
                                                                                                                },
                                                                                                                 new Question()
                                                                                                                {
                                                                                                                    DraftFrom = originalQuestion.Id,
                                                                                                                    ModifiedDate = DateTime.Now
                                                                                                                },
                                                                                                            });

            var result = questionManagementService.GetVersionHistory(course, draftQuestion.Id);
            Assert.IsTrue(result.Any());
            Assert.IsTrue(result.Count() == 4);
            Assert.IsTrue(result.Count(x=> x.IsDraftInitialVersion) == 1);

        }


        [TestMethod]
        public void UpdateQuestionField_AnyIncorrectParametrs_False()
        {
            questionCommands.UpdateQuestionField(null, null, null, null, null, null).ReturnsForAnyArgs(false);
            Assert.IsFalse(questionManagementService.UpdateQuestionField(new Course() { ProductCourseId = "23" }, null,
                null, null, new List<Capability>()));
        }

        [TestMethod]
        public void PublishDraftToOriginal_AnyCorrectParametrs_True()
        {

            var originalQuestion = new Question()
            {
                DuplicateFromShared = "14",
                DraftFrom = "111",
                Id = "15"
            };
            var draftQuestion = new Question()
            {
                DuplicateFromShared = "88989",
                DraftFrom = originalQuestion.Id,
                Id = "324"
            };
            questionCommands.GetQuestion(Arg.Any<string>(), draftQuestion.Id).Returns(draftQuestion);
            questionCommands.GetQuestion(Arg.Any<string>(), originalQuestion.Id).Returns(originalQuestion);

            Assert.IsTrue(questionManagementService.PublishDraftToOriginal(GetTestCourse(), draftQuestion.Id));
            Assert.IsTrue(draftQuestion.DraftFrom == "111");
            Assert.IsTrue(draftQuestion.IsPublishedFromDraft);
            Assert.IsTrue(draftQuestion.Id == originalQuestion.Id);
        }

        [TestMethod]
        public void PublishDraftToOriginal_NotDraftQuestion_False()
        {

            var originalQuestion = new Question()
            {
                DuplicateFromShared = "14",
                DraftFrom = "111",
                Id = "15"
            };
            var draftQuestion = new Question()
            {
                DuplicateFromShared = "88989",
                DraftFrom = originalQuestion.Id,
                Id = "324"
            };
            questionCommands.GetQuestion(Arg.Any<string>(), draftQuestion.Id).Returns(draftQuestion);
            questionCommands.GetQuestion(Arg.Any<string>(), originalQuestion.Id).Returns(originalQuestion);

            Assert.IsTrue(questionManagementService.PublishDraftToOriginal(GetTestCourse(), draftQuestion.Id));
            Assert.IsFalse(questionManagementService.PublishDraftToOriginal(GetTestCourse(), "32"));
        }

        [TestMethod]
        public void PublishDraftToOriginal_AnyIncorrectParametrs_False()
        {
            var originalQuestion = new Question()
            {
                DuplicateFromShared = "14",
                DraftFrom = "111",
                Id = "15"
            };
            var draftQuestion = new Question()
            {
                DuplicateFromShared = "88989",
                DraftFrom = originalQuestion.Id,
                Id = "324"
            };
            questionCommands.GetQuestion(Arg.Any<string>(), draftQuestion.Id).Returns(draftQuestion);
            questionCommands.GetQuestion(Arg.Any<string>(), originalQuestion.Id).Returns((Question)null);

            Assert.IsFalse(questionManagementService.PublishDraftToOriginal(GetTestCourse(), draftQuestion.Id));
        }

        [TestMethod]
        public void UpdateSharedQuestionField_AnyCorrectParametrs_True()
        {

            var course = GetTestCourse();
            questionCommands.UpdateSharedQuestionField(null, null, null, null).ReturnsForAnyArgs(true);
            questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>()).Returns(new Question()
                                                                                       {
                                                                                            ProductCourseSections = new List<QuestionMetadataSection>
                                                                 {
                                                                     new QuestionMetadataSection
                                                                     {
                                                                     
                                                                     }
                                                                 }
                                                                                       });
            productCourseManagementService.GetProductCourse(Arg.Any<string>()).Returns(new Course());
            Assert.IsTrue(questionManagementService.UpdateSharedQuestionField(course, null, null, null));
        }

        [TestMethod]
        public void UpdateSharedQuestionField_NotDraftQuestion_False()
        {

            questionCommands.UpdateSharedQuestionField(null, null, null, null).ReturnsForAnyArgs(false);
            questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>())
                            .Returns(new Question
                                     {
                                         ProductCourseSections = new List<QuestionMetadataSection>
                                                                 {
                                                                     new QuestionMetadataSection
                                                                     {
                                                                         ParentProductCourseId
                                                                             = "13"
                                                                     }
                                                                 }
                                     });
            productCourseManagementService.GetProductCourse(Arg.Any<string>()).Returns(new Course());
            Assert.IsFalse(questionManagementService.UpdateSharedQuestionField(GetTestCourse(),null, null, null));
        }

        [TestMethod]
        public void CreateTemporaryQuestion_AnyParametrs_ProperlyReturnedQuestion()
        {
            temporaryQuestionOperation.CopyQuestionToTemporaryCourse(Arg.Any<string>(), Arg.Any<string>())
                .Returns(new Question() {Id = "14"});
            var restult = questionManagementService.CreateTemporaryQuestion(new Course(), "16");
            Assert.IsTrue(restult.Id == "14");
        }

        [TestMethod]
        public void GetTemporaryQuestion_AnyParametrs_ProperlyReturnedQuestion()
        {
            temporaryQuestionOperation.CopyQuestionToTemporaryCourse(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(new Question() { Id = "14" });
            var restult = questionManagementService.GetTemporaryQuestionVersion(new Course(), "14", "");
            Assert.IsTrue(restult.Id == "14");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetTemporaryQuestion_IncorrectParametrs_Exception()
        {
            temporaryQuestionOperation.CopyQuestionToTemporaryCourse(Arg.Any<string>(), Arg.Any<string>(),
                Arg.Any<string>()).Returns(x => { throw new Exception(); });
           questionManagementService.GetTemporaryQuestionVersion(new Course(), "14", "");
           
        }

        [TestMethod]
        public void RemoveFromTitle_AnyCorrectParametrs_True()
        {
            questionCommands.RemoveFromTitle(null, null, null).ReturnsForAnyArgs(true);
            Assert.IsTrue(questionManagementService.RemoveFromTitle(new string[] { }, GetTestCourse()));
        }

        [TestMethod]
        public void RemoveFromTitle_AnyIncorrectParametrs_False()
        {
            questionCommands.RemoveFromTitle(null, null, null).ReturnsForAnyArgs(false);
            Assert.IsFalse(questionManagementService.RemoveFromTitle(new string[] { }, GetTestCourse()));
        }


        [TestMethod]
        public void GetQuestion_AnyParametrs_QuestionWithCorrectId()
        {
            questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>()).Returns(new Question()
                                                                                       {
                                                                                           Id = "15"
                                                                                       });
            var question = questionManagementService.GetQuestion(new Course(), string.Empty);
            Assert.AreEqual("15", question.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetQuestion_AnyIncorrectWithVesrion_Exception()
        {

            questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(x => { throw new Exception(); });
            questionManagementService.GetQuestion(GetTestCourse(), string.Empty, "2");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetQuestion_AnyIncorrectNoVersion_Exception()
        {

            questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(x => { throw new Exception(); });
            questionManagementService.GetQuestion(GetTestCourse(), string.Empty, string.Empty);
        }

         [TestMethod]
        public void RestoreQuestionVersion_NotExistingVersionId_Null()
        {
            questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns((Question)null);
            var questionVersion = questionManagementService.RestoreQuestionVersion(GetTestCourse(), "32", "2");
            Assert.IsTrue(questionVersion == null);
        }

         [TestMethod]
         public void RestoreQuestionVersion_ExistingVersion_VersionWithCorrectlySettedFields()
         {
             var questionFromCommands = new Question()
                             {
                                 DuplicateFromShared = "88989",
                                 DraftFrom = "324",
                                 Version = 2
                             };
             var version = "2";
             questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>(), "2").Returns(questionFromCommands);

             var questionVersion = questionManagementService.RestoreQuestionVersion(GetTestCourse(), "32", version);
             Assert.IsTrue(questionVersion.RestoredFromVersion == version);
             Assert.IsTrue(questionVersion.DraftFrom == questionFromCommands.DraftFrom);
             Assert.IsTrue(string.IsNullOrEmpty(questionVersion.DuplicateFromShared));
         }

         [TestMethod]
         public void CreateDraft_QuestionWithOneProductSection_DraftQuestionWithCorrectFields()
         {
             var id = "15";
             var questionFromCommands = new Question()
              {
                 DuplicateFromShared = "88989",
                 DraftFrom = "324",
                 Id = id
              };
             questionCommands.GetQuestion(Arg.Any<string>(), id).Returns(questionFromCommands);
             temporaryQuestionOperation.CreateQuestion(Arg.Any<string>(), Arg.Any<Question>()).Returns(x=> (Question) x[1]);


             var result = questionManagementService.CreateDraft(GetTestCourse(), id);
             Assert.IsTrue(string.IsNullOrEmpty(result.DuplicateFromShared));
             Assert.IsTrue(result.Status == ((int)QuestionStatus.InProgress).ToString());
             Assert.IsTrue(result.Id != id);
             Assert.IsTrue(result.DraftFrom == id);
         }


         [TestMethod]
         public void CreateDraft_QuestionWithTwoProductSection_DraftQuestionWithCorrectFields()
         {
             var id = "15";
             var course = GetTestCourse();
             var questionFromCommands = new Question()
             {
                 DuplicateFromShared = "88989",
                 DraftFrom = "324",
                 Id = id,
                 ProductCourseSections = new List<QuestionMetadataSection>()
                                                                       {
                                                                           new QuestionMetadataSection()
                                                                           {
                                                                               ProductCourseId = course.ProductCourseId
                                                                           },

                                                                             new QuestionMetadataSection()
                                                                           {
                                                                               ProductCourseId = "1335"
                                                                           }
                                                                       }
             };
             questionCommands.GetQuestion(Arg.Any<string>(), id).Returns(questionFromCommands);
             temporaryQuestionOperation.CreateQuestion(Arg.Any<string>(), Arg.Any<Question>()).Returns(x => (Question)x[1]);


             var result = questionManagementService.CreateDraft(course, id);
             Assert.IsTrue(string.IsNullOrEmpty(result.DuplicateFromShared));
             Assert.IsTrue(result.Status == ((int)QuestionStatus.InProgress).ToString());
             Assert.IsTrue(result.Id != id);
             Assert.IsTrue(result.DraftFrom == id);
             Assert.IsTrue(result.ProductCourseSections.Count==1);
             Assert.IsTrue(result.ProductCourseSections.First().ProductCourseId== course.ProductCourseId);
         }

        [TestMethod]
        public void BulklUpdateQuestionField_AnyParameters_TransferNotSuccessfulFromQuestionCommands()
        {
            questionCommands.BulkUpdateQuestionField(null, null, null, null, null, null).ReturnsForAnyArgs(new BulkOperationResult()
            {
                IsSuccess = false
            });

            string[] questionIds = { "1", "2" };
            var result = questionManagementService.BulklUpdateQuestionField(GetTestCourse(), questionIds, "field", "bank 1", new List<Capability>());

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void BulklUpdateQuestionField_AnyParameters_TransferSuccessfulSuccessfulFromQuestionCommands()
        {
            questionCommands.BulkUpdateQuestionField(null, null, null, null, null, null).ReturnsForAnyArgs(new BulkOperationResult()
                                                                                                           {
                                                                                                               IsSuccess = true
                                                                                                           });

            string[] questionIds = { "1", "2" };
            var result = questionManagementService.BulklUpdateQuestionField(GetTestCourse(),questionIds, "field", "bank 1",new List<Capability>());

            Assert.IsTrue(result.IsSuccess);
        }


        [TestMethod]
        public void PublishToTitle_AnyParameters_TransferNotSuccessfulFromQuestionCommands()
        {
            var course = new Course();
            string[] questionIds = {"1", "2"};
            var result = questionManagementService.PublishToTitle(questionIds, 1, "bank 1", "chapter 1", course);

            Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void PublishToTitle_AnyParameters_TransferSuccessfulSuccessfulFromQuestionCommands()
        {
            questionCommands.UpdateQuestions(Arg.Any<IEnumerable<Question>>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(true);

            var course = new Course();
            string[] questionIds = new[] {"1", "2"};
            var result = questionManagementService.PublishToTitle(questionIds, 1, "bank 1", "chapter 1", course);

            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void PublishToTitle_AnyParameters_NewProductCourseSectionAdded()
        {
            var course = GetTestCourse();
            course.FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                      {
                                          new CourseMetadataFieldDescriptor()
                                          {
                                              Name = "keywords",
                                              CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                          {
                                                                              new CourseMetadataFieldValue()
                                                                              {
                                                                                  Text = "val2"
                                                                              },
                                                                                 new CourseMetadataFieldValue()
                                                                              {
                                                                                  Text = "val3"
                                                                              }

                                                                          }
                                          }
                                      };

            string[] questionIds = new[] {"1", "2"};
            int newProductCourseId = 1100;
            List<Question> questions = new List<Question>
                                       {
                                           new Question()
                                           {
                                               ProductCourseSections = new List<QuestionMetadataSection>()
                                                                       {
                                                                           new QuestionMetadataSection()
                                                                           {
                                                                               ProductCourseId = course.ProductCourseId,
                                                                               DynamicValues = new Dictionary<string, List<string>>()
                                                                                     {
                                                                                         {"keywords", new List<string>(){"val1", "val2"}}
                                                                                     }
                                                                           }
                                                                       },
                                                DefaultSection = new QuestionMetadataSection()
                                                                 {
                                                                     DynamicValues = new Dictionary<string, List<string>>()
                                                                                     {
                                                                                         {"keywords", new List<string>(){"val1", "val2"}}
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
                                                                       },
                                                  DefaultSection = new QuestionMetadataSection()
                                                                 {
                                                                     DynamicValues = new Dictionary<string, List<string>>()
                                                                                     {
                                                                                         {"keywords", new List<string>(){"val1", "val2"}}
                                                                                     }
                                                                 }
                                           }
                                       };

            questionCommands.UpdateQuestions(Arg.Any<IEnumerable<Question>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            questionCommands.GetQuestions(Arg.Any<string>(), Arg.Any<string[]>()).Returns(questions);
            productCourseManagementService.GetProductCourse(Arg.Any<string>()).ReturnsForAnyArgs(course);

            var result = questionManagementService.PublishToTitle(questionIds, newProductCourseId, "bank 1", "chapter 1", course);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(questions[0].ProductCourseSections.Count(q => q.ProductCourseId == newProductCourseId.ToString()) == 1);
            Assert.IsTrue(questions[1].ProductCourseSections.Count(q => q.ProductCourseId == newProductCourseId.ToString()) == 1);
            Assert.IsTrue(questions[0].ProductCourseSections.First(q => q.ProductCourseId == newProductCourseId.ToString()).DynamicValues.First().Value.Count == 1);
        }

        [TestMethod]
        public void RemoveRelatedQuestionTempResources_QuestionWithImages_SuccessRun()
        {
            var course = GetTestCourse();

            questionCommands.GetAgilixQuestion(null, null).ReturnsForAnyArgs(new Bfw.Agilix.DataContracts.Question()
                                                                             {
                                                                                 QuestionXml = questionBodyWithImages
                                                                             });
            questionManagementService.RemoveRelatedQuestionTempResources("QSDDF", course);
        }


        [TestMethod]
        public void RemoveRelatedQuestionTempResources_QuestionWithoutImages_SuccessRun()
        {
            var course = GetTestCourse();
            questionCommands.GetAgilixQuestion(null, null).ReturnsForAnyArgs(new Bfw.Agilix.DataContracts.Question()
                                                                             {
                                                                                 QuestionXml = questionBodyWithoutImages
                                                                             });
            questionManagementService.RemoveRelatedQuestionTempResources("QSDDF", course);
        }

        [TestMethod]
        public void ImportFile_ParsedQuestionFileWithOnequestion_One()
        {
            var course = GetTestCourse();
            productCourseManagementService.GetProductCourse(null).ReturnsForAnyArgs(course);
            modelProfileService.GetQuestionFromParsedQuestion(Arg.Any<ParsedQuestion>(), course)
                .Returns(x => GetQuestionFromParsedQuestion((ParsedQuestion) x[0], (Course) x[1]));
            Assert.IsTrue(questionManagementService.ImportFile(1, course.ProductCourseId) == 1);
        }

        [TestMethod]
        public void ImportFile_ParsedQuestionFileWithFiveQuestions_Five()
        {

            var course = GetTestCourse();

            modelProfileService.GetQuestionFromParsedQuestion(Arg.Any<ParsedQuestion>(), course)
                .Returns(x => GetQuestionFromParsedQuestion((ParsedQuestion) x[0], (Course) x[1]));
            productCourseManagementService.GetProductCourse(null).ReturnsForAnyArgs(course);
            Assert.IsTrue(questionManagementService.ImportFile(5, course.ProductCourseId) == 5);

        }


        [TestMethod]
        public void ImportQuestions_TwoCorrectCoursesAndQuestions_SuccesImportAndTargetCourseIsUpdated()
        {

            var course = GetTestCourse();
            var targetCourse = GetTestCourse();
            targetCourse.QuestionRepositoryCourseId = "1535";
            targetCourse.FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                            {
                                                new CourseMetadataFieldDescriptor()
                                                {
                                                    Name = "keyword",
                                                    CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                                {
                                                                                    new CourseMetadataFieldValue()
                                                                                    {
                                                                                        Text = "val1"
                                                                                    }
                                                                                },
                                                    Type =  MetadataFieldType.Keywords
                                                }
                                            };
            var questions = new List<Question>
                            {
                                new Question
                                {
                                    ProductCourseSections = new List<QuestionMetadataSection>
                                                            {
                                                                new QuestionMetadataSection
                                                                {
                                                                    ProductCourseId = course.ProductCourseId,
                                                                    DynamicValues = new Dictionary<string, List<string>>()
                                                                                    {
                                                                                        {"keyword", new List<string>(){"val2"}}
                                                                                    }
                                                                }
                                                            },
                                    DefaultSection = new QuestionMetadataSection()
                                                     {
                                                         Chapter = "chapter"
                                                     }
                                }
                            };

            keywordOperation.GetKeywordList(targetCourse.ProductCourseId, "keyword")
                .Returns(new List<string>() {"val2"});
            questionCommands.GetQuestions(null, null).ReturnsForAnyArgs(questions);
            Assert.IsTrue(questionManagementService.ImportQuestions(course, new[] {"1", "2"}, targetCourse));
            Assert.IsTrue(questions.Any( x=> string.IsNullOrEmpty(x.DefaultSection.Chapter)));
            Assert.IsTrue(targetCourse.FieldDescriptors.First().CourseMetadataFieldValues.Select(x=> x.Text).Contains("val2"));
        }


        [TestMethod]
        public void ImportQuestions_NullCourses_FailedImport()
        {
            Assert.IsFalse(questionManagementService.ImportQuestions(null, new[] {"1", "2"}, null));
        }

        [TestMethod]
        public void ValidateFile_FileNameAndByteArrayWithFiveQuestions_ParsedRestultWithId()
        {
            var fileName = "qti_with_images_5_question.zip";
            Assert.IsTrue(File.Exists(fileName));
            parsedFileOperation.AddParsedFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<byte[]>()).Returns(14);

            var result = questionManagementService.ValidateFile(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Id == 14);
            Assert.IsTrue(result.FileValidationResults.First().QuestionParsed == 5);
        }

        [TestMethod]
        public void ValidateFile_FileNameAndByteArrayWithOneQuestion_ParsedRestultWithId()
        {
            var fileName = "qti_one_question_with_image.zip";
            Assert.IsTrue(File.Exists(fileName));
            parsedFileOperation.AddParsedFile(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<byte[]>()).Returns(14);

            var result = questionManagementService.ValidateFile(fileName, File.ReadAllBytes(fileName));
            Assert.IsTrue(result.FileValidationResults.First().Id == 14);
            Assert.IsTrue(result.FileValidationResults.First().QuestionParsed == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ValidateFile_InvalidFile_FailedRun()
        {

            var result = questionManagementService.ValidateFile("test", null);
            Assert.IsTrue(result.FileValidationResults.First().Id == 14);
            Assert.IsTrue(result.FileValidationResults.First().QuestionParsed == 1);
        }

        [TestMethod]
        public void GetValidatedFile_AnyFileId()
        {
            parsedFileOperation.GetParsedFile(Arg.Any<long>()).Returns(new ParsedFile());
            var result = questionManagementService.GetValidatedFile(1534);
            Assert.IsFalse(result == null);
        }

        [TestMethod]
        public void DeleteTemporaryQuestionWithQuiz_AnyQuestionId()
        {
            questionManagementService.DeleteTemporaryQuestionWithQuiz("B143DSJDS9230XPQElLQTXIWKXW");
        }

        #region private methods

        public PagedCollection<Question> GetQuestions()
        {
            return new PagedCollection<Question>()
                   {
                       CollectionPage = new List<Question>()
                                        {
                                            new Question()
                                            {
                                                ProductCourseSections = new List<QuestionMetadataSection>()
                                                                        {
                                                                            new QuestionMetadataSection()
                                                                        }
                                            },
                                            new Question()
                                            {
                                                ProductCourseSections = new List<QuestionMetadataSection>()
                                                                        {
                                                                            new QuestionMetadataSection()
                                                                        }
                                            }
                                        },
                       TotalItems = 2
                   };
        }

        private string GetSerializedQuestionData(QuestionParserModule.DataContracts.ValidationResult result)
        {
            string questionsData;
            using (var writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                var serializer = new XmlSerializer(typeof (List<ParsedQuestion>));
                serializer.Serialize(writer,
                    result.FileValidationResults.First().Questions.Where(x => x.IsParsed).ToList());
                questionsData = writer.ToString();
            }

            return questionsData;
        }

        private Question GetQuestionFromParsedQuestion(ParsedQuestion parsedQuestion, Course course)
        {
            var question = new Question();
            question.ProductCourseSections.Add(new QuestionMetadataSection
                                               {
                                                   ProductCourseId = course.ProductCourseId,
                                                   Title = parsedQuestion.Title,
                                                   DynamicValues = parsedQuestion.MetadataSection
                                               });
            return question;
        }


        private void SetSubstituteForParsedFileOperations()
        {
            parsedFileOperation = Substitute.For<IParsedFileOperation>();
            parsedFileOperation.GetParsedFile(5).Returns(GetParsedFile("qti_with_images_5_question.zip", 5));
            parsedFileOperation.GetParsedFile(1).Returns(GetParsedFile("qti_one_question_with_image.zip", 1));
        }

        private ParsedFile GetParsedFile(string fileName, int id)
        {


            var validationResut = QuestionParserProvider.Parse(fileName, File.ReadAllBytes(fileName));
            var parsedFile = new ParsedFile()
                             {
                                 FileName = fileName,
                                 Id = id,
                                 QuestionsData = GetSerializedQuestionData(validationResut),
                                 ResourcesData =
                                     StreamHelper.SerializeToByte(
                                         validationResut.FileValidationResults.First().Resources)
                             };

            return parsedFile;
        }

        private PagedCollection<ComparedQuestion> GetComparedQuestions()
        {
            return new PagedCollection<ComparedQuestion>()
                   {
                       CollectionPage = new List<ComparedQuestion>()
                                        {
                                            new ComparedQuestion(),
                                            new ComparedQuestion()
                                        },
                       TotalItems = 2
                   };
        }

        private Course GetTestCourse()
        {
         return   new Course()
            {
                QuestionRepositoryCourseId = "1525",
                ProductCourseId = "2135",
                FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
            };
        }


    #endregion

        #region private fields
        private const string questionBodyWithImages = @"<question questionid=""88b12b6e-01b7-4ada-b97e-148a49e6d0cb"" version=""2"" resourceentityid=""39768,62"" creationdate=""2014-07-08T09:49:02.167Z"" creationby=""7"" modifieddate=""2014-07-08T09:49:08.9233687Z"" modifiedby=""7"" flags=""4"" actualentityid=""39768"" schema=""2"" partial=""false"">
                  <meta>
                    
                  </meta>
                  <answer>
                    <value>1</value>
                  </answer>
                  <body>&lt;div&gt;text a lot of&amp;nbsp;[~] texttext a lot of text&lt;img src=""[~]/tumblr_m0igv3QYdQ1r28r9f.gif"" border=""0"" alt=""[~]/tumblr_m0igv3QYdQ1r28r9f.gif"" title=""[~]/tumblr_m0igv3QYdQ1r28r9f.gif"" width=""140"" height=""140"" /&gt;text a lot of text&amp;nbsp; text a lot&amp;nbsp;[~] of text text a lot of texttext a lot of text [~]&lt;/div&gt;&lt;div&gt;&amp;nbsp;text a lot of texttext&amp;nbsp; [~] /&amp;nbsp; sdfdf /dfs dfdfsfd/ [~] [~]a lot of text&lt;/div&gt;&lt;div&gt;&amp;nbsp;&lt;/div&gt;</body>
                  <interaction type=""choice"">
                    <choice id=""1"">
                      <body>&lt;div&gt;&lt;img src=""[~]/funpage24.jpg"" border=""0"" alt=""[~]/funpage24.jpg"" title=""[~]/funpage24.jpg"" width=""130"" height=""69"" /&gt;&lt;/div&gt;</body>
                    </choice>
                    <choice id=""2"">
                      <body>2</body>
                    src=""[~]/folder/image.jpg""
                    </choice>
                    <choice id=""3"">
                      <body>3</body>
                    </choice>
                  </interaction>
                </question>";


        private const string questionBodyWithoutImages = @"<question questionid=""88b12b6e-01b7-4ada-b97e-148a49e6d0cb"" version=""2"" resourceentityid=""39768,62"" creationdate=""2014-07-08T09:49:02.167Z"" creationby=""7"" modifieddate=""2014-07-08T09:49:08.9233687Z"" modifiedby=""7"" flags=""4"" actualentityid=""39768"" schema=""2"" partial=""false"">
                  <meta>
                    
                  </meta>
                  <answer>
                    <value>1</value>
                  </answer>
                
                    </choice>
                    <choice id=""2"">
                      <body>2</body>
                 
                    </choice>
                    <choice id=""3"">
                      <body>3</body>
                    </choice>
                  </interaction>
                </question>";
        #endregion
    }
}
