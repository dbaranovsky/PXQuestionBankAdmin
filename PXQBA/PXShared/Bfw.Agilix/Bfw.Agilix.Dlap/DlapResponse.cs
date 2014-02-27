using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using Bfw.Common.Collections;

namespace Bfw.Agilix.Dlap
{
    /// <summary>
    /// Contains all information that represents an Agilix DLAP API response.
    /// Responses from the DLAP API are either XML or binary resources such as ZIP files.
    /// </summary>
    public class DlapResponse : Session.IDlapResponseParser, IDisposable
    {
        #region Constants

        /// <summary>
        /// Tag that indicates the root element in DLAP XML responses when the response contains
        /// the result of only one command (i.e. non-batched request)
        /// </summary>
        /// <value>response</value>
        public const string RESPONSE_ELEMENT = "response";

        /// <summary>
        /// Tag that indicates the root element in DLAP XML responses when the response contains
        /// the result of multiple commands (i.e. batched request)
        /// </summary>
        /// <value>responses</value>
        public const string RESPONSES_ELEMENT = "responses";

        #endregion

        #region Properties

        /// <summary>
        /// If the response is a batch response, this property provides access to any of the
        /// responses in the batch.
        /// </summary>
        public IEnumerable<DlapResponse> Batch { get; protected set; }

        /// <summary>
        /// Status Code returned by the DLAP server. This value indicates if a request
        /// was successful or why it may have failed
        /// </summary>
        public DlapResponseCode Code { get; set; }

        /// <summary>
        /// The content type of the actual response
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Convenience property that is true if there is anything in the Batch collection.
        /// </summary>
        public bool IsBatch
        {
            get
            {
                return !Batch.IsNullOrEmpty();
            }
        }

        /// <summary>
        /// Message (if any) the DLAP server returned as part of the response.
        /// </summary>
        /// <value>Empty string unless <see cref="Code" /> is not OK</value>
        public string Message { get; set; }
        
        /// <summary>
        /// Raw XDocument containing all child elements of the root response element
        /// </summary>
        public XDocument ResponseXml { get; set; }

        /// <summary>
        /// Contains the stream returned by DLAP if this is a non-text based response
        /// </summary>
        public Stream ResponseStream { get; set; }

        /// <summary>
        /// Contains the auth cookie from the dlap response
        /// </summary>
        public String DlapAuthCookie { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Sets up an empty DLAP response
        /// </summary>
        public DlapResponse()
        {
            Code = DlapResponseCode.OK;
            Message = string.Empty;
            ResponseStream = null;
            ResponseXml = null;
            Batch = new List<DlapResponse>();
        }

        /// <summary>
        /// Uses the given stream to build a full DlapResponse
        /// </summary>
        /// <param name="rawData">Exact stream returns from DLAP</param>
        public DlapResponse(Stream rawData)
        {
            ResponseStream = rawData;
        }

        /// <summary>
        /// Uses the given document to build a DlapResponse
        /// </summary>
        /// <param name="rawData">XDocument containing the entire DLAP response</param>
        public DlapResponse(XDocument rawData)
        {
            ResponseXml = rawData;
            if (null != rawData)
                ParseXmlResponse(rawData);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Attempts to process the stream as XML and use the result to initialize the object
        /// </summary>
        /// <param name="rawData">Stream containing the response</param>
        public void ParseResponse(Stream rawData)
        {
            XDocument doc = XDocument.Load(rawData);

            if (null != doc)
            {
                ParseXmlResponse(doc);
            }
        }

        /// <summary>
        /// Extracts information from the XML response from DLAP and populate the object's methods
        /// </summary>
        /// <param name="doc">XML document to extract information from</param>
        protected void ParseXmlResponse(XDocument doc)
        {
            var code = doc.Root.Attribute("code");
            var message = doc.Root.Attribute("message");
            var batch = doc.Root.Element(RESPONSES_ELEMENT);
            var data = doc.Root.Elements();

            if ((doc.Root.Name == RESPONSE_ELEMENT || doc.Root.Name == RESPONSES_ELEMENT) && code != null)
            {
                Type dlapCodeType = typeof(DlapResponseCode);
                if (Enum.IsDefined(dlapCodeType, code.Value))
                {
                    try
                    {
                        Code = (DlapResponseCode)Enum.Parse(dlapCodeType, code.Value, true);
                    }
                    catch
                    {
                        //we swallow on purpose so that the command can either correct or throw a more meaningful exception
                        Code = DlapResponseCode.None;
                    }
                }
                else
                {
                    Code = DlapResponseCode.None;
                }

                if (null != message)
                {
                    Message = message.Value;
                }

                ResponseXml = new XDocument(data);

                if (Code == DlapResponseCode.ResourceNotFound)
                {
                    //this is a hack due to agilix changing how DLAP calls that can't find data behave
                    Code = DlapResponseCode.OK;
                    ResponseXml = XDocument.Parse(@"<item id=""DEFAULT""><data /></item>");
                }

                if (Code == DlapResponseCode.Solr)
                {
                    //this is a hack due to agilix changing how DLAP calls that can't find data behave
                    Code = DlapResponseCode.OK;
                }

                if (null != batch)
                {
                    Batch = from r in batch.Elements(RESPONSE_ELEMENT) select new DlapResponse(new XDocument(r));
                }
            }
            else
            {
                Code = DlapResponseCode.OK;
                var ms = new MemoryStream();
                var xw = new System.Xml.XmlTextWriter(ms, Encoding.UTF8);
                doc.WriteTo(xw);
                xw.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                ResponseStream = ms;
            }
        }

        #endregion

        #region IDlapResponseParser Members

        /// <summary>
        /// Empty implementation
        /// </summary>
        /// <param name="response">Response from DLAP to parse</param>
        public void ParseResponse(DlapResponse response) { }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Ensures that <see cref="ResponseStream" /> is cleaned up upon garbage collection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of resources during garbage collection
        /// </summary>
        /// <param name="disposing">true if the object is being disposed, false otherwise</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ResponseStream != null)
                {
                    try
                    {
                        ResponseStream.Close();
                    }
                    catch { }
                }
            }
        }

        #endregion
    }
}
