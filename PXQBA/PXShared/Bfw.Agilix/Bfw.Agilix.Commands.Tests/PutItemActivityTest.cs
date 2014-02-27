using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Dlap;


namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class PutItemActivityTest
    {
        private PutItemActivity _putItemActivity;

        [TestInitialize]
        public void TestInitialize()
        {
            _putItemActivity = new PutItemActivity();
            _putItemActivity.ItemId = "something";
            _putItemActivity.EnrollmentId = "something";
        }
       

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Invalid parameters for PutItemActivity.")]
        public void PutItemActivityTest_Request_Should_Throw_Exception_If_EnrollmentId_Is_Null()
        {
            _putItemActivity.EnrollmentId = null;
            _putItemActivity.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException), "Invalid parameters for PutItemActivity.")]
        public void PutItemActivityTest_Request_Should_Throw_Exception_If_ItemId_Is_Null()
        {
            _putItemActivity.ItemId = null;
            _putItemActivity.ToRequest();
        }

        [TestMethod]
        public void PutItemActivityTest_Request_DlapRequest_Type_Should_Be_PostRequest()
        {
            var request = _putItemActivity.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void PutItemActivityTest_Request_DlapRequest_Mode_Should_Be_BatchRequest()
        {
            var request = _putItemActivity.ToRequest();
            Assert.AreEqual(request.Mode, DlapRequestMode.Batch);
        }

        [TestMethod]
        public void PutItemActivityTest_Request_DlapRequest_Parameters_Should_Have_Command_PutItemActivity()
        {
            var request = _putItemActivity.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "PutItemActivity");
        }

        [TestCategory("PutItemActivity"), TestMethod]
        public void SetNewAttemptFalse_ExpectNewAttemptNotInTheRequest()
        {
            _putItemActivity.NewAttempt = false;
            var request = _putItemActivity.ToRequest().GetXmlRequestBody().ToString();
            Assert.IsFalse(request.Contains("newattempt=\"true\""));
        } 

        [TestCategory("PutItemActivity"),TestMethod]
        public void SetNewAttemptTrue_ExpectNewAttemptInTheRequest()
        {
            _putItemActivity.NewAttempt = true;
            var request = _putItemActivity.ToRequest().GetXmlRequestBody().ToString();
            Assert.IsTrue(request.Contains("newattempt=\"true\""));
        }


    }
}
