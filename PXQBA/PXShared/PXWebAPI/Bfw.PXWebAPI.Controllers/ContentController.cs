using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Bfw.PXWebAPI.Helpers;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.Response;
using UserType = Bfw.PXWebAPI.Models.UserType;

namespace Bfw.PXWebAPI.Controllers
{
	/// <summary>
	/// ContentController
	/// </summary>
	public class ContentController : ApiController
	{
		protected IBusinessContext Context { get; set; }

		/// <summary>
		/// Gets  the Api Content Actions.
		/// </summary>
		/// <value>
		/// The API Content actions.
		/// </value>
		protected Bfw.PXWebAPI.Helpers.IApiContentActions ApiContentActions { get; set; }


		/// <summary>
		/// Gets  the Api Search Actions.
		/// </summary>
		/// <value>
		/// The API Search actions.
		/// </value>
		protected IApiSearchActions ApiSearchActions { get; set; }

	    /// <summary>
		/// Gets  the Api Course Actions.
		/// </summary>
		/// <value>
		/// The API Course actions.
		/// </value>
		protected IApiCourseActions ApiCourseActions { get; set; }


		/// <summary>
		/// Gets  the Api User Actions.
		/// </summary>
		/// <value>
		/// The API User actions.
		/// </value>
		protected IApiUserActions ApiUserActions { get; set; }

		/// <summary>
		/// Constructor 
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="apiContentActions"> </param>
		/// <param name="apiSearchActions"> </param>
		/// <param name="apiCourseActions"> </param>
		/// <param name="apiUserActions"> </param>
		public ContentController(IBusinessContext context, IApiContentActions apiContentActions,
								 IApiSearchActions apiSearchActions, IApiCourseActions apiCourseActions,
								 IApiUserActions apiUserActions)
		{
			Context = context;
			ApiContentActions = apiContentActions;
			ApiSearchActions = apiSearchActions; 
			ApiCourseActions = apiCourseActions;
			ApiUserActions = apiUserActions;
		}

		#region "Get Content"

		/// <summary>
		/// Get Content Details as a list of found content items.
		/// Function checks Post Parameter with the key "itemids",
		/// if no "itemids" were sent, then return all items
		/// </summary>
		/// <param name="id"></param>
		/// <returns>ContentItemListResponse</returns>
		[System.Web.Http.HttpPost]
		[System.Web.Http.ActionName(Helper.DETAILS)]
		public ContentItemListResponse Details(string id)
		{
			var response = new ContentItemListResponse();
			var context = HttpContext.Current;
			var postParams = ApiHelper.GetPostParameters(context.Request);

            var itemIds = postParams.ContainsKey(Helper.ITEMIDS) ? postParams[Helper.ITEMIDS] : string.Empty;
            var toc = postParams.ContainsKey(Helper.TOC) ? postParams[Helper.TOC] : Helper.DEFAULT_TOC;
            var contentItems = ApiContentActions.GetItems(id, itemIds, toc);

            if (contentItems.IsNullOrEmpty())
            {
                response.error_message = Helper.NO_RESULTS;
            }
            else
            {
                response.results = contentItems.ToList();
            }

			return response;
		}

		#endregion

		#region "Table of Contents"
		/// <summary>
		/// Get Nested Table of Contents.
		/// Example To Test: GET /Content/TableofContents/98011?parentid=MODULE_bsi__FBE02172__2A83__45B2__B3F8__F947137A031A
		/// </summary>
		/// <param name="id"></param>
		/// <param name="parentid"></param>
		/// <param name="depth"></param>
        /// <param name="toc"></param>
        /// <param name="container"></param>
        /// <returns>TableofContentsItemListResponse</returns>
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("TableofContents")]
        public TableofContentsItemListResponse TableofContents(string id, string parentid = "", int depth = 1, string toc = "syllabusfilter", string container = "Launchpad")
		{
			var response = new TableofContentsItemListResponse();

			//TODO: Cross check with PX team on what the root parent id is. Also remove hardcoded value (SK)
            var tocitems = new List<TableofContentsItem>();
		    if (String.IsNullOrEmpty(parentid))
		    {
                //if (String.IsNullOrEmpty(parentid)) parentid = "PX_ROOT";
		        tocitems.AddRange(ApiContentActions.GetTableofContents("", id, depth, toc, container));
		        tocitems.AddRange(ApiContentActions.GetTableofContents("PX_MULTIPART_LESSONS", id, depth, toc, container));
		    }
		    else
		    {
                tocitems = ApiContentActions.GetTableofContents(parentid, id, depth, toc, container);
		    }

		    if (!tocitems.IsNullOrEmpty())
			{
                response.results = tocitems;
				return response;
			}

			//TODO: Remove hardcoded text from here (SK)
			response.error_message = "No results found";
			return response;
		}


		/// <summary>
		/// Get List of Descendents And Self
		/// Example To Test: GET /Content/PxTableOfContents/98011?parentid=MODULE_bsi__FBE02172__2A83__45B2__B3F8__F947137A031A + userRole=0
		/// </summary>
		/// <param name="userRole"> </param>
		/// <param name="id"></param>
		/// <param name="parentid"></param>
		/// <param name="depth"></param>
		/// <param name="category"></param>
		/// <returns>PxContentItemsListResponse</returns>
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("PxTableOfContents")]
		public PxContentItemsListResponse GetPxTableOfContents(string id, string userRole, string parentid = "", int depth = 1, string category = "syllabusfilter")
		{

			var response = new PxContentItemsListResponse();

			ApiHelper.InitializePxBusinessContext(Context, ApiCourseActions, id, userRole);

			if (String.IsNullOrEmpty(parentid))
				parentid = "PX_ROOT";
			var tocitems = ApiContentActions.GetPxTableOfContent(id, parentid,  depth, category );
			if (!tocitems.IsNullOrEmpty())
			{
				response.results = tocitems;
				return response;
			}

			response.error_message = "No results found";
			return response;
		}


		/// <summary>
		/// Search Table of Contents with following filter options: KeyWord, ItemId, Title, Type, DueDate.
		/// Example to TEST: 
		/// GET Content/SearchPxTableofContents/98011?parentid=MODULE_bsi__FBE02172__2A83__45B2__B3F8__F947137A031A + userRole=0 + filter=KEYWORD + term=Chapter
		/// </summary>
		/// <param name="userRole"> </param>
		/// <param name="id"></param>
		/// <param name="parentid"></param>
		/// <param name="filter"></param>
		/// <param name="term"></param>
		/// <returns>PxContentItemsListResponse</returns>
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("SearchPxTableofContents")]
		public PxContentItemsListResponse SearchPxTableofContents(string id, string parentid, string filter, string term, string userRole)
		{

			//int i = Convert.ToInt32("Res");

			var response = new PxContentItemsListResponse();

			//The following are allowed filter options
			var blnSearchByKeyword = false;
			var blnSearchByItemId = false;
			var blnSearchByTitle = false;
			var blnSearchByType = false;
			var blnSearchDueDate = false;
			if (filter.Trim().ToUpper().Equals("KEYWORD"))
				blnSearchByKeyword = true;
			else if (filter.Trim().ToUpper().Equals("ITEMID"))
				blnSearchByItemId = true;
			else if (filter.Trim().ToUpper().Equals("TITLE"))
				blnSearchByTitle = true;
			else if (filter.Trim().ToUpper().Equals("TYPE"))
				blnSearchByType = true;
			else if (filter.Trim().ToUpper().Equals("DUEDATE"))
				blnSearchDueDate = true;
			else
			{
				response.error_message = "No valid search parameters found";
				return response;
			}
			//TODO: Cross check with PX team on what the root parent id is. Also remove hardcoded value (SK)
			if (parentid == "") parentid = "PX_ROOT";

			ApiHelper.InitializePxBusinessContext(Context, ApiCourseActions, id, userRole);

			var tocitems = ApiContentActions.GetPxTableOfContent(id, parentid);

			if (!blnSearchDueDate) term = term.ToUpper();
			if (!tocitems.IsNullOrEmpty())
			{
				List<Bfw.PX.PXPub.Models.ContentItem> resulttocitems = null;
				if (blnSearchByKeyword)
				{
					resulttocitems = tocitems.Where(t => ( t.Title.ToUpper().IndexOf(term) > -1 ||
													 t.Id.ToUpper().IndexOf(term) > -1 ||
													 t.Type.ToUpper().IndexOf(term) > -1 )).ToList();
				}

				if (blnSearchByItemId) resulttocitems = tocitems.Where(t => ( t.Id.ToUpper().IndexOf(term) > -1 )).ToList();

				if (blnSearchByTitle) resulttocitems = tocitems.Where(t => ( t.Title.ToUpper().IndexOf(term) > -1 )).ToList();

				if (blnSearchByType) resulttocitems = tocitems.Where(t => ( t.Type.ToUpper().IndexOf(term) > -1 )).ToList();

				if (blnSearchDueDate) resulttocitems = tocitems.Where(tocItem => ( tocItem.DueDate.Date == Convert.ToDateTime(term).Date )).ToList();

				if (!resulttocitems.IsNullOrEmpty())
				{
					response.results = resulttocitems.Distinct().ToList();
					return response;
				}
			}

			//TODO: Remove hardcoded text from here (SK)
			response.error_message = "No results found";
			return response;
		}

		/// <summary>
		/// Search Table of Contents with following filter options: KeyWord, ItemId, Title, Type, DueDate.
		/// Example to TEST: 
		/// GET Content/SearchTableofContents/98011?parentid=MODULE_bsi__FBE02172__2A83__45B2__B3F8__F947137A031A + filter=KEYWORD + term=Chapter
		/// </summary>
		/// <param name="id"></param>
		/// <param name="parentid"></param>
		/// <param name="filter"></param>
		/// <param name="term"></param>
		/// <returns></returns>
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("SearchTableofContents")]
		public TableofContentsItemListResponse SearchTableofContents(string id, string parentid, string filter, string term)
		{
			var response = new TableofContentsItemListResponse();

			//The following are allowed filter options
			var blnSearchByKeyword = false;
			var blnSearchByItemId = false;
			var blnSearchByTitle = false;
			var blnSearchByType = false;
			var blnSearchDueDate = false;
			if (filter.Trim().ToUpper().Equals("KEYWORD"))
				blnSearchByKeyword = true;
			else if (filter.Trim().ToUpper().Equals("ITEMID"))
				blnSearchByItemId = true;
			else if (filter.Trim().ToUpper().Equals("TITLE"))
				blnSearchByTitle = true;
			else if (filter.Trim().ToUpper().Equals("TYPE"))
				blnSearchByType = true;
			else if (filter.Trim().ToUpper().Equals("DUEDATE"))
				blnSearchDueDate = true;
			else
			{
				response.error_message = "No valid search parameters found";
				return response;
			}
			//TODO: Cross check with PX team on what the root parent id is. Also remove hardcoded value (SK)
			if (parentid == "") parentid = "PX_ROOT";

			var tocitems = ApiContentActions.GetTableofContents(parentid, id);

			////if the filter is not due date, convert the term into uppercase
			if (!blnSearchDueDate) term = term.ToUpper();
			if (!tocitems.IsNullOrEmpty())
			{
				List<TableofContentsItem> resulttocitems = null;
				if (blnSearchByKeyword)
				{
					resulttocitems = tocitems.Where(t => ( t.Title.ToUpper().IndexOf(term) > -1 ||
														   t.ItemId.ToUpper().IndexOf(term) > -1 ||
														   t.Type.ToUpper().IndexOf(term) > -1 )).ToList();
				}
				if (blnSearchByItemId) resulttocitems = tocitems.Where(t => t.ItemId.ToUpper().IndexOf(term) > -1).ToList();

				if (blnSearchByTitle) resulttocitems = tocitems.Where(t => t.Title.ToUpper().IndexOf(term) > -1).ToList();

				if (blnSearchByType) resulttocitems = tocitems.Where(t => t.Type.ToUpper().IndexOf(term) > -1).ToList();

				if (blnSearchDueDate)
					resulttocitems = tocitems.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == Convert.ToDateTime(term).Date).ToList();

				if (!resulttocitems.IsNullOrEmpty())
				{
					response.results = resulttocitems;
					return response;
				}
			}

			//TODO: Remove hardcoded text from here (SK)
			response.error_message = "No results found";
			return response;
		}
		#endregion

		#region "Content Sync"

		/// <summary>
		/// Retrieves a collection of items that need to be synced from DLAP to the central content 
		/// repository that have been modified in a specific course since the given date.
		/// </summary>
		/// <param name="courseId">DLAP Course Id of the course to sync items from.</param>
		/// <param name="modifiedSince">The date and time to filter modified items by.</param>
		/// <returns>Collection of items that have been modified and need to be synced.</returns>
		[System.Web.Http.AllowAnonymous]
		[HttpGet]
		[System.Web.Http.ActionName("Sync")]
		public SyncItemList Sync(string courseId, DateTime modifiedSince)
		{
			var response = ApiContentActions.GetItemsToSync(from: courseId, since: modifiedSince);

			return response;
		}

		#endregion

		#region "Browse More Resources"

		///// <summary>
		///// DONE: Returns main view for Browse More Resources window
		///// </summary>
		///// <returns></returns>
		//[System.Web.Http.HttpGet]
		//[System.Web.Http.ActionName("FacePlateBrowseResources")]
		private DictionaryResponse FacePlateBrowseResources()
		{
			var response = new DictionaryResponse();
			var RssFeeds = Context.Course.RSSCourseFeeds.ToDictionary(k => k.Name, v => v.Url);
			if (!RssFeeds.Any())
			{
				response.error_message = "No results found";
				return response;
			}
			response.results = RssFeeds;
			return response;
		}

		///// <summary>
		///// DONE: Returns view for select unit dropdown
		///// </summary>
		///// <returns></returns>
		//[System.Web.Http.HttpGet]
		//[System.Web.Http.ActionName("FacePlateBrowseResourcesUnits")]
		//[OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
		private TupleDictionaryResponse FacePlateBrowseResourcesUnits()
		{
			var response = new TupleDictionaryResponse();
			string chapterId = string.Empty;
			var unitList = ApiContentActions.LoadContainerData("Launchpad", "", "faceplate").Concat(ApiContentActions.LoadContainerData("Launchpad", "PX_MULTIPART_LESSONS", "faceplate"))
				.Where(i => i is PxUnit)
				.Map(i => i as PxUnit).ToList();

			var unitTitles = new Dictionary<string, Tuple<string, bool>>();
			var searchResultsOLD = ApiSearchActions.GetFacetValues("meta-topic_dlap_e");

			var searchResults = ApiSearchActions.GetFacetValues("meta-topics/meta-topic_dlap_e");

			searchResults.FacetFields.AddRange(searchResultsOLD.FacetFields);

			foreach (var field in searchResults.FacetFields)
			{
				if (field.FieldName == "meta-topic_dlap_e" || field.FieldName == "meta-topics/meta-topic_dlap_e" && field.FieldValues.Count > 0)
				{
					foreach (var unit in unitList)
					{
						//match meta-field names with a chapter - for dropdown
						if (!string.IsNullOrWhiteSpace(unit.UnitChapter))
						{
							var unt = unit;
							var value = field.FieldValues.FirstOrDefault(v => v.Value == unt.UnitChapter);

							if (value != null)
							{
								var chapterTitle = unit.Title;
								var selected = unit.Id == chapterId;
								if (!unitTitles.Keys.Contains(chapterTitle))
									unitTitles.Add(chapterTitle, new Tuple<string, bool>(value.Value, selected));
							}
						}
					}
				}
			}

			if (!unitTitles.Any())
			{
				response.error_message = "No results found";
				return response;
			}

			response.results = unitTitles;
			return response;
		}


		/// <summary>
		/// Get Facet Resource Types by CourseId and UserRole, where userRole: 0 - for Instructor; 1 - for Student 
		/// Test: GET Content/ResourceTypes/110075? + userRole=0
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userRole"></param>
		/// <returns></returns>
		//[System.Web.Http.AllowAnonymous]
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("ResourceTypes")]
		public FacetedSearchResultsResponse GetResourceTypes(string id, string userRole)
		{

			var typeOfUser = (UserType)Enum.Parse(typeof(UserType), userRole);

			ApiHelper.InitializeInstructorPxBusinessContext(Context, ApiCourseActions, ApiUserActions, id, userRole);

			Context.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Student;

			if (typeOfUser == UserType.Instructor) Context.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Instructor;

			var response = new FacetedSearchResultsResponse();
			var model = ApiSearchActions.FacePlateBrowseResourcesFacets();

			if (!model.FacetFields.Any())
			{
				response.error_message = "No results found";
				return response;
			}

			response.results = model;
			return response;
		}


		///// <summary>
		///// DONE: Returns view for facet types list
		///// </summary>
		///// <returns></returns>
		//[System.Web.Http.HttpGet]
		//[System.Web.Http.ActionName("FacePlateBrowseResourcesFacetsInstructorConsole")]
		//[OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
		private FacetValueListResponse FacePlateBrowseResourcesFacetsInstructorConsole(IEnumerable<string> fieldNames)
		{
			var response = new FacetValueListResponse();
            var model = new List<PX.PXPub.Models.FacetValue>();
			foreach (var fieldName in fieldNames)
			{
				var searchResults = ApiSearchActions.GetFacetValues(fieldName);

				if (searchResults.FacetFields.Count > 0)
				{
					model.AddRange(searchResults.FacetFields.First().FieldValues);
				}
			}

			if (!model.Any())
			{
				response.error_message = "No results found";
				return response;
			}

			response.results = model;

			return response;
		}


		///// <summary>
		///// DONE:Returns view for select types dropdown
		///// </summary>
		///// <returns></returns>
		//[OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
		//[System.Web.Http.HttpGet]
		//[System.Web.Http.ActionName("FacePlateBrowseResourcesTypes")]
		private DictionaryResponse FacePlateBrowseResourcesTypes()
		{
			var response = new DictionaryResponse();

			var searchResults = ApiSearchActions.GetFacetValues("meta-content-type_dlap_e");

			var types = new Dictionary<string, string>();

			searchResults.FacetFields.ForEach(f => f.FieldValues.ForEach(v => types.Add(v.Value, v.Value)));

			var nonInstructorStudentItems = new Dictionary<string, string>();

			foreach (var type in types)
			{
				if (!type.Key.ToLower().StartsWith("instructor_") && !type.Key.ToLower().StartsWith("student_"))
				{
					nonInstructorStudentItems.Add(type.Key, type.Value);
				}
			}

			nonInstructorStudentItems.OrderBy(i => i.Key);

			foreach (var item in nonInstructorStudentItems)
			{
				types.Remove(item.Key);
			}
			types = types.Concat(nonInstructorStudentItems).ToDictionary(x => x.Key, x => x.Value);

			if (!types.Any())
			{
				response.error_message = "No results found";
				return response;
			}

			response.results = types;
			return response;

		}


		/// <summary>
		/// Get Resources By Item(courseId, role, itemId), where userRole: 0 - for Instructor; 1 - for Student 
		/// Searches for resources by Item
		/// Example to TEST: GET Content/ResourcesByItem/110079? + userRole=0 + itemid=ANGEL_econportal__stoneecon2__master_89C6FA555B6E82D493B0CE1735E90008
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userRole"></param>
		/// <param name="itemid"></param>
		/// <returns></returns>
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("ResourcesByItem")]
		public ApiSearchResultDocListResponse GetResourcesByItem(string id, string userRole, string itemid)
		{

			var typeOfUser = (UserType)Enum.Parse(typeof(UserType), userRole);

			ApiHelper.InitializeInstructorPxBusinessContext(Context, ApiCourseActions, ApiUserActions, id, userRole);

			Context.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Student;

			if (typeOfUser == UserType.Instructor) Context.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Instructor;

			var item = ApiContentActions.GetContent(id, itemid);

			//if (Context.Course.CourseType != CourseType.XBOOK.ToString())
			//{
            var query = new PX.PXPub.Models.SearchQuery()
							{
								ExactPhrase = item.Title,
								//"Chapter 1. Exploring Economics",
								MetaIncludeFields = "meta-topics/meta-topic",
								Start = 0,
								Rows = 100
							};

			return ApiSearchActions.FacePlateBrowseResourcesResults(query);
			//}
			//else
			//{
			//    return ApiContentActions.GetRelatedXBookItems(id, itemid);
			//}
		}


		/// <summary>
		/// Get XBook Resources By Item(courseId, role, itemId), where userRole: 0 - for Instructor; 1 - for Student 
		/// Searches for resources by Item
		/// Example to TEST: GET Content/XResourcesByItem/110079? + userRole=0 + itemid=ANGEL_econportal__stoneecon2__master_89C6FA555B6E82D493B0CE1735E90008
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userRole"></param>
		/// <param name="itemid"></param>
		/// <returns></returns>
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("XResourcesByItem")]
		public PxContentItemsListResponse GetXResourcesByItem(string id, string userRole, string itemid)
		{
			var typeOfUser = (UserType)Enum.Parse(typeof(UserType), userRole);

			ApiHelper.InitializeInstructorPxBusinessContext(Context, ApiCourseActions, ApiUserActions, id, userRole);

			Context.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Student;

			if (typeOfUser == UserType.Instructor) Context.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Instructor;

			return ApiContentActions.GetRelatedXBookItems(id, itemid);
		}


		/// <summary>
		/// Get Resources By Type(courseId, role, resourceType), where userRole: 0 - for Instructor; 1 - for Student 
		/// Searches for resources by meta content type
		/// Example to TEST: GET Content/ResourcesByType/110075? + userid=87414 + userRole=0 + resourceType=By%20the%20Number%20Quizzes
		/// </summary>
		/// <param name="id"></param>
		/// <param name="userRole"></param>
		/// <param name="resourceType"></param>
		/// <returns></returns>
		[System.Web.Http.HttpGet]
		[System.Web.Http.ActionName("ResourcesByType")]
		public ApiSearchResultDocListResponse GetResourcesByType(string id, string userRole, string resourceType)
		{
			var typeOfUser = (UserType)Enum.Parse(typeof(UserType), userRole);

			ApiHelper.InitializeInstructorPxBusinessContext(Context, ApiCourseActions, ApiUserActions, id, userRole);

			Context.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Student;

			if (typeOfUser == UserType.Instructor) Context.AccessLevel = PX.Biz.ServiceContracts.AccessLevel.Instructor;

            var query = new PX.PXPub.Models.SearchQuery()
							{
								ExactPhrase = String.Format("{0}_{1}", typeOfUser, resourceType),
								//instructor_ + type
								MetaIncludeFields = "meta-content-type",
								Start = 0,
								Rows = 100
							};


			return ApiSearchActions.FacePlateBrowseResourcesResults(query);

		}


		/// <summary>
		/// Returns list of top level removed items
		/// </summary>
		/// <param name="id">Course Id</param>
		/// <returns></returns>		
		[HttpGet]
		[ActionName("LaunchpadBrowseRemoved")]
        public TableofContentsItemListResponse LaunchpadBrowseRemoved(string id)
		{
            return ApiSearchActions.LaunchpadBrowseRemoved(id);
		}

		///// <summary>
		///// Loads RSS feed( will be implemented if we need)
		///// </summary>
		///// <returns></returns>
		//[OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
		//[System.Web.Http.HttpGet]
		//[System.Web.Http.ActionName("FacePlateBrowseRSS")]
		private ApiSearchResultDocListResponse FacePlateBrowseRSS(string rssUrl)
		{
			return ApiSearchActions.FacePlateBrowseRSS(rssUrl);

		}


		///// <summary>
		///// DONE: Searches for resources by meta values
		///// </summary>
		///// <param name="IncludeWords"> </param>
		///// <param name="Start"> </param>
		///// <param name="Rows"> </param>
		///// <param name="questionSearch"> </param>
		///// <returns></returns>
		//[ValidateInput(false)]
		//[OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
		//[System.Web.Http.HttpGet]
		//[System.Web.Http.ActionName("FacePlateBrowseResourcesStringSearch")]
		private ApiSearchResultDocListResponse FacePlateBrowseResourcesStringSearch(string IncludeWords, int Start, int Rows, bool questionSearch = false)
		{
			return ApiSearchActions.FacePlateBrowseResourcesStringSearch(IncludeWords, Start, Rows, questionSearch);
		}

		///// <summary>
		///// DONE: Loads user resources
		///// </summary>
		///// <returns></returns>
		//[OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
		//[System.Web.Http.HttpGet]
		//[System.Web.Http.ActionName("FacePlateBrowseMyResources")]
		private ApiSearchResultDocListResponse FacePlateBrowseMyResources()
		{
			return ApiSearchActions.FacePlateBrowseMyResources();
		}

		///// <summary>
		///// Shows a list of questions banks of a given type (will be implemented if needed)
		///// </summary>
		///// <param name="collectiontype"></param>
		///// <returns></returns>
		//[OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "url")]
		//[System.Web.Http.HttpGet]
		//[System.Web.Http.ActionName("FacePlateBrowseMyResources")]
		private Response ResourceQuestionBanks(string collectiontype)
		{
			//return View("ResourceQuestionBanks", (object)collectiontype);

			var response = new Response { error_message = "No results found" };
			return response;
		}




		#endregion


	}


}


