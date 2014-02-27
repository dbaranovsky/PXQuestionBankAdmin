using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    /// <summary>
    /// Summary description for DeleteAnnouncements
    /// </summary>
    [TestClass]
    public class DeleteAnnouncementsTests
    {
        public DeleteAnnouncementsTests()
        {
            
        }

        [TestCategory("DeleteAnnouncements"), TestMethod]
        public void DeleteAnnouncements_HasNoItem_RequestOK()
        {
            DeleteAnnouncements deleteAnnouncements = new DeleteAnnouncements();
            DlapRequest request = deleteAnnouncements.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "deleteannouncements");
            
        }

        [TestCategory("DeleteAnnouncements"), TestMethod]
        public void DeleteAnnouncements_HasItem_RequestOk()
        {
            List<Announcement> announcements = new List<Announcement> { new Announcement{ EntityId = "someCourseDNE", Path="somePathDNE"} };
            DeleteAnnouncements deleteAnnouncements = new DeleteAnnouncements { Announcements = announcements };
            DlapRequest request = deleteAnnouncements.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "deleteannouncements");
        }

        [TestCategory("DeleteAnnouncements"), TestMethod]
        [ExpectedException(typeof(DlapException), "DeleteAnnouncements request failed with response code 'NoAuthentication'.")]
        public void DeleteAnnouncements_NoAuthentication_ResponseError()
        {

            string responseString = "<response code=\"NoAuthentication\" message=\"No Authentication: action='deleteitems'\">" +
                                    "<detail>GoCourseServer.NoAuthenticationException: No Authentication: action='deleteitems'    at GoCourseServer.DlapContext.RequireAuthentication()    at GoCourseServer.Dlap.DlapCommand.Invoke(DlapContext dlapContext)    at GoCourseServer.Dlap.ProcessRequest(HttpContext context) agent='Firefox21.0' url='http://dev.dlap.bfwpub.com/Dlap.ashx?deleteitems' site='dlap' method='POST' referer='http://dev.dlap.bfwpub.com/CallMethod.aspx' process='2284'" +
                                    "</detail> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            DeleteAnnouncements deleteAnnouncements = new DeleteAnnouncements();
            deleteAnnouncements.ParseResponse(response);
        }

        [TestCategory("DeleteAnnouncements"), TestMethod]
        public void DeleteAnnouncements_ItemExist_ResponseOk()
        {
            string responseString = "<response code=\"OK\"><responses><response code=\"OK\" /></responses></response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));

            List<Announcement> announcements = new List<Announcement> { new Announcement { EntityId = "109528", Path = "somePathDNE" } };
            DeleteAnnouncements deleteAnnouncements = new DeleteAnnouncements { Announcements = announcements };
            deleteAnnouncements.ParseResponse(response);
            Assert.AreEqual(deleteAnnouncements.Failures.Count(), 0);

        }

        [TestCategory("DeleteAnnouncements"), TestMethod]
        public void DeleteAnnouncements_ItemNotExist_ResponseError()
        {
            string responseString = "<response code=\"OK\">   <responses>     <response code=\"BadRequest\" message=\"The specified entity id (asdf) is not valid! Parameter 'entityid'.\">"+
                                    "<detail>GoCourseServer.BadRequestException: The specified entity id (asdf) is not valid! Parameter 'entityid'."+
                                    "at GoCourseServer.EntityRef.TryParse(DlapContext context, Char entityType, XPathNavigator node, String parameterName)"+
                                    "at GoCourseServer.EntityRef.Parse(DlapContext context, Char entityType, XPathNavigator node, String parameterName)"+
                                    "at GoCourseServer.AnnouncementRequestHandler.DeleteAnnouncements(DlapContext context) agent='Firefox21.0' url='http://dev.dlap.bfwpub.com/Dlap.ashx?deleteannouncements' site='dlap' method='POST' referer='http://dev.dlap.bfwpub.com/CallMethod.aspx' process='2284' userid='7'"+
                                    "</detail>     </response>   </responses> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));

            List<Announcement> announcements = new List<Announcement> { new Announcement { EntityId = "someCourseDNE", Path = "somePathDNE" } };
            DeleteAnnouncements deleteAnnouncements = new DeleteAnnouncements { Announcements = announcements };
            deleteAnnouncements.ParseResponse(response);
            Assert.AreEqual(deleteAnnouncements.Failures.Count(), 1);

        }
    }
}
