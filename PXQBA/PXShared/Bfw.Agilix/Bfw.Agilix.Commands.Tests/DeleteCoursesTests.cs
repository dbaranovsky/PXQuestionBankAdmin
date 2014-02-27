using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    /// <summary>
    /// Summary description for DeleteCoursesTests
    /// </summary>
    [TestClass]
    public class DeleteCoursesTests
    {
        public DeleteCoursesTests()
        {

        }

        [TestCategory("DeleteCoursesTests"), TestMethod]
        [ExpectedException(typeof(DlapException), "Cannot Delete a Course request if there are not Courses in the Courses collection")]
        public void DeleteCoursesTests_HasNoCourse_RequestError()
        {
            DeleteCourses DeleteCourses = new DeleteCourses();
            DeleteCourses.Clear();
            DlapRequest request = DeleteCourses.ToRequest();

        }

        [TestCategory("DeleteCoursesTests"), TestMethod]
        public void DeleteCoursesTests_HasItem_RequestOk()
        {
            List<Course> courses = new List<Course> { new Course { Id = "someCourseDNE1" } };
            DeleteCourses deleteCourses = new DeleteCourses();
            deleteCourses.Add(courses);
            deleteCourses.Add(new Course { Id = "someCourseDNE2" });
            DlapRequest request = deleteCourses.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "deletecourses");
        }

        [TestCategory("DeleteCoursesTests"), TestMethod]
        [ExpectedException(typeof(DlapException), "DeleteCoursesTests request failed with response code 'NoAuthentication'.")]
        public void DeleteCoursesTests_NoAuthentication_ResponseError()
        {

            string responseString = "<response code=\"NoAuthentication\" message=\"No Authentication: action='deleteitems'\">" +
                                    "<detail>GoCourseServer.NoAuthenticationException: No Authentication: action='deleteitems'    at GoCourseServer.DlapContext.RequireAuthentication()    at GoCourseServer.Dlap.DlapCommand.Invoke(DlapContext dlapContext)    at GoCourseServer.Dlap.ProcessRequest(HttpContext context) agent='Firefox21.0' url='http://dev.dlap.bfwpub.com/Dlap.ashx?deleteitems' site='dlap' method='POST' referer='http://dev.dlap.bfwpub.com/CallMethod.aspx' process='2284'" +
                                    "</detail> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            DeleteCourses deleteCourses = new DeleteCourses();
            deleteCourses.ParseResponse(response);
        }

        [TestCategory("DeleteCoursesTests"), TestMethod]
        public void DeleteCoursesTests_CourseExist_ResponseOk()
        {
            string responseString = "<response code=\"OK\"><responses><response code=\"OK\" /></responses></response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));

            DeleteCourses deleteCourses = new DeleteCourses();
            deleteCourses.Add(new Course { Id = "someCourseDNE2" });
            deleteCourses.ParseResponse(response);
            Assert.AreEqual(deleteCourses.Failures.Count(), 0);

        }

        [TestCategory("DeleteCoursesTests"), TestMethod]
        public void DeleteCoursesTests_ItemNotExist_ResponseError()
        {
            string responseString = "<response code=\"OK\">   <responses>     <response code=\"BadRequest\" message=\"The specified entity id (asdf) is not valid! Parameter 'entityid'.\">" +
                                    "<detail>GoCourseServer.BadRequestException: The specified entity id (asdf) is not valid! Parameter 'entityid'." +
                                    "at GoCourseServer.EntityRef.TryParse(DlapContext context, Char entityType, XPathNavigator node, String parameterName)" +
                                    "at GoCourseServer.EntityRef.Parse(DlapContext context, Char entityType, XPathNavigator node, String parameterName)" +
                                    "at GoCourseServer.AnnouncementRequestHandler.DeleteCoursesTests(DlapContext context) agent='Firefox21.0' url='http://dev.dlap.bfwpub.com/Dlap.ashx?DeleteCoursesTests' site='dlap' method='POST' referer='http://dev.dlap.bfwpub.com/CallMethod.aspx' process='2284' userid='7'" +
                                    "</detail>     </response>   </responses> </response>";
            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));

            DeleteCourses deleteCourses = new DeleteCourses();
            deleteCourses.Add(new Course { Id = "someCourseDNE2" });
            deleteCourses.ParseResponse(response);
            Assert.AreEqual(deleteCourses.Failures.Count(), 1);

        }
    }
}
