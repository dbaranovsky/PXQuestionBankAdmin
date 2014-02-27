using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Bfw.Agilix.DataContracts;
using Bfw.PX.Biz.DataContracts;
using LearningCurveQuestionSettings = Bfw.PX.Biz.DataContracts.LearningCurveQuestionSettings;
using Question = Bfw.PX.Biz.DataContracts.Question;
using QuestionAnalysis = Bfw.PX.Biz.DataContracts.QuestionAnalysis;
using Resource = Bfw.PX.Biz.DataContracts.Resource;

namespace Bfw.PX.Biz.ServiceContracts
{
	/// <summary>
	/// Provides methods to retrieve, store, and otherwise manipulate content.
	/// </summary>
	public interface IContentActions
	{
        /// <summary>
        /// The id of the parent that needs to be set on an item so that it shows up in the gradebook
        /// </summary>
        string GradableParentId { get; }

        /// <summary>
        /// Lists the children.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="categoryId">The category id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="loadChild">The flag if loading of chidren is required</param>
        /// <returns></returns>
        IEnumerable<Agilix.DataContracts.Item> ListChildren(string entityId, string parentId, string categoryId, string userId, bool loadChild = false);

        /// <summary>
        /// List the descendents including student items
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">item id</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        IEnumerable<Agilix.DataContracts.Item> ListDescendents(string entityId, string itemId, string userId);

		/// <summary>
		/// Saves Row Item xml
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="itemDataXml"> </param>
		/// <param name="contentItemId"> </param>
		void SaveRowItem(string entityId, XElement itemDataXml, string contentItemId);

		/// <summary>
		/// Find List of ContentItems based on specified ItemsSearch Query
		/// </summary>
		/// <param name="search"></param>
		/// <param name="asAdmin"></param>
		/// <param name="categoryId"></param>
		/// <returns></returns>
		IEnumerable<ContentItem> FindContentItems(ItemSearch search, bool asAdmin = true, string categoryId = null);

        /// <summary>
        /// Find List of ContentItems based on specified ItemsSearch Query
        /// </summary>
        /// <param name="search"></param>
        /// <param name="asAdmin"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        IEnumerable<ContentItem> FindItemsBatch(List<ItemSearch> search, bool asAdmin = true);

		/// <summary>
		/// Provides access to the BusinessContext that the service is running under.
		/// </summary>
		IBusinessContext Context { get; }

		/// <summary>
		/// Retrieves all items that are identified as featured.
		/// </summary>
		/// <param name="entityId"></param>
		/// <returns></returns>
		IEnumerable<ContentItem> ListFeaturedItems(string entityId);

        /// <summary>
        /// Items that are in the TOC can be considered featured if they have a bfw_property for 'is_featured'
        /// This method will return a list of featured items from with in the course
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="featuredState">
        /// Three states are possible [true, false, null]
        /// true: only returns featured content that has an attribute of true
        /// false: only return featured content that has an attribute of false or empty
        /// null: returns any items that have the bfw_property 'is_featured'
        /// </param>
        /// <returns>List of Featured Content Items</returns>
        IEnumerable<ContentItem> ListItemsThatAreFeatured(string entityId, bool? featuredState = null);

		/// <summary>
		/// Retrieves the item specified, without loading associated resources.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		ContentItem GetContent(string entityId, string id);

		/// <summary>
		/// Retrieves the item specified. If loadResources is true then any resource
		/// pointed to by the Href property (if it is NOT a fully qualified URL) will also
		/// be loaded.
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="id"></param>
		/// <param name="loadResources"></param>
		/// <returns></returns>
		ContentItem GetContent(string entityId, string id, bool loadResources);

		/// <summary>
		/// Retrieves the item specified. If loadResources is true then any resource
		/// pointed to by the Href property (if it is NOT a fully qualified URL) will also
		/// be loaded.
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="id"></param>
		/// <param name="loadResources"></param>
		/// <param name="categoryid"></param>
		/// <returns></returns>
		ContentItem GetContent(string entityId, string id, bool loadResources, string categoryid);


		/// <summary>
		/// Returns the matching resource.
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="resourceUri"></param>
		/// <returns></returns>
		Resource GetResource(string entityId, string resourceUri);

		/// <summary>
		/// Returns the matching resource stream.
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="resourceUri"></param>
		/// <returns></returns>
		Stream GetResourceStream(IEnumerable<Resource> resources, out string fileName);


		/// <summary>
		/// Returns the matching resources.
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="resourceUri"></param>
		/// <returns></returns>
		IEnumerable<Resource> ListResources(string entityId, string resourceUri, string xQuery);


		/// <summary>
		/// Returns the matching resources.
		/// </summary>
		/// <param name="resourceIds"></param>
		/// <returns></returns>
		IEnumerable<Resource> ListResources(string resourceIds, string enrollmentId);

		/// <summary>
		/// Returns the matching resources.
		/// </summary>
		/// <param name="resourceIds"></param>
		/// <returns></returns>
		IEnumerable<Resource> ListResources(IEnumerable<string> resourceIds, string enrollmentId);

        /// <summary>
        /// Returns the info for matching resources. It does not load the stream content.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="resourceUri"></param>
        /// <param name="xQuery"></param>
        /// <returns></returns>
        IEnumerable<Resource> ListResourcesInfo(string entityId, string resourceUri, string xQuery);

		/// <summary>
		/// Gets the list of item and its descendents based on item id.
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="itemId"></param>
		/// <returns></returns>
		IEnumerable<ContentItem> ListDescendentsAndSelf(string entityId, string itemId);

		/// <summary>
		/// Gets the immediate children of the specified parent item, if any.
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		IEnumerable<ContentItem> ListChildren(string entityId, string parentId);

		/// <summary>
		/// Gets the children of the specified parent item for a specified number of levels, if any.
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="parentId"></param>
		/// <returns></returns>
		IEnumerable<ContentItem> ListChildren(string entityId, string parentId, int depth, string categoryId, bool skipFirst = true);

		/// <summary>
		/// Loads all items for a specific container
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="containerId"></param>
		/// <param name="subcontainerId"></param>
        /// <param name="extraQueryFilter"></param>
		/// <returns></returns>
        IEnumerable<ContentItem> GetContainerItems(string entityId, string containerId, string subcontainerId, string toc = "", string extraQueryFilter = "");

	    /// <summary>
	    /// Get items for a specific container under a specific parent item (folder) in this container
	    /// </summary>
	    /// <param name="entityId"></param>
	    /// <param name="containerId"></param>
	    /// <param name="subcontainerId"></param>
	    /// <param name="toc"></param>
	    /// <param name="parentId"></param>
	    /// <returns></returns>
	    IEnumerable<ContentItem> GetContainerItemsForParent(string entityId, string containerId, string subcontainerId, string parentId, string toc = "");


        /// <summary>
        /// Copy content resource to other entity
        /// </summary>
        /// <param name="content"></param>
        /// <param name="targetEntityid"></param>
        void CopyResourceToAnotherDomain(string destinationDomainid, string destinationPath, string sourcePath, string sourceDomain);
        

		/// <summary>
		/// Performs search on specific query parameters using GetItemsList
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="queryParams"></param>
		/// <param name="op">operation</param>
		/// <returns></returns>
		IEnumerable<ContentItem> DoItemsSearch(string entityId, Dictionary<string, string> queryParams, string op = "OR");

		/// <summary>
		/// Retrieves all the content in the entity.
		/// </summary>
		/// <param name="entityId"></param>
		/// <returns></returns>
		IEnumerable<ContentItem> ListContent(string entityId);

        /// <summary>
        /// Retrieves the content of specific subtype in the entity.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="subType"></param>
        /// <returns></returns>
        IEnumerable<ContentItem> ListContent(string entityId, string subType);

        /// <summary>
        /// Retrieves all the content in the entity.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="listStudentItems">List Student Content</param>
        /// <returns></returns>
        IEnumerable<ContentItem> ListContent(string entityId, string subType, bool listStudentItems);

		/// <summary>
		/// List Content for Dropbopx
		/// </summary>
		/// <param name="entityId"></param>
		/// <param name="itemid"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		List<ContentItem> ListContentForDropBox(string entityId, string itemid, string type);

		/// <summary>
		/// Retrieves all content that has a due date assigned.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="rootFolder">The root folder.</param>
        /// <returns></returns>
		IEnumerable<ContentItem> ListContentWithDueDates(string entityId, string rootFolder);

        /// <summary>
        /// Retrieves all content that has a due date assigned.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="containerId">The container id (for example launchpad).</param>
        /// <param name="subcontainerId">The chapter.</param>
        /// <param name="toc">The toc content belongs to</param>
        /// <returns></returns>
        IEnumerable<ContentItem> ListContentWithDueDates(string entityId, string containerId, string subcontainerId, string toc);

		/// <summary>
		/// Retrieves all content that has a due date assigned.
		/// </summary>
		/// <param name="entityId">The entity id.</param>
		/// <param name="fromDate">From Date</param>
		/// <param name="toDate">To Date</param>
		/// <returns></returns>
		IEnumerable<ContentItem> ListContentWithDueDatesWithinRange(string entityId, string fromDate, string toDate);

		/// <summary>
		/// Retrieves all content that should be loaded into the assignment center's left hand side.
		/// </summary>
		/// <param name="entityId">Id of the entity the assignment center is located in.</param>
		/// <returns>All assignment center content</returns>
		IEnumerable<ContentItem> ListContentForAssignmentCenter(string entityId);

		/// <summary>
		/// Retrieves all the uploaded and composed materials for the course (entity)
		/// </summary>
		/// <param name="entityId">Course / Entity ID</param>
		/// <returns>ContentItem</returns>
		List<ContentItem> ListContentForCourseMaterials(string entityId);

		/// <summary>
		/// Returns assest items corresponding to the resource items
		/// </summary>
		/// <param name="entityID"></param>
		/// <returns></returns>
		List<ContentItem> ListAssestsForCourseMaterials(string entityID, List<ContentItem> results);

		/// <summary>
		/// Gets the collection of supported template items for the given course/entityid.
		/// </summary>
		/// <param name="entityId">The entity ID.</param>
		/// <param name="templateName">Name of the template.</param>
		/// <returns></returns>
		IEnumerable<ContentItem> GetTemplateItems(string entityId, string templateName);

		/// <summary>
		/// Gets the collection of template items for a user created template for the given course/entityid.
		/// </summary>
		/// <param name="entityId">The entity ID.</param>
		/// <param name="templateName">Name of the template.</param>
		/// <returns></returns>
		IEnumerable<ContentItem> GetDerivedTemplateItems(string entityId, string templateName);
        
        /// <summary>
        /// Stores the specified piece of content and associated resources
        /// </summary>
        /// <param name="content">The content to store.</param>
        void StoreContent(ContentItem content, string entityId = "");

		/// <summary>
		/// Stores the specified piece of content and associated resources
		/// </summary>
		/// <param name="content">The content to store.</param>
		/// <param name="storeLocked">bool flag to indicate whether to store a flagged item.</param>
		void StoreContent(ContentItem content, bool storeLocked);

		/// <summary>
		/// Stores the specified piece of content and associated resources
		/// </summary>
		/// <param name="content">The content to store.</param>
		/// <param name="storeLocked">bool flag to indicate whether to store a flagged item.</param>
		/// <param name="entityId"></param>
        void StoreContent(ContentItem content, string entityId, bool storeLocked);

		/// <summary>
		/// Stores the specified piece of content and associated resources
		/// </summary>
		/// <param name="content">The content to store.</param>
		/// <param name="storeLocked">bool flag to indicate whether to store a flagged item.</param>
		/// <param name="entityId"></param>
		/// <param name="ignoreCategory">category name which we want to ignore when adding item</param>param>
        void StoreContent(ContentItem content, string entityId, string ignoreCategory);

		/// <summary>
		/// Copy content item to other entity
		/// </summary>
		/// <param name="content"></param>
		/// <param name="entityId"></param>
		/// <param name="targetEntityid"></param>
		void CopyItemToAnotherEntity(ContentItem content, string entityId, string targetEntityid);

		/// <summary>
		/// Copy content resource to other entity
		/// </summary>
		/// <param name="content"></param>
		/// <param name="targetEntityid"></param>
		void CopyResourceToAnotherEntity(ContentItem content, string targetEntityid);

		/// <summary>
		/// Copy content item to other entity
		/// </summary>
		/// <param name="content"></param>
		/// <param name="entityId"></param>
		/// <param name="targetEntityid"></param>
		/// <param name="lockedCourseType"></param>
		void CopyItemToAnotherEntity(ContentItem content, string entityId, string targetEntityid, string lockedCourseType);

		/// <summary>
		/// Create and save a new content item that is identical to the first, but which has a new ID
		/// This is necessary so that we can create copies or overwrite existing items, but without
		/// regenerating the content
		/// </summary>
		/// <param name="entityId">The entity ID.</param>
		/// <param name="fromItemId">From item ID.</param>
		/// <param name="toItemId">To item id.</param>
		/// 
		[Obsolete("This method is obsolete; Please use CopyItem(string,string,string,string,IEnumerable<TocCategory>) instead")]
		void CopyItem(string entityId, string fromItemId, string toItemId);

		/// <summary>
		/// Copies an existing item and adds it to the speicified categories and parentid.
		/// </summary>
		/// <param name="entityId">Id of the entity to which the item should belong.</param>
		/// <param name="fromItemId">Id of the item to copy.</param>
		/// <param name="toItemId">Id of the new item.</param>
		/// <param name="categories">Categories to add the item to.</param>
		/// <param name="parentId">Default category parent item id.</param>
		/// <param name="removeDesc">Flag to remove Description of item during copy. default is false</param>
        /// <param name="title">The title for the copy.</param>
        /// <param name="subtitle">The subTitle for the copy.</param>
        /// <param name="description">The Description for the copy.</param>
        /// <param name="hiddenFromStudent">Flag enabling/disabling visibility to students</param>
        ContentItem CopyItem(string entityId, string fromItemId, string toItemId, string parentId, IEnumerable<TocCategory> categories, bool removeDesc = false, string title = null, string subTitle = null, string description = null, bool? hiddenFromStudent = null, bool? includePoints = null);

		/// <summary>
		/// Given two existing items, copy everything from the 'from' item to the 'to' item,
		/// excluding all attributes PX is aware of.  I.e., copy only Agilix-specific attributes.
		/// </summary>
		/// <param name="entityId">The entity ID.</param>
		/// <param name="fromId">From ID.</param>
		/// <param name="toId">To ID.</param>
		void CopyItemSettings(string entityId, string fromId, string toId);
        
		/// <summary>
		/// Stores the collection of contents and associated resources.
		/// </summary>
		/// <param name="contents">The contents.</param>
        void StoreContents(IEnumerable<ContentItem> contents);

		/// <summary>
		/// Stores the collection of contents and associated resources.
		/// </summary>
		/// <param name="contents">The contents.</param>
        void StoreContents(IEnumerable<ContentItem> contents, string entityId);

		/// <summary>
		/// Calls the item service to store a collection of resources into the system.
		/// </summary>
		/// <param name="resources">The resources.</param>
		void StoreResources(IEnumerable<Resource> resources);

		/// <summary>
		/// Removes the specified resource from the entity
		/// </summary>
		/// <param name="entityId">The entity ID.</param>
		/// <param name="resourcePath">The resource path.</param>
		void RemoveResource(string entityId, string resourcePath);

		/// <summary>
		/// Removes the specified resources from the system.
		/// </summary>
		/// <param name="resources">The resources.</param>
		void RemoveResources(IList<Resource> resources);

		/// <summary>
		/// Removes the resources with given IDs.
		/// </summary>
		/// <param name="resourceIds">The resource IDs.</param>
		void RemoveResources(IEnumerable<string> resourceIds);

        /// <summary>
        /// Removes a list of resources for the same entity ID.
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="resourcePathList"></param>
        void RemoveResources(string entityId, string[] resourcePathList);


		/// <summary>
		/// Removes the specified content from the system.
		/// </summary>
		/// <param name="entityId">The entity ID.</param>
		/// <param name="itemId">The item ID.</param>
		void RemoveContent(string entityId, string itemId);

		/// <summary>
		/// Removes items with the specified IDs from the course/entity.
		/// </summary>
		/// <param name="entityId">The entity ID.</param>
		/// <param name="itemId">The item ID.</param>
		void RemoveContents(string entityId, IList<string> itemId);

		/// <summary>
		/// Marks the content as read by logging activity against it in Agilix.
		/// </summary>
		/// <param name="itemId">The item ID.</param>
		void MarkContentAsRead(string itemId);

        /// <summary>
        /// Tracks time student spent on the item
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <param name="duration">time spent on item</param>
        void StoreContentDuration(string itemId, string enrollmentId, int duration, DateTime? startDate);

		/// <summary>
		/// Lists the content read by the active user, sorted by date.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ContentItem> ListContentReadByDate();

		/// <summary>
		/// Returns the ID of a location that can store item templates.
		/// </summary>
		string TemplateFolder { get; }

		/// <summary>
		/// Returns the ID of a location that can store items temporarily.
		/// </summary>
		string TemporaryFolder { get; }

		/// <summary>
		/// Gets the collection of supported template items for the given course/entityid.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ContentItem> GetAllTemplates();

		/// <summary>
		/// Add all templates with types and subtypes that are the same as the given item.
		/// ALSO: if the item is a quiz, homeworks are related, and vice-versa.
		/// </summary>
		/// <param name="itemId">The item ID.</param>
		/// <returns></returns>
		IEnumerable<ContentItem> FindRelatedTemplates(string itemId);


		/// <summary>
		/// Find a template based on the content type
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		ContentItem FindTemplateForType(string contentType);

		/// <summary>
		/// Lists all items related to itemId by taxonomy.
		/// </summary>
		/// <param name="itemId">ID of the item to find related items for.</param>
		/// <param name="entityId">ID of the entity the taxonomy relationship should exist in.</param>
		/// <returns>Enumeration of all relationships to itemId by taxonomy.</returns>
		IEnumerable<TaxonomyRelationship> FindRelatedItems(string itemId, string entityId);

		/// <summary>
		/// Gets gradebook detail for the specified user.
		/// </summary>
		/// <param name="items">ID of the user for which to get grades.</param>
		/// <param name="entityId">Optional entity ID by which to filter the returned data.</param>        
		/// <returns></returns>
		IEnumerable<DataContracts.Grade> GetGradesPerItem(IList<Biz.DataContracts.ContentItem> items, string entityId);

		/// <summary>
		/// Updates the given items in assignment center to reflect their current state.
		/// </summary>
		/// <param name="categoryId">id of the category the items belong to</param>
		/// <param name="items">Items to update</param>
        /// <param name="entityid">entity id the item belongs to</param>
        void UpdateAssignmentCenterItems(string categoryId, IEnumerable<Biz.DataContracts.AssignmentCenterItem> items, 
            string toc, string entityid = "" );


		/// <summary>
		/// Updates the given items in assignment center to reflect their current state [unassign].
		/// </summary>
		/// <param name="items"></param>
        void UnAssignAssignmentCenterItems(string categoryId, IEnumerable<Biz.DataContracts.AssignmentCenterItem> items, string toc, bool keepInGradebook);

		/// <summary>
		/// Updates the given assignment center category.
		/// </summary>
		/// <param name="entityId">id of the entity in which the category exists.</param>
		/// <param name="category">category to update.</param>
		void UpdateAssignmentCenterCategory(string entityId, Biz.DataContracts.AssignmentCenterCategory category);

		/// <summary>
		/// Get the list of items stored only for the student.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Biz.DataContracts.ContentItem> GetAllStudentItems(string categoryId);

		/// <summary>
		/// Generates the ID of the resource containing student items.
		/// </summary>
		/// <param name="enrollmentId"></param>
		string GetStudentResourceId(string enrollmentId);

		/// <summary>
		/// Generates the ID of the resource containing student items.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="parentId"></param>
		/// <param name="sequence"></param>
		/// <param name="studentEnrollmentId"></param>
		IEnumerable<ContentItem> UpdateStudentItems(string id, string parentId, string sequence, string categoryId);


		/// <summary>
		/// Update item from student resource file.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="parentId"></param>
		/// <param name="sequence"></param>
		/// <param name="studentEnrollmentId"></param>
		ContentItem UpdateStudentItemsForStudents(string id, string parentId, string sequence, string categoryId);


        /// <summary>
		/// Generates an empty XML document of student items.
		/// </summary>
		/// <returns></returns>
		XDocument GetEmptyStudentDoc();

        /// <summary>
        /// Gets the children of the items.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemIds">The item ids.</param>
        /// <param name="tocDefinition">The name of the node element that defines the TOC</param>
        /// <param name="includingShortCuts">removed all shortcut items if false</param>
        /// <returns></returns>
        IEnumerable<DataContracts.ContentItem> GetChildItems(string entityId, List<string> itemIds, String tocDefinition, bool includingShortCuts = false);

        /// <summary>
        /// Get Items
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="itemIds"></param>
        /// <param name="includingShortCuts"> </param>
        /// <returns></returns>
        IEnumerable<DataContracts.ContentItem> GetItems(string entityId, List<string> itemIds, bool includingShortCuts = false);
        
        /// <summary>
        /// Gets the list of assignment folders in a course
        /// </summary>
        IEnumerable<ContentItem> GetAssignmentFolders();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        IEnumerable<ContentItem> GetRelatedItems(string entityId, string itemId);

        /// <summary>
        /// Performs search for items.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="queryParams">Query to use for search.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="op">The operation parameter.</param>
        /// <returns></returns>
        List<Agilix.DataContracts.Item> DoItemSearch(string entityId, Dictionary<string, string> queryParams, string userId, string op = "OR");

	    /// <summary>
	    /// Gets the raw item.
	    /// </summary>
	    /// <param name="entityId">The entity id.</param>
	    /// <param name="itemId">The item id.</param>
	    /// <returns></returns>
	    XDocument GetRawItem(string entityId, string itemId);


        /// <summary>
        /// Sets the gradebook category for.
        /// </summary>
        /// <param name="itemId">The item unique identifier.</param>
        /// <param name="categoryId">The category unique identifier.</param>
        /// <returns></returns>
        bool SetGradebookCategoryFor(string itemId, string categoryId, string entityId);

        /// <summary>
        /// Gets the item links
        /// </summary>
        /// <param name="entityId">The entity id</param>
        /// <returns></returns>
        IEnumerable<Bfw.PX.Biz.DataContracts.ItemLink> GetItemLinks(string entityId);
	}
}