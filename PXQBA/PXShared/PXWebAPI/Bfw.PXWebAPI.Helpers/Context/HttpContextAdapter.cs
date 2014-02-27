using System;
using System.Web;

namespace Bfw.PXWebAPI.Helpers.Context
{
    public class HttpContextAdapter : IHttpContextAdapter
    {
        public HttpContextBase Current
        {
            get { return new HttpContextWrapper(HttpContext.Current); }
        }
    }
}
