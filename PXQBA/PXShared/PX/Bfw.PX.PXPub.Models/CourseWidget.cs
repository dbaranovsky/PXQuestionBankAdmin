using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class CourseWidget
    {
        /// <summary>
        /// Gets or sets the courses.
        /// </summary>
        /// <value>
        /// The courses.
        /// </value>
        public List<Course> Courses
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selected course.
        /// </summary>
        /// <value>
        /// The selected course.
        /// </value>
        public Course SelectedCourse
        {
            get;
            set;
        }

    }
}
