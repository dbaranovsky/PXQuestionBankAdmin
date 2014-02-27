using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace Bfw.Common.Caching
{
    /// <summary>
    /// Interface that defines API for proxying requests to external resources.
    /// </summary>
    public interface IResponseProxy
    {
        /// <summary>
        /// Implements the "File Caching Algorithm". 
        /// Returns a CachedHttpWebResponse unless no-cache is set. 
        /// If the Cache-Control header on the proxied response contains "no-cache", 
        /// then do not cache the response regardless of CacheEnabled's value.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="original"></param>
        /// <returns></returns>
        HttpWebResponse Proxy(Uri target, HttpWebRequest original);
    }
}