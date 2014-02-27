using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using System.Xml.Linq;
using System.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetUserEnrollmentListTest
    {
        private GetUserEnrollmentList getUserEnrollmentList;

        [TestInitialize]
        public void TestInitialize()
        {
            this.getUserEnrollmentList = new GetUserEnrollmentList();
        }

        [TestMethod]
        public void GetUserEnrollmentListTest_Request_Type_Should_Be_Get()
        {
            getUserEnrollmentList.SearchParameters = new EntitySearch() { CourseId = "99999" };
            var request = getUserEnrollmentList.ToRequest();
            Assert.AreEqual(request.Type, DlapRequestType.Get);
        }

        [TestMethod]
        public void GetUserEnrollmentListTest_Request_Parameter_Should_Have_Command_getuserenrollmentlist2()
        {
            getUserEnrollmentList.SearchParameters = new EntitySearch() { CourseId = "99999" };
            var request = getUserEnrollmentList.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "getuserenrollmentlist2");
        }

        [TestMethod]
        public void GetUserEnrollmentListTest_Request_Entityid_Parameter_Should_Be_Same_As_CourseId()
        {
            getUserEnrollmentList.SearchParameters = new EntitySearch() { CourseId = "99999" };
            var request = getUserEnrollmentList.ToRequest();
            Assert.AreEqual(request.Parameters["entityid"], "99999");
        }


        [TestMethod]
        public void GetUserEnrollmentListTest_Request_Parameters_Has_Same_Values_as_SearchParameter()
        {
            getUserEnrollmentList.SearchParameters = new EntitySearch();
            getUserEnrollmentList.SearchParameters.CourseId = "56789";
            getUserEnrollmentList.SearchParameters.UserId = "12345";
            getUserEnrollmentList.SearchParameters.Query = "testquery";
            getUserEnrollmentList.SearchParameters.Flags = "1000";
            getUserEnrollmentList.AllStatus = true;
            var request = getUserEnrollmentList.ToRequest();
            Assert.AreEqual(request.Parameters["entityid"], "56789");
            Assert.AreEqual(request.Parameters["userid"], "12345");
            Assert.AreEqual(request.Parameters["query"], "testquery");
            Assert.AreEqual(request.Parameters["flags"], "1000");
            Assert.AreEqual(request.Parameters["allstatus"], true);
        }

        [TestMethod]
        public void GetUserEnrollmentListTest_ParseResponse_Users_Should_Have_Single_Enrollment()
        {

            string responseString = string.Format("<response code=\"OK\">" +                    
                        "<enrollment id=\"99999\" userid=\"3454\"></enrollment>" +
                    "</response>");

            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));

            getUserEnrollmentList.ParseResponse(dlapResponse);
            Assert.AreEqual(getUserEnrollmentList.Enrollments.Count(), 1);
        }

        [TestMethod]
        public void GetUserEnrollmentListTest_ParseResponse_Enrollments_Count_Match_With_Response()
        {
            string responseString = string.Format("<response code=\"OK\">" +
                                "<enrollments>" +
                                    "<enrollment id=\"99999\" userid=\"3454\"></enrollment>" +
                                    "<enrollment id=\"88888\" userid=\"1234\"></enrollment>" +
                                "</enrollments>" +
                                "</response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));

            getUserEnrollmentList.ParseResponse(dlapResponse);
            Assert.AreEqual(getUserEnrollmentList.Enrollments.Count(), 2);
        }

        [TestMethod]
        public void GetUserEnrollmentListTest_ParseResponse_Enrollments_Should_Be_Empty_When_AccessDenied()
        {
            string responseString = string.Format("<response code=\"AccessDenied\"></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));

            getUserEnrollmentList.ParseResponse(dlapResponse);
            Assert.AreEqual(getUserEnrollmentList.Enrollments.Count(), 0);
        }

        [TestMethod]
        public void GetUserEnrollmentListTest_ParseResponse_Enrollments_Should_Be_Empty_When_BadRequest()
        {
            string responseString = string.Format("<response code=\"BadRequest\" message=\"Bad ExtendedId\"></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));

            getUserEnrollmentList.ParseResponse(dlapResponse);
            Assert.AreEqual(getUserEnrollmentList.Enrollments.Count(), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(BadDlapResponseException), "GetEntityEnrollmentList expected a response element of 'user' or 'users', but got test instead.")]
        public void GetUserEnrollmentListTest_ParseResponse_Should_Throw_Exception_If_Root_Element_Is_Diffrent()
        {
            string responseString = string.Format("<response code=\"OK\"><test></test></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));
            getUserEnrollmentList.ParseResponse(dlapResponse);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException), "GetEntityEnrollmentList command failed with code Format")]
        public void GetUserEnrollmentListTest_ParseResponse_Should_Throw_Exception_If_Response_Code_Is_Not_Ok()
        {
            string responseString = string.Format("<response code=\"Format\"><user></user></response>");
            DlapResponse dlapResponse = new DlapResponse(XDocument.Parse(responseString));
            getUserEnrollmentList.ParseResponse(dlapResponse);
        }

    }
}
