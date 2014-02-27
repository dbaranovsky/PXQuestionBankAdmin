using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bfw.Agilix.DataContracts;

namespace Bfw.PX.PXPub.Models
{
    /// <summary>
    /// Defines the course academic term
    /// </summary>
    public class EcomerceJoinCourse
    {
        /// <summary>
        /// Academic Term for that course
        /// </summary>        
        public string AcademicTerm { get; set; }

        /// <summary>
        /// university , course domain detail
        /// </summary>
        public string University { get; set; }
        
        /// <summary>
        /// getting context current user
        /// </summary>
        public string CurrentUser { get; set; }
        
        /// <summary>
        /// source Course from which to Switch the course
        /// </summary>
        public string SwitchEnrollFromCourse { get; set; }
        
        /// <summary>
        /// course instructor
        /// </summary>
        public string Instructor { get; set; }
        
        /// <summary>
        /// user enrollment in that course
        /// </summary>
        public string EnrollmentID { get; set; }

        /// <summary>
        /// status of the enrollment if any
        /// </summary>
        public EnrollmentStatus EnrollmentStatus { get; set; }

        /// <summary>
        /// domain id
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// the course details 
        /// </summary>
        public Course course { get; set; }

        /// <summary>
        /// Flag to show or hide the course number on the join course form
        /// </summary>
        public bool ShowCourseNumber { get; set; }

        /// <summary>
        ///  Flag to show or hide the course section on the join course form
        /// </summary>
        public bool ShowCourseSection { get; set; }

        /// <summary>
        /// instance constructor
        /// </summary>
        public EcomerceJoinCourse()
        {
            //CourseNumber and CourseSection should show up by default on join course form
            ShowCourseNumber = true;
            ShowCourseSection = true;
        }
    }

}