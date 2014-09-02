using System;
using System.Collections.Generic;
using System.ComponentModel;
using AutoMapper;
using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Commands.DataContracts;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Business.Services.Automapper;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.CompareTitles;
using Macmillan.PXQBA.Web.ViewModels.MetadataConfig;
using Macmillan.PXQBA.Web.ViewModels.TiteList;
using Macmillan.PXQBA.Web.ViewModels.User;
using Macmillan.PXQBA.Web.ViewModels.Versions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Course = Macmillan.PXQBA.Business.Models.Course;
using FileValidationResult = Macmillan.PXQBA.Business.Models.FileValidationResult;
using ProductCourseViewModel = Macmillan.PXQBA.Web.ViewModels.TiteList.ProductCourseViewModel;
using Question = Macmillan.PXQBA.Business.Models.Question;
using ValidationResult = Macmillan.PXQBA.Business.Models.ValidationResult;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class ModelProfileTest
    {
        private IQuestionCommands questionCommands;
        private INoteCommands noteCommands;
        private IProductCourseOperation productCourseOperation;
        private IUserOperation userOperation;
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
        public void DataContractsCourse_ModelCourse_NoException()
        {
            Bfw.Agilix.DataContracts.Course sourse = new Bfw.Agilix.DataContracts.Course
                                                     {
                                                         Id = "Id"
                                                     };

            var target = Mapper.Map<Course>(sourse);

            Assert.IsTrue(sourse.Id == target.ProductCourseId);
        }

        [TestMethod]
        public void QuestionCardData_CourseMetadataFieldDescriptor_NoException()
        {
            QuestionCardData sourse = new QuestionCardData();

            var target = Mapper.Map<CourseMetadataFieldDescriptor>(sourse);
        }


        [TestMethod]
        public void QuestionCardDataValue_CourseMetadataFieldValue_NoException()
        {
            QuestionCardDataValue sourse = new QuestionCardDataValue();

            var target = Mapper.Map<CourseMetadataFieldValue>(sourse);
        }

        [TestMethod]
        public void Course_DataContractsCourse_NoException()
        {
            Course sourse = new Course();

            var target = Mapper.Map<Bfw.Agilix.DataContracts.Course>(sourse);
        }

        [TestMethod]
        public void CourseMetadataFieldDescriptor_QuestionCardData_NoException()
        {
            CourseMetadataFieldDescriptor sourse = new CourseMetadataFieldDescriptor();

            var target = Mapper.Map<QuestionCardData>(sourse);
        }

        [TestMethod]
        public void CourseMetadataFieldDescriptor_QuestionMetaField_NoException()
        {
            CourseMetadataFieldDescriptor sourse = new CourseMetadataFieldDescriptor();

            var target = Mapper.Map<QuestionMetaField>(sourse);
        }

        [TestMethod]
        public void CourseMetadataFieldDescriptor_MetaFieldTypeDescriptor_NoException()
        {
            CourseMetadataFieldDescriptor sourse = new CourseMetadataFieldDescriptor();

            var target = Mapper.Map<MetaFieldTypeDescriptor>(sourse);
        }

        [TestMethod]
        public void DataContractsQuestion_Question_NoException()
        {
            Bfw.Agilix.DataContracts.Question sourse = new Bfw.Agilix.DataContracts.Question();

            var target = Mapper.Map<Models.Question>(sourse);
        }

        [TestMethod]
        public void DataContractsQuestionChoice_QuestionChoice_NoException()
        {
            Bfw.Agilix.DataContracts.QuestionChoice sourse = new Bfw.Agilix.DataContracts.QuestionChoice();

            var target = Mapper.Map<Models.QuestionChoice>(sourse);
        }

        [TestMethod]
        public void Question_QuestionMetadata_NoException()
        {
            Models.Question sourse = new Models.Question()
                                     {
                                         InteractionType = "custom",
                                         CustomUrl = "CustomUrl"
                                     };

            var target = Mapper.Map<QuestionMetadata>(sourse);
        }

        [TestMethod]
        public void Question_ComparedQuestion_NoException()
        {
            Models.Question sourse = new Models.Question();
            QuestionSearchResult searchResult = new QuestionSearchResult()
                                                {
                                                    QuestionId = "QuestionId"
                                                };

            var target = Mapper.Map<ComparedQuestion>(sourse,
                opt => opt.Items.Add("CourseId", searchResult));
        }

        [TestMethod]
        public void ComparedQuestion_ComparedQuestionViewModel_NoException()
        {
            ComparedQuestion sourse = new ComparedQuestion()
                                      {
                                          Question = new Question()
                                                     {
                                                         InteractionType = "custom",
                                                         CustomUrl = "CustomUrl"
                                                     }
                                      };

            var target = Mapper.Map<ComparedQuestionViewModel>(sourse);
        }

        [TestMethod]
        public void AgilixUser_UserInfo_NoException()
        {
            AgilixUser sourse = new AgilixUser();

            var target = Mapper.Map<UserInfo>(sourse);
        }

        [TestMethod]
        public void CourseMetadataFieldValue_ChapterViewModel_NoException()
        {
            CourseMetadataFieldValue sourse = new CourseMetadataFieldValue();

            var target = Mapper.Map<ChapterViewModel>(sourse);
        }

        [TestMethod]
        public void Question_QuestionViewModel_NoException()
        {
            Models.Question sourse = new Models.Question()
                                     {
                                         InteractionType = "custom",
                                         CustomUrl = "CustomUrl",
                                         ProductCourseSections = new List<QuestionMetadataSection>()
                                                                 {
                                                                     new QuestionMetadataSection()
                                                                     {
                                                                         ParentProductCourseId = "pId",
                                                                         ProductCourseId = "Id"
                                                                     }
                                                                 }
                                     };

            productCourseOperation.GetProductCourse(string.Empty)
                                  .ReturnsForAnyArgs(new Course()
                                                    {
                                                        FieldDescriptors =
                                                            new List<CourseMetadataFieldDescriptor>()
                                                    });

            var target = Mapper.Map<QuestionViewModel>(sourse);
        }

        [TestMethod]
        public void ListQuestionMetadataSection_QuestionMetadataSection_NoException()
        {
            List<QuestionMetadataSection> sourse = new List<QuestionMetadataSection>();

            var target = Mapper.Map<QuestionMetadataSection>(sourse);
        }

        [TestMethod]
        public void Question_SharedQuestionDuplicateFromViewModel_NoException()
        {
            Models.Question sourse = new Models.Question();

            var target = Mapper.Map<Object>(sourse);
        }


        [TestMethod]
        public void QuestionViewModel_Question_NoException()
        {
            QuestionViewModel sourse = new QuestionViewModel();

            var target = Mapper.Map<Models.Question>(sourse);
        }


        [TestMethod]
        public void Course_ProductCourseViewModel_NoException()
        {                     
            Course sourse = new Course();

            var target = Mapper.Map<ProductCourseViewModel>(sourse);
        }

        [TestMethod]
        public void IEnumerableQuestion_QuestionHistoryViewModel_NoException()
        {
            IEnumerable<Question> sourse = new List<Question>();

            var target = Mapper.Map<QuestionHistoryViewModel>(sourse);
        }

        [TestMethod]
        public void Question_QuestionVersionViewModel_NoException()
        {
            Models.Question sourse = new Models.Question();

            var target = Mapper.Map<QuestionVersionViewModel>(sourse);
        }

        [TestMethod]
        public void Question_DuplicateFromViewModel_NoException()
        {
            Models.Question sourse = new Models.Question();

            var target = Mapper.Map<DuplicateFromViewModel>(sourse);
        }

        [TestMethod]
        public void Question_RestoredFromVersionViewModel_NoException()
        {
            Models.Question sourse = new Models.Question();

            var target = Mapper.Map<RestoredFromVersionViewModel>(sourse);
        }


        [TestMethod]
        public void Course_MetadataConfigViewModel_NoException()
        {
            Course sourse = new Course();

            var target = Mapper.Map<MetadataConfigViewModel>(sourse);
        }


        [TestMethod]
        public void CourseMetadataFieldDescriptor_MetadataFieldDisplayOptionsViewModel_NoException()
        {
            CourseMetadataFieldDescriptor sourse = new CourseMetadataFieldDescriptor();

            var target = Mapper.Map<MetadataFieldDisplayOptionsViewModel>(sourse);
        }


        [TestMethod]
        public void MetadataConfigViewModel_Course_NoException()
        {
            MetadataConfigViewModel sourse = new MetadataConfigViewModel();

            var target = Mapper.Map<Course>(sourse);
        }


        [TestMethod]
        public void CourseMetadataFieldValue_QuestionCardDataValue_NoException()
        {
            CourseMetadataFieldValue sourse = new CourseMetadataFieldValue();

            var target = Mapper.Map<QuestionCardDataValue>(sourse);
        }


        [TestMethod]
        public void IEnumerableRole_RoleListViewModel_NoException()
        {
            IEnumerable<Role> sourse = new List<Role>();

            var target = Mapper.Map<RoleListViewModel>(sourse);
        }


        [TestMethod]
        public void Role_RoleViewModel_NoException()
        {
            Role sourse = new Role();

            var target = Mapper.Map<RoleViewModel>(sourse);
        }


        [TestMethod]
        public void RoleViewModel_Role_NoException()
        {
            RoleViewModel sourse = new RoleViewModel()
                                   {
                                       CapabilityGroups = new List<CapabilityGroupViewModel>()
                                   };

            var target = Mapper.Map<Role>(sourse);
        }

        [TestMethod]
        public void DataContractsValidationResult_ValidationResult_NoException()
        {
            QuestionParserModule.DataContracts.ValidationResult sourse = new QuestionParserModule.DataContracts.ValidationResult();

            var target = Mapper.Map<ValidationResult>(sourse);
        }

        [TestMethod]
        public void DataContractsFileValidationResult_FileValidationResult_NoException()
        {
            QuestionParserModule.DataContracts.FileValidationResult sourse = new QuestionParserModule.DataContracts.FileValidationResult();

            var target = Mapper.Map<FileValidationResult>(sourse);
        }


        [TestMethod]
        public void ParsedQuestion_Question_NoException()
        {
            ParsedQuestion sourse = new ParsedQuestion();

            var target = Mapper.Map<Question>(sourse);
        }


        [TestMethod]
        public void ParsedResource_Resource_NoException()
        {
            ParsedResource sourse = new ParsedResource();

            var target = Mapper.Map<Resource>(sourse);
        }

        

    }
}
