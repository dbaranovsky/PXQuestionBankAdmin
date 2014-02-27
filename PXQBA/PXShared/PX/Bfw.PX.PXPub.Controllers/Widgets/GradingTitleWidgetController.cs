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
    [PerfTraceFilter]
    public class GradingTitleWidgetController : Controller, IPXWidget
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
        /// <param name="context"></param>
        public GradingTitleWidgetController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        #region IPXWidget Members

        /// <summary>
        /// Summary view of Grading Title widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            return View();
        }

        /// <summary>
        /// View All option for the Grading Title widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget widget)
        {
            return View();
        }

        #endregion



    }
}
