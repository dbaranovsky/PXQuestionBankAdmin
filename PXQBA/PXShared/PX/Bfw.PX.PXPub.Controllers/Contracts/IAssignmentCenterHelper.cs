using System;
using System.Collections.Generic;

namespace Bfw.PX.PXPub.Controllers.Contracts
{
    public interface IAssignmentCenterHelper
    {
        /// <summary>
        /// Adds the grade book category to course.
        /// </summary>
        /// <param name="newCategory">The new category.</param>
        /// <returns></returns>
        string AddGradeBookCategoryToCourse(string newCategory);

        /// <summary>
        /// Adds the grade book category to unit.
        /// </summary>
        /// <param name="unitId">The unit unique identifier.</param>
        /// <param name="categoryId">The category unique identifier.</param>
        /// <returns></returns>
        bool AddGradeBookCategoryToUnit(string unitId, string categoryId);

        /// <summary>
        /// Assigns the assignment center item date.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="toc">The toc to assign dates to</param>
        void AssignAssignmentCenterItemDate(Bfw.PX.Biz.DataContracts.ContentItem item, DateTime endDate, string groupId,
            string toc);

        /// <summary>
        /// Assigns the assignment center item date.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="pxunit">The module.</param>
        /// <param name="item">The item.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="toc">The toc to assign dates to</param>
        void AssignAssignmentCenterItemDate(Bfw.PX.PXPub.Models.AssignmentCenterFilterSection filter,
            Bfw.PX.PXPub.Models.PxUnit module, Bfw.PX.Biz.DataContracts.ContentItem item, DateTime endDate,
            string groupId, string toc);

        /// <summary>
        /// Assigns the assignment center item date.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="toc">The toc to assign dates to</param>
        void AssignAssignmentCenterItemDate(string itemId, DateTime endDate, string groupId, string toc);

        /// <summary>
        /// Assigns the category date orver load.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <param name="toc">TOC to assign the date in</param>
        /// <returns></returns>
        Bfw.PX.PXPub.Models.AssignmentCenterFilterSection AssignCategoryDate(
            Bfw.PX.PXPub.Models.AssignmentCenterFilterSection category, DateTime startDate, DateTime endDate,
            bool updateChildren, string toc);

        /// <summary>
        /// Assigns the category date overload.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <param name="toc">TOC to assign the date in</param>
        /// <returns></returns>
        Bfw.PX.PXPub.Models.AssignmentCenterFilterSection AssignCategoryDate(string categoryId, DateTime startDate,
            DateTime endDate, bool updateChildren, string toc);

        /// <summary>
        /// Assigns an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        void AssignItem(Bfw.PX.Biz.DataContracts.ContentItem item, DateTime startDate, DateTime endDate, string groupId);

		/// <summary>
		/// Assigns an item.
		/// </summary>
		/// <param name="item">item to be assigned</param>
		void AssignItem(Bfw.PX.PXPub.Models.Assign item, string toc = "syllabusfilter");

        /// <summary>
        /// Assigns an item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="toc">TOC to assign the item in</param>
        void AssignItem(string itemId, DateTime startDate, DateTime endDate, string groupId);

        /// <summary>
        /// Assign a lesson date.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="pxunit">The module.</param>
        /// <param name="lessonStartDate">The lesson start date.</param>
        /// <param name="lessonEndDate">The lesson end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <param name="toc">TOC to assign the date in</param>
        void AssignLessonDate(Bfw.PX.PXPub.Models.AssignmentCenterFilterSection filter, Bfw.PX.PXPub.Models.PxUnit module,
            DateTime lessonStartDate, DateTime lessonEndDate, bool updateChildren, string groupId, string toc);

        /// <summary>
        /// Assigns the lesson date.
        /// </summary>
        /// <param name="pxunit">The module.</param>
        /// <param name="lessonStartDate">The lesson start date.</param>
        /// <param name="lessonEndDate">The lesson end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <param name="toc">TOC to assign the item in</param>
        void AssignLessonDate(Bfw.PX.PXPub.Models.PxUnit module, DateTime lessonStartDate, DateTime lessonEndDate,
            bool updateChildren, string groupId, string toc);

        /// <summary>
        /// Assigns the lesson date.
        /// </summary>
        /// <param name="lessonId">The lesson id.</param>
        /// <param name="lessonStartDate">The lesson start date.</param>
        /// <param name="lessonEndDate">The lesson end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <param name="toc">TOC to assign the item in</param>
        void AssignLessonDate(string lessonId, DateTime lessonStartDate, DateTime lessonEndDate, bool updateChildren,
            string groupId, string toc);

        /// <summary>
        /// Finds a Filter by Id.
        /// </summary>
        /// <param name="filterId">The filter id.</param>
        /// <param name="generateDefaultIfEmpty">if set to <c>true</c> [generate default if empty].</param>
        /// <param name="isLoadData">if set to <c>true</c> [is load data].</param>
        /// <returns></returns>
        Bfw.PX.PXPub.Models.AssignmentCenterFilterSection FindFilter(string filterId, bool generateDefaultIfEmpty, bool isLoadData);

        /// <summary>
        /// Generates the root.
        /// </summary>
        /// <returns></returns>
        Bfw.PX.PXPub.Models.AssignmentCenterFilterSection GenerateRoot();

        /// <summary>
        /// Gets the syllabus.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="ignoreIfDeleted">if set to <c>true</c> [ignore if deleted].</param>
        /// <returns></returns>
        Bfw.PX.PXPub.Models.AssignmentCenterFilterSection GetSyllabus(System.Collections.Generic.List<Bfw.PX.PXPub.Models.ContentItem> items, bool ignoreIfDeleted);


        Bfw.PX.PXPub.Models.AssignmentCenterFilterSection GetSyllabusForAssignmentPad(Bfw.PX.PXPub.Models.Course course);

        /// <summary>
        /// Handles the assign item date.
        /// </summary>
        /// <param name="ai">The ai.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="updateChildren">if set to <c>true</c> [update children].</param>
        /// <param name="toc">TOC to assign the item in</param>
        void HandleAssignItemDate(Bfw.PX.PXPub.Models.AssignedItem ai, DateTime startDate, DateTime endDate, bool updateChildren, string groupId, string toc);

        /// <summary>
        /// Determines whether [is valid date] [the specified date time].
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>
        ///   <c>true</c> if [is valid date] [the specified date time]; otherwise, <c>false</c>.
        /// </returns>
        bool IsValidDate(DateTime dateTime);

        /// <summary>
        /// Items the operation.
        /// </summary>
        /// <param name="itemId">The item unique identifier.</param>
        /// <param name="targetId">The target unique identifier.</param>
        /// <param name="assignedItem">The assigned item.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="toc">TOC to run the item operation on</param>
        /// <param name="entityId">The entity unique identifier.</param>
        /// <returns></returns>
        System.Collections.Generic.List<Bfw.PX.PXPub.Models.AssignmentCenterItem> ItemOperation(string itemId,
            string targetId, Bfw.PX.PXPub.Models.AssignedItem assignedItem,
            Bfw.PX.PXPub.Models.AssignmentCenterOperation operation, bool keepInGradebook, string toc, string entityId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containerId"></param>
        /// <param name="subContainerId"></param>
        /// <param name="mode"></param>
        /// <param name="toc">TOC to load the container data from</param>
        /// <returns></returns>
        System.Collections.Generic.List<Bfw.PX.PXPub.Models.ContentItem> LoadContainerData(string containerId, string subContainerId,
            string mode, string toc);

        /// <summary>
        /// Handles the case when an item is moved.
        /// </summary>
        /// <param name="contentId">The content unique identifier.</param>
        /// <param name="newParentId">The new parent unique identifier.</param>
        /// <param name="previousParentId">The previous parent unique identifier.</param>
        /// <param name="toc">TOC to move the item in</param>
        /// <param name="container">The container.</param>
        /// <param name="subcontainerId">The subcontainer unique identifier.</param>
        /// <returns>
        /// Updated state for the tree.
        /// </returns>
        System.Collections.Generic.List<Bfw.PX.PXPub.Models.AssignmentCenterItem> MoveItem(string contentId, string newParentId,
            string previousParentId, string toc, string container, string subcontainerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentId"></param>
        /// <param name="parentId"></param>
        /// <param name="toc">TOC to add the new item to</param>
        /// <returns></returns>
        System.Collections.Generic.List<Bfw.PX.PXPub.Models.AssignmentCenterItem> NewItem(string contentId,
            string parentId, string toc);

        /// <summary>
        /// Assigns (unassigns/moves) item
        /// </summary>
        /// <param name="itemid">The itemid.</param>
        /// <param name="item">The item.</param>
        /// <param name="isUnAssigned">if set to <c>true</c> [is function assigned].</param>
        /// <param name="toc">TOC to processes the assignment in</param>
        /// <param name="keepInGradebook">Flag indicating whether gradebook entry should be preserved</param>
        /// <param name="entityId">The entity unique identifier.</param>
        /// <returns></returns>
        System.Collections.Generic.List<Bfw.PX.PXPub.Models.AssignmentCenterItem> ProcessAssignment(string itemid, Bfw.PX.PXPub.Models.AssignmentCenterItem item, 
                                                                                                    bool isUnAssigned, string toc, bool keepInGradebook, string entityId = "");

        /// <summary>
        /// Persists the state of the category.
        /// </summary>
        /// <param name="category">Category to save.</param>
        /// <param name="toc">TOC to save the navigation state against</param>
        /// <param name="keepInGradebook">Flag indicating whether an entry in the Gradebook should be preserved</param>
        /// <returns>JSON object representing the new state of the category.</returns>
        Bfw.PX.PXPub.Models.AssignmentCenterNavigationState SaveNavigationState(Bfw.PX.PXPub.Models.AssignmentCenterNavigationState state,
            string toc, bool keepInGradebook = true);

        /// <summary>
        /// Sets the items by filter.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="items">The items.</param>
        void SetItemsByFilter(Bfw.PX.PXPub.Models.AssignmentCenterFilterSection filter, System.Collections.Generic.List<Bfw.PX.PXPub.Models.ContentItem> items);

        /// <summary>
        /// Gets upcoming assignments for instructor view
        /// </summary>
        /// <returns></returns>
        IEnumerable<Bfw.PX.Biz.DataContracts.ContentItem> GetDueAssignmentsForInstructor(string entityId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets upcoming assignments for instructor view
        /// </summary>
        /// <returns></returns>
        IEnumerable<Bfw.PX.Biz.DataContracts.ContentItem> GetDueAssignmentsForInstructor(string entityId, int days);

        /// <summary>
        /// Gets upcoming assignments for student view
        /// <param name="days"></param>
        /// <param name="showCompleted"></param>
        /// <param name="showDuePast"></param>
        /// </summary>
        /// <returns></returns>
        IEnumerable<Bfw.PX.Biz.DataContracts.ContentItem> GetDueAssignmentsForStudent(int days, bool showCompleted, bool showDuePast);
    }
}
