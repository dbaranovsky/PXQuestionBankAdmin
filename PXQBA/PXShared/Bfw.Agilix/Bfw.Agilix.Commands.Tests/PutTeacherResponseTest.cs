using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class PutTeacherResponseTest
    {
        private PutTeacherResponse cmd = null;
        private void InitCommand()
        {
            cmd = new PutTeacherResponse
            {
                ItemId = "bsi__61F443B7__4076__43AF__915D__D4F002ACAF3A"
            };
        }

        [TestMethod]
        public void PutTeacherResponseAction_ToRequestTest()
        {
            this.InitCommand();
            cmd.StudentEnrollmentId = "112669";
            cmd.TeacherResponse = new DataContracts.TeacherResponse() {  PointsAssigned=20, PointsPossible=22, ScoredVersion=2,  TeacherResponseType = DataContracts.TeacherResponseType.None };
            cmd.TeacherResponse.Responses = new System.Collections.Generic.List<DataContracts.TeacherResponse>();

            cmd.TeacherResponse.Responses.Add(new DataContracts.TeacherResponse() { PointsAssigned = 20, PointsPossible = 22, ScoredVersion = 2, TeacherResponseType = DataContracts.TeacherResponseType.None, ForeignId="" });
            var request = cmd.ToRequest();
            Assert.IsNotNull(request);
        }

        [TestMethod]
        public void PutTeacherResponseAction_ParseResponseTest()
        {
            this.InitCommand();

            var sResponse = "<response code='OK'>  <teacherresponse version='1'/></response>";
            XDocument doc = XDocument.Parse(sResponse);
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = doc, ContentType = "stream" };

            cmd.ParseResponse(dlapResponse);

            Assert.IsNotNull(dlapResponse);
        }
    }
}
