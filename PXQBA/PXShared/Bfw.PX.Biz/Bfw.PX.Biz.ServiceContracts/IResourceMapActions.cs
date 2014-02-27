using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that create and manage resource-item mappings.
    /// </summary>
    public interface IResourceMapActions
    {
        /// <summary>
        /// Stores the currently logged in user's resource mappings specifying target entityId and enrollmentId.
        /// </summary>
        /// <param name="resourceMap"></param>
        /// <param name="entityId"></param>
        /// <param name="enrollmentId"></param>
        void StoreMaps(List<ResourceMap> resourceMap, string entityId, string enrollmentId);

        /// <summary>
        /// Lists the currently logged in user's resource mappings
        /// </summary>
        /// <returns></returns>
        IEnumerable<ResourceMap> ListMaps();

        /// <summary>
        /// Lists the currently logged in user's resource mappings
        /// </summary>
        /// <returns></returns>
        IEnumerable<ResourceMap> ListMaps(string enrollmentId, string entityId = null);


        /// <summary>
        /// Returns true if the item is mapped, false otherwise.
        /// </summary>
        /// <param name="itemId">The item ID.</param>
        /// <returns>
        ///   <c>true</c> if the specified item ID is mapped; otherwise, <c>false</c>.
        /// </returns>
        bool IsMapped(string itemId);

        /// <summary>
        /// This method will delete any mapping for specified item ID.
        /// </summary>
        /// <param name="itemId"></param>
        void DeleteMap(string itemId);

        /// <summary>
        /// This method will add a resource to a mapped item ID.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="mapDescription">The mapping description.</param>
        void AddResourceMap(Resource resource, string itemId, string mapDescription);

        /// <summary>
        /// This method will add a resource to a mapped item ID. Specifies target entityId
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="itemId">The item ID.</param>
        /// <param name="mapDescription">The mapping description.</param>
        void AddResourceMap(Resource resource, string itemId, string mapDescription, string applicableEntityId);

        /// <summary>
        /// This method will add resources to a mapped item ID.
        /// </summary>
        /// <param name="resourceIds">The resource IDs.</param>
        /// <param name="itemId">ID of the item to map to.</param>
        /// <param name="mapDescription">The mapping description.</param>
        void AddResourceMap(string resourceIds, string itemId, string mapDescription);

        /// <summary>
        /// This method will return a list of resources for the current enrolled user.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Resource> GetResourcesForEnrollment();

        /// <summary>
        /// This method will return a list of resources for a specific item.
        /// </summary>
        /// <param name="itemId">ID of the item to get resources for.</param>
        /// <returns></returns>
        IEnumerable<Resource> GetResourcesForItem(string itemId);

        /// <summary>
        /// This method will return a list of resources for a specific item.
        /// </summary>
        /// <param name="itemId">ID of the item to get resources for.</param>
        /// <param name="enrollmentId"></param>
        /// <returns></returns>
        IEnumerable<Resource> GetResourcesForItem(string itemId, string enrollmentId, string entityId = null);



        /// <summary>
        /// Get a string defining associations with resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        String GetAssociationsForResource(Resource resource);

        /// <summary>
        /// Helper method to find the ID of a resource from its path.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        String GetResourceId(Resource resource);

        /// <summary>
        /// Gets the resource link.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        String GetResourceLink(Resource resource);

        /// <summary>
        /// Gets the content of the resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        string GetResourceContent(Resource resource);
    }
}