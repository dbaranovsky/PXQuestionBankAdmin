using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.PXPub.Components;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Helpers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;
using System.Web.Mvc;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    public class FacePlateSupportWidgetController: Controller, IPXWidget
    {
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
        public FacePlateSupportWidgetController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, AssignmentCenterHelper assignmentCenterHelper)
        {
            Context = context;
            ContentActions = contentActions;
            AssignmentCenterHelper = assignmentCenterHelper;
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

        public ActionResult ViewSupport(Bfw.PX.PXPub.Models.Widget model)
        {
            if (model.Properties.ContainsKey("bfw_faceplate_support_detail") && model.Properties.ContainsKey("bfw_faceplate_support_detail_popup"))
            {
                ViewData["details"] = model.Properties["bfw_faceplate_support_detail"].Value.ToString();
                ViewData["details_popup"] = model.Properties["bfw_faceplate_support_detail_popup"].Value.ToString();
            }
            return View();
        }
        #endregion
    }
}
