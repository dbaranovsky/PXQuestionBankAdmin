using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Configuration;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    public class NewAssignmentWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Public Constructor
        /// </summary>
        public NewAssignmentWidgetController(BizSC.IBusinessContext context)
        {
            Context = context;
        }

        #region IPXWidget Members

        /// <summary>
        /// Summary view ofr New Assignment widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            ViewData["DisplayButton"] = Context.AccessLevel == BizSC.AccessLevel.Instructor;
            return View();
        }

        /// <summary>
        /// View All option for the New Assignment widget
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
