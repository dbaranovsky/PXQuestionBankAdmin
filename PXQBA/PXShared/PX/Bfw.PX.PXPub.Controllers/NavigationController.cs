using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Mappers;

using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Collections;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class NavigationController : Controller
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected ContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Actions for nav items
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Holds the Widget list XML setting for Zone 2, the default value is already assigned
        /// </summary>
        public static string ZONE2_CONFIG = "<WidgetList><Widget name=\"Table of Contents\" id=\"widgetconfig_tocwidget\"/><Widget name=\"My Assignments\" id=\"widgetconfig_assignmentwidget\"/><Widget name=\"My Announcements\" id=\"widgetconfig_announcementwidget\"/><Widget name=\"My Progress and Scorecard\" id=\"widgetconfig_progresswidget\"/><Widget name=\"Featured Content\" id=\"widgetconfig_featuredcontentwidget\"/><Widget name=\"You Have Read\" id=\"widgetconfig_contentreadwidget\"/></WidgetList>";

        /// <summary>
        /// Holds the Widget list XML setting for Zone 3, the default value is already assigned
        /// </summary>
        public static string ZONE3_CONFIG = "<WidgetList><Widget name=\"QuickPanel\" id=\"widgetconfig_quickpanel\"/></WidgetList>";

        /// <summary>
        /// Gets or sets the session manager.
        /// </summary>
        /// <value>
        /// The session manager.
        /// </value>
        protected ISessionManager SessionManager { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="navActions">The nav actions.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="helper">The helper.</param>        
        public NavigationController(BizSC.IBusinessContext context, BizSC.INavigationActions navActions, BizSC.IContentActions contentActions, ContentHelper helper, ISessionManager sessionManager)
        {
            Context = context;
            NavigationActions = navActions;
            ContentHelper = helper;
            ContentActions = contentActions;
            SessionManager = sessionManager;
        }
        
        /// <summary>
        /// Returns a flow of how the TOC items interact with each other in the Hierarchical way and gives some meta data for each item
        /// Assumption: No item (itemid) should appear in the TOC more than once
        /// </summary>
        /// <param name="top_item_id">The inclusive starting point to look for children</param>
        /// <param name="tocDefinition"></param>
        /// <param name="depth"></param>
        /// <param name="includeType"></param>
        /// <param name="includeSubtype"></param>
        /// <param name="includeHidden"></param>
        /// <param name="includeHiddenFromStudents"></param>
        /// <param name="includeHiddenFromToc"></param>
        /// <returns></returns>
        public JsonResult GetTocStructure(String top_item_id, String tocDefinition, int depth = -1, bool includeBfw_Subtype = true, bool includeTitle = true, bool includeHref = true, bool includeType = true, bool includeSubtype = true, bool includeHidden = true, bool includeHiddenFromStudents = true, bool includeHiddenFromToc = true)
        {
            // fail because top_item_id or tocDefinition was passed in null
            #region Error Cases

            String errorMessage = null;
            if (top_item_id.IsNullOrEmpty())
                errorMessage = "top_item_id can not be null or empty";
            if (tocDefinition.IsNullOrEmpty())
                errorMessage = "tocDefinition can not be null or empty";

            if (!errorMessage.IsNullOrEmpty())
            {
                var response1 = new
                {
                    STATUS = "Failed",
                    ErrorMessage = errorMessage,
                    Items = new String[0]
                };
                return Json(response1, JsonRequestBehavior.AllowGet);
            }

            #endregion Error Cases

            Dictionary<String, int> foundItems = new Dictionary<string,int>();
            var items = ContentActions.GetItems(Context.EntityId, new List<string> { top_item_id }).ToList();
            var orig_depth = depth;

            // get all items
            #region Get All Items associated to TOC

            while (depth-- > 0 || orig_depth == -1)
            {
                // get a list of ids that need to be found
                var itemsBeingLookedFor = items.Where(x => !foundItems.ContainsKey(x.Id)).Map(x => x.Id).ToList();

                // no need to search for more items
                if (itemsBeingLookedFor.Count == 0)
                    break;

                // set ids as looked for
                foreach (var id in itemsBeingLookedFor) { foundItems[id] = foundItems.ContainsKey(id) ? foundItems[id]+1 : 0; }

                // get children and add them to the items list
                var children = ContentActions.GetChildItems(Context.EntityId, itemsBeingLookedFor, tocDefinition);

                items.AddRange(children);
            }

            #endregion Get All Items associated to TOC

            // the item data
            #region Construct Base Item Data

            var item_base_structure = new Dictionary<String, IDictionary<String, Object>>();
            foreach (var item in items)
            {
                var variables = new Dictionary<String, Object>();
                item_base_structure[item.Id] = variables;
                
                if (includeType)
                    variables["Type"] = item.Type;
                if (includeSubtype)
                    variables["Subtype"] = item.Subtype;
                if(includeHidden)
                    variables["Hidden"] = item.Hidden;
                if (includeHiddenFromStudents)
                    variables["HiddenFromStudents"] = item.HiddenFromStudents;
                if (includeHiddenFromToc)
                    variables["HiddenFromToc"] = item.HiddenFromToc;
                if (includeHref)
                    variables["Href"] = item.Href;
                if (includeTitle)
                    variables["Title"] = item.Title;

                var tocNode = item.ItemDataXml.Descendants(tocDefinition).FirstOrDefault();
                
                // set the TOC sequence
                if (tocNode != null && tocNode.Attribute("sequence") != null)
                    variables["TOC_Sequence"] = tocNode.Attribute("sequence").Value;
                else if (tocNode != null && tocNode.Attribute("Sequence") != null)
                    variables["TOC_Sequence"] = tocNode.Attribute("Sequence").Value;

                // set the parentid
                if (tocNode != null && tocNode.Attribute("parentid") != null)
                    variables["TOC_ParentId"] = tocNode.Attribute("parentid").Value;

                // set the bfw_subtype
                var bfw_subtypeNode = item.ItemDataXml.Element("bfw_subtype");
                variables["BFW_Subtype"] = bfw_subtypeNode == null ? "" : bfw_subtypeNode.Value;

                variables["Sequence"] = item.Sequence;
            }

            #endregion Construct Base Item Data

            // adding parent ids and ordered descentent ids for each item
            #region Add Hierarchical Data for each item

            var hierarchy_base_structure = new Dictionary<String, Object>();
            var findChildren = new List<String>();
            findChildren.Add(top_item_id);

            while(findChildren.Count > 0)
            {
                var newKidsOnTheBlock = new List<String>();
                foreach (var id in findChildren)
                {
                    var kids = items.Where(x => ((String)item_base_structure[x.Id]["TOC_ParentId"]) == id).ToList();

                    // remove found kids from the master list of kids
                    foreach(var kid in kids)
                        items.Remove(kid);

                    // set the parent for every child
                    foreach (var kid in kids)
                        item_base_structure[kid.Id]["TOC_ParentId"] = id;

                    // set and order the children
                    item_base_structure[id]["Descendants"] = kids.Map(x => x.Id).OrderBy(x => item_base_structure[x]["TOC_Sequence"]).ToArray();

                    newKidsOnTheBlock.AddRange(item_base_structure[id]["Descendants"] as String[]);
                }

                findChildren = newKidsOnTheBlock;
            }

            #endregion Add Hierarchal Data for each item
            
            var response2 = new { 
                STATUS = "OK",
                ErrorMessage= "",
                Items = item_base_structure,
            };

            return Json(response2, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Displays the Vertical Menu view.
        /// </summary>
        /// <param name="menuName">Name of the menu.</param>
        /// <returns></returns>
        public ActionResult VerticalMenu(string menuName)
        {
            ActionResult result = null;

            if (!String.IsNullOrEmpty(Context.EntityId))
            {
                var navItem = NavigationActions.LoadNavigation(Context.EntityId, menuName);
                var menu = navItem.Children;

                if (!menu.IsNullOrEmpty())
                {
                    ViewData.Model = new VerticalMenu()
                    {
                        NavItems = menu.Map(ni => ni.ToNavigationItem(ContentActions)).ToList(),
                        IsInstructor = (Context.AccessLevel == BizSC.AccessLevel.Instructor),
                        IsAnonymous = (Context.IsAnonymous),
                        IsStudent = (Context.AccessLevel == BizSC.AccessLevel.Student),
                        IsProductCourse = (Context.Product != null) && (Context.Product.Id == Context.EntityId),
                        ShowAssignmentLink = !String.IsNullOrEmpty(Context.EnrollmentId)
                    };

                    ViewData["EntityId"] = Context.EntityId;
                }
            }

            result = View();
            return result;
        }

        /// <summary>
        /// Gets the navigation items.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <param name="isIncludeParent">if set to <c>true</c> [is include parent].</param>
        /// <param name="menuContainer">The menu container.</param>
        /// <returns></returns>
        private List<NavigationItem> GetNavigationItems(string parentId, bool isIncludeParent, MenuContainer menuContainer)
        {
            var items = new List<NavigationItem>();
            if (!String.IsNullOrEmpty(Context.EntityId))
            {
                var ci = NavigationActions.GetNavigation(Context.EntityId, parentId,"");
                var item = ci.ToNavigationItem(ContentActions);

                foreach (var child in item.Children)
                {
                    if (child is NavigationItem)
                    {

                        var childNav = (NavigationItem)child;
                        foreach (var subItem in childNav.Children)
                        {
                            if (subItem is Link)
                            {
                                var subItemNav = (Link)subItem;

                                if (subItemNav.ExtendedLinkType.ToLowerInvariant() == "manual")
                                {
                                    var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, "PX_TOC");
                                    var navToc = bizNavigationItem.ToNavigationItem(ContentActions);

                                    if (navToc != null && navToc.Children.Count > 0)
                                    {
                                        subItemNav.Url = Url.RouteUrl("FeaturedContentItem", new { id = navToc.Children.First().Id });
                                        subItemNav.Url = subItemNav.Url + "?category=" + subItemNav.Title.ToLowerInvariant().Replace(" ", "__");
                                    }
                                }
                            }

                            if (subItem is NavigationItem)
                            {
                                var subItemNav = (NavigationItem)subItem;

                                if (subItemNav.Id == "PX_LOCATION_ZONE1_MENU0_CONTENT" || subItemNav.Id == "PX_TOC" || subItemNav.Id.Contains("PX_LOCATION_ZONE1_MENU0_CONTENT"))
                                {
                                    var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, "PX_TOC");
                                    var navToc = bizNavigationItem.ToNavigationItem(ContentActions);

                                    if (navToc != null && navToc.Children.Count > 0)
                                    {
                                        subItemNav.TocId = navToc.Children.First().Id;
                                    }
                                }
                            }
                        }
                    }
                    else if (child is Link)
                    {
                        var lnk = (Link)child;
                        menuContainer.Links.Add(lnk);
                    }

                    if (child is NavigationItem)
                    {
                        var navItem = (NavigationItem)child;

                        navItem.Children.OrderBy(i => i.Sequence);
                        navItem.IsTopLevel = true;
                        (navItem).IsActive = false;
                        items.Add(navItem);
                        menuContainer.NavigationItems.Add(navItem);
                    }

                    menuContainer.Children.Add(child);
                }

                if (isIncludeParent)
                {
                    items.Add(item);
                }
            }

            items.OrderBy(i => i.Sequence);
            return items;
        }

        /// <summary>
        /// Update a Link's Title and Url in Agilix and returns the View.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="title">The title.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public ActionResult EditLink(string itemId, string title, string url)
        {
            if (string.IsNullOrEmpty(itemId) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(url))
            {
                throw new Exception("Invalid call to EditLink");
            }
            else
            {
                var currentItem = ContentActions.GetContent(Context.EntityId, itemId, true);
                var link = currentItem.ToLink();

                link.Hidden = false;
                link.Title = title;
                link.Url = url;
                ContentHelper.StoreLink(link, Context.EntityId);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Saves the link.
        /// </summary>
        /// <param name="lnk">The LNK.</param>
        /// <returns></returns>
        public ActionResult SaveLink(Link lnk)
        {
            lnk.Hidden = false;
            ContentHelper.StoreContent(lnk, null);
            return new EmptyResult();
        }

        /// <summary>
        /// Update a Navigation Item Title.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="targetId">The target id.</param>
        /// <returns></returns>
        public ActionResult UpdateTitle(string itemId, string newName, string targetId)
        {
            targetId = targetId.Trim();
            string entityId = (targetId == string.Empty) ? Context.EntityId : targetId;

            if (newName.Length > 25)
            {
                newName = newName.Substring(0, 25);
            }

            var currentItem = ContentActions.GetContent(entityId, itemId, true);

            if (currentItem.Type.ToLowerInvariant() == "navigationitem" || currentItem.Subtype.ToLowerInvariant() == "navigationitem")
            {
                var currentNavItem = NavigationActions.GetNavigation(entityId, itemId, "").ToNavigationItem(ContentActions);
                currentNavItem.Title = newName;

                if (currentNavItem.Id == "PX_LOCATION_ZONE1_MENU0_CONTENT")
                {
                    ContentHelper.SetVisibility(currentNavItem.Visibility, true, "instructor");
                    ContentHelper.SetVisibility(currentNavItem.Visibility, true, "student");
                }

                ContentHelper.StoreNavigationItem(currentNavItem, entityId);
            }
            else
            {
                var ci = currentItem.ToLink();
                ci.Title = newName;
                ContentHelper.StoreLink(ci, entityId);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Removes the widget.
        /// </summary>
        /// <param name="currentItemId">The current item id.</param>
        /// <param name="targetId">The target id.</param>
        /// <returns></returns>
        public ActionResult RemoveWidget(string currentItemId, string targetId)
        {
            targetId = targetId.Trim();
            string entityId = (targetId == string.Empty) ? Context.EntityId : targetId;

            if (!string.IsNullOrEmpty(currentItemId))
            {
                var currentItem = ContentActions.GetContent(entityId, currentItemId, true);
                var ci = currentItem.ToWebConfiguration();

                ci.ParentId = "PX_DELETED";
                ContentHelper.StoreWidgetConfiguration(ci, entityId);
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="itemId">The item id.</param>
        /// <param name="targetId">The target id.</param>
        /// <returns></returns>
        public ActionResult DeleteItem(string itemId, string targetId)
        {
            targetId = targetId.Trim();
            string entityId = (targetId == string.Empty) ? Context.EntityId : targetId;

            var ci = ContentActions.GetContent(Context.EntityId, itemId);

            if (ci != null)
            {
                var item = ci.ToContentItem(ContentActions);
                if (item.Type == "NavigationItem")
                {
                    var currentNavItem = NavigationActions.GetNavigation(entityId, itemId, "");
                    var ni = currentNavItem.ToNavigationItem(ContentActions);
                    ni.ParentId = "PX_DELETED";
                    ContentHelper.StoreNavigationItem(ni, entityId);
                }
                else
                {
                    item.ParentId = "PX_DELETED";
                    ContentHelper.StoreContent(item, ci);
                }
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Saves the link.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public ActionResult SaveLink(NavigationItem item)
        {
            ContentHelper.StoreNavigationItem(item, Context.EntityId);
            return new EmptyResult();
        }

        /// <summary>
        /// Sets the item visibility.
        /// </summary>
        /// <param name="modifieditemId">The modifieditem id.</param>
        /// <param name="isVisible">if set to <c>true</c> [is visible].</param>
        /// <param name="role">The role.</param>
        /// <param name="targetId">The target id.</param>
        /// <returns></returns>
        public ActionResult SetItemVisibility(string modifieditemId, bool isVisible, string role, string targetId)
        {
            targetId = targetId.Trim();
            string entityId = (targetId == string.Empty) ? Context.EntityId : targetId;

            if (!string.IsNullOrEmpty(modifieditemId))
            {
                var currentItem = ContentActions.GetContent(Context.EntityId, modifieditemId, true);

                if (currentItem != null)
                {
                    if (currentItem.Type.ToLowerInvariant() == "navigationitem" || currentItem.Subtype.ToLowerInvariant() == "navigationitem")
                    {
                        var currentNavItem = NavigationActions.GetNavigation(entityId, modifieditemId, "");
                        var ni = currentNavItem.ToNavigationItem(ContentActions);

                        ContentHelper.SetVisibility(ni.Visibility, isVisible, role);
                        ContentHelper.StoreNavigationItem(ni, entityId);
                    }
                    else
                    {
                        var ci = currentItem.ToContentItem(ContentActions);

                        ContentHelper.SetVisibility(ci.Visibility, isVisible, role);
                        ContentHelper.StoreContent(ci, currentItem);
                    }
                }
            }

            return new EmptyResult();
        }

        /// <summary>
        /// Gets the widgets.
        /// </summary>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public WidgetConfigurationCollection GetWidgets(string parentId)
        {
            var items = new WidgetConfigurationCollection();
            foreach (var item in ContentActions.ListChildren(Context.EntityId, parentId))
            {
                if (item.Subtype == "WidgetConfiguration")
                {
                    items.Widgets.Add(item.ToWebConfiguration());
                }
            }

            return items;
        }

        /// <summary>
        /// Sets the widget visibility.
        /// </summary>
        /// <param name="widgetId">The widget id.</param>
        /// <param name="role">The role.</param>
        /// <param name="isShow">if set to <c>true</c> [is show].</param>
        /// <param name="targetId">The target id.</param>
        /// <returns></returns>
        public ActionResult SetWidgetVisibility(string widgetId, string role, bool isShow, string targetId)
        {
            targetId = targetId.Trim();
            string entityId = (targetId == string.Empty) ? Context.EntityId : targetId;
            var existing = ContentActions.GetContent(Context.EntityId, widgetId);
            var instance = existing.ToWebConfiguration();

            ContentHelper.SetVisibility(instance.Visibility, isShow, role);
            ContentHelper.StoreWidgetConfiguration(instance, entityId);
            return new EmptyResult();
        }

        /// <summary>
        /// Sets the widget collapse.
        /// </summary>
        /// <param name="widgetId">The widget id.</param>
        /// <param name="isCollapsed">if set to <c>true</c> [is collapsed].</param>
        /// <param name="targetId">The target id.</param>
        /// <returns></returns>
        public ActionResult SetWidgetCollapse(string widgetId, bool isCollapsed, string targetId)
        {
            targetId = targetId.Trim();
            string entityId = (targetId == string.Empty) ? Context.EntityId : targetId;
            var existing = ContentActions.GetContent(Context.EntityId, widgetId);
            var instance = existing.ToWebConfiguration();
            instance.IsCollapsed = isCollapsed;

            ContentHelper.StoreWidgetConfiguration(instance, entityId);
            return new EmptyResult();
        }
    }

}
