using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain = Bfw.Agilix.DataContracts.Domain;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetDomainListTest
    {
        private GetDomainList _getDomainList;

        [TestInitialize]
        public void TestInitialize()
        {
            _getDomainList = new GetDomainList();
            _getDomainList.SearchParameters = new Domain();
        }

        [TestMethod]
        public void DlapRequest_Type_Is_A_Get_Request()
        {
            DlapRequest request = _getDomainList.ToRequest();
            Assert.AreEqual(DlapRequestType.Get, request.Type);
        }

        [TestMethod]
        public void DlapRequst_Command_Is_getdomainlist()
        {
            DlapRequest request = _getDomainList.ToRequest();

            Assert.AreEqual("getdomainlist", request.Parameters["cmd"]);
        }
        
        [TestMethod]
        public void Request_Support_Search_By_DomainId()
        {
            _getDomainList.SearchParameters.Id = "11111";
            DlapRequest request = _getDomainList.ToRequest();
            Assert.AreEqual(request.Parameters["domainid"],"11111");
        }

        [TestMethod]
        public void Request_Support_Search_By_Name()
        {
            _getDomainList.SearchParameters.Name = "SomeDomainName";
            DlapRequest request = _getDomainList.ToRequest();
            Assert.AreEqual(request.Parameters["name"], "SomeDomainName");
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_Response_Code_Is_Error()
        {
            DlapResponse response = new DlapResponse() { Code = DlapResponseCode.Error };

            _getDomainList.ParseResponse(response);
        }

        [TestMethod]
        public void Verify_Response_Contain_Valid_Domain_List()
        {
            const string FakeDomainListXML = @"
<response code='OK'>
  <domains>
    <domain domainid='6649' name='BFW' userspace='bfw' reference='' guid='6504e991-3b19-4c2e-84dd-46a45d10fd76' creationdate='2011-06-17T19:12:58.927Z' flags='0' />
    <domain domainid='2118' name='BFWContentDev' userspace='bfwcontentdev' reference='' guid='b899d42c-4352-4d93-8e6f-9c825b065fef' creationdate='2010-11-23T20:12:56.5Z' flags='0' />
  </domains>
</response>";

            DlapResponse response = new DlapResponse();
            response.ResponseXml = XDocument.Parse(FakeDomainListXML);

            _getDomainList.ParseResponse(response);

            Assert.AreEqual(2, _getDomainList.Domains.Count());
        }

    }
}
