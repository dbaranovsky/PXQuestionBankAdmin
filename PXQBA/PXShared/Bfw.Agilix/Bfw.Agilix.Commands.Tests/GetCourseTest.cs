using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Collections.Generic;
using Bfw.Common.Collections;
using TestHelper;
using System.Xml.Linq;
using System.IO;
namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetCourseTest
    {
        private GetCourse courses;
        [TestInitialize]
        public void TestInitialize()
        {
            this.courses = new GetCourse();
            this.courses.SearchParameters = new CourseSearch
            {
                DomainId = "9999",
                Title = "Test Course",
                Query = "sample query"
            };
        }

       
        [TestMethod]
        public void GetCourse_Request_Should_Be_For_All_Items()
        {
            DlapRequest request = this.courses.ToRequest();

            Assert.AreEqual("9999", request.Parameters["domainid"]);
            Assert.IsNotNull(request.Parameters["query"]);
        }
        [TestMethod]
        public void GetCourse_Request_Should_Use_GetCourse_Command()
        {
            this.courses.SearchParameters = new CourseSearch { CourseId="192837"};
            var request = this.courses.ToRequest();
            Assert.AreEqual("getcourse", request.Parameters["cmd"]);
            Assert.AreEqual("192837", request.Parameters["courseid"]);
        }
        [TestMethod]
        public void GetCourse_ToRequest_Check_Command_Name()
        {
            TestInitialize();
            var dlapRequest = this.courses.ToRequest();
            Assert.AreEqual("getcourselist", dlapRequest.Parameters["cmd"]);
        }


        [TestMethod]
        [ExpectedException(typeof(DlapException), "Getcourse command failed with code AccessDenied")]
        public void ParseResponse_AccessDenied_ExceptionThrown()
        {
            var messages = XDocument.Parse(Helper.GetContent(Entity.Course));
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.AccessDenied, ResponseXml = messages };
            this.courses.ParseResponse(dlapResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void GetCourse_ParseResponse_ResponseNotOK_ExceptionThrown()
        {
            var messages = XDocument.Parse(Helper.GetContent(Entity.Course));
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.BadRequest, ResponseXml = messages };
            this.courses.ParseResponse(dlapResponse);
        }

        [TestMethod]
        public void GetCourse_Response_Return_Course()
        {

            var messages = XDocument.Parse(Helper.GetContent(Entity.Course));
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = messages };
            this.courses.ParseResponse(dlapResponse);
            Assert.IsNotNull(this.courses);
        }



    }
}
