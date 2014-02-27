using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    /// <summary>
    /// Summary description for DeleteGroupsTests
    /// </summary>
    [TestClass]
    public class DeleteGroupsTests
    {
        public DeleteGroupsTests()
        {

        }

        [TestCategory("DeleteGroupsTests"), TestMethod]
        public void DeleteGroupsTests_HasNoGroup_RequestOk()
        {
            DeleteGroups deleteGroups = new DeleteGroups();
            DlapRequest request = deleteGroups.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "deletegroups");

        }

        [TestCategory("DeleteGroupsTests"), TestMethod]
        public void DeleteGroupsTests_HasItem_RequestOk()
        {
            DeleteGroups deleteGroups = new DeleteGroups { GroupIds = new List<int> { 923939 } };
            DlapRequest request = deleteGroups.ToRequest();
            Assert.AreEqual(request.Parameters["cmd"], "deletegroups");
        }

        [TestCategory("DeleteGroupsTests"), TestMethod]
        [ExpectedException(typeof(DlapException), "DeleteGroupsTests request failed with response code 'NoAuthentication'.")]
        public void DeleteGroupsTests_NoAuthentication_ResponseError()
        {

            string responseString = "<response code=\"NoAuthentication\" message=\"No Authentication: action='deleteitems'\">" +
                                    "<detail>GoCourseServer.NoAuthenticationException: No Authentication: action='deleteitems'    at GoCourseServer.DlapContext.RequireAuthentication()    at GoCourseServer.Dlap.DlapCommand.Invoke(DlapContext dlapContext)    at GoCourseServer.Dlap.ProcessRequest(HttpContext context) agent='Firefox21.0' url='http://dev.dlap.bfwpub.com/Dlap.ashx?deleteitems' site='dlap' method='POST' referer='http://dev.dlap.bfwpub.com/CallMethod.aspx' process='2284'" +
                                    "</detail> </response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            DeleteGroups deleteGroups = new DeleteGroups();
            deleteGroups.ParseResponse(response);
        }

        [TestCategory("DeleteGroupsTests"), TestMethod]
        public void DeleteGroupsTests_GroupExist_ResponseOk()
        {
            string responseString = "<response code=\"OK\"><responses><response code=\"OK\" /></responses></response>";

            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));

            DeleteGroups DeleteGroups = new DeleteGroups { GroupIds = new List<int> { 923939 } };
            DeleteGroups.ParseResponse(response);
            Assert.AreEqual(response.Code, DlapResponseCode.OK);

        }

    }
}
