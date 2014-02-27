using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Serialization;

namespace Bfw.Common.Caching
{
    /// <summary>
    /// Derived from System.Net.HttpWebResponse, this class allows the entire state of 
    /// a web response to be cached to disk using the format described in "Cache Data Format". 
    /// </summary>
    public class CachedHttpWebResponse : HttpWebResponse
    {
        #region Properties

        /// <summary>
        /// Static serializationinfo class to trick serialization constructor.
        /// </summary>
        protected static SerializationInfo _info
        {
            get
            {
                var info = new SerializationInfo(typeof(CachedHttpWebResponse), new FormatterConverter());
                info.AddValue("m_HttpResponseHeaders", new WebHeaderCollection());
                info.AddValue("m_Uri", new Uri("http://www.sample.com"));
                info.AddValue("m_Certificate", new System.Security.Cryptography.X509Certificates.X509Certificate());
                info.AddValue("m_Version", System.Environment.Version);
                info.AddValue("m_StatusCode", HttpStatusCode.OK);
                info.AddValue("m_ContentLength", 0);
                info.AddValue("m_Verb", "GET");
                info.AddValue("m_StatusDescription", string.Empty);
                info.AddValue("m_MediaType", "text/html");
                return info;
            }
        }

        /// <summary>
        /// Collection of cached meta properties for response object.
        /// </summary>
        public CachedResponseMetaData CachedData
        {
            get;
            set;
        }

        /// <summary>
        /// Content for response object.
        /// </summary>
        protected Stream Content { set; get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initializes the object using the given metadata and content.
        /// </summary>
        /// <param name="metaData"></param>
        /// <param name="stream"></param>
        public CachedHttpWebResponse(CachedResponseMetaData metaData, Stream stream)
            : base(_info, new StreamingContext())
        {           
            Content = stream;
            CachedData = metaData;            
            ContentType = metaData.ContentType;            
        }

        #endregion

        #region overrides from HttpWebResponse

        /// <summary>
        /// Returns a stream object of the response content.
        /// </summary>
        /// <returns></returns>
        public override Stream GetResponseStream()
        {
            if (Content != null)
            {
                return Content;
            }

            return base.GetResponseStream();
        }

        /// <summary>
        /// Override for base content type member that allows for setting value.
        /// </summary>
        public override String ContentType
        {
            get;set;
        }

        #endregion
    }
}