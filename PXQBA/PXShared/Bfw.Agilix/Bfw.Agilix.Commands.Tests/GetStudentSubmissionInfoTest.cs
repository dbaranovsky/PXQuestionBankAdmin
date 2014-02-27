using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetStudentSubmissionInfoTest
    {
        private GetStudentSubmissionInfo cmd;
        private const string ENROLLMENT_ID = "123456";
        private const string ITEM_ID = "4t3cd78";

        [TestInitialize]
        public void InitializeTest()
        {
            cmd = new GetStudentSubmissionInfo
            {
                Submissions = new List<Submission> { new Submission { EnrollmentId = ENROLLMENT_ID, ItemId = ITEM_ID } }
            };
        }

        [TestMethod]
        public void GetStudentSubmissionInfoAction_ToRequestTest()
        {
            var request = cmd.ToRequest();
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Parameters);
            Assert.AreEqual(request.Parameters["cmd"], "getstudentsubmissioninfo");
        }

        [TestMethod]
        public void GetStudentSubmissionInfo_ParseResponseTest()
        {
            string xmlResponse = @"<responses>
                                      <response code='OK'>
                                        <submission version='1'/>
                                      </response>                                        
                                    </responses>";
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = XDocument.Parse(xmlResponse) };
            cmd.ParseResponse(response);
            Assert.IsNotNull(cmd.Submissions);
            Assert.IsTrue(cmd.Submissions.Count == 1);
            Assert.AreEqual(cmd.Submissions[0].Version, 1);
        }

        [TestMethod]
        public void GetStudentSubmissionInfo_ParseResponseNoVersionTest()
        {
            string xmlResponse = @"<responses>
                                      <response code='OK'>
                                        <submission version=''/>
                                      </response>                                        
                                    </responses>";
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = XDocument.Parse(xmlResponse) };
            cmd.ParseResponse(response);
            Assert.IsNotNull(cmd.Submissions);
            Assert.IsTrue(cmd.Submissions.Count == 1);
            Assert.AreEqual(cmd.Submissions[0].Version, 0);
        }
    }
}
