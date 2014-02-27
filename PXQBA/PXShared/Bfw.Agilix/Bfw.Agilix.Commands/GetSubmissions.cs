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
    /// Implement the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmissionHistory command.
    /// </summary>
    public class GetSubmissions : DlapCommand
    {
        #region Properties

        /// <summary>
        /// Filters the submissions.
        /// </summary>
        public SubmissionSearch SearchParameters;

        /// <summary>
        /// Submissions that matched <see cref="SearchParameters" />.
        /// </summary>
        public IList<Submission> Submissions;

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmissionHistory command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmissionHistory</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>()
            };

            request.Parameters.Add("cmd", "GetStudentSubmissionHistory");
            request.Parameters.Add("enrollmentid", SearchParameters.EnrollmentId);
            request.Parameters.Add("itemid", SearchParameters.ItemId);

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetStudentSubmissionHistory command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            Submissions = new List<Submission>();

            if (response.Code != DlapResponseCode.OK)
                throw new DlapException(string.Format("Command failed:{0} ", response.Message));

            var submissionsElement = response.ResponseXml.Element("submissions");

            if (submissionsElement != null)
            {
                foreach (var submissionElement in submissionsElement.Elements("submission"))
                {
                    var userElement = submissionElement.Element("user");
                    var submission = new Submission();

                    // If the submission has not been graded, don't add the grade object
                    if (submissionElement.Attribute("achieved") != null)
                    {
                        Grade g = new Grade();
                        g.ParseEntity(submissionElement);
                        submission.Grade = g;
                    }

                    var submitteddate = submissionElement.Attribute("submitteddate");
                    if (submitteddate != null)
                    {
                        DateTime dt;
                        if (DateTime.TryParse(submitteddate.Value, out dt))
                        {
                            submission.SubmittedDate = dt;
                        }
                    }

                    if (userElement != null)
                    {
                        submission.StudentFirstName = userElement.Attribute("firstname").Value;
                        submission.StudentLastName = userElement.Attribute("lastname").Value;
                    }

                    var version = submissionElement.Attribute("version");
                    if (version != null)
                    {
                        int v;
                        if (Int32.TryParse(version.Value, out v))
                        {
                            submission.Version = v;
                        }
                    }

                    Submissions.Add(submission);
                }
            }
        }

        #endregion
    }
}
