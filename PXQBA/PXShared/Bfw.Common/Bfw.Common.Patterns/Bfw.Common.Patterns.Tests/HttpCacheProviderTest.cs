using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Bfw.Common.Caching;
using Bfw.Common.Patterns.Caching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace Bfw.Common.Patterns.Tests
{
    [TestClass]
    public class HttpCacheProviderTest
    {
        private HttpCacheProvider cacheProvider;
        private HttpContext httpContext;

        [TestInitialize]
        public void InitializeTest()
        {
            cacheProvider = new HttpCacheProvider();
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

        [TestMethod]
        public void Verify_Fetch_Retrieves_Stored_Object()
        {
            //arragne
            httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "on"));
            HttpContext.Current.Cache.Insert("testKey", new StringBuilder("myobj"));

            //act
            var retrievedObj = cacheProvider.Fetch("testKey","testRegion").ToString();

            //assert
            Assert.AreEqual("myobj", retrievedObj);
        }

        //TODO: This is now a private method, rewrite
        //[TestMethod]
        //public void Verify_FetchByPattern_Retrieves_Stored_Objects()
        //{
        //    //arragne
        //    httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "on"));
        //    HttpContext.Current.Cache.Insert("key_with_string_OKAY_1", new StringBuilder("test1"));
        //    HttpContext.Current.Cache.Insert("key_with_string_OKAY_2", new StringBuilder("test2"));
        //    HttpContext.Current.Cache.Insert("key_without_string_ok", new StringBuilder("test3"));
            
        //    //act
        //    var retrievedObj = cacheProvider.FetchByPattern("OKAY");

        //    //assert
        //    Assert.AreEqual(2, retrievedObj.Count());
        //}

        [TestMethod]
        public void Verify_Store_Stores_Object_Successfully_With_Aging_Sliding()
        {
            //arrange
            httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "on"));

            //act
            cacheProvider.Store("testKey", new StringBuilder("myobj"), new CacheSettings() { Aging = AgingMechanism.Sliding, Duration = 1200, Priority = CachePriority.Normal },"testRegion", "testTag");

            //assert
            var key = "testKey";
            var retrievedObj = cacheProvider.Fetch(key).ToString();
            Assert.AreEqual("myobj", retrievedObj);
        }

        [TestMethod]
        public void Verify_Store_Stores_Object_Successfully_With_Aging_Static()
        {
            //arrange
            httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "on"));

            //act
            cacheProvider.Store("testKey", new StringBuilder("myobj"), new CacheSettings() { Aging = AgingMechanism.Static, Duration = 1200, Priority = CachePriority.Normal }, "testRegion", "testTag");

            //assert
            var key = "testKey";
            var retrievedObj = cacheProvider.Fetch(key).ToString();
            Assert.AreEqual("myobj", retrievedObj);
        }

        [TestMethod]
        public void Verify_Store_Stores_Object_Successfully_With_Priority_High()
        {
            //arrange
            httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "on"));

            //act
            cacheProvider.Store("testKey", new StringBuilder("myobj"), new CacheSettings() { Aging = AgingMechanism.Sliding, Duration = 1200, Priority = CachePriority.High }, "testRegion", "testTag");

            //assert
            var key = "testKey";
            var retrievedObj = cacheProvider.Fetch(key).ToString();
            Assert.AreEqual("myobj", retrievedObj);
        }

        [TestMethod]
        public void Verify_Store_Stores_Object_Successfully_With_Priority_Low()
        {
            //arrange
            httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "on"));

            //act
            cacheProvider.Store("testKey", new StringBuilder("myobj"), new CacheSettings() { Aging = AgingMechanism.Sliding, Duration = 1200, Priority = CachePriority.Low }, "testRegion", "testTag");

            //assert
            var key = "testKey";
            var retrievedObj = cacheProvider.Fetch(key).ToString();
            Assert.AreEqual("myobj", retrievedObj);
        }

        [TestMethod]
        public void Verify_Remove_Invalidate_Cached_Object()
        {
            //arrange
            httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "on"));

            //act
            cacheProvider.Store("testKey", new StringBuilder("myobj"), new CacheSettings() { Aging = AgingMechanism.Sliding, Duration = 1200, Priority = CachePriority.High }, "testRegion", "testTag");

            //assert
            var key = "testKey";
            cacheProvider.Remove("testKey", "testRegion");
            var retrievedObj = cacheProvider.Fetch(key);
            Assert.IsNull(retrievedObj);
        }

        //TODO: This is now a private method, rewrite
        //[TestMethod]
        //public void Verify_RemoveByPattern_Invalidate_Cached_Objects()
        //{
        //    //arrange
        //    httpContext.Request.Cookies.Add(new HttpCookie("cachestat", "on"));
        //    HttpContext.Current.Cache.Insert("key_with_string_OKAY_1", new StringBuilder("test1"));
        //    HttpContext.Current.Cache.Insert("key_with_string_OKAY_2", new StringBuilder("test2"));
        //    HttpContext.Current.Cache.Insert("key_without_string_ok", new StringBuilder("test3"));

        //    //assert
        //    cacheProvider.RemoveByPattern("OKAY");
        //    var retrievedObj1 = HttpContext.Current.Cache["key_with_string_OKAY_1"];
        //    var retrievedObj2 = HttpContext.Current.Cache["key_with_string_OKAY_2"];
        //    Assert.IsNull(retrievedObj1);
        //    Assert.IsNull(retrievedObj2);
        //}

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
