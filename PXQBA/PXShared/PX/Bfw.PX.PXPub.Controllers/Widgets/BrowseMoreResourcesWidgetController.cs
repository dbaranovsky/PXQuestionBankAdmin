using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using Aspose.Words.Lists;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.PXPub.Models.ContentItem;
using System.Web;


namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class BrowseMoreResourcesWidgetController : Controller, IPXWidget
    {
        bool _doProductSearch = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["FaceplateSearchAgainstProductCourse"]);

        /// <summary>
        /// The current business context.
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
        /// Gets or sets the resource map actions.
        /// </summary>
        /// <value>
        /// The resource map actions.
        /// </value>
        protected BizSC.IResourceMapActions ResourceMapActions { get; set; }
        /// <summary>
        /// Gets or sets the search actions.
        /// </summary>
        /// <value>
        /// The search actions.
        /// </value>
        protected BizSC.ISearchActions SearchActions { get; set; }        

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomWidgetController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content actions.</param>
        public BrowseMoreResourcesWidgetController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, BizSC.ISearchActions searchActions)
        {
            Context = context;
            ContentActions = contentActions;
            SearchActions = searchActions;
        }

        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Message()
        {
            return View();
        }

        public ActionResult ResourceTypeList()
        {
            ViewData["AccessLevel"] = Context.AccessLevel;
            return View();
        }
        public ActionResult Summary(Models.Widget widget)
        {
            return View();
        }
        /// <summary>
        /// Shows all data related to the currently logged in user
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View();
        }

        /// <summary>
        /// Returns main view for Browse More Resources window
        /// </summary>
        /// <returns></returns>
        public ActionResult FacePlateBrowseResources()
        {
            var searchSchema = Context.Course.FacetSearchSchema;
            //TODO: use search schema to determine resources dropdowns
            var RssFeeds = Context.Course.RSSCourseFeeds.ToDictionary(k => k.Name, v => v.Url);

            ViewData["AccessLevel"] = Context.AccessLevel;
            return View(RssFeeds);
        }

    
        /// <summary>
        /// Returns view for facet types list
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
        public ActionResult FacePlateBrowseResourcesFacets(IEnumerable<string> fieldNames, string title)
        {
            //List<FacetValue> model = new List<FacetValue>();
            FacetedSearchResults model = new FacetedSearchResults();
            foreach (var fieldName in fieldNames)
            {
                var searchResults = GetFacetValues(fieldName, false);
                model.FacetFields.AddRange(searchResults.FacetFields);

                //if (searchResults.FacetFields.Count > 0)
                //{

                //    model.AddRange(searchResults.FacetFields.First().FieldValues);
                //}

            }
            ViewData["AccessLevel"] = Context.AccessLevel;
            ViewData["Title"] = title;
            return View("ResourceFacets",model);
        }

        /// <summary>
        /// Returns view for facet types list
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
        public ActionResult FacePlateBrowseResourcesFacetsInstructorConsole(IEnumerable<string> fieldNames)
        {
            List<FacetValue> model = new List<FacetValue>();
            foreach (var fieldName in fieldNames)
            {
                var searchResults = GetFacetValues(fieldName);

                if (searchResults.FacetFields.Count > 0)
                {

                    model.AddRange(searchResults.FacetFields.First().FieldValues);
                }

            }
            ViewData["AccessLevel"] = Context.AccessLevel;
            return View(model);
        }

     
        /// <summary>
        /// Gets values for a specific faceted field
        /// </summary>
        /// <param name="facetName"></param>
        /// <returns></returns>
        private FacetedSearchResults GetFacetValues(string facetName, bool includeFolders = true)
        {

            //get search controller instance
            SearchController searchController =
                ServiceLocator.Current.GetInstance(typeof(SearchController)) as SearchController;
            //do a faceted search for units(chapters)
            SearchQuery query = new SearchQuery()
            {
                IsFaceted = true,
                FacetedQuery = new FacetedSearchQuery
                {
                    Fields =
                                {
                                    facetName
                                },
                    Limit = -1,
                    MinCount = 1,
                    
                }
            };

            if (_doProductSearch)
            {
                query.EntityId = Context.Course.ProductCourseId;

                if (!includeFolders)
                {
                    query.ExcludeWords = "dlap_itemtype:" + (int)DlapItemType.Folder;
                }
            }

            FacetedSearchResults searchResults = searchController.DoFacetedSearch(query);
            

            return searchResults;
        }

        /// <summary>
        /// Searches for resources by meta values
        /// </summary>
        /// <param name="query">Search query to use</param>
        /// <param name="chapterId">Current selected chapter</param>
        /// <param name="ebookOnly">Return only resources marked as ebook</param>
        /// <returns></returns>
        public ActionResult FacePlateBrowseResourcesResults(SearchQuery query, bool ebookOnly = false, bool fromLearningCurve = false)
        {
            string chapterId = GetCurrentChapterIdFromCookie();
            List<SearchResultDoc> ebookItems = new List<SearchResultDoc>();
            Dictionary<string, string> searchQuery = new Dictionary<string, string>();
            foreach (var field in query.MetaIncludeFields.Split(','))
            {
                searchQuery.Add(field, query.ExactPhrase);
            }

            //var chapterItem = ContentActions.GetContent(Context.EntityId, chapterId).ToContentItem(ContentActions);
            //var chapterItems = GetContainerItems("Launchpad", chapterId); 
            //GetChildrenForParentId(chapterItem); //get parents for current chapter to speed up data retreival on initial more resources load
            //if (!string.IsNullOrWhiteSpace(chapterId))
            //{
            //    GetAllChildren(chapterId, chapterItems);
            //}

            var items =
                ContentActions.DoItemsSearch(Context.EntityId, searchQuery).Where(
                    i => i.Type.ToLowerInvariant() != "pxunit" && i.Type.ToLowerInvariant() != "folder" && !i.Id.ToLowerInvariant().StartsWith("shortcut__"));
            if (ebookOnly)
            {
                items = items.Where(item => item.Categories.Any(i => i.Id == "bfw_faceplate_filter" && i.Text == "ebook"));
            }

            // removing hidden items for student's list
            if (Context.AccessLevel == BizSC.AccessLevel.Student)
            {
                items = items.Filter(i => i.HiddenFromStudents == false);
            }

            var searchResults = items.Map(c => ToSearchResultDoc(chapterId, c)).ToList();

            Func<string, object> convert = str =>
            {
                try { return int.Parse(str); }
                catch { return str; }
            };
            
            searchResults = searchResults.OrderBy(i => Regex.Split(i.dlap_title.Replace(" ", ""), "([0-9]+)").Select(convert), new EnumerableComparer<object>()).ToList();
            
            //exclude folders
            searchResults.RemoveAll(d => d.dlap_itemtype == ((int)DlapItemType.Folder).ToString());
            ViewData["AccessLevel"] = Context.AccessLevel;
            ViewData["GroupResultsBySubtopic"] = true;
            ViewData["isFromLearningCurve"] = fromLearningCurve;

            ViewData["Title"] = query.ExactPhrase.Replace("instructor_","").Replace("student_","");
            
            if (query.MetaIncludeFields.Contains("meta-topic"))
            {
                ViewData["BreadcrumbParent"] = "Content by chapter";

            }
            else if (query.MetaIncludeFields.Contains("meta-content-type"))
            {
                ViewData["BreadcrumbParent"] = "Content by type";

            }
            ViewData["BreadcrumbAction"] = "FacePlateBrowseResourcesFacets";
            ViewData["BreadcrumbRoute"] = new RouteValueDictionary(new { fieldNames = query.MetaIncludeFields + "_dlap_e", title = ViewData["BreadcrumbParent"] }); // include _dlap_e to return to facets
            return View("ResourceResults",searchResults);
        }

        /// <summary>
        /// Loads removed resources
        /// </summary>
        /// <returns></returns>
        public ActionResult FacePlateBrowseRemoved(string toc = "syllabusfilter")
        {
            string categoryId = System.Configuration.ConfigurationManager.AppSettings["FaceplateRemoved"];
            string chapterId = GetCurrentChapterIdFromCookie();
            List<BizDC.ContentItem> children = this.ContentActions.ListChildren(Context.EntityId, categoryId, 1, toc, false).OrderByDescending(o => o.Modified).ToList();
           


            List<SearchResultDoc> docs = new List<SearchResultDoc>();
            foreach (var child in children)
            {

                var doc = ToSearchResultDoc(chapterId, child);
                docs.Add(doc);
            }
            ViewData["AccessLevel"] = Context.AccessLevel;
            ViewData["GroupResultsBySubtopic"] = false;


            ViewData["Title"] = "Removed Content";
           

            return View("ResourceResults", docs);
        }

        private string GetCurrentChapterIdFromCookie()
        {
            string chapterId = string.Empty;
            var httpCookie = Request.Cookies[Context.Course.Id + "_currentChapterId"];
            if (httpCookie != null)
                chapterId = httpCookie.Value;
            return chapterId;
        }


        /// <summary>
        /// Searches for resources by meta values
        /// </summary>
        /// <param name="query">Search query to use</param>
        /// <param name="chapterId">Current selected chapter</param>
        /// <returns></returns>
        /// 
        [ValidateInput(false)]
        public ActionResult FacePlateBrowseResourcesStringSearch(string IncludeWords, int Start, int Rows, bool questionSearch = false)
        {           
            string chapterId = GetCurrentChapterIdFromCookie();
            SearchResults searchResults = null;
            SearchQuery query = new Models.SearchQuery();
            query.IncludeWords = HttpUtility.UrlDecode(IncludeWords);
            query.Start = Start;
            query.Rows = Rows;
            query.ClassType = (questionSearch)? "question" : string.Empty;
            
            if (!questionSearch)
            {
                query.ContentTypes = String.Join(",", Enum.GetNames(typeof(DlapItemType)).Filter(e => e != DlapItemType.Folder.ToString()));
                if (_doProductSearch)
                {
                    query.EntityId = Context.Course.ProductCourseId;
                }

                searchResults = SearchActions.DoSearch(query.ToSearchQuery(), this.Url, true).ToSearchResults();
                searchResults.docs = searchResults.docs
                                                  .Filter(
                                                      i =>
                                                      !i.dlap_id.ToLower().Contains("shortcut__") &&
                                                      !i.dlap_id.Contains("LOR_") &&
                                                      !i.itemid.ToLower().StartsWith("px_"))
                                                  .ToList();
                searchResults.docs = searchResults.docs.Distinct((s1, s2) => s1.dlap_id == s2.dlap_id).ToList();

                foreach (var doc in searchResults.docs)
                {
                    ParseSearchResult(chapterId, doc);
                }

                ViewData["GroupResultsBySubtopic"] = false;
            }

            ViewData["AccessLevel"] = Context.AccessLevel;
            ViewData["Title"] = "Search Results for " + query.IncludeWords;
            ViewData["ShowQuestionsTab"] = true;
            ViewData["ShowQuestionsTab"] = true;
            ViewData["Quiz"] = new Quiz() { CourseInfo = Context.Course.ToCourse() };

            if (questionSearch)
            {
                return View("ResourceQuestionSearch", query);    
            }
            else
            {
                return View("ResourceResults", searchResults.docs);
            }
        }

        /// <summary>
        /// Loads user resources
        /// </summary>
        /// <returns></returns>
        public ActionResult FacePlateBrowseMyResources()
        {
            string categoryId = System.Configuration.ConfigurationManager.AppSettings["MyMaterials"];
            var children = this.ContentActions.ListChildren(Context.EntityId, "", 1, categoryId);
            string chapterId = GetCurrentChapterIdFromCookie();

            List<SearchResultDoc> docs = new List<SearchResultDoc>();
            var chapterItems = new List<ContentItem>();
            foreach (var child in children)
            {
                var doc = ToSearchResultDoc(chapterId, child);
                docs.Add(doc);
            }
            ViewData["AccessLevel"] = Context.AccessLevel;
            ViewData["GroupResultsBySubtopic"] = false;
            ViewData["Title"] = "Content I've created";            
            return View("ResourceResults", docs);
        }

        /// <summary>
        /// Shows a list of questions banks of a given type
        /// </summary>
        /// <param name="collectiontype"></param>
        /// <returns></returns>
        public ActionResult ResourceQuestionBanks(string collectiontype)
        {
            return View("ResourceQuestionBanks",(object)collectiontype);
        }


        private SearchResultDoc ToSearchResultDoc(string chapterId, BizDC.RSSFeed feed)
        {

            SearchResultDoc doc = new SearchResultDoc()
            {
                entityid = Context.EntityId,
                doc_class = "feed",
                dlap_class = "feed",
                itemid = feed.Link,
                score = "1",
                dlap_hiddenfromstudent = "false",
                dlap_id = feed.Link,
                dlap_itemtype = "",
                dlap_contenttype = "rss",
                dlap_title = feed.LinkTitle,
                dlap_html_text = "",
                dlap_text = "",
                Url = feed.Link,
                Metadata = new Dictionary<string, string>()
            };
            doc.Metadata["PubDate"] = feed.PubDateCalculated;

            bool included = false, inuse = false, isEbook = false;
            string parentName = feed.RssUrl;
            string parentId = "";

            parentName = "";// InspectResourceItem(chapterId, doc.itemid, ref included, ref inuse, ref isEbook, ref parentId);
            doc.Included = included;
            doc.InUse = inuse;
            doc.RootParentName = parentName;
            doc.RootParentId = parentId;
            return doc;
        }
		private void ParseSearchResult(string chapterId, SearchResultDoc doc)
		{
			doc.InUse = false;
			doc.Included = false;

			var subContainerId = doc.Metadata.FirstOrDefault(m => m.Key == "meta-subcontainers/meta-subcontainerid_dlap_e").Value;
			var containerId = doc.Metadata.FirstOrDefault(m => m.Key == "meta-containers/meta-container_dlap_e").Value;

			if (string.IsNullOrWhiteSpace(subContainerId) || string.IsNullOrWhiteSpace(containerId) || containerId.ToLowerInvariant() != "launchpad") return;
			doc.Included = subContainerId == chapterId;
			doc.InUse = true;
			var c = GetItem(subContainerId);
			if (c == null) return;
			doc.RootParentName = c.Title;
			doc.RootParentId = subContainerId;
		}

        private SearchResultDoc ToSearchResultDoc(string chapterId, Biz.DataContracts.ContentItem child, string toc = "syllabusfilter")
        {
            child.Id = child.Id.Replace("Shortcut__1__", "");
            SearchResultDoc doc = new SearchResultDoc()
            {
                entityid = Context.EntityId,
                doc_class = "item",
                dlap_class = "item",
                itemid = child.Id,
                score = "1",
                dlap_hiddenfromstudent = child.HiddenFromStudents.ToString(),
                dlap_id = child.Id,
                dlap_itemtype = child.Type,
                dlap_contenttype = child.ToContentItem(ContentActions).GetFriendlyItemContentType(),
                dlap_title = child.Title,
                dlap_subtitle = child.SubTitle,
                dlap_html_text = "",
                dlap_text = "",
                
            };
            child.FacetMetadata.ToList().ForEach(i => doc.Metadata.Add(i.Key, i.Value));

            bool included = false, inuse = false, isEbook = false;
            string parentName = "";
            string parentId = "";

            isEbook = (child.Categories.FirstOrDefault(i => i.Id == "bfw_faceplate_filter" && i.Text == "ebook") != null);


            if (!string.IsNullOrWhiteSpace(child.GetSubContainer(toc)) && child.GetContainer().ToLowerInvariant() == "launchpad")
            {
                inuse = true;
                parentId = child.GetSubContainer(toc);
                if (parentId == chapterId)
                {
                    included = true;
                }
                else
                {
                    included = false;
                }
                ContentItem item = GetItem(parentId);
                if (item != null)
                {
                    parentName = item.Title;
                }
            }

            doc.Included = included;
            doc.InUse = inuse;
            doc.RootParentName = parentName;
            doc.RootParentId = parentId;

            return doc;
        }

        public static Dictionary<string, ContentItem> TempItemCache = new Dictionary<string, ContentItem>(); //TODO: temphack, get rid of this when global caching is enabled



        private ContentItem GetItem(string itemId)
        {
            ContentItem item;
            if (TempItemCache.ContainsKey(itemId))
            {
                item = TempItemCache[itemId];
            }
            else
            {
                var dcitem = ContentActions.GetContent(Context.EntityId, itemId);
                if (dcitem == null)
                {
                    item = null;
                }
                else
                {
                    item = dcitem.ToContentItem(ContentActions, false);
                }
                TempItemCache.Add(itemId, item);
            }
            return item;
        }


    }
}
