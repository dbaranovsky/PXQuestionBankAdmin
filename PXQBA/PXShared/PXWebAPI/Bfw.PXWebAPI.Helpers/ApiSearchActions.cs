using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Xml.Linq;
using Bfw.Agilix.Commands;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common;
using Bfw.Common.Caching;
using Bfw.Common.Collections;
using Bfw.Common.Logging;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models.Response;
using DlapItemType = Bfw.PXWebAPI.Models.DlapItemType;

namespace Bfw.PXWebAPI.Helpers
{

	/// <summary>
	/// IApiSearchActions
	/// </summary>
	public interface IApiSearchActions
	{

		/// <summary>
		/// FacePlateBrowseResourcesFacets : Returns view for facet types list
		/// </summary>
        PX.PXPub.Models.FacetedSearchResults FacePlateBrowseResourcesFacets();

	    /// <summary>
		/// Loads user resources
		/// </summary>
		/// <returns></returns>
		ApiSearchResultDocListResponse FacePlateBrowseMyResources();

		/// <summary>
		///Searches for resources by meta values
		/// </summary>
		/// <param name="IncludeWords"> </param>
		/// <param name="Start"> </param>
		/// <param name="Rows"> </param>
		/// <param name="questionSearch"> </param>
		/// <returns></returns>
		ApiSearchResultDocListResponse FacePlateBrowseResourcesStringSearch(string IncludeWords, int Start, int Rows,
																				   bool questionSearch = false);

		/// <summary>
		/// Loads removed resources
		/// </summary>
		/// <returns></returns>
        TableofContentsItemListResponse LaunchpadBrowseRemoved(string entityId);

		/// <summary>
		/// Loads RSS feed( will be implemented if we need)
		/// </summary>
		/// <returns></returns>		
		ApiSearchResultDocListResponse FacePlateBrowseRSS(string rssUrl);

		/// <summary>
		/// FacePlateBrowseResourcesResults
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        ApiSearchResultDocListResponse FacePlateBrowseResourcesResults(PX.PXPub.Models.SearchQuery query);

		/// <summary>
		/// Get More resources for a specific faceted field
		/// </summary>
		/// <param name="facetName"></param>
		/// <returns></returns>
        PX.PXPub.Models.FacetedSearchResults GetFacetValues(string facetName);

		/// <summary>
		/// Builds up a SOLR syntax string based on a <see cref="PX.Biz.DataContracts.SearchQuery" /> object.
		/// </summary>
		/// <param name="query">The query to build the query string from.</param>
		/// <returns></returns>
		Bfw.PX.Biz.DataContracts.SearchParameters BuildSearchParameters(Bfw.PX.Biz.DataContracts.SearchQuery query);

		/// <summary>
		/// DoFacetedSearch
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        PX.PXPub.Models.FacetedSearchResults DoFacetedSearch(PX.PXPub.Models.SearchQuery query);
	}

	/// <summary>
	/// ApiSearchActions
	/// </summary>
	public class ApiSearchActions : IApiSearchActions
	{
		#region Properties
		/// <summary>
		/// Extension used by all PX Resource files.
		/// </summary>
		public const string PXRES_EXTENSION = "pxres";

        /// <summary>
        /// 
        /// </summary>
        protected ISearchActions SearchActions { get; set; }

        protected ICacheProvider Cache { get; set; }

		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		protected IApiContentActions ApiContentActions { get; set; }

		//determines if searches should be done against product course
		private readonly bool _doProductSearch = true;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiSearchActions"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="context"> </param>
		public ApiSearchActions(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context, IItemQueryActions itemQueryActions,
                                IContentActions contentActions, ISearchActions searchActions, ICacheProvider cache)
		{
			SessionManager = sessionManager;
		    SearchActions = searchActions;
		    Cache = cache;
			Context = context;

			_doProductSearch = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["FaceplateSearchAgainstProductCourse"]);
            ApiContentActions = new ApiContentActions(sessionManager, context, itemQueryActions, contentActions);
		}

		#endregion



		/// <summary>
		/// FacePlateBrowseResourcesFacets : Returns view for facet types list
		/// </summary>
        public PX.PXPub.Models.FacetedSearchResults FacePlateBrowseResourcesFacets()
		{			var fieldNames = new[] { "meta-content-type_dlap_e" };
			//var response = new FacetedSearchResultsResponse();
        var model = new PX.PXPub.Models.FacetedSearchResults();

			foreach (var searchResults in fieldNames.Select(fieldName => GetFacetValues(fieldName)))
			{
				model.FacetFields.AddRange(searchResults.FacetFields);
			}
			return model;

		}


		/// <summary>
		/// Loads user resources
		/// </summary>
		/// <returns></returns>
		public ApiSearchResultDocListResponse FacePlateBrowseMyResources()
		{
			var response = new ApiSearchResultDocListResponse();
			var categoryId = System.Configuration.ConfigurationManager.AppSettings["MyMaterials"];
			var children = ApiContentActions.ListChildren(Context.EntityId, "", 1, categoryId);
			const string chapterId = "";

			var searchResults = children.Select(child => ToSearchResultDoc(chapterId, child)).ToList();

			if (!searchResults.Any())
			{
				response.error_message = "No results found";
				return response;
			}

			response.results = searchResults;
			return response;
		}

		/// <summary>
		///Searches for resources by meta values
		/// </summary>
		/// <param name="IncludeWords"> </param>
		/// <param name="Start"> </param>
		/// <param name="Rows"> </param>
		/// <param name="questionSearch"> </param>
		/// <returns></returns>
		public ApiSearchResultDocListResponse FacePlateBrowseResourcesStringSearch(string IncludeWords, int Start, int Rows, bool questionSearch = false)
		{
			var response = new ApiSearchResultDocListResponse();
			const string chapterId = "";
			var query = new Models.SearchQuery { IncludeWords = IncludeWords, Start = Start, Rows = Rows, ClassType = "question" };

			if (!questionSearch)
			{
				Bfw.PX.Biz.DataContracts.SearchQuery bizQuery = query.ToSearchQuery();
				Bfw.PX.Biz.DataContracts.SearchResultSet searchResultSet = SearchActions.DoSearch(bizQuery, null, true);

				Bfw.PX.PXPub.Models.SearchResults searchResults = searchResultSet.ToSearchResults();
				searchResults.docs = searchResults.docs
												  .Filter(
													  i =>
													  !i.dlap_id.ToLower().Contains("shortcut__") &&
													  !i.CssClass.ToLower().Equals("folder") &&
													  !i.dlap_id.Contains("LOR_"))
												  .ToList();
				searchResults.docs = searchResults.docs.Distinct((s1, s2) => s1.dlap_id == s2.dlap_id).ToList();
				foreach (var doc in searchResults.docs)
				{
					ParseSearchResult(chapterId, doc);
				}

				if (!searchResults.docs.Any())
				{
					response.error_message = "No results found";
					return response;
				}

				response.results = searchResults.docs;
				return response;
			}

			response.error_message = "No results found";
			return response;

		}


		/// <summary>
		/// Loads removed resources
		/// </summary>
		/// <returns></returns>
        public TableofContentsItemListResponse LaunchpadBrowseRemoved(string entityId)
		{
            var response = new TableofContentsItemListResponse();
			var categoryId = System.Configuration.ConfigurationManager.AppSettings["FaceplateRemoved"];			

            var items = ApiContentActions.ListChildren(entityId, categoryId, 1, "syllabusfilter", false).OrderByDescending(o => o.Modified).ToList();
            var modelItems = items.Select(item => item.ToApiContentItemFromBiz()).ToList();
            var results = modelItems.Select(item => item.ToTableofContentsItem()).ToList();

		    if (!results.Any())
			{
				response.error_message = "No results found";			    
				return response;
			}

            response.results = results;
			return response;
		}


		/// <summary>
		/// FacePlateBrowseResourcesResults
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        public ApiSearchResultDocListResponse FacePlateBrowseResourcesResults(PX.PXPub.Models.SearchQuery query)
		{
			var response = new ApiSearchResultDocListResponse();
			const string chapterId = "";
			var searchQuery = query.MetaIncludeFields.Split(',').ToDictionary(field => field, field => query.ExactPhrase);

			var items =
				ApiContentActions.DoItemsSearch(Context.EntityId, searchQuery).Where(
					i => i.Type.ToLowerInvariant() != "pxunit" && i.Type.ToLowerInvariant() != "folder" && !i.Id.ToLowerInvariant().StartsWith("shortcut__"));
			if (Context.Course.CourseType.ToLower() == "ebook")
			{
				items = items.Where(item => item.Categories.Any(i => i.Id == "bfw_faceplate_filter" && i.Text == "ebook"));
			}

			// removing hidden items for student's list
			if (Context.AccessLevel == PX.Biz.ServiceContracts.AccessLevel.Student)
			{
				items = items.Filter(i => i.HiddenFromStudents == false);
			}

			List<Bfw.PX.PXPub.Models.SearchResultDoc> searchResults = items.Map(c => ToSearchResultDoc(chapterId, c)).ToList();

			searchResults = searchResults.OrderBy(d => d.dlap_title).ToList();
			//exclude folders
			searchResults.RemoveAll(d => d.dlap_itemtype == ( (int)DlapItemType.Folder ).ToString());

			if (!searchResults.Any())
			{
				response.error_message = "No results found";
				return response;
			}

			response.results = searchResults;
			return response;
		}

		/// <summary>
		/// Loads RSS feed( will be implemented if we need)
		/// </summary>
		/// <returns></returns>		
		public ApiSearchResultDocListResponse FacePlateBrowseRSS(string rssUrl)
		{
			var response = new ApiSearchResultDocListResponse();

			if (string.IsNullOrWhiteSpace(rssUrl))
			{
                var query = new PX.PXPub.Models.SearchQuery();
				FacePlateBrowseResourcesResults(query);
			}
			const string chapterId = "";
			var feeds = ApiContentActions.GetRssFeeds(rssUrl, 20, 0);

			var searchResults = feeds.Select(feed => ToSearchResultDoc(chapterId, feed)).ToList();

			if (!searchResults.Any())
			{
				response.error_message = "No results found";
				return response;
			}

			response.results = searchResults;
			return response;

		}

		/// <summary>
		/// Get More resources for a specific faceted field
		/// </summary>
		/// <param name="facetName"></param>
		/// <returns></returns>
        public PX.PXPub.Models.FacetedSearchResults GetFacetValues(string facetName)
		{
			//do a faceted search for units(chapters)
            var query = new PX.PXPub.Models.SearchQuery()
			{
				IsFaceted = true,
                FacetedQuery = new PX.PXPub.Models.FacetedSearchQuery
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
				query.EntityId = Context.Course.ProductCourseId;

            PX.PXPub.Models.FacetedSearchResults searchResults = DoFacetedSearch(query);


			return searchResults;
		}

		/// <summary>
		/// DoFacetedSearch
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
        public PX.PXPub.Models.FacetedSearchResults DoFacetedSearch(PX.PXPub.Models.SearchQuery query)
		{
			if (!query.IsFaceted) { return null; }

			var bizQuery = query.ToSearchQuery();

            var searchResults = new FacetedSearchResults {Query = query};
		    searchResults = Cache.FetchFacetedSearchResults(bizQuery);

			if (searchResults == null)
			{
				var srs = SearchActions.DoSearch(bizQuery, null, true);
				searchResults = srs.ToApiFacetedSearchResults();
                Cache.StoreFacetedSearchResults(bizQuery, searchResults);
			}
			return searchResults;
		}

		/// <summary>
		/// Builds up a SOLR syntax string based on a <see cref="PX.Biz.DataContracts.SearchQuery" /> object.
		/// </summary>
		/// <param name="query">The query to build the query string from.</param>
		/// <returns></returns>
		public PX.Biz.DataContracts.SearchParameters BuildSearchParameters(PX.Biz.DataContracts.SearchQuery query)
		{
			const string searchAllField = "text";
			var searchFields = new List<string>();
			var p1 = new PX.Biz.DataContracts.SearchParameters();
			using (Context.Tracer.DoTrace("SearchActions.BuildSearchParameters"))
			{
				p1.EntityId = !string.IsNullOrWhiteSpace(query.EntityId) ? query.EntityId : Context.EntityId;
				p1.Query = "";
				p1.Start = ( query.Start > 0 ) ? query.Start : 0;
				if (!query.IsFaceted)
				{
					p1.Rows = ( query.Rows > 0 ) ? query.Rows : 5;
				}
				else
				{ //if facet query, don't return any rows by default
					p1.Rows = ( query.Rows > 0 ) ? query.Rows : 0;
				}

				if (String.IsNullOrEmpty(query.IncludeFields) && String.IsNullOrEmpty(query.MetaIncludeFields))
				{
					searchFields.Add("dlap_" + searchAllField);
				}
				else
				{
					if (!string.IsNullOrEmpty(query.IncludeFields))
					{
						query.IncludeFields.Split(',').ToList().ForEach(type => searchFields.Add("dlap_" + type));
					}
					if (!string.IsNullOrEmpty(query.MetaIncludeFields))
					{
						query.MetaIncludeFields.Split(',').ToList().ForEach(searchFields.Add);
					}

				}

				String tempString;
				if (!String.IsNullOrEmpty(query.IncludeWords))
				{
					if (query.IncludeWords == "*")
					{
						p1.Query = "*:*";
						return p1;
					}

					tempString = String.Empty;
					foreach (var field in searchFields)
					{
						if (!String.IsNullOrEmpty(tempString)) tempString += " OR ";
						var searchString = query.IncludeWords.ToSearchString();
						if (!String.IsNullOrEmpty(searchString)) tempString += field + ":" + searchString;
					}
					if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
					p1.Query += tempString;
				}

				if (!String.IsNullOrEmpty(query.ClassType))
				{
					tempString = query.ClassType;
					if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
					p1.Query += "(" + "dlap_class:" + tempString + ")";
				}

				if (!query.MetaCategories.IsNullOrEmpty())
				{
					tempString = String.Empty;
					var catList = query.MetaCategories.ToList();
					foreach (string cat in catList)
					{
						if (!String.IsNullOrEmpty(tempString)) tempString += " OR ";
						tempString += "meta-bfw_searchcategory:" + cat;
					}
					if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
					p1.Query += "(" + tempString + ")";
				}

				if (!String.IsNullOrEmpty(query.ContentTypes))
				{
					if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
					var types = query.ContentTypes.Split(',').ToList();
					var dType = (PX.Biz.DataContracts.DlapType)Enum.Parse(typeof(PX.Biz.DataContracts.DlapType), types[0], true);
					p1.Query += "(dlap_itemtype:" + ( (int)dType ).ToString();
					foreach (string type in types)
					{
						dType = (PX.Biz.DataContracts.DlapType)Enum.Parse(typeof(PX.Biz.DataContracts.DlapType), type, true);
						p1.Query += " OR dlap_itemtype:" + ( (int)dType ).ToString();
					}
					p1.Query += ")";
				}

				if (!String.IsNullOrEmpty(query.ExcludeWords.ToSearchString()))
				{
					var words = query.ExcludeWords.Split(' ').ToList();
					if (String.IsNullOrEmpty(p1.Query))
						p1.Query = "NOT " + words[0];

					foreach (var word in words)
					{
						p1.Query += " AND NOT " + word;
					}
				}

				if (query.IncludeAssigned)
				{
					if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND meta-bfw_assigned:true";
				}

				if (!String.IsNullOrEmpty(query.ExactPhrase.ToSearchString()))
				{
					string phrase = string.Format("\"{0}\"", query.ExactPhrase);
					if (searchFields.Count > 0)
					{
						tempString = String.Empty;
						foreach (string field in searchFields)
						{
							if (!String.IsNullOrEmpty(tempString)) tempString += " OR ";
							tempString += field + ":" + phrase;
						}
						if (!String.IsNullOrEmpty(p1.Query)) p1.Query += " AND ";
						p1.Query += tempString;
					}
					else
					{
						if (String.IsNullOrEmpty(p1.Query))
							p1.Query = "\"" + query.ExactPhrase + "\"";
						else
							p1.Query += String.Format(" AND \"{0}\"", query.ExactPhrase);
					}

				}

				if (!String.IsNullOrEmpty(query.ExactQuery.ToSearchString()))
				{
					if (String.IsNullOrEmpty(p1.Query))
						p1.Query = query.ExactQuery;
					else
						p1.Query += String.Format(" AND {0}", query.ExactQuery);
				}
				if (query.IsFaceted && query.FacetedQuery != null)
				{
					p1.FacetFields = string.Join("|", query.FacetedQuery.Fields);
					p1.FacetLimit = new PX.Biz.DataContracts.FacetParam<int> { Value = query.FacetedQuery.Limit };
					p1.FacetMinCount = new PX.Biz.DataContracts.FacetParam<int> { Value = query.FacetedQuery.MinCount };
					p1.Facet = true;
				}
			}
			return p1;
		}

		#region private functions for Browse more resources


		private PX.PXPub.Models.SearchResultDoc ToSearchResultDoc(string chapterId, PX.Biz.DataContracts.RSSFeed feed)
		{

			var doc = new PX.PXPub.Models.SearchResultDoc()
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

			const bool included = false;
			const bool inuse = false;
			const string parentId = "";

			const string parentName = "";
			doc.Included = included;
			doc.InUse = inuse;
			doc.RootParentName = parentName;
			doc.RootParentId = parentId;
			return doc;
		}

		private void ParseSearchResult(string chapterId, PX.PXPub.Models.SearchResultDoc doc)
		{
			var subContainerId = doc.Metadata.FirstOrDefault(m => m.Key == "meta-subcontainerid_dlap_e").Value;
			var containerId = doc.Metadata.FirstOrDefault(m => m.Key == "meta-container_dlap_e").Value;
			doc.InUse = false;
			doc.Included = false;
			if (!string.IsNullOrWhiteSpace(subContainerId) && containerId.ToLowerInvariant() == "launchpad")
			{
				doc.Included = subContainerId == chapterId;
				var c = ApiContentActions.GetItem(subContainerId);
				if (c != null)
				{
					doc.RootParentName = c.Title;
					doc.RootParentId = subContainerId;
				}
				doc.InUse = true;
			}

		}

		private PX.PXPub.Models.SearchResultDoc ToSearchResultDoc(string chapterId, PX.Biz.DataContracts.ContentItem child, string toc ="syllabusfilter")
		{
			child.Id = child.Id.Replace("Shortcut__1__", "");
			var doc = new PX.PXPub.Models.SearchResultDoc()
			{
				entityid = Context.EntityId,
				doc_class = "item",
				dlap_class = "item",
				itemid = child.Id,
				score = "1",
				dlap_hiddenfromstudent = child.HiddenFromStudents.ToString(),
				dlap_id = child.Id,
				dlap_itemtype = child.Type,
				dlap_contenttype = child.SubTitle,
				dlap_title = child.Title,
				dlap_subtitle = child.SubTitle,
				dlap_html_text = "",
				dlap_text = "",

			};
			child.FacetMetadata.ToList().ForEach(i => doc.Metadata.Add(i.Key, i.Value));

			bool included = false, inuse = false;
			var parentName = "";
			var parentId = "";


            if (!string.IsNullOrWhiteSpace(child.GetSubContainer(toc)) && child.GetContainer(toc).ToLowerInvariant() == "launchpad")
			{
				inuse = true;
                parentId = child.GetSubContainer(toc);
				included = parentId == chapterId;
				var item = ApiContentActions.GetItem(parentId);
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

		#endregion
	}
}
