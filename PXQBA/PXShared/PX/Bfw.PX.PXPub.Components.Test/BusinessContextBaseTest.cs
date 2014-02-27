using System;
using System.Configuration;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Caching;
using Bfw.Common.Logging;
using Bfw.PX.Biz.Components.FormsAuthBusinessContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.PX.PXPub.Components.Test
{
    /// <summary>
    /// The purpose of BusinessContextBaseTest is to provide a test for the LMS ID Integration Project.
    /// This tests the call to  ExistingUser(AdminConnection(AdminAccessType.RootAdmin), Course.Domain.Id, SSOData.UserId)  w/in BusinessContextBase
    /// which is called by Bfw.PX.Biz.Components.FormsAuthBusinessContext :: InitializeUser()  -  as part of Initialize()
    /// which is called by HomeController.cs :: InitializeHomePage()
    /// </summary>
    [TestClass, Ignore]
    public class BusinessContextBaseTest
    {
        #region TestInitialize

        private string _testInstructorUsername;
        private string _testInstructorDomainId;
        private string _testInstructorReferenceId;

        private string _testStudentUsername;
        private string _testStudentDomainId;
        private string _testStudentReferenceId;

        [TestInitialize]
        public void TestInitialize()
        {
            // set private fields
            _testInstructorUsername = ConfigurationManager.AppSettings["Instructor_Username"];
            _testInstructorDomainId = ConfigurationManager.AppSettings["Instructor_DomainId"];
            _testInstructorReferenceId = ConfigurationManager.AppSettings["Instructor_ReferenceId"];

            _testStudentUsername = ConfigurationManager.AppSettings["Student_Username"];
            _testStudentDomainId = ConfigurationManager.AppSettings["Student_DomainId"];
            _testStudentReferenceId = ConfigurationManager.AppSettings["Student_ReferenceId"];
        }
        #endregion TestInitialize

        private FormsAuthBusinessContext GetBusinessContext()
        {
            return new FormsAuthBusinessContext(
                            Substitute.For<ISessionManager>(),
                            Substitute.For<ILogger>(),
                            Substitute.For<ITraceManager>(),
                            Substitute.For<ICacheProvider>(),
                            new Bfw.PX.Biz.Direct.Services.RAServices()
                        );
        }

        [TestCategory("LMSIntegration"), TestMethod]
        public void Can_Login_Using_ExistingUser_As_Instructor()
        {
            //Arrange
            string domainId = _testInstructorDomainId;  //ie, institution/college
            string referenceId = _testInstructorReferenceId;

            //Act
            var bcObject = GetBusinessContext();
            Bfw.PX.Biz.DataContracts.UserInfo result = bcObject.GetExistingUser(domainId, referenceId);

            //Assert
            Assert.AreEqual(_testInstructorUsername, result.Username, string.Format("Failed to match Username_for_Intructor on ReferenceId={0}", referenceId ?? ""));
        }

        [TestCategory("LMSIntegration"), TestMethod]
        public void Can_Login_Using_ExistingUser_As_Student()
        {
            //Arrange
            string domainId = _testStudentDomainId;  //ie, institution/college
            string referenceId = _testStudentReferenceId;

            //Act
            var bcObject = GetBusinessContext();
            Bfw.PX.Biz.DataContracts.UserInfo result = bcObject.GetExistingUser(domainId, referenceId);

            //Assert
            Assert.AreEqual(_testStudentUsername, result.Username, string.Format("Failed to match Username_for_Student on ReferenceId={0}", referenceId ?? ""));
        }

    }
}
