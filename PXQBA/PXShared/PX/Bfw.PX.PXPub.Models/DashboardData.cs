using System.Collections.Generic;

namespace Bfw.PX.PXPub.Models
{
    public class DashboardData
    {
        public List<DashboardItem> InstructorCourses { get; set; }
        public List<DashboardItem> InstructorCoursesFromProductCourse { get; set; }
        public List<DashboardItem> InstructorTemplates { get; set; }
        public List<DashboardItem> PublisherTemplates { get; set; }
        public List<DashboardItem> ProgramManagerTemplates { get; set; }
        public DashboardItem Dashboard { get; set; }
        public bool LaunchPadMode { get; set; }
        public bool isMultipleDomains { get; set; }

        /// <summary>
        /// Gets or set the visibility of Course Title column
        /// </summary>
        public bool AllowCourseTitleColumn { get; set; }

        /// <summary>
        /// Gets or set the visibility of Instructor Name column
        /// </summary>
        /// 
        public bool AllowInstructorNameColumn { get; set; }

        /// <summary>
        /// Gets or set the visibility of Domain Name column
        /// </summary>
        public bool AllowDomainNameColumn { get; set; }


        /// <summary>
        /// Gets or set the visibility of Academic Term column
        /// </summary>
        public bool AllowAcademicTermColumn { get; set; }

        /// <summary>
        /// Gets or set the visibility of Course Id column
        /// </summary>
        /// 
        public bool AllowCourseIdColumn { get; set; }


        /// <summary>
        /// Gets or set the visibility of Anothher Branch column
        /// </summary>
        public bool AllowCreateAnotherBranchColumn { get; set; }


        /// <summary>
        /// Gets or set the visibility of Activate Button column
        /// </summary>
        public bool AllowActivateButtonColumn { get; set; }


        /// <summary>
        /// Gets or set the visibility of Delete Button column
        /// </summary>
        public bool AllowDeleteButtonColumn { get; set; }

        /// <summary>
        /// Gets or set the visibility of Allow Status column
        /// </summary>
        public bool AllowStatusColumn { get; set; }

        /// <summary>
        /// Gets or set the visibility of Allow Status column
        /// </summary>
        public bool AllowCourseOpenInNewWindow { get; set; }
        /// <summary>
        /// Gets or set the visibility of Enrollment Count column
        /// </summary>
        public bool AllowEnrollmentCountColumn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow viewing roster].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow viewing roster]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowViewingRoster { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow editing course information].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow editing course information]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowEditingCourseInformation { get; set; }

        /// <summary>
        /// Gets or set the visibility of Question Admin
        /// </summary>
        public bool QuestionAdminLink { get; set; }

        /// <summary>
        /// Gets or set the visibility of Question Admin
        /// </summary>
        public bool SandBoxLink { get; set; }

        /// <summary>
        /// Sets Branch text if branch has been created or not.
        /// </summary>
        public bool IsBranchCreated { get; set; }

        /// <summary>
        /// Gets or sets JSON list of school to be used in AutoComplete
        /// </summary>
        public string SchoolList { get; set; }

        /// <summary>
        /// Gets non dashboard courses
        /// </summary>
        public bool GetNonDashboardCourses { get; set; }

        /// <summary>
        /// Gets non dashboard courses
        /// </summary>
        public string DashboardId { get; set; }

        private List<Course> _existingDashboards { get; set; }

        public List<CourseAcademicTerm> PossibleAcademicTerms { get; set; }

        public DashboardData()
        {
            InstructorCourses = new List<DashboardItem>();
            InstructorTemplates = new List<DashboardItem>();
            PublisherTemplates = new List<DashboardItem>();
            ProgramManagerTemplates = new List<DashboardItem>();
            _existingDashboards = new List<Course>();
        }

        public List<Course> GetExistingDashboards()
        {
            return _existingDashboards;
        }
    }
}
