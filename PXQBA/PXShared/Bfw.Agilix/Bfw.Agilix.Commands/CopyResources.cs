using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides an implementation of http://dev.dlap.bfwpub.com/Docs/Command/CopyResources command.
    /// </summary>
    public class CopyResources : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the copy results.
        /// </summary>
        /// <value>The copy results.</value>
        /// <remarks>XML response from DLAP.</remarks>
        public XDocument CopyResults { get; protected set; }

        /// <summary>
        /// Gets or sets the dest entity ID.
        /// </summary>
        /// <value>The dest entity id.</value>
        /// <remarks>Course or Domain the resource will be copied to.</remarks>
        public string DestEntityId { get; set; }

        /// <summary>
        /// Gets or sets the dest path.
        /// </summary>
        /// <value>The dest path.</value>
        /// <remarks>Unique path to the resource in the destination entity.</remarks>
        public string DestPath { get; set; }

        /// <summary>
        /// Gets or sets the source entity ID.
        /// </summary>
        /// <value>The source entity id.</value>
        /// <remarks>Course or Domain that owns the resource.</remarks>
        public string SourceEntityId { get; set; }

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        /// <value>The source path.</value>
        /// <remarks>Unique path to the resource in the source entity.</remarks>
        public string SourcePath { get; set; }

        #endregion

        #region Methods
        
        /// <summary>
        /// Builds the requests element.
        /// </summary>
        /// <returns>Element that serves at the request POST body.</returns>
        /// <remarks>conforms to http://dev.dlap.bfwpub.com/Docs/Command/CopyResources.</remarks>
        private XElement BuildRequestsElement()
        {
            XElement requestsElement = new XElement("requests");
            XElement resourceElement = new XElement("resource");

            resourceElement.SetAttributeValue("sourceentityid", SourceEntityId);
            resourceElement.SetAttributeValue("destinationentityid", DestEntityId);
            resourceElement.SetAttributeValue("destinationpath", DestPath);
            resourceElement.SetAttributeValue("sourcepath", SourcePath);
            requestsElement.Add(resourceElement);

            return requestsElement;
        }

        #endregion

        #region Overrides DlapCommand

        /// <summary>
        /// Parses response from http://dev.dlap.bfwpub.com/Docs/Command/CopyResources.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        /// <remarks>expects XML that conforms to  http://dev.dlap.bfwpub.com/Docs/Command/CopyResources.</remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (null != response.ResponseXml)
            {
                CopyResults = response.ResponseXml;
            }
        }

        /// <summary>
        /// Builds request required by  http://dev.dlap.bfwpub.com/Docs/Command/CopyResources.
        /// </summary>
        /// <returns><see cref="DlapRequest"/> representing a DLAP command<./returns>
        /// <remarks>POST request with XML body that conforms to  http://dev.dlap.bfwpub.com/Docs/Command/CopyResources.</remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Post,
                Parameters = new Dictionary<string, object>() { { "cmd", "copyresources" } }
            };

            if (!string.IsNullOrEmpty(SourceEntityId))
            {
                request.Parameters["sourceentityid"] = SourceEntityId;
            }
            if (!string.IsNullOrEmpty(DestEntityId))
            {
                request.Parameters["destinationentityid"] = DestEntityId;
            }
            if (!string.IsNullOrEmpty(SourcePath))
            {
                request.Parameters["sourcepath"] = SourcePath;
            }
            if (!string.IsNullOrEmpty(DestPath))
            {
                request.Parameters["destinationpath"] = DestPath;
            }

            request.AppendData(BuildRequestsElement());

            return request;
        }

        #endregion
    }
}
