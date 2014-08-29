using Bfw.Agilix.DataContracts;
using Macmillan.PXQBA.Business.Commands.Contracts;
using Macmillan.PXQBA.Business.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace Macmillan.PXQBA.Business.Services.Tests
{
    [TestClass]
    public class ProductCourseManagementServiceTest
    {
        private IProductCourseOperation productCourseOperation;
        private IRoleOperation roleOperation;

        private IProductCourseManagementService productCourseManagementService;

        [TestInitialize]
        public void TestInitialize()
        {
            productCourseOperation = Substitute.For<IProductCourseOperation>();
            roleOperation = Substitute.For<IRoleOperation>();

            productCourseManagementService = new ProductCourseManagementService(productCourseOperation, roleOperation);
        }

        [TestMethod]
        public void GetProductCourse_ProductCourseId_CorrectCommandInvoked()
        {
            const string productCourseId = "productCourseId";
            bool isCorrectInvoked = false;

            productCourseOperation.When(
                o => o.GetProductCourse(Arg.Is<string>(c => c == productCourseId), Arg.Is<bool>(b => b == false)))
                    .Do(d => { isCorrectInvoked = true; });

            productCourseManagementService.GetProductCourse(productCourseId);

            Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void GetAvailableCourses_NoArguments_CorrectCommandInvoked()
        {
            bool isCorrectInvoked = false;

            productCourseOperation.When(
                o => o.GetUserAvailableCourses(Arg.Is<bool>(a=>a==false)))
                    .Do(d => { isCorrectInvoked = true; });

            productCourseManagementService.GetAvailableCourses();

            Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void GetCourseList_NoArguments_CorrectCommandInvoked()
        {
            bool isCorrectInvoked = false;

            productCourseOperation.When(
                o => o.GetUserAvailableCourses(Arg.Is<bool>(a => a == true)))
                    .Do(d => { isCorrectInvoked = true; });

            productCourseManagementService.GetCourseList();

            Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void UpdateMetadataConfig_SomeCourse_CorrectCommandInvoked()
        {
            const string productCourseId = "productCourseId";
            var course = new Models.Course()
                         {
                             ProductCourseId = productCourseId
                         };

            bool isCorrectInvoked = false;

            productCourseOperation.When(
                o => o.UpdateCourse(Arg.Is<Models.Course>(c => c.ProductCourseId == productCourseId)))
                    .Do(d => { isCorrectInvoked = true; });

            productCourseManagementService.UpdateMetadataConfig(course);

            Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void GetAllCourses_NoArguments_CorrectCommandInvoked()
        {
            bool isCorrectInvoked = false;

            productCourseOperation.When(
                o => o.GetAllCourses())
                    .Do(d => { isCorrectInvoked = true; });

            productCourseManagementService.GetAllCourses();

            Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void CreateNewDraftCourse_Title_CreateDraftAndUpdateCourseForDruftInvoked()
        {
            const string title = "some_title";
            const string productCourseId = "productCourseId";
            bool isCorrectInvoked = false;

            productCourseOperation.CreateDraftCourse(Arg.Is<string>(t => t == title)).Returns(new Models.Course()
                                                                                              {
                                                                                                  ProductCourseId =
                                                                                                      productCourseId
                                                                                              });

            productCourseOperation.When(
                o => o.UpdateCourse(Arg.Is<Models.Course>(c => c.ProductCourseId == productCourseId)))
                    .Do(d => { isCorrectInvoked = true; });

            productCourseManagementService.CreateNewDraftCourse(title);

            Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void AddSiteBuilderCourse_SomeUrl_AddSiteBuilderCourseToQBAAndUpdateCourseInvoked()
        {
            const string url = "some_url";
            const string productCourseId = "productCourseId";
            bool isCorrectInvoked = false;

            productCourseOperation.AddSiteBuilderCourseToQBA(Arg.Is<string>(u => u == url)).Returns(productCourseId);

            productCourseOperation.GetProductCourse(Arg.Is<string>(c => c == productCourseId)).Returns(new Models.Course()
            {
                ProductCourseId =
                    productCourseId
            });

            productCourseOperation.When(
                o => o.UpdateCourse(Arg.Is<Models.Course>(c => c.ProductCourseId == productCourseId)))
                    .Do(d => { isCorrectInvoked = true; });

            productCourseManagementService.AddSiteBuilderCourse(url);

            Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void RemoveResources_TemporaryCourseIdAndQuestionRelatedResources_CorrectCommandInvoked()
        {
            const string temporaryCourseId = "temporaryCourseId";
            var questionRelatedResources = new List<string>()
                                           {
                                               "res1",
                                               "res2"
                                           };
            bool isCorrectInvoked = false;

           productCourseOperation.When(
                o => o.RemoveResources(Arg.Is<string>(c => c == temporaryCourseId), 
                                       Arg.Is<List<string>>(c =>
                                                           c[0] == questionRelatedResources[0] &&
                                                           c[1] == questionRelatedResources[1]
                                                           )))
                    .Do(d => { isCorrectInvoked = true; });

           productCourseManagementService.RemoveResources(temporaryCourseId, questionRelatedResources);

           Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void PutResources_Resources_CorrectCommandInvoked()
        {
            var resources = new List<Resource>()
                                       {
                                           new Resource()
                                           {
                                               EntityId = "Res1",
                                           },

                                           new Resource()
                                           {
                                               EntityId = "Res2",
                                           }
                                       };
            bool isCorrectInvoked = false;

            productCourseOperation.When(
                 o => o.PutResources(Arg.Is<List<Resource>>(c => c[0].EntityId == resources[0].EntityId && 
                                                                 c[1].EntityId == resources[1].EntityId)))             
                     .Do(d => { isCorrectInvoked = true; });

            productCourseManagementService.PutResources(resources);

            Assert.IsTrue(isCorrectInvoked);
        }

        [TestMethod]
        public void AddSiteBuilderCourse_NotValidUrl_Null()
        {
            const string url = "not_valid_url";

            productCourseOperation.When(o=>o.AddSiteBuilderCourseToQBA(Arg.Is<string>(u => u == url)))
                .Do(d=> { throw new Exception("TestException");});

            string courseId = productCourseManagementService.AddSiteBuilderCourse(url);

            Assert.IsTrue(String.IsNullOrEmpty(courseId));
        }

    }
}
