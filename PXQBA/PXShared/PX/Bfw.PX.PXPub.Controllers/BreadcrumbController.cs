using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using ContentItem = Bfw.PX.Biz.DataContracts.ContentItem;
using CourseType = Bfw.PX.PXPub.Models.CourseType;

namespace Bfw.PX.PXPub.Controllers
{
    /// <summary>
    /// Handles actions related to breadcrumbs
    /// </summary>
    [PerfTraceFilter]
    public class BreadcrumbController : Controller
    {
        /// <summary>
        /// Access to the current business context information
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Access to an INavigationActions implementation
        /// </summary>
        /// <value>
        /// The navigation actions.
        /// </value>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Access to an IContentActions implementation
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Constructs a default TocWidgetController. Depends on a business context
        /// and user actions implementation
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="navigationActions">The navigation actions.</param>
        /// <param name="contentActions">The content actions.</param>
        public BreadcrumbController(BizSC.IBusinessContext context, BizSC.INavigationActions navigationActions, BizSC.IContentActions contentActions)
        {
            Context = context;
            NavigationActions = navigationActions;
            ContentActions = contentActions;
        }

        /// <summary>
        /// Get the info necessary for breadcrum trail above a certain item
        /// </summary>
        /// <param name="itemId">The item above which to look for</param>
        /// <param name="rootItemIds">The root items</param>
        /// <param name="faceplateCategory">The category.</param>
        /// <param name="courseType">Type of course</param> 
        /// <returns></returns>
        public ActionResult BreadcrumbTrail(string itemId, string[] rootItemIds, string faceplateCategory, 
            CourseType courseType = CourseType.LearningCurve, bool isQuestionBank = false, string quizId = "",
            string toc = "syllabusfilter")
        {
            using (Context.Tracer.StartTrace(string.Format("breadcrumbtrail for: {0} category {1}", itemId, faceplateCategory)))
            {
                itemId = itemId.Replace("Shortcut__1__", "");
                string category = courseType == CourseType.FACEPLATE ? "syllabusfilter" : "";
                for (int i = 0; i < rootItemIds.Length; i++)
                {
                    rootItemIds[i] = rootItemIds[i].Replace("Shortcut__1__", "");
                }

                if (courseType == CourseType.FACEPLATE && (itemId == "ebook" || itemId == "launchpad"))
                {//if ebook or launchpad chosen, use RootItemId instead
                    itemId = rootItemIds[0];
                    var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, itemId, "");
                    return BreadcrumbTrail(itemId = bizNavigationItem.Children[0].Id, rootItemIds, faceplateCategory, courseType);
                }
                else if (rootItemIds.Contains(itemId)) // If we're looking for the root item, just use its first child instead
                {
                    var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, itemId, faceplateCategory);

                    if (bizNavigationItem.Children.Count > 0 && bizNavigationItem.Children[0].Id != itemId)
                    {
                        return BreadcrumbTrail(itemId = bizNavigationItem.Children[0].Id, rootItemIds, faceplateCategory, courseType);
                    }
                    else
                    {
                        return null;
                    }
                }


                Stack<BizDC.NavigationItem> q = new Stack<BizDC.NavigationItem>();
                var item = NavigationActions.LoadNavigation(Context.EntityId, itemId, category);

                q.Push(item);

                if (courseType == CourseType.FACEPLATE)
                { //for Faceplate, use bfw_toc_contents/syllabusfilter node to walk up the parent tree. 
                    while (item != null && item.Categories != null && !rootItemIds.Any(r => item.Categories.Select(c => c.ItemParentId).Contains(r)) && q.Count<6)
                    {
                        try
                        {
                            string parentId = item.GetSyllabusFilterFromCategory(toc);
                            if (string.IsNullOrEmpty(parentId))
                                break;
                            item = NavigationActions.LoadNavigation(Context.EntityId, parentId, category);
                            if (item != null && item.Categories != null) q.Push(item);
                        }
                        catch(Exception ex)
                        {
                            break;
                        }
                    }

                    if (faceplateCategory == "ebook" && q.Count > 1)
                    { //remove non-ebook entries from the 3rd level of the breadcrumb
                        q.ToArray()[1].Children.RemoveAll(
                            c => !c.Categories.Any(cat => cat.Id == "bfw_faceplate_filter" && cat.Text == "ebook"));
                    }
                }
                else
                {
                    var itemids = q.Select(i => i.Id).ToList();
                    while (!rootItemIds.Contains(item.ParentId))
                    {
                        item = NavigationActions.LoadNavigation(Context.EntityId, item.ParentId);
                        if (itemids.Contains(item.Id))
                            break;
                        q.Push(item);
                        itemids.Add(item.Id);
                        if (item.ParentId == "PX_ROOT") { q.Pop(); break; }//FIX FOR FACEPLATE ITEMS AS THEY DON'T BELONG TO PX_TOC.
                    }
                }


                var model = new Trail() { Levels = new List<Trail.Level>(), RootItem = item.ParentId };

                if (courseType == CourseType.FACEPLATE)
                {
                    var level = new Trail.Level()
                    {
                        Items = new List<Trail.Breadcrumb>(){ new Trail.Breadcrumb() {Display = "Launch Pad", Id="launchpad"}}
                    };
                    level.Selected = 0;
                    model.Levels.Add(level);

                }
                while (q.Count > 0)
                {
                    try
                    {
                        var levelForItem = LevelForItem(q.Pop(), faceplateCategory, courseType, toc);
                        model.Levels.Add(levelForItem);
                    }
                    catch { }
                }

                ViewData.Model = model;
            }
            ViewData["quizId"] = quizId;
            ViewData["isQuestionBank"] = isQuestionBank;
            Random rand = new Random((int)DateTime.Now.Ticks);
            int RandomNumber;
            RandomNumber = rand.Next(100000, 999999);
            ViewData["randomNumber"] = RandomNumber;
            return View("Breadcrumb");
        }

        /// <summary>
        /// Build a breadcrumb level given one item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="category">Catagory of items to display (ebook, launchpad)</param>
        /// <returns></returns>
        private Trail.Level LevelForItem(BizDC.NavigationItem item, string faceplateCategory, CourseType courseType, 
            string toc)
        {
            // Get this item and all of its siblings by getting the. 
            // children of its parent.  Also, filter out hidden items.
            string category = "";
            string parentId = "";
            if (courseType == CourseType.FACEPLATE)
            {
                category = "syllabusfilter";
                parentId = item.GetSyllabusFilterFromCategory(toc);

            }
            if (string.IsNullOrWhiteSpace(parentId) || parentId == "PX_FACEPLATE_DEFAULTCATEGORY")
            {
                parentId = item.ParentId;
            }

            var siblings =
                   ContentActions.ListChildren(Context.EntityId, parentId, 2, category).Filter(c => !c.Hidden).ToList();
            if (parentId.ToLower().Equals("px_multipart_lessons") && courseType == CourseType.FACEPLATE)
            {
                var siblings2 = ContentActions.ListChildren(Context.EntityId, parentId, 2, "").Filter(c => !c.Hidden).ToList();
                foreach (var sibling in siblings2)
                {
                    if (siblings.Where(i=> i.Id == sibling.Id).Count() == 0)
                    {
                        siblings.Add(sibling);
                    }
                }
            }

            if (siblings.Count < 1 || courseType != CourseType.FACEPLATE)
            {
                GetLegacySiblings(siblings, parentId);
            }
            siblings.ForEach(s => s.Id = s.Id.Replace("Shortcut__1__", ""));

            if (faceplateCategory == "ebook")
            {
                if (siblings.Any(s => s.Categories.Any(cat => cat.Id == "bfw_faceplate_filter" && cat.Text == "ebook")))
                {//if there are any items that contain an ebook catagory tag

                    //remove all items that don't contain ebook catagory tags
                    siblings.RemoveAll(
                        c => !c.Categories.Any(cat => cat.Id == "bfw_faceplate_filter" && cat.Text == "ebook"));
                }

            }

            var level = new Trail.Level() { Items = (IList<Trail.Breadcrumb>)siblings.Map(i => new Trail.Breadcrumb() { Display = i.Title, Id = i.Id }).ToList() };

            for (int i = 0; i < level.Items.Count; i++)
            {
                if (level.Items[i].Id == item.Id)
                {
                    level.Selected = i;
                    return level;
                }
            }
            throw new Exception("Could not determine selected item");
        }

        private void GetLegacySiblings(List<ContentItem> siblings, string parentId)
        {
            var legacySiblings =
                ContentActions.ListChildren(Context.EntityId, parentId, 2, "").Filter(c => !c.Hidden).ToList();

            //removing duplicate items from legacySiblings
            List<string> dups = new List<string>();
            foreach (var sibling in siblings)
            {
                string legacyIndexToRemove = "";
                for (int i = 0; i < legacySiblings.Count; i++)
                {
                    if (sibling.Id == legacySiblings[i].Id)
                        legacyIndexToRemove = i.ToString();
                }
                if (!legacyIndexToRemove.IsNullOrEmpty())
                {
                    legacySiblings.Remove(legacySiblings[Int32.Parse(legacyIndexToRemove)]);
                }
            }

            siblings.AddRange(legacySiblings);
        }
    }
}
