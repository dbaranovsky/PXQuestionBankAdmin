using System;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class GetDomainTest
    {
        private GetDomain _getDomain;

        [TestInitialize]
        public void TestInitialize()
        {
            _getDomain = new GetDomain();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Throw_Exception_If_DomainId_Is_Null()
        {
            _getDomain.DomainId = null;
            _getDomain.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Throw_Exception_If_DomainId_Is_Empty_String()
        {
            _getDomain.DomainId = "";
            _getDomain.ToRequest();
        }

        [TestMethod]
        public void DlapRequest_Type_Is_A_Get_Request()
        {
            _getDomain.DomainId = "11111";
            DlapRequest request = _getDomain.ToRequest();
            Assert.AreEqual(DlapRequestType.Get, request.Type);
        }

        [TestMethod]
        public void DlapRequest_Command_Is_getdomain()
        {
            _getDomain.DomainId = "11111";
            DlapRequest request = _getDomain.ToRequest();
            Assert.AreEqual("getdomain", request.Parameters["cmd"]);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void Throw_Exception_If_Response_Code_Is_Error()
        {
            DlapResponse response = new DlapResponse() { Code = DlapResponseCode.Error };

            _getDomain.ParseResponse(response);
        }

        [TestMethod]
        public void Verify_ParseResponse_Sets_Correct_Domain_Id()
        {
            const string fakeDomainXmlResponse = @"<domain domainid='11111' name='Baruch College CUNY' userspace='onyx102584' reference='102584'>
                                                        <data></data>
                                                   </domain>";
            DlapResponse response = new DlapResponse() {Code = DlapResponseCode.OK};
            response.ResponseXml = XDocument.Parse(fakeDomainXmlResponse);
            
            _getDomain.ParseResponse(response);

            Assert.AreEqual("11111", _getDomain.Domain.Id);
        }
    }
}
