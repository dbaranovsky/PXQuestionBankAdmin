using System;
using System.CodeDom;
using System.IO;
using System.IO.Pipes;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetDocumentTest
    {
        private GetDocument _getDocument;
        [TestInitialize]
        public void TestInitialize()
        {
            _getDocument = new GetDocument();
            _getDocument.SearchParameters = new SubmissionSearch();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_EnrollmentId_Is_Null()
        {
            _getDocument.SearchParameters.EnrollmentId = null;
            _getDocument.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_EnrollmentId_Is_Empty_String()
        {
            _getDocument.SearchParameters.EnrollmentId = "";
            _getDocument.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_ItemId_Is_Null()
        {
            _getDocument.SearchParameters.ItemId = null;
            _getDocument.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_ItemId_Is_Empty_String()
        {
            _getDocument.SearchParameters.ItemId = "";
            _getDocument.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_FilePath_Is_Null()
        {
            _getDocument.SearchParameters.FilePath = null;
            _getDocument.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_FilePath_Is_Empty_String()
        {
            _getDocument.SearchParameters.FilePath = "";
            _getDocument.ToRequest();
        }

        [TestMethod]
        public void Verify_DlapRequest_Type_Is_A_Get_Request()
        {
            _getDocument.SearchParameters.EnrollmentId = "11111";
            _getDocument.SearchParameters.ItemId = "SomeItemId";
            _getDocument.SearchParameters.FilePath = "SomeFilePath";
            _getDocument.SearchParameters.PackageType = "SomePackageType";

            DlapRequest request = _getDocument.ToRequest();
            Assert.AreEqual(DlapRequestType.Get, request.Type);
        }

        [TestMethod]
        public void Verify_DlapRequst_Command_Is_getdocument()
        {
            _getDocument.SearchParameters.EnrollmentId = "11111";
            _getDocument.SearchParameters.ItemId = "SomeItemId";
            _getDocument.SearchParameters.FilePath = "SomeFilePath";
            _getDocument.SearchParameters.PackageType = "SomePackageType";
            
            DlapRequest request = _getDocument.ToRequest();

            Assert.AreEqual("getdocument", request.Parameters["cmd"]);
        }

        [TestMethod]
        public void Verify_SubmissionXml_When_ContentType_Is_TextXml()
        {
            //Arragne
            const string testXml = "<root>test xml</root>";
            var encoding = new System.Text.UnicodeEncoding();
            byte[] testBytes = encoding.GetBytes(testXml);
            DlapResponse response = new DlapResponse() { ContentType = "text/xml", ResponseStream = new MemoryStream(testBytes) };

            //Act
            _getDocument.ParseResponse(response);
            
            //Assert
            Assert.IsNotNull(_getDocument.SubmissionXml);
        }


        //[TestMethod]
        //public void Verify_Submission_When_ContentType_Is_Not_TextXml()
        //{
        //    //Arragne
        //    const string testXml = "<root>test</root>";
        //    var encoding = new System.Text.UnicodeEncoding();
        //    byte[] testBytes = encoding.GetBytes(testXml);
        //    DlapResponse response = new DlapResponse() { ContentType = "Not text/xml", ResponseStream = new MemoryStream(testBytes)};
            
        //    //Act
        //    _getDocument.ParseResponse(response);

        //    //Assert
        //    Assert.IsNotNull(_getDocument.Submission);
        //}
    
    }
}
