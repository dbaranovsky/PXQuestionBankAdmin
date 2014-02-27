using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using Ionic.Zip;

using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/PutResource command
    /// </summary>
    public class PutResource : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        /// <value>The resource.</value>
        /// <remarks></remarks>
        public Resource Resource { get; set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/PutResource command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/PutResource</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (null == Resource)
                throw new ArgumentException("PutResource command expects a resource to be provided");

            if (string.IsNullOrEmpty(Resource.Url))
                throw new ArgumentException("PutResource command expects the resource to have a Url");

            if (string.IsNullOrEmpty(Resource.EntityId))
                throw new ArgumentException("PutResource command expects the resource to have an EntityId");

            var rstream = Resource.GetStream();

            if (null == rstream && rstream.Length <= 0)
                throw new ArgumentException("PutResource command can not store an empty resource stream");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                ContentType = Resource.ContentType,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "putresource" },
                    { "entityid", Resource.EntityId },
                    { "path", Resource.Url },
                    { "status", Resource.Status.ToString() }
                }
            };

            request.AppendData(rstream);

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/PutResource command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("could not store resource at path {0} with message {1}", Resource.Url, response.Message));
        }

        #endregion
    }
}
