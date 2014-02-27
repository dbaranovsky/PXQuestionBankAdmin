using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;


namespace Bfw.PX.PXPub.Controllers
{
    /// <summary>
    /// Provides actions necessary to support the Presentation Course header action bar
    /// </summary>
    [PerfTraceFilter]	
    public class PresentationCourseController : Controller
	{
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }
        /// <summary>
        /// The featured content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }
        /// <summary>
        ///  Gets or sets the EportfolioCourseActions actions.
        /// </summary>
        /// <value>
        /// The ePortfolioCourse actions.
        /// </value>
        protected BizSC.IEportfolioCourseActions EportfolioCourseActions { get; set; }
        /// <summary>
        /// Gets or sets the E portfolio actions.
        /// </summary>
        /// <value>
        /// The E portfolio actions.
        /// </value>
        protected BizSC.IEPortfolioActions EPortfolioActions { get; set; }
        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }
        /// <summary>
        /// Gets or sets the course actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationCourse"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The eportfolio Actions.</param>
        /// <param name="contentActions">The content Actions.</param>       
        /// <param name="eportfolioCourseActions">The eportfolioCourse Actions.</param>
        /// <param name="enrollmentActions">The enrollment Actions.</param>
        /// <param name="enrollmentActions">The course Actions.</param>
        public PresentationCourseController(
            BizSC.IBusinessContext context, 
            BizSC.IEPortfolioActions eportfolioActions, 
            BizSC.IContentActions contentActions, 
            BizSC.IEportfolioCourseActions eportfolioCourseActions, 
            BizSC.IEnrollmentActions enrollmentActions,
            BizSC.ICourseActions courseActions)
		{
			Context = context;		
            ContentActions = contentActions;
            EportfolioCourseActions = eportfolioCourseActions;
            EnrollmentActions = enrollmentActions;
            EPortfolioActions = eportfolioActions;
            CourseActions = courseActions;
		}

        /// <summary>
        /// Share Presentation Modal
        /// </summary>
        /// <returns></returns>
        public ActionResult SharePresentationModal()
		{            
            var baseUrl = Url.RouteUrl("EPortfolioBrowser", new { course = "eportfolio", section = "eportfolio", courseid = Context.CourseId }, Request.Url.Scheme);
            var hostname = string.IsNullOrEmpty(Context.AppDomainUrl) ? string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Host) : Context.AppDomainUrl;

            if (Request.Url.Host == "localhost")
            {
                hostname = @"http://www.whfreeman.com";
            }
            var aliasUrl = EPortfolioActions.GeneratePresentationUrlAlias(hostname, baseUrl);

            ViewData["CourseId"] = Context.Course.Id;
            ViewData["AliasUrl"] = aliasUrl;
            return View();
		}

        /// <summary>
        /// Share Presentation
        /// </summary>
        /// <returns></returns>
        public ActionResult SharePresentation(string emailAddresses)
        {
            bool result;
            if (!string.IsNullOrEmpty(emailAddresses))
            {
                var baseUrl = Url.RouteUrl("EPortfolioBrowser", new { course = "eportfolio", section = "eportfolio", courseid = Context.CourseId }, Request.Url.Scheme);
                var hostname = string.IsNullOrEmpty(Context.AppDomainUrl) ? string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Host) : Context.AppDomainUrl;

                if (Request.Url.Host == "localhost")
                {
                    hostname = @"http://www.whfreeman.com";
                } 
                
                var aliasUrl = EPortfolioActions.GeneratePresentationUrlAlias(hostname, baseUrl);

                if (aliasUrl != string.Empty)
                {
                    List<string> emailAddressList = new List<string>(emailAddresses.Split(','));
                    result = EPortfolioActions.SendPresentationShareEmail(emailAddressList.Distinct().ToList(), aliasUrl);
                }
                else
                    result = false;

            }
            else
                result = false;
       
            if(result)
                return Json(new { status = "success" });
            else
                /* return fail status if emailAddresses list / URL is empty*/
                return Json(new { status = "fail" });                 
        } 
	}
}
