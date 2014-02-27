using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using BizDC = Bfw.PX.Biz.DataContracts;
using System.Globalization;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Model to build the Gradebook for a course
    /// </summary>
    public class GradeBook
    {
        /// <summary>
        /// List of students enrolled for the course
        /// </summary>
        public IList<StudentProfile> Students { get; set; }

        /// <summary>
        /// List of all the assignment items in the course
        /// </summary>
        public IDictionary<BizDC.ContentItem, IList<BizDC.ContentItem>> Assignments { get; set; }

        /// <summary>
        /// Average of all the assignments of all the students in a class
        /// </summary>
        public double ClassAverage { get; set; }

        /// <summary>
        /// List containing the grades of all the assignments of all the students
        /// </summary>
        public IList<Grade> Grades { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public GradeBook()
        {
            Students = new List<StudentProfile>();
            Assignments = new Dictionary<BizDC.ContentItem, IList<BizDC.ContentItem>>();
            Grades = new List<Grade>();
        }

        /// <summary>
        /// Gets the grade for a given assignmentid
        /// </summary>
        /// <param name="enrollmentId"></param>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        public Grade GetAssignment(string enrollmentId, string assignmentId)
        {
            return Grades.Where(g => (g.EnrollmentId == enrollmentId && g.ItemId == assignmentId)).FirstOrDefault();
        }

        /// <summary>
        /// Returns the count of assignment submissions ready for grading
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <returns></returns>
        public int UpForGrading(string assignmentId) {
            return Grades.Where(g => g.ItemId == assignmentId && g.IsSubmitted && !g.IsGraded).Count();
        }

        /// <summary>
        /// Get the grade rule text 
        /// </summary>
        /// <returns></returns>
        public string GetGradeRule(GradeRule gradeRule, bool isTitleCase = false)
        {
            var gradeRuleTxt = string.Empty;

            switch (gradeRule)
            {
                case GradeRule.Last:
                    gradeRuleTxt = "most recent";
                    break;
                case GradeRule.First:
                    gradeRuleTxt = "first";
                    break;
                case GradeRule.Highest:
                    gradeRuleTxt = "highest";
                    break;
                case GradeRule.Lowest:
                    gradeRuleTxt = "lowest";
                    break;
                case GradeRule.Average:
                    gradeRuleTxt = "average";
                    break;
                case GradeRule.Total:
                    gradeRuleTxt = "last";
                    break;
                default:
                    break;
            }

            gradeRuleTxt = isTitleCase ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(gradeRuleTxt.ToLowerInvariant()) : gradeRuleTxt;

            return gradeRuleTxt;

        }
    }
}
