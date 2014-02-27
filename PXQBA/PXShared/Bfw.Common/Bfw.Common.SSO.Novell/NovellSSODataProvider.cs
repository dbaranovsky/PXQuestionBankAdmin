using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using Bfw.Common.Logging;

namespace Bfw.Common.SSO.Novell
{
    /// <summary>
    /// Pulls all SSO related data from an HttpRequest based on the Novell NAM/IDP system.
    /// </summary>
    public class NovellSSODataProvider : ISSODataProvider
    {
        /// <summary>
        /// bfw-user-id header key.
        /// </summary>
        private const string BfwAuthSession = "bfw-auth-session";

        /// <summary>
        /// bfw-user-id header key.
        /// </summary>
        private const string BfwUserId = "bfw-uid";

        /// <summary>
        /// bfw-dlap-header header key.
        /// </summary>
        private const string BfwDlapHeader = "bfw-dlap-header";

        /// <summary>
        /// bfw-bh-header header key.
        /// </summary>
        private const string BfwBhHeader = "bfw-bh-header";

        /// <summary>
        /// bfw-user-data header key.
        /// </summary>
        private const string BfwUserData = "bfw-user-data";

        /// <summary>
        /// bfw-user-data header key.
        /// </summary>
        private const string BfwSwitchToProtected = "bfw-projected-switch";

        /// <summary>
        /// Value is 1 if URL being accessed is a protected resource. Header is either missing
        /// or value is != 1 if resource being accessed is unprotected.
        /// </summary>
        private const string BfwProtected = "bfw-protected";

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        private Bfw.Common.Logging.ILogger Logger { get; set; }

        /// <summary>
        /// Tracer to use
        /// </summary>
        private Bfw.Common.Logging.ITraceManager Tracer { get; set; }

        /// <summary>
        /// Sets all values of SSOData object that can be read from HTTP headers.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="data">The data.</param>
        private void ExtractHeaderData(HttpRequest request, SSOData data)
        {
            bool testing = false;

            using (Tracer.DoTrace("ExtractHeaderData"))
            {
                if (request.QueryString["testing"] != null)
                {
                    testing = true;
                }
                var expectedHeaders = new string[] { BfwSwitchToProtected, BfwProtected, BfwAuthSession, BfwUserId, BfwDlapHeader, BfwBhHeader, BfwUserData };

                System.Collections.Specialized.NameValueCollection headers = new System.Collections.Specialized.NameValueCollection();
                if (testing)
                {
                    headers.Add(BfwSwitchToProtected, "true");
                    headers.Add(BfwProtected, "1");
                    headers.Add(BfwAuthSession, "IPCZQX03a36c6c0a=000002000a01fe8864a6d40142c20fb507ff33d8");
                    headers.Add(BfwSwitchToProtected, "1");
                    headers.Add(BfwUserId, "156");
                    headers.Add(BfwDlapHeader, "");
                    headers.Add(BfwBhHeader, "");
                    headers.Add(BfwUserData, "{\"LastName\":\"Instructor\",\"Email\":\"instructor@school.edu\",\"AgilixUsers\":[{\"Enrollments\":[{\"ID\":\"377\",\"CourseDomainID\":\"1\",\"CourseID\":\"357\"}],\"ID\":\"372\",\"Userspace\":\"root\",\"DomainID\":\"1\"}],\"FirstName\":\"Joe\",\"CustomerID\":\"123456\"}");
                }
                else
                {
                    using (Tracer.DoTrace("Iterate Headers"))
                    {
                        foreach (var expected in expectedHeaders)
                        {
                            if (request.Headers.AllKeys.Contains(expected))
                            {
                                headers.Add(expected, request.Headers[expected]);
                            }
                        }
                    }
                }

                using (Tracer.DoTrace("Set Headers if they Exist"))
                {
                    if (headers.AllKeys.Contains(BfwAuthSession))
                    {
                        data.AuthSession = headers[BfwAuthSession];
                    }
                    if (headers.AllKeys.Contains(BfwSwitchToProtected))
                    {
                        data.SwitchToProtected = (headers[BfwSwitchToProtected] == "true" || headers[BfwSwitchToProtected] == "1") ? true : false;
                    }
                    if (headers.AllKeys.Contains(BfwProtected))
                    {
                        data.IsProtected = (headers[BfwProtected] == "1") ? true : false;
                    }
                    if (headers.AllKeys.Contains(BfwSwitchToProtected))
                    {
                        data.SwitchToProtected = (headers[BfwSwitchToProtected] == "true" || headers[BfwSwitchToProtected] == "1") ? true : false;
                    }
                    if (headers.AllKeys.Contains(BfwUserId))
                    {
                        data.UserId = headers[BfwUserId];
                    }
                }

                using (Tracer.DoTrace("Set Auth Headers for userid"))
                {
                    if (!string.IsNullOrEmpty(data.UserId))
                    {
                        if (headers.AllKeys.Contains(BfwDlapHeader))
                        {
                            data.DlapAuth = headers[BfwDlapHeader];
                        }

                        if (headers.AllKeys.Contains(BfwBhHeader))
                        {
                            data.BrainHoneyAuth = request.Headers[BfwBhHeader];
                        }

                        if (headers.AllKeys.Contains(BfwUserData))
                        {
                            try
                            {
                                var jser = new JavaScriptSerializer();
                                data.User = jser.Deserialize<SSOUser>(headers[BfwUserData]);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Could not deserialize bfw-user-data JSON object", ex);
                            }
                        }
                    }
                }

                using (Tracer.DoTrace("Determine Protected Status"))
                {
                    var protectedResource = headers.AllKeys.Contains(BfwProtected);
                    if (protectedResource && string.IsNullOrEmpty(data.UserId))
                    {
                        // If user is not authenticated and trying to access a protected resource, indicate that they should be redirected.
                        data.SwitchToProtected = false;
                        data.SwitchToUnprotected = true;
                    }
                    else if (!protectedResource && !string.IsNullOrEmpty(data.UserId))
                    {
                        // If user is authenticated and is access an unprotected resource, indicate that they should be redirected.
                        data.SwitchToUnprotected = false;
                        data.SwitchToProtected = true;
                    }
                    else
                    {
                        // User is unauthenticated and accessing a non-protected resource (which is ok and should not result in redirect)
                        // OR
                        // User is authenticated and accessing a protected resource (which is ok and should not result in redirect)
                        data.SwitchToProtected = false;
                        data.SwitchToProtected = false;
                    }
                }

                using (Tracer.DoTrace("Log Headers"))
                {
                    //LogHeaders(headers);
                }
            }
        }

        /// <summary>
        /// Logs the headers.
        /// </summary>
        /// <param name="headers">The headers.</param>
        private void LogHeaders(System.Collections.Specialized.NameValueCollection headers)
        {
            var sb = new StringBuilder();

            foreach (var h in headers.AllKeys)
            {
                sb.AppendFormat("{0} => {1}\n", h, headers[h]);
            }

            Logger.Log(sb.ToString(), Bfw.Common.Logging.LogSeverity.Information);
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NovellSSODataProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public NovellSSODataProvider(Bfw.Common.Logging.ILogger logger, Bfw.Common.Logging.ITraceManager tracer)
        {
            Logger = logger;
            Tracer = tracer;
        }

        #endregion

        #region ISSODataProvider Members

        /// <summary>
        /// Extracts all available data from the given request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public SSOData GetData(HttpRequest request)
        {
            var data = new SSOData();

            ExtractHeaderData(request, data);

            return data;
        }

        #endregion
    }
}