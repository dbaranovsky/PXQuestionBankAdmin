using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Bfw.Agilix.Dlap.Session;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents a Grade business object (see http://dev.dlap.bfwpub.com/Docs/Schema/Grades)
    /// </summary>
    public class Grade : IDlapEntityParser
    {
        #region Properties

        /// <summary>
        /// The item (e.g. an assignment) to which this grade applies.
        /// </summary>
        public Item Item { get; set; }

        /// <summary>
        /// score the user received, with curving applied.
        /// </summary>
        [DataMember]
        public double Achieved { get; protected set; }

        /// <summary>
        /// Maximum possible score, with curing applied.
        /// </summary>
        [DataMember]
        public double Possible { get; protected set; }

        /// <summary>
        /// The score the user achieved in letter form.
        /// </summary>
        [DataMember]
        public string Letter { get; protected set; }

        /// <summary>
        /// Score the user achieved, WITHOUT curving applied.
        /// </summary>
        [DataMember]
        public double RawAchieved { get; protected set; }

        /// <summary>
        /// Maximum score possible, WITHOUT curving applied.
        /// </summary>
        [DataMember]
        public double RawPossible {get; protected set; }

        /// <summary>
        /// Number of times the user tried to complete the assignment.
        /// </summary>
        [DataMember]
        public int Attempts { get; protected set; }

        /// <summary>
        /// Date the assignment was scored.
        /// </summary>
        [DataMember]
        public DateTime ScoredDate { get; set; }

        /// <summary>
        /// Version of the submission that was graded.
        /// </summary>
        [DataMember]
        public int ScoredVersion { get; protected set; }

        /// <summary>
        /// Version of the submission.
        /// </summary>
        [DataMember]
        public int SubmittedVersion { get; protected set; }

        /// <summary>
        /// Date the grade was submitted.
        /// </summary>
        [DataMember]
        public DateTime SubmittedDate { get; set; }

        /// <summary>
        /// The category the grade belongs to.
        /// </summary>
        [DataMember]
        public string CategoryId { get; set; }
        
        /// <summary>
        /// Status of the grade
        /// </summary>
        [DataMember]
        public GradeStatus Status { get; set; }

        /// <summary>
        /// Seconds
        /// </summary>
        [DataMember]
        public int Seconds { get; set; }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            if (null != element)
            {
                // Build the Item object
                Item = new Item();
                Item.Data = new XElement(ElStrings.Data);

                var itemId = element.Attribute(ElStrings.ItemId);
                var itemTitle = element.Attribute(ElStrings.title);

                if (null != itemId)
                {
                    Item.Id = itemId.Value;
                }
                if (null != itemTitle)
                {
                    Item.Title = itemTitle.Value;
                }
              
                // Populate the rest of the Grade object
                var achieved = element.Attribute(ElStrings.Achieved);
                var possible = element.Attribute(ElStrings.possible);
                var letter = element.Attribute(ElStrings.Letter);
                var rawAchieved = element.Attribute(ElStrings.RawAchieved);
                var rawPossible = element.Attribute(ElStrings.RawPossible);
                var attempts = element.Attribute(ElStrings.Attempts);
                var scoreddate = element.Attribute(ElStrings.ScoredDate);
                var scoredversion = element.Attribute(ElStrings.ScoredVersion);
                var submittedversion = element.Attribute(ElStrings.SubmittedVersion);
                var duedate = element.Attribute(ElStrings.duedate);
                var categoryId = element.Attribute(ElStrings.CategoryId);
                var status = element.Attribute(ElStrings.Status);
                var seconds = element.Attribute(ElStrings.Seconds);
                var submitteddate = element.Attribute(ElStrings.SubmittedDate);

                //populate Grade object coming from GradebookSumamry
                if (achieved == null)
                {
                    achieved = element.Attribute(ElStrings.PointsAchieved);
                }
                if (possible == null)
                {
                    possible = element.Attribute(ElStrings.PointsPossible);
                }
                //END GradebookSummary
                
                
                var gradable = new XElement(ElStrings.gradable, true);
                Item.IsGradable = true;
                Item.Data.Add(gradable);

                if (null != possible)
                {
                    Item.Data.Add(new XElement(ElStrings.weight, possible.Value));
                }

                if (null != duedate)
                {
                    Item.Data.Add(new XElement(ElStrings.duedate, duedate.Value));
                }

                if (null != achieved)
                {
                    double d;
                    if (Double.TryParse(achieved.Value, out d))
                    {
                        Achieved = d;
                    }
                }

                if (null != categoryId)
                {
                    Item.Data.Add(new XElement(ElStrings.category, categoryId.Value));
                    CategoryId = categoryId.Value;
                }

                if (null != possible)
                {
                    double d;
                    if (Double.TryParse(possible.Value, out d))
                    {
                        Possible = d;
                    }
                }

                if (null != letter)
                {
                    Letter = letter.Value;
                }

                if (null != rawAchieved)
                {
                    double d;
                    if (Double.TryParse(rawAchieved.Value, out d))
                    {
                        RawAchieved = d;
                    }
                }

                if (null != rawPossible)
                {
                    double d;
                    if (Double.TryParse(rawPossible.Value, out d))
                    {
                        RawPossible = d;
                    }
                }

                if (null != attempts)
                {
                    int i;
                    if (Int32.TryParse(attempts.Value, out i))
                    {
                        Attempts = i;
                    }
                }

                if (null != scoreddate)
                {
                    DateTime dt;
                    if (DateTime.TryParse(scoreddate.Value, out dt))
                    {
                        ScoredDate = dt;
                    }
                }

                if (null != scoredversion)
                {
                    int i;
                    if (Int32.TryParse(scoredversion.Value, out i))
                    {
                        ScoredVersion = i;
                    }
                }

                if (null != submittedversion)
                {
                    int i;
                    if (Int32.TryParse(submittedversion.Value, out i))
                    {
                        SubmittedVersion = i;
                    }
                }

                if (null != submitteddate)
                {
                    DateTime dt;
                    if (DateTime.TryParse(submitteddate.Value, out dt))
                    {
                        SubmittedDate = dt;
                    }
                }
                if (null != status)
                {
                    int st = 0;
                    Int32.TryParse(status.Value, out st);
                    Status = (GradeStatus)st;
                }
                if (null != seconds)
                {
                    int i;
                    if (Int32.TryParse(seconds.Value, out i))
                    {
                        Seconds = i;
                    }
                }
            }
        }

        #endregion
    }
}
