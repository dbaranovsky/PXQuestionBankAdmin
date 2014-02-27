using System;
using System.Collections.Generic;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class LinkLibraryController : Controller
    {
        #region Data Members

        /// <summary>
        /// A private variable for the TOC Agilix ID.
        /// </summary>
        private const string ITEM_ID = "PX_TOC";

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the navigation actions.
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected BizSC.INavigationActions NavigationActions
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the search actions.
        /// </summary>
        /// <value>
        /// The search actions.
        /// </value>
        protected BizSC.ISearchActions SearchActions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the quick link collection.
        /// </summary>
        /// <value>
        /// The quick link collection.
        /// </value>
        protected IList<QuickLink> QuickLinkCollection
        {
            get;
            set;
        }

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="LinkLibraryController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="searchActions">The search actions.</param>
        /// <param name="navActions">The nav actions.</param>
        public LinkLibraryController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, BizSC.ISearchActions searchActions, BizSC.INavigationActions navActions)
        {
            Context = context;
            ContentActions = contentActions;
            NavigationActions = navActions;
            SearchActions = searchActions;
        }

        /// <summary>
        /// Returns the Index view for this Controller.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var content = new QuickLink();
            var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, ITEM_ID);

            if (!bizNavigationItem.Children.IsNullOrEmpty())
            {
                var toc = bizNavigationItem.Children.Map(ti => ti.ToTocItem());
                content.TocItem = toc;
                content.Id = ITEM_ID;
            }

            return View(content);
        }

        /// <summary>
        /// Expands the section.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="linkId">The link id.</param>
        /// <returns></returns>
        public ActionResult ExpandSection(string id, string linkId)
        {
            var content = new QuickLink();

            if (!string.IsNullOrEmpty(id))
            {
                var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, id);

                if (!bizNavigationItem.Children.IsNullOrEmpty())
                {
                    var toc = bizNavigationItem.Children.Map(ti => ti.ToTocItem());
                    content.TocItem = toc;
                    content.Id = linkId;
                }
            }

            return PartialView("~/Views/Shared/DisplayTemplates/LinkPartials/TocLinks.ascx", content);
        }

        /// <summary>
        /// Searches the link.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public ActionResult SearchLink(Models.SearchQuery query)
        {
            var SearchResults = new AdvancedSearchResults();

            if (String.IsNullOrEmpty(query.ContentTypes))
            {
                query.ContentTypes = "Resource,Comment,Discussion,AssetLink,Assignment";
            }

            SearchResults.Query = query;

            var bizQuery = query.ToSearchQuery();
            var srs = SearchActions.DoSearch(bizQuery, Url, true);
            SearchResults.Results = srs.ToSearchResults();

            ViewData.Model = SearchResults;
            return View();
        }
    }
}
