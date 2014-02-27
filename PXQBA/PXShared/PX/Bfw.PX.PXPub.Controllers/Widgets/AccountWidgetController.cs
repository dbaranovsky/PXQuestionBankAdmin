using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;
using Bfw.Common;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.Common.Collections;
using Bfw.Common.Caching;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Contracts;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provided all actions required to implement the account widget which allows
    /// users to login, log out, and view their profile information
    /// </summary>
    [PerfTraceFilter]
    public class AccountWidgetController : Controller, IPXWidget
    {
        private IUrlHelperWrapper _urlHelper;

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
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Access to PageActions functionality
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Access to PageActions functionality
        /// </summary>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Gets or sets the course helper
        /// </summary>
        public ICourseHelper CourseHelper { get; set; }

        /// <summary>
        /// Gets or sets the cache provider.
        /// </summary>
        /// <value>
        /// The cache provider.
        /// </value>
        public ICacheProvider CacheProvider { get; protected set; }

        public IUrlHelperWrapper UrlHelper
        {
            get 
            { 
                if (_urlHelper == null)
                    _urlHelper = ServiceLocator.Current.GetInstance<IUrlHelperWrapper>();
                return _urlHelper;
            }
        }

        /// <summary>
        /// Constant value for "instructor_dashboard"
        /// </summary>
        private const string INSTRUCTOR_DASHBOARD = "instructor_dashboard";

        /// <summary>
        /// Constructs a default AccountWidgetController. Depends on a business context
        /// and user actions implementation
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userActions">The user actions.</param>
        public AccountWidgetController(BizSC.IBusinessContext context, BizSC.IUserActions userActions, BizSC.IPageActions pageActions, BizSC.ICourseActions courseActions, BizSC.IEnrollmentActions enrollmentActions, ICourseHelper courseHelper, ICacheProvider cacheProvider)
        {
            Context = context;
            UserActions = userActions;
            PageActions = pageActions;
            this.CourseActions = courseActions;
            EnrollmentActions = enrollmentActions;
            CacheProvider = cacheProvider;
            CourseHelper = courseHelper;
        }

        #region IPXWidget Members

        /// <summary>
        /// Shows the login status of the currently logged in user. If the user is
        /// anonymous then they are considered to be not authenticated.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            //if isContentTab is true then the studentView link is going to get the value of PxPage.PageReload" if it's false it gets "window.location.reload();
            ViewData["isContentTab"] = RouteData.Values.ContainsKey("__px__routename") && RouteData.Values["__px__routename"].ToString().ToLowerInvariant() == "featuredcontentitem";
            var model = new AccountWidget()
            {
                IsAuthenticated = false
            };

            // Figure out whether and how the 'student view' (or 'instructor view') link should be shown.
            if (Context.CourseIsProductCourse)
            {
                model.StudentViewStatus = AccountWidget.StudentViewStates.HideLink;
            }
            else if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                model.StudentViewStatus = AccountWidget.StudentViewStates.InstructorView;
            }
            else
            {
                model.StudentViewStatus = Context.ImpersonateStudent ? AccountWidget.StudentViewStates.StudentView : AccountWidget.StudentViewStates.NotInstructor;
            }

            model.IsAuthenticated = !Context.IsAnonymous;

            if (model.IsAuthenticated || Context.IsPublicView)
            {
                var biz = Context.CurrentUser;
                model.Account = biz.ToAccount();
            }

            model.IsInstructorDashboardActive = Context.Product.IsDashboardActive;
            GetInstructorDashboard(model);

            // this code block below is duplicating the functionality that is the global.asax file.
            // I am modifing it to reflect the same values but this needs to be addressed
            // G. Chernyak

            string login;
            string logout;

            // login base url
            string configUrlBasePublic = ConfigurationManager.AppSettings["MarsUrlBasePublic"];
            // logout base url
            string configUrlBaseSecure = ConfigurationManager.AppSettings["MarsUrlBaseSecure"];

            string configPathLogin = ConfigurationManager.AppSettings["MarsPathLogin"];
            string configPathLogout = ConfigurationManager.AppSettings["MarsPathLogout"];
            string configUrlPlatformValue = ConfigurationManager.AppSettings["MarsUrlPlatformValue"];

            // handling possible null value in Request.URL
            string protocol = string.Empty;
            string host = string.Empty;
            if (Request.Url != null)
            {
                protocol = Request.Url.Scheme;
                host = Request.Url.Host;
            }

            if (string.IsNullOrEmpty(configUrlBasePublic))
            {
                login = UrlHelper.RouteUrl("Login");
                logout = UrlHelper.RouteUrl("Logout");
            }
            else
            {

                login = string.Format(configUrlBasePublic, configPathLogin, configUrlPlatformValue, protocol + "://" + host + UrlHelper.RouteUrl("CourseSectionHome"));
                logout = string.Format(configUrlBaseSecure, configPathLogout, configUrlPlatformValue, protocol + "://" + host + UrlHelper.RouteUrl("CourseSectionHome"));
            }
            model.LoginUrl = login;

            var useWif = false;
            var useWifValue = ConfigurationManager.AppSettings["UseWIF"];

            if (!string.IsNullOrEmpty(useWifValue))
            {
                bool.TryParse(useWifValue, out useWif);
            }

            if (useWif)
            {
                model.LogoutUrl = Url.Action("Logout", "Account");
            }
            else
            {
                model.LogoutUrl = logout;
            }

            List<SelectListItem> accountActionsList = null;
            if (Context.Course.CourseType == BizDC.CourseType.LearningCurve.ToString())
            {
                ViewData["AddCreateCourseLink"] = false;
                accountActionsList = new List<SelectListItem>{new SelectListItem()
                    {
                        Selected = true,
                        Text = model.Account.DisplayName,
                        Value = "user"
                    },
                    new SelectListItem()
                    {
                        Selected = false,
                        Text = "Manage Profile",
                        Value = "profile"
                    }
                };
                var courses = GetUserCourse();
                if (courses.Any())
                {
                    accountActionsList.Add(new SelectListItem()
                    {
                        Text = "--------------------------------------",
                        Value = "disabled"
                    });
                    accountActionsList.AddRange(GetUserCourse());
                }
            }
            else
            {
                ViewData["AddCreateCourseLink"] = model.StudentViewStatus == AccountWidget.StudentViewStates.InstructorView;
                accountActionsList = new List<SelectListItem>();
                accountActionsList.Add(
                    new SelectListItem()
                    {
                        Selected = true,
                        Text = model.Account.DisplayName,
                        Value = "user"
                    });
                if (Context.AccessLevel == BizSC.AccessLevel.Student && Context.Course.CourseType == "FACEPLATE")
                {
                    accountActionsList.Add(
                        new SelectListItem()
                        {
                            Selected = false,
                            Text = "Switch course enrollment",
                            Value = "switchenrollment"
                        });
                }
                accountActionsList.Add(
                    new SelectListItem()
                    {
                        Selected = false,
                        Text = "Manage Profile",
                        Value = "profile"
                    });
            }

            ViewData["accountActionsList"] = accountActionsList;

            var userHasMultipleDomains = false;
            if (!Context.IsAnonymous)
            {
                userHasMultipleDomains = (Context.GetRaUserDomains().Count() > 1);
            }

            ViewData["userHasMultipleDomains"] = userHasMultipleDomains;
            ViewData["ProductCourseId"] = Context.ProductCourseId;
            ViewData["AccessLevel"] = Context.AccessLevel.ToString();
            ViewData.Model = model;
            ViewData["CourseType"] = Context.Course.CourseType.ToString();
            ViewData["HelpUrl"] = GetHelpUrl(Context.Course.CourseType, Context.AccessLevel.ToString());

            if (Context.Course.HideStudentViewLink)
            {
                ViewData["ShowStudentViewLink"] = false;
            }
            else
            {
                ViewData["ShowStudentViewLink"] = Context.Course.ShowStudentViewLink;
            }

            return View();
        }

        /// <summary>
        /// Json formatted array of all enrollments for the user.
        /// </summary>
        /// <returns>List of enrollments for the logged in user.</returns>
        public ActionResult ListUserEnrollments()
        {
            var courseJSON = new List<SelectListItem>();
            IEnumerable<BizDC.Course> userCourses = null;

            if ((!Context.IsAnonymous) && (Context.CurrentUser != null))
            {
                try
                {
                    var userData = new Bfw.PX.Biz.DataContracts.UserInfo();
                    userData.Username = Context.CurrentUser.Username;
                    var users = UserActions.ListUsersLike(userData);
                    var localCourses = Context.FindCoursesByUserEnrollmentBatch(users, Context.ProductCourseId, true);
                    userCourses = localCourses.ToList();
                }
                catch
                {
                    userCourses = new List<BizDC.Course>();
                }

                courseJSON = userCourses.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = UrlHelper.RouteUrl("CourseSectionHome", new { courseid = c.Id })
                }).ToList();

                //course creation is now handled in the view
                //if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
                //{
                //    //PLATX-9020
                //    SelectListItem item = new SelectListItem()
                //    {
                //        Text = "Create Course",
                //        Value = "CreateCourse"
                //    };
                  
                //    SelectListItem sep = new SelectListItem()
                //    {
                //        Text = "--------------------------------------",
                //        Value = "disabled"
                //    };

                //    courseJSON.Add(sep);
                //    courseJSON.Add(item);
                //}
            }

            return Json(courseJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ListCoursesForDropDown()
        {

            var courseJSON = new List<SelectListItem>();
            if (Context.Course.CourseType.Equals(CourseType.LearningCurve.ToString(), StringComparison.CurrentCultureIgnoreCase) ||
                (Context.Course.CourseType.Equals(CourseType.XBOOK.ToString(), StringComparison.CurrentCultureIgnoreCase) || 
                (Context.Course.CourseType.Equals(CourseType.FACEPLATE.ToString(), StringComparison.CurrentCultureIgnoreCase))))
                return ListCourses();


                var enrollments = EnrollmentActions.ListEnrollments(Context.CurrentUser.Username);

                List<BizDC.Enrollment> dashboards = new List<BizDC.Enrollment>();
                foreach (BizDC.Enrollment dd in enrollments)
                {
                    if (dd.Course.CourseSubType != null && dd.Course.CourseSubType == "regular" && !dd.Course.SubType.IsNullOrEmpty() && (Context.Course.ProductCourseId == dd.Course.ParentId || Context.Course.Id == dd.Course.ParentId) && dd.Course.CourseSubType != "program_dashboard")
                    {
                        string courseType = dd.Course.CourseType.ToLowerInvariant();
                        
                        string url = UrlHelper.RouteUrl("CourseSectionHome", new
                        {
                            course = dd.Course.SubType.ToLowerInvariant(),
                            section = dd.Course.CourseType.ToLowerInvariant(),
                            courseid = dd.Course.Id
                        });
                        
                        courseJSON.Add(new SelectListItem()
                        {
                            Text = dd.Course.Title,
                            Value = url
                        });
                    }
                    if (dd.Course.CourseSubType != null && dd.Course.CourseSubType == "instructor_dashboard" && Context.AccessLevel == BizSC.AccessLevel.Instructor)
                    {
                        dashboards.Add(dd);

                    }
                }

                if (dashboards != null && Context.Product != null && Context.Product.IsDashboardActive == true && Context.AccessLevel == BizSC.AccessLevel.Instructor)
                {
                    var currentDashboardCourses = dashboards.Where(i => i.Course.ProductCourseId == Context.Course.ProductCourseId);
                    var orderedcurrentDashboardCourses = currentDashboardCourses.OrderByDescending(i => i.Course.Id);
                    BizDC.Enrollment mydashboard = !orderedcurrentDashboardCourses.IsNullOrEmpty() ? orderedcurrentDashboardCourses.First() : new BizDC.Enrollment();

                    if (Context.Course.ProductCourseId != null)
                    {
                        var currentDashboard = orderedcurrentDashboardCourses.Where(i => i.Course.Id == Context.Course.ProductCourseId);
                        if (currentDashboard.Count() > 0)
                        {
                            mydashboard = currentDashboard.First();
                        }
                    }

                    if (mydashboard != null)
                    {
                        courseJSON.Add(new SelectListItem()
                        {
                            Text = "--------------------------------------",
                            Value = "disabled"
                        });

                        courseJSON.Add(new SelectListItem()
                        {
                            Text = "My Dashboard",
                            Value = UrlHelper.RouteUrl("CourseSectionHome", new
                            {
                                course = Context.Course.SubType.ToLowerInvariant(),
                                section = Context.Course.CourseType.ToLowerInvariant(),
                                courseid = mydashboard.Course.Id
                            })
                        });
                    }
                }

                if (Context.AccessLevel == BizSC.AccessLevel.Student && !string.IsNullOrEmpty(Context.ProductCourseId))
                {

                    courseJSON.Add(new SelectListItem()
                    {
                        Text = "--------------------------------------",
                        Value = "disabled"
                    });

                    BizDC.Course parentCourse = CourseActions.GetCourseByCourseId(Context.ProductCourseId);
                    if (parentCourse.EnrollmentSwitchSupported)
                    {
                        courseJSON.Add(new SelectListItem() { Text = "Join a different course", Value = UrlHelper.RouteUrl("EcomEnroll", new { courseid = Context.Course.Id, switchEnrollFromCourse = Context.Course.Id }).ToString() });
                    }
                }

            if (Context.AccessLevel == BizSC.AccessLevel.Instructor)
            {
                courseJSON.Add(new SelectListItem()
                {
                    Text = "--------------------------------------",
                    Value = "disabled"
                });

                courseJSON.Add(new SelectListItem()
                {
                    Text =
                        (Context.Course.CourseType == CourseType.LearningCurve.ToString())
                            ? "Create a new Course"
                            : "Create Course",
                    Value = "CreateCourse"
                });
            }

            return Json(courseJSON, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> GetUserCourse()
        {
            var enrolledCourses = CourseHelper.ListCourses(Context.CurrentUser.Username, Context.CourseIsProductCourse ? Context.CourseId : Context.Course.ProductCourseId, false, true, Context.Course.CourseType);

            var courses = new List<SelectListItem>();
            foreach (BizDC.Course course in enrolledCourses)
            {
                if (course.CourseSubType.Equals("regular", StringComparison.CurrentCultureIgnoreCase))
                {

                    //Regular Courses
                    var url = UrlHelper.RouteUrl("CourseSectionHome", new
                    {
                        course = course.SubType.ToLowerInvariant(),
                        section = course.CourseSectionType == "media" ? "media" : course.CourseType.ToLowerInvariant(),
                        courseid = course.Id
                    });


                    courses.Add(new SelectListItem()
                    {
                        Text = course.Title,
                        Value = url
                    });
                }
            }

            if (Context.AccessLevel == BizSC.AccessLevel.Student)
            {
                if (!string.IsNullOrEmpty(Context.ProductCourseId))
                {
                    courses.Add(new SelectListItem()
                    {
                        Text = "--------------------------------------",
                        Value = "disabled"
                    });

                    BizDC.Course parentCourse = CourseActions.GetCourseByCourseId(Context.ProductCourseId);
                    if (parentCourse.EnrollmentSwitchSupported)
                    {
                        courses.Add(new SelectListItem() { Text = "Join a different course", Value = UrlHelper.RouteUrl("EcomEnroll", new { courseid = Context.Course.Id, switchEnrollFromCourse = Context.Course.Id }).ToString() });
                    }
                }
            }
            else
            {
                courses.Add(new SelectListItem()
                {
                    Text = "--------------------------------------",
                    Value = "disabled"
                });

                if (!Context.CourseIsProductCourse)
                {
                    courses.Add(new SelectListItem()
                    {
                        Text = "My Dashboard",
                        Value = UrlHelper.RouteUrl("CourseSectionHome", new
                        {
                            course = Context.Course.SubType.ToLowerInvariant(),
                            section = Context.Course.CourseSectionType == "media" ? "media" : Context.Course.CourseType.ToLowerInvariant(),
                            courseid = Context.Course.ProductCourseId
                        }) + "/" + "Dashboard"
                    });

                    courses.Add(new SelectListItem()
                    {
                        Text = "--------------------------------------",
                        Value = "disabled"
                    });
                }

                courses.Add(new SelectListItem()
                {
                    Text = (Context.Course.CourseType == CourseType.LearningCurve.ToString()) ? "Create a new Course" : "Create Course",
                    Value = "CreateCourse"
                });
            }
            return courses;

        }
        /// <summary>
        /// List learning curve courses to display in accountActionsList
        /// </summary>
        /// <returns></returns>
        public ActionResult ListCourses()
        {
            
            return Json(GetUserCourse(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newView"></param>
        /// <returns></returns>        
        public JsonResult SwitchView(AccountWidget.StudentViewStates to)
        {
            // Add or remove the student view cookie, and refresh the page.
            if (to == AccountWidget.StudentViewStates.StudentView)
            {
                Response.Cookies.Add(new System.Web.HttpCookie(Context.StudentViewCookieKey, "true"));
            }
            else
            {
                if (Request.Cookies.AllKeys.Contains(Context.StudentViewCookieKey))
                {

                    var expired = new System.Web.HttpCookie(Context.StudentViewCookieKey) { Expires = DateTime.Now.GetCourseDateTime().AddDays(-1) };
                    Response.Cookies.Add(expired);
                }
            }

            if (Context.Course.CourseType.Equals(CourseType.LearningCurve.ToString(), StringComparison.InvariantCultureIgnoreCase))
                CorrectLearningCurveEnrollments();

            CacheProvider.InvalidateEnrollment(Context.CurrentUser.Id, null, Context.EntityId);

            return new JsonResult() { Data = Json(new { success = true }), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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

        #region Implementation
        /// <summary>
        /// PX-3906, somehow the instructor enrollment has problem so we need to validate and correct it if necessary
        /// If we found the cause why it occurs, this method can be removed.
        /// </summary>
        private void CorrectLearningCurveEnrollments()
        {
            if (Context.Course.CourseOwner != Context.CurrentUser.Id)
                return;
            int entityId = 0;
            if (int.TryParse(Context.EntityId, out entityId))
            {
                var instructorEnrollment = EnrollmentActions.GetEnrollment((entityId + 2).ToString());
                var studentEnrollment = EnrollmentActions.GetEnrollment((entityId + 3).ToString());
                if (!string.IsNullOrEmpty(instructorEnrollment.Id) && !string.IsNullOrEmpty(studentEnrollment.Id) && instructorEnrollment.User.Id != studentEnrollment.User.Id)
                {
                    var correctedUserId = GetCorrectUserId();
                    if (!string.IsNullOrEmpty(correctedUserId))
                    {
                        if (!instructorEnrollment.User.Id.Equals(correctedUserId, StringComparison.InvariantCultureIgnoreCase))
                        {
                            instructorEnrollment.User.Id = correctedUserId;
                            EnrollmentActions.UpdateEnrollment(instructorEnrollment);
                        }
                        if (!studentEnrollment.User.Id.Equals(correctedUserId, StringComparison.InvariantCultureIgnoreCase))
                        {
                            studentEnrollment.User.Id = correctedUserId;
                            EnrollmentActions.UpdateEnrollment(studentEnrollment);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the correct user id based on domain
        /// </summary>
        /// <returns></returns>
        private string GetCorrectUserId()
        {
            var userInfo = UserActions.GetUserByReferenceAndDomainId(Context.CurrentUser.Username, Context.Domain.Id);
            return userInfo == null ? null : userInfo.Id;
        }
        /// <summary>
        /// This method helps to find the Instructor Dashboard Id from database using Dashboard Actions
        /// </summary>
        /// <param name="model"></param>
        private void GetInstructorDashboard(Models.AccountWidget model)
        {
            if (Context.AccessLevel != null && Context.AccessLevel == BizSC.AccessLevel.Instructor && !string.IsNullOrEmpty(Context.ProductCourseId) && Context.CurrentUser != null && !string.IsNullOrEmpty(Context.CurrentUser.Username))
            {
                model.InstructorDashboardCourseId = Context.Course.ProductCourseId;
            }
        }

        /// <summary>
        /// Read helpurl from web.config
        /// </summary>
        /// <param name="courseType"></param>
        /// <param name="userAccessLevel"></param>
        /// <returns></returns>
        private string GetHelpUrl(string courseType, string userAccessLevel)
        {
            if (courseType.Equals(Biz.DataContracts.CourseType.LearningCurve.ToString())
                && userAccessLevel.Equals(Biz.ServiceContracts.AccessLevel.Instructor.ToString()))
            {
                return ConfigurationManager.AppSettings["LearningCurveInstructorHelpUrl"].ToString();
            }
            else if (courseType.Equals(Biz.DataContracts.CourseType.LearningCurve.ToString())
                     && userAccessLevel.Equals(Biz.ServiceContracts.AccessLevel.Student.ToString()))
            {
                return ConfigurationManager.AppSettings["LearningCurveStudentHelpUrl"].ToString();
            }
            else if (courseType.Equals(Biz.DataContracts.CourseType.XBOOK.ToString())
                     && userAccessLevel.Equals(Biz.ServiceContracts.AccessLevel.Instructor.ToString()))
            {
                return ConfigurationManager.AppSettings["XbookInstructorHelpUrl"].ToString();
            }
            else if (courseType.Equals(Biz.DataContracts.CourseType.XBOOK.ToString())
                     && userAccessLevel.Equals(Biz.ServiceContracts.AccessLevel.Student.ToString()))
            {
                return ConfigurationManager.AppSettings["XbookStudentHelpUrl"].ToString();
            }
            else if (courseType.Equals(Biz.DataContracts.CourseType.FACEPLATE.ToString())
                     && userAccessLevel.Equals(Biz.ServiceContracts.AccessLevel.Instructor.ToString()))
            {
                return ConfigurationManager.AppSettings["LaunchpadInstructorHelpUrl"].ToString();
            }
            else if (courseType.Equals(Biz.DataContracts.CourseType.FACEPLATE.ToString())
                     && userAccessLevel.Equals(Biz.ServiceContracts.AccessLevel.Student.ToString()))
            {
                return ConfigurationManager.AppSettings["LaunchpadStudentHelpUrl"].ToString();
            }
            else if ((courseType.Equals(Biz.DataContracts.CourseType.Eportfolio.ToString()) || 
                      courseType.Equals(Biz.DataContracts.CourseType.PersonalEportfolioDashboard.ToString()))
                     && userAccessLevel.Equals(Biz.ServiceContracts.AccessLevel.Student.ToString()))
            {
                return ConfigurationManager.AppSettings["EportfolioStudentHelpUrl"].ToString();
            }
            else if ((courseType.Equals(Biz.DataContracts.CourseType.Eportfolio.ToString()) || 
                      courseType.Equals(Biz.DataContracts.CourseType.EportfolioDashboard.ToString()))
                    && userAccessLevel.Equals(Biz.ServiceContracts.AccessLevel.Instructor.ToString()))
            {
                return ConfigurationManager.AppSettings["EportfolioInstructorHelpUrl"].ToString();
            }

            return null;
        }

        #endregion Implementation
    }
}
