using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;
using Bfw.Common.Caching;
using Bfw.Common.Patterns.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Microsoft.ApplicationServer.Caching;

namespace Bfw.Common.Patterns.Tests
{
    [TestClass]
    public class AppFabricCacheProviderTest
    {
        private AppFabricCacheProvider cacheProvider;
        private IDataCacheFactoryProvider dataCacheFactoryProvider;
        private HttpContext httpContext;

        [TestInitialize]
        public void InitializeTest()
        {

            dataCacheFactoryProvider = Substitute.For<IDataCacheFactoryProvider>();
            cacheProvider = new AppFabricCacheProvider(dataCacheFactoryProvider);
            InitializeContext();
        }

        [TestMethod]
        public void Verify_IsCacheDisabledByCookie_Return_True_If_Cache_Cookie_Is_Off()
        {
            //arrange
            httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "off"));
            
            //act
            var cacheDisabled = cacheProvider.IsCacheDisabledByCookie();
            //assert

            Assert.AreEqual(true, cacheDisabled);
        }

        [TestMethod]
        public void Verify_IsCacheDisabledByCookie_Return_False_If_Cache_Cookie_Is_Not_Off()
        {
            //arrange
            httpContext.Request.Cookies.Add(new HttpCookie("cachestat", ""));

            //act
            var cacheDisabled = cacheProvider.IsCacheDisabledByCookie();
            //assert

            Assert.AreEqual(false, cacheDisabled);
        }

        [TestMethod]
        public void Verify_IsCacheDisabledByCookie_Return_False_If_Cache_Cookie_Does_Not_Exist()
        {
            //arrange
            
            //act
            var cacheDisabled = cacheProvider.IsCacheDisabledByCookie();
            //assert

            Assert.AreEqual(false, cacheDisabled);
        }

        private void InitializeContext()
        {
            httpContext = CreateHttpContext("index.aspx", "http://url", null);
            HttpContext.Current = httpContext;
        }

        private HttpContext CreateHttpContext(string fileName, string url, string queryString)
        {
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);
            var hres = new HttpResponse(sw);
            var hreq = new HttpRequest(fileName, url, queryString);
            var httpc = new HttpContext(hreq, hres);

            return httpc;
        }
    }
}
