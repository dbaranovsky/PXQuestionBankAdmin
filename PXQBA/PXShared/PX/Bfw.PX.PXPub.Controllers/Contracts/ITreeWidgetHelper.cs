using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Bfw.PX.PXPub.Models;
using Bfw.PX.Biz.DataContracts;

using BfwDC = Bfw.PX.Biz.DataContracts;
using BfwM = Bfw.PX.PXPub.Models;
using Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Contracts
{
    public interface ITreeWidgetHelper
    {
        string DefaultToc { get; }
        string DefaultContainer { get; }
        string DefaultSubContainer { get; }
        
        List<BfwM.ContentItem> LoadItem(IContentActions contentActions, IBusinessContext context, 
            IGradeActions gradeActions, string itemId, string level, string widget = "");

        List<BfwM.ContentItem> ProcessStudentGrades(IContentActions contentActions, IBusinessContext context, 
            IGradeActions gradeActions, BfwM.ContentItem item, ref BfwM.ContentItem parentItem);

        void SetGrade(BfwM.ContentItem item, BfwDC.Grade grade);

        /// <summary>
        /// Returns a list of ids for the top level item ids for all the items in <paramref name="itemsDue"/>
        /// </summary>
        /// <param name="itemsDue">List of content items in tree widget</param>
        /// <param name="toc">Toc to get top level item ids for</param>
        /// <returns>List of item IDS for top level items</returns>
        List<string> GetSubcontainerItemIds(List<BfwDC.ContentItem> itemsDue, string toc);

        /// <summary>
        /// Returns list of content items in the current course that have been assigned, have a point value and are 
        /// completable 
        /// </summary>
        /// <param name="contentActions"></param>
        /// <param name="context"></param>
        /// <param name="tocId">Filter items to a specific toc</param>
        /// <param name="chapterId">Filter items to a specific top level content item</param>
        /// <param name="toc">TOC to get assigned content for</param>
        /// <returns>List of gradeable content for the course</returns>
        List<BfwDC.ContentItem> GetAssignedContent(IContentActions contentActions, string entityId, string tocId,
            string chapterId, string toc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grades"></param>
        /// <param name="items"></param>
        /// <param name="subcontainer"></param>
        void CalculateSubcontainerCompletionData(IEnumerable<BfwDC.Grade> enrollmentGrades, 
            IEnumerable<BfwDC.ContentItem> itemsInSubcontainer, BfwM.ContentItem subcontainer);

        /// <summary>
        /// Gets the containers items.
        /// </summary>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="context">The context.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="containerId">The container unique identifier.</param>
        /// <param name="subcontainerId">The subcontainer unique identifier.</param>
        /// <param name="itemlevel">The itemlevel.</param>
        /// <returns></returns>
        List<TreeWidgetViewItem> GetContainersItems(IContentActions contentActions, IBusinessContext context,
            TreeWidgetSettings settings, string containerId, string subcontainerId, int itemlevel = 0);

        /// <summary>
        /// Returns the template id for a unit if one exists in dlap. This is used in creating assignment units
        /// </summary>
        /// <returns>Unit template id if it exists.  Empty string if not</returns>
        string GetUnitTemplateId(IContentActions contentActions);
    }
}
