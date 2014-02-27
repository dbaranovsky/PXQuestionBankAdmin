using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Bfw.Common.Collections;
using Bfw.PX.PXPub.Models;
using Bfw.PX.PXPub.Components;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers
{
    /// <summary>
    /// Default controller used to render the index page
    /// </summary>
    
    [PerfTraceFilter]
    public class PageActionController : Controller
    {

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Access to a content helper object.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper { get; set; }

        /// <summary>
        /// Access to the current business context information.
        /// </summary>
        protected BizSC.IBusinessContext Context { get; set; }

        /// <summary>
        /// Perform opertaions on the course materials created for the current course
        /// </summary>
        protected BizSC.ICourseMaterialsActions CourseMaterialsActions { get; set; }

        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Access to an INavigationActions implementation.
        /// </summary>
        protected BizSC.INavigationActions NavigationActions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="pageActions">Page Actions</param>
        public PageActionController(BizSC.IPageActions pageActions, IContentHelper helper, BizSC.IBusinessContext context, 
            BizSC.INavigationActions navigationActions, BizSC.ICourseMaterialsActions courseMaterialsActions, BizSC.IContentActions contentActions)
        {
            PageActions = pageActions;
            ContentHelper = helper;
            Context = context;
            NavigationActions = navigationActions;
            CourseMaterialsActions = courseMaterialsActions;
            ContentActions = contentActions;
        }


        public ActionResult AddWidget(string pageName, string zoneID, string templateID, string prevSequence, string nextSequence,string newWidgetID)
        {
            bool isOnBeforeAddRequired = false;
            Widget widgetTemplate = null;

            if (newWidgetID == "NotKnownYet") // Default Case
            {
                widgetTemplate = this.PageActions.GetWidgetTemplate(templateID).ToWidgetItem();
                isOnBeforeAddRequired = widgetTemplate.Callbacks.ContainsKey("OnBeforeAdd");
            }

            if (!isOnBeforeAddRequired) // Custom Widget: to update parent and sequence
            {
                var newWidget = this.PageActions.AddWidget(pageName, zoneID, templateID, prevSequence, nextSequence, newWidgetID).ToWidgetItem();
                ContentHelper.InvalidateCachedPageDefinitionsForDerivedCourses(Context.Course.Id, pageName);
                return View("WidgetWrapper", newWidget);
            }
            else // Custom Widget: to open a dialog box to get input values
            {
                return View("WidgetWrapperBeforeAdd", widgetTemplate);
            }
        }

        public ActionResult EditWidget(string widgetId)
        {
            Widget widget = this.PageActions.GetWidget(widgetId).ToWidgetItem();
            return View("WidgetWrapperBeforeAdd", widget);
        }

        public ActionResult GetWidgetTemplate(string templateID)
        {
            Widget widget = this.PageActions.GetWidgetTemplate(templateID).ToWidgetItem();
            string result = string.Empty;
            string msg = string.Empty;
            string saveController = string.Empty;
            string saveAction = string.Empty;
            string viewController = string.Empty;
            string viewAction = string.Empty;

            if (widget.Callbacks.ContainsKey("Save"))
            {
                saveController = widget.Callbacks["Save"].Controller;
                saveAction = widget.Callbacks["Save"].Action;

                viewController = widget.Callbacks["View"].Controller;
                viewAction = widget.Callbacks["View"].Action;
                result = "SUCCESS";
            }
            else
            {
                result = "FAIL";
                msg = "Save Callback is not defined with this widget";
            }
            return Json(new { Result = "SUCCESS", StatusMessage = msg, WidgetSaveController = saveController, WidgetSaveAction = saveAction, WidgetViewController = viewController, WidgetViewAction = viewAction});
        }


        public ActionResult AddMenuItem(MenuItem menuItem, string prevSequence, string nextSequence)
        {
            var dcMenuItem = menuItem.ToMenuItem();
            if (string.IsNullOrEmpty(dcMenuItem.Sequence))
            {
                dcMenuItem.Sequence = Context.Sequence(menuItem.minSequence, menuItem.maxSequence);
            }

            var newMenItem = this.PageActions.AddMenuItem("PX_PRIMARY", dcMenuItem, new Dictionary<string, string>()).ToMenuItem();
            return View("WidgetWrapper", newMenItem);
        }

        public ActionResult RemoveWidget(string widgetID, string pageName)
        {
            this.PageActions.RemoveWidget(widgetID, pageName);
            ContentHelper.InvalidateCachedPageDefinitionsForDerivedCourses(Context.Course.Id, pageName);
            return Json(new { Result = "Deleted Successfully..." });
        }

        public ActionResult RemoveMenuItem(string menuId, string menuItemId)
        {
            this.PageActions.RemoveMenuItem(menuId, menuItemId);
            return Json(new { Result = "Deleted Successfully..." });
        }

        public ActionResult SetWidgetDisplay(string widgetID, string strDisplayOptions)
        {
            var widgetDisplayOptions = ParseWidgetDisplayOptions(strDisplayOptions);
            PageActions.SetWidgetDisplay(widgetID, widgetDisplayOptions.ToWidgetDisplayOptions());
            return Json(new { Result = "SET WIDGET DISPLAY" });
        }

        public ActionResult MoveWidget(string pageName, string zoneName, string widgetId, string minSequence, string maxSequence)
        {
            string newSequence =  this.PageActions.MoveWidget(pageName, zoneName, widgetId, minSequence, maxSequence);
            if (!String.IsNullOrEmpty(newSequence))
                ContentHelper.InvalidateCachedPageDefinitionsForDerivedCourses(Context.Course.Id, pageName);
            return Json(new { Result = "Widget Updated Successfully...", Sequence = newSequence });
        }

        public ActionResult MoveMenuItem(string menuId, string menuItemId, string minSequence, string maxSequence)
        {
            string newSequence = this.PageActions.MoveMenuItem(menuId, menuItemId, minSequence, maxSequence);
            return Json(new { Result = "MenuItem Updated Successfully...", Sequence = newSequence });
        }

        protected WidgetDisplayOptions ParseWidgetDisplayOptions(string strDisplayOptions)
        {
            var displayOptions = strDisplayOptions.Split(new char[] { ',' });
            var widgetDisplayOptions = new WidgetDisplayOptions();   

            foreach (var displayOption in displayOptions)
            {
                widgetDisplayOptions.DisplayOptions.Add((BizDC.DisplayOption)Enum.Parse(typeof(BizDC.DisplayOption), displayOption, true));
            }
            return widgetDisplayOptions;
        }

        public ActionResult RenameCourse(string CourseName, string pageName)
        {
            string result = string.Empty;
            this.PageActions.RenameCourse(CourseName, pageName);
            Context.RefreshCourse();
            result = "SUCCESS";
            return Json(new { Result = result });
        }

        public ActionResult MenuItemManage(string menuItemId)
        {
            var menu = this.PageActions.LoadMenu("PX_PRIMARY").ToMenu();
            menu.SelectedMenuItem = menu.MenuItems.FirstOrDefault(i => i.Id == menuItemId);
            if (menu.SelectedMenuItem == null)
            {
                menu.SelectedMenuItem = new MenuItem();
                menu.SelectedMenuItem.WidgetDisplayOptions.DisplayOptions.Add(BizDC.DisplayOption.Instructor);
                menu.SelectedMenuItem.WidgetDisplayOptions.DisplayOptions.Add(BizDC.DisplayOption.Student);
                menu.SelectedMenuItem.VisibleByInstructor = true;
                menu.SelectedMenuItem.VisibleByStudent = true;
               
            }

            var selectedId = menu.GetSelectItemContentIdFromParameter();
            if (string.IsNullOrEmpty(selectedId))
            {
                selectedId = menu.BfwTocId;
            }
            else
            {
                menu.SelectedMenuItem.ContentItemId = selectedId;
            }


            var toc = ContentHelper.LoadToc(Context.EntityId, selectedId, "");
            ViewData["toc"] = toc;
            ViewData.Model = menu;
            return View();
        }

        public ActionResult MceAddLink(string menuItemId)
        {
            return View("MceAddLink");
        }

        /// <summary>
        /// Reuturns the ebook editor for the add link dialog of tinyMCE
        /// </summary>
        /// <returns></returns>
        public ActionResult MceEbookEditor()
        {
            var ebook = ContentActions.ListContent(Context.EntityId, "Ebook").Map(c => c.ToEbook()).ToList();
            var accessLevel = Context.AccessLevel;
            List<TocItem> toc = new List<TocItem>();
            foreach(var book in ebook)
            {
                //Instructor or a student can browse the ebooks wich are available to the students
                if (!string.IsNullOrEmpty(book.Title) && !book.HiddenFromStudents)
                {
                    var root = book.RootId;
                    var category = book.CatagoryId;
                    toc.AddRange(ContentHelper.LoadTocWithAllChild(Context.EntityId, root, category));
                }
            }
            ViewData["toc"] = toc;
            return View("MceEbookEditor");
        }

        /// <summary>
        /// Reuturns the course materials editor for the add link dialog of tinyMCE
        /// </summary>
        /// <returns></returns>
        public ActionResult MceCourseMaterial()
        {
            var courseMaterialsModel = CourseMaterialsActions.GetCourseMaterials().ToCourseMaterials();
            return View("MceCourseMaterials", courseMaterialsModel);
        }

        /// <summary>
        /// Returns the content editor for the add link dialog of tinyMCE
        /// </summary>
        /// <returns></returns>
        public ActionResult MceContentEditor()
        {
            var root = string.Empty;
            var enrollment = string.Format("enrollment_{0}", Context.EnrollmentId);
            var toc = ContentHelper.LoadTocWithAllChild(Context.EntityId, root, enrollment);
            ViewData["toc"] = toc;
            return View("MceContentEditor");
        }


        public ActionResult ContentPicker(string id, string category)
        {
            if (string.IsNullOrEmpty(id))
            {
                var menu = this.PageActions.LoadMenu("PX_PRIMARY").ToMenu();
                id = menu.BfwTocId;
            }

            var toc = ContentHelper.LoadToc(Context.EntityId, id, category);
            ViewData["toc"] = toc;
            ViewData.Model = toc;
            if (!string.IsNullOrEmpty(category) && !toc.IsNullOrEmpty())
            {
                ViewData.Model = toc.First().Children;
            }

            ViewData["first"] = true;
            return View();
        }

        public ActionResult ContentPickerNew(string id, string category)
        {
            if (string.IsNullOrEmpty(id))
            {
                var menu = this.PageActions.LoadMenu("PX_PRIMARY").ToMenu();
                id = menu.BfwTocId;
            }

            var toc = ContentHelper.LoadToc(Context.EntityId, id, category);
            ViewData["toc"] = toc;
            ViewData.Model = toc;
            ViewData["first"] = true;
            return View();
        }

        public ActionResult ExpandContentPicker(string id)
        {
            var bizNavigationItem = NavigationActions.LoadNavigation(Context.EntityId, id, "");
            IEnumerable<TocItem> toc = new List<TocItem>();

            if (!bizNavigationItem.Children.IsNullOrEmpty())
            {
                string activeId = string.Empty;

                if (id == "PX_TOC")
                {
                    activeId = bizNavigationItem.Children[0].Id;
                    bizNavigationItem.Children[0] = NavigationActions.LoadNavigation(Context.EntityId, activeId, "");
                }

                toc = bizNavigationItem.Children.Map(ti => ti.ToTocItem(activeId, ContentHelper.ShowTocControls()));
            }

            ViewData.Model = toc;

            if (toc.Count() == 0)
            {
                ViewData["Description"] = bizNavigationItem.ToTocItem().Description;
            }

            ViewData["category"] = "";
            ViewData["HasUserMaterials"] = NavigationActions.HasUserMaterials(Context.EntityId, "PX_TOC", "");
            return View("ContentPicker");
        }

        public ActionResult ExpandContentPickerUnit(string id, string toc = "syllabusfilter")
        {
            var unit = ContentHelper.LoadUnit(id, toc);
            var childItems = new List<TocItem>();

            if (unit != null)
            {
                foreach (var item in unit.GetAssociatedItems())
                {
                    var tocItem = new TocItem(item.Title, item.Id, "", item.Description, null);
                    tocItem.IsPartOfAssignmentCenter = unit.Categories.IsAssociatedWithAssignmentCenter();
                    childItems.Add(tocItem);                    
                }
            }

            if (childItems.Count > 0)
            {
                ViewData.Model = childItems;
            }

            return View("ContentPicker");            
        }

        public ActionResult SaveMenuItem(MenuItem menuItem, string behavior)
        {
            var parameters = new Dictionary<string, string>();
            ViewData["AccessLevel"] = Context.AccessLevel;

            if ( !string.IsNullOrEmpty(menuItem.ContentItemId))
                parameters.Add("Id", menuItem.ContentItemId);

            if (behavior.ToLowerInvariant() == "template")
            {
                if (menuItem.BfwMenuCreatedby.IsNullOrEmpty())
                {
                    ModelState.AddModelError("TemplateId", "You must specify a TemplateId");
                }

                if (ModelState.IsValid)
                {
                    var dcMenuItem = menuItem.ToMenuItem();
                    if (string.IsNullOrEmpty(dcMenuItem.Sequence))
                    {
                        dcMenuItem.Sequence = Context.Sequence(menuItem.minSequence, menuItem.maxSequence);
                    }
                    
                    var model = this.PageActions.AddMenuItem("PX_PRIMARY", dcMenuItem, parameters).ToMenuItem();                    
                    return View("MenuItem", model);
                }
            }
            else if (behavior.ToLowerInvariant() == "save")
            {
                if (menuItem.Title.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Title", "You must specify a Title");
                }

                if (menuItem.Url.IsNullOrEmpty())
                {
                    ModelState.AddModelError("Url", "You must specify a Url");
                }

                if (menuItem.BfwMenuCreatedby.IsNullOrEmpty())
                {
                    ModelState.AddModelError("BfwMenuCreatedby", "You must specify a BfwMenuCreatedby");
                }

                if (ModelState.IsValid)
                {
                    var dcMenuItem = menuItem.ToMenuItem();
                    if (string.IsNullOrEmpty(dcMenuItem.Sequence))
                    {
                        dcMenuItem.Sequence = Context.Sequence(menuItem.minSequence, menuItem.maxSequence);
                    }

                    //var model = this.PageActions.AddMenuItem("PX_PRIMARY", menuItem.Id, menuItem.BfwMenuCreatedby, menuItem.Title, menuItem.Url, menuItem.minSequence, menuItem.maxSequence, parameters);

                    var model = this.PageActions.AddMenuItem("PX_PRIMARY", dcMenuItem, parameters);
                    return View("MenuItem", model.ToMenuItem());
                }
            }

            return new EmptyResult();
        }
        public ActionResult LoadPage(string pageId)
        {

            var layout = new LayoutConfiguration() { Title = pageId };

            var pageDefinitions = this.PageActions.LoadPageDefinition(pageId);
            layout.PageDefinitions = pageDefinitions.ToPageDefinition();
            ViewData["AccessLevel"] = Context.AccessLevel.ToString().ToLowerInvariant();
            ViewData["IsAllowedToCreateCourse"] = Context.CanCreateCourse;
            ViewData["IsProductCourse"] = Context.CourseIsProductCourse;
            ViewData.Model = layout.PageDefinitions;


            return View("PageContainer");

        }
    }
}



