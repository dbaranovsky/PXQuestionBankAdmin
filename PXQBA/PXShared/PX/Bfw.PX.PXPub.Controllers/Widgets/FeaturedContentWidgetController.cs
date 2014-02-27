using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

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
    /// Provides actions necessary to support the Featured Content widget
    /// </summary>
    [PerfTraceFilter]
    public class FeaturedContentWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// The featured content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Depends on the IBusinessContext and IAnnouncementActions interfaces.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content actions.</param>
        public FeaturedContentWidgetController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions)
        {
            Context = context;
            ContentActions = contentActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Gets a summarized list of all featured content items for the current
        /// entity and renders them in a view
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            ActionResult result = null;

            var biz = ContentActions.ListFeaturedItems(Context.EntityId);
            var model = new FeaturedContentWidget();
            model.FeaturedContentItems = biz.Map(ci => ci.ToFeaturedContentItem());

            ViewData.Model = model;
            result = View();

            return result;
        }

        /// <summary>
        /// Lists all featured content items.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
