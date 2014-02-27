using System;
using System.Collections.Specialized;
using System.Web;
using NSubstitute;

namespace Bfw.PXWebAPI.Helpers.Context
{
    /// <summary>
    /// Used for Integration Tests where service locator is not mocked
    /// </summary>
    public class HttpContextAdapterMock : IHttpContextAdapter
    {
        private HttpContextBase httpContext;
        private const string PRODUCT_COURSE_ID = "109554";

        public HttpContextAdapterMock()
        {
            // Set any needed values for tests here for collections that are read-only
            httpContext = Substitute.For<HttpContextBase>();
            var request = Substitute.For<HttpRequestBase>();
            request.Form.Returns(new NameValueCollection { 
                { "productcourseids", PRODUCT_COURSE_ID }, 
                { "domainname", "My Domain Test"} 
            });
            httpContext.Request.Returns(request);
        }

        public HttpContextBase Current
        {
            get { return httpContext; }
        }
    }
}
