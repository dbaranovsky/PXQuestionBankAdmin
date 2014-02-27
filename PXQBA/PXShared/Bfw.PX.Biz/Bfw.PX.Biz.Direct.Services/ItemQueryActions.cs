using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.Common.Collections;
using Bfw.Agilix.Commands;
using Adc = Bfw.Agilix.DataContracts;
using Bfw.Agilix.Dlap.Session;
using Bdc = Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using System.Xml.Linq;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// The item search service.
    /// </summary>
    public class ItemQueryActions : IItemQueryActions
    {
        public ItemQueryActions()
        {

        }

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
        public Agilix.DataContracts.ItemSearch BuildListChildrenQuery(string entityId, string parentId, int depth, string categoryId, string userId, bool isCourseSync = false)
        {
            var search = new Adc.ItemSearch()
            {
                EntityId = entityId,
                ItemId = parentId,
                Depth = depth
            };

            if (isCourseSync == true)
            {
                search.Query = string.Format("/parent='{0}'", parentId);
            }
            else if (categoryId == Constants.USE_AGILIX_PARENT)
            {
                // Here we don't do any special query; we just increase the
                // depth by one to make sure that the children are loaded.
                depth += 1;
            }
            else if (categoryId == System.Configuration.ConfigurationManager.AppSettings["MyMaterials"])
            {
                search.Query = string.Format(@"/bfw_tocs/my_materials@parentid='{0}'", categoryId + "_" + userId);

            }
            else if (categoryId == System.Configuration.ConfigurationManager.AppSettings["MyQuizes"])
            {
                search.Query = string.Format(@"/bfw_tocs/syllabusfilter@parentid<>'{0}'", "") + "AND /type='Assessment'  or /type='Homework'";
            }
            else if (!string.IsNullOrEmpty(categoryId))
            {
                search.Query = string.Format(@"/bfw_tocs/{0}@parentid='{1}'", categoryId, parentId);
            }
            else
            {
                search.Query = string.Format(@"/bfw_tocs/bfw_toc_contents@parentid='{0}'", parentId);
            }

            return search;
        }

        /// <summary>
        /// Build query for custom Agilix get items command
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="queryParams"></param>
        /// <param name="userId"></param>
        /// <param name="op">logical operator to use in query</param>
        /// <returns></returns>
        public Adc.ItemSearch BuildItemSearchQuery(string entityId, Dictionary<string, string> queryParams, string userId, string op)
        {
            var search = new Adc.ItemSearch()
            {
                EntityId = entityId,
                Query = ""
            };

            List<string> queryExpressions = new List<string>();

            foreach (string key in queryParams.Keys)
            {
                if (key == "freequery")
                {
                    queryExpressions.Add(string.Format("{0}", queryParams[key]));
                }
                else
                {
                    queryExpressions.Add(string.Format(@"/{0}='{1}'", key, queryParams[key]));
                }
            }

            search.Query = string.Join(" " + op + " ", queryExpressions);

            return search;
        }
    }
}
