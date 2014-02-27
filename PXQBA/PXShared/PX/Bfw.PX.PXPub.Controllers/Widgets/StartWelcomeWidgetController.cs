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
using Bfw.PX.PXPub.Controllers.Helpers;

using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using System.Web;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class StartWelcomeWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an IUserActions implementation
        /// </summary>
        /// <value>
        /// The user actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected AssignmentCenterHelper AssignmentCenterHelper { get; set; }
        protected BizSC.ICourseActions CourseActions { get; set; }

        public StartWelcomeWidgetController(BizSC.IBusinessContext context, BizSC.IPageActions pageActions, BizSC.IContentActions contentActions, AssignmentCenterHelper assignmentCenterHelper, ICourseActions courseActions)
        {
            Context = context;
            PageActions = pageActions;
            ContentActions = contentActions;
            AssignmentCenterHelper = assignmentCenterHelper;
            CourseActions = courseActions;
        }

        #region IPXWidget Members

        public ActionResult Summary(Models.Widget widget)
        {
            return View();
        }

        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }

        public ActionResult Index(Bfw.PX.PXPub.Models.Widget model)
        {
            return View(GetWidget(model));
        }

        public ActionResult Edit(string widgetId)
        {
            Widget model = this.PageActions.GetWidget(widgetId).ToWidgetItem();            

            return View(GetWidget(model));
        }

        [ValidateInput(false)]
        public ActionResult Save(StartWelcomeWidget model, string pageName, string WidgetZoneID, string WidgetTemplateID, string WidgetID)
        {
            var result = string.Empty;
            var errorMsg = string.Empty;
            var mode = string.Empty;

            if (string.IsNullOrEmpty(model.Title))
            {
                result = "Fail";
                errorMsg = "You must specify Title";
            }
            else
            {
                var properties = new Dictionary<string, BizDC.PropertyValue>();

                properties["bfw_faceplate_welcome_message_modified"] = new BizDC.PropertyValue()
                {
                    Type = BizDC.PropertyType.Boolean,
                    Value = true
                };                

                properties["bfw_faceplate_welcome_title"] = new BizDC.PropertyValue()
                {
                    Type = BizDC.PropertyType.String,
                    Value = model.Title
                };

                properties["bfw_faceplate_welcome_message"] = new BizDC.PropertyValue()
                {
                    Type = BizDC.PropertyType.String,
                    Value = model.Contents
                };                

                var widget = PageActions.UpdateWidget(pageName, WidgetID, null, properties);

                mode = "EDIT";
                result = "Success";
            }

            return Json(new { Result = result, Mode = mode, OldWidgetID = WidgetID, ErrorMes = errorMsg, WidgetZoneID = WidgetZoneID, WidgetTemplateID = WidgetTemplateID, WidgetId = WidgetID }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        private StartWelcomeWidget GetWidget(Bfw.PX.PXPub.Models.Widget model)
        {
            var widget = new StartWelcomeWidget();
            var displayMessage = true;

            if (model.Properties.ContainsKey("bfw_faceplate_welcome_message"))
            {
                if (Context.Course.ToCourse().IsActivated)
                {
                    if (model.Properties.ContainsKey("bfw_faceplate_welcome_message_modified"))
                    {
                        if (!Boolean.Parse(model.Properties["bfw_faceplate_welcome_message_modified"].Value.ToString()))
                        {
                            displayMessage = false;
                        }
                    }
                }

                if (model.Properties.Where(o => o.Key.Equals("bfw_faceplate_welcome_title")).Count() > 0 && !string.IsNullOrEmpty(model.Properties["bfw_faceplate_welcome_title"].Value.ToString()))
                {
                    widget.Title = HttpUtility.HtmlDecode(model.Properties["bfw_faceplate_welcome_title"].Value.ToString());
                }

                if (!string.IsNullOrEmpty(model.Properties["bfw_faceplate_welcome_message"].Value.ToString()) && displayMessage)
                {
                    widget.Contents = HttpUtility.HtmlDecode( model.Properties["bfw_faceplate_welcome_message"].Value.ToString() );
                }
            }
            else
            {
                var widgetTemp = this.PageActions.GetWidget(model.Id).ToWidgetItem();

                widget = GetWidget(widgetTemp);
            }

            if (String.IsNullOrEmpty(widget.Title))
            {
                widget.Title = String.Format("Welcome to {0}!", Context.Course.CourseDescription);
            }

            return widget;
        }
    }
}
