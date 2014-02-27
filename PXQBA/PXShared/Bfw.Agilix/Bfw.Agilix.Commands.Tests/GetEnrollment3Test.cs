using System;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetEnrollment3Test
    {
        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void ToRequest_Should_Throw_Exception_If_No_EnrollmentId()
        {
            var cmd = new GetEnrollment3()
            {
                Select = DataContracts.EnrollmentSelect.Course | DataContracts.EnrollmentSelect.CourseData |
                  DataContracts.EnrollmentSelect.User | DataContracts.EnrollmentSelect.UserData |
                  DataContracts.EnrollmentSelect.Domain | DataContracts.EnrollmentSelect.DomainData
            };

            var request = cmd.ToRequest();
        }

        [TestMethod]
        public void ToRequest_Should_Create_RequestObject()
        {
            var cmd = new GetEnrollment3()
            {
                EnrollmentId = "1234",
                Select = DataContracts.EnrollmentSelect.Course | DataContracts.EnrollmentSelect.CourseData |
                    DataContracts.EnrollmentSelect.User | DataContracts.EnrollmentSelect.UserData |
                    DataContracts.EnrollmentSelect.Domain | DataContracts.EnrollmentSelect.DomainData
            };

            var request = cmd.ToRequest();

            Assert.AreEqual("query: cmd=getenrollment3&enrollmentid=1234&select=course%2ccourse.data%2cdomain%2cdomain.data%2cuser%2cuser.data:params: [cmd|getenrollment3][enrollmentid|1234][select|course,course.data,domain,domain.data,user,user.data]", 
                request.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void ParseResponse_Should_Throw_Exception_If_Bad_Request()
        {
            var cmd = new GetEnrollment3();

            cmd.ParseResponse(new DlapResponse() { Code = DlapResponseCode.BadRequest });
        }

        [TestMethod]
        public void ParseResponse_Should_Parse_Response()
        {
            var response = XDocument.Parse(Helper.GetContent(Entity.GetEnrollment3));
            var cmd = new GetEnrollment3();

            cmd.ParseResponse(new DlapResponse() { Code = DlapResponseCode.OK, ResponseXml = response });

            Assert.AreEqual("163760", cmd.Enrollments.First().Id);
            Assert.AreEqual("163756", cmd.Enrollments.First().Course.Id);
            Assert.AreEqual("116705", cmd.Enrollments.First().User.Id);
            Assert.AreEqual("66159", cmd.Enrollments.First().Domain.Id);
        }
    }
}
