using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Bfw.PX.Abstractions;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.Common.Collections;
using Bfw.Common;

using BizSC = Bfw.PX.Biz.ServiceContracts;
using System.Xml.Linq;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    public class ContentAreaWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }

        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }
        protected BizSC.IContentActions ContentActions { get; set; }
        public ContentAreaWidgetController(BizSC.IBusinessContext c, BizSC.IEnrollmentActions e, BizSC.IContentActions ca)
        {
            Context = c;
            EnrollmentActions = e;
            ContentActions = ca;
        }

        public ActionResult GetDocumentViewer(string contentItemId)
        {
            ViewData["CourseTitle"] = Context.Course == null ? string.Empty : Context.Course.Title;
            return View("~/Views/ContentAreaWidget/ContentArea.ascx", GetContentArea(contentItemId));
        }

        public ActionResult GetRelatedContents(string itemId, string categoryName = "bfw_toc_contents")
        {
            var items = ContentActions.GetRelatedItems(Context.EntityId, itemId);
            
            // remove instructor only items for students
            if (Context.AccessLevel == AccessLevel.Student)
                items = items.Where(x => XDocument.Parse(x.Visibility).Descendants("student").Count() != 0);
            
            // identify a unique list of parent ids for the related content
            IEnumerable<string> parentids = items.Map(f => f.Categories.Where(y => y.Id == categoryName).First().ItemParentId).Distinct((a, b) => a == b);

            // order the parent items by their toc sequence
            var parentitems = ContentActions.GetItems(Context.EntityId, parentids.ToList()).OrderBy(e => e.Categories.Where(y => y.Id == categoryName).FirstOrDefault().Sequence).ToList();

            var relatedItems = new List<RelatedItems>();

            // create an Model object to pass to the view
            foreach (var parentItem in parentitems)
            {
                var category = new RelatedItems();

                category.Category = parentItem.Title;
                // finding the children for each category and ordering them based on the TOC sequence
                category.Items = items.Where(x => x.Categories.Where(y => y.Id == categoryName).FirstOrDefault().ItemParentId == parentItem.Id).
                                     OrderBy(x => x.Categories.Where(y => y.Id == categoryName).FirstOrDefault().Sequence).Select(c =>
                                        new RelatedItem(c.ToContentItem(ContentActions), Url.GetComponentHash("item", c.Id, new
                                        {
                                            mode = ContentViewMode.Preview,
                                            includeNavigation = false,
                                            isBeingEdited = false,
                                            renderFNE = true,
                                            isRelatedContent = true
                                        }))
                                    ).ToList();
                
                relatedItems.Add(category);
            }

            ViewData["Count"] = items.Count();
            ViewData["ShowRelatedContent"] = Context.Course.ShowRelatedContent;
            ViewData.Model = relatedItems;
            return View("~/Views/Shared/RelatedItems.ascx");
        }

        /// <summary>
        /// Used in the ContentAreaWidget, reutns a view that combines the XBookContentOptions view with a default button click option.
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns></returns>
        public ActionResult ContentAreaViewOptions(string contentItemId)
        {
            return View(GetContentArea(contentItemId));
        }

        /// <summary>
        /// Returns the dropdown options widget with options specified to xbook.
        /// </summary>
        /// <param name="id">ID of the content item</param>
        /// <param name="mode">Default view mode for the content</param>
        /// <param name="renderInFne">Render in the FNE window.</param>
        /// <returns>Dropdown widget containing view options for the content item</returns>
        public ActionResult XbookContentOptions(string id, ContentViewMode mode, bool renderInFne)
        {
            var itemBiz = ContentActions.GetContent(Context.EntityId, id);
            var itemModel = itemBiz.ToContentItem(ContentActions);
            var model = new XbookContentOptions(id, itemModel.GetType(), Context.ProductType, Context.AccessLevel,
                                                Context.Course.IsSandboxCourse, Context.IsSharedCourse, Context.CourseIsProductCourse,
                                                itemModel.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString());
            model.ActiveOption = mode;
            return View(model);
        }

        /// <summary>
        /// Returns he ContentItem Subtype property
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns>ContentItem.Subtype or string.Empty if <paramref name="contentItemid"/> is invalid</returns>
        public string GetContentType(string contentItemId)
        {
            var items = ContentActions.GetItems(Context.CourseId, new List<string>() { contentItemId }, false);
            if (items.Count() == 1)
                return items.ElementAt(0).Subtype.ToLower();
            return string.Empty;
        }

        /// <summary>
        /// Creates a ContentArea object for the ContentItem associated with the id
        /// </summary>
        /// <param name="contentItemId"></param>
        /// <returns>New ContentArea object containing display options for the <paramref name="contentItemId"/>. Null if <paramref name="contentItemId"/> is invalid</returns>
        private ContentArea GetContentArea(string contentItemId)
        {
            var item = ContentActions.GetContent(Context.CourseId, contentItemId, true);
            if (item == null)
                return null;

            var content = item.ToContentItem(ContentActions, false);
            var options = new XbookContentOptions(content.Id, content.GetType(), Context.ProductType, Context.AccessLevel,
                                            Context.Course.IsSandboxCourse, Context.IsSharedCourse, Context.CourseIsProductCourse,
                                            content.DueDate.ToShortDateString() != DateTime.MinValue.ToShortDateString());
            content.UserAccess = Context.AccessLevel;
            return new ContentArea(content, options);
        }
        #region IPXWidget Members
        public ActionResult Summary(Models.Widget widget)
        {
            ViewData["CourseTitle"] = Context.Course == null ? string.Empty : Context.Course.Title;
            return View();
        }

        public ActionResult ViewAll(Models.Widget widget)
        {
            throw new NotImplementedException();
        }
        #endregion IPXWidget Members
    }
}
