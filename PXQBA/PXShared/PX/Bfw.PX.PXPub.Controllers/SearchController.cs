using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.Common.Collections;
using Bfw.Common.Caching;
using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Controllers.Mappers;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.Common;
using System.Xml.Linq;
using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers
{
    // [BfwActionFilter]
    [PerfTraceFilter]
    public class SearchController : Controller
    {
        /// <summary>
        /// The current business context
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the search actions.
        /// </summary>
        /// <value>
        /// The search actions.
        /// </value>
        protected BizSC.ISearchActions SearchActions { get; set; }
        /// <summary>
        /// Gets or sets the task actions.
        /// </summary>
        /// <value>
        /// The task actions.
        /// </value>
        protected BizSC.ITaskActions TaskActions { get; set; }
        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        protected Bfw.Common.Caching.ICacheProvider Cache { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="searchActions">The search actions.</param>
        /// <param name="taskActions">The task actions.</param>
        /// <param name="enrollmentActions">The enrollment actions.</param>
        public SearchController(BizSC.IBusinessContext context, BizSC.ISearchActions searchActions, BizSC.ITaskActions taskActions, BizSC.IEnrollmentActions enrollmentActions, Bfw.Common.Caching.ICacheProvider cache)
        {
            Context = context;
            SearchActions = searchActions;
            TaskActions = taskActions;
            EnrollmentActions = enrollmentActions;
            Cache = cache;
        }

        /// <summary>
        /// <summary>
        /// Returns the Search view for this Controller.
        /// </summary>
        /// <returns></returns>
        public ActionResult Search()
        {
            if (Context.IsAnonymous)
            {
                return new EmptyResult();
            }

            ViewData["AllowSiteSearch"] = Context.Course.AllowSiteSearch;
            return View();
        }
        /// Returns the Faceted Search view for this Controller.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public ActionResult FacetedSearchResults(Models.SearchQuery query)
        {
            ViewData.Model = DoFacetedSearch(query);
            return View();
        }

        public SearchResults DoSearch(Models.SearchQuery query, UrlHelper url, bool postProcess = false)
        {
            if (url == null)
            {
                url = this.Url;

            }

            var bizQuery = query.ToSearchQuery();

            SearchResultSet srs = SearchActions.DoSearch(bizQuery, url, postProcess);
            var searchResults = srs.ToSearchResults();


            return searchResults;
        }

        public FacetedSearchResults DoFacetedSearch(Models.SearchQuery query)
        {

            if (!query.IsFaceted)
            {
                return null;
            }
            var searchResults = new FacetedSearchResults();

            searchResults.Query = query;
            var bizQuery = query.ToSearchQuery();

            searchResults = Cache.FetchFacetedSearchResults(bizQuery);

            if (searchResults == null)
            {
                var srs = SearchActions.DoSearch(bizQuery, Url, true);
                searchResults = srs.ToFacetedSearchResults();
                Cache.StoreFacetedSearchResults(bizQuery, searchResults);
            }
            return searchResults;
        }
    }
}