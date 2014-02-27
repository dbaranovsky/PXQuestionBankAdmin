using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    public class AboutCourse
    {
        public AboutCourse()
        {
            InstructorName = string.Empty;
            OfficeHours = string.Empty;
            ContactInfo = new List<AboutCourseContactInfo>();
            SyllabusUrl = string.Empty;
            SyllabusFile = null;
            SyllabusFileName = string.Empty;
        }

        [Required(ErrorMessage = "You must specify a name")]
        public string InstructorName { get; set; }

        public string OfficeHours { get; set; }

        public List<AboutCourseContactInfo> ContactInfo { get; set; }

        public string SyllabusUrl { get; set; }

        public string SyllabusFileName { get; set; }

        public string SyllabusFile { get; set; }

        public string CourseTitle { get; set; }

        public string CourseType { get; set; }

        public string SyllabusDisplayText { get; set; }

        public bool ShowCourseTitle { get; set; }

        public bool ShowInstructorName { get; set; }

        public bool ShowOfficeHours { get; set; }

        public bool ShowEmail { get; set; }

        public bool ShowPhoneNumber { get; set; }

        public bool ShowSyllabusUrl { get; set; }

        public bool ShowEditLink { get; set; }

        public string CourseNumber { get; set; }

        public string SectionNumber { get; set; }

        public string CampusLmsId { get; set; }
        
        public bool LmsIdEnabled { get; set; }

        /// <summary>
        /// Link type tells whether the link is a Url or path to file stored on DLAP
        /// </summary>
        public AboutCourseLinkType SyllabusLinkType { get; set; }

        /// <summary>
        /// Gets or sets the access level.
        /// </summary>
        /// <value>
        /// The access level.
        /// </value>
        public Bfw.PX.Biz.ServiceContracts.AccessLevel AccessLevel { get; set; }
    }


    public class AboutCourseContactInfo
    {
        public AboutCourseContactInfo()
        {
            Type = string.Empty;
            Info = string.Empty;
        }

        public string Type { get; set; }

        public string Info { get; set; }
    }

    public class FileUploadResults
    {
        public string UploadPath { get; set; }
        public string UploadFileName { get; set; }
        public string UploadMessage { get; set; }        
    }

    public enum AboutCourseLinkType
    {
        None,
        Url,
        File
    }
}