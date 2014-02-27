namespace Bfw.Agilix.Commands.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Bfw.Agilix.DataContracts;
    using Bfw.Agilix.Dlap;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using NSubstitute;

    [TestClass]
    public class UpdateEnrollmentsTest
    {
        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void Verify_We_Get_Correct_DlapRequest_When_NO_Enrollements()
        {
            // Arrange
            var updateEnrollemnt = new UpdateEnrollments();
            var enrollment = Substitute.For<Enrollment>();

            // Act
            var request = updateEnrollemnt.ToRequest();

            // Assert
            Assert.AreEqual(DlapRequestType.Post, request.Type);
            Assert.AreEqual("updateenrollments", request.Parameters["cmd"]);
            enrollment.DidNotReceive().ToEntity();
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void Verify_We_Get_Correct_DlapRequest_With_Some_Enrollements()
        {
            // Arrange
            var updateEnrollemnt = new UpdateEnrollments();
            var request = updateEnrollemnt.ToRequest();
            var agilixUser = new AgilixUser
            {
                FirstName = "first",
                LastName = "last",
                Email = "first.last@macmillan.com",
                Reference = "raId",
                Credentials = new Credentials { Username = "first.last@macmillan.com" }
            };

            var domain = new Domain { Id = "onyxId", Name = "Some University", Reference = "onyxId" };
            var course = new Course { Id = "entityId" };
            var e1Enrollment = Substitute.For<Enrollment>();
            e1Enrollment.Id = "111111";
            e1Enrollment.Course = course;
            e1Enrollment.User = agilixUser;
            e1Enrollment.Domain = domain;
            e1Enrollment.CourseId = course.Id;
            e1Enrollment.EndDate = DateTime.MaxValue;
            e1Enrollment.Flags = DlapRights.Participate;

            var e2Enrollment = Substitute.For<Enrollment>();
            e2Enrollment.Id = "222222";
            e2Enrollment.Course = course;
            e2Enrollment.User = agilixUser;
            e2Enrollment.Domain = domain;
            e2Enrollment.CourseId = course.Id;
            e2Enrollment.EndDate = DateTime.MaxValue;
            e2Enrollment.Flags = DlapRights.Participate;

            updateEnrollemnt.Enrollments = new List<Enrollment> { e1Enrollment, e2Enrollment };

            //Act
            request.ToRequest();

            // Assert
            e1Enrollment.Received(2).ToEntity();
            //e2Enrollment.Received().ToEntity();
        }


        [TestMethod]
        [ExpectedException(typeof(DlapException), "UpdateEnrollments command failed with code AccessDenied")]
        // ReSharper disable once InconsistentNaming
        public void Check_Response_When_ResponseCode_IS_NOT_OK()
        {
            var response = new DlapResponse { Code = DlapResponseCode.AccessDenied, ResponseXml = new XDocument() };
            new UpdateEnrollments().ParseResponse(response);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "UpdateEnrollments command failed with code AccessDenied")]
        // ReSharper disable once InconsistentNaming
        public void Check_Response_With_Inner_Exception()
        {
            var response = new DlapResponse(XDocument.Parse(@"<response code=""OK""><responses><response code=""OK""/><response code=""DoesNotExist""/></responses></response>"));
            new UpdateEnrollments().ParseResponse(response);
        }

        [TestMethod]
        // ReSharper disable once InconsistentNaming
        public void Check_Success_Response()
        {
            var response = new DlapResponse(XDocument.Parse(@"<response code=""OK""><responses><response code=""OK""/><response code=""OK""/></responses></response>"));
            new UpdateEnrollments().ParseResponse(response);
        }
    }
}