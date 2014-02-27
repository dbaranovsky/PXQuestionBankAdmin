using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// 
    /// </summary>
    public class StudentProfile
    {
        /// <summary>
        /// EnrollmentId of the student
        /// </summary>
        [DataMember]
        public string EnrollmentId { get; set; }

        /// <summary>
        /// UserId of the student
        /// </summary>
        [DataMember]
        public string  UserId { get; set; }

        /// <summary>
        /// First name of the student
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the student
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Email address of the student
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Formatted Name = LastName, FirstName
        /// </summary>
        [DataMember]
        public string FormattedName { get; set; }

        /// <summary>
        /// Total of possible points for all the assignments
        /// </summary>
        [DataMember]
        public double TotalPossiblePoints { get; set; }

        /// <summary>
        /// Total of assigned points for all the assignments
        /// </summary>
        [DataMember]
        public double TotalAssignedPoints { get; set; }

        /// <summary>
        /// grade average of all the assignments
        /// </summary>
        [DataMember]
        public double GradeAverage { get; set; }

        /// <summary>
        /// Last login of the student
        /// </summary>
        [DataMember]
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public StudentProfile()
        {

        }
    }
}
