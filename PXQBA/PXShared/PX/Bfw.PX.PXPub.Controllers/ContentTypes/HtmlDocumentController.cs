using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class HtmlDocumentController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context. Folder 
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to a content helper object
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlDocumentController"/> class.
        /// </summary>
        /// <param name="context">The context. </param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="helper">The helper.</param>
        public HtmlDocumentController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, ContentHelper helper, AssignmentCenterHelper assignmentCenterHelper)
        {
            Context = context;
            ContentActions = contActions;
            ContentHelper = helper;
            AssignmentCenterHelper = assignmentCenterHelper;
        }

        /// <summary>
        /// Attempts to save the Html Document, if behavior is Cancel then nothing is saved and
        /// the action is redirected to CreateAndAssign.
        /// </summary>
        /// <param name="content">The content. <see cref="HtmlDocument"/></param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>Returns a view of the representing the HTML content.</returns>
        [ValidateInput(false)]
        public ActionResult SaveHtmlDocument(HtmlDocument content, string behavior, Assign assign, string toc = "syllabusfilter")
        {
            ActionResult result = null;
            ContentView model = null;

            switch (behavior.ToLowerInvariant())
            {
                case "cancel":
                    var idToLoad = string.IsNullOrEmpty(content.Id) ? content.ParentId : content.Id;
                    model = ContentHelper.LoadContentView(idToLoad, ContentViewMode.Preview, false, toc);
                    result = View("DisplayItem", model);
                    break;

                case "save":
                    if ( ModelState.IsValid )
                    {
                        ContentHelper.StoreHtmlDocument(content, Context.EntityId);
                        if (assign != null && assign.DueDate.Year > DateTime.MinValue.Year)
                        {
                            assign.Id = content.Id;
                            AssignmentCenterHelper.AssignItem(assign, toc);
                        }
                        model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Preview, false, toc);
                        result = View("DisplayItem", model);
                    }
                    else 
                    {
                        content.EnvironmentUrl = Context.EnvironmentUrl;
                        content.CourseInfo = Context.Course.ToCourse();
                        content.EnrollmentId = Context.EnrollmentId;
                        content.Status = string.IsNullOrEmpty( content.Id ) ? ContentStatus.New : ContentStatus.Existing;
                        content.Body = System.Web.HttpUtility.HtmlDecode( content.Body );
                        result = View( "CreateContent", content );
                    }
                    break;

                default:
                    result = View();
                    break;
            }

            return result;
        }
    }
}
