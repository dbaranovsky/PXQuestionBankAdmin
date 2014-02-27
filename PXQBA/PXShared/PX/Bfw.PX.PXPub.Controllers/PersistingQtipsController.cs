using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class PersistingQtipsController : Controller
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Helper functions to access the user info
        /// </summary>
        protected BizSC.IUserActions UserActions { get; set; }

        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// instance controller
        /// </summary>
        public PersistingQtipsController(BizSC.IBusinessContext context, BizSC.IUserActions userActions, BizSC.IContentActions contentActions)
        {
            Context = context;
            UserActions = userActions;
            ContentActions = contentActions;
        }

        /// <summary>
        /// show the persistent qtip given the userid and the itemid
        /// </summary>
        /// <param name="tooltipId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ActionResult ShowPersistentQtip(string tooltipId)
        {
            var currentUser = Context.CurrentUser;
            var toolTipItem = ContentActions.GetContent(Context.EntityId, tooltipId);
            var showToolTip = true;
            var isToolTipHidden = false;
            var qtipTargetPosition = string.Empty;
            var qtipTooltipPosition = string.Empty;
            var qtipTarget = string.Empty;
            var showStudent = false;

            if (currentUser.Properties.ContainsKey(tooltipId))
            {
                showToolTip = (bool)currentUser.Properties[tooltipId].Value;
            }

            //show the persistent tooltip for a student
            if (toolTipItem.Properties.ContainsKey("showStudent"))
            {
                showStudent = (bool)toolTipItem.Properties["showStudent"].Value;
            }

            if (showToolTip && ((Context.AccessLevel == BizSC.AccessLevel.Instructor) | (showStudent && Context.AccessLevel == BizSC.AccessLevel.Student)))
            {
                if (toolTipItem.Properties.ContainsKey("isToolTipHidden"))
                {
                    isToolTipHidden = (bool)toolTipItem.Properties["isToolTipHidden"].Value;
                }

                if (toolTipItem.Properties.ContainsKey("qtipTargetPosition"))
                {
                    qtipTargetPosition = toolTipItem.Properties["qtipTargetPosition"].Value.ToString();
                }

                if (toolTipItem.Properties.ContainsKey("qtipTooltipPosition"))
                {
                    qtipTooltipPosition = toolTipItem.Properties["qtipTooltipPosition"].Value.ToString();
                }

                if (toolTipItem.Properties.ContainsKey("qtipTarget"))
                {
                    qtipTarget = toolTipItem.Properties["qtipTarget"].Value.ToString();
                }

                ViewData["tooltipId"] = tooltipId;
                ViewData["toolTipDescription"] = toolTipItem.Description;
                ViewData["isToolTipHidden"] = isToolTipHidden;
                ViewData["qtipTargetPosition"] = qtipTargetPosition;
                ViewData["qtipTooltipPosition"] = qtipTooltipPosition;
                ViewData["qtipTarget"] = qtipTarget;

                return View();
            }
            else
            {
                return Content(""); // nothing to display if the tooltip does not need to be displayed
            }
        }

        /// <summary>
        /// Close the Qtip and persist the state 
        /// </summary>
        /// <param name="tooltipId"></param>
        /// <returns></returns>
        public JsonResult ClosePersistentQtip(string tooltipId)
        {
            var user = Context.CurrentUser;

            if (user.Properties.ContainsKey(tooltipId))
            {
                user.Properties[tooltipId].Value = false;
            }
            else
            {
                user.Properties.Add(tooltipId, new BizDC.PropertyValue() { Type = BizDC.PropertyType.Boolean, Value = false });
            }

            
            UserActions.UpdateUser(user);

            return Json(new { message = "success" });
        }
    }
}
