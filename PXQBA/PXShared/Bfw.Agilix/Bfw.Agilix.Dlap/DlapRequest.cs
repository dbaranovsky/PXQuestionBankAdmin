using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bfw.Common.Collections;

namespace Bfw.Agilix.Dlap
{
    /// <summary>
    /// Contains all information required to submit a request to the Agilix DLAP API.
    /// </summary>
    public class DlapRequest : Session.IDlapRequestTransformer, IDisposable
    {
        #region Constants

        /// <summary>
        /// Root element used if the request is a batch command
        /// </summary>
        public const string BATCH_ELEMENT = "requests";

        /// <summary>
        /// Root element "batch" is used 
        /// 1. request Mode is DlapRequestMode.Batch 
        /// 2. request type is DlapRequestType.Post
        /// 3. Parameter is with empty cmd like {"cmd", ""}. 
        /// Example is demonstrated in Bfw.Agilix.Commands.GetUsersBatch
        /// Check ~/Docs/Concept/CommandUsage for more information
        /// </summary>
        public const string BATCH_ELEMENT_WITHOUT_PARAMETERS = "batch";

        /// <summary>
        /// Root element used if the request is a single command
        /// </summary>
        public const string SINGLE_ELEMENT = "request";

        #endregion

        #region Properties

        /// <summary>
        /// Contains any attributes that live in the response element
        /// </summary>
        /// <value>Depends on the exact DLAP command being used</value>
        public Dictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Determines what type of content is being sent with the request. Defaults to Text
        /// </summary>
        /// <value>Any valid MIME type</value>
        public string ContentType { get; set; }

        /// <summary>
        /// Determines if this request represents a batch, single request, etc. Defaults to Single
        /// </summary>
        /// <value>Any value of <see cref="DlapRequestMode" /></value>
        public DlapRequestMode Mode { get; set; }

        /// <summary>
        /// Contains any query string parameters expected by the DLAP request
        /// </summary>
        /// <value>Depends on the exact DLAP command being used</value>
        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Contains any stream based data that needs to be posted as part of the request
        /// </summary>
        /// <value>A file or other binary resource being POSTed into DLAP</value>
        protected Stream PostStreamData { get; set; }

        /// <summary>
        /// Contains any XML fragments that need to be children of the root request element
        /// </summary>
        /// <value>XML fragments that make up the POST data</value>
        protected List<XElement> PostXmlData { get; set; }

        /// <summary>
        /// True if this specific request's Request root element should be suppressed. Defaults to false
        /// </summary>
        public bool SuppressWrapper { get; set; }

        /// <summary>
        /// Timeout that applies to this specific request. Defaults to null
        /// </summary>
        public int? Timeout { get; set; }

        /// <summary>
        /// Determines the type of HTTP method used to make the request to Dlap
        /// </summary>
        /// <value>Any value of DlaptRequestType /></value>
        public DlapRequestType Type { get; set; }

        /// <summary>
        /// True if this specific request should use compression or not
        /// </summary>
        public bool? UseCompression { get; set; }

        private XDocument RequestBody { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes all collections and sets Type = None and Mode = Single
        /// </summary>
        public DlapRequest()
        {
            Type = DlapRequestType.None;
            Mode = DlapRequestMode.Single;
            Attributes = new Dictionary<string, object>();
            Parameters = new Dictionary<string, object>();
            PostXmlData = new List<XElement>();
            PostStreamData = null;
            Timeout = null;
            UseCompression = null;
            ContentType = "text/xml";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Appends the given document fragment as the next child of the root request element
        /// </summary>
        /// <param name="postData">Document fragment to add to the request</param>
        public void AppendData(XElement postData)
        {
            if (null != postData)
            {
                PostXmlData.Add(postData);

                if (null != PostStreamData)
                {
                    PostStreamData.Close();
                }
                PostStreamData = null;
            }
        }

        /// <summary>
        /// Appends stream data to an existing request
        /// </summary>
        /// <param name="postData">Binary stream to append to request</param>
        public void AppendData(Stream postData)
        {
            if (null != postData)
            {
                PostStreamData = postData;
                PostXmlData = new List<XElement>();
            }
        }

        /// <summary>
        /// Compiles all information into an XML request to send to DLAP
        /// </summary>
        /// <param name="data">Stream to write XML request to</param>
        private void BuildDocument(Stream data)
        {
            IEnumerable<XAttribute> attributes;
            string root = SINGLE_ELEMENT;

            if (SuppressWrapper)
            {
                RequestBody = new XDocument(PostXmlData);
            }
            else
            {
                switch (Mode)
                {
                    case DlapRequestMode.Single:
                        root = SINGLE_ELEMENT;
                        break;

                    case DlapRequestMode.Batch:
                        root = (!Parameters.IsNullOrEmpty() && !Parameters.FirstOrDefault().Value.ToString().IsNullOrEmpty())
                                   ? BATCH_ELEMENT
                                   : BATCH_ELEMENT_WITHOUT_PARAMETERS;
                        break;

                    default:
                        root = SINGLE_ELEMENT;
                        break;
                }

                attributes = from a in Attributes select new XAttribute(a.Key, a.Value);

                RequestBody = new XDocument(
                    new XElement(root, attributes, PostXmlData)
                    );
            }
            using (var xw = new System.Xml.XmlTextWriter(data, null))
            {
                RequestBody.WriteTo(xw);
            }
        }
        /// <summary>
        /// Returns first line of batch command if found
        /// </summary>
        /// <returns>string</returns>
        public string BatchDescription()
        {
            if (!PostXmlData.IsNullOrEmpty() && PostXmlData.DescendantNodes().ToList().Count > 0 && Mode == DlapRequestMode.Batch)
            {
                return PostXmlData.DescendantNodes().ToList().First().ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Combines all necessary data into a single request stream. The resulting stream
        /// can then be POSTed to DLAP
        /// </summary>
        /// <param name="request">Stream into which the request data is copied</param>
        public void BuildRequest(Stream request)
        {
            Stream data = null;

            switch (Type)
            {
                case DlapRequestType.Get:
                    break;
                case DlapRequestType.Post:
                    if (null == PostStreamData)
                    {
                        BuildDocument(request);
                    }
                    else
                    {
                        data = PostStreamData;
                    }
                    break;
                default:
                    break;
            }

            //copy the data stream into the request stream
            if (null != data)
            {
                if (data.CanSeek)
                {
                    data.Seek(0, SeekOrigin.Begin);
                }

                byte[] buffer = new byte[0x4000];
                int bytes;
                while ((bytes = data.Read(buffer, 0, buffer.Length)) > 0)
                {
                    request.Write(buffer, 0, bytes);
                }
            }
        }

        /// <summary>
        /// Builds the request XML body and returns it to the caller. This does not send the actual
        /// request to DLAP.
        /// </summary>
        /// <returns></returns>
        public XDocument GetXmlRequestBody()
        {
            using (var stream = new MemoryStream())
            {
                BuildRequest(stream);
            }

            return RequestBody;
        }

        /// <summary>
        /// Builds a URL query string based on the data contained by the Parameters collection. The values for each
        /// parameter will be encoded automatically.
        /// </summary>
        /// <returns>Query string for the request</returns>
        public string BuildQuery()
        {
            string query = string.Empty;

            if (!Parameters.IsNullOrEmpty())
            {
                query = Parameters.Fold("&", p => string.Format("{0}={1}", HttpUtility.UrlEncode(p.Key), HttpUtility.UrlEncode(p.Value.ToString())));
            }

            return query;
        }

        /// <summary>
        /// Clears all document fragments and stream data
        /// </summary>
        public void ClearData()
        {
            PostXmlData = new List<XElement>();

            if (null != PostStreamData)
            {
                PostStreamData.Close();
            }

            PostStreamData = null;
        }

        #endregion

        #region overrides from System.Object

        /// <summary>
        /// Generates a string containing the query string and attribute parameters that identify the request
        /// </summary>
        /// <returns>string representation of the request</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("query: {0}:", BuildQuery());
            sb.AppendFormat("params: ");

            foreach (var p in Parameters.Keys)
            {
                sb.AppendFormat("[{0}|{1}]", p, Parameters[p].ToString());
            }

            return sb.ToString();
        }

        #endregion

        #region IDlapRequestTransformer Members

        /// <summary>
        /// Empty implementation
        /// </summary>
        /// <returns>current instance object</returns>
        public DlapRequest ToRequest()
        {
            return this;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Cleans up any system resources in use by the request
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Cleans up any system resources in use by the request
        /// </summary>
        /// <param name="disposing">true if object is being disposed, false otherwise</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (PostStreamData != null)
                {
                    try
                    {
                        PostStreamData.Close();
                    }
                    catch { }
                }
            }
        }

        #endregion
    }
}
