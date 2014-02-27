using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using Bdc = Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PXWebAPI.Mappers;
using Bfw.PXWebAPI.Models;
using Bfw.PXWebAPI.Models.Response;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using Resource = Bfw.PXWebAPI.Models.Resource;
using XmlResource = Bfw.PXWebAPI.Models.XmlResource;

namespace Bfw.PXWebAPI.Helpers
{
	/// <summary>
	/// IContentActions
	/// </summary>
	public interface IApiContentActions
	{

		/// <summary>
		/// Loads all items for a specific  container
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="itemId"></param>
		/// <returns></returns>
		PxContentItemsListResponse GetRelatedXBookItems(string entityId, string itemId);


		/// <summary>
		/// Get Item and the List of Descendents
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="itemid"></param>
		/// <param name="depth"></param>
		/// <param name="category"></param>
		/// <returns></returns>
		List<Bfw.PX.PXPub.Models.ContentItem> GetPxTableOfContent(string courseid, string itemid, int depth = 1,
		                                                          string category = "syllabusfilter");

		/// <summary>
		/// LoadContainerData
		/// </summary>
		/// <param name="containerId"></param>
		/// <param name="subContainerId"></param>
		/// <param name="mode"></param>
		/// <param name="toc"></param>
		/// <returns></returns>
		List<PX.PXPub.Models.ContentItem> LoadContainerData(string containerId, string subContainerId, string mode,
																   string toc = "");

		/// <summary>
		/// Get Items
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="itemids"></param>
		/// <returns></returns>
		List<Bfw.PX.PXPub.Models.ContentItem> PxGetItems(string courseid, string itemids);


		/// <summary>
		/// GetItem PX.PXPub.Models.ContentItem
		/// </summary>
		/// <param name="itemId"></param>
		/// <returns></returns>
		PX.PXPub.Models.ContentItem GetItem(string itemId);

		/// <summary>
		/// Get Items
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="itemids"></param>
        /// <param name="toc"></param>
		/// <param name="query"></param>
		/// <returns>List of ContentItems </returns>
		List<Models.ContentItem> GetItems(string courseid, string itemids, string toc, string query = "");

		/// <summary>
		/// GetTableofContents one level deep
		/// </summary>
		/// <param name="itemid"></param>
		/// <param name="courseid"></param>
		/// <param name="depth"></param>
        /// <param name="toc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        List<TableofContentsItem> GetTableofContents(string itemid, string courseid, int depth = 1, string toc = "syllabusfilter", string container = "Launchpad");

		/// <summary>
		/// Returns the matching resource.
		/// </summary>
		/// <param name="entityId">Id of the entity to load resource from.</param>
		/// <param name="resourceUri">Uri to the resource.</param>
		/// <returns></returns>
		Resource GetResource(string entityId, string resourceUri);

		/// <summary>
		/// GetContainerItems
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="containerId"></param>
		/// <param name="subcontainerId"></param>
		/// <param name="toc"></param>
		/// <returns></returns>
		List<PX.Biz.DataContracts.ContentItem> GetContainerItems(string entityId, string containerId,
																		string subcontainerId, string toc = "");

		/// <summary>
		/// DoItemsSearch
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="queryParams"></param>
		/// <param name="op"></param>
		/// <returns></returns>
		IEnumerable<PX.Biz.DataContracts.ContentItem> DoItemsSearch(string entityId, Dictionary<string, string> queryParams,
																	string op = "OR");

		/// <summary>
		/// Gets the children of the specified parent item for a specified number of levels, if any.
		/// </summary>
		/// <param name="entityId">Id of the entity from which to load items.</param>
		/// <param name="parentId">Id of the parentItem whose children are to be loaded.</param>
		/// <param name="depth">Level to which to load children.</param>
		/// <param name="categoryId">Id of the category the children must belong to.</param>
		/// <param name="skipFirst"> </param>
		/// <returns></returns>
		IEnumerable<Bfw.PX.Biz.DataContracts.ContentItem> ListChildren(string entityId, string parentId, int depth,
																	   string categoryId, bool skipFirst = true);


		/// <summary>
		/// GetRssFeeds
		/// </summary>
		/// <param name="rssUrl"></param>
		/// <param name="retrievalLimit"></param>
		/// <param name="currentPageIndex"></param>
		/// <returns></returns>
		List<RSSFeed> GetRssFeeds(string rssUrl, int retrievalLimit, int currentPageIndex);


		/// <summary>
		/// Get Biz Content Item
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="itemId"></param>
		/// <returns></returns>
		ContentItem GetContent(string entityId, string itemId);

		/// <summary>
		/// Get list of items that have been added, modified, or deleted so that
		/// they can be synced back to a host system.
		/// </summary>
		/// <param name="from">Id of the course in DLAP to sync items from</param>
		/// <param name="since">Date and time to get items added, modified, or deleted after</param>
		/// <returns>List of items that must be synced back to the host system</returns>
		SyncItemList GetItemsToSync(string from, DateTime since);
	}

	/// <summary>
	/// Content Actions
	/// </summary>
	public class ApiContentActions : IApiContentActions
	{
		#region Constants

		protected static readonly XName ITEM_VERSION_ATTR = "version";
		protected static readonly XName ITEM_CREATIONDATE_ATTR = "creationdate";
		protected static readonly XName ITEM_CREATIONBY_ATTR = "creationby";
		protected static readonly XName ITEM_MODIFIEDDATE_ATTR = "modifieddate";
		protected static readonly XName ITEM_MODIFIEDBY_ATTR = "modifiedby";
		protected static readonly XName ITEM_RESOURCEENTITYID_ATTR = "resourceentityid";
		protected static readonly XName ITEM_ACTUALENTITYID_ATTR = "actualentityid";
		protected static readonly XName ITEM_ENTITYID_ATTR = "entityid";
		protected static readonly XName ITEM_ID_ATTR = "id";
		protected static readonly XName ITEM_ITEMID_ATTR = "itemid";
		protected static readonly XName ITEM_PARENT_ELM = "parent";
		protected static readonly XName ITEM_DATA_ELM = "data";
		#endregion

		#region Properties
		/// <summary>
		/// Extension used by all PX Resource files.
		/// </summary>
		public const string PXRES_EXTENSION = "pxres";

		protected ISessionManager SessionManager { get; set; }

		protected IBusinessContext Context { get; set; }

		/// <summary>
		/// Gets or sets the API Grade Actions actions.
		/// </summary>
		/// <value>
		/// The API enrollment actions.
		/// </value>
		protected IApiGradeActions ApiGradeActions { get; set; }


		/// <summary>
		/// Gets or sets the API Item Actions actions.
		/// </summary>
		/// <value>
		/// The API enrollment actions.
		/// </value>
		protected IApiGradeBookActions ApiGradeBookActions { get; set; }


		/// <summary>
		/// Gets or sets the API enrollment actions.
		/// </summary>
		/// <value>
		/// The API enrollment actions.
		/// </value>
		protected IApiItemActions ApiItemActions { get; set; }


		/// <summary>
		/// Gets or sets the PX enrollment actions.
		/// </summary>
		/// <value>
		/// The PX enrollment actions.
		/// </value>
		protected IEnrollmentActions PxEnrollmentActions { get; set; }


		/// <summary>
		/// Gets or sets the PX Content Actions.
		/// </summary>
		/// <value>
		/// The PX Content Actions.
		/// </value>
		protected IContentActions PxContentActions { get; set; }



		/// <summary>
		/// Gets or sets the PX RSSFeed Actions.
		/// </summary>
		/// <value>
		/// The PX Content Actions.
		/// </value>
		protected IRSSFeedActions PxRSSFeedActions { get; set; }


		protected IDocumentConverter DocumentConverter { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ApiContentActions"/> class.
		/// </summary>
		/// <param name="sessionManager">The session manager.</param>
		/// <param name="context"> </param>
        public ApiContentActions(ISessionManager sessionManager, PX.Biz.ServiceContracts.IBusinessContext context, IItemQueryActions itemQueryActions, IContentActions contentActions)
		{
			SessionManager = sessionManager;
			Context = context;
			DocumentConverter = new AsposeDocumentConverter();
            var databaseManager = new Bfw.Common.Database.DatabaseManager();
            PxContentActions = contentActions;
			ApiItemActions = new ApiItemActions(sessionManager, context);
			ApiGradeBookActions = new ApiGradeBookActions(sessionManager, context);
			PxRSSFeedActions = new RSSFeedActions(context, sessionManager);
		}

		#endregion

		internal static Dictionary<string, PX.PXPub.Models.ContentItem> TempItemCache = new Dictionary<string, PX.PXPub.Models.ContentItem>();

		/// <summary>
		/// Get Item and the List of Descendents
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="itemid"></param>
		/// <param name="depth"> </param>
		/// <returns></returns>
		public List<PX.PXPub.Models.ContentItem> PxTableOfContentResponse(string courseid, string itemid, int depth = 1)
		{
			return null;
		}

		/// <summary>
		/// LoadContainerData
		/// </summary>
		/// <param name="containerId"></param>
		/// <param name="subContainerId"></param>
		/// <param name="mode"></param>
		/// <param name="toc"></param>
		/// <returns></returns>
		public List<PX.PXPub.Models.ContentItem> LoadContainerData(string containerId, string subContainerId, string mode, string toc = "")
		{
			List<PX.Biz.DataContracts.ContentItem> items = GetContainerItems(Context.CourseId, containerId, subContainerId, toc).ToList();

			var itms = items.Map(c => c.ToContentItem(PxContentActions)).ToList();

			List<PX.PXPub.Models.ContentItem> contentItems = itms;

			contentItems.Sort(ApiHelper.ContentItemSort);

			return contentItems;
		}


		/// <summary>
		/// GetItem PX.PXPub.Models.ContentItem
		/// </summary>
		/// <param name="itemId"></param>
		/// <returns></returns>
		public PX.PXPub.Models.ContentItem GetItem(string itemId)
		{
			PX.PXPub.Models.ContentItem item;
			if (TempItemCache.ContainsKey(itemId))
			{
				item = TempItemCache[itemId];
			}
			else
			{

				var dcitem = GetContent(Context.EntityId, itemId);

				item = dcitem == null ? null : dcitem.ToContentItem(PxContentActions);

				TempItemCache.Add(itemId, item);
			}
			return item;
		}


		/// <summary>
		/// Get Item and the List of Descendents
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="itemid"></param>
		/// <param name="depth"> </param>
		/// <param name="category"></param>
		/// <returns></returns>
		public List<Bfw.PX.PXPub.Models.ContentItem> GetPxTableOfContent(string courseid, string itemid, int depth = 1, string category = "syllabusfilter")
		{
			string categoryId = "syllabusfilter";
			//var tableOfContent = PxContentActions.ListDescendentsAndSelf(courseid, itemid).Select(c => c.ToContentItem(PxContentActions, loadChildren)).ToList();
			var tableOfContent = PxContentActions.ListChildren(courseid, itemid,1,categoryId).Select(c => c.ToContentItem(PxContentActions)).ToList();

			return tableOfContent;
		}

		/// <summary>
		/// GetTableofContents one level deep
		/// </summary>
		/// <param name="itemid"></param>
		/// <param name="courseid"></param>
		/// <param name="depth"></param>
        /// <param name="toc"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public List<TableofContentsItem> GetTableofContents(string itemid, string courseid, int depth = 1, string toc = "syllabusfilter", string container = "Launchpad")
		{
            var itemChildren = new List<TableofContentsItem>();
            var childrenItems = PxContentActions.ListChildren(courseid, itemid, 1, toc);
            
            if (!childrenItems.Any())
            {
                return new List<TableofContentsItem>();
            }

            Bdc.TocCategory cat = null;
            childrenItems = childrenItems.OrderBy(i =>(i.Categories != null 
                && (i.Categories.Any(c => c.Id == toc)) )? 
                    i.Categories.First(c => c.Id == toc).Sequence: (i.Sequence ?? ""));

            foreach (var childrenItem in childrenItems.Where(i => (i.GetContainer(toc) == null ? "" : i.GetContainer(toc).ToLowerInvariant()) == container.ToLowerInvariant()))
		    {
		        List<TableofContentsItem> grandChildItems = null;

		        if (depth > 1)
		        {
		            grandChildItems = GetTableofContents(childrenItem.Id, courseid, depth - 1, toc, container);
		        }

		        var tocItem = childrenItem.ToTableofContentsItem(toc);
		        itemChildren.Add(tocItem);

		        if (!grandChildItems.IsNullOrEmpty())
		        {
		            itemChildren.AddRange(grandChildItems);
		        }
		    }
            
            return itemChildren;
		}

		/// <summary>
		/// Get Items
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="itemids"></param>
		/// <param name="query"></param>
		/// <returns>List of ContentItems </returns>
		public List<Bfw.PXWebAPI.Models.ContentItem> GetItems(string courseid, string itemids, string toc, string query = "")
		{
			var contentItemlist = new List<Bfw.PXWebAPI.Models.ContentItem>();

			try
			{
				var itemidlist = itemids.Split('|');

                foreach (var itmid in itemidlist)
				{
					var items = ApiItemActions.GetItems(courseid, itmid, query);

                    if (items.IsNullOrEmpty())
                    {
                        continue;
                    }

					foreach (var item in items)
					{					    
						var pxitem = item.ToContentItem();
						var apiContentItm = pxitem.ToApiContentItemFromBiz(toc); 
						
						apiContentItm.Category = ApiHelper.GetCategoryName(courseid, item.Category, SessionManager, ApiGradeBookActions);
                        contentItemlist.Add(apiContentItm);
					}
				}

                if (contentItemlist.IsNullOrEmpty())
				{
                    contentItemlist = null;
				}
			}
			catch (Exception ex)
			{
                //TODO: this must be fixed
				contentItemlist = null;
			}

            return contentItemlist;
		}

		/// <summary>
		/// Get Items
		/// </summary>
		/// <param name="courseid"></param>
		/// <param name="itemids"></param>
		/// <returns>List of ContentItems </returns>
		public List<Bfw.PX.PXPub.Models.ContentItem> PxGetItems(string courseid, string itemids)
		{
			var contentitemlist = new List<Bfw.PX.PXPub.Models.ContentItem>();
			try
			{
				var itemidlist = itemids.Split('|');

				foreach (var itmid in itemidlist)
				{
					var itm = GetItem(itmid);
					if (itm == null) continue;
					contentitemlist.Add(itm);
				}

				if (!contentitemlist.IsNullOrEmpty())
				{
					return contentitemlist;
				}
			}
			catch (Exception ex)
			{
                //TODO: this must be fixed
				return null;
			}
			return null;
		}

		/// <summary>
		/// Returns the matching resource.
		/// </summary>
		/// <param name="entityId">Id of the entity to load resource from.</param>
		/// <param name="resourceUri">Uri to the resource.</param>
		/// <returns></returns>
		public Resource GetResource(string entityId, string resourceUri)
		{
			Resource result = LoadResource(entityId, resourceUri);
			return result;
		}

		/// <summary>
		/// Loads the resource.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="resourceUri">The resource URI.</param>
		/// <param name="query"> </param>
		/// <returns></returns>
		public Resource LoadResource(string entityId, string resourceUri, string query = "")
		{
			Resource resource = null;


			if (IsResourceUri(resourceUri))
			{
				var cmd = new GetResource { EntityId = entityId, ResourcePath = resourceUri };
				SessionManager.CurrentSession.Execute(cmd);

				if (null != cmd.Resource)
				{
					resource = cmd.Resource.ToApiResource();

					if (resource.Extension == PXRES_EXTENSION)
					{
						XmlResource xmlRes = cmd.Resource.ToApiXmlResource();
						return xmlRes;
					}

				}
			}


			return resource;
		}

		/// <summary>
		/// Determines whether [is resource URI] [the specified resource URI].
		/// </summary>
		/// <param name="resourceUri">The resource URI.</param>
		/// <returns>
		///   <c>true</c> if [is resource URI] [the specified resource URI]; otherwise, <c>false</c>.
		/// </returns>
		private bool IsResourceUri(string resourceUri)
		{
			Uri uri;

			bool result = Uri.TryCreate(resourceUri, UriKind.RelativeOrAbsolute, out uri) && !uri.IsAbsoluteUri;

			return result;
		}


		/// <summary>
		/// GetRssFeeds
		/// </summary>
		/// <param name="rssUrl"></param>
		/// <param name="retrievalLimit"></param>
		/// <param name="currentPageIndex"></param>
		/// <returns></returns>
		public List<RSSFeed> GetRssFeeds(string rssUrl, int retrievalLimit, int currentPageIndex)
		{
			return PxRSSFeedActions.GetRssFeeds(rssUrl, retrievalLimit, currentPageIndex);
		}

		/// <summary>
		/// Get Bfw.PX.Biz.DataContracts. Content Item
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="itemId"></param>
		/// <returns></returns>
		public Bfw.PX.Biz.DataContracts.ContentItem GetContent(string entityId, string itemId)
		{
			return PxContentActions.GetContent(entityId, itemId);
		}

		/// <summary>
		/// Loads all items for a specific container
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="containerId"></param>
		/// /// <param name="subcontainerId"></param>
		/// <param name="toc"> </param>
		/// <returns></returns>
		public List<PX.Biz.DataContracts.ContentItem> GetContainerItems(string entityId, string containerId, string subcontainerId, string toc = "")
		{
			var items = PxContentActions.GetContainerItems(entityId, containerId, subcontainerId, toc).ToList();

			return items;

		}

		/// <summary>
		/// DoItemsSearch
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="queryParams"></param>
		/// <param name="op"></param>
		/// <returns></returns>
		public IEnumerable<PX.Biz.DataContracts.ContentItem> DoItemsSearch(string entityId, Dictionary<string, string> queryParams, string op = "OR")
		{
			return PxContentActions.DoItemsSearch(entityId, queryParams, op);
		}

		/// <summary>
		/// Gets the children of the specified parent item for a specified number of levels, if any.
		/// </summary>
		/// <param name="entityId">Id of the entity from which to load items.</param>
		/// <param name="parentId">Id of the parentItem whose children are to be loaded.</param>
		/// <param name="depth">Level to which to load children.</param>
		/// <param name="categoryId">Id of the category the children must belong to.</param>
		/// <param name="skipFirst"> </param>
		/// <returns></returns>
		public IEnumerable<Bfw.PX.Biz.DataContracts.ContentItem> ListChildren(string entityId, string parentId, int depth, string categoryId, bool skipFirst = true)
		{
            return PxContentActions.ListChildren(entityId, parentId, depth, categoryId, skipFirst);

		}

		/// <summary>
		/// Get list of items that have been added, modified, or deleted so that
		/// they can be synced back to a host system.
		/// </summary>
		/// <param name="courseId">Id of the course in DLAP to sync items from</param>
		/// <param name="since">Date and time to get items added, modified, or deleted after</param>
		/// <returns>List of items that must be synced back to the host system</returns>
		public SyncItemList GetItemsToSync(string from, DateTime since)
		{
			var courseId = from;
			var date = since;

			var response = new SyncItemList
			{
				Error = false,
				Message = string.Empty
			};

			try
			{
				var allCourseItems = GetAllCourseItems(from: courseId);
				var itemsModified = FindCourseItemsModified(inSet: allCourseItems, since: date);
				var itemsToSync = TranslateGetItemsToSyncItems(itemsModified, courseId);
				response.SyncItems = itemsToSync;
			}
			catch (Bfw.Agilix.Dlap.DlapException ex)
			{
				response.Error = true;
				response.Message = ex.Message;
			}
			catch (FormatException ex)
			{
				response.Error = true;
				response.Message = ex.Message;
			}

			return response;
		}


		/// <summary>
		/// Loads all items for a specific container
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="itemId"></param>
		/// <returns></returns>
		public PxContentItemsListResponse GetRelatedXBookItems(string entityId, string itemId)
		{
			var response = new PxContentItemsListResponse();

			var relatedItems = PxContentActions.GetRelatedItems(entityId, itemId).Select(c => c.ToContentItem(PxContentActions)).ToList();

			if (!relatedItems.IsNullOrEmpty())
			{
				response.results = relatedItems;
				return response;
			}

			response.error_message = "No results found";
			return response;
		}

		/// <summary>
		/// Gets the XML document describing all items in a course.
		/// </summary>
		/// <param name="courseId">ID of the course that contains the items we are looking for</param>
		/// <returns>XML document containing all items in the course.</returns>
		private XElement GetAllCourseItems(string from)
		{
			var request = new Bfw.Agilix.Dlap.DlapRequest
			{
				Type = Agilix.Dlap.DlapRequestType.Get,
				Parameters = new Dictionary<string, object> {
                    { "cmd", "getitemlist" },
                    { "entityid", from }
                }
			};

			var response = SessionManager.CurrentSession.Send(request, asAdmin: true);

			if (response.Code != Agilix.Dlap.DlapResponseCode.OK)
			{
				throw new Bfw.Agilix.Dlap.DlapException("could not load item data from course");
			}

			return response.ResponseXml.Root;
		}

		/// <summary>
		/// Gets a list of items modified since a specified date.
		/// </summary>
		/// <param name="since">Date after which modifications should be synced</param>
		/// <returns>List of item elements from the document that were modified after "since"</returns>
		private IEnumerable<XElement> FindCourseItemsModified(XElement inSet, DateTime since)
		{
			var items = inSet;
			var date = since;
			IEnumerable<XElement> modifiedItems = null;

			try
			{
				modifiedItems = ( from item in items.Elements("item")
								  where date.CompareTo(DateTime.Parse(item.Attribute(ITEM_MODIFIEDDATE_ATTR).Value)) <= 0
								  select item ).ToList();
			}
			catch (Exception ex)
			{
				throw new FormatException("could not parse returned item data", ex);
			}

			return modifiedItems;
		}

		/// <summary>
		/// Takes a set of XElement objects returned by a getitem or getitemlist response and translates them
		/// into XElement objects that can be used as the body of a putitems request.
		/// </summary>
		/// <param name="items">Items that were returned by getitemlist</param>
		/// <param name="courseId">ID of the course that the items will be put into</param>
		/// <returns>Items that can be sent to DLAP in a putitems request</returns>
		private IEnumerable<SyncItem> TranslateGetItemsToSyncItems(IEnumerable<XElement> items, string courseId)
		{
			var syncItems = new List<SyncItem>();

			foreach (var item in items)
			{
				DateTime creationDate;
				DateTime modifiedDate;
				SyncStatus status;

				var id = item.Attribute(ITEM_ID_ATTR);
				var versionAttr = item.Attribute(ITEM_VERSION_ATTR);
				var resourceEntityIdAttr = item.Attribute(ITEM_RESOURCEENTITYID_ATTR);
				var actualEntityIdAttr = item.Attribute(ITEM_ACTUALENTITYID_ATTR);
				var creationDateAttr = item.Attribute(ITEM_CREATIONDATE_ATTR);
				var creationByAttr = item.Attribute(ITEM_CREATIONBY_ATTR);
				var modifiedDateAttr = item.Attribute(ITEM_MODIFIEDDATE_ATTR);
				var modifiedByAttr = item.Attribute(ITEM_MODIFIEDBY_ATTR);
				var dataElm = item.Element(ITEM_DATA_ELM);
				var parentIdElm = dataElm.Element(ITEM_PARENT_ELM);

				var itemId = id.Value;

				if (creationDateAttr == null || !DateTime.TryParse(creationDateAttr.Value, out creationDate))
				{
					throw new FormatException("creation date missing from item or is not properly formatted");
				}

				if (modifiedDateAttr == null || !DateTime.TryParse(modifiedDateAttr.Value, out modifiedDate))
				{
					throw new FormatException("modified date missing from item or is not properly formatted");
				}

				if (parentIdElm != null && parentIdElm.Value.ToLowerInvariant().CompareTo("px_deleted") == 0)
				{
					status = SyncStatus.Deleted;
				}
				else if (creationDate.CompareTo(modifiedDate) == 0)
				{
					status = SyncStatus.Created;
				}
				else if (creationDate.CompareTo(modifiedDate) != 0)
				{
					status = SyncStatus.Modified;
				}
				else
				{
					status = SyncStatus.None;
				}

				id.Remove();
				versionAttr.Remove();
				resourceEntityIdAttr.Remove();
				actualEntityIdAttr.Remove();
				creationDateAttr.Remove();
				creationByAttr.Remove();
				modifiedDateAttr.Remove();
				modifiedByAttr.Remove();

				// this is odd, but the attribute containing the "item id" is different
				// in a putitems request than it is in a getitem response
				item.Add(new XAttribute(ITEM_ITEMID_ATTR, itemId));
				item.Add(new XAttribute(ITEM_ENTITYID_ATTR, courseId));

				var content = WrapElementInPutItemsBatch(item);

				syncItems.Add(new SyncItem
				{
					Id = itemId,
					Status = status,
					ModifiedDate = modifiedDate.ToLongDateString(),
					ModifiedByUser = modifiedByAttr.Value,
					Content = content.ToString()
				});
			}

			return syncItems;
		}

		private XElement WrapElementInPutItemsBatch(XElement request)
		{
			var batch = new XElement("batch",
				new XElement("request", new XAttribute("cmd", "putitems"),
					new XElement("requests", request)
				)
			);

			return batch;
		}
	}
}
