using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.PX.Biz.Direct.Services.Tests
{
    [TestClass, Ignore]
    public class RAServicesTest
    {
        #region TestInitialize

        string _testInstructorId;
        string _testInstructorPwd;
        string _testInstructorRaId;

        string _testStudentId;
        string _testStudentPwd;
        string _testStudentRaId;

        [TestInitialize]
        public void TestInitialize()
        {
            _testInstructorId = ConfigurationManager.AppSettings["TestInstructorId"];
            _testInstructorPwd = ConfigurationManager.AppSettings["TestInstructorPwd"];
            _testInstructorRaId = ConfigurationManager.AppSettings["TestInstructorRaId"];
            _testStudentId = ConfigurationManager.AppSettings["TestStudentId"];
            _testStudentPwd = ConfigurationManager.AppSettings["TestStudentPwd"];
            _testStudentRaId = ConfigurationManager.AppSettings["TestStudentRaId"];
        }

        #endregion TestInitialize

        [TestCategory("LMSIntegration"), TestMethod]
        public void Can_AuthenticateUser_As_Instructor()
        {
            //Arrange
            var svc = new Bfw.PX.Biz.Direct.Services.RAServices();
            var account = new Bfw.PX.PXPub.Models.Account
            {
                Username = _testInstructorId,
                Password = _testInstructorPwd
            };

            //Act
            var response = svc.AuthenticateUser(account.Username, account.Password);

            //Assert
            Assert.AreEqual(_testInstructorRaId, response.UserInfo.UserId, String.Format("The Instructor's RaId ({0}) didn't match their UserInfo.UserId ({1})", _testInstructorRaId ?? "", response.UserInfo.UserId ?? ""));
        }


        [TestCategory("LMSIntegration"), TestMethod]
        public void Cannot_AuthenticateUser_As_Instructor_Using_Invalid_Id()
        {
            //Arrange
            var svc = new Bfw.PX.Biz.Direct.Services.RAServices();
            var account = new Bfw.PX.PXPub.Models.Account
            {
                Username = _testInstructorId,
                Password = "BadPassword"
            };

            //Act
            var response = svc.AuthenticateUser(account.Username, account.Password);

            //Assert
            Assert.IsNull(response.UserInfo);
            Assert.IsTrue(response.Error.Message.StartsWith("Invalid password for User"));
        }


        [TestCategory("LMSIntegration"), TestMethod]
        public void Can_AuthenticateUser_As_Student()
        {
            //Arrange
            var svc = new Bfw.PX.Biz.Direct.Services.RAServices();
            var account = new Bfw.PX.PXPub.Models.Account
            {
                Username = _testStudentId,
                Password = _testStudentPwd
            };

            //Act
            var response = svc.AuthenticateUser(account.Username, account.Password);

            //Assert
            Assert.AreEqual(_testStudentRaId, response.UserInfo.UserId, String.Format("The Student's RaId ({0}) didn't match their UserInfo.UserId ({1})", _testStudentRaId ?? "", response.UserInfo.UserId ?? ""));
        }

        [TestCategory("LMSIntegration"), TestMethod]
        public void Cannot_AuthenticateUser_As_Student_Using_Invalid_Id()
        {
            //Arrange
            var svc = new Bfw.PX.Biz.Direct.Services.RAServices();
            var account = new Bfw.PX.PXPub.Models.Account
            {
                Username = _testStudentId,
                Password = "BadPassword"
            };

            //Act
            var response = svc.AuthenticateUser(account.Username, account.Password);

            //Assert
            Assert.IsNull(response.UserInfo);
            Assert.IsTrue(response.Error.Message.StartsWith("Invalid password for User"));
        }

    }
}
