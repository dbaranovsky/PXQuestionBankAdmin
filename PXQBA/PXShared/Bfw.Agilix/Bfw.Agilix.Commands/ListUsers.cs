using System;
using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the ListUsers DLAP command (http://dlap.bfwpub.com/js/docs/#!/Command/ListUsers).
    /// </summary>
    public class ListUsers : DlapCommand
    {
        /// <summary>
        /// Defaults to zero which searches against all domains the executing user
        /// has rights to. Otherwise should be set to the ID of the domain to search
        /// user information from.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Allows for XML Query as documented in DLAP.
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Contains the list of AgilixUser object resulting from executing the 
        /// command as configured. This property will be null until after ParseResponse
        /// has been called.
        /// </summary>
        public List<AgilixUser> Users { get; set; }

        public ListUsers()
        {
            DomainId = "0";
        }

        /// <summary>
        /// Create a DlapRequest object representing this command.
        /// </summary>
        /// <returns>DlapRequest object representing this command and parameters.</returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "listusers" } }
            };

            if (string.IsNullOrEmpty(DomainId))
            {
                throw new ArgumentException("Must specify a DomainId");
            }

            request.Parameters["domainid"] = DomainId;

            if (!string.IsNullOrEmpty(Query))
            {
                request.Parameters["query"] = Query;
            }

            return request;
        }

        /// <summary>
        /// Parses the DLAP results from executing a ListUsers DLAP command.
        /// </summary>
        /// <param name="response">DlapResponse object containing raw DLAP response.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (response.Code != DlapResponseCode.OK)
            {
                throw new DlapException(string.Format("ListUsers command failed with code {0}", response.Code));
            }

            if ("users" == response.ResponseXml.Root.Name)
            {
                Users = new List<AgilixUser>();
                AgilixUser user;
                foreach (var userElm in response.ResponseXml.Root.Elements("user"))
                {
                    user = new AgilixUser();
                    user.ParseEntity(userElm);
                    Users.Add(user);
                }
            }
        }
    }
}
