using System;
using System.Xml.Linq;
using Bfw.Agilix.Dlap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bfw.Agilix.Commands.Tests
{
    [TestClass]
    public class CreateDomainsTest
    {
        private CreateDomains _cmd;
        private string _parentDomainId;

        [TestInitialize]
        public void Setup()
        {
            _parentDomainId = "6650"; //Baruch?
            _cmd = new CreateDomains(_parentDomainId);
        }

        [TestMethod]
        public void CreateDomainsAction_CreateObject_ParentIdIsSetToArg()
        {
            Assert.AreEqual<string>(_cmd.ParentId, _parentDomainId);
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateDomainsAction_CreateDlapRequestWithoutDomain_ThrowsDlapException()
        {
            DlapRequest request = _cmd.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateDomainsAction_CreateDlapWithDomainWithoutDomainName_ThrowsDlapException()
        {
            _cmd.Domain = new DataContracts.Domain() { Userspace = "blahspace" };
            DlapRequest request = _cmd.ToRequest();
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateDomainsAction_CreateDlapWithDomainWithoutDomainUserspace_ThrowsDlapException()
        {
            _cmd.Domain = new DataContracts.Domain() { Name = "Blah" };
            DlapRequest request = _cmd.ToRequest();
        }

        [TestMethod]
        public void CreateDomainsAction_CreateDlapRequest_RequestTypeIsPost()
        {
            _cmd.Domain = new DataContracts.Domain() { Name = "Blah", Userspace = "blahspace" };
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<DlapRequestType>(request.Type, DlapRequestType.Post);
        }

        [TestMethod]
        public void CreateDomainsAction_CreateDlapRequest_RequestModeIsBatch()
        {
            _cmd.Domain = new DataContracts.Domain() { Name = "Blah", Userspace = "blahspace" };
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<DlapRequestMode>(request.Mode, DlapRequestMode.Batch);
        }

        [TestMethod]
        public void CreateDomainsAction_CreateDlapRequest_ParametersInitialized()
        {
            _cmd.Domain = new DataContracts.Domain() { Name = "Blah", Userspace = "blahspace" };
            DlapRequest request = _cmd.ToRequest();
            Assert.IsNotNull(request.Parameters);
        }

        [TestMethod]
        public void CreateDomainsAction_CreateDlapRequest_ParametersHasCmd()
        {
            _cmd.Domain = new DataContracts.Domain() { Name = "Blah", Userspace = "blahspace" };
            DlapRequest request = _cmd.ToRequest();
            Assert.AreEqual<string>(request.Parameters["cmd"].ToString(), "createdomains");
        }

        [TestMethod]
        [ExpectedException(typeof(DlapException))]
        public void CreateDomainsAction_ParseDlapResonseWithError_ThrowsDlapException()
        {
            _cmd.Domain = new DataContracts.Domain() { Name = "Blah", Userspace = "blahspace" };
            _cmd.ParseResponse(new DlapResponse() { Code = DlapResponseCode.Error });
        }

        [TestMethod]
        public void CreateDomainsAction_ParseDlapResonse_DomainIdMatchesReturnId()
        {
            var responseString = @"<response code=""OK"">
  <responses>
    <response code=""OK"">
      <domain domainid=""4879""/>
    </response>
  </responses>
</response>";
            _cmd.Domain = new DataContracts.Domain() { Name = "Blah", Userspace = "blahspace" };
            DlapResponse response = new DlapResponse(XDocument.Parse(responseString));
            _cmd.ParseResponse(response);

            Assert.AreEqual<string>(_cmd.Domain.Id, "4879");
        }
    }
}
