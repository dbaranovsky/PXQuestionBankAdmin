using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Bfw.Common;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Helpers;

using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Web.Routing;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Generic widget to hold the brainhoney components
    /// </summary>
    public class XbookWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; } 

        /// <summary>
        /// widget's pubic constructor
        /// </summary>
        /// <param name="context"></param>
        public XbookWidgetController(BizSC.IBusinessContext context, BizSC.IEnrollmentActions enrollmentActions)
        {
            Context = context;
            EnrollmentActions = enrollmentActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Summary view for the BrainHoney component widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            string sComponent = "";
            var parameters = new Dictionary<string, string>();
            foreach (string str in widget.BHProperties.Keys)
            {
                sComponent = str;
                foreach (var parm in widget.BHProperties[str].Parameters)
                {
                    switch (parm.Key.ToUpper())
                    {
                        case "ENROLLMENTID":
                            var enrollmentId = string.Empty;
                            var enrollment = EnrollmentActions.GetEnrollment(Context.CurrentUser.Id, Context.EntityId);
                            if (enrollment != null)
                            {
                                enrollmentId = enrollment.Id;
                            }
                            parameters.Add(parm.Key, enrollmentId);
                            break;
                        case "BRAINHONEYURL":
                            parameters.Add(parm.Key, HttpUtility.UrlEncode(ConfigurationManager.AppSettings["BrainHoneyUrl"]));
                            break;
                        case "ASSIGNMENTID":
                            parameters.Add(parm.Key, "");
                            break;
                        case "DISPLAYASSIGNMENT":
                            parameters.Add(parm.Key, "");
                            break;
                        case "DISPLAYHIDESHOW":
                            parameters.Add(parm.Key, "");
                            break;
                        case "FRAGMENTID":
                            parameters.Add(parm.Key, "");
                            break;
                        case "PRODUCTCOURSEID":
                            parameters.Add(parm.Key, Context.ProductCourseId);
                            break;
                        case "PXAPPPATH":
                            var fullurl = Request.Url.AbsoluteUri;
                            var index_of_courseid = fullurl.IndexOf(Context.CourseId);
                            var url_length = fullurl.Length;

                            // remove all text after the '/xbook/' part of the url
                            var _url = fullurl.Remove(index_of_courseid, url_length - index_of_courseid);

                            parameters.Add(parm.Key, _url);
                            break;
                            
                        default:
                            parameters.Add(parm.Key, parm.Value.Value);
                            break;
                    }
                }
            }

            //add the css override parameter
            var url = new UrlHelper(ControllerContext.RequestContext);
            var cssUrl = url.RouteUrl("CourseStyleCourseCss", new { 
                component = sComponent, 
                courseProductName = RouteData.Values["course"].ToString(), 
                courseType= Context.Course.CourseType
            }, Request.Url.Scheme);

            parameters.Add("CssOverride", System.Web.HttpUtility.UrlEncode(cssUrl));

            parameters.Add("DlapCookie", Context.BhAuthCookieValue);
            parameters.Add("StudentOverride", Context.ImpersonateStudent.ToString().ToUpper());

            return View("~/Views/Shared/XbIFrameComponent.ascx",
                new BhComponent { Id = widget.Abbreviation, ComponentName = sComponent, Parameters = parameters });
        }

        /// <summary>
        /// View All option for the BrainHoney component widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget widget)
        {
            return View();
        }

        #endregion
    }
}
