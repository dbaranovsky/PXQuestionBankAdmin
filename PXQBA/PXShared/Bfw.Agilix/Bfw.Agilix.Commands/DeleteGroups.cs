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
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/DeleteGroups command.
    /// </summary>
    public class DeleteGroups : DlapCommand
    {
        #region Properties

        /// <summary>
        /// IDs of the groups to delete.
        /// </summary>
        public IEnumerable<int> GroupIds { get; set; }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/.
        /// </returns>
        public override DlapRequest ToRequest()
        {
            if (GroupIds == null)
            {
                GroupIds = new List<int>();
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "deletegroups" } }
            };

            var requestsEl = new XElement("requests");

            foreach (var groupId in GroupIds)
            {
                var groupEl = new XElement("group");
                groupEl.Add(new XAttribute("groupid", groupId));
                request.AppendData(groupEl);
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
                throw new DlapException(string.Format("DeleteGroups request failed with response code {0}", response.Code));
            }
        }

        #endregion
    }
}
