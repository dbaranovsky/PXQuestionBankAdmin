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
    /// Implementation of the GetResource commands
    /// </summary>
    public class GetResource : DlapCommand
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
        /// Resource that resulted from the call
        /// </summary>
        /// <value>
        /// The resource.
        /// </value>
        public Resource Resource { get; set; }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetResource command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetResource
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "GetResource" },
                    { "entityid", EntityId },
                    { "path", ResourcePath }
                }
            };

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetResource command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (null == response.ResponseXml)
            {
                ParseStreamResponse(response);
            }
        }

        /// <summary>
        /// Parses the stream response.
        /// </summary>
        /// <param name="response">The response.</param>
        protected void ParseStreamResponse(DlapResponse response)
        {
            Resource = new Resource()
            {
                EntityId = EntityId,
                Url = ResourcePath,
                ContentType = response.ContentType
            };

            var rstream = Resource.GetStream();
            response.ResponseStream.Copy(rstream);
            rstream.Flush();
        }
    }
}
