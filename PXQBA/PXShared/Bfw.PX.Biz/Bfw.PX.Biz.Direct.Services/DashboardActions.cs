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
    /// Implements IDiscussionActions using direct connection to DLAP.
    /// </summary>
    public class DashboardActions : IDashboardActions
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

        /// <summary>
        /// Exposes the related functions for 'Course Sharing' functionality across PX
        /// </summary>
        protected IAutoEnrollmentActions AutoEnrollmentActions { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardActions"/> class.
        /// </summary>
        /// <param name="ctx">The IBusinessContext implementation.</param>
        /// <param name="sessionManager">The session manager.</param>
        public DashboardActions(IBusinessContext ctx, ISessionManager sessionManager, ICourseActions courseActions, IAutoEnrollmentActions autoEnrollmentActions, IEnrollmentActions enrollmentActions)
        {
            Context = ctx;
            SessionManager = sessionManager;
            CourseActions = courseActions;
            AutoEnrollmentActions = autoEnrollmentActions;
            EnrollmentActions = enrollmentActions;
        }

        #endregion

        #region IDiscussionActions Members

        /// <summary>
        /// Create all necessary dashboard courses.
        /// </summary>
        /// <param name="targetDomain"></param>
        /// <param name="courseSubType"></param>
        /// <returns></returns>
        public Course CreateDashboardCourses(string targetDomain, string courseSubType)
        {
            Course resultCourse = null;

            using (Context.Tracer.DoTrace("DashboardActions.CreateDashboardCourses(targetDomain={0}, courseSubType={1})", targetDomain, courseSubType))
            {

                bool instructorDbExist = false;
                bool programDbExist = false;
                string contextUrl = string.Empty;

                string currentCourseId = Context.Course.Id;
                string productCourseId = Context.ProductCourseId;

                string programDashboardId = "";
                string instructorDashboardId = "";

                // Check if a program dashboard exist for the current domain
                if (!string.IsNullOrEmpty(targetDomain))
                {
                    programDashboardId = GetProgramDashboardId(targetDomain, courseSubType, productCourseId);
                }
                programDbExist = !string.IsNullOrEmpty(programDashboardId);

                if (!programDbExist)
                {
                    //Creating Program Dashboard
                    programDashboardId = CreateDashboard(
                        productCourseId,
                        "Program Dashboard Course",
                        targetDomain,
                        "",
                        Context.Course.CourseTimeZone,
                          "program_dashboard",
                        true
                       );

                    programDbExist = !string.IsNullOrEmpty(programDashboardId);
                }

                instructorDashboardId = GetDashboardId(
                        Context.ProductCourseId,
                        "instructor_dashboard",
                        Context.CurrentUser.Username);

                instructorDbExist = !String.IsNullOrEmpty(instructorDashboardId);

                // if user does not have a dashboard yet create one 
                if (!instructorDbExist && programDbExist)
                {
                    string dbTitle = Context.Course.Title + " Dashboard";
                    //Creates Instructor Dashboard
                    instructorDashboardId = CreateDashboard(
                        programDashboardId,
                        dbTitle,
                        targetDomain,
                        "",
                        Context.Course.CourseTimeZone,
                        "instructor_dashboard");
                }

                resultCourse = CourseActions.GetCourseByCourseId(instructorDashboardId);
            }
            return resultCourse;
        }

        public string CreateDashboard(string parentId, string title, string targetDomain, string academicTerm, string courseTimeZone, string dashboard_type, bool createProgramDashboard = false)
        {
            using (Context.Tracer.DoTrace("DashboardActions.CreateDashboard(parentId={0}, title={1}, targetDomain={2}, academicTerm={3})", parentId, title, targetDomain, academicTerm))
            {
                var productId = string.IsNullOrEmpty(Context.ProductCourseId) ? Context.EntityId : Context.ProductCourseId;
                var programCourse = CourseActions.GetCourseByCourseId(parentId);
                if (programCourse != null)
                {
                    var course = CourseActions.CreateDerivedCourse(programCourse, targetDomain);

                    course.Title = title;
                    course.CourseProductName = title;
                    course.InstructorName = "";
                    course.CourseTimeZone = courseTimeZone;
                    course.CourseType = Context.Course.CourseType.ToString();
                    course.CourseHomePage = Context.Course.CourseHomePage;
                    course.CourseOwner = Context.CurrentUser.Id;
                    course.CourseTemplate = parentId;
                    course.ProductCourseId = productId;
                    /*course.DashboardSettings.IsInstructorDashboardOn = true;
                    if (createProgramDashboard == true)
                    {
                        course.DashboardSettings.IsProgramDashboardOn = true;
                    }*/
                    course.CourseSubType = dashboard_type;
                    if (academicTerm != null) { course.AcademicTerm = academicTerm; }

                    course = CourseActions.UpdateCourses(new List<Bdc.Course>() { course }).First();
                    CourseActions.ActivateCourse(course);

                    // Create and enroll users for new course.
                    //AutoEnrollmentActions.CreateEnrollments();

                    //Insert Record in DB with newly created dashboard course id
                    InsertDashboardData(Context.CurrentUser.Id, Context.CurrentUser.Username, targetDomain, course.Id, createProgramDashboard);


                    return course.Id;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the Instructor dashboard course id based on the search criteria.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public string GetDashboardId(string productCourseId, string dashbord_type, string ref_id)
        {
            using (Context.Tracer.DoTrace("DashboardActions.GetDashboardId(productCourseId={0}, dashbord_type={1}, ref_id={2} )", productCourseId, dashbord_type, ref_id))
            {
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();

                    var records = db.Query("GetDashboardId2 @0, @1, @2", productCourseId, dashbord_type, ref_id);

                    if (!records.IsNullOrEmpty())
                    {
                        var item = records.First();
                        if (item.IsDBNull("Dashboard_Id"))
                            return string.Empty;
                        return item.Int64("Dashboard_Id").ToString();
                    }
                }
                finally
                {
                    db.EndSession();
                }

                return null;
            }
        }
        /// <summary>
        /// Gets the Program dashboard course id based on the search criteria.
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public string GetProgramDashboardId(string domainId, string coursetype, string productCourseId)
        {
            using (Context.Tracer.DoTrace("DashboardActions.GetProgramDashboardId(domainId={0}, coursetype={1})", domainId, coursetype))
            {
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();

                    var records = db.Query("GetProgramDashboardId @0, @1, @2", Int64.Parse(domainId), coursetype.ToLowerInvariant(), productCourseId.ToLowerInvariant());

                    if (!records.IsNullOrEmpty())
                    {
                        var item = records.First();
                        return item.Int64("Dashboard_id").ToString();
                    }
                }
                finally
                {
                    db.EndSession();
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a set of eportfolio courses.
        /// </summary>
        /// <returns></returns>
        public Bdc.DashboardData GetDashboardData(bool getChildrenCourses, bool getNonDashboardCourses)
        {

            using (Context.Tracer.StartTrace("DashboardActions.GetDashboardData2()"))
            {
                Bdc.DashboardData result = new Bdc.DashboardData();

                List<Bdc.Course> resultMyCourses = new List<Bdc.Course>();
                List<Bdc.Course> coursesFromProductCourses = new List<Bdc.Course>();
                using (Context.Tracer.StartTrace("Search For and load Product Template Courses"))
                {

                    using (Context.Tracer.StartTrace("Search For Product Template Courses"))
                    {
                        if (result == null)
                        {
                            result = new DashboardData();
                        }

                        List<Enrollment> enrollments = new List<Enrollment>();
                        enrollments = EnrollmentActions.ListEnrollments(Context.CurrentUser.Username, Context.ProductCourseId, getEnrollmentCount: true).ToList();

                        if (enrollments != null)
                        {
                            List<Bdc.Course> coursesFromDashboard = new List<Course>();

                            coursesFromDashboard = FilterEnrollments(enrollments, Context.CourseId);
                            coursesFromDashboard = coursesFromDashboard.OrderByDescending(course => course.CreatedDate).ToList();
                            if (getChildrenCourses == true && coursesFromDashboard != null)
                            {

                                coursesFromDashboard = ChildrenCourse(enrollments, coursesFromDashboard);
                            }

                            if (getNonDashboardCourses == true)
                            {
                                List<Bdc.Course> coursesNotFromDashboard = new List<Course>();

                                coursesNotFromDashboard = FilterEnrollments(enrollments, Context.Course.ProductCourseId);
                                coursesNotFromDashboard = coursesNotFromDashboard.OrderByDescending(course => course.CreatedDate).ToList();
                                if (getChildrenCourses == true && coursesNotFromDashboard != null)
                                {

                                    coursesNotFromDashboard = ChildrenCourse(enrollments, coursesNotFromDashboard);
                                }

                                coursesFromDashboard.AddRange(coursesNotFromDashboard);
                            }
                            coursesFromDashboard = coursesFromDashboard.Distinct<Course>(new CourseComparer()).ToList();
                            result.InstructorCourses = ToDashboarItem(coursesFromDashboard);

                        }
                    }
                }
                return result;
            }
        }

        public List<Bdc.DashboardItem> ToDashboarItem(List<Bdc.Course> resultMyCourses)
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
                    courseData.Count = GetStudentsEnrolledCount(item);
                                        
                    var domain = domains.FirstOrDefault(d => d.Id == item.Domain.Id);
                    courseData.DomainName = (domain == null || (domain.Name.ToLowerInvariant() == "pxgeneric"))? String.Empty : domain.Name;

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
        /// <returns></returns>
        public List<Bdc.Course> FilterEnrollments(List<Bdc.Enrollment> enrollments, string derviedId)
        {
            List<Bdc.Course> resultMyCourses = new List<Course>();
            List<Bdc.Course> coursesFromProductCourses = new List<Course>();
            bool result = false;

            foreach (Enrollment e in enrollments)
            {
                result = false;
                Bdc.Course localCourse = e.Course;
                string strCourseType = localCourse.CourseType;


                if (localCourse != null && !localCourse.ParentId.IsNullOrEmpty() && localCourse.ParentId == derviedId && localCourse.CourseSubType != "program_dashboard" && localCourse.ProductCourseId != localCourse.Id)
                {
                    result = true;
                }

                if ((strCourseType == Context.Course.CourseType.ToString()) && result)
                {

                    resultMyCourses.Add(localCourse);
                }

            }

            return resultMyCourses;

        }


        /// <summary>
        /// Gets all dashboard courses.
        /// </summary>
        /// <param name="dashboardCourseId">The dashboard course id.</param>
        /// <returns></returns>
        public List<Bdc.Course> GetAllDashboardCourses(string dashboardCourseId)
        {
            using (Context.Tracer.StartTrace("DashboardActions.GetAllDashboardCourses()"))
            {
                var resultMyCourses = new List<Bdc.Course>();
                using (Context.Tracer.StartTrace("Search For Courses"))
                {
                    using (Context.Tracer.StartTrace("Search For Product Template Courses"))
                    {
                        var programInfo = InitializeProgramInfo(dashboardCourseId);
                        var enrollments = EnrollmentActions.ListEnrollments(Context.CurrentUser.Username).ToList();
                        if (enrollments != null)
                        {
                            resultMyCourses = GetOnlyDashboardCoursesFromList(enrollments, programInfo);

                            if (resultMyCourses.Count > 0)
                            {
                                resultMyCourses = resultMyCourses.OrderByDescending(course => course.CreatedDate).ToList();
                            }

                        }
                        return resultMyCourses;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the only dashboard courses from list.
        /// </summary>
        /// <param name="enrollments">The enrollments.</param>
        /// <param name="programInfo">The program info.</param>
        /// <returns></returns>
        private List<Course> GetOnlyDashboardCoursesFromList(List<Enrollment> enrollments, UserProgram programInfo)
        {
            var courses = new List<Course>();
            foreach (Enrollment e in enrollments)
            {
                var course = e.Course;
                if (course != null)
                {
                    if (course.ProductCourseId == Context.ProductCourseId)
                    {
                        bool CourseIsFromDashboardFlag = false;
                        if (Context.AccessLevel == AccessLevel.Instructor)
                        {
                            if (programInfo != null)
                            {
                                CourseIsFromDashboardFlag =
                                    CourseIsFromInstDashboard(course, programInfo);
                            }
                        }
                        else
                            CourseIsFromDashboardFlag = true;

                        if ((course.CourseType == Context.Course.CourseType.ToString()) && CourseIsFromDashboardFlag)
                        {
                            courses.Add(course);
                        }
                    }
                }
            }
            return courses;
        }

        /// <summary>
        /// Initializes the program info.
        /// </summary>
        /// <param name="dashboardCourseId">The dashboard course id.</param>
        /// <returns></returns>
        private UserProgram InitializeProgramInfo(string dashboardCourseId)
        {
            var programInfo = new UserProgram();
            if (Context.AccessLevel == AccessLevel.Instructor)
            {
                programInfo.UserDashboardId = Int64.Parse(dashboardCourseId);

            }
            return programInfo;
        }

        public Bdc.DashboardData GetProgramDashboardData()
        {
            using (Context.Tracer.DoTrace("DashboardActions.GetProgramDashboardData()"))
            {
                Bdc.DashboardData results = new DashboardData();

                CourseType courseType = (Bdc.CourseType)Enum.Parse(typeof(Bdc.CourseType), Context.Course.CourseType, true);
                List<Course> courseList = CourseActions.ListCourses(courseType, Context.Course.AcademicTerm.ToString(), true).ToList();

                List<Course> courses = CourseActions.GetCourseListInformation(courseList).ToList();

                foreach (Bdc.Course item in courses)
                {
                    if (item.CourseSubType == "regular")
                    {
                        Bdc.DashboardItem courseData = new Bdc.DashboardItem();

                        courseData.Course = item;
                        courseData.Status = GetCourseStatus(item);
                        courseData.Count = GetStudentsEnrolledCount(item);
                        courseData.CourseId = item.Id;
                        courseData.CourseTitle = item.Title;
                        courseData.OwnerName = item.InstructorName;

                        if (item == null)
                        {
                            courseData.SharedInstructors = string.Join(", ", SharedCourseActions.getSharedToUsers(item.Id).Select(user => user.FormattedName).ToArray());
                        }

                        results.InstructorCourses.Add(courseData);
                    }
                }

                return results;
            }

        }


        /// <summary>
        /// Updates the user program record with the instructor dashboard id
        /// </summary>
        /// <param name="search"></param>
        /// <param name="dashboardId"></param>
        private void UpdateUserProgram(Bdc.UserProgramSerach search, string dashboardId)
        {
            using (Context.Tracer.DoTrace("DashboardActions.UpdateUserProgram(search={0}, dashboardId={1}) db(UpdateUserProgram)", search, dashboardId))
            {
                var db = new DatabaseManager("PXData");

                try
                {
                    db.StartSession();
                    db.ExecuteNonQuery("UpdateUserProgram @0, @1, @2, @3, @4", search.ProgramId, dashboardId, search.UserId, search.UserRefId, search.UserDomainId);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        private int GetStudentsEnrolledCount(Bdc.Course eportfolioCourses)
        {
            int enrollmentCount = 0;
            using (Context.Tracer.DoTrace("EportfolioCourseActions.GetStudentsEnrolledCount(eportfolioCourses(eportfolioCourses)) eportfolioCourses.Id={0}", eportfolioCourses.Id))
            {
                Adc.EntitySearch searchParameters = new Adc.EntitySearch() { CourseId = eportfolioCourses.Id };

                var cmd = new GetEntityEnrollmentList()
                {
                    SearchParameters = searchParameters
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);

                if (cmd.Enrollments != null)
                {
                    enrollmentCount = cmd.Enrollments.Map(i => i.ToEnrollment()).Where(e => e.Flags.Contains("Participate") == true).Count();
                }
            }
            return enrollmentCount;
        }

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
        /// This method checks to see if the course is part of either the instructor's dashboard.
        /// This is important because program template should be
        /// shown in the course list widget as well as templates created by the instructor.
        /// </summary>
        /// <param name="course">Course to check hierarchy of</param>
        /// <param name="program">Information about the program the current user is in</param>
        /// <returns>true if the course is acceptable</returns>
        private bool CourseIsFromInstDashboard(Bdc.Course course, Bdc.UserProgram program)
        {
            using (Context.Tracer.DoTrace("DashboardActions.CourseIsFromInstDashboard(course={0}, program={1})", course, program))
            {
                bool result = false;
                if (program != null)
                {
                    if (!course.ParentId.IsNullOrEmpty() && course.ParentId == program.UserDashboardId.Value.ToString())
                    {
                        result = true;
                    }
                }

                return result;

            }
        }

        /// <summary>
        /// This method checks to see if the course is part of either the instructor's dashboard.
        /// This is important because program template should be
        /// shown in the course list widget as well as templates created by the instructor.
        /// </summary>
        /// <param name="course">Course to check hierarchy of</param>
        /// <param name="program">Information about the program the current user is in</param>
        /// <returns>true if the course is acceptable</returns>
        private bool CourseIsFromProductCourse(Bdc.Course course)
        {
            using (Context.Tracer.DoTrace("DashboardActions.CourseIsFromInstDashboard(course={0})", course))
            {
                bool result = false;

                if (!course.ParentId.IsNullOrEmpty() && course.ParentId == Context.ProductCourseId)
                {
                    result = true;
                }

                return result;

            }
        }
        public List<Bdc.Course> ChildrenCourse(List<Enrollment> enrollments, List<Course> parentCourses)
        {
            List<Course> courses = new List<Course>(parentCourses);
            List<Enrollment> childrenCourseenrollmentsFound = new List<Enrollment>();
            List<Bdc.Course> childrenCourses = new List<Course>();

            if (parentCourses.Count > 0)
            {
                foreach (Course p in parentCourses)
                {
                    childrenCourseenrollmentsFound = enrollments.FindAll(i => i.Course.ParentId == p.Id);

                    if (childrenCourseenrollmentsFound.Count > 0)
                    {
                        int index = 0;
                        index = courses.FindIndex(i => i.Id == p.Id);
                        index++;
                        foreach (Enrollment e in childrenCourseenrollmentsFound)
                        {
                            courses.Insert(index, e.Course);
                            index++;
                        }

                    }
                }

            }
            return courses;


        }

        /// <summary>
        /// This method checks to see if the course is part of either the
        /// program manager dashboard. This is important because program template should be
        /// shown in the course list widget as well as templates created by the instructor.
        /// </summary>
        /// <param name="course">Course to check hierarchy of</param>
        /// <param name="program">Information about the program the current user is in</param>
        /// <returns>true if the course is acceptable</returns>
        private bool CourseIsFromPMDashboard(Bdc.Course course, Bdc.UserProgram program)
        {
            using (Context.Tracer.DoTrace("DashboardActions.CourseIsFromPMDashboard(course={0}, program={1})", course, program))
            {
                bool result = false;
                if (program != null)
                {
                    if (program.ProgramDashboardId.HasValue && course.DashboardCourseId == program.ProgramDashboardId.Value.ToString())
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        public void InsertDashboardData(string userId, string refId, string domainId, string courseId, bool programDashboard = false)
        {
            using (Context.Tracer.DoTrace("DashboardActions.InsertDashboard(userId={0}, refId={1}, domainId={2}, courseId={3}, programDashboard={4})", userId, refId, domainId, courseId, programDashboard))
            {
                string dashboardType = "";
                var db = new DatabaseManager("PXData");
                try
                {


                    db.StartSession();
                    if (programDashboard == true)
                    {
                        dashboardType = "program_dashboard";

                        //check for program if it's not here then create it

                        var records = db.Query("GetProgram @0", Convert.ToInt32(domainId));

                        if (records.IsNullOrEmpty())
                        {
                            db.ExecuteNonQuery("InsertProgram2  @0", Convert.ToInt32(domainId));
                        }

                        db.ExecuteNonQuery("InsertProgramDashboardData  @0, @1, @2, @3, @4, @5, @6, @7", Convert.ToInt32(userId), Convert.ToInt32(refId), Convert.ToInt32(domainId), Convert.ToInt32(courseId), Convert.ToString(Context.Course.SubType), dashboardType, Convert.ToString(Context.Course.SubType), Convert.ToString(Context.ProductCourseId));
                    }
                    else if (programDashboard == false)
                    {


                        dashboardType = "instructor_dashboard";
                        dashboardType.ToLowerInvariant();
                        db.ExecuteNonQuery("InsertDashboardData2  @0, @1, @2, @3, @4, @5, @6", Convert.ToInt32(userId), Convert.ToInt32(refId), Convert.ToInt32(domainId), Convert.ToInt32(courseId), Convert.ToString(Context.Course.SubType), dashboardType, Convert.ToString(Context.ProductCourseId));

                    }
                    else
                    {

                        dashboardType = "student_dashboard";
                        dashboardType.ToLowerInvariant();
                        db.ExecuteNonQuery("InsertDashboardData2  @0, @1, @2, @3, @4, @5, @6", Convert.ToInt32(userId), Convert.ToInt32(refId), Convert.ToInt32(domainId), Convert.ToInt32(courseId), Convert.ToString(Context.Course.SubType), dashboardType, Convert.ToString(Context.ProductCourseId));
                    }
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        public bool DeleteCourses(string courseId)
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
        /// <returns></returns>
        public IList<Course> GetDashboardCoursesSpecificToProduct(string referenceId, string productId, string termId)
        {  

            var allEnrollments = EnrollmentActions.ListEnrollments(referenceId, productId);
            var courses = (from c in allEnrollments where c.Course != null select c.Course).Distinct(new CourseComparer()).Where(i => i.AcademicTerm == termId).ToList();
            return courses;
        }

        #endregion

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