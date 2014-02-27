using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bfw.Agilix.Dlap;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class PutRawItemTest
    {
        private PutRawItem cmd = null;
        private void InitCommand()
        {
            XDocument doc = XDocument.Parse("<requests>  <item entityid='4378' itemid='Assignment12'>    <data>      <type>Assignment</type>      <parent>DEFAULT</parent>      <sequence>a</sequence>      <title>Assignment 12</title>      <href>Assets/assignment12.htm</href>    </data>  </item> </requests>");
            cmd = new PutRawItem
            {
                ItemDoc = doc
            };
        }

        [TestMethod]
        public void PutRawItemAction_ToRequestTest()
        {
            this.InitCommand();

            var request = cmd.ToRequest();
            Assert.IsNotNull(request);
        }

        [TestMethod]
        public void PutRawItemAction_ParseResponseTest()
        {
            this.InitCommand();

            var sResponse = "<response code='OK'>  <responses>    <response code='OK'/>  </responses> </response>";
            XDocument doc = XDocument.Parse(sResponse);
            var dlapResponse = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = doc, ContentType = "stream" };

            cmd.ParseResponse(dlapResponse);

            Assert.IsNotNull(dlapResponse);
        }
    }
}
