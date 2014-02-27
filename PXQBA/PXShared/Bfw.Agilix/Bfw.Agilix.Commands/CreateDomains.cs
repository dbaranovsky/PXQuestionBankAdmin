// -----------------------------------------------------------------------
// <copyright file="CreateDomains.cs" company="Macmillan Higher education">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Bfw.Agilix.Commands
{
    using System.Collections.Generic;
    using Bfw.Agilix.DataContracts;
    using Bfw.Agilix.Dlap;
    using Bfw.Agilix.Dlap.Session;

    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/CreateDomains command.
    /// This allow to create one domain at a time. In case, we need multiple domains to be created use Batch mode to create multiple and handle there.
    /// </summary>
    public class CreateDomains : DlapCommand
    {     
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the CreateDomains class with parent id
        /// </summary>
        /// <param name="parentId">Parent Domain id, it can be taken from Context.Course.DomainId</param>
        public CreateDomains(string parentId)
        {
            this.ParentId = parentId;
        }

        /// <summary>
        /// Prevents a default instance of the CreateDomains class from being created. 
        /// In case, we are allowed to create domain without parent id then modify it.
        /// </summary>
        private CreateDomains()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the domain that is going to be added/updated.
        /// </summary>
        public Domain Domain { get; set; }

        /// <summary>
        /// Gets or sets attribute of parentid that is required at the time of CreateDomain. 
        /// Parent id should already be existing to create association of parent - child association within domains.
        /// This becomes part of DLAP cmd parameter 
        /// </summary>
        public string ParentId { get; set; }

        #endregion

        #region Methods

        #endregion

        #region overrides from DlapCommand

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/CreateDomains command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        /// <remarks>XML response conforming to http://dev.dlap.bfwpub.com/Docs/Command/CreateDomains.</remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("CreateDomains request failed with response code {0}", response.Code));
            }

            string message = string.Empty;
            
            if (DlapResponseCode.OK != response.Code)
            {
                message = !string.IsNullOrEmpty(response.Message) ? response.Message : response.Code.ToString();
                throw new DlapException(message);
            }
            else
            {
                this.Domain.Id = response.ResponseXml.Element("responses").Element("response").Element("domain").Attribute("domainid").Value.ToString();
            }
        }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/CreateDomains command.
        /// </summary>
        /// <returns><see cref="DlapRequest"/> representing a DLAP command.</returns>
        /// <remarks>POST type request with XML body conforming to http://dev.dlap.bfwpub.com/Docs/Command/CreateDomains.</remarks>
        public override DlapRequest ToRequest()
        {
            if (this.Domain == null || string.IsNullOrEmpty(this.Domain.Name) || string.IsNullOrEmpty(this.Domain.Userspace))
            {
                throw new DlapException("Cannot create a Create Domain request if there are no domain attributes passed");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>() { { "cmd", "createdomains" } }
            };

            if (!string.IsNullOrEmpty(this.ParentId))
            {
                request.Parameters.Add("parentid", this.ParentId);
            }

            request.AppendData(this.Domain.ToEntity());

            return request;
        }

        #endregion
    }
}
