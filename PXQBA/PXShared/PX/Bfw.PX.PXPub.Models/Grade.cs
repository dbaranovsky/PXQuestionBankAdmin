using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Bfw.PX.PXPub.Models
{
    public class Grade
    {
        /// <summary>
        /// Gets or sets the graded item.
        /// </summary>
        public string ItemId { get; set; }

        /// <summary>
        /// Title of the item to which the grade belongs
        /// </summary>
        public string ItemTitle { get; set; }

        /// <summary>
        /// Assignment Folder Id for the current grade's assignment
        /// </summary>
        public string ParentFolderId { get; set; }

        /// <summary>
        /// EnrollmentId of the student
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// Name of the student
        /// </summary>
        public string EnrollmentName { get; set; }

        /// <summary>
        /// UserId of the student
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The letter grade achieved for the course according to the grade calculator.
        /// </summary>
        public string Letter { get; set; }

        /// <summary>
        /// The number of points achieved for the course according to the grade calculator.
        /// </summary>
        public double Achieved { get; set; }

        /// <summary>
        /// Maximum score possible, WITHOUT curving applied.
        /// </summary>
        public double RawAchieved { get; set; }

        /// <summary>
        /// Maximum score possible, WITHOUT curving applied.
        /// </summary>
        public double RawPossible { get; set; }

        /// <summary>
        /// The number of points possible for the course according to the grade calculator.
        /// </summary>
        public double Possible { get; set; }

        /// <summary>
        /// The date of the last submission.
        /// </summary>
        public DateTime? SubmittedDate { get; set; }

        /// <summary>
        /// The date the item was last scored.
        /// </summary>
        public DateTime? ScoredDate { get; set; }

        /// <summary>
        /// Version of the Submission that this response applies to.
        /// </summary>
        public int ScoredVersion { get; set; }

        /// <summary>
        /// The number of attempts made on this item.
        /// </summary>
        public int Attempts { 
            get {
                return AttemptList.Count();
                } 
        }

        /// <summary>
        /// The number of attempts the student may submit. The value 0 means that the user may use an unlimited amount of attempts.
        /// </summary>
        public int AttemptLimit { get; set; }

        /// <summary>
        /// Whether unlimited attempts are possible on the Assignment
        /// </summary>
        public bool IsUnlimitedAttempts { 
            get {
                return (AttemptLimit == 0);
            } 
        }

        /// <summary>
        /// Whether it is an assignment folder grade
        /// </summary>
        public Boolean IsAssignmentFolder { get; set; }

        /// <summary>
        /// Whether the student has submitted the assignment
        /// </summary>
        public Boolean IsSubmitted {
            get
            {
                return SubmittedDate.HasValue && SubmittedDate.Value.Year > DateTime.MinValue.Year;
            }
        }

        /// <summary>
        /// Whether the assignment has been graded
        /// </summary>
        public Boolean IsGraded { 
            get 
            {
                return ScoredDate.HasValue && ScoredDate.Value.Year > DateTime.MinValue.Year;
            }
        }

        /// <summary>
        /// Grade text to be displayed in the Gradebook grid
        /// </summary>
        public string GradeDisplay { 
            get {
                var gradeDisplay = string.Empty;
                if (Possible > 0)
                {
                    if (GradeRule == GradeRule.First || GradeRule == GradeRule.Last || GradeRule == GradeRule.Highest || GradeRule == GradeRule.Lowest)
                        gradeDisplay = string.Format("{0}% ({1}/{2})",  Math.Round(Achieved / Possible, 3)*100, Math.Round(Achieved, 2), Possible);
                    else
                        gradeDisplay = string.Format("{0}%", Math.Round(Achieved / Possible, 3) * 100);
                }
                else
                {
                    gradeDisplay = "0%";
                }

                return gradeDisplay;
            }
        
        }

        /// <summary>
        /// Attempts text to be displayed in the student gradebook view
        /// </summary>
        public string AttemptDisplay { 
            get {

                var attemptDisplay = string.Empty;

                if (!IsSubmitted)
                {
                    attemptDisplay = "no attempt";
                }
                else if (AttemptLimit > 0)
                {
                    attemptDisplay = string.Format("{0}/{1}", AttemptList.Count(), AttemptLimit);
                }
                else
                {
                    attemptDisplay = string.Format("{0}", AttemptList.Count());
                }
                return attemptDisplay;
            }
        }
        public string GradeScore {
            get
            {
                return string.Format("{0:0.0%} ", GradeScoreNumeric);
            }
        }

        public double GradeScoreNumeric
        {
            get
            {
                if (Possible > 0)
                {
                    return Math.Round(Achieved / Possible, 3);
                }
                return 0;
            }
        }

        /// <summary>
        /// Grade Rule
        /// </summary>
        public GradeRule GradeRule { get; set; }

        /// <summary>
        /// gets the student attempts
        /// </summary>
        public IList<Attempt> AttemptList { get; set; }

        /// <summary>
        /// Get the grade rule text 
        /// </summary>
        /// <returns></returns>
        public string GetGradeRule(bool isTitleCase = false)
        {
            var gradeRule = string.Empty;

            switch (GradeRule)
            {
                case GradeRule.Last:
                    gradeRule = "most recent";
                    break;
                case GradeRule.First:
                    gradeRule = "first";
                    break;
                case GradeRule.Highest:
                    gradeRule = "highest";
                    break;
                case GradeRule.Lowest:
                    gradeRule = "lowest";
                    break;
                case GradeRule.Average:
                    gradeRule = "average";
                    break;
                case GradeRule.Total:
                    gradeRule = "last";
                    break;
                default:
                    break;
            }

            gradeRule = isTitleCase ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gradeRule.ToLowerInvariant()) : gradeRule;

            return gradeRule;

        }

        /// <summary>
        /// instance constructor
        /// </summary>
        public Grade()
        {
        }

    }
    public class Attempt
    {
        public int Count { get; set; }

        public DateTime Submitted { get; set; }

        public string Score
        {
            get
            {
                return RawPossible > 0 ? string.Format("{0}%", Math.Round((RawAchieved / RawPossible), 3) * 100D) : "0%";
            }
        }

        /// <summary>
        /// score % calculate based on the achieved and possible score
        /// </summary>
        public string ActualScore
        {
            get
            {
                return Possible > 0 ? string.Format("{0}%", Math.Round((Achieved / Possible), 3) * 100D) : "0%";
            }
        }
        public double RawAchieved { get; set; }

        public double RawPossible { get; set; }

        public double Achieved { get; set; }

        public double Possible { get; set; }

        /// <summary>
        /// Grade text to be displayed in the Gradebook grid
        /// </summary>
        public string AttemptDisplay
        {
            get
            {
                var attemptDisplay = string.Empty;

                if (RawPossible > 0 && RawAchieved > 0)
                {
                    attemptDisplay = string.Format("{0}({1}/{2})", string.Format("{0:0%} ", RawAchieved / RawPossible), RawAchieved, RawPossible);
                }
                else
                {
                    attemptDisplay = string.Format("{0}/{1}", RawAchieved, RawPossible);
                }

                return attemptDisplay;
            }

        }
    }
}
