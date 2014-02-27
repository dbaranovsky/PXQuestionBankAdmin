using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using AgxDC = Bfw.Agilix.DataContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    /// <summary>
    /// Default controller used to render the index page
    /// </summary>

    [PerfTraceFilter]
    public class HomeController : Controller
    {
        /// <summary>
        /// Contains business layer context information
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
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }
        /// <summary>
        /// Gets or sets the navigation actions.
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected BizSC.INavigationActions NavigationActions { get; set; }
        /// <summary>
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        protected BizSC.IUserActions UserActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="courseAction">The course action.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="navigationActions">The navigation actions.</param>
        /// <param name="contentHelper">The content helper.</param>
        public HomeController(BizSC.IBusinessContext context, BizSC.ICourseActions courseAction, BizSC.IContentActions contentActions, BizSC.INavigationActions navigationActions, IContentHelper contentHelper, BizSC.IPageActions pageActions, BizSC.IEnrollmentActions enrollmentActions, BizSC.IUserActions userActions)
        {
            Context = context;
            CourseActions = courseAction;
            ContentActions = contentActions;
            NavigationActions = navigationActions;
            ContentHelper = contentHelper;
            PageActions = pageActions;
            EnrollmentActions = enrollmentActions;
            UserActions = userActions;
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return InitializeHomePage();
        }

        private ActionResult InitializeHomePage()
        {
            if (Context.CurrentUser == null)
            {
                Context.Initialize();
            }

            var pageDefinitionId = string.Empty;

            //determine if start page has been viewed by this user in this course
            bool startPageViewed = ProcessStartCookie();
            if (Context.AccessLevel == BizSC.AccessLevel.Student && !Context.Course.StudentStartPage.IsNullOrEmpty() && Context.Course.CourseSubType == "regular")
            {
                pageDefinitionId = Context.Course.StudentStartPage;
            }
            else if (Context.Course.CourseSubType == "instructor_dashboard")
            {
                pageDefinitionId = Context.Course.DashboardSettings.DashboardHomePageStart;
            }
            else if (Context.Course.IsLoadStartOnInit && !startPageViewed)
            {
                pageDefinitionId = (string.IsNullOrEmpty(Context.Course.CourseStartPage) || Context.Course.CourseStartPage.ToLowerInvariant() == "index") ? "HOME" : Context.Course.CourseStartPage;
            }
            else
            {
                pageDefinitionId = (string.IsNullOrEmpty(Context.Course.CourseHomePage) || Context.Course.CourseHomePage.ToLowerInvariant() == "index") ? "HOME" : Context.Course.CourseHomePage;
            }

            ProcessPageDefinition(pageDefinitionId);

            var model = ViewData.Model as LayoutConfiguration;
            if (model != null && !model.IsActivated)
            {
                //Custom Course Activation header
                var activationItem = ContentActions.GetContent(Context.CourseId, "PX_PRODUCT_ACTIVATION_TEXT");
                if (activationItem != null)
                {
                    var activationText = activationItem.Description;
                    ViewData["ActivationText"] = HttpUtility.HtmlDecode(activationText);
                }
            }

            if (Context.Course.IsLoadStartOnInit && !startPageViewed && Context.Course.CourseSubType != "instructor_dashboard")
            {
                if (Context.Course.Id == Context.ProductCourseId)
                {
                    return RedirectToRoute("Dashboard");
                }
                else
                {
                    return View("Start");
                }
            }
            else
            {
                return View();
            }
        }


        public ActionResult Home()
        {
            return InitializeHomePage();
        }

        public ActionResult LoadPageDefinition(string pageDefnId)
        {
            var pageDefinitions = this.PageActions.LoadPageDefinition(pageDefnId);

            if (pageDefinitions != null)
            {
                ProcessPageDefinition(pageDefnId);

                return View("PageContainer", pageDefinitions.ToPageDefinition());
            }
            else
                return View("PageContainer", null);
        }

        private bool ProcessStartCookie()
        {
            if (Request.Cookies["StartPageViewed"] != null)
            {
                var data = Request.Cookies["StartPageViewed"].Value.Split(new char[] { '|' });
                var cookieCourseId = data[0];
                var cookieUserName = data[1];

                if (cookieCourseId == Context.CourseId && cookieUserName == Context.CurrentUser.Username)
                {
                    return true;
                }
            }
            return false;
        }

        public ActionResult FromStart()
        {
            if (Request.Cookies["StartPageViewed"] != null)
            {
                Request.Cookies.Remove("StartPageViewed");
            }

            Response.Cookies.Add(new System.Web.HttpCookie("StartPageViewed", String.Format("{0}|{1}", Context.CourseId.ToString(), Context.CurrentUser.Username)));

            return RedirectToAction("IndexDefault");
        }

        public ActionResult IndexDefault()
        {
            var pageDefinitionId = (string.IsNullOrEmpty(Context.Course.CourseHomePage) || Context.Course.CourseHomePage.ToLowerInvariant() == "index") ? "HOME" : Context.Course.CourseHomePage;
            ProcessPageDefinition(pageDefinitionId);
            return View("Index");
        }

        public ActionResult IndexStart()
        {
            var pageDefinitionId = (string.IsNullOrEmpty(Context.Course.CourseStartPage) || Context.Course.CourseStartPage.ToLowerInvariant() == "index") ? "HOME" : Context.Course.CourseStartPage;
            ProcessPageDefinition(pageDefinitionId);
            return View("Start");
        }

        public ActionResult IndexDashboard()
        {
            var pageDefinitionId = Context.Course.DashboardSettings.DashboardHomePageStart;
            ProcessPageDefinition(pageDefinitionId);
            return View("Dashboard");
        }

        public ActionResult Enroll()
        {
            string courseid = RouteData.Values["courseid"].ToString();
            string univ = "";
            string instructor = "";
            BizDC.Course course = new BizDC.Course();
            if (courseid != "-1")
            {
                course = CourseActions.GetCourseByCourseId(courseid);
                univ = Context.Domain.Name;
                instructor = course.InstructorName;
                ViewData["AcademicTerm"] = CourseActions.ListAcademicTerms(course.Domain.Id).First(x => x.Id == course.AcademicTerm).ToAcademicTerm().Name;
                ViewData["Coursetitle"] = course.Title;
            }

            ViewData["University"] = univ;
            ViewData["CurrentUser"] = Context.CurrentUser.FormattedName;

            ViewData["Instructor"] = instructor;

            ViewData["EnrollmentID"] = Context.EnrollmentId.IsNullOrEmpty() ? "" : Context.EnrollmentId;

            var model = course.ToCourse();

            return View(model);
        }



        private void ProcessPageDefinition(string pageDefinitionId)
        {
            //pageDefinitionId = (string.IsNullOrEmpty(Context.Course.CourseHomePage) || Context.Course.CourseHomePage.ToLowerInvariant() == "index") ? "HOME" : Context.Course.CourseHomePage;
            var layout = new LayoutConfiguration() { Title = "Home" };
            
            if (!String.IsNullOrEmpty(Context.EntityId))
            {
                layout.Course = SetupActiveAndCreateAccess(layout);
            }
            else
            {
                layout.Course = Context.Course.ToCourse();
            }

            var pageDefinitions = this.PageActions.LoadPageDefinition(pageDefinitionId);
            layout.PageDefinitions = pageDefinitions.ToPageDefinition();

            ViewData["AccessLevel"] = Context.AccessLevel.ToString().ToLowerInvariant();
            ViewData["theme"] = Context.Course.Theme;
            ViewData["CourseType"] = Context.Course.CourseType;
            ViewData["IsActivated"] = (layout.Course == null) ? false : layout.Course.IsActivated;
            ViewData["CurrentUser"] = Context.CurrentUser.FormattedName;
            ViewData["IsProductCourse"] = Context.CourseIsProductCourse;
            ViewData["DomainCount"] = Context.GetRaUserDomains().Distinct().Count().ToString();

            ViewData.Model = layout;
        }

        [HttpPost]
        public ActionResult EnrollmentConfirmation()
        {
            string courseid = RouteData.Values["courseid"].ToString();
            string instructor = "";
            BizDC.Course course = new BizDC.Course();
            String studentPermissionFlags = ConfigurationManager.AppSettings["StudentPermissionFlags"];
            List<BizDC.Enrollment> enroll = new List<BizDC.Enrollment>();
            var user = Context.CurrentUser;

            if (courseid != "-1")
            {
                course = CourseActions.GetCourseByCourseId(courseid);
                instructor = course.InstructorName;

                if (string.IsNullOrEmpty(Context.CurrentUser.Id) && !string.IsNullOrEmpty(Context.CurrentUser.Username) && !string.IsNullOrEmpty(Context.CurrentUser.Email))
                {
                    user = Context.GetNewUserData();
                    user = UserActions.CreateUser(user.Username, user.Password, user.PasswordQuestion, user.PasswordAnswer, user.FirstName, user.LastName, user.Email, course.Domain.Id, course.Domain.Name, user.ReferenceId);
                }

                enroll = EnrollmentActions.CreateEnrollments(course.Domain.Id, user.Id, course.Id, studentPermissionFlags, "1", DateTime.Now, DateTime.Now.AddYears(1), string.Empty, string.Empty);
            }

            ViewData["CurrentUser"] = user.FormattedName;
            ViewData["Instructor"] = instructor;

            var model = course.ToCourse();

            if (enroll.First().Id.IsNullOrEmpty())
            {
                return View("Enroll", model);
            }


            return View(model);
        }

        public ActionResult DashBoard()
        {
            var layout = new LayoutConfiguration() { Title = "HOME_DASHBOARD" };

            if (!String.IsNullOrEmpty(Context.EntityId))
            {
                SetupActiveAndCreateAccess(layout);
            }

            var pageDefinitions = this.PageActions.LoadPageDefinition("HOME_DASHBOARD");
            layout.PageDefinitions = pageDefinitions.ToPageDefinition();

            ViewData["AccessLevel"] = Context.AccessLevel.ToString().ToLowerInvariant();

            ViewData.Model = layout;

            return View();
        }



        /// <summary>
        /// Menu
        /// </summary>
        /// <returns></returns>
        public ActionResult Menu()
        {
            var menu = new Menu();

            var course = Context.Course.ToCourse();

            string menuKey = "";
            if (course.CourseType == CourseType.XCLASS)
            {
                menuKey = "PX_PRIMARY_XCLASS";
            }

            if (!string.IsNullOrEmpty(menuKey))
            {
                menu = this.PageActions.LoadMenu(menuKey).ToMenu();

                if (Context.CourseIsProductCourse)
                    menu.MenuItems.RemoveAll(i => i.BfwDisplayOnProductCourse == false);

                menu.SetActiveMenuItem(ControllerContext.RouteData.Values, Request.Url);                

                var displayOption = (Context.AccessLevel == BizSC.AccessLevel.Student) ? BizDC.DisplayOption.Student : BizDC.DisplayOption.Instructor;
                menu.RemoveMenuItemsBasedOnRole(displayOption);
            }

            ViewData.Model = menu;
            ViewData["AccessLevel"] = Context.AccessLevel;
            ViewData["IsSharedCourse"] = Context.IsSharedCourse;
            return View("Menu");
        }

        /// <summary>
        /// When hit, this action method will set a cookie called PXPROFILER with the value of set.
        /// This value determines if the on screen profiler is enabled or not.
        /// </summary>
        /// <param name="set">true (default) to enable tracing, false to disable it.</param>
        /// <returns>String containing text showing if tracing is enabled or not.</returns>
        public ActionResult EnableTracing(bool set = true)
        {
            ControllerContext.HttpContext.Response.Cookies.Add(new System.Web.HttpCookie("PXPROFILER", set.ToString()));

            return Content(string.Format("tracing {0}", set));
        }


        /// <summary>
        /// Show preview as visitor
        /// </summary>
        /// <returns></returns>
        public string ShowPreviewAsVisitor()
        {
            Response.Cookies.Add(new System.Web.HttpCookie(Context.PreviewAsVisitorCookieKey, "true"));
            return "EPortfolioBrowser";
        }

        /// <summary>
        /// Resume Editing
        /// </summary>
        /// <returns></returns>
        public string ResumeEditing()
        {
            if (Request.Cookies.AllKeys.Contains(Context.PreviewAsVisitorCookieKey))
            {
                var expired = new System.Web.HttpCookie(Context.PreviewAsVisitorCookieKey) { Expires = Context.Course.UtcRelativeAdjust(DateTime.Now).AddDays(-1) };
                Response.Cookies.Add(expired);
            }
            return "EPortfolioBrowser";
        }


        /// <summary>
        /// Pings this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Ping()
        {
            return new EmptyResult();
        }

        /// <summary>
        /// Mies the courses.
        /// </summary>
        /// <returns></returns>
        public ActionResult MyCourses()
        {
            var layout = new LayoutConfiguration() { Title = "My Courses" };

            if (!Context.IsAnonymous)
            {
                layout.AddWidget("Zone1", new WidgetConfiguration()
                {
                    Controller = "CourseWidget",
                    Action = "ViewAll",
                    CssClass = "course-widget summary",
                    Order = 2,
                    Title = "My Courses",
                    ViewAllFne = true
                });
            }

            ViewData.Model = layout;

            return View("Index");
        }

        /// <summary>
        /// Helps this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Help()
        {
            return View();
        }

        /// <summary>
        /// Versions this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Version()
        {
            var path = Server.MapPath("~/version.txt");
            var version = "local build";

            if (System.IO.File.Exists(path))
            {
                version = System.IO.File.ReadAllText(path);
            }

            return Content(version);
        }

        /// <summary>
        /// Tests the raw.
        /// </summary>
        /// <returns></returns>
        public ActionResult TestRaw()
        {
            return View();
        }

        /// <summary>
        /// Tests the breadcrumb.
        /// </summary>
        /// <returns></returns>
        public ActionResult TestBreadcrumb()
        {
            return View();
        }

        /// <summary>
        /// Tests the loading of a new project item type.
        /// </summary>
        /// <returns></returns>
        /*
        public ActionResult TestProjectItem()
        {
            //IEnumerable<BizDC.ContentItem> clist = ContentActions.ListContent(Context.EntityId, "Project");
            //IEnumerable<Project> projectList = clist.Map(i => i.ToProjectItem());
            //ViewData.Model = projectList;
            //return View();
            
            
            //IEnumerable<BizDC.ContentItem> clist = ContentActions.ListContent(Context.EntityId, "HtmlDocument");
            IEnumerable<BizDC.ContentItem> clist = ContentActions.ListChildren(Context.EntityId,"PJX_1",1,"bfw_toc_PJX_1",true);

            IEnumerable<ContentItem> mlist = clist.Map(i => i.ToContentItem(ContentActions));
            ViewData.Model = mlist;
            return View();
        }
        */


        /// <summary>
        /// Tests the I frame.
        /// </summary>
        /// <returns></returns>
        public ActionResult TestIFrame()
        {
            return View();
        }


        public ActionResult ModalTest()
        {
            return View();
        }
        /// <summary>
        /// Setups the active and create access.
        /// </summary>
        /// <param name="layout">The layout.</param>
        /// <returns></returns>
        private Course SetupActiveAndCreateAccess(LayoutConfiguration layout)
        {
            Course course = CourseActions.GetCourseByCourseId(Context.EntityId).ToCourse();
            layout.IsActivated = course.IsActivated;

            var productId = string.IsNullOrEmpty(Context.ProductCourseId) ? Context.EntityId : Context.ProductCourseId; //temp work around for the Context Product

            layout.IsAllowedToActivate = !course.IsActivated
                                            && course.AllowActivation
                                            && (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Instructor)
                //&& (Context.AccessType == Bfw.PX.Biz.ServiceContracts.AccessType.Adopter) //https://macmillanhighered.atlassian.net/browse/PX-3919 Remove Sampling Instructor text for instructors created from Onyx
                                            && (Context.EntityId != productId);
            layout.IsAllowedToCreateCourse = Context.CanCreateCourse;
            layout.CourseType = (Context.AccessType == Bfw.PX.Biz.ServiceContracts.AccessType.Adopter) ? "Course" : "Demo Course";
            return course;
        }

        /// <summary>
        /// Ins the active course.
        /// </summary>
        /// <returns></returns>
        public ActionResult InActiveCourse()
        {
            return View("InActiveCourse", Context.Course.ToCourse());
        }

        /// <summary>
        /// Ins the active course.
        /// </summary>
        /// <returns></returns>
        public ActionResult CourseNotAdopted()
        {
            return View("CourseNotAdopted", Context.Course.ToCourse());
        }

        /// <summary>
        /// Gets the widgets.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public List<WidgetConfiguration> GetWidgets(string parentId)
        {
            var items = new List<WidgetConfiguration>();

            foreach (var item in ContentActions.ListChildren(Context.EntityId, parentId))
            {
                if (item.Subtype == "WidgetConfiguration")
                    items.Add(item.ToWebConfiguration());
            }

            return items;
        }

        public ActionResult TinyMCEPage()
        {
            return View();
        }

        public ActionResult LoadingSplashScreen()
        {
            if (this.Context.Course.EnableLoadingScreen)
            {
                ViewData["loadingEntity"] = Context.Course.CourseSubType == "regular" ? "Course" : "Dashboard";

                return View();
            }
            else
            {
                return new EmptyResult();
            }
        }

        /// <summary>
        /// Selects the course.
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectCourse()
        {
            ViewData.Model = Context.FindCoursesByUserEnrollment(Context.CurrentUser.Id, Context.ProductCourseId).Map(c => c.ToCourse());

            Context.Course = new Bfw.PX.Biz.DataContracts.Course()
            {
                Title = "Choose a course",
                InstructorName = "User"
            };

            return View();
        }
    }
}
