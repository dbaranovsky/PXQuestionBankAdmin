using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Web;

using Bfw.Common;
using Bfw.Common.Logging;

namespace WebApplicationTest
{
    /// <summary>
    /// Contains all necessary information required to establish a connection with the DLAP server
    /// </summary>
    public class DlapConnection
    {
        #region Properties

        /// <summary>
        /// Container for any cookies returned by Dlap that need to be round triped (e.g. authentication, etc)
        /// </summary>
        public System.Net.CookieContainer CookieContainer { get; set; }

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
        /// <value>set value of <see cref="_url" /></value>
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
            Timeout = 30;

            CookieContainer = new CookieContainer(int.MaxValue, int.MaxValue, int.MaxValue);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the details of extracting the response data stream and building the DlapResponse
        /// </summary>
        /// <param name="webResponse">Raw web response</param>
        /// <returns>Fully processed DlapResponse</returns>
        private Bfw.Agilix.Dlap.DlapResponse ProcessResponse( HttpWebResponse webResponse )
        {
            Bfw.Agilix.Dlap.DlapResponse response = null;

            using (Tracer.DoTrace("Process Response"))
            {
                if (null != webResponse)
                {
                    Stream webData = webResponse.GetResponseStream();
                    Stream data = new MemoryStream();
                    webData.Copy(data);
                    data.Flush();
                    data.Seek(0, SeekOrigin.Begin);

                    webResponse.Close();

                    CookieContainer.Add(webResponse.Cookies);

                    if ("gzip" == webResponse.Headers["Agilix-Encoding"])
                    {
                        data = new GZipStream(data, CompressionMode.Decompress);
                    }

                    if (webResponse.ContentType.StartsWith("text/xml"))
                    {
                        response = new Bfw.Agilix.Dlap.DlapResponse();
                        response.ParseResponse(data);
                    }
                    else
                    {
                        response = new Bfw.Agilix.Dlap.DlapResponse( data );
                    }
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

            return response;
        }

        /// <summary>
        /// Sends a request to the DLAP server and returns the response
        /// </summary>
        /// <param name="request">Request to send to DLAP</param>
        /// <returns>Response sent by DLAP</returns>
        public Bfw.Agilix.Dlap.DlapResponse Send_t( Bfw.Agilix.Dlap.DlapRequest request )
        {
            Bfw.Agilix.Dlap.DlapResponse response = null;
            HttpWebResponse webResponse = null;

            using (Tracer.DoTrace("DlapConnection.Send"))
            {
                webResponse = Transmit_t( request );
                response = ProcessResponse(webResponse);
            }

            return response;
        }

        /// <summary>
        /// Handles the details of sending the DLAP request over HTTP/S
        /// </summary>
        /// <param name="request">Dlap Request to transmit</param>
        /// <returns>Raw web response for processing</returns>
        protected HttpWebResponse Transmit_t( Bfw.Agilix.Dlap.DlapRequest request )
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

                webRequest.ContentType = request.ContentType;

                if (compression)
                {
                    webRequest.Headers.Add("Agilix-Encoding", "gzip");
                }

                if (!string.IsNullOrEmpty(TrustHeaderUsername) && !string.IsNullOrEmpty(TrustHeaderKey))
                {
                    AddTrustHeader(webRequest);
                }

                switch (request.Type)
                {
                    case Bfw.Agilix.Dlap.DlapRequestType.Post:
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

                    case Bfw.Agilix.Dlap.DlapRequestType.Get:
                        webRequest.Method = "GET";
                        break;

                    default:
                        throw new Bfw.Agilix.Dlap.DlapException( "Could not determine the correct HTTP Method for the request" );
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
    }
}
