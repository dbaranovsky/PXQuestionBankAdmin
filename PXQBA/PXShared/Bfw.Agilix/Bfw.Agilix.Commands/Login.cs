using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/Login command
    /// </summary>
    public class Login : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Username of the user to login as.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password of the user logging in.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// User that was returned as the result of loging in
        /// </summary>
        public AgilixUser User { get; protected set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/Login command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/Login</returns>
        /// <remarks></remarks>
        public override Bfw.Agilix.Dlap.DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                ContentType = "text/xml",
                Attributes = new Dictionary<string, object>() {
                    { "cmd","login" },
                    { "username", Username },
                    { "password", Password }
                }
            };

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/Login command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(Bfw.Agilix.Dlap.DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapAuthenticationException(string.Format("Could not authenticate user {0}", Username));

            if ("user" == response.ResponseXml.Root.Name)
            {
                var user = new AgilixUser();
                user.ParseEntity(response.ResponseXml.Root);
                User = user;
            }
            else
            {
                throw new BadDlapResponseException(string.Format("Login command expected a response element of 'user', but got {0} instead.", response.ResponseXml.Root.Name));
            }
        }

        #endregion
    }
}
