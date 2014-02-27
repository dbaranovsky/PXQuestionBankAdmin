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
    /// Provides and implementation of the GetItem Agilix commands
    /// </summary>
    public class GetRawItemList : DlapCommand
    {
        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public string EntityId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public string Query { get; set; }

        /// <summary>
        /// Gets the item document.
        /// </summary>
        public XDocument ItemDocument { get; private set; }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetItem command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetItem
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest();
            request.Parameters = new Dictionary<string, object>();
            request.Parameters.Add("cmd", "getitemlist");
            request.Parameters.Add("entityid", EntityId);
            if (!string.IsNullOrEmpty(ItemId))
            {
                request.Parameters.Add("itemid", ItemId);
            }
            request.Parameters.Add("query", Query);
            request.Type = DlapRequestType.Post;
            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetItem command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("GetRawItem command failed with code {0}", response.Code));
            }

            ItemDocument = response.ResponseXml;
        }
    }
}
