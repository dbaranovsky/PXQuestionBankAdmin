﻿using System.Collections.Generic;
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
        public void GetAvailableFields_AnyCourse_AvailibleFieldsNotNull()
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
        public void GetDataForFields_AnyCourse_AvailibleFieldsNotNull()
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

       
    }


}