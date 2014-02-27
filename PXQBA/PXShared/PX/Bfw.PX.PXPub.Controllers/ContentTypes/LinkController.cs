using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class LinkController : Controller
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
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkController"/> class.
        /// </summary>
        /// <param name="context">The context. </param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="helper">The helper.</param>
        public LinkController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, IContentHelper helper, AssignmentCenterHelper assignmentCenterHelper)
        {
            Context = context;
            ContentActions = contActions;
            ContentHelper = helper;
            AssignmentCenterHelper = assignmentCenterHelper;
        }

        /// <summary>
        /// Saves a link to Agilix.
        /// </summary>
        /// <param name="content">The content. <see cref="Link"/></param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="linkTitle">The link title.</param>
        /// <param name="linkUrl">The link URL.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SaveLink(Link content, string behavior, string linkUrl, Assign assign, string toc = "syllabusfilter")
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
                    if (string.IsNullOrEmpty(linkUrl))
                    {
                        ModelState.AddModelError("linkTitle", "You must specify url");
                    }

                    if (string.IsNullOrEmpty(content.Title))
                    {
                        ModelState.AddModelError("linkTitle", "You must specify a title");
                    }

                    if (ModelState.IsValid)
                    {
                        content.Url = linkUrl;

                        ContentHelper.StoreLink(content, Context.EntityId);

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

        /// <summary>
        /// Adds external links (from RSS feeds, etc.)
        /// </summary>
        /// <param name="link">The Url</param>
        /// <param name="title">Title of the Url</param>
        /// <param name="parentId">Parent Id of the container</param>
        /// <returns></returns>
        public ActionResult AddLink(string link, string title, string parentId)
        {
            Link content = new Link();
            content.Url = link;
            content.Title = title;
            content.ParentId = parentId;

            ContentHelper.StoreLink(content, Context.EntityId);

            return Json(new { Result = "SUCCESS", parentId = parentId, Id = content.Id });
        }
    }
}