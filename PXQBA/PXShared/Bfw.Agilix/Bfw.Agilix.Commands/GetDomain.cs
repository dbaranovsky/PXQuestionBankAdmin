using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetDomain command.
    /// </summary>
    public class GetDomain : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// ID of the domain to load.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Domain loaded from response.
        /// </summary>
        public Domain Domain { get; protected set; }

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
            if (String.IsNullOrEmpty(DomainId))
            {
                throw new ArgumentException("GetDomain requires a non-empty domain id to load.");
            }

            var parameters = new Dictionary<string, object>();
            parameters.Add("cmd", "getdomain");
            parameters.Add("domainid", DomainId);

            return new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = parameters
            };
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("GetDomain command failed with code {0}", response.Code));
            }

            Domain = new Domain();
            Domain.ParseEntity(response.ResponseXml.Element("domain"));
        }

        #endregion
    }
}
