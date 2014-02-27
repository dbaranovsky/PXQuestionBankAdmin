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
    /// Encapsulates the GetGradebookSummary (Docs/Command/GetGradebookSummary) command
    /// </summary>
    public class GetGradebookSummary : DlapCommand
    {
        public GetGradebookSummary()
        {
            Calculated = false;
            Verbose = true;
            EnrollmentGrades = new Dictionary<string, Grade>();
        }

        #region Data Members

        /// <summary>
        /// The Entity for which to find grades.  At least one of
        /// EnrollmentId, EntityId, or UserId must be set to perform a search.
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// A list of ItemIds for which to return grades (within the enrollment,
        /// entity, or user specified by other parameters).  If this is null
        /// then all items will be returned.  If it is an empty list then no item
        /// data will be returned.
        /// </summary>
        public IEnumerable<string> ItemIds { get; set; }

        /// <summary>
        /// Optional, when true, returns more detail about each grade book entry similar to GetSectionGradebook. T
        /// he default is TRUE.
        /// </summary>
        bool Verbose { get; set; }

        /// <summary>
        /// Optional, when true, returns calculated score values as they appear in the end-user's gradebook as opposed to the raw score values. 
        /// The default is FALSE
        /// </summary>
        bool Calculated { get; set; }

        /// <summary>
        /// Optional, when true, returns the detailed data about the student’s response represented as name-value pairs defined by SCORM. 
        /// The default is false.
        /// </summary>
        bool Scorm { get; set; }

        public Dictionary<string, Grade> EnrollmentGrades { get; set; }

        #endregion

        /// <summary>
        /// Builds request required by the http://dev.dlap.bfwpub.com/Docs/Command/GetGradebookSummary command.
        /// </summary>
        /// <returns>
        /// Request conforming to http://dev.dlap.bfwpub.com/Docs/Command/GetGradebookSummary
        /// </returns>
        public override DlapRequest ToRequest()
        {
            var request = new DlapRequest()
            {
                Type = DlapRequestType.Get,
                Parameters = new Dictionary<string, object>()
            };
            
            if (!String.IsNullOrEmpty(EntityId) )
            {
                request.Parameters["cmd"] = "GetGradebookSummary";
                request.Parameters["entityid"] = EntityId;
            }
            else
            {
                throw new ArgumentException(@"GetGradebookSummary requires an entity id.");
            }

            // If any Item IDs were provided in the search parameters, then format and use them
            if (ItemIds != null && ItemIds.Any())
            {
                request.Parameters["itemid"] = String.Join("|", ItemIds.ToArray());
            }

            if (Verbose)
            {
                request.Parameters["verbose"] = Verbose.ToString().ToLowerInvariant();
            }

            if (Calculated)
            {
                request.Parameters["calculated"] = Calculated.ToString().ToLowerInvariant();
            }

            if (Scorm)
            {
                request.Parameters["scorm"] = Scorm.ToString().ToLowerInvariant();
            }

            return request;
        }

        /// <summary>
        /// Parses the response of the http://dev.dlap.bfwpub.com/Docs/Command/GetGradebookSummary command.
        /// Sample response with verbose = true, calculated = false, scorm = false: (note: other parameters have different results, currently not supported)
        /// <items>
        ///     <item itemid="6135df4f44a9482db44f96c5bdf94917">
        //          <enrollment enrollmentid="216455" firstname="sahba" lastname="abolfazli" status="261" submittedversion="1" submitteddate="2013-09-03T19:27:12.233Z" scoredversion="1" scoreddate="2013-09-03T19:27:12.327Z" pointsachieved="5" pointspossible="6" score="0.83333333333333337" attempts="1" seconds="576" />
        //          <enrollment enrollmentid="219849" firstname="Brooke" lastname="Adams" status="261" submittedversion="1" submitteddate="2013-09-06T03:59:33.127Z" scoredversion="1" scoreddate="2013-09-06T03:59:33.207Z" pointsachieved="5" pointspossible="6" score="0.83333333333333337" attempts="1" seconds="523" />
        /// </summary>
        /// <param name="response"><see cref="DlapResponse"/> to parse</param>
        public override void ParseResponse(DlapResponse response)
        {
            if (null != response.ResponseXml)
            {
                var itemsElement = response.ResponseXml.Element("items");
                foreach (var item in itemsElement.Elements("item"))
                {
                    var itemId = item.Attribute("itemid").Value;
                    foreach (var enrollmentElement in item.Elements("enrollment"))
                    {
                        var enrollmentId = enrollmentElement.Attribute("enrollmentid").Value;
                        Grade g = new Grade();
                        g.ParseEntity(enrollmentElement);
                        g.Item.Id = itemId;
                        EnrollmentGrades.Add(enrollmentId, g);      
                    }
                 
                }
            }

        }
    }
}
