using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using System.Collections.Generic;
using System.Xml;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class PutStudentSubmissionTest
    {
        private PutStudentSubmission cmd = null;
        private void InitCommand()
        {
            DataContracts.Submission submission = new Submission() { };
            cmd = new PutStudentSubmission
            {
                EntityId = "112652",
                Submission = submission
            };
        }

        [TestMethod]
        public void PutStudentSubmissionAction_ToRequestTest()
        {
            this.InitCommand();

            cmd.Submission.EnrollmentId = "112669";

            var request = cmd.ToRequest();
            Assert.IsNotNull(request);
        }

        [TestMethod]
        public void PutStudentSubmissionAction_ParseResponseTest()
        {
            this.InitCommand();

            var sResponse = "<response><responses> <response code='OK'> <submission version='1'/> </response> </responses></response>";
            XDocument doc = XDocument.Parse(sResponse);
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = doc, ContentType = "stream" };

            cmd.ParseResponse(dlapResponse);

            Assert.IsNotNull(dlapResponse);
        }
    }
}
