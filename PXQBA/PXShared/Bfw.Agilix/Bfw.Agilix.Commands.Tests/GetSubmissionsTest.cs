using System;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetSubmissionsTest
    {
        private GetSubmissions submissionsCmd;
        private const string ENROLLMENT_ID = "1234";
        private const string ITEM_ID = "abcde";

        [TestInitialize]
        public void InitializeTest()
        {
            submissionsCmd = new GetSubmissions { SearchParameters = new SubmissionSearch { EnrollmentId = ENROLLMENT_ID, ItemId = ITEM_ID } };
        }

        [TestMethod]
        public void ToRequestTest()
        {
            var request = submissionsCmd.ToRequest();
            Assert.IsNotNull(request);
            Assert.IsNotNull(request.Parameters);
            Assert.AreEqual(request.Parameters["cmd"], "GetStudentSubmissionHistory");
            Assert.AreEqual(request.Parameters["enrollmentid"], ENROLLMENT_ID);
            Assert.AreEqual(request.Parameters["itemid"], ITEM_ID);
        }

        [TestMethod]
        public void ParseResponseOKTest()
        {
            string xmlResponse =  @"<submissions>
                                      <submission achieved='10' submitteddate='5/31/2013' version='1'>
                                        <user firstname='John' lastname='Doe' />
                                      </submission>                                        
                                    </submissions>";
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = XDocument.Parse(xmlResponse) };
            submissionsCmd.ParseResponse(response);
            var submissions = submissionsCmd.Submissions;
            Assert.IsNotNull(submissions);
            Assert.IsTrue(submissions.Count == 1);
            var submission = submissions[0];
            Assert.AreEqual(submission.Grade.Achieved, 10);
            Assert.AreEqual(submission.SubmittedDate, new DateTime(2013, 5, 31));
            Assert.AreEqual(submission.StudentFirstName, "John");
            Assert.AreEqual(submission.StudentLastName, "Doe");
            Assert.AreEqual(submission.Version, 1);
        }

        [TestMethod]
        public void ParseResponseNoGradeTest()
        {
            string xmlResponse = @"<submissions>
                                      <submission achieved=''>
                                        <user firstname='John' lastname='Doe' />
                                      </submission>                                        
                                    </submissions>";
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = XDocument.Parse(xmlResponse) };
            submissionsCmd.ParseResponse(response);
            var submissions = submissionsCmd.Submissions;
            Assert.IsNotNull(submissions);
            Assert.IsTrue(submissions.Count == 1);
            var submission = submissions[0];
            Assert.AreEqual(submission.Grade.Achieved, 0);
        }

        [TestMethod]
        public void ParseResponseNoSubmittedDateTest()
        {
            string xmlResponse = @"<submissions>
                                      <submission submitteddate=''>
                                        <user firstname='John' lastname='Doe' />
                                      </submission>                                        
                                    </submissions>";
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = XDocument.Parse(xmlResponse) };
            submissionsCmd.ParseResponse(response);
            var submissions = submissionsCmd.Submissions;
            Assert.IsNotNull(submissions);
            Assert.IsTrue(submissions.Count == 1);
            var submission = submissions[0];
            Assert.AreEqual(submission.SubmittedDate, DateTime.MinValue);
        }

        [TestMethod]
        public void ParseResponseNoUserTest()
        {
            string xmlResponse = @"<submissions>
                                      <submission>
                                      </submission>                                        
                                    </submissions>";
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = XDocument.Parse(xmlResponse) };
            submissionsCmd.ParseResponse(response);
            var submissions = submissionsCmd.Submissions;
            Assert.IsNotNull(submissions);
            Assert.IsTrue(submissions.Count == 1);
            var submission = submissions[0];
            Assert.AreEqual(submission.StudentFirstName, null);
            Assert.AreEqual(submission.StudentLastName, null);
        }
    }
}
