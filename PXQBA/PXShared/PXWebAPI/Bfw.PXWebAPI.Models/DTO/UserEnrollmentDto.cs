using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models.DTO
{
    public class UserEnrollmentDto
    {
        public string EnrollmentId { get; set; }
        public BaseUser User { get; set; }
        public string DomainId { get; set; }
        public string DomainName { get; set; }
        public string CourseTitle { get; set; }
        public string CourseId { get; set; }
        public string CourseUrl { get; set; }
        public DateTime? CourseActivationDate { get; set; }
        public string InstructorName { get; set; }
        public DateTime? StartDate { get; set; }
        public string Status { get; set; }
        public string OverallGrade { get; set; }
        public double Score { get; set; }
        public string UserRole { get; set; }
    }
}
