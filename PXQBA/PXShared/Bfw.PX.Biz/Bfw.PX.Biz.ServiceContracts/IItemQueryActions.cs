using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Adc = Bfw.Agilix.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// The item search service.
    /// </summary>
    public interface IItemQueryActions
    {
        /// <summary>
        /// Builds query for the agilix ListChildren command.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="categoryId">The category id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="isCourseSync">The flag if course is synched.</param>
        /// <returns></returns>
        Adc.ItemSearch BuildListChildrenQuery(string entityId, string parentId, int depth, string categoryId, string userId, bool isCourseSync = false);

        /// <summary>
        /// Build query for custom Agilix get items command
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="queryParams"></param>
        /// <param name="userId"></param>
        /// <param name="op">logical operator to use in query</param>
        /// <returns></returns>
        Adc.ItemSearch BuildItemSearchQuery(string entityId, Dictionary<string, string> queryParams, string userId, string op);
    }
}
