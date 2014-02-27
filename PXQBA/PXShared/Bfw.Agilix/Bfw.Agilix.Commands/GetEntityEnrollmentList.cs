using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

using Bfw.Common.Collections;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetEntityEnrollmentList command.
    /// </summary>
    [Obsolete("This class should probably be using the GetEntityEnrollmentList2 command.")]
    public class GetEntityEnrollmentList : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// ID of the user to find enrollments for.
        /// </summary>
        public EntitySearch SearchParameters { get; set; }

        /// <summary>
        /// The <see cref="Enrollment"/> objects found by the command.
        /// </summary>
        public List<Enrollment> Enrollments { get;  set; }

        #endregion

        #region Overrides from DlapCommand

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/.
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() { { "cmd", "getentityenrollmentlist" } }
            };

            if (!string.IsNullOrEmpty(SearchParameters.CourseId))
            {
                request.Parameters["entityid"] = SearchParameters.CourseId;
            }

            if (!string.IsNullOrEmpty(SearchParameters.UserId))
            {
                request.Parameters["cmd"] = "getuserenrollmentlist";
                request.Parameters["userid"] = SearchParameters.UserId;
                request.Parameters["schema"] = "2";
            }

            if (SearchParameters.AllStatus)
            {
                request.Parameters["allstatus"] = true;
            }

            if (!string.IsNullOrEmpty(SearchParameters.EnrollmentId))
            {
                request.Parameters["cmd"] = "getenrollment2";
                request.Parameters["enrollmentid"] = SearchParameters.EnrollmentId;
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/ command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse.</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                if (DlapResponseCode.AccessDenied == response.Code)
                {
                    AgilixUser single = null;
                    single = new AgilixUser();
                }
                else
                {
                    throw new DlapException(string.Format("GetEntityEnrollmentList command failed with code {0}", response.Code));
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
    }

        #endregion
}