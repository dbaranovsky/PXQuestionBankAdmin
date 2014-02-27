using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides an implementation of GetGrade Agilix command
    /// </summary>
    public class GetGradeList : DlapCommand
    {
        /// <summary>
        /// Gets or sets the search parameter.
        /// </summary>
        /// <value>
        /// The search parameter.
        /// </value>
        public GradeListSearch SearchParameters { get; set; }

        /// <summary>
        /// Gets or sets the grade list.
        /// </summary>
        /// <value>
        /// The grade list.
        /// </value>
        public GradeList GradeList { get; protected set; }

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetGrade command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetGrade
        /// </returns>
        public override DlapRequest ToRequest()
        {
            if (string.IsNullOrWhiteSpace(SearchParameters.EnrollmentId) ||
                string.IsNullOrWhiteSpace(SearchParameters.ItemId))
                throw new ArgumentException(@"GradeList requires both EnrollmentId and ItemId to do a proper search");

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() {
                    { "cmd", "GetGrade" },
                    { "enrollmentid", SearchParameters.EnrollmentId },
                    { "itemid", SearchParameters.ItemId }
                }
            };

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetGrade command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (response.ResponseXml != null)
            {
                GradeList = new DataContracts.GradeList();
                GradeList.ParseEntity(response.ResponseXml.Element("grade"));
            }
        }
    }
}
