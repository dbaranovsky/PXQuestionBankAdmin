using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Provides contracts for actions that manipulate Navigation entities.
    /// </summary>
    public interface INavigationActions
    {
        /// <summary>
        /// Loads a navigation entity for the given site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="navigationId">The navigation ID.</param>
        /// <returns></returns>
        NavigationItem LoadNavigation(string siteId, string navigationId);

        /// <summary>
        /// Loads a navigation entity for the given site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="navigationId">The navigation ID.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        NavigationItem LoadNavigation(string siteId, string navigationId, string category);

        /// <summary>
        /// Loads a navigation entity for the given site.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="navigationId">The navigation ID.</param>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        NavigationItem LoadNavigation(string siteId, string navigationId, string category, bool loadChild);

        /// <summary>
        /// Get the navigation items from agilix.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="navigationId">The navigation ID.</param>
        /// <param name="levels">The levels.</param>
        /// <param name="categoryId">The category ID.</param>
        /// <returns></returns>
        NavigationItem GetNavigation(string siteId, string navigationId, string categoryId);

        /// <summary>
        /// Gets all child widget items under the specified parent item.
        /// </summary>
        /// <param name="parentId">ID of the parent.</param>
        /// <returns></returns>
        List<ContentItem> GetWidgets(string parentId);

        /// <summary>
        /// To determine if a course has content created by the active user.
        /// </summary>
        /// <param name="siteId">The site ID.</param>
        /// <param name="navigationId">The navigation ID.</param>
        /// <param name="levels">The levels.</param>
        /// <param name="categoryId">The category ID.</param>
        /// <returns>
        ///   <c>true</c> if the specified site id has user materials; otherwise, <c>false</c>.
        /// </returns>
        bool HasUserMaterials(string siteId, string navigationId, string categoryId);
    }
}