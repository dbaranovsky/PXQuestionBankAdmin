using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Bfw.Common.Collections;
using Bfw.Common;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class ContentReadWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// Access to the current business context information.
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
        /// Gets or sets the navigation actions.
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentReadWidgetController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="navigationActions">The navigation actions.</param>
        public ContentReadWidgetController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, BizSC.INavigationActions navigationActions)
        {
            Context = context;
            ContentActions = contentActions;
            NavigationActions = navigationActions;
        }

        #region IPXWidget Members

        /// <summary>
        /// Action providing the display of the widget when it is in 'Summary' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            var readItems = ContentActions.ListContentReadByDate();

            if (!readItems.IsNullOrEmpty())
                ViewData["read"] = readItems.Map(r => r.ToContentItem(ContentActions));

            var c = ContentActions.GetContent(Context.EntityId, "widgetconfig_contentreadwidget", true).ToWebConfiguration();
            string contentId = c.TargetId;

            if (string.IsNullOrEmpty(contentId))
            {
                var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, "PX_TOC");
                var navToc = bizNavigationItem.ToNavigationItem(ContentActions);

                if (navToc != null && navToc.Children.Count > 0)
                    contentId = navToc.Children.First().Id;
            }

            var content = ContentActions.GetContent(Context.EntityId, contentId);

            if (content != null)
            {
                ViewData["contentDescription"] = content.ToFolder().Description;
                ViewData["contentId"] = content.Id;
            }

            return View();
        }

        /// <summary>
        /// Action providing the display of the widget when it is in 'View All' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }

        #endregion
    }
}
