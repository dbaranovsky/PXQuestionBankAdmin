using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers.Contracts
{
    public interface IContentHelper
    {
        /// <summary>
        /// Gets a value indicating whether this instance has access level as instructor.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has access level as instructor; otherwise, <c>false</c>.
        /// </value>
        bool HasAccessLevelAsInstructor { get; }

        /// <summary>
        /// Gets a value indicating whether this instance has access level as student.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has access level as student; otherwise, <c>false</c>.
        /// </value>
        bool HasAccessLevelAsStudent { get; }

        /// <summary>
        /// Stores the link collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="title">The title.</param>
        /// <param name="url">The URL.</param>
        /// <param name="courseId">The course id.</param>
        void StoreLinkCollection(LinkCollection collection, string title, string url, string courseId);

        /// <summary>
        /// Stores the link.
        /// </summary>
        /// <param name="lnk">The LNK.</param>
        /// <param name="courseId">The course id.</param>
        void StoreLink(Link lnk, string courseId);

        /// <summary>
        /// Stores the link.
        /// </summary>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="link">The link.</param>
        /// <param name="url">The URL.</param>
        /// <param name="courseId">The course id.</param>
        void StoreLink(string collectionId, string link, string url, string courseId);

        /// <summary>
        /// Stores the widget configuration.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="courseId">The course id.</param>
        void StoreWidgetConfiguration(WidgetConfiguration model, string courseId);

        /// <summary>
        /// Saves a document collection along with any initial file posted.
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="docTitle">The doc title.</param>
        /// <param name="docFile">The doc file.</param>
        /// <param name="courseId">The course id.</param>
        void StoreDocumentCollection(DocumentCollection doc, string docTitle, System.Web.HttpPostedFileBase docFile, 
            string courseId);

        string StoreDocumentCollection(DocumentCollection doc, string docTitle, System.Web.HttpPostedFileBase docFile, 
            string courseId, bool hasDisplay);
        
        DocumentCollection ToDocumentCollection(Upload uploadModel);

        /// <summary>
        /// Stores the module.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="courseId">The course id.</param>
        void StoreModule(PxUnit module, string courseId);

        /// <summary>
        /// Saves a new document to an existing document collection.
        /// </summary>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="docTitle">The doc title.</param>
        /// <param name="docFile">The doc file.</param>
        /// <param name="courseId">The course id.</param>
        /// <returns></returns>
        string StoreDocument(string collectionId, string docTitle, System.Web.HttpPostedFileBase docFile, 
            string courseId);

        /// <summary>
        /// Saves a new document to an existing document collection
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="docTitle"></param>
        /// <param name="docFile"></param>
        /// <param name="courseId"></param>
        /// <param name="id"></param>
        void StoreDocument(string collectionId, string docTitle, System.Web.HttpPostedFileBase docFile, 
            string courseId, string id);

        void CopyDocumentResource(string collectionId, Document document, string courseId);

        /// <summary>
        /// Stores the assignment.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="courseId">The course id.</param>
        void StoreAssignment(Assignment a, string courseId);

        /// <summary>
        /// Saves a new document to an existing assignment document collection
        /// </summary>
        /// <param name="assignmentId"></param>
        /// <param name="collectionId"></param>
        /// <param name="docTitle"></param>
        /// <param name="docFile"></param>
        /// <param name="courseId"></param>
        void StoreItemDocument(string assignmentId, string collectionId, string docTitle, System.Web.HttpPostedFileBase docFile, 
            string courseId);

        /// <summary>
        /// Stores the assignment link.
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="collectionId">The collection id.</param>
        /// <param name="link">The link.</param>
        /// <param name="url">The URL.</param>
        /// <param name="courseId">The course id.</param>
        void StoreAssignmentLink(string assignmentId, string collectionId, string link, string url, string courseId);

        /// <summary>
        /// Stores the folder.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <param name="courseId">The course id.</param>
        void StoreFolder(Folder f, string courseId);

        /// <summary>
        /// Stores the custom widget.
        /// </summary>
        /// <param name="cw">The Custom Widget.</param>
        /// <param name="courseId">The course id.</param>
        void StoreCustomWidget(CustomWidget cw, string courseId);

        /// <summary>
        /// Stores the navigation item.
        /// </summary>
        /// <param name="h">The Html document.</param>
        /// <param name="courseId">The course id.</param>
        void StoreHtmlDocument(HtmlDocument h, string courseId);

        Dictionary<string, Biz.DataContracts.MetadataValue> ExtendedPropertiesToMetaData(Hashtable extendedProperty);

        /// <summary>
        /// Stores the navigation item.
        /// </summary>
        /// <param name="navItem">The nav item.</param>
        /// <param name="courseId">The course id.</param>
        void StoreNavigationItem(NavigationItem navItem, string courseId);

        /// <summary>
        /// Stores the assignment center filter section.
        /// </summary>
        /// <param name="filterSection">The filter section.</param>
        /// <param name="courseId">The course id.</param>
        void StoreAssignmentCenterFilterSection(AssignmentCenterFilterSection filterSection, string courseId);

        /// <summary>
        /// Stores the quiz.
        /// </summary>
        /// <param name="q">The q.</param>
        void StoreQuiz(Quiz q);

        /// <summary>
        /// Stores the content.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="biz"></param>
        void StoreContent(ContentItem item, Biz.DataContracts.ContentItem biz);

        /// <summary>
        /// Stores the discussion.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="courseId">The course id.</param>
        void StoreDiscussion(Discussion d, string courseId);

        /// <summary>
        /// Removes the RSS Feed.
        /// </summary>
        /// <param name="rssFeedId"></param>
        /// <param name="courseId">The course id.</param>
        void RemoveRssFeed(string rssFeedId, string courseId);

        /// <summary>
        /// Removes the RSS Link.
        /// </summary>
        /// <param name="rssArticleId"></param>
        /// <param name="courseId">The course id.</param>
        void RemoveRssLink(string rssArticleId, string courseId);

        /// <summary>
        /// Stores the RSS feed.
        /// </summary>
        /// <param name="article">The RssFeed.</param>
        /// <param name="courseId">The course id.</param>
        void StoreRssFeed(RssFeed article, string courseId);

        /// <summary>
        /// Stores the RSS Link.
        /// </summary>
        /// <param name="article">The RssFeed.</param>
        /// <param name="courseId">The course id.</param>
        /// <param name="parentId"></param>
        void StoreRssLink(RssLink article, string courseId, string parentId = "", string toc = "syllabusfilter");

        /// <summary>
        /// Sets the default parent.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="biz">The biz.</param>
        void SetDefaultParent(ContentItem model, Biz.DataContracts.ContentItem biz);

        /// <summary>
        /// Loads the unit specified.
        /// </summary>
        /// <param name="unitId">The unit ID.</param>
        /// <param name="toc">TOC to load the unit in</param>
        /// <returns></returns>
        PxUnit LoadUnit(string unitId, string toc);

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        ContentView LoadContentView(string id, ContentViewMode mode, string toc = "syllabusfilter");

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="ci">The ci.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <returns></returns>
        ContentView LoadContentView(ContentItem ci, ContentViewMode mode, bool loadToc);

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="ci">The ci.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <param name="loadSubmissions"></param>
        /// <returns></returns>
        ContentView LoadContentView(ContentItem ci, ContentViewMode mode, bool loadToc, bool loadSubmissions);

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <param name="getChildrenGrades"></param>
        /// <returns></returns>
        ContentView LoadContentView(string id, ContentViewMode mode, bool loadToc, string toc = "syllabusfilter", bool getChildrenGrades = false);

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="extEntityId"></param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        ContentView LoadContentView(string id, string extEntityId, ContentViewMode mode, string toc = "syllabusfilter");

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <param name="loadSubmissions"></param>
        /// <param name="getChildrenGrades"></param>
        /// <param name="categoryid"></param>
        /// <param name="extEntityId"></param>
        /// <returns></returns>
        ContentView LoadContentView(string id, ContentViewMode mode, bool loadToc, bool loadSubmissions, 
            string toc = "syllabusfilter", bool getChildrenGrades = false, string categoryid = null, string extEntityId = null);

        /// <summary>
        /// Loads the content view.
        /// </summary>
        /// <param name="item">The item.</param>S
        /// <param name="mode">The mode.</param>
        /// <param name="loadToc">if set to <c>true</c> [load toc].</param>
        /// <returns></returns>
        ContentView LoadContentView(Biz.DataContracts.ContentItem item, ContentViewMode mode, bool loadToc, 
            bool loadSubmissions, string toc = "syllabusfilter", string categoryid = null, string extEntityId = null);

        bool HasMyMaterials(string category, IEnumerable<TocItem> items);

        /// <summary>
        /// Loads the Toc.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        IEnumerable<TocItem> LoadToc(string entityId, string itemId);

        /// <summary>
        /// Loads the Toc.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        IEnumerable<TocItem> LoadToc(string entityId, string itemId, string category);

        /// <summary>
        /// Loads the Toc with all the childs.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">The item id.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        IEnumerable<TocItem> LoadTocWithAllChild(string entityId, string itemId, string category);

        /// <summary>
        /// Gets the current tree item and its descendents
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        IEnumerable<Biz.DataContracts.ContentItem> GetTreeItems(string categoryId, string itemId);

        /// <summary>
        /// gets the list of student updates 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        IEnumerable<Biz.DataContracts.ItemUpdate> GetItemUpdates(string categoryId);

        /// <summary>
        /// Loads the Toc tree.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="rootId">The root id.</param>
        /// <param name="child">The child.</param>
        /// <returns></returns>
        Biz.DataContracts.NavigationItem LoadTocTree(string entityId, string rootId, Biz.DataContracts.NavigationItem child);

        /// <summary>
        /// Shows the toc controls.
        /// </summary>
        /// <returns></returns>
        bool ShowTocControls();

        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        void SetVisibility(XElement visibility);

        /// <summary>
        /// Sets the visibility.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="roles">The roles.</param>
        void SetVisibility(XElement visibility, bool isVisible, string roles);

        /// <summary>
        /// Sets the product visibility.
        /// </summary>
        /// <param name="visibility">The visibility.</param>
        /// <param name="product">The product.</param>
        /// <param name="role">The role.</param>
        void SetProductVisibility(XElement visibility, string product, string role);

        /// <summary>
        /// Returns a list of item ids
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        List<ContentItem> ListChildren(string id);

        /// <summary>
        /// Lists the children.
        /// </summary>
        /// <param name="ci">The ci.</param>
        /// <param name="includeLead">if set to <c>true</c> [include lead].</param>
        /// <returns></returns>
        List<ContentItem> ListChildren(ContentItem ci, Boolean includeLead);

        /// <summary>
        /// Gets the TOC id.
        /// </summary>
        /// <returns></returns>
        string GetTOCId();

        /// <summary>
        /// Adds a item for instructor review if that item has been modified by the student
        /// </summary>
        /// <param name="itemId"></param>
        void AddItemForReview(string itemId);

        /// <summary>
        /// Removes the item from updated items list
        /// </summary>
        /// <param name="itemId"></param>
        void RemoveItemFromReview(string itemId);

        /// <summary>
        /// marks the student updated item as reviewed by instructor
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="enrollmentId"></param>
        void FlagItemAsReviewed(string itemId, string enrollmentId);

        /// <summary>
        /// Gets the parent heirachy.
        /// </summary>L
        /// <param name="contentId">The content id.</param>
        /// <param name="category">The category.</param>g
        /// <param name="entityId">The entity id.</param>
        /// <returns></returns>
        List<Biz.DataContracts.ContentItem> GetParentHeirachy(string contentId, TreeCategoryType category, string toc, string entityId = "");

        /// <summary>
        /// Gets all parent.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="category">The category.</param>
        /// <param name="parents">The parents.</param>
        void GetAllParent(string itemId, TreeCategoryType category, List<Biz.DataContracts.ContentItem> parents, string toc, string entityId = "");

        /// <summary>
        /// Creates the new content of the related.
        /// </summary>
        /// <param name="relatedContentId">The related content id.</param>
        /// <param name="itemIdToAdd">The item id to add.</param>
        /// <returns></returns>
        RelatedContent CreateNewRelatedContent(string relatedContentId, string itemIdToAdd);

        /// <summary>
        /// Gets the related content ID.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <returns></returns>
        string GetRelatedContentID(string itemId);

        /// <summary>
        /// Removes cached page definitions for courses that branch from the course ID
        /// </summary>
        /// <param name="courseId">The course id</param>
        /// <param name="pageName">The page name</param>
        void InvalidateCachedPageDefinitionsForDerivedCourses(string courseId, string pageName);
    }
}