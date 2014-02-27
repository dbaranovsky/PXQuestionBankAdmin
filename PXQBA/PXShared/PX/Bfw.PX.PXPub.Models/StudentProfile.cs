using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class StudentProfile
    {
        /// <summary>
        /// EnrollmentId of the student
        /// </summary>
        public string EnrollmentId { get; set; }

        /// <summary>
        /// UserId of the student
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Last login of the student
        /// </summary>
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// First name of the student
        /// </summary>
        public string  FirstName { get; set; }

        /// <summary>
        /// Last name of the student
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email address of the student
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Formatted Name = LastName, FirstName
        /// </summary>
        public string FormattedName { get; set; }

        /// <summary>
        /// Total of possible points for all the assignments
        /// </summary>
        public double TotalPossiblePoints { get; set; }

        /// <summary>
        /// Total of assigned points for all the assignments
        /// </summary>
        public double TotalAssignedPoints { get; set; }

        /// <summary>
        /// grade average of all the assignments
        /// </summary>
        public double GradeAverage { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public StudentProfile()
        {

        }


    }
}
