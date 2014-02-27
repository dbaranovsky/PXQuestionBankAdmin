using System;
using System.Collections.Generic;
using System.Linq;

using Bfw.Common.Collections;
using Bfw.Agilix.DataContracts;

using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Logging;


namespace Bfw.PX.Biz.Direct.Services
{
    /// <summary>
    /// Provides all business logic functions in regards to all forms of navigation.
    /// </summary>
    public class NavigationActions : INavigationActions
    {
        #region Properties

        /// <summary>
        /// The IBusinessContext implementation to use.
        /// </summary>
        protected IBusinessContext Context { get; set; }

        /// <summary>
        /// The IContentActions implementation to use.
        /// </summary>
        protected IContentActions ContentActions { get; set; }

        /// <summary>
        /// The Item Query Actions
        /// </summary>
        protected IItemQueryActions ItemQueryActions { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationActions"/> class.
        /// </summary>
        /// <param name="context">The IBusinessContext implementation.</param>
        /// <param name="itemService">The IItemService implementation.</param>
        /// <param name="contentActions">The IContentActions implementation.</param>
        public NavigationActions(IBusinessContext context, IContentActions contentActions, IItemQueryActions itemQueryActions)
        {
            Context = context;
            ContentActions = contentActions;
            ItemQueryActions = itemQueryActions;
        }

        #endregion

        #region INavigationActions Methods

        /// <summary>
        /// Loads a navigation entity for the given site.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="navigationId"></param>
        /// <returns></returns>
        public NavigationItem LoadNavigation(string siteId, string navigationId)
        {
            using (Context.Tracer.DoTrace("NavigationActions.LoadNavigation(siteId={0},navigationId={1})", siteId, navigationId))
            {
                return LoadNavigation(siteId, navigationId, string.Empty);
            }
        }

        /// <summary>
        /// Get the navigation items from agilix.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="navigationId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public NavigationItem GetNavigation(string siteId, string navigationId, string categoryId)
        {
            NavigationItem menu = new NavigationItem() { Id = navigationId };

            using (Context.Tracer.StartTrace(String.Format("NavigationActions.GetNavigation(siteId={0}, navigationId={1}, categoryId={2})", siteId, navigationId, categoryId)))
            {
                var userId = Context.CurrentUser != null ? Context.CurrentUser.Id : "";
                var menuData = ContentActions.ListChildren(siteId, navigationId, categoryId, userId);

                var root = menuData.FirstOrDefault();

                if (null != root)
                {
                    root = RemoveHidden(root);
                    menu = root.ToMenuItem(Context, categoryId);
                    if (!root.Children.IsNullOrEmpty())
                    {
                        menu.Links = root.Children.Where(i => i.Type == Bfw.Agilix.Dlap.DlapItemType.AssetLink || i.Type == Bfw.Agilix.Dlap.DlapItemType.CustomActivity).Map(mi => mi.ToContentItem(Context)).OrderBy(mi => mi.Sequence).ToList();
                        menu.Children = root.Children.Where(i => i.Type == Bfw.Agilix.Dlap.DlapItemType.Folder).Map(mi => mi.ToMenuItem(Context, categoryId)).OrderBy(mi => mi.Sequence).ToList();

                        foreach (var child in menu.Children)
                        {
                            var subMenu = GetNavigation(siteId, child.Id, categoryId);
                            if (!subMenu.Children.IsNullOrEmpty())
                            {
                                child.Children.AddRange(subMenu.Children);
                            }

                            if (!subMenu.Links.IsNullOrEmpty())
                            {
                                if (child.Links == null)
                                {
                                    child.Links = new List<ContentItem>();
                                }
                                child.Links.AddRange(subMenu.Links);
                            }
                        }
                    }
                }
            }

            return menu;
        }


        /// <summary>
        /// Loads a navigation entity for the given site.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="navigationId"></param>
        /// <returns></returns>
        public NavigationItem LoadNavigation(string siteId, string navigationId, string categoryId)
        {

            NavigationItem menu = new NavigationItem() { Id = navigationId };

            var excludeWithParents = new string[] { ContentActions.TemplateFolder, ContentActions.TemporaryFolder };

            using (Context.Tracer.StartTrace(String.Format("NavigationActions.LoadNavigation(siteId={0}, navigationId={1}, categoryId={2})", siteId, navigationId, categoryId)))
            {
                var userId = Context.CurrentUser != null ? Context.CurrentUser.Id : "";
                IEnumerable<Item> menuData = null;
                if (categoryId == "ebook")
                {
                    var ebook = ContentActions.GetContent(Context.EntityId, navigationId);
                    menuData = ContentActions.ListChildren(Context.EnrollmentId, ebook.DefaultCategoryParentId, "", userId);
                }
                else
                {
                    menuData = ContentActions.ListChildren(siteId, navigationId, categoryId, userId);
                }

                var root = menuData.FirstOrDefault();

                if (null != root)
                {
                    root = RemoveHidden(root);
                    menu = root.ToMenuItem(Context, categoryId);
                    if (!root.Children.IsNullOrEmpty())
                    {
                        //Do not show the folder in the navigation when My Materials category is shown
                        if (categoryId == System.Configuration.ConfigurationManager.AppSettings["MyMaterials"])
                        {
                            menu.Children = root.Children.Where(a => a.Type != Bfw.Agilix.Dlap.DlapItemType.Folder).Map(mi => mi.ToMenuItem(Context, categoryId)).OrderBy(mi => mi.Sequence).ToList();
                            menu.Children.RemoveAll(mi => mi.Categories.Any(c => c.ItemParentId == "PX_TEMP"));
                        }
                        else
                        {
                            menu.Children = root.Children.Map(mi => mi.ToMenuItem(Context, categoryId)).OrderBy(mi => mi.Sequence).ToList();
                        }
                    }
                }
            }

            if (menu.Children != null && menu.Children.Count > 0)
            {
                menu.Children = menu.Children.Filter(c => !excludeWithParents.Contains(c.ParentId)).ToList();
            }

            return menu;
        }

        /// <summary>
        /// Loads a navigation entity for the given site.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="navigationId"></param>
        /// <param name="categoryId"></param>
        /// <param name="loadChild">Load Student child items at page load</param>
        /// <returns></returns>
        public NavigationItem LoadNavigation(string siteId, string navigationId, string categoryId, bool loadChild)
        {
            NavigationItem menu = new NavigationItem() { Id = navigationId };

            var excludeWithParents = new string[] { ContentActions.TemplateFolder, ContentActions.TemporaryFolder };

            using (Context.Tracer.StartTrace(String.Format("NavigationActions.LoadNavigation(siteId={0}, navigationId={1}, categoryId={2})", siteId, navigationId, categoryId)))
            {
                var userId = Context.CurrentUser != null ? Context.CurrentUser.Id : "";
                if (Context.IsPublicView)
                {
                    userId = Context.Course.CourseOwner;
                }
                IEnumerable<Item> menuData = null;
                if (categoryId == "ebook")
                {
                    var ebook = ContentActions.GetContent(Context.EntityId, navigationId);
                    menuData = ContentActions.ListChildren(Context.EnrollmentId, ebook.DefaultCategoryParentId, "", userId, loadChild);
                }
                else
                {
                    menuData = ContentActions.ListChildren(siteId, navigationId, categoryId, userId, loadChild);
                }

                var root = menuData.FirstOrDefault();

                if (null != root)
                {
                    root = RemoveHidden(root);
                    menu = root.ToMenuItem(Context, categoryId);
                    if (!root.Children.IsNullOrEmpty())
                    {
                        //Do not show the folder in the navigation when My Materials category is shown
                        if (categoryId == System.Configuration.ConfigurationManager.AppSettings["MyMaterials"])
                        {
                            menu.Children = root.Children.Where(a => a.Type != Bfw.Agilix.Dlap.DlapItemType.Folder).Map(mi => mi.ToMenuItem(Context, categoryId)).OrderBy(mi => mi.Sequence).ToList();
                            menu.Children.RemoveAll(mi => mi.Categories.Any(c => c.ItemParentId == "PX_TEMP"));
                        }
                        else
                        {
                            menu.Children = root.Children.Map(mi => mi.ToMenuItem(Context, categoryId)).OrderBy(mi => mi.Sequence).ToList();
                        }
                    }
                }
            }

            if (menu.Children != null && menu.Children.Count > 0)
            {
                menu.Children = menu.Children.Filter(c => !excludeWithParents.Contains(c.ParentId)).ToList();
            }

            return menu;
        }

        /// <summary>
        /// To determine if a course has content created by the active user.
        /// </summary>
        /// <param name="siteId"></param>
        /// <param name="navigationId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        ///     
        [Obsolete("DO NOT USE THIS IT IS HORRIBLY SLOW: Use ContentHelper.HasMyMaterials instead")]
        public bool HasUserMaterials(string siteId, string navigationId, string categoryId)
        {
            if (categoryId.ToLowerInvariant() != System.Configuration.ConfigurationManager.AppSettings["MyMaterials"].ToLowerInvariant())
                return false;

            NavigationItem menu = new NavigationItem() { Id = navigationId };
            bool hasUserMaterials = false;

            using (Context.Tracer.StartTrace(String.Format("NavigationActions.HasUserMaterials(siteId={0}, navigationId={1}, categoryId={2})", siteId, navigationId, categoryId)))
            {
                var userId = Context.CurrentUser != null ? Context.CurrentUser.Id : "";
                var menuData = ContentActions.ListChildren(siteId, navigationId, categoryId, userId);

                var root = menuData.FirstOrDefault();
                menu = root.ToMenuItem(Context, categoryId);

                if (!root.Children.IsNullOrEmpty())
                {
                    //Do not show the folder (or module) in the navigation when My Materials category is shown.
                    menu.Children = root.Children.Where(a => a.Type != Bfw.Agilix.Dlap.DlapItemType.Folder).Map(mi => mi.ToMenuItem(Context, categoryId)).OrderBy(mi => mi.Sequence).ToList();
                }

                var excludeWithParents = new string[] { ContentActions.TemplateFolder, ContentActions.TemporaryFolder };
                menu.Children = menu.Children.Filter(c => !excludeWithParents.Contains(c.ParentId)).ToList();

                hasUserMaterials = menu.Children.Count() > 0;
            }
            return hasUserMaterials;
        }

        /// <summary>
        /// Gets all child widget items under the specified parent item.
        /// </summary>
        /// <param name="parentId">ID of the parent.</param>
        /// <returns></returns>
        public List<ContentItem> GetWidgets(string parentId)
        {
            using (Context.Tracer.DoTrace("NavigationActions.GetWidgets(parentId={0})", parentId))
            {
                var items = new List<ContentItem>();
                foreach (var item in ContentActions.ListChildren(Context.EntityId, parentId))
                {
                    if (item.Properties.ContainsKey("bfw_type"))
                    {
                        if (item.Properties["bfw_type"].Value.ToString().ToLowerInvariant() == "widgetconfiguration")
                        {
                            items.Add(item);
                        }
                    }
                }

                return items;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Removes all child items that are marked as hidden.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <returns></returns>
        private Item RemoveHidden(Item root)
        {
            if (!root.ItemIsHidden(Context))
            {
                root.Children = RemoveHidden(root.Children);
            }

            return root;
        }

        /// <summary>
        /// Removes all child items that are marked as hidden.
        /// </summary>
        /// <param name="children">The children item collection.</param>
        /// <returns></returns>
        private List<Item> RemoveHidden(IEnumerable<Item> children)
        {
            using (Context.Tracer.StartTrace("NavigationActions.RemoveHidden"))
            {
                var result = new List<Item>();

                if (!children.IsNullOrEmpty())
                {
                    foreach (var child in children)
                    {
                        if (!child.ItemIsHidden(Context))
                        {
                            // Hidden from toc isn't true, and the user viewing the content is allowed to see it.
                            result.Add(child);
                            child.Children = RemoveHidden(child.Children);
                        }
                    }
                }

                return result.OrderBy(i => i.Sequence).ToList();
            }
        }

        #endregion
    }
}