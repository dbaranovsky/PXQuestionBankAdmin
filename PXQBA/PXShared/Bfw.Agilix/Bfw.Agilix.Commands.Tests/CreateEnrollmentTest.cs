using System;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class CreateEnrollmentTest
    {
        CreateEnrollment _cmd;

        [TestInitialize]
        public void Setup()
        {
            _cmd = new CreateEnrollment();
        }

        [TestMethod]
        public void CreateEnrollmentAction_CreateObject_EnrollmentShouldNotBeNull()
        {
            Assert.IsNotNull(_cmd.Enrollments);
        }

        [TestMethod]
        public void CreateEnrollmentAction_AddEnrollment_EnrollmentsCountIsOne()
        {
            _cmd.Add(new Enrollment());
            Assert.AreEqual<int>(_cmd.Enrollments.Count, 1);
        }

        [TestMethod]
        public void CreateEnrollmentAction_ClearEnrollments_EnrollmentsCountIsZero()
        {
            _cmd.Add(new Enrollment());
            _cmd.Clear();
            Assert.AreEqual<int>(_cmd.Enrollments.Count, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateEnrollmentAction_CreateDlapRequestWithoutEnrollment_ThrowsDlapException()
        {
            DlapRequest request = _cmd.ToRequest();
        }

        [TestMethod]
        public void CreateEnrollmentAction_CreateDlapRequest_RequestTypeIsPost()
        {
            _cmd.Add(new Enrollment());
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<DlapRequestType>(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void CreateEnrollmentAction_CreateDlapRequest_RequestModeIsBatch()
        {
            _cmd.Add(new Enrollment());
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<DlapRequestMode>(request.Mode, DlapRequestMode.Batch);
        }

        [TestMethod]
        public void CreateEnrollmentAction_CreateDlapRequest_RequestParametersInitialized()
        {
            _cmd.Add(new Enrollment());
            DlapRequest request = _cmd.ToRequest();
            Assert.IsNotNull(request.Parameters);
        }

        [TestMethod]
        public void CreateEnrollmentAction_CreateDlapRequest_RequestParametersHasCmd()
        {
            _cmd.Add(new Enrollment());
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<string>(request.Parameters["cmd"].ToString(), "createenrollments");
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateEnrollmentAction_ParseResponseWithError_ThrowsDlapException()
        {
            _cmd.ParseResponse(new DlapResponse() { Code = DlapResponseCode.Error });
        }

        [TestMethod]
        public void CreateEnrollmentAction_ParseResponse_EnrollmentsObjectPopulatedWithResponseId()
        {
            string responseString = @"<response code=""OK"">
  <responses>
    <response code=""OK"">
      <enrollment enrollmentid=""5201""/>
    </response>
  </responses>
</response>";
            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            _cmd.Add(new Enrollment());
            _cmd.ParseResponse(response);

            Assert.AreEqual(_cmd.Enrollments[0].Id, "5201");
        }
    }
}
