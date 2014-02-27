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
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/CreateGroups command.
    /// </summary>
    public class CreateGroups : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of groups that are going to be added/updated.
        /// </summary>
        public List<Group> Groups { get; protected set; }

        /// <summary>
        /// Id of the group that was created.
        /// </summary>
        public string GroupId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes command state.
        /// </summary>
        public CreateGroups()
        {
            Groups = new List<Group>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the group to the list of groups to create.
        /// </summary>
        /// <param name="group">Groupd to create.</param>
        public void Add(Group group)
        {
            Groups.Add(group);
        }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Parses response from http://dev.dlap.bfwpub.com/Docs/Command/CreateGroups command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        /// <remarks>XML response conforming to http://dev.dlap.bfwpub.com/Docs/Command/CreateGroups.</remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("Creategroups request failed with response code {0}", response.Code));
            }

            int index = 0;
            string message = string.Empty;
            foreach (var resp in response.Batch)
            {
                if (DlapResponseCode.OK != resp.Code)
                {
                    message = (!string.IsNullOrEmpty(resp.Message) ? resp.Message : resp.Code.ToString());
                }
                else
                {
                    GroupId = response.ResponseXml.Element("responses").Element("response").Element("group").Attribute("groupid").Value.ToString();
                }
                ++index;
            }
        }

        /// <summary>
        /// Builds command required by http://dev.dlap.bfwpub.com/Docs/Command/CreateGroups command.
        /// </summary>
        /// <returns><see cref="DlapRequest"/> representing a DLAP command.</returns>
        /// <remarks>POST type request with XML body conforming to http://dev.dlap.bfwpub.com/Docs/Command/CreateGroups.</remarks>
        public override DlapRequest ToRequest()
        {
            if (Groups.IsNullOrEmpty())
            {
                throw new DlapException("Cannot create a Create Groups request if there are no group in the Groups collection");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "creategroups" } }
            };

            foreach (var group in Groups)
            {
                request.AppendData(group.ToEntity());
            }
            return request;
        }

        #endregion
    }
}
