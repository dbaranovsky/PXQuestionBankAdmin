using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    public class DashboardData
    {
        public List<DashboardItem> InstructorCourses { get; set; }
        public List<DashboardItem> NonInstructorCourse { get; set; }
        public List<DashboardItem> InstructorCoursesFromProductCourse { get; set; }
        public List<DashboardItem> PublisherTemplates { get; set; }
        public List<DashboardItem> ProgramManagerTemplates { get; set; }
        public DashboardItem Dashboard { get; set; }
   
        private List<Course> _existingDashboards = new List<Course>();

        public DashboardData()
        {
            InstructorCourses = new List<DashboardItem>();
            NonInstructorCourse = new List<DashboardItem>();
            PublisherTemplates = new List<DashboardItem>();
            ProgramManagerTemplates = new List<DashboardItem>();
            InstructorCoursesFromProductCourse = new List<DashboardItem>();
        }

        public List<Course> GetExistingDashboards()
        {
            return _existingDashboards;
        }

        public string GetDashboardId()
        {
            try
            {
                return Dashboard.Course.Id;
            }
            catch
            {
                return string.Empty;
            }            
        }

        public void AddDashboardCourse(Course course)
        {
            if (course.CourseType == CourseType.EportfolioDashboard.ToString() && !_existingDashboards.Exists ( i=>i.Id == course.Id))
            {
                _existingDashboards.Add(course);
            }
        }
    }
}
