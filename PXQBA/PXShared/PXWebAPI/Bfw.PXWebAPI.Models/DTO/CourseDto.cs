using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace Bfw.PXWebAPI.Models.DTO
{
    /// <summary>
    /// Represents a course item in the system. This includes settings and metadata.
    /// </summary>
    public class CourseDto
    {

        /// <summary>
        /// The ID of the course (entity ID).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// mishka
        /// parent course id
        /// </summary>
        public string ProductCourseId { get; set; }
        /// <summary>
        /// The course's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The course's domain.
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Parent Id
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// Title of the product course, which is appended to the Title property.
        /// </summary>
        public string CourseProductName
        {
            get
            {
                return Properties.GetPropertyValue<string>("CourseProductName", "");
            }
            set
            {
                Properties.SetPropertyValue("CourseProductName", PropertyType.String, value);
            }
        }

        /// <summary>
        /// TimeZone of the product course
        /// </summary>
        public string CourseTimeZone
        {
            get
            {
                return Properties.GetPropertyValue<string>("CourseTimeZone", TimeZoneInfo.Local.DisplayName);
            }
            set
            {
                Properties.SetPropertyValue("CourseTimeZone", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the instructor that created the course.
        /// </summary>
        /// <value>
        /// The name of the instructor.
        /// </value>
        public string InstructorName
        {
            get
            {
                return Properties.GetPropertyValue<string>("CourseUserName", "");
            }
            set
            {
                Properties.SetPropertyValue("CourseUserName", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Collection of instructors for given course
        /// </summary>
        public IEnumerable<Instructor> Instructors { get; set; }

        /// <summary>
        /// Gets or sets the coursenumber the instructor set  on the course
        /// </summary>
        public string CourseNumber
        {
            get
            {
                return Properties.GetPropertyValue<string>("CourseNumber", "");
            }
            set
            {
                Properties.SetPropertyValue("CourseNumber", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Gets or sets the section number the instructor set on the course
        /// </summary>
        public string SectionNumber
        {
            get
            {
                return Properties.GetPropertyValue<string>("SectionNumber", "");
            }
            set
            {
                Properties.SetPropertyValue("SectionNumber", PropertyType.String, value);
            }
        }

        public string Isbn13
        {
            get
            {
                return Properties.GetPropertyValue<string>("meta-book-isbn", "");
            }
            set
            {
                Properties.SetPropertyValue("meta-book-isbn", PropertyType.String, value);
            }
        }
        
        /// <summary>
        /// Date the course was activated.
        /// </summary>
        public string ActivatedDate { get; set; }

        /// <summary>
        /// The type of this course
        /// </summary>
        public string CourseType
        {
            get
            {
                string retval = Properties.GetPropertyValue<string>("bfw_course_type", "LMS");
                if (string.IsNullOrEmpty(retval))
                    return "LMS";
                return retval;
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_type", PropertyType.String, value);
                Properties.SetPropertyValue("meta-bfw_course_type", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Academic term in which the course is present
        /// </summary>
        public string AcademicTerm
        {
            get
            {
                string retval = Properties.GetPropertyValue<string>("meta-bfw_academic_term", "");
                if (string.IsNullOrEmpty(retval))
                    return "";
                return retval;
            }
            set
            {
                Properties.SetPropertyValue("meta-bfw_academic_term", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores the user-id of the instructor that created the course.
        /// </summary>
        public string CourseOwner
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_owner", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_owner", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores the user-id of the instructor that created the course.
        /// </summary>
        public string CourseDescription
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_description", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_description", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Property that stores the user-id of the instructor that created the course.
        /// </summary>
        public string CourseAuthor
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_course_author", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_course_author", PropertyType.String, value);
            }
        }


        /// <summary>
        /// Property that stores a dashboard course-id.
        /// </summary>
        public string DashboardCourseId
        {
            get
            {
                return Properties.GetPropertyValue<string>("bfw_dashboard_course", "");
            }
            set
            {
                Properties.SetPropertyValue("bfw_dashboard_course", PropertyType.String, value);
            }
        }

        /// <summary>
        /// Set of properties stored in the item
        /// </summary>
        public IDictionary<string, PropertyValue> Properties { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseDto"/> class.
        /// </summary>
        public CourseDto()
        {
            Properties = new Dictionary<string, PropertyValue>();
        }

        /// <summary>
        /// Collection of contact details tied to the course.
        /// </summary>
        public List<ContactInfo> ContactInformation { get; set; }

        /// <summary>
        /// Syllabus Name
        /// </summary>
        public string Syllabus { get; set; }

        /// <summary>
        /// Syllabus Type: Url or File
        /// </summary>
        public string SyllabusType { get; set; }

        /// <summary>
        /// Syllabus Url
        /// </summary>
        public string SyllabusUrl
        {
            get
            {
                var result = string.Empty;

                if (SyllabusType == "Url")
                {
                    result = Syllabus;
                }

                return result;
            }
        }

        /// <summary>
        /// Syllabus File Name
        /// </summary>
        public string SyllabusFileName
        {
            get
            {
                var result = string.Empty;

                if (SyllabusType == "File")
                {
                    result = Syllabus;
                    var index = Syllabus.IndexOf("/");

                    if (index > 0)
                    {
                        result = Syllabus.Substring(Syllabus.IndexOf("/") + 1);
                    }
                }

                return result;
            }
        }
     }

    /// <summary>
    /// 
    /// </summary>
    public class Instructor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
