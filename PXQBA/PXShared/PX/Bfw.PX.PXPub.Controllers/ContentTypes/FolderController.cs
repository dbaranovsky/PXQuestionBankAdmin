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
    public class FolderController : Controller
    {
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context. 
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
        /// Access to a content helper object.
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
        /// Initializes a new instance of the <see cref="FolderController"/> class.
        /// </summary>
        /// <param name="context">The context. </param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="helper">The helper.</param>
        public FolderController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, ContentHelper helper, AssignmentCenterHelper assignmentCenterHelper)
        {
            Context = context;
            ContentActions = contActions;
            ContentHelper = helper;
            AssignmentCenterHelper = assignmentCenterHelper;
        }

        /// <summary>
        /// Saves a new folder or updates and existing one, and its description.
        /// </summary>
        /// <param name="content">The content. </param>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        public ActionResult SaveFolder(Folder content, string behavior, Assign assign, string toc = "syllabusfilter")
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

                    if (ModelState.IsValid)
                    {
                        ContentHelper.StoreFolder(content, Context.EntityId);
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
                        content.Status = string.IsNullOrEmpty(content.Id) ? ContentStatus.New : ContentStatus.Existing;
                        content.Description = System.Web.HttpUtility.HtmlDecode(content.Description);
                        result = View("CreateContent", content);
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
    