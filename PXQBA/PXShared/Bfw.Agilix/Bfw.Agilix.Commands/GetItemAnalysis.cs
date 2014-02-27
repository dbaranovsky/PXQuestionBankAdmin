// -----------------------------------------------------------------------
// <copyright file="GetItemAnalysis.cs" company="Macmillan">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;

namespace Bfw.Agilix.Commands
{

    /// <summary>
    /// Provides an implementation of GetItemAnalysis Agilix command
    /// </summary>
    public class GetItemAnalysis : DlapCommand
    {
        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>
        /// The search parameters.
        /// </value>
        public ItemAnalysisSearch SearchParameters { get; set; }


        /// <summary>
        /// Gets or sets the item analysis.
        /// </summary>
        /// <value>
        /// The item analysis.
        /// </value>
        public IEnumerable<ItemAnalysisDetail> ItemAnalysis { get; protected set; }


        /// <summary>
        /// To the request.
        /// </summary>
        /// <returns></returns>
        public override DlapRequest ToRequest()
        {
            if (!(SearchParameters.EntityId.IsNullOrEmpty() || SearchParameters.ItemId.IsNullOrEmpty()))
            {
                var request = new DlapRequest()
                {
                    Type = DlapRequestType.Get,
                    Parameters = new Dictionary<string, object>() {
                        { "cmd", "GetItemAnalysis" },
                        { "entityid", SearchParameters.EntityId },
                        { "itemid", SearchParameters.ItemId },
                        { "verbose", SearchParameters.Verbose }
                       
                    }
                };
                if (!SearchParameters.GroupId.IsNullOrEmpty())
                {
                    request.Parameters["groupid"] = SearchParameters.GroupId;
                }
                if (!SearchParameters.EnrollmentId.IsNullOrEmpty())
                {
                    request.Parameters["enrollmentid"] = SearchParameters.EnrollmentId;
                }
                if (!SearchParameters.Summary.IsNullOrEmpty())
                {
                    request.Parameters["summary"] = SearchParameters.Summary;
                }
                if (!SearchParameters.SetId.IsNullOrEmpty())
                {
                    request.Parameters["setid"] = SearchParameters.SetId;
                }
                return request;
            }

            throw new InvalidOperationException("Invalid parameters for generating item analysis search request.");
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetItemAnalysis command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            ItemAnalysis = new List<ItemAnalysisDetail>();

            if (response.Code == DlapResponseCode.OK)
            {
                if (response.ResponseXml != null)
                {
                    var rubricElements = response.ResponseXml.Elements("analysis").Elements("details").Elements("detail");

                    foreach (var responseElement in rubricElements)
                    {
                        ItemAnalysisDetail individualItemAnalysis = new ItemAnalysisDetail();
                        individualItemAnalysis.ParseEntity(responseElement);
                        ((List<ItemAnalysisDetail>)ItemAnalysis).Add(individualItemAnalysis);
                    }
                }                
            }
        }
    }
}
