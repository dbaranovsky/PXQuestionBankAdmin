using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Common;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides an implementation of both the http://dev.dlap.bfwpub.com/Docs/Command/GetUser and 
    /// http://dev.dlap.bfwpub.com/Docs/Command/GetUserList commands.
    /// </summary>
    public class GetUsers : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>The search parameters.</value>
        /// <remarks>Filters the users.</remarks>
        public UserSearch SearchParameters { get; set; }

        /// <summary>
        /// Gets or sets the users.
        /// </summary>
        /// <value>The users.</value>
        /// <remarks>Users that match <see cref="SearchParameters" />.</remarks>
        public List<AgilixUser> Users { get; protected set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetUser or http://dev.dlap.bfwpub.com/Docs/Command/GetUserList command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetUser or http://dev.dlap.bfwpub.com/Docs/Command/GetUserList</returns>
        /// <remarks>Command executed depends on the <see cref="SearchParameters" />.</remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "getuser" } }
            };

            if (!string.IsNullOrEmpty(SearchParameters.Id))
            {
                request.Parameters["userid"] = SearchParameters.Id;
            }
            else
            {
                request.Parameters["cmd"] = "getuserlist";

                if (!string.IsNullOrEmpty(SearchParameters.Name))
                    request.Parameters["name"] = SearchParameters.Name;

                if (!string.IsNullOrEmpty(SearchParameters.DomainId))
                    request.Parameters["domainid"] = SearchParameters.DomainId;

                if (!string.IsNullOrEmpty(SearchParameters.ExternalId))
                {
                    request.Parameters["username"] = SearchParameters.ExternalId;
                }
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetUser or http://dev.dlap.bfwpub.com/Docs/Command/GetUserList command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            AgilixUser single = new AgilixUser();
            if (DlapResponseCode.OK != response.Code)
            {
                if (DlapResponseCode.BadRequest == response.Code && response.Message.Contains("Bad ExtendedId"))
                {
                    single = null;
                    Users = new List<AgilixUser>();
                    Users.Add(single);
                }
                else if (DlapResponseCode.AccessDenied == response.Code)
                {
                    single = null;
                    Users = new List<AgilixUser>();
                    Users.Add(single);
                }
                else
                {
                    throw new DlapException(string.Format("GetUsers command failed with code {0}", response.Code));
                }
            }
            else
            {
                single = null;
                if ("users" == response.ResponseXml.Root.Name)
                {
                    Users = new List<AgilixUser>();
                    foreach (var userElm in response.ResponseXml.Root.Elements("user"))
                    {
                        single = new AgilixUser();
                        single.ParseEntity(userElm);
                        Users.Add(single);
                    }
                }
                else if ("user" == response.ResponseXml.Root.Name)
                {
                    single = new AgilixUser();
                    single.ParseEntity(response.ResponseXml.Root);
                    Users = new List<AgilixUser>() { single };
                }
                else
                {
                    throw new BadDlapResponseException(string.Format("GetUsers expected a response element of 'user' or 'users', but got '{0}' instead.", response.ResponseXml.Root.Name));
                }
            }
        }

        #endregion
    }
}
