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
    /// Provides actions necessary to support the Featured Content widget in Xbook
    /// </summary>
    [PerfTraceFilter]
    public class FeaturedWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the Content
        /// </summary>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext interface
        /// </summary>
        /// <param name="businessContext"></param>
        public FeaturedWidgetController(BizSC.IBusinessContext businessContext, BizSC.IContentActions contentActions)
        {
            Context = businessContext;
            ContentActions = contentActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Displays the Featured Content widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            var featuredWidget = ContentActions.GetContent(Context.EntityId, widget.Id, true).ToFeatureWidget(ContentActions);
            return View(featuredWidget);
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
