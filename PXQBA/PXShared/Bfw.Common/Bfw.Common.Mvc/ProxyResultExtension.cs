using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Net;

namespace Bfw.Common.Mvc
{
    /// <summary>
    /// ProxyResult extension methods.  This may not require its own class.
    /// </summary>
    public static class ProxyResultExtension
    {
        /// <summary>
        /// Proxies the result.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="targetSystem">The target system.</param>
        /// <param name="proxySystem">The proxy system.</param>
        /// <param name="headers">The headers.</param>
        /// <returns></returns>
        public static ProxyResult ProxyResult(this Controller controller, Uri targetSystem, Uri proxySystem, WebHeaderCollection headers)
        {
            return new ProxyResult(targetSystem, proxySystem, headers);
        }
    }
}
