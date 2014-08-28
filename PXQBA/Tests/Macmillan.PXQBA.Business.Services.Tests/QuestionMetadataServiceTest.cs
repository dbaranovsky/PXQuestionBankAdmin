using System.Collections.Generic;
using System.Linq;
using Bfw.Common.Database;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.Services.DLAP;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services.Automapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class QuestionMetadataServiceTest
    {
        private IQuestionMetadataService questionMetadataService;

        private IProductCourseOperation productCourseOperation;
        private IKeywordOperation keywordOperation;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;

        [TestInitialize]
        public void TestInitialize()
        {

            modelProfileService = Substitute.For<IModelProfileService>();
            productCourseOperation = Substitute.For<IProductCourseOperation>();
            keywordOperation = Substitute.For<IKeywordOperation>();

            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();

            questionMetadataService = new QuestionMetadataService(productCourseOperation, keywordOperation);
        }


        [TestMethod]
        public void GetAvailableFields_CourseWithOneDescriptors_AvailibleFieldsNotNull()
        {
            var course = new Course()
                         {
                             FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                                {
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "test",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>(),
                                                        Filterable = true,
                                                        FriendlyName = "Test",
                                                        Searchterm = "",
                                                        Type = MetadataFieldType.Text
                                                    }
                                                },
                         };

            var fields = questionMetadataService.GetAvailableFields(course);

            Assert.IsNotNull(fields);
            Assert.IsTrue(fields.Count>0);
        }

        [TestMethod]
        public void GetDataForFields_CourseWithTwoDescriptors_AvailibleFieldsNotNull()
        {

           
            
            var course = new Course()
            {
                FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                                {
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "test",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>(),
                                                        Filterable = true,
                                                        FriendlyName = "Test",
                                                        Searchterm = "",
                                                        Type = MetadataFieldType.Text
                                                    },

                                                     new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "test1",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>(),
                                                        Filterable = true,
                                                        FriendlyName = "Test",
                                                        Searchterm = "",
                                                        Type = MetadataFieldType.Text
                                                    }
                                                },
            };

            productCourseOperation.GetUserAvailableCourses().Returns(new List<Course>()
                                                                     {
                                                                         course
                                                                     });

            var fields = questionMetadataService.GetDataForFields(course, new List<string>{"test1"});

            Assert.IsNotNull(fields);
            Assert.IsTrue(fields.Count == 1);
        }

        [TestMethod]
        public void GetQuestionCardLayout_CourseWithQuestionCardLayout_QuestionCardLayout()
        {
            var course = new Course()
            {
                QuestionCardLayout = "QuestionCardLayout"
            };

            Assert.IsTrue(questionMetadataService.GetQuestionCardLayout(course) == course.QuestionCardLayout);
        }


        [TestMethod]
        public void GetDataForFields_CourseWithTwoDescriptorsAndManuallyAddedKeywords_KeywordsSettedToCourse()
        {



            var course = new Course()
            {
               QuestionRepositoryCourseId = "12",
               ProductCourseId = "12",
               Title = "Test title",
                FieldDescriptors = new List<CourseMetadataFieldDescriptor>()
                                                {
                                                    new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "test",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>(),
                                                        Filterable = true,
                                                        FriendlyName = "Test",
                                                        Searchterm = "",
                                                        Type = MetadataFieldType.Text
                                                    },

                                                     new CourseMetadataFieldDescriptor()
                                                    {
                                                        Name = "test1",
                                                        CourseMetadataFieldValues = new List<CourseMetadataFieldValue>()
                                                                                    {
                                                                                        new CourseMetadataFieldValue()
                                                                                        {
                                                                                            Text = "keyword 1",
                                                                                            Id = "keyword 1"
                                                                                        }
                                                                                    },
                                                        Filterable = true,
                                                        FriendlyName = "Test",
                                                        Searchterm = "",
                                                        Type = MetadataFieldType.Keywords
                                                    }
                                                },
            };

            productCourseOperation.GetUserAvailableCourses().Returns(new List<Course>()
                                                                     {
                                                                         course
                                                                     });

            keywordOperation.GetKeywordList("12", "test1").Returns(new List<string>()
                                                                   {
                                                                       "keyword 1",
                                                                       "keyword 2"
                                                                   });

            var fields = questionMetadataService.GetDataForFields(course, new List<string> { "test1" });
     
            Assert.IsNotNull(fields);
            Assert.IsTrue(fields.Count == 1);
            Assert.IsTrue(fields.Select(x => x.TypeDescriptor.AvailableChoice.Select(y => y.Text)).Select(x => x.Count(y => y == "keyword 1" || y == "keyword 2")).Sum() == 2);
        }

       
    }


}
