using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    /// <summary>
    /// Controller used to control views related to item templates.
    /// </summary>
    [PerfTraceFilter]
    public class TemplateController : Controller
    {
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
        protected IContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }


        /// <summary>
        /// Access to an IGradeActions implementation
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Constructor to initialize
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="gradeActions">The grade actions.</param>
        /// <param name="contentHelper">The content helper.</param>
        public TemplateController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, BizSC.IGradeActions gradeActions, IContentHelper contentHelper)
        {
            Context = context;
            ContentActions = contentActions;
            GradeActions = gradeActions;
            ContentHelper = contentHelper;
        }

        /// <summary>
        /// Items the templates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public JsonResult ItemTemplates(TemplateContext.TemplateDisplayContext context)
        {
            var templates = GetTemplates(context);
            var jList = templates.ToList();
            return Json(jList);
        }

        /// <summary>
        /// Items from template.
        /// </summary>
        /// <param name="templateItemId">The template item id.</param>
        /// <returns></returns>
        public JsonResult ItemFromTemplate(string templateItemId, string parentId, string title)
        {
            if (Context.AccessLevel == Bfw.PX.Biz.ServiceContracts.AccessLevel.Student)
            {
                var item = ContentActions.GetContent(Context.EntityId, templateItemId);
                item.Id = Context.NewItemId();
                item.DefaultCategoryParentId = "";
                return Json(new { id = item.Id, title = item.Title });
            }
            else
            {
                var newItemId = Context.NewItemId();
                BizDC.ContentItem item = null;

                using (Context.Tracer.DoTrace("ItemFromTemplate(entityId = {0}, productid {1}, courseid = {2}", Context.EntityId, Context.ProductCourseId, Context.CourseId))
                {
                    var myMaterialsId = System.Configuration.ConfigurationManager.AppSettings["MyMaterials"];
                    var myMaterials = new Biz.DataContracts.TocCategory()
                    {
                        Id = myMaterialsId,
                        ItemParentId = string.Format("{0}_{1}", myMaterialsId, Context.CurrentUser.Id),
                        Sequence = "",
                        Text = myMaterialsId
                    };

                    item = ContentActions.GetContent(Context.EntityId, templateItemId);
                    newItemId = (!item.Subtype.IsNullOrEmpty() && (item.Subtype.ToLowerInvariant() == "pxunit" || item.Subtype.ToLowerInvariant() == "module")) ? "MODULE_" + newItemId : newItemId;

                    if (parentId.IsNullOrEmpty())
                    {
                        parentId = ContentActions.TemporaryFolder;
                    }

                    if (title.IsNullOrEmpty())
                    {
                        title = String.Format("{0}{1}", "Untitled ", item.Type == "Folder" && parentId != "PX_MULTIPART_LESSONS" ? item.Type : item.Title);
                    }

                    item = ContentActions.CopyItem(
                        Context.EntityId,
                        templateItemId,
                        newItemId,                        
                        parentId,
                        new List<Biz.DataContracts.TocCategory> { 
                            myMaterials
                        },
                        true,
                        title: title,
                        hiddenFromStudent: false
                    );

                    item.Template = templateItemId;
                }

                return Json(new { id = item.Id, title = item.Title });
            }
        }

        /// <summary>
        /// Copies the item settings.
        /// </summary>
        /// <param name="fromId">From id.</param>
        /// <param name="toId">To id.</param>
        /// <returns></returns>
        public JsonResult CopyItemSettings(string fromId, string toId)
        {
            ContentActions.CopyItemSettings(Context.EntityId, fromId, toId);
            return Json(new { status = "success" });
        }

        /// <summary>
        /// Templates the picker.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public ActionResult TemplatePicker(TemplateContext.TemplateDisplayContext context)
        {
            ViewData["TemplateDisplayContext"] = context;
            ViewData.Model = GetTemplates(context);
            return View();
        }

        /// <summary>
        /// Templates the management.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public ActionResult TemplateManagement(TemplateContext.TemplateDisplayContext context)
        {
            ViewData.Model = GetTemplates(context);
            return View();
        }

        /// <summary>
        /// For a given item type, return templates that have compatible settings.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        public JsonResult GetRelatedTemplates(TemplateContext.TemplateDisplayContext context, string itemId)
        {
            var relatedTemplates = ContentActions.FindRelatedTemplates(itemId);
            return Json(relatedTemplates.Map(rt => rt.ToContentItem(ContentActions).ToTemplate()));
        }

        /// <summary>
        /// Gets the template info.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns></returns>
        public ActionResult GetTemplateInfo(string itemId)
        {
            // return some basic data about the template
            var returnData = new Dictionary<string, string>();
            var newItem = ContentActions.GetContent(Context.EntityId, itemId);
            BizDC.Resource nr = ContentActions.GetResource(Context.EntityId, newItem.Href);
            string str1 = "";

            try
            {
                using (var sw = new System.IO.StreamReader(nr.GetStream())) str1 = sw.ReadToEnd();
            }
            catch
            {
            }

            var model = ContentActions.GetContent(Context.EntityId, itemId, true).ToContentItem(ContentActions);
            returnData.Add("title", newItem.Title);
            returnData.Add("description", str1.ToString());
            StringBuilder sb1 = new StringBuilder();
            sb1.Append("{\"title\":\"" + newItem.Title + "\",\"description\":\"" + str1.ToString() + "\"}");
            str1 = "{\"items\": [" + sb1.ToString() + "]}";
            return Content(str1);
        }

        public ActionResult TemplateManagementDetails(string itemId)
        {
            ViewData.Model = ContentActions.GetContent(Context.EntityId, itemId).ToContentItem(ContentActions);
            ViewData["EnrollmentId"] = Context.EnrollmentId;
            return View();
        }

        /// <summary>
        /// Gets the templates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private IEnumerable<Template> GetTemplates(TemplateContext.TemplateDisplayContext context)
        {
            var items = ContentActions.GetAllTemplates();
            switch (context)
            {
                case TemplateContext.TemplateDisplayContext.Content:
                    items = items.Where(i => i.Subtype != "PxUnit" && !i.HiddenFromToc);
                    break;
                case TemplateContext.TemplateDisplayContext.FacePlate:
                    items = items.Where(i => i.Type != "RssFeed");
                    break;
                case TemplateContext.TemplateDisplayContext.Xbook:
					items = items.Where(i => i.Subtype == "DocumentCollection" || (i.Type == "Folder" && i.Subtype.IsNullOrEmpty()) || i.Subtype == "HtmlDocument");
                    break;
                case TemplateContext.TemplateDisplayContext.Assignments:
                    items =  items.Where(i => i.Title != "Folder");
                    break;                 
                case TemplateContext.TemplateDisplayContext.Default:
                    break;
            }
            return items.Map(i => i.ToTemplate(ContentActions));
        }

        /// <summary>
        /// Creates a Template from an existing item.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        public string CreateTemplate(string itemId)
        {
            var newId = Context.NewItemId();
            ContentActions.CopyItem(Context.EntityId, itemId, newId);
            ContentActions.CopyItemSettings(Context.EntityId, itemId, newId);
            var newItem = ContentActions.GetContent(Context.EntityId, newId);
            newItem.ParentId = ContentActions.TemporaryFolder;
            newItem.Href = string.Format("Templates/Data/{0}/index.html", newId);
            ContentActions.StoreContent(newItem);
            return newItem.Id;
        }

        /// <summary>
        /// Save the new item (the one that was copied and modified) over the original.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        public void SaveTemplate(string itemId, AssessmentSettings settings = null)
        {
            var item = ContentActions.GetContent(Context.EntityId, itemId);
            // Keep the location of the item
            item.ParentId = ContentActions.TemplateFolder;
            ContentActions.StoreContent(item);

            // Save assessement settings if it's an assessment template
            if (Request.Params["AssessmentId"] != null)
            {
                // Bind assessment settings because of the prefix problem
                // (input fields have an "AssessmentSettings." prefix)
                // Once fixed the following lines can me removed, as the model binder
                // will take care of the mapping
                if (Request.Params["AssessmentSettings.ShowScoreAfter"] != null)
                {
                    settings.ShowScoreAfter = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowScoreAfter"]);
                    settings.ShowQuestionsAnswers = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowQuestionsAnswers"]);
                    settings.ShowRightWrong = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowRightWrong"]);
                    settings.ShowAnswers = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowAnswers"]);
                    settings.ShowFeedbackAndRemarks = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowFeedbackAndRemarks"]);
                    settings.ShowSolutions = (Models.ReviewSetting)Enum.Parse(typeof(Models.ReviewSetting), Request.Params["AssessmentSettings.ShowSolutions"]);
                }
                // Persist the settings
                var helper = new AssessmentSettingsHelper(Context, ContentActions);
                helper.Save(settings);
            }
        }

        /// <summary>
        /// Save the old template as a new template in the PX_TEMPLATE folder
        /// </summary>
        /// <param name="responseIdNew">The response id new.</param>
        /// <param name="responseIdOld">The response id old.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="oldItemId">The old item id.</param>
        public void SaveTemplateAs(string responseIdNew, string responseIdOld, string title, string description, string oldItemId)
        {
            ContentActions.CopyItem(Context.EntityId, responseIdOld, oldItemId);
            DeleteTemplate(responseIdOld);

            BizDC.ContentItem oldItem = ContentActions.GetContent(Context.EntityId, oldItemId);
            oldItem.ParentId = ContentActions.TemplateFolder;
            oldItem.Href = string.Format("Templates/Data/{0}/index.html", oldItemId);
            ContentActions.StoreContent(oldItem);

            // now take the new item and make appropriate updates to it:
            BizDC.ContentItem ci = ContentActions.GetContent(Context.EntityId, responseIdNew);
            ci.ParentId = ContentActions.TemplateFolder;
            ci.Title = title;
            ci.Template = responseIdNew;
            ci.Properties["templateparent"] = new BizDC.PropertyValue() { Type = Bfw.PX.Biz.DataContracts.PropertyType.String, Value = oldItemId };
            var rez = new BizDC.Resource()
            {
                ContentType = "text/html",
                Extension = "html",
                Status = Bfw.PX.Biz.DataContracts.ResourceStatus.Normal,
                Url = ci.Href,
                EntityId = Context.EntityId
            };
            var sw = new System.IO.StreamWriter(rez.GetStream());
            sw.Write(System.Web.HttpUtility.HtmlDecode(description));
            sw.Flush();
            ci.Resources = new List<BizDC.Resource>() { rez };
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Save the new item (the one that was copied and modified) over the original.
        /// Then apply this change to all items derived from the template.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        public void SaveTemplateUpdate(string itemId)
        {
            // Apply changes to content derived from the template
            var templateItems = ContentActions.GetTemplateItems(Context.EntityId, itemId);

            foreach (var c1 in templateItems)
            {
                if (!GradeActions.HasSubmissions(c1.Id))
                {
                    ContentActions.CopyItemSettings(Context.EntityId, itemId, c1.Id);
                }
            }
        }

        /// <summary>
        /// Applies the template changes.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="contentTitle">The content title.</param>
        /// <param name="contentDesc">The content desc.</param>
        [ValidateInput(false)]
        public void ApplyTemplateChanges(string itemId, string contentTitle, string contentDesc)
        {
            // now take the new item and make appropriate updates to it:
            BizDC.ContentItem ci = ContentActions.GetContent(Context.EntityId, itemId);
            if (string.IsNullOrEmpty(ci.Href)) ci.Href = string.Format("Templates/Data/{0}/index.html", itemId);
            ci.ParentId = ContentActions.TemplateFolder;
            ci.Title = contentTitle;
            // ci.Properties["templateparent"] = new BizDC.PropertyValue() { Type = Bfw.PX.Biz.DataContracts.PropertyType.String, Value = oldItemId };
            var rez = new BizDC.Resource()
            {
                ContentType = "text/html",
                Extension = "html",
                Status = Bfw.PX.Biz.DataContracts.ResourceStatus.Normal,
                Url = ci.Href,
                EntityId = Context.EntityId
            };
            var sw = new System.IO.StreamWriter(rez.GetStream());
            sw.Write(System.Web.HttpUtility.HtmlDecode(contentDesc));
            sw.Flush();
            ci.Resources = new List<BizDC.Resource>() { rez };
            ContentActions.StoreContent(ci);
        }

        /// <summary>
        /// Gets the template items.
        /// </summary>
        /// <param name="parentId">The ID of the template for which to get the template items.</param>
        /// <returns></returns>
        public ActionResult GetTemplateItems(string templateId)
        {
            var templateItems = ContentActions.GetTemplateItems(Context.EntityId, templateId);
            var responseObj = new { items = new List<dynamic>() };

            foreach (var templateItem in templateItems)
            {
                // We should be checking whether this item has been submitted by the user.  That appears to be
                // broken for the time being, so ignoring this for now.
                //if (!GradeActions.HasSubmissions(c1.Id))
                {
                    responseObj.items.Add(new { title = templateItem.Title, id = templateItem.Id });
                }
            }

            return Json(responseObj, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks if the template has child items.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public bool TemplateHasChildItems(string parentId)
        {
            var templateItems = ContentActions.GetDerivedTemplateItems(Context.EntityId, parentId);
            if (templateItems.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Delete the custom template
        /// </summary>
        /// <param name="itemId">The item id.</param>
        public ActionResult DeleteTemplate(string itemId)
        {
            if (!TemplateHasChildItems(itemId))
            {
                ContentActions.RemoveContent(Context.EntityId, itemId);
                return Content("deleted");
            }
            else
            {
                return Content("haschildren");
            }
        }
    }
}
