using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using TestHelper;
using System.Xml.Linq;
namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class ListCoursesTest
    {
        private ListCourses _listCourses;
        private const string Query = "/meta-bfw_course_type='Eportfolio'";
        private const string Show = "current";
        private const string Text = "my course";
        private const string Subtype = "C";
        private const string Domain = "0";
        private const string Cmd = "listcourses";
        private const int Limit = 0;

        [TestInitialize]
        public void TestInitialize()
        {
            _listCourses = new ListCourses();
            _listCourses.SearchParameters = new CourseSearch
            {
                DomainId = Domain,
                Query = Query,
                Limit = Limit
            };
        }

        [TestCategory("ListCourses"), TestMethod]
        public void ListCoursesTest_ToRequest_ExpectSuccess()
        {
            DlapRequest request = _listCourses.ToRequest();

            Assert.AreEqual(Domain, request.Parameters["domainid"]);
            Assert.AreEqual(Limit, request.Parameters["limit"]);
            Assert.AreEqual(Query, request.Parameters["query"]);
            Assert.AreEqual(Cmd, request.Parameters["cmd"]);

        }

        [TestCategory("ListCourses"), TestMethod]
        public void ListCoursesTest_SetsDefault_SearchParameters()
        {
            _listCourses.SearchParameters = new CourseSearch
            {
                Query = Query             
            };

            DlapRequest request = _listCourses.ToRequest();

            Assert.AreEqual(Domain, request.Parameters["domainid"]);
            Assert.AreEqual(Limit, request.Parameters["limit"]);
            Assert.AreEqual(Query, request.Parameters["query"]);
            Assert.AreEqual(Cmd, request.Parameters["cmd"]);
        }

        [TestCategory("ListCourses"), TestMethod]
        public void ListCoursesTest_ToRequest_CheckSearchParameters()
        {

            _listCourses.SearchParameters = new CourseSearch
            {
                Query = Query,
                Show = Show,
                Text = Text,
                Subtype = Subtype
            };

            DlapRequest request = _listCourses.ToRequest();

            Assert.AreEqual(Show, request.Parameters["show"]);
            Assert.AreEqual(Text, request.Parameters["text"]);
            Assert.AreEqual(Subtype, request.Parameters["subtype"]);
            Assert.AreEqual(Cmd, request.Parameters["cmd"]);
        }

        [TestCategory("ListCourses"), TestMethod]
        [ExpectedException(typeof(DlapException), "ListCourses command failed with code AccessDenied")]
        public void ParseResponse_AccessDenied_ExceptionThrown()
        {
            var messages = XDocument.Parse(Helper.GetContent(Entity.ListCourses));
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.AccessDenied, ResponseXml = messages };
            _listCourses.ParseResponse(dlapResponse);
        }

        [TestCategory("ListCourses"), TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void ParseResponse_ResponseNotOK_ExceptionThrown()
        {
            var messages = XDocument.Parse(Helper.GetContent(Entity.ListCourses));
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.BadRequest, ResponseXml = messages };
            _listCourses.ParseResponse(dlapResponse);
        }

        [TestCategory("ListCourses"), TestMethod]
        public void ListCourses_Response_Return_Course()
        {
            var messages = XDocument.Parse(Helper.GetContent(Entity.ListCourses));
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = messages };
            _listCourses.ParseResponse(dlapResponse);
            Assert.IsNotNull(_listCourses.Courses);
        }

        [TestCategory("ListCourses"), TestMethod]
        public void ListCourses_Response_Returns_Two_Course()
        {
            // ListCourses.xml files should have 2 courses.
            var messages = XDocument.Parse(Helper.GetContent(Entity.ListCourses));
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = messages };
            _listCourses.ParseResponse(dlapResponse);
            Assert.AreEqual(2,_listCourses.Courses.Count());
        }

    }
}
