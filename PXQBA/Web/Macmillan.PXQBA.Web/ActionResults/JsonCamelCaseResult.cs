using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Macmillan.PXQBA.Web.ActionResults
{
    /// <summary>
    /// Serialize object to json in camel notation.
    /// See http://www.matskarlsson.se/blog/serialize-net-objects-as-camelcase-json
    /// </summary>
    public class JsonCamelCaseResult : ActionResult
    {
        public JsonCamelCaseResult()
        {
            JsonRequestBehavior = JsonRequestBehavior.DenyGet;
        }

        /// <summary>
        /// Encoding of the content to use
        /// </summary>
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        /// Content type
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Data to be returned
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Specifies wheather GET request from client is allowed
        /// </summary>
        public JsonRequestBehavior JsonRequestBehavior { get; set; }

        /// <summary>
        /// Executes result
        /// </summary>
        /// <param name="context">Controller contex</param>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet && String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("This request has been blocked because sensitive information could be disclosed to third party web sites when this is used in a GET request. To allow GET requests, set JsonRequestBehavior to AllowGet.");
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = !String.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null)
            {
                var jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                response.Write(JsonConvert.SerializeObject(Data, Formatting.Indented, jsonSerializerSettings));
            }
        }
    }
}