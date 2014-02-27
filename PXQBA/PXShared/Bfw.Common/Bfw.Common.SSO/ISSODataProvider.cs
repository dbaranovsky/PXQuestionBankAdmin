using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Common.SSO
{
    /// <summary>
    /// Abstraction surrounding how data is pulled from a web request to populate 
    /// Single-Sign-On data.
    /// </summary>
    public interface ISSODataProvider
    {
        /// <summary>
        /// Extracts all available data from the given request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        SSOData GetData(System.Web.HttpRequest request);
    }
}
