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

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// 
    /// </summary>
    [PerfTraceFilter]
    public class PageContainerWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="announcementActions">The announcement actions.</param>
        public PageContainerWidgetController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        #region IPXWidget Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            ViewData.Model = widget;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult ViewAll(Widget widget)
        {
            return View();
        }


        #endregion
    }
}
