using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://http://dev.dlap.bfwpub.com/Docs/Command/AddGroupMembers command.
    /// </summary>
    public class AddGroupMembers : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Group member to add.
        /// </summary>
        public GroupMember GroupMember { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes command state.
        /// </summary>
        public AddGroupMembers()
        {
            GroupMember = new GroupMember();
        }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Parses the response of the http://http://dev.dlap.bfwpub.com/Docs/Command/AddGroupMembers command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        /// <remarks>Expects XML response as described in http://http://dev.dlap.bfwpub.com/Docs/Command/AddGroupMembers. </remarks>
        public override void ParseResponse(DlapResponse response)
        {
            string message = string.Empty;

            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("Addgroupmembers request failed with response code {0}", response.Code));
            }
            
            foreach (var resp in response.Batch)
            {
                if (DlapResponseCode.OK != resp.Code)
                {
                    message = (!string.IsNullOrEmpty(resp.Message) ? resp.Message : resp.Code.ToString());
                }                
            }
        }

        /// <summary>
        /// Produces request format required by http://http://dev.dlap.bfwpub.com/Docs/Command/AddGroupMembers command.
        /// </summary>
        /// <returns><see cref="DlapRequest"/> representing a DLAP command.</returns>
        /// <remarks>POST of XML conforming to http://http://dev.dlap.bfwpub.com/Docs/Command/AddGroupMembers command.</remarks>
        public override DlapRequest ToRequest()
        {
            if (GroupMember == null)
            {
                throw new DlapException("Cannot Add a group to the Member");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "addgroupmembers" } }
            };

            request.AppendData(GroupMember.ToEntity());

            return request;
        }

        #endregion
    }
}
