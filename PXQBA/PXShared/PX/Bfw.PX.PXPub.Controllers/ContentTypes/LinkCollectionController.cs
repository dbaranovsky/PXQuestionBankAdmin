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
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class LinkCollectionController : Controller
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
        protected IAssignmentCenterHelper AssignmentCenterHelper { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkCollectionController"/> class.
        /// </summary>
        /// <param name="context">The context. </param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="helper">The helper.</param>
        public LinkCollectionController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, IContentHelper helper, IAssignmentCenterHelper assignmentCenterHelper)
        {
            Context = context;
            ContentActions = contActions;
            ContentHelper = helper;
            AssignmentCenterHelper = assignmentCenterHelper;
        }

        /// <summary>
        /// Adds a link to the Link Collection and stores it in Agilix.
        /// </summary>
        /// <param name="content">The content. <see cref="LinkCollection"/></param>
        /// <param name="id">The id.</param>
        /// <param name="linkTitle">The link title.</param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="linkUrl">The link URL.</param>
        /// <returns>The view representing the link being added.</returns>
        public ActionResult AddLinkToCollection(LinkCollection content, string id, string linkTitle, string behavior, 
            string linkUrl, bool? isBeingEdited, string toc = "syllabusfilter")
        {
            ActionResult result = null;
            ContentView model = null;

            switch (behavior.ToLowerInvariant())
            {
                case "save":
                    if (string.IsNullOrEmpty(linkTitle) && !string.IsNullOrEmpty(linkUrl))
                        ModelState.AddModelError("linkTitle", "You must specify a title");

                    if (ModelState.IsValid)  
                    {
                        ContentHelper.StoreLink(id, linkTitle, linkUrl, Context.EntityId);
                    }
                    var collection = ContentHelper.LoadContentView(id, ContentViewMode.Edit, true, toc).Content as LinkCollection;
            
                    return View("LinkList", collection);                
                    break;

                default:
                    result = View();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Removes a link from Agilix.
        /// </summary>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="linkIds">The link ids.</param>
        /// <returns></returns>
        public ActionResult RemoveLinksFromCollection(string collectionId, string[] linkIds, bool? isBeingEdited,
            string toc)
        {

            if (linkIds != null &&  linkIds.Length > 0)
            {
                foreach (string id in linkIds)
                {
                    ContentActions.RemoveContent(Context.EntityId, id);
                }
            }

            var collection = ContentHelper.LoadContentView(collectionId, ContentViewMode.Edit, true, toc).Content as LinkCollection;
            return View("LinkList", collection);
        }

        /// <summary>
        /// Saves a link to Agilix.
        /// </summary>
        /// <param name="content">The content. <see cref="LinkCollection"/></param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="linkTitle">The link title.</param>
        /// <param name="linkUrl">The link URL.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SaveLinkCollection(LinkCollection content, string behavior, string linkTitle, string linkUrl, Assign assign,
            string toc = "syllabusfilter")
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

                    if (string.IsNullOrEmpty(linkTitle) && !string.IsNullOrEmpty(content.linkUrl))
                    {
                        ModelState.AddModelError("linkTitle", "You must specify a title");
                    }

                    if (ModelState.IsValid)
                    {
                        ContentHelper.StoreLinkCollection(content, content.Title, content.linkUrl, Context.EntityId);
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
        /// Renders list of links for a collection
        /// </summary>
        /// <param name="collection">Already populated list</param>
        /// <param name="collectionId">id used to retrieve the collection in case the one passed in isn't populated</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public PartialViewResult LinkList(LinkCollection collection, string collectionId, string toc = "syllabusfilter")
        {
            if (collection == null || collection.Links.Count == 0)
            {
                collection = ContentHelper.LoadContentView(collectionId, ContentViewMode.Edit, true, toc).Content as LinkCollection;
            }

            return PartialView(collection);
        }
    }
}
