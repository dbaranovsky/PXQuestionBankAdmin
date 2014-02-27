using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using TocCategory = Bfw.PX.PXPub.Models.TocCategory;
using Resource = Bfw.PX.Biz.DataContracts.Resource;
using Bfw.Common;
using Bfw.PX.PXPub.Components;

namespace Bfw.PX.PXPub.Controllers {
    [PerfTraceFilter]
    public class CourseMaterialsController : Controller
    {

        #region Properties

        /// <summary>
        /// Access to an INavigationActions implementation.
        /// </summary>
        protected INavigationActions NavigationActions { get; set; }
        /// <summary>
        /// Content Actions
        /// </summary>
        protected IContentActions ContentActions { get; set; }
        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }
        /// <summary>
        /// Access to an IUserActions implementation.
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IUserActions UserActions { get; set; }

        protected ICourseMaterialsActions CourseMaterialsActions { get; set; }

        protected IPageActions PageActions { get; set; }

        protected IDocumentConverter DocumentConverter { get; set; }
        #endregion

        #region Constructors

        /// <summary>
        /// Constructs 
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="userActions">The user actions.</param>
        /// <param name="contentHelper">Context Helper</param>
        public CourseMaterialsController(BizSC.IBusinessContext context, BizSC.IUserActions userActions, ContentHelper contentHelper,
            IContentActions contentActions, ICourseMaterialsActions courseMaterialsActions, INavigationActions navActions, IDocumentConverter docConverter)
        {
            Context = context;
            UserActions = userActions;
            ContentHelper = contentHelper;
            ContentActions = contentActions;
            CourseMaterialsActions = courseMaterialsActions;
            NavigationActions = navActions;
            DocumentConverter = docConverter;
        }

        #endregion

        #region Methods

        public ActionResult Index()
        {
            var cmModel = CourseMaterialsActions.GetCourseMaterials().ToCourseMaterials();
            cmModel.AccessLevel = Context.AccessLevel;
            ViewData["CourseType"] = Context.Course.CourseType.ToString();
            ViewData["AccessLevel"] = Context.AccessLevel.ToString();

            if (Context.IsCourseReadOnly)
                ViewData["IsReadOnly"] = true;
            return PartialView("Index", cmModel);
        }

        public ActionResult ComposeMaterials()
        {
            HtmlDocument newDoc = new HtmlDocument();
            if (Context.Course.Categories.Count > 0 && Context.Course.Categories.FirstOrDefault() != null)
            {
                TocCategory toc = new TocCategory { Id = "course_materials", ItemParentId = "PX_COURSE_MATERIALS", Sequence = "", Active = true, Text = "Course Materials" };
                newDoc.Categories = new List<TocCategory>() { toc };
            }

            newDoc.ParentId = "PX_COURSE_MATERIALS";
            ViewData["DisableButtonOnSaved"] = true;

            return PartialView("~/Views/Shared/EditorTemplates/HtmlDocument.ascx", newDoc);
        }

        public ActionResult UploadMaterials()
        {
            return PartialView("Index");
        }

        public ActionResult SaveHtmlDocument(HtmlDocument Content)
        {
            if (!string.IsNullOrEmpty(Content.Body) && !Content.Body.EndsWith("&lt;br /&gt;&lt;br /&gt;"))
            {
                Content.Body += "&lt;br /&gt;&lt;br /&gt;";
            }
            Content.SubmittedDate = Context.Course.UtcRelativeAdjust(DateTime.Now);
            Content.ParentId = "PX_COURSE_MATERIALS";
            Content.ExtendedProperties.Add("PX_COURSE_MATERIALS_STORAGE_TYPE", "HtmlOnly");

            var documentCreateDate = Context.Course.UtcRelativeAdjust(DateTime.Now);
            Content.ExtendedProperties.Add("DOCUMENT_CREATED_DATE", documentCreateDate);

            if (Context.Course.Categories.Count > 0 && Context.Course.Categories.FirstOrDefault() != null)
            {
                TocCategory toc = new TocCategory
                {
                    Id = "course_materials",
                    ItemParentId = "PX_COURSE_MATERIALS",
                    Sequence = "",
                    Active = true,
                    Text = "Course Materials"
                };
                Content.Categories = new List<TocCategory>() { toc };
                if (!string.IsNullOrEmpty(Content.Id))
                {
                    var contentItem = ContentActions.GetContent(Context.EntityId, Content.Id);
                    if (contentItem.Subtype == "UploadOrCompose")
                    {
                        TocCategory bfwtoc = new TocCategory
                        {
                            Id = "bfw_toc_contents",
                            ItemParentId = "PX_COURSE_MATERIALS",
                            Sequence = "",
                            Active = true,
                            Text = "Course Materials"
                        };
                        Content.Categories = new List<TocCategory>() { bfwtoc };
                    }
                }
            }
            ContentHelper.StoreHtmlDocument(Content, Context.EntityId);

            var results = ContentActions.ListChildren(Context.EntityId, "PX_COURSE_MATERIALS", 2, "course_materials").ToList();
            List<Bfw.PX.Biz.DataContracts.ContentItem> list = results.Select(contentItem => new ContentItem { Title = contentItem.Title, Href = contentItem.Href, Id = contentItem.Id }).ToList();

            var cmModel = CourseMaterialsActions.GetCourseMaterials().ToCourseMaterials();
            cmModel.AccessLevel = Context.AccessLevel;
            return View("CourseMaterialResourceList", cmModel);
        }

        public ActionResult EditMaterials(string itemToEditId, string itemParentId)
        {
            var item = ContentActions.GetContent(Context.EntityId, itemToEditId);
            var res = ContentActions.GetResource(Context.EntityId, item.Href);
            StreamReader reader = new StreamReader(res.GetStream());
            string html = reader.ReadToEnd();

            try
            {
                if (string.IsNullOrEmpty(html) && !string.IsNullOrEmpty(itemParentId))
                {
                    //item = ContentActions.GetContent(Context.EntityId, itemParentId);
                    res = ContentActions.GetResource(Context.EnrollmentId, item.Href);

                    var stream = DocumentConverter.ConvertDocument(new Biz.DataContracts.DocumentConversion() { DataStream = res.GetStream(), FileName = res.Name == null ? string.Empty : res.Name, OutputType = Biz.DataContracts.DocumentOutputType.Html });
                    reader = new StreamReader(stream);
                    html = reader.ReadToEnd();
                }
            }
            catch { }


            HtmlDocument newDoc = new HtmlDocument { Title = item.Title, Body = html, Id = itemToEditId };

            var cats = new List<TocCategory>();
            foreach (var cat in item.Categories)
            {
                cats.Add(cat.ToTocCategory());
            }
            if (!cats.Exists(i => i.Id.ToLowerInvariant() == "course_materials"))
                cats.Add(new TocCategory { Id = "course_materials", ItemParentId = "PX_COURSE_MATERIALS", Sequence = "", Active = true, Text = "Course Materials" });
                
            newDoc.Categories = cats;
            newDoc.IsBeingEdited = true;
            return PartialView("~/Views/Shared/EditorTemplates/HtmlDocument.ascx", newDoc);
        }

        public ActionResult UpdateCourseMaterialResourceList()
        {
            var cmModel = CourseMaterialsActions.GetCourseMaterials().ToCourseMaterials();
            cmModel.AccessLevel = Context.AccessLevel;
            return View("CourseMaterialResourceList", cmModel);
        }

        public ActionResult DownloadCourseMaterialResource(string itemid)
        {            
            var item = ContentActions.GetContent(Context.EntityId, itemid);
            
            var res = ContentActions.GetResource(Context.EntityId, item.Href);

            var contentType = "text/html";

            if(item.Properties.ContainsKey("mimetype"))
            {
                contentType = item.Properties["mimetype"].Value.ToString();
            }

            return File(res.GetStream(), contentType, item.Title);
        }

        public JsonResult DeleteCourseMaterialsResource(string itemID)
        {
            CourseMaterialsActions.DeleteCourseMaterialResource(itemID);

            return Json(new { Result = "Deleted the resource successfully!" });
        }

        #endregion

    }
}
