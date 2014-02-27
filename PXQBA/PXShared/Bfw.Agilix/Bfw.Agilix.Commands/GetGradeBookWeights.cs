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
    /// Encapsulates the GetEnrollmentGradebook2 (Docs/Command/GetGradeBookWeights),
    /// GetGradeBookWeights (Docs/Command/GetEnrollmentGradebook2), and GetGradeBookWeights
    /// (Docs/Command/GetGradeBookWeights) DLAP requests
    /// </summary>
    public class GetGradeBookWeights : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>
        /// The search parameters.
        /// </value>
        public GradeBookWeightSearch SearchParameters { get; set; }

        /// <summary>
        /// Gets or sets the grade book weights.
        /// </summary>
        /// <value>
        /// The grade book weights.
        /// </value>
        public GradeBookWeights GradeBookWeights { get; protected set; }

        #endregion

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetGradeBookWeights command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetGradeBookWeights
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>()
            };

            request.Parameters["cmd"] = "GetGradeBookWeights";
            request.Parameters["entityid"] = SearchParameters.EntityId;

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetGradeBookWeights command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.AccessDenied == response.Code)
            {
                throw new DlapAuthenticationException("getgradebookweights command failed with code AccessDenied");
            }

            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("getgradebookweights command failed with code {0}", response.Code));
            }

            GradeBookWeights = new GradeBookWeights();

            var weights = response.ResponseXml.Element("weights");

            GradeBookWeights q = new GradeBookWeights();
            q.ParseEntity(weights);
            GradeBookWeights = q;
        }
    }
}
