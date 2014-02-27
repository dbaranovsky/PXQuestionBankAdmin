using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;

namespace Bfw.Common.Caching
{
    /// <summary>
    /// Helper class for serializing WebResponse meta data.
    /// </summary>
    [Serializable()]
    public class CachedResponseMetaData
    {
        #region Properties

        /// <summary>
        /// Gets the character set of the response.
        /// </summary>        
        public string CharacterSet { get; set; }
        
        /// <summary>
        /// Gets the method that is used to encode the body of the response.
        /// </summary>
        public string ContentEncoding { get; set; }
        
        /// <summary>
        /// Gets the length of the content returned by the request.
        /// </summary>
        public long ContentLength { get; set; }
        
        /// <summary>
        /// Gets the content type of the response.
        /// </summary>
        public string ContentType { get; set; }
        
        /// <summary>
        /// Gets or sets the cookies that are associated with this response.
        /// </summary>
        public CookieCollection Cookies { get; set; }

        /// <summary>
        /// Gets the headers that are associated with this response from the server.
        /// </summary>
        public WebHeaderCollection Headers { get; set; }
        
        /// <summary>
        /// Gets a System.Boolean value that indicates whether both client and server
        /// were authenticated.
        /// </summary>
        public bool IsMutuallyAuthenticated { get; set; }
        
        /// <summary>
        /// Gets the last date and time that the contents of the response were modified.
        /// </summary>
        public DateTime LastModified { get; set; }
        
        /// <summary>
        /// Gets the method that is used to return the response.
        /// </summary>
        public string Method { get; set; }
        
        /// <summary>
        /// Gets the version of the HTTP protocol that is used in the response.
        /// </summary>
        public Version ProtocolVersion { get; set; }
        
        /// <summary>
        /// Gets the URI of the Internet resource that responded to the request.
        /// </summary>        
        public Uri ResponseUri { get; set; }
        
        /// <summary>
        /// Gets the name of the server that sent the response.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets the status of the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        
        /// <summary>
        /// Gets the status description returned with the response.
        /// </summary>
        public string StatusDescription { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedResponseMetaData"/> class.
        /// </summary>
        /// <param name="response">Response to pull values from.</param>
        public CachedResponseMetaData(HttpWebResponse response)
        {
            CharacterSet = response.CharacterSet;
            ContentEncoding = response.ContentEncoding;
            ContentLength = response.ContentLength;
            ContentType = response.ContentType;
            Cookies = response.Cookies;
            Headers = response.Headers;
            IsMutuallyAuthenticated = response.IsMutuallyAuthenticated;
            LastModified = response.LastModified;
            Method = response.Method;
            ProtocolVersion = response.ProtocolVersion;
            ResponseUri = response.ResponseUri;
            Server = response.Server;
            StatusCode = response.StatusCode;
            StatusDescription = response.StatusDescription;
        }

        /// <summary>
        /// Default constructor for serialization purposes.
        /// </summary>
        public CachedResponseMetaData() { }

        #endregion
    }
}