using System;
using System.Collections.Generic;

using Bdc = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that manipulate Rubric items.
    /// </summary>
    public interface IRubricActions
    {
        /// <summary>
        /// Retrieves a collections of items that are associated with the specified rubric path.
        /// </summary>
        /// <param name="entityId">ID of the entity/course.</param>
        /// <param name="rubricUri">Uri path to the rubric.</param>
        /// <returns>Collection of items that have the specified rubric attached.</returns>
        IEnumerable<Bdc.ContentItem> ListAssociatedItems(String entityId, String rubricUri);

        /// <summary>
        /// Retrieves all rubric items for the specified entity ID.
        /// </summary>
        /// <param name="entityId">ID of the entity/course.</param>
        /// <returns>Collection of items that have the specified rubric attached.</returns>
        IEnumerable<Bdc.ContentItem> ListRubrics(String entityId);

        /// <summary>
        /// Retrieves all content items that have a rubric attached.
        /// </summary>
        /// <param name="entityId">ID of the entity/course.</param>
        /// <returns>Collection of items that have a rubric attached.</returns>
        IEnumerable<Bdc.ContentItem> ListContentWithRubrics(String entityId);

        /// <summary>
        /// Retrieves all content items that have the specified rubrics attached.
        /// </summary>
        /// <param name="entityId">ID of the entity/course.</param>
        /// <returns>Collection of items with rubric aligned.</returns>
        IEnumerable<Bdc.ContentItem> ListAlignedContent(string entityId, List<string> selectedRubricList);

        /// <summary>
        /// Lists the content of the aligned.
        /// </summary>
        /// <param name="entityIds">The entity ids.</param>
        /// <param name="selectedRubricList">The selected rubric list.</param>
        /// <returns></returns>
        IEnumerable<Bdc.ContentItem> ListAlignedContent(List<string> entityIds, List<string> selectedRubricList);

        /// <summary>
        /// Retrieves rubric item for the specified rubric path.
        /// </summary>
        /// <param name="rubricPath">Path of the rubric resource file.</param>
        /// <returns>Rubric content item.</returns>
        Bdc.ContentItem GetRubric(String rubricPath);

        /// <summary>
        /// Retrieves list of rubrics and updates to add course activation.
        /// </summary>
        /// <param name="selectedRubricList">Ids of rubrics to add to course.</param>
        /// <returns>Collection of rubrics modified.</returns>
        IEnumerable<Bdc.ContentItem> AddCourseRubrics(List<string> selectedRubricList);

        /// <summary>
        /// Retrieves list of rubrics and updates to remove course activation.
        /// </summary>
        /// <param name="selectedRubricList">Ids of rubrics to remove from course.</param>
        /// <returns>Collection of rubrics modified.</returns>
        IEnumerable<Bdc.ContentItem> RemoveCourseRubrics(List<string> selectedRubricList);

    }
}