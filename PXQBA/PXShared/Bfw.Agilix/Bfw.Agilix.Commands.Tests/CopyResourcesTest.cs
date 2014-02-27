using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class CopyResourcesTest
    {
        private CopyResources cmdCopyResource = null;
        private string xmlResponse = "";
        public CopyResourcesTest()
        {
            cmdCopyResource = new CopyResources()
            {
                SourceEntityId = "8841",
                DestEntityId = "9067"
            };

            xmlResponse = @"<responses>
                          <response code='OK'>
                                 <submission version='1'/>
                                 </response>                                        
                                    </responses>";
        }

        [TestMethod]
        public void CopyResourcesAction_ToRequestTest()
        {
            Assert.IsNotNull(cmdCopyResource.SourceEntityId);
            Assert.IsNotNull(cmdCopyResource.DestEntityId);
        }

        [TestMethod]
        public void CopyResourcesAction_ParseResponseTest()
        {
            var response = new Dlap.DlapResponse { Code = Dlap.DlapResponseCode.OK, ResponseXml = XDocument.Parse(xmlResponse) };
            cmdCopyResource.ParseResponse(response);
            Assert.IsNotNull(response);                                                                                                                
        }
    }
}
