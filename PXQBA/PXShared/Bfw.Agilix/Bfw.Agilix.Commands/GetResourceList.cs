using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implementation of the GetResourceList commands
    /// </summary>
    public class GetResourceList : DlapCommand
    {
        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the resource path.
        /// </summary>
        /// <value>
        /// The resource path.
        /// </value>
        public string ResourcePath { get; set; }

        /// <summary>
        /// True if only the resource's metadata should be returned, i.e. empty stream.
        /// False (default) otherwise.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [metadata only]; otherwise, <c>false</c>.
        /// </value>
        public bool MetadataOnly { get; set; }

        /// <summary>
        /// XPath syntax based query where / is assumed to be the data element of the
        /// resource.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Resource that resulted from the call
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public List<Resource> Resources { get; protected set; }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetResourceList command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetResourceList
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "GetResourceList" },
                    { "entityid", EntityId }
                }
            };

            if(!ResourcePath.IsNullOrEmpty())
                request.Parameters.Add("path", ResourcePath);
            
            if(!Query.IsNullOrEmpty())
                request.Parameters.Add("query", Query);

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetResourceList command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (null != response.ResponseXml)
            {
                Resources = ParseXmlResponse(response);
            }          
        }

        /// <summary>
        /// Parses out the list of items as expected by the GetResourceList DLAP request
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private List<Resource> ParseXmlResponse(DlapResponse response)
        {
            var listElm = response.ResponseXml.Root;
            var list = new List<Resource>();

            if (response.Code != DlapResponseCode.OK)
                return list;

            if (null != listElm && "resources" == listElm.Name)
            {
                Resource single = null;
                foreach (var rElm in listElm.Elements("resource"))
                {
                    single = new Resource();
                    single.ParseEntity(rElm);

                    list.Add(single);
                }
            }
            else
            {                
                throw new BadDlapResponseException(string.Format("GetResourceList expected a resources element, but none was found: {0}", response.ResponseXml.ToString()));
            }

            return list;
        }
    }
}
