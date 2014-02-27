using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
namespace Bfw.Common
{
    public static class RouteUtils
    {
        public static RouteData GetRouteDataByUrl(string url)
        {
            return RouteTable.Routes.GetRouteData(new RewritedHttpContextBase(url));
        }


        private class RewritedHttpContextBase : HttpContextBase
        {
            private readonly HttpRequestBase mockHttpRequestBase;


            public RewritedHttpContextBase(string appRelativeUrl)
            {
                this.mockHttpRequestBase = new MockHttpRequestBase(appRelativeUrl);
            }




            public override HttpRequestBase Request
            {
                get
                {
                    return mockHttpRequestBase;
                }
            }


            private class MockHttpRequestBase : HttpRequestBase
            {
                private readonly string appRelativeUrl;


                public MockHttpRequestBase(string appRelativeUrl)
                {
                    this.appRelativeUrl = appRelativeUrl;
                }


                public override string AppRelativeCurrentExecutionFilePath
                {
                    get { return appRelativeUrl; }
                }


                public override string PathInfo
                {
                    get { return ""; }


                }
            }
        }
    }
}
