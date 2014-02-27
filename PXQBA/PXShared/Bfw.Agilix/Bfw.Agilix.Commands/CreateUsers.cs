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
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/CreateUsers command.
    /// </summary>
    public class CreateUsers : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of users that are going to be added/updated.
        /// </summary>
        public List<AgilixUser> Users { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes command state.
        /// </summary>
        public CreateUsers()
        {
            Users = new List<AgilixUser>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a user to create.
        /// </summary>
        /// <param name="user">User to create.</param>
        public void Add(AgilixUser user)
        {
            Users.Add(user);
        }

        /// <summary>
        /// Clears list of users to create.
        /// </summary>
        /// <value>Clear <see cref="Users" />.</value>
        public void Clear()
        {
            Users.Clear();
        }

        #endregion

        #region overrides from DlapCommand

        /// <summary>
        /// Parses response from http://dev.dlap.bfwpub.com/Docs/Command/CreateUsers command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        /// <remarks>XML response conforming to http://dev.dlap.bfwpub.com/Docs/Command/CreateUsers.</remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("CreateUsers request failed with response code {0}", response.Code));
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
                    Users[index].Id = resp.ResponseXml.Element("user").Attribute("userid").Value.ToString();
                }
                ++index;
            }
        }

        /// <summary>
        /// Builds request required by http://dev.dlap.bfwpub.com/Docs/Command/CreateUsers.
        /// </summary>
        /// <returns><see cref="DlapRequest"/> representing a DLAP command.</returns>
        /// <remarks>POST type command conforming to http://dev.dlap.bfwpub.com/Docs/Command/CreateUsers.</remarks>
        public override DlapRequest ToRequest()
        {
            if (Users.IsNullOrEmpty())
            {
                throw new DlapException("Cannot create a CreateUsers request if there are no user in the Users collection");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "createusers" } }
            };

            foreach (var user in Users)
            {
                request.AppendData(user.ToEntity());
            }

            return request;
        }

        #endregion
    }
}
