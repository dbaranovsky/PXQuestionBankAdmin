using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System;
using Bfw.PX.PXPub.Components;
using System.Configuration;
using Bfw.PX.Biz.Direct.Services;
using Bfw.Common.Exceptions;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class CourseController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the course actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Gets or sets the domain actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.IDomainActions DomainActions { get; set; }

        /// <summary>
        /// Gets or sets the task actions.
        /// </summary>
        /// <value>
        /// The task actions.
        /// </value>
        protected BizSC.ITaskActions TaskActions { get; set; }

        /// <summary>
        /// Gets or sets the auto enrollment actions.
        /// </summary>
        /// <value>
        /// The auto enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the user actions.
        /// </summary>
        /// <value>
        /// The auto enrollment actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }

        /// <summary>
        /// Gets or sets the auto enrollment actions.
        /// </summary>
        /// <value>
        /// The auto enrollment actions.
        /// </value>
        protected BizSC.IAutoEnrollmentActions AutoEnrollmentActions { get; set; }

        /// <summary>
        /// A const for that holds the instructor persmission flags.
        /// </summary>
        private readonly string INSTRUCTOR_FLAGS = System.Configuration.ConfigurationManager.AppSettings["InstructorPermissionFlags"];
        /// <summary>
        /// Constructs a default AccountWidgetController. Depends on a business context
        /// and user actions implementation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="courseActions">The course actions.</param>
        /// <param name="taskActions">The task actions.</param>
        /// <param name="autoEnrollmentActions">The auto enrollment actions.</param>
        public CourseController(BizSC.IBusinessContext context, BizSC.ICourseActions courseActions, BizSC.ITaskActions taskActions, BizSC.IAutoEnrollmentActions autoEnrollmentActions, BizSC.IEnrollmentActions enrollmentActions, BizSC.IDomainActions domainActions, BizSC.IUserActions userActions)
        {
            Context = context;
            CourseActions = courseActions;
            TaskActions = taskActions;
            AutoEnrollmentActions = autoEnrollmentActions;            
            this.EnrollmentActions = enrollmentActions;
            this.DomainActions = domainActions;
            this.UserActions = userActions;
        }

        /// <summary>
        /// Displays a login for to the user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Find()
        {
            return View();
        }

        /// <summary>
        /// Dislays the Same View.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Sample()
        {
            return View();
        }

        /// <summary>
        /// Displays the Auto Enroll View.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AutoEnroll()
        {
            return Content("AwesomeBackgrounds");
        }

        /// <summary>
        /// Displays the Edit course View.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveCourse(Course content, string behavior)
        {
            List<BizDC.Course> courses = new List<Bfw.PX.Biz.DataContracts.Course>();
            courses.Add(content.ToCourse());
            CourseActions.UpdateCourses(courses);
            return View("Edit", content);
        }

        [HttpPost]
        public ActionResult UpdateCourse(Course content, string behavoir)
        {
            List<BizDC.Course> courses = new List<Bfw.PX.Biz.DataContracts.Course>();
            string courseid = "";
            if (behavoir == "edit"){
                courseid = content.Id;
                if (courseid.EndsWith("__"))
                    courseid = courseid.Substring(0, courseid.Length - 2);
            }
            else{
                courseid = RouteData.Values["courseid"].ToString();
            }
            BizDC.Course course = new BizDC.Course();
            string selectedDomainId = (Request.Form["PossibleDomains"] != null) ? Request.Form["PossibleDomains"].ToString() : string.Empty;
            string selectedDomainName = (Request.Form["SelectedDerivativeDomain"] != null) ? Request.Form["SelectedDerivativeDomain"].ToString() : string.Empty;
            BizDC.Domain domain = null;
            int domainId = 0;

            if (!string.IsNullOrEmpty(selectedDomainId))
            {
                int.TryParse(selectedDomainId, out domainId);
                string mainParenDomain = ConfigurationManager.AppSettings["MainParentDomain"].ToString();
                string userspacePrefix = ConfigurationManager.AppSettings["NewDomainUserspacePrefix"].ToString();
                string userspaceSuffix = ConfigurationManager.AppSettings["NewDomainUserspaceSuffix"].ToString();

                if (ConfigurationManager.AppSettings["NewDomainUserspaceSuffix"].ToString().Equals("Id",StringComparison.OrdinalIgnoreCase) && domainId > 0)
                {
                    domain = this.DomainActions.GetDomainById(domainId.ToString());
                    if (domain == null)
                    {
                        domain = this.DomainActions.GetOrCreateDomain(selectedDomainName, mainParenDomain, selectedDomainId, userspacePrefix, userspaceSuffix);
                    }
                }
                else
                {
                    domain = this.DomainActions.GetOrCreateDomain(selectedDomainName, mainParenDomain, selectedDomainId, userspacePrefix, userspaceSuffix);
                }

                if (domain != null)
                {
                    int.TryParse(domain.Id, out domainId);
                }
                else
                {
                    domainId = 0;
                }
            }

            if (courseid != "-1")
            {
                course = CourseActions.GetCourseByCourseId(courseid);
                
                course.Title = Context.Course.CourseType == CourseType.LearningCurve.ToString()? content.Title.Trim() : String.Format("{0} {1} {2}", content.CourseNumber, content.SectionNumber, content.Title).Trim();
                course.CourseProductName = content.Title;
                course.InstructorName= content.CourseUserName;
                course.SectionNumber = content.SectionNumber;
                course.CourseNumber = content.CourseNumber;
                course.CourseTimeZone = content.CourseTimeZone;
                course.AcademicTerm = content.AcademicTerm;
                if (behavoir != "edit")
                {
                    course.ActivatedDate = content.UtcRelativeAdjust(DateTime.Now).ToString();
                }
                if (null != course.Domain && null != domain && course.Domain.Id != domain.Id)
                {
                    course.Domain = domain;
                    UpdateCourseEnrollments(course);
                }
            }               

            courses.Add(course);
            CourseActions.UpdateCourses(courses);



            //BizDC.Enrollment originalEnrollment = EnrollmentActions.GetEnrollment(Context.CurrentUser.Id, courseid);


            if (domainId > 0)
            {
                BizDC.UserInfo userToEnroll = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, domainId.ToString());
                EnrollmentActions.SwitchUserEntityEnrollment(Context.CurrentUser.Id, courseid, userToEnroll);                
                
                //BizDC.Enrollment enrollment = EnrollmentActions.GetEnrollment(userToEnroll.Id, courseid);
                //if (enrollment == null)
                //{
                //    CourseActions.SetCourseEnrollments(courses, userToEnroll.Id);
                //    enrollment = EnrollmentActions.GetEnrollment(userToEnroll.Id, courseid);
                //    Context.CacheProvider.InvalidateEnrollment(enrollment);
                //    Context.CacheProvider.StoreEnrollment(enrollment);
                //}
            }
            if (behavoir == "edit")
            {
                ViewData["edit"] = new Object();
            }
            return View("ShowActivateCourse", content);
        }

        /// <summary>
        /// Create a course object derived from a parent course.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult Create(Course content, string behavior)
        {
            if (ModelState.IsValid)
            {
                // This is a dev hack for when SSO doesn't set the productId.
                var productId = string.IsNullOrEmpty(Context.ProductCourseId) ? Context.EntityId : Context.ProductCourseId;

                var course = CourseActions.CreateDerivedCourse(Context.Course, content.SelectedDerivativeDomain);
                course.Title = content.Title;
                course.CourseProductName = content.Title;
                course.InstructorName = content.CourseUserName;
                course.CourseTimeZone = content.CourseTimeZone;

                course.CourseType = content.CourseType.ToString();
                course.CourseHomePage = content.CourseHomePage;
                course.CourseOwner = content.CourseOwner;
                course.CourseTemplate = content.CourseTemplate;
                course.DashboardCourseId = content.DashboardCourseId;

                course.Theme = content.Theme;
                course.WelcomeMessage = content.WelcomeMessage;
                course.BannerImage = content.BannerImage;
                course.AllowedThemes = content.AllowedThemes;

                var currentAcademicTerm = CourseActions.CurrentAcademicTerm();
                if (currentAcademicTerm != null) { course.AcademicTerm = currentAcademicTerm.Id; }

                course = CourseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { course }).First();

                var model = course.ToCourse();
                return View("CreateCourseStep2", model);
            }
            else
            {
                // There was an error return the save view.
                content.PossibleDerivativeDomains = Context.GetRaUserDomains().Map(d => d.ToDomain());
                return View("CreateCourse", content);
            }
        }

              
        public ActionResult ShowCreateDashboard()
        {
            Course course = new Course();
            course.CurrentUserName = Context.CurrentUser.FirstName + " " + Context.CurrentUser.LastName;
            course.CourseUserName = course.CurrentUserName;
            course.CourseTimeZone = course.CourseTimeZone;
            course.ProductName = (Context.Product == null) ? "[Digital Product Name]" : Context.Product.Title;
            course.CourseProductName = "[Content Title][Digital Product Name]";
            course.PossibleDerivativeDomains = Context.GetRaUserDomains().Map(d => d.ToDomain());
            course.CourseHomePage = "PX_HOME_EPORTFOLIODASHBOARD";
            return View("CreateDashboard", course);
        }
        
        /// <summary>
        /// Show the Create course screen.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShowCreateCourse()
        {

            //PLATX-9009 - check to see if the user is in multiple domains
            IEnumerable<BizDC.Domain> userDomains = Context.GetRaUserDomains().Distinct();
            if (userDomains.Count() > 1)
            {
                return RedirectToAction("DomainSelection", "DomainSelectionDialog", new { callbackFunction = "Course/CreateCourseInDomain" });
            }
            string defaultDomain = System.Configuration.ConfigurationManager.AppSettings["GenericCourseDomain"];
            
            String selectedDomain = "";
            if(userDomains.Any())
            {
                selectedDomain = userDomains.First().Id.ToString();
            }
            else
            {
                throw new DomainNotFoundException("Your account has not been created properly. Please contact Technical Support for further assistance.");
            }
            
            var model = CreateDefaultCourse(BaseCourse(selectedDomain)).ToCourse();

            return RedirectToRoute("CourseCreationRedirect", new { courseid = model.Id });
        }

        [HttpPost]
        public ActionResult CreateCourseInDomain(int selectedDomainId)
        {

            var productId = !string.IsNullOrEmpty(Context.EntityId) ? Context.EntityId : Context.ProductCourseId;

            var content = BaseCourse(selectedDomainId.ToString());
            BizDC.Course course = null;
            try
            {
                course = CreateDefaultCourse(content);
                var model = course.ToCourse();
            }
            catch (Exception e)
            {
                throw e;
            }
            
            return RedirectToRoute("CourseCreationRedirect", new { courseid = course.Id });
        }

       
        /// <summary>
        /// Diplay the Edit course View.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit()
        {
            string id = RouteData.Values["id"].ToString();
            Course course = new Course();

            if (id != "-1")
            {
                course = CourseActions.GetCourseByCourseId(id).ToCourse();
            }

            return View(course);
        }

        /// <summary>
        /// Show the Edit of the generic Model course screen.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShowCreateNameCourse(string id = null, bool hasEnrollment = false, bool activateCourse = true)
        {
            string courseid = id == null ? RouteData.Values["courseid"].ToString() : id;

            Course course = new Course();
            if (courseid != "-1")
            {
                course = CourseActions.GetCourseByCourseId(courseid).ToCourse();
                string prodname = course.CourseProductName; //[CourseNumber]-[SectionNumber] [Title],{0}
                prodname = prodname.Substring(prodname.LastIndexOf(']') + 2);
                course.ProductName = prodname;

                var forceList = ConfigurationManager.AppSettings["institution_force_list"].Split(',');
                var domains = EnrollmentActions.GetEnrollableDomains(Context.CurrentUser.Username, ConfigurationManager.AppSettings["institution_domain_id"], forceList);
                var allDomains = new List<Domain>();
                if (id != null && hasEnrollment && !course.DerivativeDomainId.IsNullOrEmpty())
                {
                    var selected = domains.SingleOrDefault(d => d.Id == course.DerivativeDomainId);
                    if (selected != null)
                    {
                        allDomains.Add(selected.ToDomain());
                    }
                }
                else
                {
                    Domain blank = new Domain { Id = "", Name = "--Select your school--" };
                    allDomains.Add(blank);
                    foreach (var d in domains)
                    {
                        allDomains.Add(d.ToDomain());
                    }
                }
                course.PossibleDomains = allDomains;
            }

            if (Context.CurrentUser.DomainId.IsNullOrEmpty())
            {
                IEnumerable<BizDC.Domain> userDomains = Context.GetRaUserDomains().Distinct();

                if (userDomains.Any())
                {
                    Context.CurrentUser.DomainId = userDomains.First().Id.ToString();
                }
            }

            course.PossibleAcademicTerms = CourseActions.ListAcademicTerms().Map(e => e.ToAcademicTerm());


            ViewData["activateCourse"] = activateCourse;


            return View("CreateCourse", course);
        }

        /// <summary>
        /// This is the method for showing the Activate screeen. 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShowActivateCourse()
        {
            string courseid = RouteData.Values["courseid"].ToString();
            Course course = new Course();
            if (courseid != "-1")
            {
                course = CourseActions.GetCourseByCourseId(courseid).ToCourse();
            }

            return View(course);
        }

        /// <summary>
        /// Set the activate date stamp on a course.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ActivateCourse()
        {
            var course = CourseActions.GetCourseByCourseId(Context.EntityId);
           
            course.Domain = Context.Domain;
            CourseActions.ActivateCourse(course);

            Context.CacheProvider.InvalidateCourseContent(course);

            // Create and enroll users for new course.
            //make it check the config value
            bool autoEnroll = Boolean.Parse(ConfigurationManager.AppSettings["AutoEnrollStudents"].ToString());
            if (autoEnroll)
            {
                AutoEnrollmentActions.CreateEnrollments();
            }
            ViewData["School"] = course.Domain.Name;
            ViewData["InstructorEmail"] = Context.CurrentUser.Email;
            try
            {
                bool sendActivationEmail = false;
                bool.TryParse(ConfigurationManager.AppSettings["SendCourseActivationEmail"] != null ? ConfigurationManager.AppSettings["SendCourseActivationEmail"].ToString() : "false", out sendActivationEmail);
                string instructorUserId = string.Empty;
                var domainUser = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, course.Domain.Id);
                instructorUserId = (domainUser == null || string.IsNullOrEmpty(domainUser.Id)) ? Context.CurrentUser.Id : domainUser.Id;
                if (course.Domain.Name == null)
                {
                    ViewData["School"] = domainUser.DomainName;
                }

                if (sendActivationEmail)
                {
                    string emailText = string.Empty;
                    emailText = Helpers.PartialViewRenderer.ToString(this.ControllerContext, "StudentEmailTemplate", course.ToCourse(), this.ViewData, this.TempData);
                    CourseActions.SendInstructorEmail(emailText, course.Title, course.Id, course.ProductCourseId, instructorUserId);
                }
            }
            catch { }
            return View("ShowActivateCourseStep2", course.ToCourse());
        }

        /// <summary>
        /// Content items sort.
        /// </summary>
        /// <param name="l1">The l1.</param>
        /// <param name="l2">The l2.</param>
        /// <returns></returns>
        public static int ContentItemSort(ContentItem l1, ContentItem l2)
        {
            if (l1.DueDate == l2.DueDate)
            {
                try { return l1.Title.CompareTo(l2.Title); }
                catch { return 0; }
            }

            return l1.DueDate.CompareTo(l2.DueDate);
        }

        /// <summary>
        /// Gets the type of the product.
        /// </summary>
        /// <returns></returns>
        public string GetProductType()
        {
            string prodType = Context.ProductType;
            if (Context.Course.CourseType.Equals("faceplate", StringComparison.CurrentCultureIgnoreCase))
            {
                prodType = "faceplate";
            }
          
            return prodType;
        }

        /// <summary>
        /// Temporary hack to allow Marc to style the toc differently.
        /// </summary>
        /// <returns></returns>
        public string GetHtmlClassHack()
        {
            BizDC.PropertyValue val;
            if (Context.Course.Properties.TryGetValue("bfw_htmlclasshack", out val))
                return val.Value.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Gets the type of the product.
        /// </summary>
        /// <returns></returns>
        public string GetCourseSubType()
        {
            string courseSubType = "";

            if(Context.Course != null && !string.IsNullOrEmpty(Context.Course.CourseSubType))
            courseSubType = Context.Course.CourseSubType;

            return courseSubType;
        }

        /// <summary>
        /// Gets the tinyMCE editor options
        /// </summary>
        /// <returns></returns>
        public string GetCourseTinyMCEOptions()
        {
            string result = string.Empty;

            result = Context.Course.ToCourse().TextEditorConfiguration.EditorOptions;

            return result;
        }

        /// <summary>
        /// Gets the Start Page or Launchpad.
        /// </summary>
        /// <returns></returns>
        public string GetStartPage()
        {
            bool isLoadStartOnInit = Context.Course.IsLoadStartOnInit;
            string startPage = string.Empty;
            if (Context.Course.CourseType.Equals("faceplate", StringComparison.CurrentCultureIgnoreCase) && !Context.CourseIsProductCourse && isLoadStartOnInit)
            {
                startPage = "facePlateStartPageBody";
            }
            return startPage;
        }

        

        /// <summary>
        /// Gets the type of the course.
        /// </summary>
        /// <returns></returns>
        public string GetCourseType()
        {
            string CourseType = string.Empty;
            if(Context.Course != null)
                CourseType = Context.Course.CourseType.ToLower();
            return CourseType;
        }

        /// <summary>
        /// Gets the type of the course.
        /// </summary>
        /// <returns></returns>
        public string GetProductCourseId()
        {
            string id = string.Empty;
            if (Context.Course != null)
                id = Context.ProductCourseId;
            return id;
        }

        /// <summary>
        /// Gets the type of the course.
        /// </summary>
        /// <returns></returns>        
        public JsonResult GetContextCourse()
        {
            BizDC.Course course = new BizDC.Course();
            BizDC.UserInfo userInfo = new BizDC.UserInfo();

            userInfo = UserActions.GetUser(Context.CurrentUser.Id);

            if (Context.Course != null && userInfo != null)
            {
                course = Context.Course;
            }

            course.InstructorName = Context.CurrentUser.FormattedName;

            if (!Context.Course.Domain.Id.IsNullOrEmpty())
            {
                var userDomains = Context.GetRaUserDomains().Distinct();

                if (userDomains.Any())
                {
                    course.Domain.Name = userDomains.First().Name.ToString();
                }
            }

            course.LmsIdRequired = false;  //Default value for LMSIDRequired on create new course.

            var model = new
            {
                course.Title,
                course.CourseNumber,
                course.SectionNumber,
                course.InstructorName,
                course.Domain,
                course.AcademicTerm,
                course.CourseTimeZone,
                course.ActivatedDate,
                course.LmsIdRequired
            };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the product name of the course.
        /// </summary>
        /// <returns></returns>
        public string GetCourseProductName()
        {
            if (RouteData.Values.ContainsKey("course"))
                return RouteData.Values["course"].ToString();
            else
                return string.Empty;
           
        }

        /// <summary>
        /// Checks is course activation is an option.
        /// </summary>
        /// <returns></returns>
        public string GetCourseActivation()
        {
            Course course = Context.Course.ToCourse();
            
            var productId = string.IsNullOrEmpty(Context.ProductCourseId) ? Context.EntityId : Context.ProductCourseId; //temp work around for the Context Product

            bool IsAllowedToActivate = !course.IsActivated
                                            && course.AllowActivation
                                            && (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
                                            && (Context.EntityId != productId);
            return IsAllowedToActivate.ToString().ToLower();
        }


        /// <summary>
        /// Gets the course theme.
        /// </summary>
        /// <returns></returns>
        public string GetCourseTheme()
        {
            return Context.Course.Theme;
        }

        /// <summary>
        /// Determines whether [is product course].
        /// </summary>
        /// <returns></returns>
        public string IsProductCourse()
        {
            var isProductCourse = Context.CourseIsProductCourse ? bool.TrueString : bool.FalseString;

            return isProductCourse.ToLower();
        }


        /// <summary>
        /// Determines whether [is sandbox course].
        /// </summary>
        /// <returns></returns>
        public string IsSandboxCourse()
        {
            var isSandboxCourse = Context.Course.IsSandboxCourse ? bool.TrueString : bool.FalseString;

            return isSandboxCourse.ToLower();
        }

        /// <summary>
        /// Shows the fne window based on wether it is a product course or not
        /// </summary>
        /// <returns></returns>
        public ActionResult RenderFne()
        {
            ViewData["courseType"] = Context.Course.CourseType.ToLowerInvariant();
            return View("~/Views/Shared/RenderFneWindow.ascx");
        }

        /// <summary>
        /// Renders and product specific scripts
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductScripts()
        {
            ViewData.Model = Context.Course.ToCourse();
            return View();
        }

        public JsonResult FindOnyxSchool(string searchType, string city, string regionCode, string countryCode, string instituteType, string zipCode)
        {
            var result = CourseActions.FindDomainFromOnyx(searchType, city, regionCode, countryCode, instituteType, zipCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        private BizDC.Course CreateDefaultCourse(Course content)
        {
            var productId = string.IsNullOrEmpty(Context.ProductCourseId) ? Context.EntityId : Context.ProductCourseId;
            BizDC.Course course = new BizDC.Course();
            string courseSubType = Context.Course.SubType;
            
            if (courseSubType == null)
            {
                var routeData = System.Web.Routing.RouteTable.Routes.GetRouteData(new System.Web.HttpContextWrapper(System.Web.HttpContext.Current));                
                courseSubType = routeData.Values["course"] as String;
                //Response.Redirect("COURSESUBTYPEISNULL" + bizContext.Course.Id.ToString(), true);
            }

            course = CourseActions.CreateDerivedCourse(Context.Course, content.SelectedDerivativeDomain);


            //BizDC.UserInfo newUserInfo = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, content.SelectedDerivativeDomain);
                course.Title = content.Title;
                course.CourseProductName = content.CourseProductName;
                course.InstructorName = content.CourseUserName;
                course.CourseTimeZone = TimeZoneInfo.Local.StandardName;
                course.CourseSubType = "regular";
                course.CourseType = content.CourseType.ToString();
                course.CourseHomePage = content.CourseHomePage;
                course.CourseOwner = Context.CurrentUser.Id;
                course.CourseTemplate = content.CourseTemplate;
                course.DashboardCourseId = content.DashboardCourseId;
                course.DerivedCourseId = content.DerivedCourseId;
                course.Theme = content.Theme;


                course.WelcomeMessage = content.WelcomeMessage;
                course.BannerImage = content.BannerImage;
                course.AllowedThemes = content.AllowedThemes;
                var currentAcademicTerm = CourseActions.CurrentAcademicTerm();
                if (currentAcademicTerm != null) { course.AcademicTerm = currentAcademicTerm.Id; }




                course = CourseActions.UpdateCourses(new List<Bfw.PX.Biz.DataContracts.Course>() { course }).First();
                
            return course;
            
        }
        

        /// <summary>
        /// SEts the defaults on the course object that is then passed to the course creation method
        /// </summary>
        /// <param name="domainId">the id of the domain the create the course in</param>
        /// <returns>Course object</returns>
        private Course BaseCourse(string domainId)
        {
            var productId = string.IsNullOrEmpty(Context.ProductCourseId) ? Context.EntityId : Context.ProductCourseId;

            Course course = new Course();
            course.Title = String.Format("DEMO-{0}", course.UtcRelativeAdjust(DateTime.Now).ToShortDateString());
            course.CurrentUserName = Context.CurrentUser.FirstName + " " + Context.CurrentUser.LastName;
            course.CourseUserName = course.CurrentUserName;
            course.ProductName = (Context.Product == null) ? "[Digital Product Name]" : Context.Product.Title;
            course.CourseProductName = String.Format("[CourseNumber]-[SectionNumber] [Title],{0}", course.ProductName);
            course.CourseType = (Context.Product == null) ? course.CourseType : (CourseType)Enum.Parse(typeof(CourseType), Context.Product.CourseType, true); ;
            // course.CourseProductName = "[Content Title][Digital Product Name]";
            course.SelectedDerivativeDomain = domainId.ToString();
            course.CourseTimeZone = course.CourseTimeZone;
            if(course.DashboardSettings != null){
                course.DashboardSettings.DashboardHomePageStart = Context.Course.DashboardSettings.DashboardHomePageStart;
                course.DashboardSettings.IsInstructorDashboardOn = false;
                course.DashboardSettings.IsProgramDashboardOn = false;
            }
            course.CourseSubType = "regular";
            course.DerivedCourseId = Context.Course.Id;
            return course;

        }


        public ActionResult ClearCourse(string id)
        {
            BizDC.Course course = new BizDC.Course();
            course.Id = id;
            string returnMessage = string.Format("Could not clear cache for {0}", id);
            if (id != null)
            {
                course.ProductCourseId = Context.ProductCourseId;
                Context.CacheProvider.InvalidateCourseContent(course);
                returnMessage = string.Format("Cleared Cache for Course:{0}", id);
            }


            return Content(returnMessage);
        }

        public ActionResult ClearProductCourse(string id)
        {
            BizDC.Course course = new BizDC.Course();
            course.Id = id;
            string returnMessage = string.Format("Could not clear cache for {0}", id);
            if (id != null)
            {
                Context.CacheProvider.InvalidateProductCourse(course);
                returnMessage = string.Format("Cleared Cache for Product Course:{0}", id);
            }


            return Content(returnMessage);
        }



        public ActionResult ClearPageDefinitions(string id)
        {

            string returnMessage = string.Format("Could not clear cache for page definition {0}", id);
            if (id != null)
            {
                Context.CacheProvider.InvalidatePageDefinition(Context.Course.Id, id);
                returnMessage = string.Format("Cleared Cache for page definition :{0}", id);
            }

            return Content(returnMessage);
        }

        #region implementation
        /// <summary>
        /// When course's domain changed, we need to update the course enrollments' user ids.
        /// </summary>
        /// <param name="course"></param>
        private void UpdateCourseEnrollments(BizDC.Course course)
        {
            if (course.Domain == null)
                return;

            var enrollments = EnrollmentActions.GetAllEntityEnrollmentsAsAdmin(course.Id);
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
        #endregion implementation

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is ProgramManagerNotFoundException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.HttpContext.Response.Clear();
                filterContext.HttpContext.Response.StatusCode = 200;
                filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

                var routeData = ControllerContext.RouteData;
                routeData.Values["controller"] = "ErrorPage";
                routeData.Values["action"] = "DisplayError";
                routeData.Values.Add("exception", filterContext.Exception);
                IController errorController = new ErrorPageController();
                errorController.Execute(ControllerContext.RequestContext);
            }
        }
    }
}