using System;


namespace Bfw.PXWebAPI.Models
{
    public class Enrollment
    {
        /// <summary>
        /// The ID of the Enrollment object.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The user info associated with the enrollment.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// The course item which the user is linked to.
        /// </summary>
        //To Do --Include course when we create the course class
        //public PXdc.Course Course { get; set; }
        public string CourseId { get; set; }

        /// <summary>
        /// String representation of the overall grade for the course to date (could be a percentage, a letter grade, &c.).
        /// </summary>
        public string OverallGrade { get; set; }

        /// <summary>
        /// The percent of gradable items that are graded.
        /// </summary>
        public double PercentGraded { get; set; }

        /// <summary>
        /// The set of grades for the enrollment.
        /// </summary>
        //To Do --Include course when we create the course class
        //public IEnumerable<PXdc.Grade> Grades { get; set; }

        /// <summary>
        /// Information about the domain the enrollment belongs to.
        /// </summary>        
        public string DomainId { get; set; }

        /// <summary>
        /// Information about the domain the enrollment belongs to.
        /// </summary>        
        public string DomainName { get; set; }

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
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The date when enrollment ends.
        /// </summary>       
        public DateTime? EndDate { get; set; }

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
