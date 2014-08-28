using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.Mappers;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Common.Caching;
using Bfw.Common.JqGridHelper;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Helpers;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Business.Services.Automapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Course = Macmillan.PXQBA.Business.Models.Course;
using Question = Bfw.Agilix.DataContracts.Question;
using QuestionChoice = Bfw.Agilix.DataContracts.QuestionChoice;

namespace Macmillan.PXQBA.Business.Commands.Tests
{
    [TestClass]
    public class QuestionCommandsTest
    {
        private IContext context;
        private IProductCourseOperation productCourseOperation;
        private IUserOperation userOperation;
        private INoteCommands noteCommands;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;
        private IQuestionCommands questionCommands;
        private string productCourseId = "12";
      
        [TestInitialize]
        public void TestInitialize()
        {
            context = Substitute.For<IContext>();
            context.CurrentUser = new UserInfo();
            context.CurrentUser.Id = "633478";

            productCourseOperation = Substitute.For<IProductCourseOperation>();
            questionCommands = new QuestionCommands(context, productCourseOperation);

            userOperation = Substitute.For<IUserOperation>();
            noteCommands = Substitute.For<INoteCommands>();
            modelProfileService = new ModelProfileService(productCourseOperation, questionCommands, userOperation, noteCommands);

            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();
        }


        [TestMethod]
        public void GetQuestionList_TwoUserFilters_NoQuestions()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(x => ExecuteAsAdminFillNoQuestions(((Search)x))));

            var filter = new List<FilterFieldDescriptor>
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Bank,
                                 Values = new List<string>(){"Bank 1"}
                             },
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Chapter,
                                 Values = new List<string>(){"Chapter 1"}
                             }
                         };
            AddNessesaryProductCourseFilter(filter);



            var criteria = new SortCriterion
            {
                ColumnName = MetadataFieldNames.DlapType,
                SortType = SortType.Asc
            };

            var  result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);
            Assert.IsTrue(result.TotalItems == 0);

        }

        [TestMethod]
        public void GetQuestionList_OneUserFilters_NoQuestions()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(x => ExecuteAsAdminFillNoQuestions(((Search)x))));

            var filter = new List<FilterFieldDescriptor>
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Bank,
                                 Values = new List<string>(){"Bank 1"}
                             }
                           
                         };
            AddNessesaryProductCourseFilter(filter);



            var criteria = new SortCriterion
            {
                ColumnName = MetadataFieldNames.DlapType,
                SortType = SortType.Desc
            };

            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);
            Assert.IsTrue(result.TotalItems == 0);

        }

        [TestMethod]
        public void GetQuestionList_OneUserFilter_ListOfTwoFilteredQuestions()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));

            var filter = new List<FilterFieldDescriptor>
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Bank,
                                 Values = new List<string>(){"Bank 1"}
                             }
                         };

           
            AddNessesaryProductCourseFilter(filter);


            var criteria = new SortCriterion()
            {
                SortType = SortType.None,
                ColumnName = "CustomField"
            };

            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);
            Assert.IsTrue(result.TotalItems == 2);
            Assert.IsTrue(result.CollectionPage.ToList().Select(x => x.Id).Contains("4684f693-7997-4dc2-8496-56eb645e47ac"));
            Assert.IsTrue(result.CollectionPage.ToList().Select(x => x.ProductCourseSections.First().Sequence).Contains("2"));
        }

        [TestMethod]
        public void GetQuestionList_OneUserFilter_ListOfTwoFilteredQuestionsWithSequenceMaxIntValue()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestionsWithCorruptedSequense));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));

            var filter = new List<FilterFieldDescriptor>
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Bank,
                                 Values = new List<string>(){"Bank 1"}
                             }
                         };


            AddNessesaryProductCourseFilter(filter);


            var criteria = new SortCriterion
            {
                ColumnName = MetadataFieldNames.Sequence,
                SortType = SortType.Desc
            };
            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);
            Assert.IsTrue(result.TotalItems == 2);
            Assert.IsTrue(result.CollectionPage.ToList().Count(x => string.IsNullOrEmpty(x.ProductCourseSections.First().Sequence) ) == 2);
        }
    [TestMethod]
        public void GetQuestionList_OneUserFilter_ListOfTwoFilteredQuestionsWithFilteredDlapType()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestionsWithCorruptedSequense));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));

            var filter = new List<FilterFieldDescriptor>
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Bank,
                                 Values = new List<string>(){"Bank 1"}
                             }
                         };


            AddNessesaryProductCourseFilter(filter);


            var criteria = new SortCriterion
            {
                ColumnName = MetadataFieldNames.DlapType,
                SortType = SortType.Asc
            };
            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);
            Assert.IsTrue(result.TotalItems == 2);
            Assert.IsTrue(result.CollectionPage.ToList().Count(x => string.IsNullOrEmpty(x.ProductCourseSections.First().Sequence) ) == 2);
        }

        [TestMethod]
        public void GetQuestionList_NoSortingParameters_SeqSortingByDefault()
        {
            const string expectedFieldsParameters = "draftfrom|product-course-id-12/bank";
            bool searchCommandCorrectlyBuildSortingCriteria = false;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));
            
            context.SessionManager.CurrentSession.When(x=>x.ExecuteAsAdmin(
                                                      Arg.Is<Search>(a=>a.SearchParameters.Fields==expectedFieldsParameters)
                                                      )).Do(x => searchCommandCorrectlyBuildSortingCriteria=true);

            var filter = new List<FilterFieldDescriptor>()
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.ContainsText,
                                 Values = new List<string>(){"choice"}
                             }
                         };
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion
            {
              SortType = SortType.None
            };

            questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);

            Assert.IsTrue(searchCommandCorrectlyBuildSortingCriteria);
        }

        [TestMethod]
        public void GetQuestionList_SeqByDesc_SeqSortingByDesc()
        {
            const string expectedFieldsParameters = "draftfrom|product-course-id-12/bank";
            bool searchCommandCorrectlyBuildSortingCriteria = false;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));

            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(
                                                      Arg.Is<Search>(a => a.SearchParameters.Fields == expectedFieldsParameters)
                                                      )).Do(x => searchCommandCorrectlyBuildSortingCriteria = true);

            var filter = new List<FilterFieldDescriptor>()
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Flag,
                                 Values = new List<string>()
                                          {
                                              ((int)QuestionFlag.Flagged).ToString(),
                                                ((int)QuestionFlag.NotFlagged).ToString()
                                          }
                             }
                         };
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion
                           {
                               ColumnName = MetadataFieldNames.Sequence,
                               SortType = SortType.Desc
                           };

            questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);

            Assert.IsTrue(searchCommandCorrectlyBuildSortingCriteria);
        }


        [TestMethod]
        public void GetQuestionList_SortingCustomFieldDesc_ApplySortingForSearchCommand()
        {
            const string expectedFieldsParameters = "draftfrom|product-course-id-12/CustomField";
            bool searchCommandCorrectlyBuildSortingCriteria = false;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));

            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(
                                                      Arg.Is<Search>(a => a.SearchParameters.Fields == expectedFieldsParameters)
                                                      )).Do(x => searchCommandCorrectlyBuildSortingCriteria = true);

            var filter = new List<FilterFieldDescriptor>()
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.DlapType,
                                 Values = new List<string>(){"choice"}
                             }
                         };
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion()
                           {
                               SortType = SortType.Desc,
                               ColumnName = "CustomField"
                           };

            questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);

            Assert.IsTrue(searchCommandCorrectlyBuildSortingCriteria);
        }
        [TestMethod]
        public void GetQuestionList_SortingCustomFieldDescForQuestionWithDrafts_ApplySortingForSearchCommand()
        {
            const string expectedFieldsParameters = "draftfrom|product-course-id-12/CustomField";
            bool searchCommandCorrectlyBuildSortingCriteria = false;

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillThreeQuestionsWithDrafts));
            var counter = 0;
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(x =>
                                                                                      {
                                                                                          if (counter < 2)
                                                                                          {
                                                                                              ExecuteAsAdminGetThreeAgilixQuestionsWithDrafts(x);
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                             ExecuteAsAdminGetTwoAgilixQuestions(x);
                                                                                          }
                                                                                          counter++;
                                                                                      }));

            context.SessionManager.CurrentSession.When(x => x.ExecuteAsAdmin(
                                                      Arg.Is<Search>(a => a.SearchParameters.Fields == expectedFieldsParameters)
                                                      )).Do(x => searchCommandCorrectlyBuildSortingCriteria = true);

            var filter = new List<FilterFieldDescriptor>()
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.DlapType,
                                 Values = new List<string>(){"choice"}
                             }
                         };
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion()
                           {
                               SortType = SortType.Desc,
                               ColumnName = "CustomField"
                           };

            questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 10);

            Assert.IsTrue(searchCommandCorrectlyBuildSortingCriteria);
        }

        [TestMethod]
        public void GetQuestionList_OneQuestionPerPageAndPageNo2_ReturnSecondQuestion()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestion));


            var filter = new List<FilterFieldDescriptor>()
                         {
                             new FilterFieldDescriptor()
                             {
                                 Field = MetadataFieldNames.Flag,
                                 Values = new List<string>()
                                          {
                                              ((int)QuestionFlag.NotFlagged).ToString()
                                          }
                             }
                         };
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion
            {
                ColumnName = "questionstatus",
                SortType = SortType.Asc
            };

            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 1, 1);

            Assert.IsTrue(result.TotalItems==2);
            Assert.IsTrue(result.CollectionPage.Count() == 1);
            Assert.IsTrue(result.CollectionPage.FirstOrDefault().Id == "4684f693-7997-4dc2-8496-56eb645e47ac");
        }

        [TestMethod]
        public void GetQuestionList_OneQuestionPerPageAndPageNo1_ReturnFirstQuestion()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestion));

            var filter = new List<FilterFieldDescriptor>();
            AddNessesaryProductCourseFilter(filter);
            var criteria = new SortCriterion();

            var result = questionCommands.GetQuestionList(productCourseId, productCourseId, filter, criteria, 0, 1);

            Assert.IsTrue(result.TotalItems == 2);
            Assert.IsTrue(result.CollectionPage.Count() == 1);
            Assert.IsTrue(result.CollectionPage.FirstOrDefault().Id == "f13f2cd1-2ddf-430c-85c9-2577a5f009f4");
        }


        [TestMethod]
        public void GetComparedQuestionList_OneQuestionPerPageAndPageNo1_ReturnFirstQuestion()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestion));

           
            var result = questionCommands.GetComparedQuestionList("131", "431", "3541", 0, 1);

            Assert.IsTrue(result.TotalItems == 2);
            Assert.IsTrue(result.CollectionPage.Count() == 1);
        }


        //Use VPN to get Faceted xml for tests
        [TestMethod]
        public void GetFacetedResults_OneQuestionPerPageAndPageNo1_ReturnFirstQuestion()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(search =>
                                                                                {
                                                                                    Assert.IsTrue(search.SearchParameters.Facet);
                                                                                 //   Assert.IsTrue(search.SearchParameters.Query == "");
                                                                                    search.ParseResponse(new DlapResponse(XDocument.Parse(facetedResult)));
                                                                                }));

            var result = questionCommands.GetFacetedResults("210017", "12", "chapter");
            Assert.IsTrue(result.Count() == 24);
            Assert.IsTrue(result.First(x => x.FacetedFieldValue == "Chapter 6").FacetedCount == 434);

         
        }

        [TestMethod]
        public void CreateQuestions_TwoQuestionsToCreateWithoutBank_ProperplySettedQuestionsFieldsWithInitialSequence()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillNoQuestions));
            var course = GetTestCourse();
            var questions = new List<Models.Question>
                           {
                               new Models.Question()
                               {
                                   Id = "1",
                                   Version = 3,
                                   ProductCourseSections = new List<QuestionMetadataSection>()
                                                           {
                                                               new QuestionMetadataSection()
                                                               {
                                                                   ProductCourseId = course.ProductCourseId
                                                               }
                                                           },
                                   InteractionType = "2"

                               },
                               new Models.Question()
                               {
                                   Id = "1",
                                   ProductCourseSections = new List<QuestionMetadataSection>()
                                                           {
                                                               new QuestionMetadataSection()
                                                               {
                                                                   ProductCourseId = course.ProductCourseId
                                                               }
                                                           },
                                  InteractionType = "2"
                               }
                           };

           questionCommands.CreateQuestions(course.ProductCourseId, questions);

            foreach (var question in questions)
            {
                Assert.IsTrue(question.Id != "1");
                Assert.IsTrue(question.ModifiedBy == context.CurrentUser.Id);
                Assert.IsTrue(question.ProductCourseSections.First().Sequence == "1");
            }


          

        }

        [TestMethod]
        public void CreateQuestions_TwoQuestionsToCreate_ProperplySettedQuestionsSeq()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            var course = GetTestCourse();
            var questions = new List<Models.Question>
                           {
                               new Models.Question()
                               {
                                   Id = "1",
                                   Version = 3,
                                   ProductCourseSections = new List<QuestionMetadataSection>()
                                                           {
                                                               new QuestionMetadataSection()
                                                               {
                                                                   ProductCourseId = course.ProductCourseId,
                                                                   Bank = "Test Bank"
                                                               }
                                                           },
                                   InteractionType = "2"

                               },
                               new Models.Question()
                               {
                                   Id = "1",
                                   ProductCourseSections = new List<QuestionMetadataSection>()
                                                           {
                                                               new QuestionMetadataSection()
                                                               {
                                                                   ProductCourseId = course.ProductCourseId,
                                                                   Bank = "Test Bank"
                                                               }
                                                           },
                                  InteractionType = "2"
                               }
                           };

            questionCommands.CreateQuestions(course.ProductCourseId, questions);

            foreach (var question in questions)
            {
                Assert.IsTrue(question.Id != "1");
                Assert.IsTrue(question.ModifiedBy == context.CurrentUser.Id);
                Assert.IsTrue(question.ProductCourseSections.First().Sequence == "19");
            }

        } 
        [TestMethod]
        public void CreateQuestions_NoQuestionsToCreate_EmptyResult()
        {
           
            var course = GetTestCourse();
            var questions = new List<Models.Question>();

            questionCommands.CreateQuestions(course.ProductCourseId, questions);

            Assert.IsFalse(questions.Any());
        }

        [TestMethod]
        public void CreateQuestion_QuestionToCreate_ProperplySettedQuestionsSeq()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            var course = GetTestCourse();
            var question = new Models.Question()
                            {
                                Id = "1",
                                Version = 3,
                                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = course.ProductCourseId,
                                                                Bank = "Test Bank"
                                                            }
                                                        },
                                InteractionType = "2"

                            };
                             

            questionCommands.CreateQuestion(course.ProductCourseId, question);
            Assert.IsTrue(question.Id != "1");
            Assert.IsTrue(question.ModifiedBy == context.CurrentUser.Id);
            Assert.IsTrue(question.ProductCourseSections.First().Sequence == "19");
           

        }

        [TestMethod]
        public void ExecuteSolrUpdateTask_NoParametrs_SuccesRun()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetTaskList>(ExecuteAsAdminReturnTaskList));
            questionCommands.ExecuteSolrUpdateTask();
        }

        [TestMethod]
        public void ExecuteSolrUpdateTask_NoTask_SuccesRun()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetTaskList>(x => x.ParseResponse(new DlapResponse(XDocument.Parse(@"<responses>
                                                                                                                                          
                                                                                                                                        </responses>")))));
            questionCommands.ExecuteSolrUpdateTask();
        }


         [TestMethod]
        public void ExecutePutQuestion_AgilixQuestionsWithVersion_ProperlySettedDuplicateFromShared()
         {
             var question = new Question()
                            {
                                QuestionVersion = "14",
                                MetadataElements = new Dictionary<string, XElement>
                                                   {
                                                       {MetadataFieldNames.DuplicateFromShared, new XElement("test")}
                                                   },
                                InteractionType = "2"
                            };
             
             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetTaskList>(ExecuteAsAdminReturnTaskList));
             questionCommands.ExecutePutQuestion(question);
             Assert.IsTrue(question.MetadataElements[MetadataFieldNames.DuplicateFromShared].Name != "test");
        }


         [TestMethod]
         [ExpectedException(typeof(ArgumentNullException))]
         public void ExecutePutQuestion_InvalidQuestion_FailedOnInvalidateCache()
         {
             var question = new Question()
             {
                 QuestionVersion = "14",
                 MetadataElements = new Dictionary<string, XElement>
                                                   {
                                                       {MetadataFieldNames.DuplicateFromShared, new XElement("test")}
                                                   }
             };

             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetTaskList>(ExecuteAsAdminReturnTaskList));
             questionCommands.ExecutePutQuestion(question);
         }

         [TestMethod]
         public void DeleteItem_Params_SuccessRun()
         {
             questionCommands.DeleteItem("2423", " 4353");
         }

         [TestMethod]
         [ExpectedException(typeof(ArgumentNullException))]
         public void DeleteQuestion_Nulls_FailedRun()
         {
             questionCommands.DeleteQuestion(null, null);
         }


         [TestMethod]
         [ExpectedException(typeof(ArgumentNullException))]
         public void DeleteQuestion_NotExistingQuestion_FailedRunOnInvalidateCache()
         {
             questionCommands.DeleteQuestion("2423", "4353");
         }


         [TestMethod]
         public void DeleteQuestion_CorrectParams_SuccessRun()
         {
             context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestion));
             questionCommands.DeleteQuestion("2423", "4353");
         }

        [TestMethod]
        public void GetQuestionDrafts_AnyQuestion_QuestionDrafts()
        {
            var course = GetTestCourse();
            var question = new Models.Question
                           {
                               Id = "f13f2cd1-2ddf-430c-85c9-2577a5f009f4"
                           };
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(command =>
                                                                                      {
                                                                                          Assert.IsTrue(command.SearchParameters.QuestionIds.Contains("4684f693-7997-4dc2-8496-56eb645e47ac"));
                                                                                          ExecuteAsAdminGetTwoAgilixQuestions(command);
                                                                                      }));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(search =>
                                                                                {
                                                                                    Assert.IsTrue(search.SearchParameters.Query == string.Format("(dlap_class:question) AND (draftfrom:{0})", question.Id));
                                                                                    Assert.IsTrue(search.SearchParameters.EntityId == course.QuestionRepositoryCourseId);
                                                                                    ExecuteAsAdminFillTwoQuestions(search);
                                                                                }));
            var drafts = questionCommands.GetQuestionDrafts(course.QuestionRepositoryCourseId, question);
            Assert.IsTrue(drafts.Count() == 2);
            Assert.IsTrue(drafts.Select(d => d.Id).Contains("4684f693-7997-4dc2-8496-56eb645e47ac"));
        }


        [TestMethod]
        public void GetVersionHistory_AnyQuestion_QuestionHistoryWithCorrectPreviewLinks()
        {
            
            
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));
            var course = GetTestCourse();
            var versions = questionCommands.GetVersionHistory(course.QuestionRepositoryCourseId, "21");
            Assert.IsTrue(versions.Count() == 2);
            Assert.IsTrue(versions.Select(x => x.Preview).Any(x => x.Contains(String.Format(@"src=""/brainhoney/Resource/{0}/[~]/folder/image.jpg""", course.QuestionRepositoryCourseId))));

        }


        [TestMethod]
        public void GetVersionHistory_EmptyQuestionId_EmptyList()
        {


            var course = GetTestCourse();
            var versions = questionCommands.GetVersionHistory(course.QuestionRepositoryCourseId, string.Empty);
            Assert.IsFalse(versions.Any());
           

        }


        [TestMethod]
        public void RemoveFromTitle_AnyQuetionIds_True()
        {

            var agilixQuestions = new List<Question>()
                                       {
                                           new Question()
                                           {
                                               Id = "f13f2cd1-2ddf-430c-85c9-2577a5f009f4",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-1234", XElement.Parse(productCourseSection)},
                                                                      {"product-course-id-122434", XElement.Parse(productCourseSection)}
                                                                  },
                                                InteractionType = "2"
                                               

                                           }
                                       };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(questions => questions.Questions = agilixQuestions));
            var course = GetTestCourse();


            Assert.IsTrue(questionCommands.RemoveFromTitle(new[] { "3232", "223" }, course.QuestionRepositoryCourseId, "1234"));
            Assert.IsTrue(agilixQuestions.First().MetadataElements.Count == 1);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveFromTitle_Null_False()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));
            Assert.IsFalse(questionCommands.RemoveFromTitle(null, null, null));

        }


        [TestMethod]
        public void SetSequence_Question_ProperplySettedQuestionSeq()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            var course = GetTestCourse();
            var question = new Models.Question()
            {
                Id = "1",
                Version = 3,
                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = course.ProductCourseId,
                                                                Bank = "Test Bank",
                                                                Sequence = "0"
                                                            }
                                                        },
                InteractionType = "2"

            };


            questionCommands.SetSequence(course.ProductCourseId, question);
            Assert.IsTrue(question.ProductCourseSections.First().Sequence == "19");
   
        }

        [TestMethod]
        public void UpdateQuestion_QuestionToSave_QuestionWithModifBySetted()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));
            var course = GetTestCourse();
            var question = new Models.Question()
            {
                Id = "1",
                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = course.ProductCourseId
                                                            }
                                                        },
                InteractionType = "2"

            };

           var result = questionCommands.UpdateQuestion(question);
           Assert.IsTrue(result.Id == question.Id);
           Assert.IsTrue(result.ModifiedBy == context.CurrentUser.Id);

        }


        [TestMethod]
        public void UpdateQuestions_QuestionsToSave_UpdatedQuestions()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));
            var course = GetTestCourse();
            var questions = new List<Models.Question>
                           {
                               new Models.Question()
                               {
                                   Id = "1",
                                   InteractionType = "2"

                               },
                               new Models.Question()
                               {
                                   Id = "1",
                                  InteractionType = "2"
                               }
                           };

            Assert.IsTrue(questionCommands.UpdateQuestions(questions, course.QuestionRepositoryCourseId, course.ProductCourseId));

            foreach (var question in questions)
            {
                Assert.IsTrue(question.Id == "1");
            }

        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateQuestions_Null_False()
        {
           Assert.IsFalse(questionCommands.UpdateQuestions(null,null)); 
           
        }

        [TestMethod]
        public void GetAgilixQuestion_QuestionId_Question()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestion));
            var course = GetTestCourse();
            var question = questionCommands.GetAgilixQuestion(course.QuestionRepositoryCourseId, "df");
            Assert.IsTrue(question.Id == "df");
            Assert.IsTrue(question.InteractionType == "2");
        }

        [TestMethod]
        public void GetAgilixQuestion_QuestionIdAndVersion_SpecificVersion()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetTwoAgilixQuestions));
            var course = GetTestCourse();
            var question = questionCommands.GetAgilixQuestion(course.QuestionRepositoryCourseId, "df", "3");
            Assert.IsTrue(question.QuestionVersion == "3");
        }



        [TestMethod]
        public void GetQuestion_QuestionId_Question()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestion));
            var question = questionCommands.GetQuestion(GetTestCourse().QuestionRepositoryCourseId, "df");
            Assert.IsTrue(question.Id == "df");

        }

        [TestMethod]
        public void GetQuestions_QuestionIds_Questions()
        {
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(ExecuteAsAdminGetAgilixQuestion));
            var course = GetTestCourse();
            var questions = questionCommands.GetQuestions(course.QuestionRepositoryCourseId, new []{"12", "35"});
            Assert.IsTrue(questions.Any(x=> x.Id == "12"));
            Assert.IsTrue(questions.Any(x => x.Id == "35"));
        }


        [TestMethod]
        public void GetQuizIdForQuestion_QuestionIdWithQuizId_QuizId()
        {
            var quizId = "4331";
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(items =>
                                                                                  {
                                                                                      Assert.IsTrue(items.SearchParameters.Query == "/Questions/question@id='123'");
                                                                                      items.Items =
                                                                                          new PagedList<Item>(
                                                                                              new List<Item>()
                                                                                              {
                                                                                                  new Item()
                                                                                                  {
                                                                                                      Id = quizId
                                                                                                  }
                                                                                              }, 0, 1);

                                                                                  }));
            Assert.IsTrue(quizId == questionCommands.GetQuizIdForQuestion("123", "345"));
        }

        [TestMethod]
        public void GetQuizIdForQuestion_QuestionIdWithoutQuizId_EmptyString()
        {

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetItems>(items =>
                    {
                        Assert.IsTrue(items.SearchParameters.Query == "/Questions/question@id='123'");
                        items.Items = new PagedList<Item>(new List<Item>(), 0, 1);
                    }));
           Assert.IsTrue(string.IsNullOrEmpty(questionCommands.GetQuizIdForQuestion("123", "345")));
        }

        [TestMethod]
        public void UpdateSharedQuestionField_QuestionWithStaticField_True()
        {

            var course = GetTestCourse();
            var question = new Question()
            {
                MetadataElements = new Dictionary<string, XElement>()
                                                  {
                                                      {"product-course-id-12/bank",  XElement.Parse(@"<arr name=""product-course-id-12/bank"">
                                                                                                  <bank>Test Bank</bank>
                                                                                                   <productcourseid>12</productcourseid>
                                                                                                    </arr>")},
                                                      {"product-course-defaults", XElement.Parse(@"<arr name=""product-course-defaults"">
                                                                                                  <bank>Test Bank</bank>
                                                                                                    </arr>")}
                                                  },
                InteractionType = "2",
                QuestionVersion = "0",
                Id = "432",
                EntityId = course.QuestionRepositoryCourseId
            };

            


            productCourseOperation.GetProductCourse(Arg.Any<string>()).Returns(course);
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
                                                                                      {
                                                                                          cmd.Questions = new List<Question>
                                                                                                          {
                                                                                                              question
                                                                                                          };
                                                                                      }));


            Assert.IsTrue(questionCommands.UpdateSharedQuestionField(course.QuestionRepositoryCourseId, "questionId", "bank", new List<string> { "Test" }));
            var changedQuestion = Mapper.Map<Models.Question>(question);
            Assert.IsTrue(changedQuestion.DefaultSection.Bank == "Test");
            Assert.IsTrue(changedQuestion.ProductCourseSections.First().Bank == "Test");
          
      
        }

        [TestMethod]
        public void UpdateSharedQuestionField_QuestionWithStaticTitle_True()
        {

            var course = GetTestCourse();
            var question = new Question()
            {
                MetadataElements = new Dictionary<string, XElement>()
                                                  {
                                                      {"product-course-id-12",  XElement.Parse(productCourseSection)},
                                                      {"product-course-defaults", XElement.Parse(@"<arr name=""product-course-defaults"">
                                                                                                  <title>title</title>
                                                                                                    </arr>")}
                                                  },
                InteractionType = "2",
                QuestionVersion = "0",
                Id = "432",
                EntityId = course.QuestionRepositoryCourseId
            };




            productCourseOperation.GetProductCourse(Arg.Any<string>()).Returns(course);
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>
                                                                                                          {
                                                                                                              question
                                                                                                          };
            }));


            Assert.IsTrue(questionCommands.UpdateSharedQuestionField(course.QuestionRepositoryCourseId, "questionId", "title", new List<string> { "Test" }));
            var changedQuestion = Mapper.Map<Models.Question>(question);
            Assert.IsTrue(changedQuestion.DefaultSection.Title == "Test");
            Assert.IsTrue(changedQuestion.ProductCourseSections.First().Title == "Test");


        }

        [TestMethod]
        public void UpdateSharedQuestionField_QuestionWithStaticChapter_True()
        {

            var course = GetTestCourse();
            var question = new Question()
            {
                MetadataElements = new Dictionary<string, XElement>()
                                                  {
                                                      {"product-course-id-12/chapter",  XElement.Parse(productCourseSection)},
                                                      {"product-course-defaults", XElement.Parse(@"<arr name=""product-course-defaults"">
                                                                                                  <chapter>Test Chapter</chapter>
                                                                                                    </arr>")}
                                                  },
                InteractionType = "2",
                QuestionVersion = "0",
                Id = "432",
                EntityId = course.QuestionRepositoryCourseId
            };




            productCourseOperation.GetProductCourse(Arg.Any<string>()).Returns(course);
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>
                                                                                                          {
                                                                                                              question
                                                                                                          };
            }));


            Assert.IsTrue(questionCommands.UpdateSharedQuestionField(course.QuestionRepositoryCourseId, "questionId", "chapter", new List<string> { "Test" }));
            var changedQuestion = Mapper.Map<Models.Question>(question);
            Assert.IsTrue(changedQuestion.DefaultSection.Chapter == "Test");
            Assert.IsTrue(changedQuestion.ProductCourseSections.First().Chapter == "Test");


        }


        [TestMethod]
        public void UpdateSharedQuestionField_QuestionWithDynamicField_True()
        {
            var course = GetTestCourse();
            var question = new Question()
            {
                MetadataElements = new Dictionary<string, XElement>()
                                                  {
                                                      {"product-course-id-12/bank",  XElement.Parse(@"<arr name=""product-course-id-12/bank"">
                                                                                                  <field>field 1</field>
                                                                                                   <productcourseid>12</productcourseid>
                                                                                                    </arr>")},
                                                      {"product-course-defaults", XElement.Parse(@"<arr name=""product-course-defaults"">
                                                                                                  <field>field 1</field>
                                                                                                    </arr>")},
                                                    {"product-course-id-123/bank", XElement.Parse(@"<arr name=""product-course-id-123/bank"">
                                                    
                                                    </arr>")}
                                                  },
                InteractionType = "2",
                QuestionVersion = "0",
                Id = "432",
                EntityId = course.QuestionRepositoryCourseId
            };

  


            productCourseOperation.GetProductCourse(Arg.Any<string>()).Returns(course);
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>
                                                                                                          {
                                                                                                              question
                                                                                                          };
            }));


            Assert.IsTrue(questionCommands.UpdateSharedQuestionField(course.QuestionRepositoryCourseId, "questionId", "field", new List<string> { "value" }));
            var changedQuestion = Mapper.Map<Models.Question>(question);
            Assert.IsTrue(changedQuestion.ProductCourseSections.First().DynamicValues.First().Value.First() == "value");
           


        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UpdateSharedQuestionField_AnyIncorrectParametrs_False()
        {

            
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>
                                                                                                          {
                                                                                                              new Question()
                                                                                                          };
            }));
            Assert.IsFalse(questionCommands.UpdateSharedQuestionField("12", "questionId", "field", new List<string> { "value" }));
        }


        [TestMethod]
        public void BulkUpdateQuestionField_UnknownFieldBulkUpdate_EmptyResult()
        {
            var result = questionCommands.BulkUpdateQuestionField(null, null, null, "field", null, null);
            Assert.IsFalse(result.IsSuccess);
        }


        [TestMethod]
        public void BulkUpdateQuestionField_QuestionIdsForBulkStatusUpdateWithOneDraftAndNotAllowed_SuccessResult()
        {
            var course = GetTestCourse();
            var questions = new List<Question>
                            {
                                new Question()
                                {
                                    QuestionStatus = "2",
                                    InteractionType = "2",
                                      MetadataElements = new Dictionary<string, XElement>()
                                },

                                  new Question()
                                {
                                   InteractionType = "2",
                                   QuestionStatus = "1",
                                     MetadataElements = new Dictionary<string, XElement>()
                                },

                                  new Question()
                                {
                                    InteractionType = "2" ,
                                    QuestionStatus = "2",
                                    MetadataElements = new Dictionary<string, XElement>
                                                       {
                                                           {MetadataFieldNames.DraftFrom, XElement.Parse(string.Format("<{0}>sdfdf</{0}>", MetadataFieldNames.DraftFrom))}
                                                       }
                                }


                            };
               context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
                                                                                         {
                                                                                             cmd.Questions = questions;
                                                                                         }));
            var capabilities = new List<Capability>
                               {
                                  Capability.ChangeStatusFromDeletedToAvailable,
                                  Capability.ChangeDraftStatus
                               };

            var result = questionCommands.BulkUpdateQuestionField(course.ProductCourseId,
                course.QuestionRepositoryCourseId, new[] {"1", "2", "3"}, MetadataFieldNames.QuestionStatus, "0",
                capabilities);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.DraftSkipped == 1);
            Assert.IsTrue(result.PermissionStatusSkipped == 1);
            Assert.IsTrue(questions.Select(x => x.QuestionStatus).Contains("0"));
        }


        [TestMethod]
        public void BulkUpdateQuestionField_QuestionIdsForBulkStatusUpdateWithNoAccess_AllQuestionsSkipped()
        {
            var course = GetTestCourse();
            var questions = new List<Question>
                            {
                                new Question()
                                {
                                    QuestionStatus = "0",
                                    InteractionType = "2",
                                      MetadataElements = new Dictionary<string, XElement>()
                                },

                                  new Question()
                                {
                                   InteractionType = "2",
                                   QuestionStatus = "1",
                                     MetadataElements = new Dictionary<string, XElement>()
                                },

                                  new Question()
                                {
                                    InteractionType = "2" ,
                                    QuestionStatus = "2",
                                    MetadataElements = new Dictionary<string, XElement>()
                                },

                                   new Question()
                                {
                                    InteractionType = "2" ,
                                    QuestionStatus = "2",
                                    MetadataElements = new Dictionary<string, XElement>
                                                       {
                                                           {MetadataFieldNames.DraftFrom, XElement.Parse(string.Format("<{0}>sdfdf</{0}>", MetadataFieldNames.DraftFrom))}
                                                       }
                                }


                            };
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = questions;
            }));
            var capabilities = new List<Capability>
                               {
                               };

            var result = questionCommands.BulkUpdateQuestionField(course.ProductCourseId,
                course.QuestionRepositoryCourseId, new[] { "1", "2", "3" }, MetadataFieldNames.QuestionStatus, "0",
                capabilities);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.PermissionStatusSkipped == 3);

             result = questionCommands.BulkUpdateQuestionField(course.ProductCourseId,
           course.QuestionRepositoryCourseId, new[] { "1", "2", "3" }, MetadataFieldNames.QuestionStatus, "1",
           capabilities);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.PermissionStatusSkipped == 3);

            result = questionCommands.BulkUpdateQuestionField(course.ProductCourseId,
           course.QuestionRepositoryCourseId, new[] { "1", "2", "3" }, MetadataFieldNames.QuestionStatus, "2",
           capabilities);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.PermissionStatusSkipped == 3);
        }


        [TestMethod]
        public void BulkUpdateQuestionField_QuestionIdsForBulkChapterUpdateWithOneDraft_SuccessResult()
        {
            var course = GetTestCourse();
            var questionToCheckChanges = new Question()
                                         {
                                             InteractionType = "2",
                                             QuestionStatus = "0",
                                             MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-12",
                                                                        XElement.Parse(productCourseSection)
                                                                    }
                                                                },
                                             Id = "1"
                                         };
            var questions = new List<Question>
                            {
                               questionToCheckChanges,

                                  new Question()
                                {
                                    InteractionType = "2",
                                    QuestionStatus = "0",
                                    MetadataElements = new Dictionary<string, XElement>()
                                                       {
                                                           {"product-course-id-12", XElement.Parse(productCourseSection)}
                                                       },
                                                         Id = "2"

                                },
                                   new Question()
                                {
                                   InteractionType = "2",
                                   QuestionStatus = "2",
                                    MetadataElements = new Dictionary<string, XElement>()
                                                       {
                                                           {"product-course-id-12", XElement.Parse(productCourseSection)}
                                                       },
                                                         Id = "3"
                                }

                            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = questions;
            }));
            var capabilities = new List<Capability>
                               {
                                  Capability.EditAvailableQuestion,
                               };

            var result = questionCommands.BulkUpdateQuestionField(course.ProductCourseId,
                course.QuestionRepositoryCourseId, new[] { "1", "2", "3" }, MetadataFieldNames.Chapter, "bulkupdate",
                capabilities);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(result.PermissionSkipped == 1);
            var chsdf = Mapper.Map<IEnumerable<Models.Question>>(questions);
            Assert.IsTrue(Mapper.Map<Models.Question>(questionToCheckChanges).ProductCourseSections.First().Chapter == "bulkupdate");
        }






        [TestMethod]

        public void UpdateQuestionField_SequenceToUpdate_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.EditDeletedQuestion
                               };

            var question = new Question()
            {
                InteractionType = "2",
                QuestionStatus = "0",
                MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-12",
                                                                        XElement.Parse(productCourseSection)
                                                                    }
                                                                },
                Id = "1"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                    question
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", MetadataFieldNames.Sequence, "1", capabilities));
            Assert.IsTrue(Mapper.Map<Models.Question>(question).ProductCourseSections.First().Sequence == "9.17");
        }
  [TestMethod]

        public void UpdateQuestionField_SequenceToUpdateWithouBank_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.EditDeletedQuestion
                               };

            var question = new Question()
            {
                InteractionType = "2",
                QuestionStatus = "0",
                MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-12",
                                                                        XElement.Parse(productCourseSection)
                                                                    }
                                                                },
                Id = "1"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                   // question
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", MetadataFieldNames.Sequence, "1", capabilities));
          
        }


        [TestMethod]

        public void UpdateQuestionField_StatusToUpdate_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.ChangeStatusFromAvailableToDeleted
                               };

            var question = new Question()
            {
                InteractionType = "2",
                QuestionStatus = "0",
                MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-12",
                                                                        XElement.Parse(productCourseSection)
                                                                    }
                                                                },
                Id = "1"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                    question
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", MetadataFieldNames.QuestionStatus, "2", capabilities));
            Assert.IsTrue(Mapper.Map<Models.Question>(question).Status == "2");
        } [TestMethod]

        public void UpdateQuestionField_StatusToUpdateForNoQuestions_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.ChangeStatusFromAvailableToDeleted
                               };

           

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                   
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", MetadataFieldNames.QuestionStatus, "2", capabilities));
        }


        [TestMethod]

        public void UpdateQuestionField_ChapterToUpdate_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.EditAvailableQuestion
                               };

            var question = new Question()
            {
                InteractionType = "2",
                QuestionStatus = "0",
                MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-12",
                                                                        XElement.Parse(productCourseSection)
                                                                    }
                                                                },
                Id = "1"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                    question
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", MetadataFieldNames.Chapter, "ch", capabilities));
            Assert.IsTrue(Mapper.Map<Models.Question>(question).ProductCourseSections.First().Chapter== "ch");
        }



        [TestMethod]

        public void UpdateQuestionField_TitleToUpdate_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.EditDeletedQuestion
                               };

            var question = new Question()
            {
                InteractionType = "2",
                QuestionStatus = "2",
                MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-12",
                                                                        XElement.Parse(productCourseSection)
                                                                    }
                                                                },
                Id = "1"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                    question
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField(course.ProductCourseId, course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", MetadataFieldNames.DlapTitle, "1", capabilities));
            Assert.IsTrue(Mapper.Map<Models.Question>(question).ProductCourseSections.First().Title == "1");
        }

        [TestMethod]

        public void UpdateQuestionField_AnyFieldToUpdateExistingInSection_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.EditDeletedQuestion
                               };

            var question = new Question()
            {
                InteractionType = "2",
                QuestionStatus = "2",
                MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-12",
                                                                        XElement.Parse(productCourseSection)
                                                                    }
                                                                },
                Id = "1"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                    question
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField("12", course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", "field", "1", capabilities));
            Assert.IsTrue(Mapper.Map<Models.Question>(question).ProductCourseSections.First().DynamicValues.First().Value.First() == "1");
        }



        [TestMethod]

        public void UpdateQuestionField_AnyFieldToUpdateNotExistingInSection_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.EditDeletedQuestion
                               };

            var question = new Question()
            {
                InteractionType = "2",
                QuestionStatus = "2",
                MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-123",
                                                                        XElement.Parse(productCourseSection123)
                                                                    }
                                                                },
                Id = "1"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                    question
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField("123", course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", "field", "1", capabilities));
            Assert.IsTrue(Mapper.Map<Models.Question>(question).ProductCourseSections.First().DynamicValues.First().Value.First() == "1");
        }

        [TestMethod]

        public void UpdateQuestionField_FlagToUpdate_SuccessUpdate()
        {
            var course = GetTestCourse();

            var capabilities = new List<Capability>
                               {
                                  Capability.EditDeletedQuestion
                               };

            var question = new Question()
            {
                InteractionType = "2",
                QuestionStatus = "2",
                MetadataElements = new Dictionary<string, XElement>()
                                                                {
                                                                    {
                                                                        "product-course-id-12",
                                                                        XElement.Parse(productCourseSection)
                                                                    }
                                                                },
                Id = "1"
            };

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(ExecuteAsAdminFillTwoQuestions));
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = new List<Question>()
                                {
                                    question
                                };
            }));

            Assert.IsTrue(questionCommands.UpdateQuestionField("12", course.QuestionRepositoryCourseId, "f13f2cd1-2ddf-430c-85c9-2577a5f009f4", MetadataFieldNames.Flag, "flagged", capabilities));
            Assert.IsTrue(Mapper.Map<Models.Question>(question).ProductCourseSections.First().Flag == "flagged");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void UpdateQuestionField_IncorrectParams_False()
        {
            Assert.IsFalse(questionCommands.UpdateQuestionField(null, null, "", MetadataFieldNames.Sequence, "df", null));
        }


        [TestMethod]
        public void GetQuestionCountByChapters_AnyCorrectParams_TwoQuestionForOneChapter()
        {
            var course = GetTestCourse();
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(search =>
                                                                                {
                                                                                    Assert.IsTrue( search.SearchParameters.Query == @"(dlap_class:question) AND (product-course-id-12/productcourseid_dlap_e:""12"") AND (product-course-id-12/chapter_dlap_e:""Test Chapter"")");
                                                                                    ExecuteAsAdminFillTwoQuestions(search);
                                                                                }));
            var result = questionCommands.GetQuestionCountByChapters(course.QuestionRepositoryCourseId, course.ProductCourseId, new List<string>() {"Test Chapter"});
            Assert.IsTrue(result.First().Value == 2);
        }

         [TestMethod]
        public void GetComparedQuestionList_AnyCorrectParams_ComparedList()
        {

            var course = GetTestCourse();
            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<Search>(search =>
            {
                Assert.IsTrue(search.SearchParameters.Query == @"(dlap_class:question) AND (product-course-id-12/productcourseid_dlap_e:""12"" OR product-course-id-123/productcourseid_dlap_e:""123"")");
               search.ParseResponse(new DlapResponse(XDocument.Parse(questionsForComparedList)));
            }));

            context.SessionManager.CurrentSession.ExecuteAsAdmin(Arg.Do<GetQuestions>(cmd =>
            {
                cmd.Questions = GetQuestionsForComparedQuestion();
            }));
          

           var result =  questionCommands.GetComparedQuestionList(course.QuestionRepositoryCourseId, "12", "123", 0, 5);
           Assert.IsTrue(result.TotalItems == 4);
             foreach (var comparedQuestion in result.CollectionPage)
             {
                // Assert.IsFalse(comparedQuestion.CompareLocationResult == CompareLocationType.OnlySecondCourse);
             }

        }

        #region private methods

        private void ExecuteAsAdminReturnTaskList(GetTaskList taskCmd)
        {
            taskCmd.ParseResponse(new DlapResponse(XDocument.Parse(taskResponse)));
        }

        private void ExecuteAsAdminFillTwoQuestions(Search search)
        {
            search.SearchResults = XDocument.Parse(twoQuestionsSOLRSearchResultResponse);
        }

        private void ExecuteAsAdminFillThreeQuestionsWithDrafts(Search search)
        {
            search.SearchResults = XDocument.Parse(threeQuestionsWithDraftsSOLRSearchResultResponse);
        }
        private void ExecuteAsAdminFillTwoQuestionsWithCorruptedSequense(Search search)
        {
            search.SearchResults = XDocument.Parse(twoQuestionsWithCorruptedSeqSOLRSearchResultResponse);
        }

        private void ExecuteAsAdminFillNoQuestions(Search search)
        {
            search.SearchResults = new XDocument();

        }

        private void ExecuteAsAdminGetAgilixQuestion(GetQuestions questionSearch)
        {
            var questions = new List<Question>();
            foreach (var quesionId in questionSearch.SearchParameters.QuestionIds)
            {
                questions.Add(new Question()
                              {
                                  Id = quesionId,
                                  MetadataElements = new Dictionary<string, XElement> { { "product-course-id-12", XElement.Parse(productCourseSection) } },
                                  InteractionType = "2"
                              });
            }

            questionSearch.Questions = questions;
        }

        private void AddNessesaryProductCourseFilter(  List<FilterFieldDescriptor> filter)
        {
            filter.Add(new FilterFieldDescriptor()
            {
                Field = MetadataFieldNames.ProductCourse,
                Values = new List<string>() { productCourseId }
            });
            
        }

        private void ExecuteAsAdminGetTwoAgilixQuestions(GetQuestions questionSearch)
        {
            questionSearch.Questions = new List<Question>()
                                       {
                                           new Question()
                                           {
                                               Id = "f13f2cd1-2ddf-430c-85c9-2577a5f009f4",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-12", XElement.Parse(productCourseSection)}
                                                                  },
                                                                       ModifiedDate = DateTime.Now
                                               
        
                                           },
                                           new Question(){
                                               Id = "4684f693-7997-4dc2-8496-56eb645e47ac",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-12", XElement.Parse(productCourseSection)},
                                                                      {"draftfrom", XElement.Parse("<draftfrom>f13f2cd1-2ddf-430c-85c9-2577a5f009f4</draftfrom>")}
                                                                  },
                                                                       ModifiedDate = DateTime.Now,
                                               InteractionType = "choice",
                                               Choices = new List<QuestionChoice>()
                                                         {
                                                             new QuestionChoice()
                                                             {
                                                                 Id = "1",
                                                                 Text = @"src=""[~]/folder/image.jpg"""
                                                             }
                                                         },
                                                QuestionVersion = "3"
                                           }
                                       };
        }
        private void ExecuteAsAdminGetThreeAgilixQuestionsWithDrafts(GetQuestions questionSearch)
        {
            questionSearch.Questions = new List<Question>()
                                       {
                                           new Question()
                                           {
                                               Id = "1",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-12", XElement.Parse(productCourseSection)}
                                                                      
                                                                  },
                                                                  ModifiedDate = DateTime.Now
                                               

                                           },
                                           new Question()
                                           {
                                               Id = "2",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-12", XElement.Parse(productCourseSection)},
                                                                        {"draftfrom", XElement.Parse(@" <draftfrom>1</draftfrom>")}
                                                                  },
                                                 ModifiedDate = DateTime.Now

                                           },
                                           new Question()
                                           {
                                               Id = "3",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-12", XElement.Parse(productCourseSection)},
                                                                       {"draftfrom", XElement.Parse(@" <draftfrom>2</draftfrom>")}
                                                                  },
                                                 ModifiedDate = DateTime.Now

                                           },
                                          
                                       };
        }

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
                                           Name = "chapter",
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
                                           Name = "title",
                                           CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
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

        #endregion

        #region test xml's

        private string facetedResult = @"<results>
  <result name=""response"" numFound=""6934"" start=""0"" maxScore=""9.422424"" time=""36"">
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000044"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000044</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A400005C"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A400005C</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000068"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000068</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000074"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000074</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000080"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000080</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A400008C"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A400008C</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000098"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000098</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A40000A0"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A40000A0</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A40000AC"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A40000AC</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A40000B8"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A40000B8</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A40000C4"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A40000C4</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A40000D0"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A40000D0</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A40000DC"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A40000DC</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A40000E8"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A40000E8</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A40000F4"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A40000F4</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000104"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000104</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000110"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000110</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A400011C"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A400011C</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000128"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000128</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000134"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000134</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000140"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000140</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A400014C"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A400014C</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000158"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000158</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000164"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000164</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
    <doc entityid=""6712"" class=""question"" questionid=""2CB4C3C29D8EEDCFC6316EC5A4000170"">
      <str name=""dlap_id"">6712|Q|2CB4C3C29D8EEDCFC6316EC5A4000170</str>
      <str name=""dlap_class"">question</str>
      <arr name=""score"">
        <float name=""score"">9.422424</float>
      </arr>
    </doc>
  </result>
  <lst name=""facet_counts"">
    <lst name=""facet_fields"">
      <lst name=""product-course-id-12/chapter_dlap_e"">
        <int name=""Chapter 6"">434</int>
        <int name=""Chapter 11"">421</int>
        <int name=""Chapter 12"">417</int>
        <int name=""Chapter 3"">412</int>
        <int name=""Chapter 7"">384</int>
        <int name=""Chapter 10"">378</int>
        <int name=""Chapter 2"">376</int>
        <int name=""Chapter 8"">369</int>
        <int name=""Chapter 13"">356</int>
        <int name=""Chapter 19"">336</int>
        <int name=""Chapter 15"">327</int>
        <int name=""Chapter 16"">325</int>
        <int name=""Chapter 17"">324</int>
        <int name=""Chapter 5"">312</int>
        <int name=""Chapter 14"">311</int>
        <int name=""Chapter 4"">294</int>
        <int name=""Chapter 9"">291</int>
        <int name=""Chapter 20"">287</int>
        <int name=""Chapter 10A"">244</int>
        <int name=""Chapter 18"">217</int>
        <int name=""Chapter 2A"">75</int>
        <int name=""Chapter 19A"">41</int>
        <int name=""Chapter 1"">3</int>
        <int name=""First Chapter"">0</int>
      </lst>
    </lst>
  </lst>
</results>";

        private string twoQuestionsSOLRSearchResultResponse = @" <results>
                                                              <result name=""response"" numFound=""2"" start=""0"" maxScore=""16.067871"" time=""8"">
                                                                <doc entityid=""39768"" class=""question"" questionid=""f13f2cd1-2ddf-430c-85c9-2577a5f009f4"">
                                                                  <str name=""dlap_id"">39768|Q|f13f2cd1-2ddf-430c-85c9-2577a5f009f4</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                   <str name=""dlap_q_type"">choice</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>
                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>

                                                                   <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.38</float>
                                                                  </arr>
                                                                </doc>
                                                                <doc entityid=""39768"" class=""question"" questionid=""4684f693-7997-4dc2-8496-56eb645e47ac"">
                                                                  <str name=""dlap_id"">39768|Q|4684f693-7997-4dc2-8496-56eb645e47ac</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                  <str name=""dlap_q_type"">text</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>

                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                  <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.34</float>
                                                                  </arr>
                                                                </doc>
                                                              </result>
                                                            </results>";
        private string threeQuestionsWithDraftsSOLRSearchResultResponse = @" <results>
                                                              <result name=""response"" numFound=""2"" start=""0"" maxScore=""16.067871"" time=""8"">
                                                                <doc entityid=""39768"" class=""question"" questionid=""1"">
                                                                  <str name=""dlap_id"">39768|Q|1</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                   <str name=""dlap_q_type"">choice</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>
                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>

                                                                   <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.38</float>
                                                                  </arr>
                                                                </doc>
                                                                <doc entityid=""39768"" class=""question"" questionid=""2"">
                                                                  <str name=""dlap_id"">39768|Q|2</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                  <str name=""dlap_q_type"">choice</str>
                                                              

                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr> 
                                                                    <arr name=""draftfrom"">
                                                                    <str>1</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>

                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                  <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.34</float>
                                                                  </arr>
                                                                </doc>

                                                            <doc entityid=""39768"" class=""question"" questionid=""3"">
                                                                  <str name=""dlap_id"">39768|Q|3</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                  <str name=""dlap_q_type"">choice</str>
                                                              

                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr> 
                                                                    <arr name=""draftfrom"">
                                                                    <str>2</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>

                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                  <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.34</float>
                                                                  </arr>
                                                                </doc>
                                                             
                                                              </result>
                                                            </results>";
        private string twoQuestionsWithCorruptedSeqSOLRSearchResultResponse = @" <results>
                                                              <result name=""response"" numFound=""2"" start=""0"" maxScore=""16.067871"" time=""8"">
                                                                <doc entityid=""39768"" class=""question"" questionid=""f13f2cd1-2ddf-430c-85c9-2577a5f009f4"">
                                                                  <str name=""dlap_id"">39768|Q|f13f2cd1-2ddf-430c-85c9-2577a5f009f4</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                   <str name=""dlap_q_type"">choice</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>
                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>

                                                                   <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">dfs</float>
                                                                  </arr>
                                                                </doc>
                                                                <doc entityid=""39768"" class=""question"" questionid=""4684f693-7997-4dc2-8496-56eb645e47ac"">
                                                                  <str name=""dlap_id"">39768|Q|4684f693-7997-4dc2-8496-56eb645e47ac</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                     <str name=""dlap_q_type"">choice</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>

                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                  <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">sdfd</float>
                                                                  </arr>
                                                                </doc>
                                                              </result>
                                                            </results>";

        private string questionsForComparedList = @"<results>
                                                              <result name=""response"" numFound=""2"" start=""0"" maxScore=""16.067871"" time=""8"">
                                                                <doc entityid=""39768"" class=""question"" questionid=""f13f2cd1-2ddf-430c-85c9-2577a5f009f4"">
                                                                  <str name=""dlap_id"">39768|Q|f13f2cd1-2ddf-430c-85c9-2577a5f009f4</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/productcourseid"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>
                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>

                                                                   <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.38</float>
                                                                  </arr>
                                                                 <arr name=""product-course-id-12/field"">
                                                                    <str>Test Bank</str>
                                                                    </arr>

                                                                    <arr name=""product-course-id-123/chapter"">
                                                                    <str>Test Chapter 123</str>
                                                                    </arr>
                                                                
                                                                   <arr name=""product-course-id-123/sequence"">
                                                                    <float name=""score"">18.38</float>
                                                                  </arr>
                                                                </doc>
                                                                <doc entityid=""39768"" class=""question"" questionid=""df"">
                                                                  <str name=""dlap_id"">39768|Q|4684f693-7997-4dc2-8496-56eb645e47ac</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>

                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                  <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.34</float>
                                                                  </arr>

                                                                <arr name=""product-course-id-123/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>
                                                                </doc>

                                                                 <doc entityid=""39768"" class=""question"" questionid=""af"">
                                                                  <str name=""dlap_id"">39768|Q|asds32</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>

                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                  <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.34</float>
                                                                  </arr>
                                                                </doc>



                                                            <doc entityid=""39768"" class=""question"" questionid=""4684f693-7997-4dc2-8496-56eb645e47ac"">
                                                                  <str name=""dlap_id"">39768|Q|cvxvfdvdf</str>
                                                                  <str name=""dlap_class"">question</str>
                                                                    <arr name=""product-course-id-12/bank"">
                                                                    <str>Test Bank</str>
                                                                    </arr>
                                                                    <arr name=""product-course-id-12/chapter"">
                                                                    <str>Test Chapter</str>
                                                                    </arr>

                                                                  <arr name=""score"">
                                                                    <float name=""score"">16.067871</float>
                                                                  </arr>
                                                                  <arr name=""product-course-id-12/sequence"">
                                                                    <float name=""score"">18.34</float>
                                                                  </arr>
                                                                </doc>
                                                              </result>
                                                            </results>";

        private const string productCourseSection = @"<product-course-id-12>
<bank>Test Bank</bank>
<chapter>Test Chapter</chapter>
<productcourseid>12</productcourseid>
<sequence>3</sequence>
<field>1</field>
<title>title</title>
</product-course-id-12>";

        private const string productCourseSectionWithoutBank = @"<product-course-id-12>
<chapter>Test Chapter</chapter>
<productcourseid>12</productcourseid>
<sequence>3</sequence>
<field>1</field>
<title>title</title>
</product-course-id-12>";

        private const string productCourseSection123 = @"<product-course-id-123>
<bank>Test Bank</bank>
<chapter>Test Chapter</chapter>
<productcourseid>123</productcourseid>
<sequence>3</sequence>
</product-course-id-123>";

        private const string taskResponse = @"<responses>
                                               <task running=""false"">
                                               
                                                </task> 
                                            </responses>";

        private IEnumerable<Question> GetQuestionsForComparedQuestion()
        {
            return new  List<Question>()
                                       {
                                           new Question()
                                           {
                                               Id = "f13f2cd1-2ddf-430c-85c9-2577a5f009f4",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-12", XElement.Parse(productCourseSection)},
                                                                      {"product-course-id-123", XElement.Parse(productCourseSection123)}
                                                                  },
                                               

                                           },
                                           new Question(){
                                               Id = "4684f693-7997-4dc2-8496-56eb645e47ac",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-12", XElement.Parse(productCourseSection)},
                                                                      {"product-course-id-123", XElement.Parse(productCourseSection123)}
                                                                  },
                                               InteractionType = "choice",
                                               Choices = new List<QuestionChoice>()
                                                         {
                                                             new QuestionChoice()
                                                             {
                                                                 Id = "1",
                                                                 Text = @"src=""[~]/folder/image.jpg"""
                                                             }
                                                         },
                                                QuestionVersion = "3"
                                           },
                                              new Question(){
                                               Id = "df",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-12", XElement.Parse(productCourseSection)}
                                                                  },
                                               InteractionType = "choice",
                                               Choices = new List<QuestionChoice>()
                                                         {
                                                             new QuestionChoice()
                                                             {
                                                                 Id = "1",
                                                                 Text = @"src=""[~]/folder/image.jpg"""
                                                             }
                                                         },
                                                QuestionVersion = "3"
                                           },

                                             new Question(){
                                               Id = "df",
                                               MetadataElements = new Dictionary<string, XElement>
                                                                  {
                                                                      {"product-course-id-123", XElement.Parse(productCourseSection123)}
                                                                  },
                                               InteractionType = "choice",
                                               Choices = new List<QuestionChoice>()
                                                         {
                                                             new QuestionChoice()
                                                             {
                                                                 Id = "1",
                                                                 Text = @"src=""[~]/folder/image.jpg"""
                                                             }
                                                         },
                                                QuestionVersion = "3"
                                           }
                                       };
        } 
        #endregion
    }
}
