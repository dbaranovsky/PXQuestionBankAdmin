using System;
using System.Collections.Generic;
using System.Linq;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.QuestionParserModule;
using Macmillan.PXQBA.Business.QuestionParserModule.QTI;
using Macmillan.PXQBA.Business.Services.Automapper;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class ModelProfileServiceTest
    {

        private IQuestionCommands questionCommands;
        private INoteCommands noteCommands;
        private IProductCourseOperation productCourseOperation;
        private IUserOperation userOperation;
        private IParsedFileOperation parsedFileOperation;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;
       

        [TestInitialize]

        public void TestInitialize()
        {
            questionCommands = Substitute.For<IQuestionCommands>();
            noteCommands = Substitute.For<INoteCommands>();
            productCourseOperation = Substitute.For<IProductCourseOperation>();
            userOperation = Substitute.For<IUserOperation>();
            modelProfileService = new ModelProfileService(productCourseOperation, questionCommands, userOperation, noteCommands);
            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();
        }
        [TestMethod]
        public void GetProductCourseSections_ViewModelForQuestionWithProductCourses_ListOfMetadataSections()
        {
            var viewModel = new QuestionViewModel()
                            {
                                LocalSection = new QuestionMetadataSection()
                                               {
                                                   ProductCourseId = "12"
                                               },
                                EntityId = "123",
                                Id = "temp_question_3112"
                            };
            questionCommands.GetQuestion(viewModel.EntityId, viewModel.Id).Returns(new Question()
                                                                      {
                                                                          ProductCourseSections =
                                                                              new List<QuestionMetadataSection>()
                                                                              {
                                                                                  new QuestionMetadataSection()
                                                                                  {
                                                                                      ProductCourseId = "12"
                                                                                  },

                                                                                  new QuestionMetadataSection()
                                                                                  {
                                                                                      ProductCourseId = "34",
                                                                                      Title = "Test"
                                                                                  }
                                                                              }
                                                                    });
            var sections = modelProfileService.GetProductCourseSections(viewModel);
            Assert.IsTrue(sections.Count == 2);
            Assert.IsTrue(sections.Last().Title == "Test");

        }  
        
        [TestMethod]
        public void GetChaptersViewModel_CourseWithFacetedResult_Chapters()
        {
           
            var course = new Course()
            {
                ProductCourseId = "12",
                FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                   {
                                       new CourseMetadataFieldDescriptor()
                                       {
                                           Name = MetadataFieldNames.Chapter,
                                           CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                       {
                                                                           new CourseMetadataFieldValue()
                                                                           {
                                                                               Text = "Chapter 1",
                                                                           },
                                                                           
                                                                           new CourseMetadataFieldValue()
                                                                           {
                                                                               Text = "Chapter 2",
                                                                           }
                                                                       }
                                       }
                                   },
            QuestionRepositoryCourseId = "43"
                
               
            };


            questionCommands.GetFacetedResults(course.QuestionRepositoryCourseId, course.ProductCourseId,
                MetadataFieldNames.Chapter).Returns(new List<QuestionFacetedSearchResult>()
                                                    {
                                                        new QuestionFacetedSearchResult()
                                                        {
                                                            FacetedCount = 6,
                                                            FacetedFieldValue = "Chapter 1"
                                                        },
                                                        
                                                        new QuestionFacetedSearchResult()
                                                        {
                                                            FacetedCount = 1,
                                                            FacetedFieldValue = "Chapter 2"
                                                        }
                                                    });


        
            var chapters = modelProfileService.GetChaptersViewModel(course);

            Assert.IsTrue(chapters.Count(x => (x.Title == "Chapter 1" && x.QuestionsCount == 6) || (x.Title == "Chapter 2" && x.QuestionsCount == 1)) == 2);

        } 
        
        [TestMethod]
        public void GetSourceQuestionSharedFrom_EmptyIdDuplicateFrom_Null()
        {
            Assert.IsTrue(modelProfileService.GetSourceQuestionSharedFrom(null, null) == null);
        }

        [TestMethod]
        public void GetSourceQuestionSharedFrom_IdDuplicateFrom_SharedQuestionDuplicateFrom()
        {

            var course = new Course()
            {
                ProductCourseId = "12",
                QuestionRepositoryCourseId = "45"
            };
            
            var question = new Question()
            {
                DraftFrom = "131",
                InteractionType = "choice",
                Status = "1",
                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = "1",
                                                                Title = "Test1",
                                   
                                                            },  
                                                            
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = "2",
                                                                Title = "Test",
                                   
                                                            },

                                                              new QuestionMetadataSection()
                                                            {
                                                              
                                                            }

                                                           
                                                        },
                Id = "questionId"
            };

            questionCommands.GetQuestion(course.QuestionRepositoryCourseId, "33").Returns(question);

            productCourseOperation.GetCoursesByCourseIds(null)
                                  .ReturnsForAnyArgs(new List<Course>
                                                         {
                                                             new Course()
                                                             {
                                                                 Title = "Test1"
                                                             },
                                                             new Course()
                                                             {
                                                                 Title = "Test"
                                                             },
                                                       }
                                                    );
                           

            var sharedQuestion = modelProfileService.GetSourceQuestionSharedFrom("33", course);
            Assert.IsTrue(sharedQuestion.SharedWith == "Test1, Test");
            Assert.IsTrue(sharedQuestion.QuestionId == "33");
        }  
        
        [TestMethod]
        public void GetQuestionMetadataForCourse_QuestionWithNoSections_ProperMetadata()
        {
            var question = new Question()
                            {
                                DraftFrom = "131",
                                InteractionType = "choice",
                                Status = "1"
                            };
                           
        
            var metadata = modelProfileService.GetQuestionMetadataForCourse(question);
            Assert.IsTrue(metadata.Data.Count() == 7);
            Assert.IsTrue(metadata.Data[MetadataFieldNames.DraftFrom] == "131");
            Assert.IsTrue(metadata.Data[MetadataFieldNames.DlapType] == "Multiple Choice");


        } 
        
        [TestMethod]
        public void GetQuestionMetadataForCourse_QuestionWithSectionsWithoutDynamicValuesAndCourse_ProperMetadata()
        {
            var course = new Course()
                         {
                             ProductCourseId = "12"
                         };
            
            var question = new Question()
                            {
                                DraftFrom = "131",
                                InteractionType = "choice",
                                Status = "1",
                                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = "1",
                                                                Title = "Test1",
                                   
                                                            },  
                                                            
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = "2",
                                                                Title = "Test",
                                   
                                                            },

                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = course.ProductCourseId,
                                                                Bank = "Test",
                                                               
                                                            }
                                                        },
                                Id = "questionId"
                            };

            noteCommands.GetQuestionNotes(question.Id).Returns(new List<Note>()
                                                               {
                                                                   new Note()
                                                                   {
                                                                       Text = "note1"
                                                                   },
                                                                   new Note()
                                                                   {
                                                                       Text = "note2"
                                                                   },
                                                               });

            productCourseOperation.GetCoursesByCourseIds(null)
                .ReturnsForAnyArgs(new List<Course>
                         {
                             new Course()
                             {
                                 Title = "Test1"
                             },
                             new Course()
                             {
                                 Title = "Test"
                             },
                         });
                           
        
            var metadata = modelProfileService.GetQuestionMetadataForCourse(question, course);

            Assert.IsTrue(metadata.Data[MetadataFieldNames.DraftFrom] == "131");
            Assert.IsTrue(metadata.Data[MetadataFieldNames.DlapType] == "Multiple Choice");
            Assert.IsTrue(metadata.Data[MetadataFieldNames.Bank] == "Test");
            Assert.IsTrue(metadata.Data[MetadataFieldNames.SharedWith] == "Test1<br>Test");
            Assert.IsTrue(metadata.Data[MetadataFieldNames.Notes] == "- note1<br>- note2");


        } 
        
        [TestMethod]
        public void GetQuestionMetadataForCourse_QuestionWithSectionsWithDynamicItemLink_ProperMetadata()
        {
            var course = new Course()
                         {
                             ProductCourseId = "12",
                             FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                                {
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "itemlink",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                                    {
                                                                                        new CourseMetadataFieldValue()
                                                                                        {
                                                                                            Id = "1",
                                                                                            Text = "test link 1"
                                                                                        }, 
                                                                                        
                                                                                        new CourseMetadataFieldValue()
                                                                                        {
                                                                                            Id = "2",
                                                                                            Text = "test link 2"
                                                                                        },

                                                                                         new CourseMetadataFieldValue()
                                                                                        {
                                                                                            Id = "3",
                                                                                            Text = "test 3"
                                                                                        }
                                                                                    },
                                                      Type = MetadataFieldType.ItemLink
                                                    }
                                                }
                         };
            
            var question = new Question()
                            {
                                DraftFrom = "131",
                                InteractionType = "choice",
                                Status = "1",
                                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                          
                                                            new QuestionMetadataSection()
                                                            {
                                                                ProductCourseId = course.ProductCourseId,
                                                                Bank = "Test",
                                                                DynamicValues = new Dictionary<string, List<string>>()
                                                                                {
                                                                                    {"itemlink", new List<string>()
                                                                                                 {
                                                                                                     "#/launchpad/item/1?mode=preview",
                                                                                                     "#/launchpad/item/2?mode=preview",
                                                                                                 }}
                                                                                }
                                                               
                                                            }
                                                        },
                                Id = "questionId"
                            };

          

    
            var metadata = modelProfileService.GetQuestionMetadataForCourse(question, course);

            Assert.IsTrue(metadata.Data[MetadataFieldNames.DraftFrom] == "131");
            Assert.IsTrue(metadata.Data[MetadataFieldNames.DlapType] == "Multiple Choice");
            Assert.IsTrue(metadata.Data[MetadataFieldNames.Bank] == "Test");
            Assert.IsTrue(metadata.Data["itemlink"] == "test link 1, test link 2");
            


        }        

        [TestMethod]
        public void GetDefaultSectionForViewModel_QuestionWithProductCourseWithoutParentId_DefaultSection()
        {
            var course = new Course()
                         {
                             ProductCourseId = "12",
                             FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                                {
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "field 1",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>(),
                                                      Type = MetadataFieldType.Text
                                                    },
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "field 2",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>(),
                                                      Type = MetadataFieldType.Text
                                                    },

                                                     new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "field 3",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>
                                                                                    {
                                                                                        new CourseMetadataFieldValue(),
                                                                                        new CourseMetadataFieldValue()
                                                                                    },
                                                      Type = MetadataFieldType.Text
                                                    }
                                                }
                         };
            
            var question = new Question()
                            {
                                DraftFrom = "131",
                                InteractionType = "choice",
                                Status = "1",
                                DefaultSection = new QuestionMetadataSection()
                                                 {
                                                     ParentProductCourseId = "12",
                                                     ProductCourseId = "23",
                                                    
                                                 },
                                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                            new QuestionMetadataSection()
                                                            {
                                                                 ProductCourseId = "23",
                                                                 DynamicValues = new Dictionary<string, List<string>>()
                                                                     {
                                                                         {"field 1", new List<string>(){"23"}},
                                                                         {"field 2", new List<string>(){"23"}}
                                                                     }
                                                            }
                                                        },
                                Id = "questionId"
                            };




            productCourseOperation.GetProductCourse(Arg.Any<string>()).Returns(course);

            var defaultSection = modelProfileService.GetDefaultSectionForViewModel(question);
            Assert.IsTrue(defaultSection.DynamicValues.Count()==3);
            Assert.IsFalse(defaultSection.DynamicValues.First(x=> x.Key == "field 3").Value.Any());



        } 
        
        [TestMethod]
        public void GetDefaultSectionForViewModel_QuestionWithProductCourseWithParentId_DefaultSection()
        {
            var course = new Course()
                         {
                             ProductCourseId = "12",
                             
                             FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                                {
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "field 1",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>(),
                                                      Type = MetadataFieldType.Text
                                                    },
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "field 2",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>(),
                                                      Type = MetadataFieldType.Text
                                                    },

                                                     new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "field 3",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>
                                                                                    {
                                                                                        new CourseMetadataFieldValue(),
                                                                                        new CourseMetadataFieldValue()
                                                                                    },
                                                      Type = MetadataFieldType.Text
                                                    }
                                                }
                         };
            
            var question = new Question()
                            {
                                DraftFrom = "131",
                                InteractionType = "choice",
                                Status = "1",
                                DefaultSection = new QuestionMetadataSection()
                                                 {
                                                     ParentProductCourseId = "12",
                                                     ProductCourseId = "23",
                                                    
                                                 },
                                ProductCourseSections = new List<QuestionMetadataSection>()
                                                        {
                                                            new QuestionMetadataSection()
                                                            {
                                                                ParentProductCourseId = "12",
                                                                 ProductCourseId = "23",
                                                                 DynamicValues = new Dictionary<string, List<string>>()
                                                                     {
                                                                         {"field 1", new List<string>(){"23"}},
                                                                         {"field 2", new List<string>(){"23"}}
                                                                     }
                                                            }
                                                        },
                                Id = "questionId"
                            };




            productCourseOperation.GetProductCourse(Arg.Any<string>()).Returns(course);

            var defaultSection = modelProfileService.GetDefaultSectionForViewModel(question);
            Assert.IsTrue(defaultSection.DynamicValues.Count()==3);
            Assert.IsFalse(defaultSection.DynamicValues.First(x=> x.Key == "field 3").Value.Any());



        }


        [TestMethod]
        public void GetModifierName_EmptyName_Unknown()
        {
            Assert.IsTrue(modelProfileService.GetModifierName(null) == "(Unknown)");
        }
        [TestMethod]
        public void GetModifierName_NameToGet_Unknown()
        {
            userOperation.GetUser("id").Returns(new UserInfo()
                                                {
                                                    LastName = "last",
                                                    FirstName = "first"
                                                });
            Assert.IsTrue(modelProfileService.GetModifierName("id") == "first last");
        }

        [TestMethod]
        public void GetModifierName_NameToUserIsNull_Unknown()
        {

            Assert.IsTrue(modelProfileService.GetModifierName("id") == "(Unknown)");
        }


         [TestMethod]
        public void GetDuplicateFromQuestion_RepoIdAndDuplicateFrom_Question()
         {
             questionCommands.GetQuestion("12", "4343").Returns(new Question()
                                                                {
                                                                    Score = 1
                                                                });
            Assert.IsTrue(modelProfileService.GetDuplicateFromQuestion("12", "4343").Score == 1);
        }

      [TestMethod]
         public void GetCourseBanks_Course_Bank()
         {
             var course = new Course()
             {
                 ProductCourseId = "12",

                 FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                                {
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = MetadataFieldNames.Bank,
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                                    {
                                                                                       new CourseMetadataFieldValue()
                                                                                       {
                                                                                           Text = "bank"
                                                                                       }
                                                                                    },
                                                      Type = MetadataFieldType.Text
                                                    }
                                                   
                                                }
             };

            Assert.IsTrue(modelProfileService.GetCourseBanks(course) == "bank");
        }

      [TestMethod]
      public void GetCourseBanks_CourseWithoutBanks_EmptyString()
      {
          var course = new Course()
          {
              ProductCourseId = "12",
              FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
          };

          Assert.IsTrue(string.IsNullOrEmpty(modelProfileService.GetCourseBanks(course)));
      }


      [TestMethod]
      public void GetCourseChapters_Course_Chapter()
      {
          var course = new Course()
          {
              ProductCourseId = "12",

              FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                                {
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = MetadataFieldNames.Chapter,
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                                    {
                                                                                       new CourseMetadataFieldValue()
                                                                                       {
                                                                                           Text = "chapter"
                                                                                       },
                                                                                       new CourseMetadataFieldValue()
                                                                                       {
                                                                                           Text = "chapter1"
                                                                                       }
                                                                                    },
                                                      Type = MetadataFieldType.SingleSelect
                                                    }
                                                   
                                                }
          };

          Assert.IsTrue(modelProfileService.GetCourseChapters(course) == "chapter\nchapter1");
      }
        
      [TestMethod]
      public void GetMetadataFieldValues_Field_AvailibleChoices()
      {
          var field = new CourseMetadataFieldDescriptor()
                      {
                          CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                      {
                                                          new CourseMetadataFieldValue()
                                                          {
                                                              Id = "23",
                                                              Text = "Text1"
                                                          },
                                                          new CourseMetadataFieldValue()
                                                          {
                                                              Text = "Text2"
                                                          }
                                                      }
                      };

          var choices = modelProfileService.GetMetadataFieldValues(field);
          Assert.IsTrue(choices.Count()== 2);
          Assert.IsTrue(choices.Any(x=> x.Value=="23"));
          Assert.IsTrue(choices.Any(x => x.Value == "Text2" && x.Text == "Text2"));
      }


      [TestMethod]
      public void GetMetadataFieldValues_FieldsAreNull_DefaultFielddescriptors()
      {
          var metadataConfigViewModel = new MetadataConfigViewModel()
                                        {
                                            Banks = "bank\nbank1",
                                            Chapters = null
                                        };

          var descriptors = modelProfileService.GetCourseFieldDescriptors(metadataConfigViewModel);
          Assert.IsTrue(descriptors.Count() == 4);
          Assert.IsTrue(descriptors.First(x=> x.Name == MetadataFieldNames.Bank).CourseMetadataFieldValues.Select(x=> x.Text).Count(x=> x == "bank" || x=="bank1") == 2);
     
      } 
        
        [TestMethod]
      public void GetMetadataFieldValues_ModelWithField_CorrectFieldDescriptors()
      {
          var metadataConfigViewModel = new MetadataConfigViewModel()
                                        {
                                            Banks = "bank\nbank1",
                                            Chapters = null,
                                            Fields = new List<ProductCourseSpecificMetadataFieldViewModel>()
                                                     {
                                                         new ProductCourseSpecificMetadataFieldViewModel()
                                                         {
                                                             DisplayOptions = new MetadataFieldDisplayOptionsViewModel(){},
                                                             FieldName = "field1",
                                                             InternalName = "field1",
                                                             ValuesOptions = new List<AvailableChoiceItem>()
                                                                             {
                                                                                 new AvailableChoiceItem()
                                                                                 {
                                                                                     Text = "text1",
                                                                                     Value = "value1"
                                                                                 },
                                                                                   new AvailableChoiceItem()
                                                                                 {
                                                                                     Text = "text1"
                                                                                 },

                                                                                   new AvailableChoiceItem()
                                                                                 {
                                                                                    Value = "value1"
                                                                                 },

                                                                                   new AvailableChoiceItem()
                                                                                 {
                                                                                    
                                                                                 }
                                                                             },
                                                            FieldType = MetadataFieldType.SingleSelect
                                                         },

                                                          new ProductCourseSpecificMetadataFieldViewModel()
                                                         {
                                                             DisplayOptions = new MetadataFieldDisplayOptionsViewModel(){},
                                                             FieldName = "field2",
                                                             InternalName = "value2",
                                                             ValuesOptions = new List<AvailableChoiceItem>()
                                                                             {
                                                                                 new AvailableChoiceItem()
                                                                                 {
                                                                                     Text = "text1",
                                                                                     Value = "value1",
                                                                                     
                                                                                 },
                                                                                
                                                                             },
                                                             FieldType = MetadataFieldType.ItemLink
                                                            
                                                         }
                                                     }
                                        };

          var descriptors = modelProfileService.GetCourseFieldDescriptors(metadataConfigViewModel);
          Assert.IsTrue(descriptors.Count() == 6);
          Assert.IsTrue(descriptors.First(x=> x.Name == MetadataFieldNames.Bank).CourseMetadataFieldValues.Select(x=> x.Text).Count(x=> x == "bank" || x=="bank1") == 2);
          Assert.IsTrue(descriptors.First(x=> x.Name == "field1").CourseMetadataFieldValues.Any(x=> x.Text == "text1" || x.Id=="value1"));
     
      }


    }
}
