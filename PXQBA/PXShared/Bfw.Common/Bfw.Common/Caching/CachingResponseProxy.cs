using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using Bfw.Common.Logging;

namespace Bfw.Common.Caching
{
    /// <summary>
    /// Implements the IResponseProxy interface using the "File Caching Algorithm".
    /// </summary>
    public class CachingResponseProxy : IResponseProxy
    {
        #region Properties

        /// <summary>
        /// TraceManager for performance tracing.
        /// </summary>
        protected ITraceManager Tracer{set;get;}

        /// <summary>
        /// ILogger implmentation.
        /// </summary>
        protected ILogger Logger { set; get; }        

        /// <summary>
        /// Configuration section that stores all of the DLAP connection information.
        /// </summary>
        private CachingResponseProxySection configuration = null;

        /// <summary>
        /// Configuration section that stores all of the Cache settings information.
        /// </summary>
        /// <value>Set <see cref="configuration"/></value>
        protected CachingResponseProxySection Configuration
        {
            get
            {
                if (this.configuration == null)
                {
                    this.configuration = ConfigurationManager.GetSection("pxCacheManager") as CachingResponseProxySection;
                }

                return this.configuration;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CachingResponseProxy"/> class.
        /// </summary>
        /// <param name="tracer">ITraceManager used for performance tracing.</param>
        /// <param name="logger">ILogger used for loggin.</param>
        public CachingResponseProxy(ITraceManager tracer, ILogger logger)
        {
            Tracer = tracer;
            Logger = logger;
        }

        #endregion

        #region IResponseProxy

        /// <summary>
        /// Implements the "File Caching Algorithm". 
        /// Returns a CachedHttpWebResponse unless no-cache is set. 
        /// If the Cache-Control header on the proxied response contains "no-cache", 
        /// then do not cache the response regardless of CacheEnabled's value.
        /// </summary>
        /// <param name="target">The URL that is being requested.</param>
        /// <param name="original">Original web request for the content.</param>
        /// <returns>Either HttpWebResponse or CachedHttpWebResponse depdning on outcome of algorithm.</returns>
        public HttpWebResponse Proxy(Uri target, HttpWebRequest original)
        {

            HttpWebResponse response = null;
            original.UserAgent = "Proxy Client Request";
            using (Tracer.StartTrace("IResponseProxy.Proxy"))
            {
                if (CanServeCachedFile(target))
                {
                    response = GetCachedResponse(target);
                }
                else
                {
                    using (Tracer.StartTrace("IResponse.Proxy Get Non-Cached Response"))
                    {
                        response = (HttpWebResponse)original.GetResponse();

                        if (Configuration.ProxyCacheEnabled && !NoCacheHeaderSet(response))
                        {
                            response = CacheResponse(response);
                        }
                    }
                }
            }
                        
            return response;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines if the target content can be served from the cache.
        /// </summary>
        /// <param name="target">Content being requested from the cache.</param>
        /// <returns>true if request can be served from cache, false otherwise.</returns>
        private bool CanServeCachedFile(Uri target)
        {
            bool canServe = false;

            if (Configuration.ProxyCacheEnabled)
            {
                FileInfo cacheFileInfo = GetCacheFileInfo(target);
                DateTime cacheExpirationTime = DateTime.Now.AddMinutes(-Configuration.CacheDuration);

                if (cacheFileInfo.Exists && (cacheFileInfo.LastWriteTime > cacheExpirationTime))
                {
                    canServe = true;
                }
            }

            return canServe;
        }

        /// <summary>
        /// Saves a web response to a file cache. 
        /// Responses from the external source are stored in two files.  
        /// One file stores the metadata about the response such as the ContentType, CharacterSet, etc.  
        /// The second file stores the verbatim response body.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private CachedHttpWebResponse CacheResponse(HttpWebResponse response)
        {
            using (Tracer.StartTrace("Caching original web response"))
            {
                Uri target = response.ResponseUri;

                String contentFile = GetCacheFileName(target, Configuration.CacheContentExtension);
                String metaFile = GetCacheFileName(target, Configuration.CacheMetaExtension);
                String filePath = Configuration.CacheLocation;

                CachedResponseMetaData metaData = new CachedResponseMetaData(response);
                
                // Serialize the request context.
                IFormatter formatter = new BinaryFormatter();
                FileStream metaStream = new FileStream(Configuration.CacheLocation + metaFile, FileMode.Create);
                formatter.Serialize(metaStream, metaData);
                metaStream.Close();

                Stream stream = response.GetResponseStream();
                StreamReader oReader = new StreamReader(stream, Encoding.ASCII);
                StreamWriter oWriter = new StreamWriter(Configuration.CacheLocation + contentFile);
                oWriter.Write(oReader.ReadToEnd());

                oReader.Close();
                oWriter.Close();

                return GetCachedResponse(response.ResponseUri);
            }
        }

        /// <summary>
        /// Checks if response has the "no-cache" header attached.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private Boolean NoCacheHeaderSet(HttpWebResponse response)
        {
            return response.Headers.AllKeys.Contains("Pragma") && response.Headers["Pragma"].Equals("no-cache");            
        }

        /// <summary>
        /// Gets a cached response from the file system from the specified Uri.
        /// </summary>
        /// <param name="target">Content being requested from the cache.</param>
        /// <returns>Cached copy of the original response.</returns>
        private CachedHttpWebResponse GetCachedResponse(Uri target)
        {
            CachedHttpWebResponse cacheResponse = null;

            using (Tracer.StartTrace("IResponseProxy Get Cached Response"))
            {
                String contentFile = GetCacheFileName(target, Configuration.CacheContentExtension);
                String metaFile = GetCacheFileName(target, Configuration.CacheMetaExtension);
                FileStream contentStream = new FileStream(Configuration.CacheLocation + contentFile, FileMode.Open, FileAccess.Read);

                // Serialize the request context.
                IFormatter formatter = new BinaryFormatter();
                using (FileStream metaStream = new FileStream(Configuration.CacheLocation + metaFile, FileMode.Open, FileAccess.Read))
                {
                    CachedResponseMetaData metaData = (CachedResponseMetaData)formatter.Deserialize(metaStream);
                    cacheResponse = new CachedHttpWebResponse(metaData, contentStream);
                }
            }

            return cacheResponse;
        }

        /// <summary>
        /// Returns a hashed string value from a Uri.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private String HashKey(Uri target)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] hash = md5Hasher.ComputeHash(Encoding.Default.GetBytes(target.AbsoluteUri));
            String base64 = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");
            return base64;
        }

        /// <summary>
        /// Returns a formatted filename from from a Uri.
        /// The URI of the external content (e.g. URL or file path) is first hashed.  
        /// The resulting hash is assumed to be the name of the file in the File Cache's storage location.  
        /// </summary>
        /// <param name="target"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        private String GetCacheFileName(Uri target, String extension)
        {
            String fileName = String.Format("{0}.{1}", HashKey(target), extension);
            return fileName;
        }

        /// <summary>
        /// Returns a file info object from the specified file name.
        /// </summary>
        /// <param name="target">URL that is being requested from cache.</param>
        /// <returns>FileInfo object for the cached item.</returns>
        private FileInfo GetCacheFileInfo(Uri target)
        {
            String contentFile = GetCacheFileName(target, Configuration.CacheContentExtension);
            String filePath = String.Format("{0}{1}", Configuration.CacheLocation, contentFile);
            FileInfo info = new FileInfo(filePath);

            return info;
        }

        #endregion
    }
}