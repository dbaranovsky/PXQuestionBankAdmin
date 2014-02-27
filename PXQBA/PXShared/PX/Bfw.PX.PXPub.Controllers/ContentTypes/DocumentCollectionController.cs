using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.Common;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.IO;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers.ContentTypes
{
    [PerfTraceFilter]
    public class DocumentCollectionController : Controller
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
        protected IContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the Document converter.
        /// </summary>
        /// <value>
        /// The  Document converter.
        /// </value>
        protected BizSC.IDocumentConverter DocumentConverter
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<BizSC.IDocumentConverter>();
            }
        }


        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<BizSC.IEnrollmentActions>();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentCollectionController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contActions">The cont actions.</param>
        /// <param name="helper">The helper.</param>
        public DocumentCollectionController(BizSC.IBusinessContext context, BizSC.IContentActions contActions, IContentHelper helper)
        {
            Context = context;
            ContentActions = contActions;
            ContentHelper = helper;
        }


        /// <summary>
        /// Saves an uploaded document to Agilix.
        /// </summary>
        /// <param name="content">The content. <see cref="DocumentCollection"/></param>
        /// <param name="behavior">The behavior.</param>
        /// <param name="docTitle">The doc title.</param>
        /// <param name="docFile">The doc file. <see cref="System.Web.HttpPostedFileBase"/></param>
        /// <param name="hasParentLesson">The has parent lesson.</param>
        /// <returns>Returns a view representing the saved document collection</returns>
        [ValidateInput(false)]
        public ActionResult SaveDocumentCollection(DocumentCollection content, string behavior, string docTitle,
            System.Web.HttpPostedFileBase docFile, string hasParentLesson, string toc = "syllabusfilter")
        {
            ActionResult result = null;
            ContentView model = null;

            if (string.IsNullOrEmpty(behavior))
            {
                return new EmptyResult();
            }

            switch (behavior.ToLowerInvariant())
            {
                case "cancel":
                    var idToLoad = string.IsNullOrEmpty(content.Id) ? content.ParentId : content.Id;
                    model = ContentHelper.LoadContentView(idToLoad, ContentViewMode.Preview, false, toc);
                    result = View("DisplayItem", model);
                    break;

                case "save":
                    if (string.IsNullOrEmpty(docTitle) && docFile != null)
                    {
                        ModelState.AddModelError("docTitle", "You must specify a title");
                    }

                    if (!string.IsNullOrEmpty(docTitle) && docFile == null)
                    {
                        ModelState.AddModelError("docFile", "You must specify file to upload");
                    }

                    if (ModelState.IsValid)
                    {
                        ContentHelper.StoreDocumentCollection(content, docTitle, docFile, Context.EntityId);
                        model = ContentHelper.LoadContentView(content.Id, ContentViewMode.Edit, false);

                        ViewData["isScriptLoaded"] = true;

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

                    ViewData["hasParentLesson"] = hasParentLesson;
                    break;

                default:
                    result = View();
                    break;
            }

            return result;
        }

        /// <summary>
        /// Renders list of documents for a collection
        /// </summary>
        /// <param name="collection">Already populated list</param>
        /// <param name="collectionId">id used to retrieve the collection in case the one passed in isn't populated</param>
        /// <returns></returns>
		[ValidateInput(false)]
        public PartialViewResult DocumentList(DocumentCollection collection, string collectionId, 
            string toc = "syllabusfilter")
        {
            if (collection == null || collection.Documents.Count == 0)
            {
                collection = ContentHelper.LoadContentView(collectionId, ContentViewMode.Edit, true, toc).Content as DocumentCollection;
            }

            return PartialView(collection);
        }

        /// <summary>
        /// Removes a document item from Agilix.
        /// </summary>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="docIds">The doc ids.</param>
        /// <returns>Returns a view representing the Document Collection</returns>
        public ActionResult RemoveDocumentsFromCollection(string collectionId, string[] docIds)
        {
            if (docIds != null && docIds.Length > 0)
            {
                foreach (string id in docIds)
                {
                    ContentActions.RemoveContent(Context.EntityId, id);
                }
            }

            return RedirectToAction("DocumentList", new { collectionId = collectionId });
        }

        /// <summary>
        /// Returns edit view for the document collection
        /// </summary>
        /// <param name="collectionId"></param>
        /// <returns></returns>
        public ActionResult EditView(string collectionId, bool? isEdit = true, string title = "", string subTitle = "",
            string description = "", string toc = "syllabusfilter")
        {            
            ContentView model = ContentHelper.LoadContentView(collectionId, ContentViewMode.Edit, true, toc);
            model.IncludeNavigation = false;
            model.Content.IsBeingEdited = (bool)isEdit;

            ViewData["courseType"] = Context.Course.CourseType;

            if (!title.IsNullOrEmpty())
            {
                model.Content.Title = title;
            }
            if (!subTitle.IsNullOrEmpty())
            {
                model.Content.SubTitle = subTitle;
            }
            if (!description.IsNullOrEmpty())
            {
                model.Content.Description = description;
            }

            if (!(bool)isEdit)
            {
                ContentHelper.StoreDocumentCollection(model.Content as DocumentCollection, "", null, Context.EntityId);
            }

            return View("DisplayItem", model);
        }
    }
}
