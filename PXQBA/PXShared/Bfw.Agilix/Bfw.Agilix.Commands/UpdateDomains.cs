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
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/UpdateDomains command
    /// </summary>
    public class UpdateDomains : DlapCommand
    {
        #region Properties

        /// <summary>
        /// The list of items that are going to be added/updated
        /// </summary>
        public List<Domain> Domains { get; protected set; }

        /// <summary>
        /// All the responses from the call.
        /// </summary>
        public List<DlapResponse> Responses { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDomains"/> class.
        /// </summary>
        /// <remarks></remarks>
        public UpdateDomains()
        {
            Domains = new List<Domain>();
            Responses = new List<DlapResponse>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified domain to the list of domains to update.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <remarks></remarks>
        public void Add(Domain domain)
        {
            Domains.Add(domain);
        }

        /// <summary>
        /// Adds the specified domains to the list of domains to update.
        /// </summary>
        /// <param name="domains">The domains.</param>
        /// <remarks></remarks>
        public void Add(List<Domain> domains)
        {
            Domains = domains;
        }

        /// <summary>
        /// Clears this instance of all domains.
        /// </summary>
        /// <remarks></remarks>
        public void Clear()
        {
            Domains.Clear();
        }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/UpdateDomains command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/UpdateDomains</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            if (Domains.IsNullOrEmpty())
                throw new DlapException("Cannot create a UpdateDomains request if there are not Domains in the Domains collection");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "updatedomains" } }
            };

            foreach (var domain in Domains)
            {
                request.AppendData(domain.ToEntity());
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/UpdateDomains command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("UpdateDomains request failed with response code {0}", response.Code));

            if (response.IsBatch && !response.Batch.IsNullOrEmpty())
            {
                foreach (var resp in response.Batch)
                {
                    Responses.Add(resp);                    
                }
            }
        }

        #endregion
    }
}
