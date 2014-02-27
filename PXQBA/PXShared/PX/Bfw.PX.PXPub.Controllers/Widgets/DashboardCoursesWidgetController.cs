using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provided all actions required to implement the account widget which allows
    /// users to login, log out, and view their profile information
    /// </summary>
    [PerfTraceFilter]
    public class DashboardCoursesWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// Access to the current business context information
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }
        /// <summary>
        /// Access to an IUserActions implementation
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IDashboardActions2 DashboardActions { get; set; }

        /// <summary>
        /// The Course Actions.
        /// </summary>
        /// <value>
        /// The Course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// The Domain Actions.
        /// </summary>
        /// <value>
        /// The Domain actions.
        /// </value>
        protected BizSC.IDomainActions DomainActions { get; set; }

        /// <summary>
        /// The Enrollment Actions for getting domain list
        /// </summary>
        /// <value>
        /// The Enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// The Content Actions for getting domain list
        /// </summary>
        /// <value>
        /// The Content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Constructs a default AccountWidgetController. Depends on a business context
        /// and user actions implementation
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userActions">The user actions.</param>
        public DashboardCoursesWidgetController(BizSC.IBusinessContext context, BizSC.IUserActions userActions,
            BizSC.ICourseActions courseActions, BizSC.IDashboardActions2 dashboardActions, BizSC.IDomainActions domainActions,
            BizSC.IEnrollmentActions enrollmentActions, BizSC.IContentActions contentActions)
        {
            Context = context;
            UserActions = userActions;
            CourseActions = courseActions;
            DashboardActions = dashboardActions;
            DomainActions = domainActions;
            EnrollmentActions = enrollmentActions;
            ContentActions = contentActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Shows the login status of the currently logged in user. If the user is
        /// anonymous then they are considered to be not authenticated.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            BizDC.DashboardData bizModel = new BizDC.DashboardData();
            Models.DashboardData model = new Models.DashboardData();

            model.AllowCreateAnotherBranchColumn = GetBooleanPropertyValue("allow_createanotherbranch_column", widget);
            model.AllowCourseTitleColumn = GetBooleanPropertyValue("allow_title_column", widget);
            model.AllowInstructorNameColumn = GetBooleanPropertyValue("allow_instructorname_column", widget);
            model.AllowDomainNameColumn = GetBooleanPropertyValue("allow_allowdomainname_column", widget);
            model.AllowAcademicTermColumn = GetBooleanPropertyValue("academicterm", widget);
            model.AllowCourseIdColumn = GetBooleanPropertyValue("allow_courseid_column", widget);
            model.AllowDeleteButtonColumn = GetBooleanPropertyValue("allow_deletebutton_column", widget);
            model.AllowStatusColumn = GetBooleanPropertyValue("allow_status_column", widget);
            model.AllowEnrollmentCountColumn = GetBooleanPropertyValue("allow_enrollmentcount_column", widget);
            model.AllowActivateButtonColumn = GetBooleanPropertyValue("allow_activatebutton_column", widget);
            model.AllowCourseOpenInNewWindow = GetBooleanPropertyValue("allow_courseopeninnewwindow_column", widget);
            model.AllowViewingRoster = GetBooleanPropertyValue("allow_view_roster", widget);
            model.AllowEditingCourseInformation = GetBooleanPropertyValue("allow_edit_course", widget);

            bizModel = DashboardActions.GetDashboardData(model.AllowCreateAnotherBranchColumn);
            model.LaunchPadMode = model.AllowCreateAnotherBranchColumn;

            widget.SchoolList = GetSchoolList();
            model.SchoolList = widget.SchoolList;

            ViewData["TimeZones"] = GetTimeZones();

            var courseUrl = Url.RouteUrl("CourseSectionHome", new { courseid = -1 }, Request.Url.Scheme) + "/";
            ViewData["CourseDomainUrl"] = courseUrl.Replace("/-1/", "/");

            if (!bizModel.InstructorCourses.IsNullOrEmpty())
            {
                model.InstructorCourses = bizModel.InstructorCourses.Map(i => i.ToDashboardItem()).ToList();
            }

            if (!bizModel.ProgramManagerTemplates.IsNullOrEmpty())
            {
                model.ProgramManagerTemplates = bizModel.ProgramManagerTemplates.Map(i => i.ToDashboardItem()).ToList();
            }

            if (!bizModel.PublisherTemplates.IsNullOrEmpty())
            {
                model.PublisherTemplates = bizModel.PublisherTemplates.Map(i => i.ToDashboardItem()).ToList();
            }

            if (model.InstructorCourses.Count > 0 && model.InstructorCourses.Find(i => i.Level == "1") != null)
            {
                model.IsBranchCreated = true;
            }

            IEnumerable<BizDC.Domain> userDomains = Context.GetRaUserDomains().Distinct();
            model.isMultipleDomains = !userDomains.IsNullOrEmpty() && (userDomains.Count() > 1);

            model.PossibleAcademicTerms = CourseActions.ListAcademicTerms().Map(i => i.ToAcademicTerm()).ToList();

            if (Context.CurrentUser.WebRights != null && Context.CurrentUser.WebRights.QuestionBank != null)
            {
                model.QuestionAdminLink = Context.CurrentUser.WebRights.QuestionBank.ShowQuestionBankManager;
            }

            if (Context.CurrentUser.WebRights != null && Context.CurrentUser.WebRights.AdminTool != null)
            {
                model.SandBoxLink = Context.CurrentUser.WebRights.AdminTool.AllowEditSandboxCourse;
            }

            model.DashboardId = Context.CourseId;
            ViewData.Model = model;

            return View();
        }

        /// <summary>
        /// Shows all data related to the currently logged in user
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }

        #endregion

        #region Controller Methods

        public ActionResult CreateBranchConfirmation()
        {
            return View();
        }

        public ActionResult CreateCourse()
        {
            Models.DashboardItem model = new Models.DashboardItem();
            model.Course = Context.Course.ToCourse();
            model.SchoolList = GetSchoolList();
            model.Course.PossibleAcademicTerms = CourseActions.ListAcademicTerms(Context.Domain.Id).Map(i => i.ToAcademicTerm()).ToList();

            ViewData.Model = model;
            return View();
        }

        public ActionResult CreateCourseOption()
        {
            return View();
        }

        public ActionResult DeleteDashboardCourse(string courseId)
        {
            return View();
        }

        /// <summary>
        /// Shows the roster.
        /// </summary>
        /// <param name="courseIdForRoster">The course id for roster.</param>
        /// <returns></returns>
        public ActionResult ViewRoster(string courseIdForRoster)
        {
            List<Models.Student> studentsEnrolled = new List<Models.Student>();

            if (!string.IsNullOrEmpty(courseIdForRoster))
            {
                studentsEnrolled = EnrollmentActions.GetEntityEnrollments(courseIdForRoster, UserType.Student).Map(e => e.ToStudent()).ToList();
            }

            return View(studentsEnrolled);
        }

        #endregion

        #region Controller Json Post Methods

        [HttpPost]
        public JsonResult ActivateDashboardCourse(string courseId)
        {
            var result = new Dictionary<string, string>();

            try
            {
                var course = CourseActions.GetCourseByCourseId(courseId: courseId);                
                CourseActions.ActivateCourse(course);

                Context.CacheProvider.InvalidateCourseContent(course);

                ViewData["School"] = course.Domain.Name;
                ViewData["InstructorEmail"] = Context.CurrentUser.Email;

                result.Add("courseid", course.Id);
                result.Add("status", "True");
                result.Add("activationDate", course.ActivatedDate);
                result.Add("url", Url.RouteUrl("CourseSectionHome", new { courseid = course.Id }, Request.Url.Scheme));
                result.Add("school", course.Domain.Name);
                result.Add("instructoremail", Context.CurrentUser.Email);

                InvalidateDashboardInformation();

                try
                {
                    bool sendActivationEmail = false;
                    bool.TryParse(ConfigurationManager.AppSettings["SendCourseActivationEmail"] != null ? ConfigurationManager.AppSettings["SendCourseActivationEmail"].ToString() : "false", out sendActivationEmail);

                    var domainUser = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, course.Domain.Id);
                    string instructorUserId = (domainUser == null || string.IsNullOrEmpty(domainUser.Id)) ? Context.CurrentUser.Id : domainUser.Id;
                    
                    if (course.Domain.Name == null)
                    {
                        ViewData["School"] = domainUser.DomainName;
                    }

                    if (sendActivationEmail)
                    {
                        string emailText = string.Empty;
                        emailText = Helpers.PartialViewRenderer.ToString(this.ControllerContext, @"~/Views/Course/StudentEmailTemplate.ascx", course.ToCourse(), this.ViewData, this.TempData);
                        CourseActions.SendInstructorEmail(emailText, course.Title, courseId, course.ProductCourseId, instructorUserId);
                    }
                }
                catch { }
            }
            catch 
            { 
                result.Add("status", "False"); 
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult CreateCourseFromDashboard(string selectedDomainId, string courseTitle, string courseNumber, string sectionNumber, string instructorName, string school, string academicTerm, string timezone, string parentCourseId, string creationMode, bool lmsIdRequired)
        {
            var result = new Dictionary<string, string>();
            result.Add("status", "False");
            var fail = Json(result); 

            if (creationMode == "dropdown" && Context.Product.IsDashboardActive == true)
            {
                parentCourseId = Context.ProductCourseId;
            }

            string mainParenDomain = ConfigurationManager.AppSettings["MainParentDomain"].ToString();
            string userspacePrefix = ConfigurationManager.AppSettings["NewDomainUserspacePrefix"].ToString();
            string userspaceSuffix = ConfigurationManager.AppSettings["NewDomainUserspaceSuffix"].ToString();

            int domainId;
            var domain = DomainActions.GetOrCreateDomain(school, mainParenDomain, selectedDomainId, userspacePrefix: userspacePrefix, userspaceSuffix: userspaceSuffix, copyResourcesForNewDomain: true);

            if (domain != null)
            {
                Int32.TryParse(domain.Id, out domainId);
            }
            else
            {                
                return fail; 
            }

            var productId = !string.IsNullOrEmpty(Context.EntityId) ? Context.EntityId : Context.ProductCourseId;
            BizDC.UserInfo userToJoin = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, domainId.ToString());

            var content = BaseCourse(domainId.ToString());
            BizDC.Course course = null;

            try
            {
                course = CreateDefaultCourse(content, courseTitle, courseNumber, sectionNumber, instructorName, academicTerm, timezone, parentCourseId, creationMode, lmsIdRequired);
            }
            catch (Exception e)
            {
                return fail;
            }

            InvalidateDashboardInformation();

            if (creationMode == "dropdown" && Context.Product.IsDashboardActive == true)
            {
                return Json(Url.RouteUrl("CourseSectionHome", new { courseId = course.Id }, Request.Url.Scheme));
            }

            return Json(course);
        }

        [HttpPost]
        public JsonResult DeactivateDashboardCourse(string courseId)
        {
            var result = new Dictionary<string, string>();

            try
            {
                var course = CourseActions.GetCourseByCourseId(courseId: courseId);
                
                CourseActions.DeactivateCourse(course);
                
                Context.CacheProvider.InvalidateCourseContent(new Course() { Id = courseId });
                
                result.Add("status", "True");
                result.Add("activationDate", course.ActivatedDate);
                
                InvalidateDashboardInformation();
            }
            catch
            {
                if (result.ContainsKey("status"))
                    result["status"] = "False";
                else
                    result.Add("status", "False"); 
            }

            return Json(result);
        }

        public bool DeleteCourse(string coursesToDelete)
        {
            var result = false;

            if (!coursesToDelete.IsNullOrEmpty())
            {
                List<string> listOfIds = new List<string>();

                if (coursesToDelete.Contains(","))
                {
                    listOfIds = coursesToDelete.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    listOfIds.Add(coursesToDelete);
                }

                if (listOfIds.Count > 0)
                {
                    foreach (string courseId in listOfIds)
                    {
                        result = DashboardActions.DeleteCourse(courseId);
                        //Todo: Need to see if lmsid changes required
                        var courseEnrollmentRefIds =
                            EnrollmentActions.GetEntityEnrollmentsAsAdmin(courseId)
                                .Where(e => e.User != null && !e.User.ReferenceId.IsNullOrEmpty())
                                .Select(e => e.User.ReferenceId)
                                .Distinct();
                        foreach (var userRefId in courseEnrollmentRefIds)
                        {
                            var productCourseId = Context.CourseIsProductCourse
                                ? Context.CourseId
                                : Context.ProductCourseId;
                            Context.CacheProvider.InvalidateUserEnrollmentList(userRefId, productCourseId); //***?LMS - we need to invalidate these based on username instead
                        }

                    }

                    InvalidateDashboardInformation();
                }
            }

            return result;
        }

        [HttpPost]
        public JsonResult EditDashboardCourse(string courseId, string courseTitle, string courseNumber,
                                                string sectionNumber, string instructorName, string schoolName,
                                                string academicTerm, string courseTimeZone, bool lmsIdRequired)
        {
            Course updatedCourse = new Course();

            try
            {
                var course = CourseActions.GetCourseByCourseId(courseId: courseId);
                course.LmsIdRequired = lmsIdRequired;
                course.Title = courseTitle;
                course.CourseNumber = courseNumber;
                course.SectionNumber = sectionNumber;
                course.InstructorName = instructorName;

                if (!String.IsNullOrWhiteSpace(schoolName))
                {
                    var domain = DomainActions.GetDomain(schoolName);
                    if (null != course.Domain && null != domain)
                    {
                        if (course.Domain.Id != domain.Id)
                        {
                            course.Domain = domain;
                            UpdateCourseEnrollments(course);
                        }
                        else
                        {
                            course.Domain.Name = domain.Name;
                        }
                    }
                }

                course.AcademicTerm = academicTerm;
                course.CourseTimeZone = courseTimeZone;

                updatedCourse = CourseActions.UpdateCourse(course);

                InvalidateDashboardInformation();
            }
            catch { }

            return Json(updatedCourse);
        }

        [HttpGet]
        public JsonResult GetAcademicTermsByDomain()
        {
            string domainName = Request["domainname"].ToString();
            BizDC.Domain domain = new BizDC.Domain();
            List<BizDC.CourseAcademicTerm> academicTerms = new List<BizDC.CourseAcademicTerm>();

            if (domainName != null)
            {
                domain = DomainActions.GetDomain(domainName);
            }
            if (domain == null)
            {
                domain = DomainActions.GetDomainById(ConfigurationManager.AppSettings["BfwUsersDomainId"]);
            }

            if (domain != null)
            {
                academicTerms = CourseActions.GetAcademicTermsByDomain(domain.Id);
            }

            return Json(academicTerms.ToArray(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCurrentDateTime(string dashboardCourseId)
        {
            var timezoneDateTime = String.Empty;

            if (String.IsNullOrWhiteSpace(dashboardCourseId))
            {
                timezoneDateTime = Bfw.PX.PXPub.Models.ExtensionMethods.GetCourseDateTime(DateTime.Now).ToString("MM/dd/yyyy");
            }
            else
            {
                var course = CourseActions.GetCourseByCourseId(dashboardCourseId);
                if (course != null)
                {
                    timezoneDateTime = course.UtcRelativeAdjust(DateTime.Now).ToString("MM/dd/yyyy");
                }
            }

            return Json(timezoneDateTime, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// SEts the defaults on the course object that is then passed to the course creation method
        /// </summary>
        /// <param name="domainId">the id of the domain the create the course in</param>
        /// <returns>Course object</returns>
        private Models.Course BaseCourse(string domainId)
        {
            var productId = string.IsNullOrEmpty(Context.ProductCourseId) ? Context.EntityId : Context.ProductCourseId;
            Models.Course course = new Models.Course();

            course.Title = String.Format("DEMO-{0}", course.UtcRelativeAdjust(DateTime.Now).ToShortDateString());
            course.CurrentUserName = Context.CurrentUser.FirstName + " " + Context.CurrentUser.LastName;
            course.CourseUserName = course.CurrentUserName;
            course.ProductName = (Context.Product == null) ? "[Digital Product Name]" : Context.Product.Title;
            course.CourseProductName = String.Format("[CourseNumber]-[SectionNumber] [Title],{0}", course.ProductName);
            course.CourseType = (Context.Product == null) ? course.CourseType : (Models.CourseType)Enum.Parse(typeof(Models.CourseType), Context.Product.CourseType, true); ;
            // course.CourseProductName = "[Content Title][Digital Product Name]";
            course.SelectedDerivativeDomain = domainId.ToString();
            course.CourseTimeZone = course.CourseTimeZone;
            if (course.DashboardSettings != null)
            {
                course.DashboardSettings.DashboardHomePageStart = Context.Course.DashboardSettings.DashboardHomePageStart;
                course.DashboardSettings.IsInstructorDashboardOn = false;
                course.DashboardSettings.IsProgramDashboardOn = false;
            }
            course.CourseSubType = "regular";
            course.DerivedCourseId = Context.Course.Id;
            return course;

        }

        /// <summary>
        /// Creates the default course.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="courseTitle">The course title.</param>
        /// <param name="courseNumber">The course number.</param>
        /// <param name="sectionNumber">The section number.</param>
        /// <param name="instructorName">Name of the instructor.</param>
        /// <param name="academicTerm">The academic term.</param>
        /// <param name="timezone">The timezone.</param>
        /// <param name="parentCourseId">The parent course id.</param>
        /// <param name="creationMode">The creation mode.</param>
        /// <returns></returns>
        private BizDC.Course CreateDefaultCourse(Models.Course content, string courseTitle, string courseNumber, string sectionNumber, string instructorName, string academicTerm, string timezone, string parentCourseId, string creationMode, bool lmsIdRequired)
        {
            var productId = string.IsNullOrEmpty(Context.ProductCourseId) ? Context.EntityId : Context.ProductCourseId;
            BizDC.Course course = new BizDC.Course();
            string courseSubType = Context.Course.SubType;

            if (courseSubType == null)
            {
                var routeData = System.Web.Routing.RouteTable.Routes.GetRouteData(new System.Web.HttpContextWrapper(System.Web.HttpContext.Current));
                courseSubType = routeData.Values["course"] as String;
            }

            var copymode = string.Empty;

            if (creationMode == "new")
            {
                parentCourseId = Context.Course.Id;
            }
            else if (creationMode == "copy")
            {
                copymode = "copy";
            }

            BizDC.Course parentCourse = CourseActions.GetCourseByCourseId(parentCourseId);
            course = CourseActions.CreateDerivedCourse(parentCourse, content.SelectedDerivativeDomain, copymode);

            course.LmsIdRequired = lmsIdRequired;
            course.Title = courseTitle;
            course.CourseNumber = courseNumber;
            course.SectionNumber = sectionNumber;
            course.AcademicTerm = academicTerm;
            course.CourseTimeZone = timezone;
            course.CourseProductName = content.CourseProductName;
            course.InstructorName = instructorName;
            course.CourseSubType = "regular";
            course.CourseType = content.CourseType.ToString();
            course.CourseHomePage = content.CourseHomePage;
            course.CourseOwner = Context.CurrentUser.Id;
            course.CourseTemplate = content.CourseTemplate;
            course.DashboardCourseId = content.DashboardCourseId;

            course.Theme = content.Theme;

            if (copymode == "copy")
            {
                course.DerivedCourseId = Context.Course.Id;
            }
            else
            {
                course.ParentId = parentCourseId;
                course.DerivedCourseId = parentCourseId;
            }

            course.WelcomeMessage = content.WelcomeMessage;
            course.BannerImage = content.BannerImage;
            course.AllowedThemes = content.AllowedThemes;
            course.Syllabus = null;

            course = CourseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { course }).First();

            var domains = GetDomainList();

            if (domains.Count > 0)
            {
                course.Domain.Name = domains.Find(d => d.Id == course.Domain.Id).Name;
            }

            return course;
        }

        /// <summary>
        /// Gets the school list.
        /// </summary>
        /// <returns></returns>
        private string GetSchoolList()
        {
            var result = String.Empty;
            var alldomain = GetDomainList();

            if (alldomain.Count > 0)
            {
                if (alldomain != null && alldomain.Count() > 0)
                {
                    var domains = from s in alldomain
                                 select string.Concat("{ \"label\": \"", System.Web.HttpUtility.HtmlEncode(s.Name), "\", \"value\": \"", s.Id, "\" }");

                    StringBuilder sb = new StringBuilder();
                    sb.Append("[");

                    int i = 0;

                    foreach (var school in domains)
                    {
                        if (i++ != 0) { sb.Append(","); }
                        sb.Append(school);
                    }

                    sb.Append("]");

                    result = sb.ToString();
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a list of all domains based on the parent one
        /// </summary>
        /// <returns></returns>
        private List<Domain> GetDomainList()
        {
            List<Domain> result = new List<Domain>();
            var parentDomain = this.DomainActions.GetDomain(ConfigurationManager.AppSettings["MainParentDomain"]);

            if (parentDomain != null)
            {
                result = this.EnrollmentActions.GetEnrollableDomains(Context.CurrentUser.Username, parentDomain.Id, ConfigurationManager.AppSettings["institution_force_list"].Split(',')).ToList();
            }

            return result;
        }

        /// <summary>
        /// Sort the time zones by putting the time zones with "US" on top.
        /// </summary>
        /// <returns>Time zones</returns>
        private List<TimeZoneInfo> GetTimeZones()
        {
            var timezones = TimeZoneInfo.GetSystemTimeZones();
            var timezonesUs = timezones.Filter(z => z.DisplayName.Contains("(US")).OrderByDescending(z => z.BaseUtcOffset);
            var timezonesNonUs = timezones.Filter(z => !z.DisplayName.Contains("(US"));
            return timezonesUs.Concat(timezonesNonUs).ToList();
        }

        /// <summary>
        /// Gets the boolean property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="widget">The widget.</param>
        /// <returns></returns>
        private bool GetBooleanPropertyValue(string propertyName, Models.Widget widget)
        {
            bool outValue = false;
            if (widget.Properties.ContainsKey(propertyName) && widget.Properties[propertyName].Value.ToString().ToLowerInvariant() == "true")
            {

                bool.TryParse(widget.Properties[propertyName].Value.ToString().ToLowerInvariant(), out outValue);
            }
            return outValue;
        }

        /// <summary>
        /// Invalidates the dashboard information. Clear all the cached information about enrollment and course list.
        /// </summary>
        private void InvalidateDashboardInformation()
        {
            var courseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
            Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username, courseId);
            Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username);
            Context.CacheProvider.InvalidateCourseList(Context.CurrentUser.Username);
        }
        /// <summary>
        /// When course's domain changed, we need to update the course enrollments' user ids.
        /// </summary>
        /// <param name="course"></param>
        private void UpdateCourseEnrollments(BizDC.Course course)
        {
            if (course.Domain == null)
                return;

            var enrollments = EnrollmentActions.GetAllEntityEnrollmentsAsAdmin(course.Id);
            if (enrollments.IsNullOrEmpty())
                return;
            var updatedEnrollments = new List<BizDC.Enrollment>();
            foreach (BizDC.Enrollment enrollment in enrollments)
            {
                //Enrollment with user id '7' must not change.
                if (enrollment.User.Id == UserActions.PxMigrationUserId)
                    continue;
                var correctedUserInfo = UserActions.GetUserByReferenceAndDomainId(enrollment.User.Username, course.Domain.Id);

                if (correctedUserInfo != null)
                {
                    enrollment.User = correctedUserInfo;
                    updatedEnrollments.Add(enrollment);
                }
            }
            Context.CacheProvider.InvalidateUsersByReference(Context.CurrentUser.Username);
            EnrollmentActions.UpdateEnrollments(updatedEnrollments);
        }
        #endregion
    }
}
