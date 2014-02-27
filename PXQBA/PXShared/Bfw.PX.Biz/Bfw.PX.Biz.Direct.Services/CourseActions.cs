using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bdc = Bfw.PX.Biz.DataContracts;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Common.Logging;
using System.Xml.Linq;
using Bfw.PX.Biz.DataContracts;
using System.Configuration;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class CourseActions : ICourseActions
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

        /// <summary>
        /// Gets or sets the note actions.
        /// </summary>
        /// <value>
        /// The note actions.
        /// </value>
        protected INoteActions NoteActions { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected IDomainActions DomainActions { get; set; }

        /// <summary>
        /// A const for that holds the instructor persmission flags.
        /// </summary>
        private readonly DlapRights INSTRUCTOR_FLAGS = (DlapRights)Int64.Parse(System.Configuration.ConfigurationManager.AppSettings["InstructorPermissionFlags"]);

        /// <summary>
        /// A const for that holds the student persmission flags.
        /// </summary>
        private readonly DlapRights STUDENT_FLAGS = (DlapRights)Int64.Parse(System.Configuration.ConfigurationManager.AppSettings["StudentPermissionFlags"]);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseActions"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="sessionManager">The session manager.</param>
        public CourseActions(IBusinessContext context, ISessionManager sessionManager, INoteActions noteActions, IContentActions contentActions, IDomainActions domainActions)
        {
            Context = context;
            SessionManager = sessionManager;
            NoteActions = noteActions;
            ContentActions = contentActions;
            this.DomainActions = domainActions;
        }

        #endregion

        #region Methods


        /// <summary>
        /// Sets up the currently logged in user as the instructor. This fixes the problem were the admin
        /// user is set as the instructor. Also, set up the instructor's shadow student user.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <param name="courseOwner">Agilix Course ID (within current domain / institute) for Owner of the course.</param>
        public void SetCourseEnrollments(IEnumerable<Bdc.Course> courses, string courseOwner, bool enableStudentView=true)
        {
            using (Context.Tracer.DoTrace("CourseActions.SetCourseEnrollments(courses, courseOwner={0})", courseOwner))
            {
                var cmd = new CreateEnrollment();

                foreach (var course in courses)
                {
                    cmd.Add(new Adc.Enrollment()
                    {
                        User = new Adc.AgilixUser() { Id = courseOwner },
                        Course = course.ToCourse(),
                        Status = "1",
                        StartDate = DateTime.Now.GetCourseDateTime(Context),
                        EndDate = DateTime.MaxValue,
                        Flags = INSTRUCTOR_FLAGS
                    });

                    if (enableStudentView)
                    {
                        cmd.Add(new Adc.Enrollment()
                        {
                            User = new Adc.AgilixUser() {Id = courseOwner},
                            Course = course.ToCourse(),
                            Status = "10",
                            StartDate = DateTime.Now.GetCourseDateTime(Context),
                            EndDate = DateTime.MaxValue,
                            Flags = STUDENT_FLAGS
                        });
                    }
                }

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
            }
        }

        #endregion

        #region ICourseActions Members

        /// <summary>
        /// Adds email for Instructor after course gets activated 
        /// </summary>
        /// <param name="email">Actual Email content after rendering course confirmation template</param>
        /// <param name="courseTitle">Title of course</param>
        /// <param name="courseId">current Course Id, it can be different from context course id in case called from dashboard</param>
        /// <param name="productCourseId">Product course id for the course id</param>
        public void SendInstructorEmail(string email, string courseTitle, string courseId, string productCourseId, string instructorId)
        {
            using (Context.Tracer.StartTrace(String.Format("AssignmentActions.AddReminder(email={0}, courseTitle={1}, courseId={2}, productCourseId={3}, InstructorId={4}) - db (AddEmailTracking)", email, courseTitle, courseId, productCourseId, instructorId)))
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(courseId) || string.IsNullOrEmpty(productCourseId) || string.IsNullOrEmpty(instructorId))
                {
                    return;
                }
                var db = new Bfw.Common.Database.DatabaseManager("PXData");

                if (!string.IsNullOrEmpty(email))
                {
                    email = System.Text.RegularExpressions.Regex.Replace(email, @"\s+", " ");
                }

                try
                {
                    var sendOn = DateTime.Now.AddMinutes(1);
                    var templateId = System.Configuration.ConfigurationManager.AppSettings["CourseActivationEmailTemplate"];
                    var EventTypeId = System.Configuration.ConfigurationManager.AppSettings["CourseActivationEmailEvent"];

                    db.StartSession();
                    db.ExecuteNonQuery("AddEmailTracking @0, @1, @2, @3, @4, @5, @6, @7, @8, @9", courseId,
                        Context.ProductCourseId, instructorId, sendOn, "add", EventTypeId, courseId, /*Guid.NewGuid().ToString(),*/
                        "Your course is ready", email, templateId);
                }
                finally
                {
                    db.EndSession();
                }
            }
        }

        public String getApplicationType(String courseType)
        {
            String application_type = "";
            if (Context != null && Context.Course != null && !String.IsNullOrEmpty(Context.Course.CourseType))
            {
                if (Context.Course.CourseType.Contains(CourseType.Eportfolio.ToString()) || Context.Course.CourseType.Equals(CourseType.ProgramDashboard.ToString()))
                {
                    application_type = CourseType.Eportfolio.ToString();
                }
                else
                {
                    application_type = Context.Course.CourseType;
                }
            }
            return application_type;
        }

        /// <summary>
        /// Gets an enrollment for the given user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="entityId">The entity ID.</param>
        /// <returns>Enrollment that matches the parameters</returns>
        public Bdc.Enrollment GetEnrollment(string userId, string entityId)
        {
            Bdc.Enrollment result = null;

            using (Context.Tracer.DoTrace("CourseActions.GetEnrollment(userId={0}, entityId={1})", userId, entityId))
            {
                var cmd = new GetGrades()
                {
                    SearchParameters = new Adc.GradeSearch()
                    {
                        UserId = userId,
                        EntityId = entityId
                    }
                };

                SessionManager.CurrentSession.Execute(cmd);

                if (cmd.Enrollments.Count() > 1)
                {
                    // Note: is this allowed?
                    throw new Exception("Received more than one enrollment for a single user and single entity.");
                }

                if (!cmd.Enrollments.IsNullOrEmpty())
                {
                    result = cmd.Enrollments.FirstOrDefault().ToEnrollment();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a course by course ID.
        /// </summary>
        /// <param name="courseId">The course ID.</param>
        /// <returns></returns>
        public Bdc.Course GetCourseByCourseId(string courseId)
        {
            Bdc.Course result = null;

            using (Context.Tracer.DoTrace("CourseActions.GetCourseByCourseId(courseId={0})", courseId))
            {
                result = Context.CacheProvider.FetchCourse(courseId);
                if (result != null)
                {
                    Context.Logger.Log(string.Format("Course {0} loaded from cache", courseId), LogSeverity.Debug);
                }
                else
                {
                    var cmd = new GetCourse()
                    {
                        SearchParameters = new Adc.CourseSearch()
                        {
                            CourseId = courseId
                        }
                    };

                    SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                    result = cmd.Courses.First().ToCourse();

                    Context.CacheProvider.StoreCourse(result);
                }

            }

            return result;
        }

        /// <summary>
        /// Gets a list of courses by a list of ids
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public List<Course> GetCoursesByCourseIds(IEnumerable<string> courseIds)
        {
            List<Course> courses = new List<Course>();
            using (Context.Tracer.DoTrace("CourseActions.GetCoursesByCourseIds(courseId={0})", courseIds))
            {
                var batch = new Batch { RunAsync = true };

                foreach (var courseId in courseIds)
                {
                    batch.Add(new GetCourse()
                    {
                        SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
                        {
                            CourseId = courseId
                        }
                    });
                }


                SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                for (int index = 0; index < batch.Commands.Count(); index++)
                {
                    if (batch.CommandAs<GetCourse>(index).Courses.IsNullOrEmpty())
                    {
                        continue;
                    }
                    courses.Add(batch.CommandAs<GetCourse>(index).Courses.First().ToCourse());
                }
            }
            return courses;
        }

        public IEnumerable<Bdc.Course> GetCourseListInformation(IEnumerable<Bdc.Course> courses)
        {
            var fullcourses = new List<Bdc.Course>();

            using (Context.Tracer.DoTrace("CourseActions.GetCourseListInformation()"))
            {
                Batch batch = new Batch();
                batch.RunAsync = true;
                foreach (Bdc.Course c in courses)
                {

                    batch.Add(new GetCourse()
                    {
                        SearchParameters = new Adc.CourseSearch()
                        {
                            CourseId = c.Id
                        }
                    });
                }

                if (!batch.Commands.IsNullOrEmpty())
                {
                    SessionManager.CurrentSession.ExecuteAsAdmin(batch);
                }

                var list = new List<Adc.Course>();
                for (int ord = 0; ord < batch.Commands.Count(); ord++)
                {
                    if (batch.CommandAs<GetCourse>(ord).Courses != null)
                    {
                        list.AddRange(batch.CommandAs<GetCourse>(ord).Courses);
                    }
                }

                if (!list.IsNullOrEmpty())
                {
                    foreach (Adc.Course c in list)
                    {
                        fullcourses.Add(c.ToCourse());
                    }
                }

            }

            return fullcourses;
        }

        /// <summary>
        /// Gets a set of courses by title match.
        /// </summary>
        /// <param name="titlePart">The title part.</param>
        /// <returns></returns>
        public IEnumerable<Bdc.Course> GetCoursesByTitleMatch(string titlePart)
        {
            IEnumerable<Bdc.Course> result = null;

            using (Context.Tracer.DoTrace("CourseActions.GetCourseByTitleMatch(titlePart={0})", titlePart))
            {
                var cmd = new GetCourse()
                {
                    SearchParameters = new Adc.CourseSearch()
                    {
                        Title = titlePart
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                result = cmd.Courses.Map(c => c.ToCourse());
            }

            return result;
        }

        /// <summary>
        /// returns courses available by university and academic term
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="termId"></param>
        /// <param name="applicationType"></param>
        /// <returns></returns>
        public IEnumerable<Bdc.Course> GetCourseByDomainTerm(string domainId, string termId, string applicationType)
        {
            IEnumerable<Bdc.Course> result = null;

            using (Context.Tracer.DoTrace("CourseActions.GetCourseByDomainTerm(domainId={0}, termid={1}, applicationType={2})", domainId, termId, applicationType))
            {
                var cmd = new GetCourse()
                {
                    SearchParameters = new Adc.CourseSearch()
                    {
                        DomainId = domainId,
                        Query = string.Format(@"/meta-bfw_academic_term='{0}' AND /meta-bfw_course_type = '{1}'", termId, applicationType)
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                result = cmd.Courses.Map(c => c.ToCourse());
            }

            return result;
        }

        /// <summary>
        /// Lists all courses.
        /// </summary>
        /// <returns></returns>
        public List<Bdc.Course> ListCourses()
        {
            List<Bdc.Course> result = null;

            using (Context.Tracer.StartTrace("CourseActions.ListCourses()"))
            {
                var cmd = new GetCourse()
                {
                    SearchParameters = new Adc.CourseSearch()
                    {
                        Title = string.Empty
                    }
                };

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                result = cmd.Courses.Map(c => c.ToCourse()).ToList();
            }

            return result;
        }

        /// <summary>
        /// Updates a course.
        /// </summary>
        /// <param name="courses">The course.</param>
        /// <returns></returns>
        public Bdc.Course UpdateCourse(Bdc.Course course)
        {
            List<Bdc.Course> courseList = new List<Bdc.Course>();
            courseList.Add(course);

            var result = UpdateCourses(courseList);

            return result.FirstOrDefault();
        }

        /// <summary>
        /// Updates a set of courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public List<Bdc.Course> UpdateCourses(List<Bdc.Course> courses)
        {
            List<Bdc.Course> result = null;

            using (Context.Tracer.StartTrace("CourseActions.UpdateCourses(courses)"))
            {
                var cmd = new UpdateCourses();
                cmd.Add(courses.Map(c => c.ToCourse()).ToList());

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                result = cmd.Courses.Map(c => c.ToCourse()).ToList();

                foreach (Bdc.Course course in result)
                {
                    Context.CacheProvider.InvalidateCourseContent(course);
                    Context.CacheProvider.InvalidateLearningCurveDashboard(course.Id);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a set of courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public List<Bdc.Course> CreateCourses(List<Bdc.Course> courses)
        {
            List<Bdc.Course> result = null;

            using (Context.Tracer.DoTrace("CourseActions.CreateCourses(courses)"))
            {
                var cmd = new CreateCourses();
                cmd.Add(courses.Map(c => c.ToCourse()).ToList());

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                result = cmd.Entity.Map(c => c.ToCourse()).ToList();
            }

            return result;
        }

        /// <summary>
        /// Deletes a set of courses.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public List<Bdc.Course> DeleteCourses(List<Bdc.Course> courses)
        {
            List<Bdc.Course> result = null;

            using (Context.Tracer.StartTrace("CourseActions.DeleteCourses()"))
            {
                var cmd = new DeleteCourses();
                cmd.Add(courses.Map(c => c.ToCourse()));

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                result = courses;

                foreach (Bdc.Course course in courses)
                {
                    Context.CacheProvider.InvalidateCourseContent(course);
                }
            }

            return result;
        }

        /// <summary>
        /// Merge the deltas from a derivative course into its immediate master course.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public void MergeCourses(Course course)
        {
            using (Context.Tracer.StartTrace("CourseActions.MergeCourses()"))
            {
                var cmd = new MergeCourses();
                cmd.CourseId = course.Id;
                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                Context.CacheProvider.InvalidateCourseContent(course);
            }
        }
        
        /// <summary>
        /// Copies a set of courses to a new domain.
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <param name="newDomainId">The new domain ID.</param>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public List<Bdc.Course> CopyCourses(List<Bdc.Course> courses, string newDomainId, string method)
        {
            List<Bdc.Course> result = null;

            using (Context.Tracer.DoTrace("CourseActions.CopyCourses(courses, newDomainId={0}, method={1})", newDomainId, method))
            {
                var cmd = new CopyCourses()
                {
                    DomainId = newDomainId,
                    Method = method
                };

                cmd.Add(courses.Map(c => c.ToCourse()).ToList());

                SessionManager.CurrentSession.ExecuteAsAdmin(cmd);
                result = cmd.Courses.Map(c => c.ToCourse()).ToList();
            }

            return result;
        }

        /// <summary>
        /// Course owner to enroll. Default is current user
        /// </summary>
        /// <param name="courses"></param>
        /// <param name="owner"></param>
        public void EnrollCourses(List<Bdc.Course> courses, string owner = null, bool enableStudentView=true)
        {
            owner = owner ?? Context.CurrentUser.Id;

            SetCourseEnrollments(courses, owner, enableStudentView);

            SetCourseGradebookViewFlags(owner, courses);            
        }
        
        /// <summary>
        /// Sets the gradebook view flags for this course and user
        /// </summary>
        /// <param name="courseOwner"></param>
        /// <param name="result"></param>
        private void SetCourseGradebookViewFlags(string courseOwner, List<Course> result)
        {
            var user = new Adc.AgilixUser()
                {
                    Id = courseOwner,
                };
            foreach (var course in result)
            {
                user.GradebookViewFlagsCourse.Add(course.Id, ConfigurationManager.AppSettings["DefaultGradebookViewFlags"]);
                user.GradebookSettingsFlagsCourse.Add(course.Id, ConfigurationManager.AppSettings["DefaultGradebookSettingsFlags"]);
            }
            var cmdUpdateUser = new UpdateUsers();
            cmdUpdateUser.Users.Add(user);
            SessionManager.CurrentSession.ExecuteAsAdmin(cmdUpdateUser);
        }

        public Course GetGenericCourse(string genericCourseId, out string parentDomainId)
        {
            Course result = null;
            parentDomainId = string.Empty;

            if (!string.IsNullOrEmpty(genericCourseId))
            {
                result = GetCourseByCourseId(genericCourseId);
                parentDomainId = this.DomainActions.GetGenericDomainId();
            }
            else
            {
                string parentCourseId = (Context.CourseIsProductCourse) ? Context.Course.Id : Context.Course.ProductCourseId;
                Course ParentCourseInfo = GetCourseByCourseId(parentCourseId);
                List<Course> courseList = new List<Course>();
                courseList.Add(ParentCourseInfo);

                try
                {
                    string genericDomainId = this.DomainActions.GetGenericDomainId();
                    parentDomainId = genericDomainId;
                    var copiedCourse = CopyCourses(courseList, genericDomainId, CopyMethod.Derivative);
                    result = copiedCourse.FirstOrDefault();
                    result.ProductCourseId = parentCourseId;
                    result.CourseSubType = Agilix.DataContracts.CourseSubType.Generic;

                    // TODO: Why we need this
                    result.CourseTimeZone = !string.IsNullOrEmpty(ParentCourseInfo.CourseTimeZone) ? ParentCourseInfo.CourseTimeZone : TimeZoneInfo.Local.StandardName;

                    result.CourseProductName = ParentCourseInfo.Title;
                    result.CourseType = ParentCourseInfo.CourseType.ToString();
                    result.CourseHomePage = ParentCourseInfo.CourseHomePage;
                    result.CourseOwner = ParentCourseInfo.CourseOwner;
                    result.CourseTemplate = ParentCourseInfo.CourseTemplate;
                    result.DashboardCourseId = ParentCourseInfo.DashboardCourseId;

                    result.Theme = ParentCourseInfo.Theme;
                    result.WelcomeMessage = ParentCourseInfo.WelcomeMessage;
                    result.BannerImage = ParentCourseInfo.BannerImage;
                    result.AllowedThemes = ParentCourseInfo.AllowedThemes;

                    result.ActivatedDate = DateTime.Now.ToString();

                    // TODO: Why we need this

                    ParentCourseInfo.GenericCourseId = result.Id;
                    courseList.Add(result);
                    UpdateCourses(courseList);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return result;
        }

        public List<Bdc.ContentItem> LoadContainerData(string courseId, string containerName, string subcontainerId, string toc = "")
        {
            List<Bdc.ContentItem> items = new List<Bdc.ContentItem>();
            using (Context.Tracer.DoTrace("CourseActions.LoadContainerData(containerName={0})", containerName))
            {
                items = ContentActions.GetContainerItems(courseId, containerName, subcontainerId, toc).ToList();
            }
            return items;
        }


        public List<Bdc.ContentItem> LoadContainerData(Bdc.Course course, string containerName, string subcontainerId, string toc = "")
        {
            List<Bdc.ContentItem> items = new List<Bdc.ContentItem>();
            using (Context.Tracer.DoTrace("CourseActions.LoadContainerData(containerName={0})", containerName))
            {
                items = ContentActions.GetContainerItems(Context.CourseId, containerName, subcontainerId, toc).ToList();
            }
            return items;
        }

        /// <summary>
        /// Activates a course.
        /// </summary>
        /// <param name="course">The course.</param>
        public void ActivateCourse(Bdc.Course course)
        {
            using (Context.Tracer.DoTrace("CourseActions.ActivateCourse(course) course.Id={0}", course.Id))
            {
                course.ActivatedDate = DateTime.Now.ToString();
                UpdateCourses(new List<Bdc.Course>() { course });
                Context.CacheProvider.InvalidateCourseContent(course);
            }
        }

        /// <summary>
        /// Deactivates a course.
        /// </summary>
        /// <param name="course">The course.</param>
        public void DeactivateCourse(Bdc.Course course)
        {
            using (Context.Tracer.DoTrace("CourseActions.ActivateCourse(course) course.Id={0}", course.Id))
            {
                course.ActivatedDate = DateTime.MinValue.ToString();
                UpdateCourses(new List<Bdc.Course>() { course });
                Context.CacheProvider.InvalidateCourseContent(course);
            }
        }
        
        /// <summary>
        /// Creates a derived course.
        /// </summary>
        /// <param name="deriveFrom">The course to derive from.</param>
        /// <param name="newDomainId">The domain ID for the new course.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public Bdc.Course CreateDerivedCourse(Bdc.Course deriveFrom, string newDomainId, string copymode = "derivative", string userId = null)
        {
            Bdc.Course result = null;
            
            using (Context.Tracer.DoTrace("CourseActions.CreateDerivedCourse(deriveFrom, newDomainId={0}) deriveFrom.Id={1})", newDomainId, deriveFrom.Id))
            {
                if (userId.IsNullOrEmpty())
                {
                    //set the current Agilix user to a user of the domain we are creating a course in
                    Context.UpdateCurrentUser(newDomainId);
                    //set user id to current user
                    userId = Context.CurrentUser.Id;
                }

                // Now that we know the domain for the user, we can have the context update the 
                // current user to that specific agilix user.
                List<Course> copies = new List<Course>();

                var courses = new List<Bdc.Course>() {deriveFrom};
                if (copymode == "copy")
                {
                    copies = CopyCourses(courses, newDomainId, Bdc.CopyMethod.DerivativeSiblingCopy);
                    EnrollCourses(copies, userId);

                }
                else
                {
                    copies = CopyCourses(courses, newDomainId, Bdc.CopyMethod.Derivative);
                    EnrollCourses(copies, userId);
                }

                if (copies.Count != 1)
                {
                    throw new Exception(string.Format("Got wrong number of copies ({0} when there should be 1)", copies.Count));
                }

                result = GetCourseByCourseId(copies.First().Id);
                result.ActivatedDate = DateTime.MinValue.ToString();

                var productId = deriveFrom.Id;
                if (!string.IsNullOrEmpty(deriveFrom.ProductCourseId))
                {
                    productId = deriveFrom.ProductCourseId;
                }

                result.ProductCourseId = productId;
                result.DashboardCourseId = deriveFrom.DashboardCourseId;
                result.Isbn13 = deriveFrom.Isbn13;

                
                NoteActions.InitializeUser(Context.CurrentUser, result.Id, Bdc.UserType.Instructor);

                Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username);
                Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username, productId);
                Context.CacheProvider.InvalidateCourseList(Context.CurrentUser.Username);
            }

            return result;
        }

        /// <summary>
        /// Store Launch Pada data
        /// </summary>
        /// <param name="preBuiltItems"></param>
        public void CacheLaunchPadData(List<Bdc.ContentItem> preBuiltItems)
        {
            using (Context.Tracer.StartTrace("CourseActions.CacheLaunchPadData(preBuiltItems)"))
            {
                Context.CacheProvider.StoreLaunchPadData(preBuiltItems, Context.CourseId);
            }
        }

        /// <summary>
        /// Fetch Launch pad data
        /// </summary>
        /// <returns></returns>
        public List<Bdc.ContentItem> FetchLaunchPadData()
        {
            using (Context.Tracer.StartTrace("CourseActions.FetchLaunchPadData()"))
            {
                var retval = Context.CacheProvider.FetchLaunchPadData(Context.CourseId);
                if (retval.IsNullOrEmpty())
                    return new List<Bdc.ContentItem>();

                return Context.CacheProvider.FetchLaunchPadData(Context.CourseId).ToList();
            }
        }

        /// <summary>
        /// returns the list of course that matches the search course types
        /// </summary>
        /// <param name="courseType">a list of course type</param>
        /// <returns>list of matching courses</returns>
        public IEnumerable<Course> ListCoursesByCourseTypes(List<CourseType> courseTypes)
        {
            using (Context.Tracer.DoTrace("CourseActions.ListCoursesByCourseTypes(courseType={0})", courseTypes.ToString()))
            {
                string query = courseTypes.IsNullOrEmpty() ? "" : "/meta-bfw_course_type='" + courseTypes.First().ToString() + "'";
                for (int i = 1; i < courseTypes.Count(); i++)
                {
                    query += " OR /meta-bfw_course_type='" + courseTypes[i].ToString() + "'";
                }
                var search = new ListCourses()
                {
                    SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
                    {
                        Query = query,
                        DomainId = "0",
                        Limit = 0
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(search);

                foreach (var course in search.Courses)
                {
                    yield return course.ToCourse();
                }
            }
        }

        /// <summary>
        /// returns the list of course that matches the search query.
        /// </summary>
        /// <param name="query">query</param>
        /// <returns></returns>
        public IEnumerable<Course> ListCoursesMatchQuery(string query)
        {
            using (Context.Tracer.DoTrace("CourseActions.ListCoursesWithQuery(query={0})", query))
            {

                var search = new ListCourses()
                {
                    SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
                    {
                        Query = query,
                        DomainId = "0",
                        Limit = 0
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(search);

                foreach (var course in search.Courses)
                {
                    yield return course.ToCourse();
                }
            }
        }

        /// <summary>
        /// returns the list of course that matches the search course type and academicterm
        /// </summary>
        /// <param name="courseType">course type</param>
        /// <param name="academicTerm">academic term id</param>
        /// <returns>list of matching courses</returns>
        public IEnumerable<Bdc.Course> ListCourses(Bdc.CourseType courseType, string academicTerm, bool includeSubtypeInSearch = false)
        {
            using (Context.Tracer.DoTrace("CourseActions.ListCourses(courseType, academicTerm={0})", academicTerm))
            {
                string courseSubtype = "";
                if (includeSubtypeInSearch == true)
                {
                    courseSubtype = string.Format("AND /meta-course_subtype='{0}'", Context.Course.SubType);
                }
                var search = new GetCourse()
                {
                    SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
                    {
                        Query = string.Format(@"/meta-bfw_academic_term='{0}' AND /meta-bfw_course_type='{1}' {2} ", academicTerm, courseType.ToString(), courseSubtype)
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(search);

                foreach (var course in search.Courses)
                {
                    yield return course.ToCourse();
                }
            }
        }

        public IEnumerable<Bdc.Course> ListCourses(Bdc.CourseType courseType, List<string> academicTerms)
        {
            using (Context.Tracer.DoTrace("CourseActions.ListCourses(courseType, allAcademicTerm)"))
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < academicTerms.Count; i++)
                {
                    var end = i < academicTerms.Count - 1 ? " OR" : "";
                    sb.AppendFormat("/meta-bfw_academic_term='{0}'{1}", academicTerms[i], end);
                }
                var search = new GetCourse()
                {
                    SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
                    {
                        DomainId = Context.Domain.Id,
                        Query = String.Format(@"{0} AND /meta-bfw_course_type='{1}'", sb.ToString(), courseType.ToString())
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(search);

                foreach (var course in search.Courses)
                {
                    yield return course.ToCourse();
                }
            }
        }

        /// <summary>
        /// Gets the list of academic terms
        /// </summary>
        /// <returns></returns>
         public IEnumerable<Bdc.CourseAcademicTerm> ListAcademicTerms()
         {
            var domainid = Context.CurrentUser.DomainId;
            if (domainid.IsNullOrEmpty()) return null;

             var terms = ListAcademicTerms(domainid);
            if (terms == null || !terms.Any())
             {
                domainid = Context.GetRaUserDomains().Distinct().FirstOrDefault().Id;
                terms = ListAcademicTerms(domainid);
            }                

            return terms;
         }


        /// <summary>
        /// Gets the list of academic terms
        /// </summary>
        /// <param name="domainId">domain id</param>
        /// <returns></returns>
        public IEnumerable<Bdc.CourseAcademicTerm> ListAcademicTerms(string domainId)
        {
            var result = new List<CourseAcademicTerm>();

            using (Context.Tracer.DoTrace("CourseActions.ListAcademicTerms(domainid={0})", domainId))
            {
                XDocument doc;
                var resource = ContentActions.GetResource(domainId, "PX/academicterms.xml");

                if (resource != null && resource.ContentType != null)
                {
                    doc = XDocument.Load(resource.GetStream());
                }
                else
                {
                    doc = XDocument.Parse("<academic_terms/>");
                }

                foreach (var export in doc.Root.Elements("academic_term"))
                {
                    result.Add(export.ToCourseAcademicTerm());
                }
            }
            return result;
        }

        /// <summary>
        /// Get the current academic term
        /// </summary>
        /// <returns>current academic term</returns>
        public Bdc.CourseAcademicTerm CurrentAcademicTerm()
        {
            using (Context.Tracer.DoTrace("CourseActions.CurrentAcademicTerm()"))
            {
                var terms = ListAcademicTerms();
                if (terms != null)
                {
                    return terms.FirstOrDefault(e => (e.StartDate < DateTime.Now.GetCourseDateTime(Context) && e.EndDate > DateTime.Now.GetCourseDateTime(Context)));
                }
                else
                {
                    return null;
                }
                
            }
        }

        public IDictionary<string, string> FindDomainFromOnyx(string searchType, string city, string regionCode, string countryCode, string instituteType, string zipCode)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            ILoginServices loginService = new ILoginServices();
            SchoolRespose response = null;

            if (searchType.Equals("City", StringComparison.OrdinalIgnoreCase))
            {
                response = loginService.OnyxSchoolCitySearch(instituteType, countryCode, regionCode, city);
            }

            if (searchType.Equals("Zip", StringComparison.OrdinalIgnoreCase))
            {
                string radiusMile = ConfigurationManager.AppSettings["OnyxSchoolRadiusSearchMiles"].ToString();
                string includeRadius = ConfigurationManager.AppSettings["OnyxSchoolRadiusSearchIncludeRadius"].ToString();
                response = loginService.OnyxSchoolRadiusSearch(radiusMile, zipCode, instituteType, includeRadius);
            }

            if (response.Schools != null && response.Schools.School.Count > 0)
            {
                foreach (var school in response.Schools.School)
                {
                    result.Add(school.CompanyId, school.CompanyName);
                }
            }

            return result;
        }

        /// <summary>
        /// Finds the active sandbox course
        /// </summary>
        /// <param name="courseType">course type</param>
        /// <param name="domainId">domain</param>
        /// <returns>sandbox course</returns>
        public Bdc.Course FindSandboxCourse(Bdc.CourseType courseType, string domainId, string productCourseId)
        {
            using (Context.Tracer.DoTrace("CourseActions.FindSandboxCourse(courseType, domainId={0})", domainId))
            {
                var search = new GetCourse()
                {
                    SearchParameters = new Bfw.Agilix.DataContracts.CourseSearch()
                    {
                        DomainId = domainId,
                        Query = string.Format(@"/meta-bfw_is_sandbox_course='{0}' AND /meta-bfw_course_type='{1}'", true, courseType.ToString())
                    }
                };
                SessionManager.CurrentSession.ExecuteAsAdmin(search);
                var courses = search.Courses.Where(c => c.ParentId == productCourseId);
                if (courses.Count() > 0)
                    return courses.First().ToCourse();
                else
                    return null;

            }
        }


        /// <summary>
        /// Checks if Course is updated (Checks Items, Questions, Resources for updates)
        /// </summary>
        /// <param name="courses">The courses.</param>
        /// <returns></returns>
        public bool CourseUpdateFlag(Course course)
        {
            bool isUpdated = false;
            string CreationDate = course.CreationDate;

            DateTime newDate = DateTime.Parse(CreationDate, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
            CreationDate = newDate.ToString("yyyy-MM-ddTHH:mm:ss");


            using (Context.Tracer.StartTrace("CourseActions.CourseUpdateFlag"))
            {
                string query = string.Format("/creationdate > {0}", CreationDate);

                Batch batch = new Batch();
                batch.Add(new GetItems()
                {
                    SearchParameters = new Adc.ItemSearch
                    {
                        EntityId = course.Id,
                        Query = query
                    }
                });
                batch.Add(new GetQuestions()
                {
                    SearchParameters = new Adc.QuestionSearch
                    {
                        EntityId = course.Id,
                        Query = query
                    }
                });
                batch.Add(new GetResourceList()
                {
                    EntityId = course.Id,
                    Query = query
                });

                SessionManager.CurrentSession.ExecuteAsAdmin(batch);

                if (batch.CommandAs<GetItems>(0).Items.Count > 0)
                    isUpdated = true;
                else if (batch.CommandAs<GetQuestions>(1).Questions.Count() > 0)
                    isUpdated = true;
                else if (batch.CommandAs<GetResourceList>(2).Resources.Count() > 0)
                    isUpdated = true;

            }
            return isUpdated;
        }

        /// <summary>
        /// Gets the academic terms by domain.
        /// </summary>
        /// <param name="domainId">The domain id.</param>
        /// <returns></returns>
        public List<Bdc.CourseAcademicTerm> GetAcademicTermsByDomain(string domainId)
        {
            var academicTerms = new List<Bdc.CourseAcademicTerm>();

            if (domainId != null)
            {
                academicTerms = ListAcademicTerms(domainId).ToList();

                if (academicTerms.Count <= 0)
                {
                    //ContentActions.CopyResourceToAnotherDomain(domainId, "PX/academicterms.xml", "PX/academicterms.xml", ConfigurationManager.AppSettings["BfwUsersDomainId"]);
                    academicTerms = ListAcademicTerms(domainId).ToList();
                }
            }

            return academicTerms;
        }

        #endregion
    }
}
