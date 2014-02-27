using System;
using System.Collections.Generic;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Implements the http://dev.dlap.bfwpub.com/Docs/Command/GetCourse and 
    /// http://dev.dlap.bfwpub.com/Docs/Command/GetCourseList commands.
    /// </summary>
    public class GetCourse : DlapCommand
    {
        #region Data Members

        /// <summary>
        /// Parameters for the search.
        /// </summary>
        public CourseSearch SearchParameters { get; set; }

        /// <summary>
        /// Courses found as a result of the request.
        /// </summary>
        public IEnumerable<Course> Courses { get; set; }

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
                Parameters = new Dictionary<string, object>()
            };

            // If the search parameters have a course id, then use that (GetCourse).
            if (!String.IsNullOrEmpty(SearchParameters.Query))
            {
                request.Parameters.Add("cmd", "getcourselist");
                request.Parameters.Add("query", SearchParameters.Query);

                if (!string.IsNullOrEmpty(SearchParameters.DomainId))
                {
                    request.Parameters.Add("domainid", SearchParameters.DomainId);
                }
            }
            else if (!String.IsNullOrEmpty(SearchParameters.CourseId))
            {
                request.Parameters.Add("cmd", "getcourse");
                request.Parameters.Add("courseid", SearchParameters.CourseId);
            }
            else if (null != SearchParameters.Title)
            {
                request.Parameters.Add("cmd", "getcourselist");
                request.Parameters.Add("title", SearchParameters.Title);
            }
            else if (null != SearchParameters.DomainId)
            {
                request.Parameters.Add("cmd", "getcourselist");
                request.Parameters.Add("domainid", SearchParameters.DomainId);
            }
            else
            {
                request.Parameters.Add("cmd", "getcourselist");
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
                var responseBody = response.ResponseXml.ToString();
                throw new DlapException(string.Format("GetCourse or GetCourseList command failed with code {0}, body {1}", response.Code, responseBody));
            }

            Courses = new List<Course>();

            var coursesElement = response.ResponseXml.Element("courses");
            var courseElements = coursesElement != null ? coursesElement.Elements("course") : response.ResponseXml.Elements("course");

            foreach (var courseElement in courseElements)
            {
                Course c = new Course();
                c.ParseEntity(courseElement);
                ((IList<Course>)Courses).Add(c);
            }
        }

        #endregion
    }
}
