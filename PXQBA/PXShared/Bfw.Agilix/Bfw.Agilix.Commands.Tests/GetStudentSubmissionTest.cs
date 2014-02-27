using System;
using System.IO;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetStudentSubmissionTest
    {
        private GetStudentSubmission cmd;
        private const string ENROLLMENT_ID = "123456";
        private const string ITEM_ID = "ab3cd78";
        private const int VERSION = 2;

        [TestInitialize]
        public void InitializeTest()
        {
            cmd = new GetStudentSubmission
                {
                    SearchParameters = new SubmissionSearch
                    {
                        EnrollmentId = ENROLLMENT_ID,
                        ItemId = ITEM_ID,
                        Version = VERSION
                    }
                };
        }

        [TestMethod]
        public void GetStudentSubmissionAction_ToRequestTest()
        {
            var request = cmd.ToRequest();
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Parameters);
            Assert.AreEqual(request.Parameters["cmd"], "getstudentsubmission");
            Assert.AreEqual(request.Parameters["enrollmentid"], ENROLLMENT_ID);
            Assert.AreEqual(request.Parameters["itemid"], ITEM_ID);
            Assert.AreEqual(request.Parameters["version"], VERSION);
        }

        [TestMethod]
        public void GetStudentSubmissionAction_ToRequestNoPackageTypeSpecifiedTest()
        {
            var request = cmd.ToRequest();
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Parameters);
            Assert.AreEqual(request.Parameters["packagetype"], "zip");
        }

        [TestMethod]
        public void GetStudentSubmissionAction_ToRequestWithPackageTypeSpecifiedTest()
        {
            cmd.SearchParameters.PackageType = "xml";
            cmd.SearchParameters.FilePath = "/file.xml";
            var request = cmd.ToRequest();
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Parameters);
            Assert.AreEqual(request.Parameters["packagetype"], "xml");
            Assert.AreEqual(request.Parameters["filepath"], "/file.xml");
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void GetStudentSubmissionAction_ToRequestNoEnrollmentIdTest()
        {
            cmd.SearchParameters.EnrollmentId = null;
            var request = cmd.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void GetStudentSubmissionAction_ToRequestNoItemIdTest()
        {
            cmd.SearchParameters.ItemId = String.Empty;
            var request = cmd.ToRequest();
        }

        [TestMethod]
        public void GetStudentSubmissionAction_ParseResponseXmlTest()
        {
            string xmlResponse = @"<responses>
                                      <response code='OK'>
                                        <submission version='1'/>
                                      </response>                                        
                                    </responses>";
            Stream stream = new MemoryStream();
            (XDocument.Parse(xmlResponse)).Save(stream);
            stream.Position = 0;
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseStream = stream, ContentType = "text/xml" };
            cmd.ParseResponse(response);
            Assert.IsNotNull(cmd.SubmissionXml);
            Assert.IsNull(cmd.Submission);
        }

        [TestMethod]
        public void GetStudentSubmissionAction_ParseResponseStreamTest()
        {
            string xmlResponse = @"<response><responses>
                                      <response code='OK'>
                                        <submission version='1'/>
                                      </response>                                        
                                    </responses></response>";
            Stream stream = new MemoryStream();
            (XDocument.Parse(xmlResponse)).Save(stream);
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseStream = stream, ContentType = "stream" };
            cmd.ParseResponse(response);
            Assert.IsNull(cmd.SubmissionXml);
            Assert.IsNotNull(cmd.Submission);
        }
    }
}
