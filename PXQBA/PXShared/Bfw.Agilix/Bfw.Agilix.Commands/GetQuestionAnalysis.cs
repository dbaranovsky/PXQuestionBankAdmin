using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using System.Xml.Linq;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides and implementation of the GetQuestionAnalysis Agilix commands
    /// </summary>
    public class GetQuestionAnalysis : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// Gets or sets the entity id.
        /// </summary>
        /// <value>
        /// The entity id.
        /// </value>
        public string EntityId { get; set; }

        /// <summary>
        /// Gets or sets the item id.
        /// </summary>
        /// <value>
        /// The item id.
        /// </value>
        public string ItemId { get; set; }

        /// <summary>
        /// Gets or sets the question analysis.
        /// </summary>
        /// <value>
        /// The question analysis.
        /// </value>
        public IEnumerable<QuestionAnalysis> QuestionAnalysis { get; protected set; }

        #endregion

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionAnalysis command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionAnalysis
        /// </returns>
        public override DlapRequest ToRequest()
        {
            if (!String.IsNullOrEmpty(EntityId))
            {
                var request = new DlapRequest()
                {
                    Type = DlapRequestType.Get,
                    Parameters = new Dictionary<string, object>()
                };

                request.Parameters["cmd"] = "GetQuestionAnalysis";
                request.Parameters["entityid"] = EntityId;
                request.Parameters["itemid"] = ItemId;                

                return request;
            }

            throw new InvalidOperationException("Invalid parameters for generating question analysis search request.");
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetQuestionAnalysis command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            QuestionAnalysis = new List<QuestionAnalysis>();

            if (response.Code == DlapResponseCode.OK)
            {
                var questionElements = response.ResponseXml.Elements("analysis").Elements("details").Elements("detail");

                foreach (var responseElement in questionElements)
                {
                    QuestionAnalysis q = new QuestionAnalysis();
                    q.ParseEntity(responseElement);
                    ((List<QuestionAnalysis>)QuestionAnalysis).Add(q);
                }
            }
        }
    }
}
