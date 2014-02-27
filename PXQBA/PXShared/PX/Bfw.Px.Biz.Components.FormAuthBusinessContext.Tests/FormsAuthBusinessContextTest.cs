using System.IO;
using System.Web;
using Bfw.Common.SSO;
using Bfw.PX.Biz.Components.FormsAuthBusinessContext;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using BFW.RAg;
using Microsoft.Practices.ServiceLocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Logging;
using Bfw.Common.Caching;
using System.Configuration;

using NSubstitute;

namespace Bfw.Px.Biz.Components.FormAuthBusinessContext.Tests
{
    
    
    /// <summary>
    ///This is a test class for FormsAuthBusinessContextTest and is intended
    ///to contain all FormsAuthBusinessContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FormsAuthBusinessContextTest
    {


        private TestContext testContextInstance;
        private ISessionManager _sm;
        private ILogger _logger;
        private ITraceManager _tracer;
        private ICacheProvider _cacheProvider;
        private IRAServices _raServices;


        /// <summary>
        /// Public class to expose the InitializePermissions protected function
        /// </summary>
        public class TestFormsAuthContext : FormsAuthBusinessContext
        {
            public TestFormsAuthContext(ISessionManager sm, ILogger logger, ITraceManager tracer,
                ICacheProvider cacheProvider, IRAServices _raServices)
                : base(sm, logger, tracer, cacheProvider, _raServices)
            {
                ProductCourseId = "productCourseId";
                SSOData = new SSOData()
                {
                    UserId = "user1"
                };
                CurrentURL = @"http://www.myurl.com/product/title/1234";
            }

            public void PublicInitializePermissions()
            {
                InitializePermissions();
            }

        }
        private TestFormsAuthContext _formsContext;
        private IBusinessContext _context;
        private IServiceLocator _serviceLocator;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestInitialize()]
        public void MyTestInitialize()
        {
            _sm = Substitute.For<ISessionManager>();
            _logger = Substitute.For<ILogger>();
            _tracer = Substitute.For<ITraceManager>();
            _cacheProvider = Substitute.For<ICacheProvider>();
            _raServices = Substitute.For<IRAServices>();

            _context = Substitute.For<IBusinessContext>();
            _serviceLocator = Substitute.For<IServiceLocator>();
            _serviceLocator.GetInstance<IBusinessContext>().Returns(_context);
            ServiceLocator.SetLocatorProvider(() => _serviceLocator);


            System.Configuration.ConfigurationManager.AppSettings["AllowedSubdomains"] = "www";

            _formsContext = new TestFormsAuthContext(_sm, _logger, _tracer, _cacheProvider, _raServices);
            
        }


        /// <summary>
        ///A test for FormsAuthBusinessContext to ensure that user data for users who are not entitled is not cached
        ///</summary>
        [TestMethod()]
        public void FormsAuthBusinessContext_Does_Not_Cache_NotEntitled()
        {

            var cacheKey = "user1:1234";
            var cacheFetched = false;
            _cacheProvider.Fetch(cacheKey).Returns(null).AndDoes(a => cacheFetched = true);
            _raServices.GetAccessLevelByBaseUrl("user1","http://www.myurl.com/product/title/1234").Returns(
                new RAAccessInfo()
                {
                    AccessLevel = new UserAccessLevel()
                    {
                        LevelOfAccess = 10,
                        ExpirationDate = DateTime.Now.AddYears(1)
                    }
                }
            );


            var cacheStored = false;
            _cacheProvider.When(a => a.Store(cacheKey, Arg.Any<SiteUserData>(), Arg.Any<CacheSettings>()))
                .Do(a => cacheStored = true);

           _formsContext.PublicInitializePermissions();


            Assert.IsTrue(cacheFetched);
            Assert.IsFalse(cacheStored);
            Assert.AreEqual(_formsContext.AccessLevel, AccessLevel.None);
            Assert.AreEqual(_formsContext.AccessType, AccessType.Anonymous);
        }

        /// <summary>
        ///A test for FormsAuthBusinessContext to ensure that user data for users who are entitled IS cached
        /// 
        ///</summary>
        [TestMethod()]
        public void FormsAuthBusinessContext_Cache_Entitled()
        {

            var cacheKey = "user1:1234";
            var cacheFetched = false;
            _cacheProvider.Fetch(cacheKey).Returns(null).AndDoes(a => cacheFetched = true);
            _raServices.GetAccessLevelByBaseUrl("user1", "http://www.myurl.com/product/title/1234").Returns(
                new RAAccessInfo()
                {
                    AccessLevel = new UserAccessLevel()
                    {
                        LevelOfAccess = 30,
                        ExpirationDate = DateTime.Now.AddYears(1)
                    }
                }
            );
            var cacheStored = false;
            _cacheProvider.When(a => a.Store(cacheKey, Arg.Any<RAg.Net.RAWS.GetSiteUserData.SiteUserData>(), Arg.Any<CacheSettings>()))
                .Do(a => 
                    cacheStored = true);

            _formsContext.PublicInitializePermissions();

            Assert.IsTrue(cacheFetched);
            Assert.IsTrue(cacheStored);
            Assert.AreEqual(_formsContext.AccessLevel, AccessLevel.Student);
            Assert.AreEqual(_formsContext.AccessType, AccessType.Adopter);
        }
    }
}
