using System;
using System.Collections.Generic;
using System.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Database;
using Bfw.Common.Logging;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Adc = Bfw.Agilix.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Implements IDashboardActions2
    /// </summary>
    public class DashboardActions2 : IDashboardActions2
    {
        #region Properties

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        protected IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// The Courses actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Exposes the related functions for 'Course Sharing' functionality across PX
        /// </summary>
        protected ISharedCourseActions SharedCourseActions { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardActions"/> class.
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>
        public DashboardActions2(IBusinessContext ctx, ISessionManager sessionManager, ICourseActions courseActions, IEnrollmentActions enrollmentActions, ISharedCourseActions sharedCourseActions)
        {
            Context = ctx;
            SessionManager = sessionManager;
            CourseActions = courseActions;
            EnrollmentActions = enrollmentActions;
            SharedCourseActions = sharedCourseActions;
        }

        #endregion

        #region IDashboardActions Members

        /// <summary>
        /// Gets list of courses for instructor dashboard
        /// </summary>
        /// <param name="getChildrenCourses">Include children or not</param>
        /// <returns>Bdc.DashboardData</returns>
        public Bdc.DashboardData GetDashboardData(bool getChildrenCourses)
        {
            PopulateUser();

            using (Context.Tracer.StartTrace("DashboardActions.GetDashboardData2()"))
            {
                Bdc.DashboardData result = new Bdc.DashboardData();

                using (Context.Tracer.StartTrace("Search For and load Product Template Courses"))
                {
                    using (Context.Tracer.StartTrace("Search For Product Template Courses"))
                    {
                        List<Enrollment> enrollments = EnrollmentActions.ListEnrollments(Context.CurrentUser.Username, Context.ProductCourseId, loadCourses: true, getEnrollmentCount: true).ToList();

                        if (enrollments != null)
                        {
                            List<Bdc.Course> courses = FilterEnrollments(enrollments).OrderByDescending(course => course.CreatedDate).ToList();

                            if (getChildrenCourses == true && courses != null)
                            {
                                courses = SortCourseChildren(courses);
                            }

                            result.InstructorCourses = ToDashboardItem(courses);
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Deletes Course specified by ID
        /// </summary>
        /// <param name="courseId">string</param>
        /// <returns></returns>
        public bool DeleteCourse(string courseId)
        {
            using (Context.Tracer.DoTrace("EportfolioCourseActions.DeleteCourses(courseId={0})", courseId))
            {
                var cmd = new DeleteCourses();
                Bdc.Course course = CourseActions.GetCourseByCourseId(courseId);

                cmd.Add(course.ToCourse());

                try
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                    courseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
                    Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username, courseId);
                    Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username);
                    Context.CacheProvider.InvalidateCourseList(Context.CurrentUser.Username);

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets the dashboard courses.
        /// This should be new one to use to get DashboardCourses
        /// </summary>
        /// <param name="referenceId">The reference id.</param>
        /// <returns>IList<Course></returns>
        public IList<Course> GetDashboardCoursesSpecificToProduct(string referenceId, string productId, string termId)
        {
            var allEnrollments = EnrollmentActions.ListEnrollments(referenceId, productId);
            var courses = (from c in allEnrollments where c.Course != null select c.Course).Distinct(new CourseComparer()).Where(i => i.AcademicTerm == termId).ToList();

            return courses;
        }

        #endregion

        /// <summary>
        /// Converts list of courses to list of dashboard items for the dashboard view 
        /// </summary>
        /// <param name="resultMyCourses">List<Bdc.Course></param>
        /// <returns>List<Bdc.DashboardItem></returns>
        private List<Bdc.DashboardItem> ToDashboardItem(List<Bdc.Course> resultMyCourses)
        {
            List<Bdc.DashboardItem> results = new List<DashboardItem>();

            if (!resultMyCourses.IsNullOrEmpty())
            {
                var domains = Context.GetRaUserDomains().Distinct().OrderBy(d => d.Id);
                string parentId = resultMyCourses.First().Id;
                var possibleAcademicTerms = CourseActions.ListAcademicTerms();

                foreach (Bdc.Course item in resultMyCourses)
                {
                    Bdc.DashboardItem courseData = new Bdc.DashboardItem();
                    courseData.Course = item;

                    Bdc.CourseAcademicTerm academicTerm = possibleAcademicTerms.FirstOrDefault(a => a.Id == item.AcademicTerm);
                    courseData.Course.AcademicTerm = (academicTerm != null && !String.IsNullOrWhiteSpace(academicTerm.Id))
                                                        ? academicTerm.Id
                                                        : (possibleAcademicTerms.Filter(a => a.Name == item.AcademicTerm).Count() > 0)
                                                            ? possibleAcademicTerms.FirstOrDefault(a => a.Name == item.AcademicTerm).Id
                                                            : String.Empty;

                    courseData.Status = GetCourseStatus(item);
                    courseData.CourseId = item.Id;
                    courseData.CourseTitle = item.Title;
                    courseData.Count = item.StudentEnrollmentCount;

                    var domain = domains.FirstOrDefault(d => d.Id == item.Domain.Id);
                    courseData.DomainName = (domain == null || (domain.Name.ToLowerInvariant() == "pxgeneric")) ? String.Empty : domain.Name;
                    courseData.Level = (parentId == item.ParentId) ? "1" : "0";

                    if (parentId != item.ParentId)
                    {
                        parentId = item.Id;
                    }

                    if (item == null)
                    {
                        courseData.SharedInstructors = string.Join(", ", SharedCourseActions.getSharedToUsers(item.Id).Select(user => user.FormattedName).ToArray());
                    }

                    results.Add(courseData);
                }
            }

            return results;
        }

        /// <summary>
        /// Filters course of enrollment from dashboard
        /// </summary>
        /// <param name="dashboardCourseId">The dashboard course id.</param>
        /// <returns>List<Bdc.Course></returns>
        private List<Bdc.Course> FilterEnrollments(List<Bdc.Enrollment> enrollments)
        {
            List<Bdc.Course> resultMyCourses = new List<Course>();
            List<Bdc.Course> coursesFromProductCourses = new List<Course>();
            bool result = false;

            foreach (Enrollment e in enrollments)
            {
                result = false;
                Bdc.Course localCourse = e.Course;
                string strCourseType = localCourse.CourseType;

                if (localCourse != null &&
                    !localCourse.ParentId.IsNullOrEmpty() &&
                    localCourse.CourseSubType != "instructor_dashboard" &&
                    localCourse.CourseSubType != "program_dashboard" &&
                    localCourse.ProductCourseId != localCourse.Id)
                {
                    result = true;
                }

                if ((strCourseType == Context.Course.CourseType.ToString()) && result && resultMyCourses.Count(c => c.Id.Equals(localCourse.Id)) == 0)
                {
                    resultMyCourses.Add(localCourse);
                }
            }

            return resultMyCourses;
        }

        

        /// <summary>
        /// Gets course status
        /// </summary>
        /// <param name="course">Bdc.Course</param>
        /// <returns>Closed/Open</returns>
        private string GetCourseStatus(Bdc.Course course)
        {
            string status = string.Empty;

            if (Convert.ToDateTime(course.ActivatedDate) == DateTime.MinValue)
            {
                status = "Closed";
            }
            else
            {
                status = "Open";
            }

            return status;
        }

        /// <summary>
        /// sorts list of child courses based on parentid
        /// </summary>
        /// <param name="courses">List<Bdc.Course></param>
        /// <returns>List<Bdc.Course></returns>
        private List<Bdc.Course> SortCourseChildren(List<Course> courses)
        {
            var result = new List<Bdc.Course>();

            foreach (var course in courses)
            {
                if (result.Count(o => o.Id.Equals(course.Id)) == 0)
                {
                    if (courses.Count(o => o.Id.Equals(course.ParentId)) == 0 || result.Count(o => o.Id.Equals(course.ParentId)) > 0)
                    {
                        result.Add(course);

                        foreach (var child in courses.Where(c => c.ParentId.Equals(course.Id)))
                        {
                            result.Add(child);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Populates user's domain if one is missing
        /// </summary>
        private void PopulateUser()
        {
            if (Context.CurrentUser.DomainId.IsNullOrEmpty())
            {
                IEnumerable<Domain> userDomains = Context.GetRaUserDomains().Distinct();

                if (userDomains.Any())
                {
                    Context.CurrentUser.DomainId = userDomains.First().Id.ToString();
                }
            }
        }

        /// <summary>
        /// This is a very unique way of comparing two reference objects, hence the private accessibility
        /// </summary>
        private class CourseComparer : IEqualityComparer<Course>
        {

            public bool Equals(Course x, Course y)
            {
                return x.Id.Equals(y.Id);
            }

            public int GetHashCode(Course obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}