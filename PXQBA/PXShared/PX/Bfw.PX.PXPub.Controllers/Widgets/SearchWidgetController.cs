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
    /// Provides the necessary methods for BeingMeta search
    /// </summary>
    [PerfTraceFilter]
    public class SearchWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext interface
        /// </summary>
        /// <param name="context">The context</param>
        public SearchWidgetController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        #region IPXWidget Members

        /// <summary>
        /// Displays the BeingMeta search widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            return View("BeingMetaSearch");
        }

        /// <summary>
        /// 'View All' mode
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }
        
        #endregion

    }
}
