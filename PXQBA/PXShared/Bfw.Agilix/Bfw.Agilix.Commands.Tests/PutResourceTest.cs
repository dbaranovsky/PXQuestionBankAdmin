using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Commands;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class PutResourceTest
    {
        private PutResource cmd = null;
        private void InitCommand()
        {
            XDocument doc = XDocument.Parse("<requests>  <item entityid='4378' itemid='Assignment12'>    <data>      <type>Assignment</type>      <parent>DEFAULT</parent>      <sequence>a</sequence>      <title>Assignment 12</title>      <href>Assets/assignment12.htm</href>    </data>  </item> </requests>");
            cmd = new PutResource
            {
                Resource = new DataContracts.Resource() { ContentType = "xml", EntityId = "112652", CreationDate = DateTime.Today, Url = "content/test", Status = DataContracts.ResourceStatus.Normal }
            };
        }

        [TestMethod]
        public void PutResourceAction_ToRequestTest()
        {
            this.InitCommand();

            var request = cmd.ToRequest();
            Assert.IsNotNull(request);
        }

        [TestMethod]
        public void PutResourceAction_ParseResponseTest()
        {
            this.InitCommand();

            var sResponse = "<response code='OK'>  <resource version='1'/></response>";
            XDocument doc = XDocument.Parse(sResponse);
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = doc, ContentType = "stream" };

            cmd.ParseResponse(dlapResponse);

            Assert.IsNotNull(dlapResponse);
        }
    }
}
