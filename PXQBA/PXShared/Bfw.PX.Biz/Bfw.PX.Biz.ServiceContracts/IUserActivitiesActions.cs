using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// provides contract for various user activities
    /// </summary>
    public interface IUserActivitiesActions
    {
        /// <summary>
        /// List of item updates for an enrollment 
        /// </summary>
        /// <param name="enrollmentId">enrollment id of the student</param>
        /// <returns>list of student updates</returns>
        List<ItemUpdate> GetItemUpdates(string enrollmentId);

        /// <summary>
        /// List of item update count by enrollment
        /// </summary>
        /// <param name="enrollmentId"></param>
        /// <returns>List of item update count</returns>
        List<KeyValuePair<string, int>> GetItemUpdateCount(string courseId);

        /// <summary>
        /// Mark an item as updated
        /// </summary>
        /// <param name="update">updated item details</param>
        void MarkItemAsUpdated(ItemUpdate update);

        /// <summary>
        /// Unmark the item as updated
        /// </summary>
        /// <param name="update">updated item details</param>
        void UnmarkItemAsUpdated(ItemUpdate update);

        /// <summary>
        /// Remove item updates data
        /// </summary>
        /// <param name="removeBefore">till date</param>
        void CleanupItemUpdatesData(DateTime removeBefore);

        /// <summary>
        /// Delete a single item update data
        /// </summary>
        /// <param name="update">item detail</param>
        void DeleteItemUpdateData(ItemUpdate update);
    }
}
