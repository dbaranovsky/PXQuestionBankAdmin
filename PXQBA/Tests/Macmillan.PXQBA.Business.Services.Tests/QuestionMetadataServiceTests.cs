using System.Collections.Generic;
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
    public class QuestionMetadataServiceTests
    {
        private IQuestionMetadataService questionMetadataService;

        private IProductCourseOperation productCourseOperation;
        private AutomapperConfigurator automapperConfigurator;
        private IModelProfileService modelProfileService;

        [TestInitialize]
        public void TestInitialize()
        {

            modelProfileService = Substitute.For<IModelProfileService>();
            productCourseOperation = Substitute.For<IProductCourseOperation>();

            automapperConfigurator = new AutomapperConfigurator(new ModelProfile(modelProfileService));
            automapperConfigurator.Configure();

            questionMetadataService = new QuestionMetadataService(productCourseOperation);
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
                                                        Hidden = false,
                                                        Searchterm = "",
                                                        Type = MetadataFieldType.Text
                                                    }
                                                },
                            LearningObjectives = new List<LearningObjective>()
                         };

            var fields = questionMetadataService.GetAvailableFields(course);

            Assert.IsNotNull(fields);
            Assert.IsTrue(fields.Count>0);
        }
    }


}
