using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class CourseHeader
    {
        /// <summary>
        /// storage of the application level js files that should be loaded when the application loads
        /// </summary>
        public string ApplicationJSFile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the product course title.
        /// </summary>
        /// <value>
        /// The product course title.
        /// </value>
        public string ProductCourseTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the course title.
        /// </summary>
        /// <value>
        /// The course title.
        /// </value>
        public string CourseTitle
        {
            get;
            set;
        }

        /// <summary>
        /// Course Discipline Name (ie: Bio, Psych, Econ, etc) used for course title
        /// </summary>
        public string CourseDisciplineAbbreviation { get; set; }

        /// <summary>
        /// Gets or sets the name of the instructor.
        /// </summary>
        /// <value>
        /// The name of the instructor.
        /// </value>
        public string InstructorName
        {
            get;
            set;
        }

        /// <summary>
        /// Instructor Name to be displayed on the courseheader.
        /// </summary>
        public string DisplayedInstructorName
        {

            get
            {
                if (CourseSubType.Equals("generic", StringComparison.OrdinalIgnoreCase))
                {
                    return "None";
                }
               
                return InstructorName ?? "";
            }

        }

        public bool IsAllowPageEdit
        {
            get;
            set;
        }

        /// <summary>
        /// Whether current course is product course or not
        /// </summary>
        public bool IsProductCourse
        {
            get;
            set;
        }
        /// <summary>
        /// Instructors Email
        /// </summary>
        public string InstructorEmail
        {
            get;
            set;
        }
        public string CourseType { get; set; }

        /// <summary>
        /// Course Sub type like "generic" / "dashboard" etc.
        /// </summary>
        public string CourseSubType { get; set; }

        /// <summary>
        /// Published Status of the course
        /// </summary>
        public string PublishedStatus { get; set; }
        /// <summary>
        /// CourseId
        /// </summary>
        public string CourseId { get; set; }

        /// <summary>
        /// Returns the Standard TimezoneOffset for the course (outside of Daylight Savings)
        /// </summary>
        public int TimeZoneStandardOffset { get; set; }
        /// <summary>
        /// Returns the TimezoneOffset for the course during Daylight Savings
        /// </summary>
        public int TimeZoneDaylightOffset { get; set; }
        /// <summary>
        /// Returns the date when daylight savings ends
        /// </summary>
        public DateTime TimeZoneStandardStartTime { get; set; }
        /// <summary>
        /// Returns the date when daylight savings starts
        /// </summary>
        public DateTime TimeZoneDaylightStartTime { get; set; }
        /// <summary>
        /// Returns the date when daylight savings ends next year
        /// </summary>
        public DateTime TimeZoneStandardStartTimeNextYear { get; set; }
        /// <summary>
        /// Returns the date when daylight savings starts  next year
        /// </summary>
        public DateTime TimeZoneDaylightStartTimeNextYear { get; set; }




        /// <summary>
        /// Returns the Timezone abbreviatiopn for of the course's time zone
        /// </summary>
        public string TimeZoneAbbreviation { get; set; }

        /// <summary>
        /// Returns the Description for the course
        /// </summary>
        public string CourseDescription { get; set; }

        /// <summary>
        /// Returns the Author for the course
        /// </summary>
        public string CourseAuthor { get; set; }

    }
}
