using System;
using System.Linq;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Bfw.Common.Patterns.Unity;
using System.Web.Mvc;
using System.Configuration;
using Bfw.Agilix.Dlap.Session;
using Bdc = Bfw.PX.Biz.DataContracts;
using Bsc = Bfw.PX.Biz.ServiceContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.Response;
using Bfw.Agilix.Dlap.Components.Session;
using System.Collections.Generic;
using Bfw.Agilix.Commands;
using Bfw.Common.Collections;
using Bfw.PXWebAPI.Helpers;
using Bfw.PX.Biz.Services.Mappers;
using System.Xml.Linq;
using Bfw.PXWebAPI.Models.DTO;
using Bfw.PXWebAPI.Mappers;

namespace Bfw.PXWebAPI.Controllers.Tests
{
    [TestClass]
    public class DomainControllerTest
    {
        private ISession currentSession;
        private DomainController domainController;
        private IApiDomainActions domainActions;
        private const string DOMAIN_NAME = "My Domain Test";
        private const string REFERENCE = "888888";

        public DomainControllerTest()
        {
            var dummyObject = new ThreadSessionManager(null, null); //to force MsTest to copy Bfw.Agilix.Dlap.Components to output directory
            ConfigureServiceLocator();
            InitializeSessionManager();
            domainController = ServiceLocator.Current.GetInstance<DomainController>();
            domainActions = ServiceLocator.Current.GetInstance<IApiDomainActions>();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            //make sure all domains with the test userspace are deleted before running a test
            var userspace = String.Format("{0}{1}", ConfigurationManager.AppSettings["Onyx"], REFERENCE);
            var cmd = new GetRawResponse()
            {
                Parameters = new Dictionary<string, object>
                {
                    { "cmd", "listdomains" },
                    { "domainid", "0" },
                    { "query", String.Format("/userspace='{0}'", userspace) }
                }
            };
            currentSession.ExecuteAsAdmin(cmd);
            if (cmd.XmlResponse != null)
            {
                var allDomains = cmd.XmlResponse.Descendants("domain");
                foreach (var node in allDomains)
                {
                    var domain = new DomainDto { Id = node.Attribute("id").Value, Name = node.Attribute("name").Value, Reference = REFERENCE, Userspace = userspace };
                    DeleteDomain(domain);
                }
            }
        }

        [TestMethod]
        public void GetDetails_ExistingDomain()
        {
            DomainDto domain = domainActions.CreateDomain(DOMAIN_NAME, REFERENCE).ToDomainDto();
            try
            {
                var response = domainController.Details(domain.Id);
                Assert.IsNotNull(response.results);
                domain = response.results;
                Assert.AreEqual(REFERENCE, domain.Reference);
                Assert.AreEqual(DOMAIN_NAME, domain.Name);
            }
            finally
            {
                DeleteDomain(domain);
            }
        }

        [TestMethod]
        public void CheckandCreateDomainByInstitution_NonExistingDomain()
        {
            DomainDto domain = null;
            try
            {
                var response = domainController.CheckandCreateDomainByInstitution(REFERENCE);
                Assert.IsNotNull(response.results);
                domain = response.results;
                Assert.AreEqual(REFERENCE, domain.Reference);
                Assert.AreEqual(DOMAIN_NAME, domain.Name);
            }
            finally
            {
                DeleteDomain(domain);
            }
        }

        [TestMethod]
        public void CheckandCreateDomainByInstitution_ExistingDomain()
        {
            DomainDto domain = null;
            try
            {
                domain = domainActions.CreateDomain(DOMAIN_NAME, REFERENCE).ToDomainDto();
                var response = domainController.CheckandCreateDomainByInstitution(REFERENCE);
                Assert.IsNotNull(response.results);
                var retrievedDomain = response.results;
                Assert.AreEqual(domain.Id, retrievedDomain.Id);
                Assert.AreEqual(domain.Reference, retrievedDomain.Reference);
            }
            finally
            {
                DeleteDomain(domain);
            }
        }

        private void ConfigureServiceLocator()
        {
            var locator = new Bfw.Common.Patterns.Unity.UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);

            var container = locator.Container;
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);

            DependencyResolver.SetResolver(new UnityDependencyResolver(locator.Container));
        }

        private void InitializeSessionManager()
        {
            var userName = ConfigurationManager.AppSettings["DlapUserName"];
            var password = ConfigurationManager.AppSettings["DlapUserPassword"];
            var userId = ConfigurationManager.AppSettings["DlapUserId"];
            var sessionManager = ServiceLocator.Current.GetInstance<ISessionManager>();
            currentSession = sessionManager.StartNewSession(userName, password, false, userId);
            sessionManager.CurrentSession = currentSession;
        }

        private void DeleteDomain(DomainDto domain)
        {
            if (domain != null)
            {
                var dataDomain = new Adc.Domain { Id = domain.Id, Name = domain.Name, Reference = domain.Reference, Userspace = domain.Userspace };
                var cmd = new DeleteDomain { Domain = dataDomain };
                currentSession.ExecuteAsAdmin(cmd);
            }
        }
    }
}