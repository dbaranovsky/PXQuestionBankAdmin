using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides an implementation of http://dev.dlap.bfwpub.com/Docs/Command/GetUserEnrollmentList2 Agilix command
    /// </summary>
    public class GetUserEnrollmentList : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>The search parameters.</value>
        /// <remarks>Filters the enrollments.</remarks>
        public EntitySearch SearchParameters { get; set; }

        /// <summary>
        /// Enrollments that match the <see cref="SearchParameters" />.
        /// </summary>
        public List<Enrollment> Enrollments = new List<Enrollment>();

        /// <summary>
        /// Defaults to false, if set to true all enrollments are returned regardless of status.
        /// </summary>
        public bool? AllStatus { get; set; }

        #endregion

        #region override DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetUserEnrollmentList2 command.
        /// </summary>
        /// <returns>Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetUserEnrollmentList2</returns>
        /// <remarks></remarks>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "getuserenrollmentlist2" } }
            };

            if (!string.IsNullOrEmpty(SearchParameters.CourseId))
            {
                request.Parameters["entityid"] = SearchParameters.CourseId;
            }

            if (!string.IsNullOrEmpty(SearchParameters.UserId))
            {
                request.Parameters["userid"] = SearchParameters.UserId;
            }


            if (!String.IsNullOrEmpty(SearchParameters.Query))
            {
                request.Parameters.Add("query", SearchParameters.Query);
            }

            if (!String.IsNullOrEmpty(SearchParameters.Flags))
            {
                request.Parameters.Add("flags", SearchParameters.Flags);
            }

            if (AllStatus.HasValue)
            {
                request.Parameters["allstatus"] = AllStatus.Value;
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetUserEnrollmentList2 command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        /// <remarks></remarks>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                if (DlapResponseCode.AccessDenied == response.Code)
                {
                    AgilixUser single = null;
                    single = new AgilixUser();
                    //Users.Add(single);  
                }
                else if (DlapResponseCode.BadRequest == response.Code || DlapResponseCode.Error == response.Code)
                {
                    //most likely the enrollment does not exist
                    Enrollments = new List<Enrollment>();
                }
                else
                {
                    throw new DlapException(string.Format("GetUserEnrollmentList command failed with code {0}.", response.Code));
                }
            }
            else
            {
                Enrollment single = null;
                if ("enrollments" == response.ResponseXml.Root.Name)
                {
                    Enrollments = new List<Enrollment>();
                    foreach (var userElm in response.ResponseXml.Root.Elements("enrollment"))
                    {
                        single = new Enrollment();
                        single.ParseEntity(userElm);
                        Enrollments.Add(single);
                    }
                }
                else if ("enrollment" == response.ResponseXml.Root.Name)
                {
                    single = new Enrollment();
                    single.ParseEntity(response.ResponseXml.Root);
                    Enrollments = new List<Enrollment>() { single };
                }
                else
                {
                    throw new BadDlapResponseException(string.Format("GetEntityEnrollmentList expected a response element of 'user' or 'users', but got '{0}' instead.", response.ResponseXml.Root.Name));
                }
            }
        }

        #endregion
    }
}