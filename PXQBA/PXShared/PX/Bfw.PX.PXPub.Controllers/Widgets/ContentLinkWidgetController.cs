using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class ContentLinkWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context
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
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Actions for courses
        /// </summary>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// A private const to store the Widget Content Id
        /// </summary>
        private const string WidgetContentId = "PX_LOCATION_ZONE1_MENU_1";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLinkWidgetController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="navActions">The nav actions.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="helper">The helper.</param>
        public ContentLinkWidgetController(BizSC.IBusinessContext context, BizSC.INavigationActions navActions, BizSC.IContentActions contentActions, ContentHelper helper)
        {
            ContentActions = contentActions;
            Context = context;
            NavigationActions = navActions;
            ContentHelper = helper;
        }

        /// <summary>
        /// Action providing the display of the widget when it is in 'Summary' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            var contentLinks = ContentActions.GetContent(Context.EntityId, WidgetContentId);
            var links = ContentActions.ListChildren(Context.EntityId, contentLinks.Id);
            var linkItems = new List<Link>();

            foreach (var link in links)
            {
                var lnk = link.ToLink();

                if(link.Subtype.ToLowerInvariant() == "navigationitem")
                {
                    lnk.Url = Url.RouteUrl("FeaturedContentItem", new { id = link.Properties["bfw_tocid"].Value });
                }

                linkItems.Add(lnk);
            }
            
            ViewData["parentName"] = WidgetContentId;
            ViewData.Model = linkItems;
            return View("~/Views/Navigation/ContentLinks.ascx");
        }

        /// <summary>
        /// Action providing the display of the widget when it is in 'View All' mode.
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }
    }
}