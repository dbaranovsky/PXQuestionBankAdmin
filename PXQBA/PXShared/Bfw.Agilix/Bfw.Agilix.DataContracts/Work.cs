using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents the work submitted by student.
    /// </summary>
    [DataContract]
    public class Work : IDlapEntityParser
    {
        //Enrollment Id for student who submitted something, which generated thsi work item
        [DataMember]
        public string EnrollmentId { get; set; }

        //Item ID for which the student submitted something.
        [DataMember]
        public string ItemId { get; set; }

        //The ID of this work entry. 
        //Multiple work items for an enrollmentid-itemid pair can exist if the student has resubmitted for the item, 
        //and workid distinguishes multiple entries. It is unique only within enrollmentid-item pair.
        [DataMember]
        public string WorkId { get; set; }

        //The version of the student submission that this work item corresponds to.
        [DataMember]
        public string SubmittedVersion { get; set; }

        //The date that the student submitted the work item.
        [DataMember]
        public string SubmittedDate { get; set; }

        //Version of the last scored submission.
        [DataMember]
        public string ScoredVersion { get; set; }

        //The date the item was scored.
        [DataMember]
        public string ScoredDate { get; set; }

        //The assigned score for this item.
        [DataMember]
        public string Score { get; set; }

        //The number of points possible for this item.
        [DataMember]
        public string PointsPossible { get; set; }

        //The number of points achieved for this item.
        [DataMember]
        public string PointsAchieved { get; set; }

        //The letter grade, if any, for this item.
        [DataMember]
        public string Grade { get; set; }

        //Parent title
        [DataMember]
        public string Title { get; set; }


        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            // Set the Enrollment items
            var id = element.Attribute("enrollmentid") ?? element.Attribute("id");
            var itemid = element.Attribute("itemid");
            var workid = element.Attribute("workid");
            var submittedversion = element.Attribute("submittedversion");
            var submitteddate = element.Attribute("submitteddate");
            var scoredversion = element.Attribute("scoredversion");
            var scoreddate = element.Attribute("scoreddate");
            var score = element.Attribute("score");
            var pointspossible = element.Attribute("pointspossible");
            var pointsachieved = element.Attribute("pointsachieved");
            var grade = element.Attribute("grade");
            var title = element.Attribute("itemtitle") ?? element.Attribute("itemtitle");

            EnrollmentId =id!=null?id.Value:"";
            ItemId = itemid != null ? itemid.Value : "";
            WorkId = workid != null ? workid.Value : "";
            SubmittedVersion = submittedversion != null ? submittedversion.Value : "";
            SubmittedDate = submitteddate != null ? submitteddate.Value : "";
            ScoredVersion = scoredversion != null ? scoredversion.Value : "";
            ScoredDate = scoreddate != null ? scoreddate.Value : "";
            Score = score != null ? score.Value : "";
            PointsPossible = pointspossible != null ? pointspossible.Value : "";
            PointsAchieved = pointsachieved != null ? pointsachieved.Value : "";
            Grade = grade != null ? grade.Value : "";
            Title = title != null ? title.Value : "";
        }
    }
}
