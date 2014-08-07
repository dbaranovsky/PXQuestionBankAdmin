using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
            questionCommands.GetQuestionList(string.Empty, string.Empty, null, null, 0, 0).ReturnsForAnyArgs(GetQuestions());
            questionCommands.GetComparedQuestionList(null, null, null, 0, 0).ReturnsForAnyArgs(GetComparedQuestions());

            temporaryQuestionOperation = Substitute.For<ITemporaryQuestionOperation>();
            productCourseManagementService = Substitute.For<IProductCourseManagementService>();
            keywordOperation = Substitute.For<IKeywordOperation>();
            SetSubstituteForParsedFileOperations();
            questionManagementService = new QuestionManagementService(questionCommands, temporaryQuestionOperation, productCourseManagementService, keywordOperation, parsedFileOperation);
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

            temporaryQuestionOperation.CreateQuestion(Arg.Any<string>(), Arg.Any<Question>()).Returns(x => (Question)x[1]);
            var result = questionManagementService.CreateQuestion(new Course()
                                                                  {
                                                                      QuestionRepositoryCourseId = "1213",
                                                                      ProductCourseId = "1331"
                                                                  }, "HTS", "bank", "chapter");
            Assert.IsTrue(result.ProductCourseSections.First().Bank == "bank");
            Assert.IsTrue(result.ProductCourseSections.First().Chapter == "chapter");
        }

        [TestMethod]
        public void GetQuestion_AnyParametrs_Question()
        {
          questionCommands.GetQuestion(Arg.Any<string>(), Arg.Any<string>()).Returns(new Question()
                                                                                       {
                                                                                           Id = "15"
                                                                                       });
           var question = questionManagementService.GetQuestion(new Course(), string.Empty);
           Assert.AreEqual("15", question.Id);
        }




        [TestMethod]
        public void PublishToTitle_AnyParameters_TransferNotSuccessfulFromQuestionCommands()
        {
          Course course = new Course();
          string[] questionIds = {"1", "2"};
          var result = questionManagementService.PublishToTitle(questionIds, 1, "bank 1", "chapter 1", course);

          Assert.IsFalse(result.IsSuccess);
        }

        [TestMethod]
        public void PublishToTitle_AnyParameters_TransferSuccessfulSuccessfulFromQuestionCommands()
        {
            questionCommands.UpdateQuestions(Arg.Any<IEnumerable<Question>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            Course course = new Course();
            string[] questionIds = new[] { "1", "2" };
            var result = questionManagementService.PublishToTitle(questionIds, 1, "bank 1", "chapter 1", course);

            Assert.IsTrue(result.IsSuccess);
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

            questionCommands.UpdateQuestions(Arg.Any<IEnumerable<Question>>(), Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            questionCommands.GetQuestions(Arg.Any<string>(), Arg.Any<string[]>()).Returns(questions);

            var result = questionManagementService.PublishToTitle(questionIds, newProductCourseId, "bank 1", "chapter 1", course);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(questions[0].ProductCourseSections.Count(q => q.ProductCourseId == newProductCourseId.ToString()) == 1);
            Assert.IsTrue(questions[1].ProductCourseSections.Count(q => q.ProductCourseId == newProductCourseId.ToString()) == 1);
        }

        [TestMethod]
        public void RemoveRelatedQuestionTempResources_QuestionWithImages_SuccessRun()
        {
            Course course = new Course()
            {
                ProductCourseId = "123",
                FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
            };

            questionCommands.GetAgilixQuestion(null, null).ReturnsForAnyArgs(new Bfw.Agilix.DataContracts.Question()
                                                                             {
                                                                                 QuestionXml = questionBodyWithImages
                                                                             });
            questionManagementService.RemoveRelatedQuestionTempResources("QSDDF", course);
        }


        [TestMethod]
        public void RemoveRelatedQuestionTempResources_QuestionWithoutImages_SuccessRun()
        {
            Course course = new Course()
            {
                ProductCourseId = "123",
                FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
            };

            questionCommands.GetAgilixQuestion(null, null).ReturnsForAnyArgs(new Bfw.Agilix.DataContracts.Question()
            {
                QuestionXml = questionBodyWithoutImages
            });
            questionManagementService.RemoveRelatedQuestionTempResources("QSDDF", course);
        }

        [TestMethod]
        public void ImportFile_ParsedQuestionFileWithOnequestion_One()
        {
            var course = new Course()
                         {
                             QuestionRepositoryCourseId = "1525",
                             ProductCourseId = "2135"
                         };
            productCourseManagementService.GetProductCourse(null).ReturnsForAnyArgs(course);
            modelProfileService.GetQuestionFromParsedQuestion(Arg.Any<ParsedQuestion>(), course)
             .Returns(x => GetQuestionFromParsedQuestion((ParsedQuestion)x[0], (Course)x[1]));
            Assert.IsTrue(questionManagementService.ImportFile(1, "2135") == 1);
        }

        [TestMethod]
        public void ImportFile_ParsedQuestionFileWithFiveQuestions_Five()
        {

            var course = new Course()
                         {
                             QuestionRepositoryCourseId = "1525",
                             ProductCourseId = "2135"
                         };

            modelProfileService.GetQuestionFromParsedQuestion(Arg.Any<ParsedQuestion>(), course)
                .Returns(x => GetQuestionFromParsedQuestion((ParsedQuestion) x[0], (Course)x[1]));
            productCourseManagementService.GetProductCourse(null).ReturnsForAnyArgs(course);
            Assert.IsTrue(questionManagementService.ImportFile(5, "2135")== 5);

        }


        [TestMethod]
        public void ImportQuestions_TwoCorrectCoursesAndQuestions_SuccesImportAndTargetCourseIsUpdated()
        {

            var course = new Course()
                         {
                             QuestionRepositoryCourseId = "1525",
                         };

            var targetCourse = new Course()
                               {
                                   QuestionRepositoryCourseId = "1523",
                               };
            Assert.IsTrue(questionManagementService.ImportQuestions(course, new []{"1", "2"}, targetCourse));
        }


        [TestMethod]
        public void ImportQuestions_NullCourses_FailedImport()
        {
            Assert.IsFalse(questionManagementService.ImportQuestions(null, new[] { "1", "2" }, null));
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
                var serializer = new XmlSerializer(typeof(List<ParsedQuestion>));
                serializer.Serialize(writer, result.FileValidationResults.First().Questions.Where(x => x.IsParsed).ToList());
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
