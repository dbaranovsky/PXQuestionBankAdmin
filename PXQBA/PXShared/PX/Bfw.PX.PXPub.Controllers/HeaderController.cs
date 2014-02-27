using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.Common;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;

using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{

    [PerfTraceFilter]
    public class HeaderController : Controller
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// The user actions.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public HeaderController(BizSC.IBusinessContext context, BizSC.IPageActions pageActions, BizSC.IUserActions userActions)
        {
            Context = context;
            PageActions = pageActions;
            UserActions = userActions;
        }

        /// <summary>
        /// Courses the header.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns></returns>
        public ActionResult CourseHeader(string menuName)
        {
            // If this is not a product course, then give a model to the view
            if (!Context.CourseIsProductCourse)
            {
                var model = new CourseHeader() { CourseTitle = Context.Course.Title, InstructorName = Context.Course.InstructorName, CourseType = Context.Course.CourseType, ApplicationJSFile = Context.Course.ApplicationJSFiles };
                ViewData.Model = model;
            }

            return View();
        }

        
        /// <summary>
        /// Homes the page course header.
        /// </summary>        
        /// <returns></returns>
        public ActionResult StartPageCourseHeader()
        {
            var pageDefinitionId = string.Empty;
            pageDefinitionId = (string.IsNullOrEmpty(Context.Course.CourseStartPage) || Context.Course.CourseStartPage.ToLowerInvariant() == "index") ? "HOME" : Context.Course.CourseStartPage;
            var pageDefinition = this.PageActions.LoadPageDefinition(pageDefinitionId);            

            var model = new CourseHeader();
            model.IsAllowPageEdit = false;
            model.IsProductCourse = Context.CourseIsProductCourse;
            model.ApplicationJSFile = Context.Course.ApplicationJSFiles;
            
            if (!Context.CourseIsProductCourse)
            {
                model.CourseTitle = Context.Course.Title;
                Biz.DataContracts.UserInfo user = UserActions.GetUser(Context.Course.CourseOwner);
                if (Context.Course.CourseOwner == "")
                {
                    model.InstructorName = string.Format("{0} {1}", Context.CurrentUser.FirstName, Context.CurrentUser.LastName);

                }
                else
                {
                    model.InstructorName = string.Format("{0} {1}", user.FirstName, user.LastName);
                }
                model.IsAllowPageEdit = (Context.AccessLevel == BizSC.AccessLevel.Instructor) && pageDefinition.IsEditable;
                model.CourseType = Context.Course.CourseType.ToString();
                model.InstructorEmail = Context.CurrentUser.Email;
                model.PublishedStatus = Context.Course.PublishedStatus;
                model.CourseId = Context.Course.Id;
                model.CourseDescription = Context.Course.CourseDescription;
                model.CourseAuthor = Context.Course.CourseAuthor;

                BuildTimezoneData(model);

                

                try
                {
                    model.ProductCourseTitle = Context.Product.Title;
                }
                catch { }
            }
            var previewAsVisitorCookie = Request.Cookies[Context.PreviewAsVisitorCookieKey];
            var isPublicView = Context.IsPublicView;
            if (previewAsVisitorCookie != null)
            {
                isPublicView = true;
            }
            ViewData["IsPublicView"] = isPublicView;
            
            if (Context.Course.CourseType == Biz.DataContracts.CourseType.PersonalEportfolioPresentation.ToString())
            {
                string bfw_shared = string.Empty;
                if (Context.Course.Properties.ContainsKey("bfw_shared"))
                {
                    bfw_shared = Context.Course.Properties["bfw_shared"].Value.ToString().ToLowerInvariant();
                }

                if (bfw_shared == "public" && !Context.IsAnonymous && previewAsVisitorCookie == null)
                {
                    ViewData["ShowPreviewAsVisitor"] = true;
                }
                else if (bfw_shared == "public" && !Context.IsAnonymous && previewAsVisitorCookie != null)
                {
                    ViewData["ShowResumeEditing"] = true;
                }
            }

            return View("~/Views/Shared/StartPageCourseHeader.ascx",model);
        }

        /// <summary>
        /// Homes the page course header.
        /// </summary>        
        /// <returns></returns>
        public ActionResult HomePageCourseHeader()
        {
            var pageDefinitionId = (string.IsNullOrEmpty(Context.Course.CourseHomePage) || Context.Course.CourseHomePage.ToLowerInvariant() == "index") ? "HOME" : Context.Course.CourseHomePage;
            var pageDefinition = this.PageActions.LoadPageDefinition(pageDefinitionId);            

            var model = new CourseHeader();
            model.IsAllowPageEdit = false;
            model.IsProductCourse = Context.CourseIsProductCourse;
            model.ApplicationJSFile = Context.Course.ApplicationJSFiles;
            
            if (!Context.CourseIsProductCourse)
            {
                model.CourseTitle = Context.Course.Title;
                Biz.DataContracts.UserInfo user = UserActions.GetUser(Context.Course.CourseOwner);
                if (Context.Course.CourseOwner == "")
                {
                    model.InstructorName = string.Format("{0} {1}", Context.CurrentUser.FirstName, Context.CurrentUser.LastName);
                    model.InstructorEmail = Context.CurrentUser.Email;

                }
                else
                {
                    model.InstructorName = string.Format("{0} {1}", user.FirstName, user.LastName);
                    model.InstructorEmail = user.Email;

                }
                model.CourseDisciplineAbbreviation = Context.Course.CourseDisciplineAbbreviation;
                model.IsAllowPageEdit = (Context.AccessLevel == BizSC.AccessLevel.Instructor) && (pageDefinition != null && pageDefinition.IsEditable);
                model.CourseType = Context.Course.CourseType.ToString();
                model.PublishedStatus = Context.Course.PublishedStatus;
                model.CourseId = Context.Course.Id;
                model.CourseDescription = Context.Course.CourseDescription;
                model.CourseAuthor = Context.Course.CourseAuthor;


                BuildTimezoneData(model);



                try
                {
                    model.ProductCourseTitle = Context.Product.Title;
                }
                catch { }
            }
            var previewAsVisitorCookie = Request.Cookies[Context.PreviewAsVisitorCookieKey];
            var isPublicView = Context.IsPublicView;
            if (previewAsVisitorCookie != null)
            {
                isPublicView = true;
            }
            ViewData["IsPublicView"] = isPublicView;
            ViewData["IsSharedCourse"] = Context.IsSharedCourse;            
            model.CourseSubType = Context.Course.CourseSubType;
            model.CourseType = Context.Course.CourseType;

            return View(model);
        }

        private void BuildTimezoneData(CourseHeader model)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(Context.Course.CourseTimeZone);
            var adjustment = tz.GetAdjustment(DateTime.Now.Year);
            model.TimeZoneStandardOffset = (int)tz.BaseUtcOffset.TotalMinutes;
            if (adjustment != null)
            {
                model.TimeZoneDaylightOffset = (int)(adjustment.DaylightDelta.TotalMinutes + tz.BaseUtcOffset.TotalMinutes);
                
                model.TimeZoneDaylightStartTime = adjustment.DaylightTransitionStart
                    .GetTransitionInfo(DateTime.Now.Year)
                    .ToUniversalTime();
                model.TimeZoneStandardStartTime = adjustment.DaylightTransitionEnd
                    .GetTransitionInfo(DateTime.Now.Year)
                    .ToUniversalTime();

                model.TimeZoneDaylightStartTimeNextYear = adjustment.DaylightTransitionStart
                    .GetTransitionInfo(DateTime.Now.AddYears(1).Year)
                    .ToUniversalTime();
                model.TimeZoneStandardStartTimeNextYear = adjustment.DaylightTransitionEnd
                    .GetTransitionInfo(DateTime.Now.AddYears(1).Year)
                    .ToUniversalTime();
            }

            model.TimeZoneAbbreviation = Context.Course.GetCourseTimeZoneAbbreviation();
        }

        /// <summary>
        /// Homes the page course header.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns></returns>
        public ActionResult EcomHomePageCourseHeader(string menuName)
        {
            var model = new CourseHeader();
            model.IsAllowPageEdit = false;
            model.IsProductCourse = Context.CourseIsProductCourse;
            model.ApplicationJSFile = Context.Course.ApplicationJSFiles;
            model.CourseDisciplineAbbreviation = Context.Course.CourseDisciplineAbbreviation;

            if (!Context.CourseIsProductCourse)
            {
               // model.CourseTitle = Context.Course.Title;
               // model.InstructorName = Context.Course.InstructorName;
             //   model.IsAllowPageEdit = (Context.AccessLevel == BizSC.AccessLevel.Instructor);
                model.CourseType = Context.Course.CourseType.ToString();
               // model.InstructorEmail = Context.CurrentUser.Email;
                model.PublishedStatus = Context.Course.PublishedStatus;
                model.CourseId = Context.Course.Id;

                BuildTimezoneData(model);

				if (Context.Product!=null) model.ProductCourseTitle = Context.Product.Title;               
            }

           
            ViewData["Authenticated"] = Context.CurrentUser != null ? true : false;
            model.CourseSubType = Context.Course.CourseSubType;
           
            return View(model);
        }
    }
}
