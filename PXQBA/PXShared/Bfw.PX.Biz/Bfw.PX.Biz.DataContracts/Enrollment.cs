using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Represents the relationship between a user and an entity (such as a class or section).
    /// </summary>
    public class Enrollment
    {
        /// <summary>
        /// The ID of the Enrollment object.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The user info associated with the enrollment.
        /// </summary>
        public UserInfo User { get; set; }

        /// <summary>
        /// The course item which the user is linked to.
        /// </summary>
        public Course Course { get; set; }

        /// <summary>
        /// Gets or sets course id that is required for passing request to DLAP and getting course id info by parsing
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Representation of the overall achieved for the course to date.
        /// </summary>
        public double OverallAchieved { get; set; }

        /// <summary>
        /// Representation of the overall possible for the course to date.
        /// </summary>
        public double OverallPossible { get; set; }

        /// <summary>
        /// String representation of the overall grade for the course to date (could be a percentage, a letter grade, &c.).
        /// </summary>
        public string OverallGrade { get; set; }

        /// <summary>
        /// The percent of gradable items that are graded.
        /// </summary>
        public double PercentGraded { get; set; }

        private List<Grade> _grades;
        /// <summary>
        /// The set of grades for the enrollment.
        /// </summary>
        public IEnumerable<Grade> ItemGrades
        {
            get
            {
                return _grades;
            }
            set
            {
                if (value != null)
                {
                    _grades = value.ToList();
                }
                else
                {
                    _grades = null;
                }
            }
        }

        private List<CategoryGrade> _categorygrades;
        /// <summary>
        /// The set of grades for the enrollment.
        /// </summary>
        public IEnumerable<CategoryGrade> CategoryGrades
        {
            get
            {
                return _categorygrades;
            }
            set
            {
                if (value != null)
                {
                    _categorygrades = value.ToList();
                }
                else
                {
                    _categorygrades = null;
                }
            }
        }

        /// <summary>
        /// Information about the domain the enrollment belongs to.
        /// </summary>        
        public string DomainId { get; set; }

        /// <summary>
        /// Information about the rights flags.
        /// </summary>    
        public string Flags { get; set; }

        /// <summary>
        /// Status of the enrollment.
        /// </summary>        
        public string Status { get; set; }

        /// <summary>
        /// The date when enrollment begins.
        /// </summary>      
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date when enrollment ends.
        /// </summary>       
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Reference Id mapping from RA authentication.
        /// </summary>        
        public string Reference { get; set; }

        /// <summary>
        /// Optional member to specify how to interpret flags in Agilix. 
        /// If schema is 2 then SubmitFinalGrade privilege is treated as a distinct privilege and 
        /// you must explicitly specify it in flags. If schema is 1, then specifying GradeExam, 
        /// GradeAssignment, or GradeForum for flags automatically include the SubmitFinalGrade right. 
        /// The default schema is 1.
        /// </summary>      
        public string Schema { get; set; }

    }
}