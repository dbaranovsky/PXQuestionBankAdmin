using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Controllers.ContentTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.PX.Biz.ServiceContracts;
using System.Web.Mvc;
using NSubstitute;

namespace Bfw.PX.PXPub.Controllers.Tests
{
    
    
    /// <summary>
    ///This is a test class for LearningCurveActivityControllerTest and is intended
    ///to contain all LearningCurveActivityControllerTest Unit Tests
    ///</summary>
    [TestClass]
    public class LearningCurveActivityControllerTest
    {
        private LearningCurveActivityController _controller;
        private IBusinessContext _context;
        private IEnrollmentActions _enrollmentActions;
        private IContentActions _contActions;
        private IQuestionActions _questionActions;

        [TestInitialize()]
        public void MyTestInitialize()
        {
            
            _contActions = Substitute.For<IContentActions>();
            _questionActions = Substitute.For<IQuestionActions>();
            _context = Substitute.For<IBusinessContext>();
            _enrollmentActions = Substitute.For<IEnrollmentActions>();
            _controller = new LearningCurveActivityController(_context, _contActions, _questionActions, null, _enrollmentActions, null); 
            
        }

        /// <summary>
        /// If content item is found, then it should return a response with status = Success, item id and title.
        /// </summary>
        [TestCategory("LearningCurveActivityController"), TestMethod]
        public void LearningCurveActivityController_GetEbookInfo_ExpectResponsewithContentItemInfo()
        {
            _contActions.GetContent("TestEntityId", "TestId").ReturnsForAnyArgs(new ContentItem{ Id="TestId", Title="TestContentItem" });
            JsonResult result = _controller.GetEbookInfo("TestEntityId", "TestId");
            var jsonData = result.Data.ToString();
            Assert.IsTrue(jsonData.Contains("Status = Success"));
            Assert.IsTrue(jsonData.Contains("Id = TestId"));
            Assert.IsTrue(jsonData.Contains("Title = TestContentItem"));

        }

        /// <summary>
        /// If content item is not found, it should return response with status = Fail.
        /// </summary>
        [TestCategory("LearningCurveActivityController"), TestMethod]
        public void LearningCurveActivityController_GetEbookInfo_ExpectContentItemNotFoundResponseError()
        {
            _contActions.GetContent("TestEntityId", "TestId").ReturnsForAnyArgs((ContentItem) null);
            JsonResult result = _controller.GetEbookInfo("TestEntityId", "TestId");
            var jsonData = result.Data.ToString();
            Assert.IsTrue(jsonData.Contains("Status = Fail"));

        }

        /// <summary>
        /// If angel item is found, it should return response with status = Success, item id and title.
        /// </summary>
        [TestCategory("LearningCurveActivityController"), TestMethod]
        public void LearningCurveActivityController_GetEbookInfo_ExpectResponsewithAngelItemInfo()
        {
            _contActions.FindContentItems(null)
                .ReturnsForAnyArgs(new List<ContentItem> {new ContentItem { Id = "TestAngelId", Title = "TestAngelItem" }});
            JsonResult result = _controller.GetEbookInfo("TestEntityId", "TestAngelId");
            var jsonData = result.Data.ToString();
            Assert.IsTrue(jsonData.Contains("Status = Success"));
            Assert.IsTrue(jsonData.Contains("Id = TestAngelId"));
            Assert.IsTrue(jsonData.Contains("Title = TestAngelItem"));
        }

        /// <summary>
        /// If angel item is not found, it should return response with status = Fail.
        /// </summary>
        [TestCategory("LearningCurveActivityController"), TestMethod]
        public void LearningCurveActivityController_GetEbookInfo_ExpectAngelItemNotFoundResponseError()
        {
            _contActions.FindContentItems(null).ReturnsForAnyArgs((IEnumerable) null);
            JsonResult result = _controller.GetEbookInfo("TestEntityId", "TestId");
            var jsonData = result.Data.ToString();
            Assert.IsTrue(jsonData.Contains("Status = Fail"));

        }

        /// <summary>
        /// Pass no user id should return null
        /// </summary>
        [TestCategory("LearningCurveActivityController"), TestMethod]
        public void LearningCurveActivityController_GetUserEmailAddress_ExpectReturnsNull()
        {
            var result = _controller.GetUserEmailAddress(null);
            Assert.IsNull(result);

        }

        /// <summary>
        /// Pass one user id should return one user email
        /// </summary>
        [TestCategory("LearningCurveActivityController"), TestMethod]
        public void LearningCurveActivityController_GetUserEmailAddress_ExpectReturnsOneUserEmail()
        {
            _context.EntityId.Returns("TestEntityId");
            var sampleEnrollmentList = new List<Enrollment> { new Enrollment{ Id="TestEnrollmentId1", User = new UserInfo{ Email = "testEmail@asdf.com" }}};
            _enrollmentActions.GetAllEntityEnrollmentsAsAdmin("TestEntityId").Returns(sampleEnrollmentList);
            var result = _controller.GetUserEmailAddress(new List<string> { "TestEnrollmentId1" });
            Assert.IsNotNull(result.FirstOrDefault(email => email == HttpUtility.UrlEncode("testEmail@asdf.com")));
            Assert.AreEqual(result.Count(), 1);

        }

        /// <summary>
        /// Pass one two id should return two user email
        /// </summary>
        [TestCategory("LearningCurveActivityController"), TestMethod]
        public void LearningCurveActivityController_GetUserEmailAddress_ExpectReturnsTwoUserEmail()
        {
            _context.EntityId.Returns("TestEntityId");
            var sampleEnrollmentList = new List<Enrollment>
            {
                new Enrollment { Id = "TestEnrollmentId1", User = new UserInfo { Email = "test1Email@asdf.com" } },
                new Enrollment { Id = "TestEnrollmentId2", User = new UserInfo { Email = "test2Email@asdf.com" } }
            };
            _enrollmentActions.GetAllEntityEnrollmentsAsAdmin("TestEntityId").Returns(sampleEnrollmentList);
            var result = _controller.GetUserEmailAddress(new List<string> { "TestEnrollmentId1", "TestEnrollmentId2" });
            Assert.AreEqual(result.Count(), 2);
            Assert.IsNotNull(result.FirstOrDefault(email => email == HttpUtility.UrlEncode("test1Email@asdf.com")));
            Assert.IsNotNull(result.FirstOrDefault(email => email == HttpUtility.UrlEncode("test2Email@asdf.com")));


        }
    }
}
