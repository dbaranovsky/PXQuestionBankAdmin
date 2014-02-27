using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap;
using Bfw.Common.Collections;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmissionInfo command
    /// </summary>
    public class GetStudentSubmissionInfo : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Used to filter the submissions.
        /// </summary>
        public SubmissionSearch SearchParameters;

        /// <summary>
        /// Submissions that matched <see cref="SearchParameters" />.
        /// </summary>
        public IList<Submission> Submissions;

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmissionInfo command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmissionInfo</returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest {
                Type = DlapRequestType.Post,
                Mode = DlapRequestMode.Batch,
                Parameters = new Dictionary<string, object>()
            };

            request.Parameters.Add("cmd", "getstudentsubmissioninfo");

            if (!Submissions.IsNullOrEmpty())
            {
                foreach (var submission in Submissions)
                {
                    var subElement = new XElement("submission",
                                                  new XAttribute("enrollmentid", submission.EnrollmentId),
                                                  new XAttribute("itemid", submission.ItemId));
                    request.AppendData(subElement);
                }
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmissionInfo command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
                throw new DlapException(string.Format("GetStudentSubmissionInfo request failed with response code {0}", response.Code));

            var responseElements = response.ResponseXml.Descendants("response").ToList();
            
            for (var i = 0; i < responseElements.Count(); i++) {
                Submissions[i].Submitted = responseElements[i].Attribute("code").Value == "OK";
                var subElement = responseElements[i].Element("submission");
                if (subElement == null) continue;
                if (subElement.Attribute("version") != null) {
                    int version;
                    int.TryParse(subElement.Attribute("version").Value, out version);
                    Submissions[i].Version = version;
                }
            }
        }

        #endregion
    }
}
