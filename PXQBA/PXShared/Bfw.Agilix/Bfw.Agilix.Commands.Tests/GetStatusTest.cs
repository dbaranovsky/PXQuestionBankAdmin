using System;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetStatusTest
    {
        private GetStatus statusCmd;

        [TestInitialize]
        public void InitializeTest()
        {
            statusCmd = new GetStatus();
        }

        [TestMethod]
        public void GetStatusAction_ToRequestTest()
        {
            var request = statusCmd.ToRequest();
            Assert.IsNotNull(request);
            Assert.IsTrue(request.Mode == DlapRequestMode.Single);
            Assert.IsTrue(request.Type == DlapRequestType.Get);
            Assert.IsNotNull(request.Parameters);
            Assert.AreEqual(request.Parameters["cmd"], "getstatus");
        }

        [TestMethod]
        public void GetStatusAction_ParseResponseOKTest()
        {
            DlapResponse response = new DlapResponse { Code = DlapResponseCode.OK, ResponseXml = new XDocument() };
            statusCmd.ParseResponse(response);
            Assert.IsNotNull(statusCmd.Status);
        }
    }
}
