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
    /// Encapsulates the GetEnrollmentGradebook2 (Docs/Command/GetEnrollmentGradebook2),
    /// GetUserGradebook2 (Docs/Command/GetEnrollmentGradebook2), and GetEntityGradebook2
    /// (Docs/Command/GetEntityGradebook2) DLAP requests
    /// </summary>
    public class GetGrades : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// Gets or sets the search parameters.
        /// </summary>
        /// <value>
        /// The search parameters.
        /// </value>
        public GradeSearch SearchParameters { get; set; }

        /// <summary>
        /// Gets or sets the enrollments.
        /// </summary>
        /// <value>
        /// The enrollments.
        /// </value>
        public IEnumerable<Enrollment> Enrollments { get; set; }

        #endregion

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetEnrollmentGradebook2 command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetEnrollmentGradebook2
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>()
            };
            
            // The DLAP command we need to use depends on the search parameters offered:
            // * If we have an Enrollment ID, then use GetEnrollmentGradebook2
            // * If we have a user ID, then use GetUserGradebook2 (could also have an Entity ID; it should also be used)
            // * If we have an Entity ID, then we can use GetEntityGradebook2
            // * If we have none of those, throw an exception
            if (!String.IsNullOrEmpty(SearchParameters.EnrollmentId))
            {
                request.Parameters["cmd"] = "GetEnrollmentGradebook2";
                SearchParameters.CommandRequested = "GetEnrollmentGradebook2";
                request.Parameters["enrollmentid"] = SearchParameters.EnrollmentId;
                
            }
            else if (!String.IsNullOrEmpty(SearchParameters.UserId))
            {
                request.Parameters["cmd"] = "GetUserGradebook2";
                request.Parameters["userid"] = SearchParameters.UserId;
                SearchParameters.CommandRequested = "GetUserGradebook2";

                if (!String.IsNullOrEmpty(SearchParameters.EntityId))
                {
                    request.Parameters["entityid"] = SearchParameters.EntityId;
                }
            }
            else if (!String.IsNullOrEmpty(SearchParameters.EntityId))
            {
                request.Parameters["cmd"] = "GetEntityGradebook2";
                SearchParameters.CommandRequested = "GetEntityGradebook2";
                request.Parameters["entityid"] = SearchParameters.EntityId;
            }
            else
            {
                throw new ArgumentException(@"Grade search parameters supplied do not contain enough information to do a search;
                    need one of Entity ID, User ID, or Enrollment ID");
            }

            // If any Item IDs were provided in the search parameters, then format and use them, otherwise,
            // use '*', which gets grades for all items with grades.
            if (null != SearchParameters.ItemIds)
            {
                request.Parameters["itemid"] = String.Join("|", SearchParameters.ItemIds.ToArray());
            }
            else
            {
                request.Parameters["itemid"] = "*";
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetEnrollmentGradebook2 command.
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            Enrollments = new List<Enrollment>();
            bool isGetEnrollmentGradebook2 = SearchParameters.CommandRequested == "GetEnrollmentGradebook2";
            if (isGetEnrollmentGradebook2 && response.ResponseXml != null)
            {
                if (response.ResponseXml.Elements("enrollment") != null)
                {
                    var enrollmentElements = response.ResponseXml.Elements("enrollment");

                    foreach (XElement enrollmentElement in enrollmentElements)
                    {
                        Enrollment e = new Enrollment();
                        e.ParseEntity(enrollmentElement);
                        ((List<Enrollment>)Enrollments).Add(e);
                    }
                }

            }
            else
            {
                if (null != response.ResponseXml)
                {
                    var enrollmentsElement = response.ResponseXml.Element("enrollments");

                    if (null != enrollmentsElement)
                    {
                        var enrollmentElements = enrollmentsElement.Elements("enrollment");
                        if (enrollmentElements != null)
                        {
                            foreach (XElement enrollmentElement in enrollmentElements)
                            {
                                Enrollment e = new Enrollment();
                                e.ParseEntity(enrollmentElement);
                                ((List<Enrollment>)Enrollments).Add(e);
                            }
                        }
                    }
                }
            }
        }
    }
}
