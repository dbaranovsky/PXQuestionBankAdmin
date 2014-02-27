using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Text;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class CustomWidgetController : Controller, IPXWidget
    {

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }



        /// <summary>
        /// Access to a content helper object
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }


        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }
        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }
        /// <summary>
        /// Gets or sets the resource map actions.
        /// </summary>
        /// <value>
        /// The resource map actions.
        /// </value>
        protected BizSC.IResourceMapActions ResourceMapActions { get; set; }

        /// <summary>
        /// Actions for courses
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomWidgetController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content actions.</param>
        public CustomWidgetController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, BizSC.IPageActions pageActions, ContentHelper helper)
        {
            Context = context;
            ContentActions = contentActions;
            PageActions = pageActions;
            ContentHelper = helper;
        }

        /// <summary>
        /// Action providing the display of the widget when it is in 'Summary' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            return this.RouteData.Values["id"] != null ? ViewDetail(RouteData.Values["id"].ToString()) : View("CustomWidget");
        }

        /// <summary>
        /// Return the Details view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult ViewDetail(string id)
        {
            id = id.Replace("custom_widget_", "");
            var biz = ContentActions.GetContent(Context.EntityId, id, true);
            var model = biz.ToHtmlDocument();

            model.Body = string.IsNullOrEmpty(model.Body) ? "<div></div>" : model.Body;
            return View("CustomWidget", model);
        }

        /// <summary>
        /// To get the arguments which is required before add a widget
        /// </summary>
        /// <returns></returns>
        public ActionResult OnBeforeAdd(Bfw.PX.PXPub.Models.Widget model)
        {
            if (model.Id == "PX_Custom")
            {
                var newItemId = Context.NewItemId();
                var item = ContentActions.CopyItem(Context.EntityId, model.Id, newItemId, "PX_TEMP", null);
                ViewData["WidgetId"] = item.Id;
                ViewData["Mode"] = "ADD";
            }
            else
            {
                BizDC.ContentItem ci = ContentActions.GetContent(Context.EntityId, model.Id, true);
                CustomWidget cw = ci.ToCustomWidget(ContentActions);
                //cw.WidgetContents =  (cw.WidgetContents);
                ViewData.Model = cw;
                ViewData["WidgetId"] = model.Id;
                ViewData["Mode"] = "EDIT";
            }
            return View("AddEdit");
        }

        /// <summary>
        /// To get the arguments which is required before add a widget
        /// </summary>
        /// <param name="WidgetName"></param>
        /// <param name="WidgetContents"></param>
        /// <param name="pageName"></param>
        /// <param name="WidgetZoneID"></param>
        /// <param name="WidgetTemplateID"></param>
        /// <param name="WidgetID"></param>
        /// <param name="PrevSeq"></param>
        /// <param name="NextSeq"></param>
        /// <returns></returns>
        //public ActionResult AddCustomWidget(string WidgetName,string WidgetContents, string pageName, string WidgetZoneID, string WidgetTemplateID, string WidgetID, string PrevSeq, string NextSeq)
        [ValidateInput(false)]
        public ActionResult AddCustomWidget(CustomWidget content, string behavior, string Id, string ZoneId,
            string belowSequence)
        {
            string errorMsg = string.Empty;
            content.ParentId = ZoneId;

            if (!belowSequence.IsNullOrEmpty())
            {
                content.Sequence = Context.Sequence(string.Empty, belowSequence);
            }

            if (!content.WidgetContents.IsNullOrEmpty() && content.Description.IsNullOrEmpty())
            {
                content.Description = content.WidgetContents;
            }

            ContentHelper.StoreCustomWidget(content, Context.EntityId);
            return null;
        }

        /// <summary>
        /// Gets the view for widget
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult View(Bfw.PX.PXPub.Models.Widget model)
        {
            string widgetId = model.Id;
            CustomWidget cw = ContentActions.GetContent(Context.EntityId, widgetId, true).ToCustomWidget(ContentActions);
            var widgetModel = new CustomWidget();
            widgetModel.Title = cw.Title;
            widgetModel.WidgetContents = cw.WidgetContents;
            ViewData.Model = widgetModel;
            return View("Summary");
        }

        /// <summary>
        /// Get Content Id
        /// </summary>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        protected ContentItem getContentItem(string widgetId)
        {
            ContentItem ci = null;

            return ci;
        }


        /// <summary>
        /// Action providing the display of the widget when it is in 'View All' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return new EmptyResult();
        }




        /// <summary>
        /// Return the Index View
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Shows the widget config.
        /// </summary>
        /// <param name="widgetId">The widget id.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult ShowWidgetConfig(string widgetId)
        {
            if (!string.IsNullOrEmpty(widgetId))
            {
                var ci = ContentActions.GetContent(Context.EntityId, widgetId.Replace("custom_widget_", ""), true);
                var htmlDoc = ci.ToHtmlDocument();

                ViewData["isEdit"] = true;
                return View("CustomWidget", htmlDoc);
            }
            else
            {
                return View("CustomWidget");
            }
        }
    }
}
