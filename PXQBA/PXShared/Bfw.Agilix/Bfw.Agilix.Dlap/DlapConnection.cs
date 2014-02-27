using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.Agilix.Dlap.Configuration;
using System.Configuration;

namespace Bfw.Agilix.Dlap
{
    /// <summary>
    /// Contains all necessary information required to establish a connection with the DLAP server
    /// </summary>
    public class DlapConnection : ICloneable
    {
        #region Properties

        /// <summary>
        /// Container for any cookies returned by Dlap that need to be round triped (e.g. authentication, etc)
        /// </summary>
        public System.Net.CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// access to the dlap config files
        /// </summary>
        public SessionManagerSection SessionManagerSection { get; set; }

        /// <summary>
        /// If not null, this is the <see cref="Bfw.Common.Logging.ILogger">ILogger</see> instance that any
        /// log messages will be writen to.
        /// </summary>
        public Bfw.Common.Logging.ILogger Logger { get; set; }

        /// <summary>
        /// Sets the default request timeout for the connection, can be overriden by the command
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// If not null, this is the <see cref="Bfw.Common.Logging.ITraceManager">ITraceManager</see> instance that any
        /// trace messages will be writen to.
        /// </summary>
        public Bfw.Common.Logging.ITraceManager Tracer { get; set; }

        /// <summary>
        /// URL that points to the appropriate DLAP server
        /// </summary>
        private readonly string url;

        /// <summary>
        /// URL that points to the appropriate DLAP server for this environment
        /// </summary>
        /// <value>set value of url</value>
        public string Url
        {
            get
            {
                return url;
            }
        }

        /// <summary>
        /// True if compression should be used, false (default) otherwise. can be overriden by the command
        /// </summary>
        public bool UseCompression { get; set; }

        /// <summary>
        /// UserAgent string sent to DLAP. Defaults to Bfw.Agilix.Dlap.DlapConnection, but can be overriden by
        /// each application for tracking purposes.
        /// </summary>
        public string UserAgent { get; set; }

        private string trustHeaderUsername = string.Empty;
        /// <summary>
        /// When set to a non-empty, non-null value of the form userspace/username then
        /// the connection will automatically inject the DlapUserId trust header. This means
        /// that no explicit login is necessary. The ID of the user can also be used, which
        /// is faster because the user doesn't have to be loaded in DLAP.
        /// </summary>
        public string TrustHeaderUsername
        {
            get
            {
                return trustHeaderUsername;
            }
            set
            {
                int id = -1;
                if (int.TryParse(value, out id))
                {
                    trustHeaderUsername = value;
                }
                else
                {
                    var parts = value.Split('/');
                    trustHeaderUsername = string.Format("//{0}//{1}", parts[0], parts[1]);
                }
            }
        }

        /// <summary>
        /// This must be set to the same secret key as configured on the DLAP domain.
        /// </summary>
        public string TrustHeaderKey { protected get; set; }

        /// <summary>
        /// True if trust headers are being used, false otherwise.
        /// You can use this method to check to see if all data necessary to use trust headers has been
        /// set on the object.
        /// </summary>
        public bool UsingTrustHeaders
        {
            get
            {
                return !string.IsNullOrEmpty(TrustHeaderUsername) && !string.IsNullOrEmpty(TrustHeaderKey);
            }
        }        

        #endregion

        #region Constructors

        /// <summary>
        /// In order to create a connection, you must specify the Url to the DLAP server. This value
        /// can not be changed after the object has been created
        /// </summary>
        /// <param name="dlapUrl"></param>
        public DlapConnection(string dlapUrl)
        {
            url = dlapUrl;
            UserAgent = "Bfw.Agilix.Dlap.DlapConnection";
            Timeout = 600000;

            CookieContainer = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);

            SessionManagerSection = ConfigurationManager.GetSection("agilixSessionManager") as SessionManagerSection;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the details of extracting the response data stream and building the DlapResponse
        /// </summary>
        /// <param name="webResponse">Raw web response</param>
        /// <returns>Fully processed DlapResponse</returns>
        protected DlapResponse ProcessResponse(HttpWebResponse webResponse)
        {
            Bfw.Agilix.Dlap.DlapResponse response = null;

            using (Tracer.DoTrace("Process Response"))
            {
                if (null != webResponse)
                {
                    Stream webData = webResponse.GetResponseStream();

                    if (!UsingTrustHeaders)
                    {
                        CookieContainer.Add(webResponse.Cookies);
                    }

                    if ("gzip" == webResponse.Headers["Agilix-Encoding"])
                    {
                        webData = new GZipStream(webData, CompressionMode.Decompress);
                    }

                    if (webResponse.ContentType.StartsWith("text/xml"))
                    {
                        response = new Bfw.Agilix.Dlap.DlapResponse();

                        try
                        {
                            response.ParseResponse(webData);
                        }
                        catch (Exception ex)
                        {
                            webResponse.Close();
                            webData.Dispose();

                            StreamReader sr = new StreamReader(webData);
                            Logger.Log(string.Format("Error: {0}, Response: {1}", ex.Message, sr.ReadToEnd()), LogSeverity.Error);
                            sr.Close();

                            response = new Bfw.Agilix.Dlap.DlapResponse()
                            {
                                Code = Bfw.Agilix.Dlap.DlapResponseCode.Error,
                                Message = ex.Message
                            };
                        }
                    }
                    else
                    {
                        var bufferSize = (int)webResponse.ContentLength;

                        // If there's no content (length -1 or 0), use a default buffer length to prevent errors.
                        if (bufferSize <= 0)
                        {
                            bufferSize = 1024;
                        }

                        Stream data = new MemoryStream(bufferSize);
                        webData.Copy(data, bufferSize);
                        data.Flush();
                        data.Seek(0, SeekOrigin.Begin);

                        response = new Bfw.Agilix.Dlap.DlapResponse(data);
                    }

                    webResponse.Close();
                    webData.Dispose();

                    response.ContentType = webResponse.ContentType;
                }
                else
                {
                    response = new Bfw.Agilix.Dlap.DlapResponse()
                    {
                        Code = Bfw.Agilix.Dlap.DlapResponseCode.Error,
                        Message = "DLAP Request failed"
                    };
                }
            }

            response.DlapAuthCookie = getDlapAuthCookieFromResponse(webResponse);

            return response;
        }

        private String getDlapAuthCookieFromResponse(HttpWebResponse webResponse)
        {
            if (webResponse == null)
                return null;

            if (webResponse.Headers == null)
                return null;

            if (String.IsNullOrEmpty(webResponse.Headers["Set-Cookie"]))
                return null;

            try
            {
                String dlapAuthCookieName = SessionManagerSection.Connection.CookieName;
                String setCookieHeader = webResponse.Headers["Set-Cookie"];
                String[] setCookieValues = setCookieHeader.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String cookie in setCookieValues)
                {
                    if (!cookie.StartsWith(dlapAuthCookieName))
                        continue;

                    return cookie.Split(new char[] { '=' }, StringSplitOptions.None)[1];
                }
            }

            catch (Exception ex)
            {
                Logger.Exception(ex);
            }

            return null;
        }

        /// <summary>
        /// Sends a request to the DLAP server and returns the response
        /// </summary>
        /// <param name="request">Request to send to DLAP</param>
        /// <returns>Response sent by DLAP</returns>
        public virtual DlapResponse Send(DlapRequest request)
        {
            var callMessage = String.Join("&", request.Parameters.Keys.Map(k => k + "=" + request.Parameters[k]));

            using (Tracer.DoTrace("DlapConnection.Send"))
            {
                System.Diagnostics.Stopwatch sw = null;

                var logDlap = Logger == null ? false : Logger.ShouldLog("DLAP");
                if (logDlap)
                {
                    sw = new System.Diagnostics.Stopwatch();
                    sw.Start();
                }

                HttpWebResponse webResponse = Transmit(request);
                DlapResponse response = ProcessResponse(webResponse);

                if (logDlap)
                {
                    sw.Stop();

                    if (String.IsNullOrEmpty(callMessage))
                    {
                        var xmlRequestBody = request.GetXmlRequestBody();
                        callMessage = xmlRequestBody != null
                                          ? xmlRequestBody.ToString().Replace("\n", "")
                                          : "Could not determine DLAP command specifics.";
                    }
                    if (HttpContext.Current != null)
                    {
                        var actionNameItem = HttpContext.Current.Items["EXECUTING_ACTION"];
                        var actionName = (actionNameItem ?? "").ToString();
                        var actionDescription = !String.IsNullOrEmpty(actionName) ? String.Format(", [{0}]", actionName) : "";
                        var url = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
                        Logger.Debug(String.Format("{0:H:mm:ss}, DLAP call ({2:ss\\.ffffff}): {3}{1} [{4}]", DateTime.Now, actionDescription, sw.Elapsed, callMessage, url), LogSeverity.Debug, new List<string>() { "DLAP" });
                    }
               }
                return response;
            }
        }

        /// <summary>
        /// Handles the details of sending the DLAP request over HTTP/S
        /// </summary>
        /// <param name="request">Dlap Request to transmit</param>
        /// <returns>Raw web response for processing</returns>
        protected virtual HttpWebResponse Transmit(DlapRequest request)
        {
            HttpWebResponse response = null;
            string cmd = string.Empty;

            if (request.Parameters != null && request.Parameters.ContainsKey("cmd"))
            {
                cmd = request.Parameters["cmd"].ToString();
            }
            else if (request.Attributes != null && request.Attributes.ContainsKey("cmd"))
            {
                cmd = request.Attributes["cmd"].ToString();
            }

            if (request.Mode == DlapRequestMode.Batch)
            {

                cmd = string.Format("Executing Batch:{0}...", request.BatchDescription());

            }
  
            using (Tracer.DoTrace("Transmit(cmd={0})", cmd))
            {
                HttpWebRequest webRequest = null;
                int timeout = (request.Timeout.HasValue ? request.Timeout.Value : Timeout);
                bool compression = (request.UseCompression.HasValue ? request.UseCompression.Value : UseCompression);
                string url = Url;
                string query = string.Empty;


                using (Tracer.DoTrace("BuildQuery"))
                {
                    query = request.BuildQuery();
                }

                if (!string.IsNullOrEmpty(query))
                {
                    url = string.Format("{0}?{1}", url, query);
                }

                Uri uri = new Uri(url);
                webRequest = WebRequest.Create(url) as HttpWebRequest;
                webRequest.UserAgent = UserAgent;
                webRequest.ReadWriteTimeout = timeout;
                webRequest.AllowAutoRedirect = false;
                webRequest.Proxy = null;

                if (UsingTrustHeaders)
                {
                    AddTrustHeader(webRequest);
                }
                else
                {
                    //we only need to use cookies if we are NOT using trustheaders
                    if (null != webRequest.CookieContainer)
                    {
                        using (Tracer.DoTrace("Copy Cookies to Request"))
                        {
                            webRequest.CookieContainer.Add(CookieContainer.GetCookies(uri));
                        }
                    }
                    else
                    {
                        webRequest.CookieContainer = CookieContainer;
                    }

                    var cookies = webRequest.CookieContainer.GetCookies(uri);
                    for (int i = 0; i < cookies.Count; ++i)
                    {
                        Logger.Debug("{0} -> {1}", cookies[i].Domain, cookies[i].Name);
                    }
                }

                webRequest.ContentType = request.ContentType;

                if (compression)
                {
                    webRequest.Headers.Add("Agilix-Encoding", "gzip");
                }

                switch (request.Type)
                {
                    case DlapRequestType.Post:
                        webRequest.Method = "POST";
                        using (Tracer.DoTrace("Write POST body"))
                        {
                            using (var rs = webRequest.GetRequestStream())
                            {
                                request.BuildRequest(rs);
                                rs.Close();
                            }
                        }
                        break;

                    case DlapRequestType.Get:
                        webRequest.Method = "GET";
                        break;

                    default:
                        throw new DlapException("Could not determine the correct HTTP Method for the request");
                }

                try
                {
                    using (Tracer.DoTrace("Get Response cmd={0}", cmd))
                    {
                        response = (HttpWebResponse)webRequest.GetResponse();
                    }
                }
                catch (WebException ex)
                {
                    response = null;
                    Logger.Exception(ex);
                }
            }

            return response;
        }

        /// <summary>
        /// Adds the DLAP trust header to the given request.
        /// </summary>
        /// <param name="request">Request to add the trust header to.</param>
        private void AddTrustHeader(HttpWebRequest request)
        {
            var date = System.Xml.XmlConvert.ToString(DateTime.UtcNow, System.Xml.XmlDateTimeSerializationMode.Utc);
            var hash = ComputeHash(TrustHeaderUsername, date, TrustHeaderKey);
            var headerValue = string.Format("userid={0}&timestamp={1}&hash={2}", HttpUtility.UrlEncode(TrustHeaderUsername), HttpUtility.UrlEncode(date), HttpUtility.UrlEncode(hash));

            request.Headers.Add("DlapUserId", headerValue);
        }

        /// <summary>
        /// Computes the DLAP Trust header hash.
        /// </summary>
        /// <param name="user">Username of user to authenticate in userspace//username format.</param>
        /// <param name="date">Date in UTC formatted using XmlConvert.</param>
        /// <param name="key">Secret key as configured in DLAP.</param>
        /// <returns>Trust header hash value</returns>
        private string ComputeHash(string user, string date, string key)
        {
            var encoder = new UTF8Encoding(false);
            var hashData = string.Format("{0}{1}", user, date);
            var utf8Bytes = encoder.GetBytes(hashData);
            var sha1 = System.Security.Cryptography.HMACSHA1.Create();

            sha1.Key = encoder.GetBytes(key);
            var hash = sha1.ComputeHash(utf8Bytes);

            return Convert.ToBase64String(hash);
        }

        #endregion

        public object Clone()
        {
            var clone = ConnectionFactory.GetDlapConnection(Url);
            clone.CookieContainer = null;
            clone.Logger = Logger;
            clone.Tracer = Tracer;
            clone.TrustHeaderKey = TrustHeaderKey;
            clone.TrustHeaderUsername = TrustHeaderUsername;
            clone.UserAgent = UserAgent;
            clone.UseCompression = UseCompression;
            clone.Timeout = Timeout;

            return clone;
        }
    }
}
