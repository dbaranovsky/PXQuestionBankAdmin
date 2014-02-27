using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.Commands
{
    /// <summary>
    /// Provides implementation of the GetDueSoonList DLAP command
    /// </summary>
    public class GetDueSoonList : DlapCommand
    {
        /// <summary>
        /// Parameters for the search results
        /// </summary>
        /// <value>
        /// The search parameters.
        /// </value>
        public DueSoonSearch SearchParameters { get; set; }

        /// <summary>
        /// List of grades found as a result of the request
        /// </summary>
        /// <value>
        /// The grades.
        /// </value>
        public IEnumerable<Grade> Grades { get; set; }

        /// <summary>
        /// Initializes new instance
        /// </summary>
        public GetDueSoonList()
        {
            SearchParameters = new DueSoonSearch()
            {

            };
        }

        public override DlapRequest ToRequest()
        {
            if (string.IsNullOrEmpty(SearchParameters.UserId) && string.IsNullOrEmpty(SearchParameters.EnrollmentId))
            {
                throw new DlapException("GetDueSoonList command requires either User Id or Enrollment Id");
            }

            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>() 
                { 
                    { "cmd", "getduesoonlist" },
                    { "showcompleted", SearchParameters.ShowCompleted },
                    { "showpastdue", SearchParameters.ShowPastDue },
                    { "days", SearchParameters.Days },
                    { "utcoffset", SearchParameters.UtcOffSet }
                }
            };

            if (string.IsNullOrEmpty(SearchParameters.UserId))
            {
                request.Parameters.Add("enrollmentid", SearchParameters.EnrollmentId);
            }
            else
            {
                request.Parameters.Add("userid", SearchParameters.UserId);
            }

            return request;
        }

        public override void ParseResponse(DlapResponse response)
        {
            if (DlapResponseCode.OK != response.Code)
            {
                throw new DlapException(string.Format("GetDueSoonList command failed with code {0}", response.Code));
            }

            Grade single = null;
            var grades = new List<Grade>();

            if ("items" == response.ResponseXml.Root.Name)
            {
                foreach (var itemElm in response.ResponseXml.Root.Elements("item"))
                {
                    single = new Grade();
                    single.ParseEntity(itemElm);
                    grades.Add(single);
                }
            }
            else
            {
                throw new BadDlapResponseException(string.Format("GetDueSoonList command expected a response element of 'items', but got {0} instead.", response.ResponseXml.Root.Name));
            }

            Grades = grades;
        }
    }
}
