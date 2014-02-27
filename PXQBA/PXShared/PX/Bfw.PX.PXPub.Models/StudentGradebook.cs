using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Model to build the student gradebook
    /// </summary>
    public class StudentGradebook
    {
        /// <summary>
        /// student details
        /// </summary>
        public StudentProfile Student { get; set; }

        /// <summary>
        /// List of all the assignment items in the course
        /// </summary>
        public IDictionary<BizDC.ContentItem, IList<BizDC.ContentItem>> Assignments { get; set; }

        /// <summary>
        /// Assigned Item grades
        /// </summary>
        public IEnumerable<Grade> Grades { get; set; }

        /// <summary>
        /// Average of all the assignments of the student
        /// </summary>
        public double AssignedAverage { get; set; }

        /// <summary>
        /// Average of all the assignments of the student
        /// </summary>
        public double UnAssignedAverage { get; set; }

        /// <summary>
        /// Unassigned Item grades
        /// </summary>
        public IEnumerable<Grade> UnAssingedGrades { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public StudentGradebook()
        {
        }

    }
}
