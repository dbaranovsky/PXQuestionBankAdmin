using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.DataContracts;
using Bfw.Common;
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
    /// Helper class to build item queries.
    /// </summary>
    public static class ItemQueryHelper
    {

        /// <summary>
        /// Builds query for the agilix ListChildren command.
        /// </summary>
        /// <param name="entityId">The entity id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="categoryId">The category id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static Adc.ItemSearch BuildListChildrenQuery(string entityId, string parentId, int depth, string categoryId,string userId, bool isCourseSync = false)
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
            }else if (categoryId == Constants.USE_AGILIX_PARENT)
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
                //search.Query = string.Format(@"/bfw_tocs/my_materials@parentid='{0}'", System.Configuration.ConfigurationManager.AppSettings["MyMaterials"] + "_" + userId) + "AND /type='Assessment'  or /type='Homework'";
                search.Query = string.Format(@"/bfw_tocs/syllabusfilter@parentid<>'{0}'","" ) + "AND /type='Assessment'  or /type='Homework'";
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
        /// Lists the children.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="categoryId">The category id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static IEnumerable<Adc.Item> ListChildren(ISession session, string entityId, string parentId, string categoryId, string userId, IContentActions iContentActions, bool loadChild)
        {
            List<Adc.Item> result = null;

            var batch = new Batch();
            var tempCategory = categoryId;
            if (!categoryId.IsNullOrEmpty())
            {
                if (categoryId.IndexOf("enrollment_") > -1)
                {
                    tempCategory = "";
                }
            }
            var childrenCmd = new GetItems()
            {
                SearchParameters = BuildListChildrenQuery(entityId, parentId, 1, tempCategory, userId)
            };
            var parentCmd = new GetItems()
            {
                SearchParameters = new Adc.ItemSearch()
                {
                    EntityId = entityId,
                    ItemId = parentId
                }
            };

            batch.Add(parentCmd);
            batch.Add(childrenCmd);

            session.ExecuteAsAdmin(batch);

            if (!parentCmd.Items.IsNullOrEmpty())
            {
                var parent = parentCmd.Items.First();
                parent.Children = childrenCmd.Items;
                result = new List<Adc.Item>() { parent };
                result.AddRange(childrenCmd.Items);
            }

            if (!string.IsNullOrEmpty(userId))
            {
                var contentActions = iContentActions as ContentActions;
                if (contentActions != null)
                {
                    if (result == null)
                    {
                        result = new List<Adc.Item>();
                    }

                    if (!categoryId.IsNullOrEmpty())
                    {
                        if (categoryId.IndexOf("enrollment_") > -1)
                        {
                            tempCategory = categoryId;
                        }
                    }
                    childrenCmd.SearchParameters = BuildListChildrenQuery(entityId, parentId, 1, tempCategory, userId);
                    if (!categoryId.IsNullOrEmpty())
                    {
                        if (categoryId.IndexOf("enrollment_") > -1)
                        {
                            contentActions.ApplyStudentItems(result, categoryId.Substring(11), loadChild, childrenCmd.SearchParameters);
                        }
                        else
                        {
                            contentActions.ApplyStudentItems(result, string.Empty, loadChild, childrenCmd.SearchParameters);
                        }
                    }
                    else
                    {
                        contentActions.ApplyStudentItems(result, string.Empty, loadChild, childrenCmd.SearchParameters);
                    }
                    
                }
            }
            return result;
        }

        /// <summary>
        /// Lists the children.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="parentId">The parent id.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="categoryId">The category id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static IEnumerable<Adc.Item> ListChildren(ISession session, string entityId, string parentId, 
            string categoryId, string userId, IContentActions iContentActions)
        {
            List<Adc.Item> result = null;

            var batch = new Batch();
            var tempCategory = categoryId;
            if (!categoryId.IsNullOrEmpty())
            {
                if (categoryId.IndexOf("enrollment_") > -1)
                {
                    tempCategory = "";
                }
            }
            var childrenCmd = new GetItems()
            {
                SearchParameters = BuildListChildrenQuery(entityId, parentId, 1, tempCategory, userId)
            };
            var parentCmd = new GetItems()
            {
                SearchParameters = new Adc.ItemSearch()
                {
                    EntityId = entityId,
                    ItemId = parentId
                }
            };

            batch.Add(parentCmd);
            batch.Add(childrenCmd);

            session.ExecuteAsAdmin(batch);

            if (!parentCmd.Items.IsNullOrEmpty())
            {
                var parent = parentCmd.Items.First();
                parent.Children = childrenCmd.Items;
                result = new List<Adc.Item>() { parent };
                result.AddRange(childrenCmd.Items);
            }
            else
            {//Parent doesn't exist (fake parent), use children items
                result = new List<Adc.Item>();
                result.AddRange(childrenCmd.Items);
            }
            if (!string.IsNullOrEmpty(userId))
            {
                var contentActions = iContentActions as ContentActions;
                if (contentActions != null)
                {
                    if (result == null)
                    {
                        result = new List<Adc.Item>();
                    }

                    if (!categoryId.IsNullOrEmpty())
                    {
                        if (categoryId.IndexOf("enrollment_") > -1)
                        {
                            tempCategory = categoryId;
                        }
                    }
                    childrenCmd.SearchParameters = BuildListChildrenQuery(entityId, parentId, 1, tempCategory, userId);
                    if (!categoryId.IsNullOrEmpty())
                    {
                        if (categoryId.IndexOf("enrollment_") > -1)
                        {
                            contentActions.ApplyStudentItems(result, categoryId.Substring(11),false, childrenCmd.SearchParameters);
                        }
                        else
                        {
                            contentActions.ApplyStudentItems(result, string.Empty,false, childrenCmd.SearchParameters);
                        }
                    }
                    else
                    {
                        contentActions.ApplyStudentItems(result, string.Empty,false, childrenCmd.SearchParameters);
                    }

                }
            }
            return result;
        }


        /// <summary>
        /// List the descendents including student items
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="entityId">The entity id.</param>
        /// <param name="itemId">item id</param>
        /// <param name="userId">user id</param>
        /// <param name="iContentActions">content actions</param>
        /// <returns></returns>
        public static IEnumerable<Adc.Item> ListDescendents(ISession session, string entityId, string itemId, string userId, IContentActions iContentActions)
        {
            List<Adc.Item> result = null;
            
            var parentCmd = new GetItems()
            {
                SearchParameters = new Adc.ItemSearch()
                {
                    EntityId = entityId,
                    ItemId = itemId
                }
            };

            session.ExecuteAsAdmin(parentCmd);

            if (!parentCmd.Items.IsNullOrEmpty())
            {
                var parent = parentCmd.Items.First();
                result = new List<Adc.Item>() { parent };

                var studentItems = GetStudentItems(iContentActions, userId, string.Empty).ToList();
                ListDescendents(session, entityId, userId, iContentActions, parent, result, studentItems);
            }
            else
            {
                var studentItems = GetStudentItems(iContentActions, userId, string.Empty).ToList();
                var item = studentItems.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    result = new List<Adc.Item>() { item };
                    ListDescendents(session, entityId, userId, iContentActions, item, result, studentItems);
                }
            }

            return result;
        }

        /// <summary>
        /// Recursive function used by the ListDescendents parent method
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="entityId">entity id</param>
        /// <param name="userId">user id</param>
        /// <param name="iContentActions">content actions</param>
        /// <param name="item"></param>
        /// <param name="result"></param>
        /// <param name="studentItems"></param>
        private static void ListDescendents(ISession session, string entityId, string userId, IContentActions iContentActions,
            Adc.Item item, List<Adc.Item> result, List<Adc.Item> studentItems)
        {
            var childrenCmd = new GetItems()
            {
                SearchParameters = BuildListChildrenQuery(entityId, item.Id, 1, string.Empty, userId)
            };

            session.ExecuteAsAdmin(childrenCmd);

            if (!childrenCmd.Items.IsNullOrEmpty())
            {
                item.Children = childrenCmd.Items;
                result.AddRange(childrenCmd.Items);

            }

            foreach (var studentItem in studentItems)
            {
                if (!String.IsNullOrEmpty(childrenCmd.SearchParameters.Query) && childrenCmd.SearchParameters.Query.ToLower().Contains("parentid"))
                {
                    var searchParentId = childrenCmd.SearchParameters.Query.Split('@')[1].Split('=')[1].Trim('\'');
                    if (studentItem.ParentId == searchParentId && result.Count(i => i.Id == studentItem.Id) == 0)
                    {
                        item.Children.Add(studentItem);
                        result.Add(studentItem);
                    }
                }
            }

            foreach (var childItem in item.Children)
            {
                ListDescendents(session, entityId, userId, iContentActions, childItem, result, studentItems);
            }
        }
        /// <summary>
        /// Recursive function used by the ListDescendents parent method
        /// </summary>
        /// <param name="session">The session</param>
        /// <param name="entityId">entity id</param>
        /// <param name="userId">user id</param>
        /// <param name="iContentActions">content actions</param>
        /// <param name="item"></param>
        /// <param name="result"></param>
        /// <param name="studentItems"></param>
        private static void ListDescendentingItems(ISession session, string entityId, IContentActions iContentActions,
            Adc.Item item, List<Adc.Item> result, List<Adc.Item> studentItems, string environment )
        {
            var childrenCmd = new GetItems()
            {
                SearchParameters = BuildListChildrenQuery(entityId, item.Id, 5, string.Empty, "", true)
            };

            session.ExecuteAsAdmin(environment,childrenCmd);

            if (!childrenCmd.Items.IsNullOrEmpty())
            {
                item.Children = childrenCmd.Items;
                result.AddRange(childrenCmd.Items);

            }

            foreach (var childItem in item.Children)
            {
                ListDescendentingItems(session, entityId, iContentActions, childItem, result, studentItems, environment);
            }

        }

        /// <summary>
        /// Gets the student items 
        /// </summary>
        /// <param name="iContentActions">content actions</param>
        /// <param name="enrollmentId">enrollment id</param>
        /// <param name="categoryId">category id</param>
        /// <returns></returns>
        public static IEnumerable<Adc.Item> GetStudentItems(IContentActions iContentActions, string enrollmentId, string categoryId)
        {
            var studentResourceDoc = iContentActions.GetResource(enrollmentId, iContentActions.GetStudentResourceId(enrollmentId));
            List<Adc.Item> allStudentItems = new List<Adc.Item>();

            if (!categoryId.IsNullOrEmpty())
            {
                studentResourceDoc = iContentActions.GetResource(categoryId, iContentActions.GetStudentResourceId(categoryId));
            }

            if (studentResourceDoc != null)
            {
                var stream = studentResourceDoc.GetStream();
                XDocument doc;
                try
                {
                    doc = stream != null && stream.Length > 0
                                        ? XDocument.Load(stream)
                                        : iContentActions.GetEmptyStudentDoc();
                }
                catch
                {
                    doc = iContentActions.GetEmptyStudentDoc();
                }
                foreach (var itemElement in doc.Root.Elements("item"))
                {
                    var item = new Adc.Item();
                    item.ParseEntity(itemElement);
                    allStudentItems.Add(item);
                }
            }

            return allStudentItems;
        }

        /// <summary>
        /// Get presentation course data from dashboard
        /// </summary>
        /// <param name="iContentActions"></param>
        /// <param name="entityId"></param>
        /// <param name="studentItems"></param>
        /// <returns></returns>
        public static IEnumerable<Adc.Item> GetPresentationStudentItems(IContentActions iContentActions, IEnrollmentActions iEnrollmentActions, IEportfolioCourseActions iEportfolioCourseActions, string entityId, List<Adc.Item> studentItems, String domainid)
        {
            if (iContentActions.Context.CurrentUser != null)
            {
                string dashboardCourseId = iEportfolioCourseActions.GetPersonalEportfolio(iContentActions.Context.CurrentUser.RAId, domainid);

                string currentUserId = string.Empty;
                if (iContentActions.Context.IsPublicView)
                {
                    currentUserId = iContentActions.Context.Course.CourseOwner;
                }
                else if (iContentActions.Context.CurrentUser != null)
                {
                    currentUserId = iContentActions.Context.CurrentUser.Id;
                }

                string dashboardEnrollmentId = iEnrollmentActions.GetUserEnrollmentId(currentUserId, dashboardCourseId);
                var studentResourceDocPresentation = iContentActions.GetResource(dashboardEnrollmentId, iContentActions.GetStudentResourceId(dashboardEnrollmentId));
                XDocument docPresentation;
                if (studentResourceDocPresentation != null)
                {
                    var streamPresentation = studentResourceDocPresentation.GetStream();
                    docPresentation= streamPresentation.Length > 0 ? XDocument.Load(streamPresentation) : iContentActions.GetEmptyStudentDoc();
                }
                else
                {
                    docPresentation = iContentActions.GetEmptyStudentDoc();
                }

                foreach (var itemElement in docPresentation.Root.Elements("item"))
                {
                    var item = new Adc.Item();
                    item.ParseEntity(itemElement);
                    string presentationCategoryEntityId = string.Format("ep_course_{0}", iContentActions.Context.CourseId);
                    var presentationCourse = item.ToContentItem().Categories.Filter(i => i.Id == presentationCategoryEntityId);
                    if (presentationCourse != null && presentationCourse.Count() > 0)
                    {
                        item.ParentId = presentationCourse.FirstOrDefault().ItemParentId;
                        item.Sequence = presentationCourse.FirstOrDefault().Sequence;
                        var elm = item.Data.Descendants("bfw_toc_contents").FirstOrDefault();
                        if (elm != null)
                        {
                            elm.Attribute("parentid").Value = item.ParentId;
                            elm.Attribute("sequence").Value = item.Sequence;
                        }

                        studentItems.Add(item);
                    }
                }
            }
            return studentItems;
        }

        

        public static List<Adc.Item> DoItemSearch(ISession session, string entityId, Dictionary<string,string> queryParams,  string userId, string op = "OR")
        {
            var queryCmd = new GetItems()
                               {
                                   SearchParameters = BuildItemSearchQuery(entityId, queryParams, userId, op)
                               };
            var batch = new Batch();
            batch.Add(queryCmd);

            session.ExecuteAsAdmin(batch);

            var items = queryCmd.Items;
            if (!queryCmd.Items.IsNullOrEmpty())
            {
                var tempChildren = queryCmd.Items.ToList();
                tempChildren.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
                items = tempChildren.ToList();
            }

            return items;
        }

        public static List<Adc.Item> GetChildItems(ISession session, string entityId, List<string> itemIds, String tocDefinition, bool includingShortCuts = false)
        {
            var batch = new Batch();
            foreach (string id in itemIds)
            {
                var queryCmd = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = entityId,
                        Query = String.Format("/bfw_tocs/{0}@parentid='{1}'", tocDefinition, id)
                    }
                };

                batch.Add(queryCmd);

            }
            if (batch.Commands.Any())
            {
                session.ExecuteAsAdmin(batch);
            }
            List<Item> result = new List<Item>();
            foreach (GetItems cmd in batch.Commands)
            {
                var items = cmd.Items;

                if (!cmd.Items.IsNullOrEmpty())
                {
                    var tempChildren = cmd.Items.ToList();
                    if (!includingShortCuts) tempChildren.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
                    items = tempChildren.ToList();
                }

                result.AddRange(items);
            }
            return result;
        }

        public static List<Adc.Item> GetItems(ISession session, string entityId, List<string> itemIds , string userId, bool includingShortCuts=false)
        {
            var batch = new Batch();
            foreach (string id in itemIds)
            {
                var queryCmd = new GetItems()
                                   {
                                       SearchParameters = new ItemSearch()
                                                              {
                                                                  EntityId = entityId,
                                                                  ItemId = id
                                                              }
                                   };
                
                batch.Add(queryCmd);

            }
            if (batch.Commands.Any())
            {
                session.ExecuteAsAdmin(batch);
            }
            List<Item> result = new List<Item>();
            foreach (GetItems cmd in batch.Commands)
            {
                var items = cmd.Items;

                if (!cmd.Items.IsNullOrEmpty())
                {
                    var tempChildren = cmd.Items.ToList();
                    if(!includingShortCuts) tempChildren.RemoveAll(i => i.Id.ToLowerInvariant().StartsWith("shortcut__"));
                    items = tempChildren.ToList();
                }

                result.AddRange(items);
            }
            return result;
        }
            
        /// <summary>
        /// Build query for custom Agilix get items command
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="queryParams"></param>
        /// <param name="userId"></param>
        /// <param name="op">logical operator to use in query</param>
        /// <returns></returns>
        private static Adc.ItemSearch BuildItemSearchQuery(string entityId, Dictionary<string, string> queryParams, string userId, string op)
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
