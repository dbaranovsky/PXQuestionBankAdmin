using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/DeleteResources command.
    /// </summary>
    public class DeleteResources : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// List of resources to delete.
        /// </summary>
        public IList<Resource> ResourcesToDelete { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteResources"/> class.
        /// </summary>
        public DeleteResources()
        {
            ResourcesToDelete = new List<Resource>();
        }

        #endregion

        #region DlapCommand Overrides

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/.
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Parameters = new Dictionary<string, object>() { { "cmd", "deleteresources" } },
                Mode = DlapRequestMode.Batch
            };

            if (!ResourcesToDelete.IsNullOrEmpty())
            {
                foreach (var resource in ResourcesToDelete)
                {
                    request.AppendData(resource.ToEntity());
                }
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("DeleteResources request failed with response code {0}", response.Code));
            }
        }

        #endregion
    }
}
