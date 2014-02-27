using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Direct.Services;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Bfw.Common;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    /// <summary>
    /// Provides actions necessary to support the Personal Eportfolio Presentation widget
    /// </summary>
    [PerfTraceFilter]
    public class InstructorConsoleWidgetController : Controller, IPXWidget
    {
        /// <summary>
        /// The current business context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }
        /// <summary>
        /// The featured content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }

        /// <summary>
        /// Gets or sets the course actions.
        /// </summary>
        /// <value>
        /// The course actions.
        /// </value>
        protected BizSC.ICourseActions CourseActions { get; set; }

        /// <summary>
        /// Gets or sets the Page actions.
        /// </summary>
        protected BizSC.IPageActions PageActions { get; set; }

        /// <summary>
        /// Access to an IAssignmnetActions implementation.
        /// </summary>
        protected BizSC.IAssignmentActions AssignmentActions { get; set; }

        /// <summary>
        /// Gets or sets the grade actions.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected BizSC.IGradeActions GradeActions { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected BizSC.IEnrollmentActions EnrollmentActions { get; set; }

        /// <summary>
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the assignment center helper.
        /// </summary>
        /// <value>
        /// The assignment center helper.
        /// </value>
        protected IAssignmentCenterHelper AssignmentCenterHelper
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the gradebook export helper.
        /// </summary>
        protected IGradebookExportHelper GradebookExportHelper
        {
            get;
            set;
        }

        private const string _defaultLmsMessage = "Students, enter your Campus ID here:";

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonalEportfolioPresentationWidgetController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content Actions.</param>       
        /// <param name="courseActions">The course Actions.</param>
        public InstructorConsoleWidgetController(
            BizSC.IBusinessContext context,
            BizSC.IContentActions contentActions,
            BizSC.ICourseActions courseActions,
            BizSC.IPageActions pageActions,
            BizSC.IAssignmentActions assignmentActions,
            BizSC.IGradeActions gradeActions,
            BizSC.IEnrollmentActions enrollmentActions,
            IContentHelper contentHelper,
            IAssignmentCenterHelper assignmentCenterHelper,
            IGradebookExportHelper gradebookExportHelper)
        {
            Context = context;
            ContentActions = contentActions;
            CourseActions = courseActions;
            PageActions = pageActions;
            AssignmentActions = assignmentActions;
            GradeActions = gradeActions;
            EnrollmentActions = enrollmentActions;
            ContentHelper = contentHelper;
            AssignmentCenterHelper = assignmentCenterHelper;
            GradebookExportHelper = gradebookExportHelper;
        }

        /// <summary>
        /// Summary view
        /// </summary>
        /// <returns></returns>
        public ActionResult Summary(Models.Widget widget)
        {
            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["CourseID"] = Context.CourseId;

            var settings = PageActions.GetInstructorConsoleSettings("PX_Instructor_Console_Widget");

            ViewData["ShowWelcomeReturn"] = Context.Course.IsLoadStartOnInit && settings.ShowWelcomeReturn;
            ViewData["ShowBatchUpdater"] = settings.ShowBatchUpdater;
            ViewData["ShowManageAnnouncemets"] = settings.ShowManageAnnouncemets;
            ViewData["IsSandboxCourse"] = Context.Course.IsSandboxCourse;

            return View();
        }

        /// <summary>
        /// Full Screen View
        /// </summary>
        /// <returns></returns>
        public ActionResult FullView(string view)
        {
            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["CourseID"] = Context.CourseId;

            if (view.IsNullOrEmpty())
            {
                ViewData["View"] = string.Empty;
                ViewData["Action"] = "View";
            }
            else
            {
                if (view == "BatchDueDateUpdater")
                {
                    ViewData["View"] = "FullView";
                    ViewData["Action"] = view;
                }
                else
                {
                    ViewData["View"] = view;
                    ViewData["Action"] = "Settings";
                }
            }

            return View();
        }

        /// <summary>
        /// Edit Screen View
        /// </summary>
        /// <returns></returns>
        public PartialViewResult EditView()
        {
            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["CourseID"] = Context.CourseId;
            ViewData["IsLoadStartOnInit"] = Context.Course.IsLoadStartOnInit;

            var widget = (this.PageActions.GetInstructorConsoleSettings("PX_Instructor_Console_Widget"));
            ViewData.Model = widget;

            return PartialView();
        }

        /// <summary>
        /// The Browse More Resources View
        /// </summary>
        /// <returns></returns>
        public ActionResult BrowseResourcesItems()
        {
            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["CourseID"] = Context.CourseId;

            var widget = (this.PageActions.GetInstructorConsoleSettings("PX_Instructor_Console_Widget"));
            ViewData.Model = widget;

            return View();
        }

        /// <summary>
        /// The Settings items list for the home page
        /// </summary>
        /// <returns></returns>
        public ActionResult SettingsItems()
        {
            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["CourseID"] = Context.CourseId;

            var widget = (this.PageActions.GetInstructorConsoleSettings("PX_Instructor_Console_Widget"));
            ViewData.Model = widget;

            return PartialView();
        }

        /// <summary>
        /// The Settings View
        /// </summary>
        /// <returns></returns>
        public ActionResult Settings(string view, string toc = "syllabusfilter")
        {
            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["CourseID"] = Context.CourseId;

            if (view.ToLower().Equals("dashboard"))
            {
                ViewData["fromDashboard"] = true;
                view = "general";
            }
            else
            {
                ViewData["fromDashboard"] = false;
            }

            if (view == "general" || view == "navigation" || view == "launchpad")
            {
                if (view == "navigation")
                {
                    var settings = PageActions.GetInstructorConsoleSettings("PX_Instructor_Console_Widget");
                    ViewData["IsLoadStartOnInit"] = Context.Course.IsLoadStartOnInit;
                }

                ViewData["View"] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(view);
            }
            else
            {
                ViewData["View"] = "General";
            }

            Bfw.PX.PXPub.Models.Course course = CourseActions.GetCourseByCourseId(Context.CourseId).ToCourse();
            course.PossibleAcademicTerms = CourseActions.ListAcademicTerms().Map(e => e.ToAcademicTerm());

            if (view.ToLower().Equals("launchpad"))
            {
                var items = GetDefaultLaunchPadItems(view.ToLower(), toc);

                if (items != null && items.Count() > 0)
                {
                    ViewData["launchpadItemsIncluded"] = true;
                }
                else
                {
                    ViewData["launchpadItemsIncluded"] = false;
                }

                var launchPadSettings = GetInstructorConsoleLaunchPadSettings();
                return View("LaunchPad", launchPadSettings);
            }

            return View("Settings", course);
        }

        public JsonResult SetLaunchpadUnits(bool include, string toc = "syllabusfilter")
        {
            var currentContainerId = !include ? "Launchpad" : "LaunchPadRemoved";
            var newContainerId = include ? "Launchpad" : "LaunchPadRemoved";

            try
            {
                var items = GetDefaultLaunchPadItems(currentContainerId, toc);

                items.ForEach(i => {
                    i.SetContainer(newContainerId, toc);
                });

                ContentActions.StoreContents(items);

                return Json(new { status = "success" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// Batch Due Date Updated
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public ActionResult BatchDueDateUpdater()
        {
            return View();
        }

        /// <summary>
        /// Returns view for facet types in instructor console
        /// </summary>
        /// <returns></returns>
        /// [ChildActionOnly]
        [OutputCache(Duration = 3600, VaryByParam = "*", VaryByCustom = "product")]
        public ActionResult LoadFacetValues(string type, BizDC.InstructorConsoleSettings instructorConsoleSettings, string fieldName1, string fieldName2 = "", bool edit = false, string flag = "")
        {
            var route = "~/Views/InstructorConsoleWidget/FacetValues.ascx";
            if (edit)
            {
                ViewData["flag"] = flag;
                route = "~/Views/InstructorConsoleWidget/FacetValuesEdit.ascx";
            }

            List<FacetValueSetting> FacetValues = new List<FacetValueSetting>();
            var searchResults = GetFacetValues(fieldName1);

            if (searchResults.FacetFields.Count > 0)
            {
                FacetValues.AddRange(searchResults.FacetFields.First().FieldValues.Map(f => new FacetValueSetting(f)));
            }
            if (!fieldName2.IsNullOrEmpty())
            {
                searchResults = GetFacetValues(fieldName2);
                if (searchResults.FacetFields.Count > 0)
                {
                    FacetValues.AddRange(searchResults.FacetFields.First().FieldValues.Map(f => new FacetValueSetting(f)));
                }
            }

            if (edit && instructorConsoleSettings.Resources != null)
            {
                foreach (var resource in instructorConsoleSettings.Resources.Where(r => r.Type == type))
                {
                    FacetValueSetting facet = FacetValues.Where(f => f.Value.Trim().Equals(resource.Name)).FirstOrDefault();
                    if (facet != null)
                    {
                        facet.Selected = true;
                    }

                }
            }
            ViewData["AccessLevel"] = Context.AccessLevel;
            return View(route, FacetValues);
        }

        bool _doProductSearch = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["FaceplateSearchAgainstProductCourse"]);

        /// <summary>
        /// Returns view for facet types list
        /// </summary>
        /// <returns></returns>
        [OutputCache(Duration = 3600, VaryByParam = "*")]
        public ActionResult FacetValuesEdit(string type, IEnumerable<string> fieldNames, BizDC.InstructorConsoleSettings instructorConsoleSettings)
        {
            List<FacetValueSetting> FacetValues = new List<FacetValueSetting>();
            foreach (var fieldName in fieldNames)
            {
                var searchResults = GetFacetValues(fieldName);

                if (searchResults.FacetFields.Count > 0)
                {

                    FacetValues.AddRange(searchResults.FacetFields.First().FieldValues.Map(f => new FacetValueSetting(f)));
                }

            }
            foreach (var resource in instructorConsoleSettings.Resources.Where(r => r.Type == type))
            {
                FacetValueSetting facet = FacetValues.Where(f => f.Value == resource.Name).FirstOrDefault();
                if (facet != null)
                {
                    facet.Selected = true;
                }
            }

            ViewData["AccessLevel"] = Context.AccessLevel;


            return View(FacetValues);
        }

        /// <summary>
        /// Updates settings
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult UpdateSettings(Bfw.PX.PXPub.Models.InstructorConsoleSettings model)
        {
            ViewData["requireRefresh"] = "true";
            var course = CourseActions.GetCourseByCourseId(Context.CourseId);
            course.ConsoleSettings = new BizDC.InstructorConsoleSettings()
            {
                ShowGeneral = model.ShowGeneral,
                ShowLaunchPad = model.ShowLaunchPad,
                ShowNavigation = model.ShowNavigation,
                ShowChapters = model.ShowChapters,
                ShowMyResources = model.ShowMyResources,
                ShowEbook = model.ShowEbook,
                ShowTypes = model.ShowTypes,
                FacetsToShow = model.FacetsToShow
            };
            CourseActions.UpdateCourse(course);
            var req = HttpContext.Request;
            var widget = this.PageActions.GetWidget("PX_Instructor_Console_Widget");
            widget.InstructorConsoleSettings.Resources = new List<BizDC.InstructorConsoleSettings.SettingResource>(); 
            if (req != null)
            {
                if (req["rss"] != null)
                {
                    var rssFeeds = Context.Course.RSSCourseFeeds.ToDictionary(k => k.Name, v => v.Url);
                    string checkedRssItems = req["rss"];
                    string[] checkedRssItemsArr = checkedRssItems.Substring(1, checkedRssItems.Length - 2).Replace("|,|", "|").Split('|');
                    foreach (var rssItem in checkedRssItemsArr)
                    {
                        if (rssFeeds.ContainsKey(rssItem))
                        {
                            var rssFeed = rssFeeds[rssItem];
                            widget.InstructorConsoleSettings.Resources.Add(new BizDC.InstructorConsoleSettings.SettingResource(rssItem, rssFeed, "rss"));
                        }
                    }
                }
                if (req["topic"] != null)
                {
                    string topicChunk = req["topic"];
                    string[] topics = topicChunk.Substring(1, topicChunk.Length - 2).Replace("|,|", "|").Split('|');
                    foreach (var topic in topics)
                    {
                        widget.InstructorConsoleSettings.Resources.Add(new BizDC.InstructorConsoleSettings.SettingResource(topic, "topic", "topic"));
                    }
                }
                if (req["ebook"] != null)
                {
                    string ebookChunk = req["ebook"];
                    string[] topics = ebookChunk.Substring(1, ebookChunk.Length - 2).Replace("|,|", "|").Split('|');
                    foreach (var topic in topics)
                    {
                        widget.InstructorConsoleSettings.Resources.Add(new BizDC.InstructorConsoleSettings.SettingResource(topic, "ebook", "ebook"));
                    }
                }
                if (req["content-type"] != null)
                {
                    string contentTypeChunk = req["content-type"];
                    string[] topics = contentTypeChunk.Substring(1, contentTypeChunk.Length - 2).Replace("|,|", "|").Split('|');
                    foreach (var topic in topics)
                    {
                        widget.InstructorConsoleSettings.Resources.Add(new BizDC.InstructorConsoleSettings.SettingResource(topic, "content-type", "content-type"));
                    }
                }
            }
            widget.InstructorConsoleSettings.ShowChapters = model.ShowChapters;
            widget.InstructorConsoleSettings.ShowTypes = model.ShowTypes;
            widget.InstructorConsoleSettings.ShowEbook = model.ShowEbook;
            widget.InstructorConsoleSettings.ShowMyResources = model.ShowMyResources;
            widget.InstructorConsoleSettings.ShowGeneral = model.ShowGeneral;
            widget.InstructorConsoleSettings.ShowLaunchPad = model.ShowLaunchPad;
            widget.InstructorConsoleSettings.ShowNavigation = model.ShowNavigation;
            widget.InstructorConsoleSettings.ShowWelcomeReturn = model.ShowWelcomeReturn;
            widget.InstructorConsoleSettings.ShowBatchUpdater = model.ShowBatchUpdater;
            widget.InstructorConsoleSettings.ShowManageAnnouncemets = model.ShowManageAnnouncemets;

            this.PageActions.UpdateWidget(widget);

            return PartialView("View");
        }


        public ActionResult UpdateLaunchPadSettings(Bfw.PX.PXPub.Models.LaunchPadSettings model)
        {
            ViewData["requireRefresh"] = "true";

            var widget = GetInstructorConsoleLaunchPadSettings().ToDcItem();
            var settings = model;

            widget.ShowCollapseUnassigned = settings.ShowCollapseUnassigned;
            widget.CollapseUnassigned = settings.CollapseUnassigned;

            widget.CollapseDueLater = settings.CollapseDueLater;

            if (!settings.CollapseDueLater)
            {
                var cookie = new System.Web.HttpCookie(Context.EntityId + "hide_due_later");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            widget.CollapsePastDue = settings.CollapsePastDue;

            if (!settings.CollapsePastDue)
            {
                var cookie = new System.Web.HttpCookie(Context.EntityId + "hide_past_due");
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }

            widget.CollapseUnassignedItems = settings.CollapseUnassignedItems;
            widget.CollapseUnassigned = settings.CollapseUnassigned;
            
            if (settings.CollapseUnassigned)
            {
                settings.ShowItemsOnly = "assigned";
            }

            widget.DisableDragAndDrop = settings.DisableDragAndDrop;
            widget.DisableEditing = settings.DisableEditing;
            widget.DueLaterDays = settings.DueLaterDays;
            widget.GrayoutPastDueLater = settings.GrayoutPastDueLater;
            widget.CategoryName = settings.CategoryName;
            widget.ShowCollapsePastDue = settings.ShowCollapsePastDue;
            widget.ShowItemsOnly = settings.ShowItemsOnly;
            widget.SortByDueDate = settings.SortByDueDate;
            widget.SplitAssigned = settings.SplitAssigned;
            
            this.PageActions.UpdateWidget(widget);
            this.PageActions.EmptySettingsCache(widget.Id);
            
            return PartialView("View");
        }

        [ValidateInput(false)]
        public JsonResult UploadSyllabus()
        {
            List<Resource> resources = new List<Resource>();
            Resource r = new Resource();
            FileUploadResults resultMessage = new FileUploadResults();

            try
            {
                if (Request.Files[0] != null)
                {
                    string[] acceptedFileTypes = { ".doc", ".txt", ".pdf", ".docx" };
                    var uploadFile = Request.Files[0];

                    var filenameVal = System.IO.Path.GetFileName(uploadFile.FileName);
                    var contenttypeVal = uploadFile.ContentType;
                    var cleanCourseID = Context.CourseId.Split(',').FirstOrDefault();

                    var resourceStream = r.GetStream();
                    uploadFile.InputStream.CopyTo(resourceStream);
                    resourceStream.Flush();

                    r.EntityId = Context.CourseId;
                    r.ContentType = contenttypeVal;
                    r.Name = filenameVal;
                    r.Extension = Path.GetExtension(filenameVal);
                    bool isVaild = false;

                    foreach (string type in acceptedFileTypes)
                    {
                        if (r.Extension == type)
                        {
                            isVaild = true;
                        }
                    }

                    if (isVaild != true)
                    {
                        resultMessage.UploadMessage = "File uploaded failed : File type not accepted. Please upload this document in a doc, txt, and pdf file type.";
                        resultMessage.UploadPath = string.Empty;
                        resultMessage.UploadFileName = string.Empty;

                        return Json(new { resultMessage }, "text/plain");
                    }
                    else
                    {
                        r.Url = string.Format("{0}/{1}", cleanCourseID, filenameVal);
                        resources.Add(r);
                        ContentActions.StoreResources(resources);

                        resultMessage.UploadMessage = filenameVal + " was uploaded successfully.";
                        resultMessage.UploadFileName = filenameVal;
                        resultMessage.UploadPath = r.Url;
                    }
                }
                else
                {
                    resultMessage.UploadMessage = "No files found!";
                    resultMessage.UploadPath = string.Empty;
                    resultMessage.UploadFileName = string.Empty;
                }
            }
            catch (Exception ex)
            {
                resultMessage.UploadMessage = "File uploaded failed :" + ex.Message;
                resultMessage.UploadPath = string.Empty;
                resultMessage.UploadFileName = string.Empty;
            }

            return Json(new { resultMessage }, "text/plain");
        }

        /// <summary>
        /// Returns main view for Browse More Resources window
        /// </summary>
        /// <returns></returns>
        public ActionResult ResourcesEdit()
        {
            var searchSchema = Context.Course.FacetSearchSchema;
            //TODO: use search schema to determine resources dropdowns
            var widget = (this.PageActions.GetInstructorConsoleSettings("PX_Instructor_Console_Widget"));
            var RssFeeds = Context.Course.RSSCourseFeeds.ToDictionary(k => k.Name, v => v.Url);
            widget.FacetsToShow = RssFeeds;
            ViewData.Model = widget;
            ViewData["AccessLevel"] = Context.AccessLevel;
            return View();
        }

        /// <summary>
        /// Returns main view for Resources in instructor console
        /// </summary>
        /// <returns></returns>
        public ActionResult Resources()
        {
            var searchSchema = Context.Course.FacetSearchSchema;
            //TODO: use search schema to determine resources dropdowns
            var RssFeeds = Context.Course.RSSCourseFeeds.ToDictionary(k => k.Name, v => v.Url);

            ViewData["AccessLevel"] = Context.AccessLevel;
            return View(RssFeeds);
        }

        /// <summary>
        /// Updates the course settings 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        public ActionResult Update(Bfw.PX.PXPub.Models.Course model, FormCollection collection)
        {
            string courseId = string.Empty;
            courseId = Context.CourseId;
            if (!string.IsNullOrEmpty(model.Id) && !string.IsNullOrEmpty(Context.CourseId) && !model.Id.Equals(Context.CourseId))
            {
                courseId = model.Id;
            }
            ViewData["requireRefresh"] = "true";
            var course = CourseActions.GetCourseByCourseId(courseId: courseId);// Taking courseid different in case we have different course to update than original course loading the content
            switch (collection["View"].ToLower().ToString())
            {
                case "dashboard":
                case "general":
                    JavaScriptSerializer seializer = new JavaScriptSerializer();
                    var errorString = string.Empty;

                    if (!ModelState.IsValid)
                    {
                        var error = (from e in ModelState.Where(o => o.Value.Errors.Count > 0)
                                     select new { id = e.Key, messages = e.Value.Errors.Select(o => o.ErrorMessage).ToArray() }).ToArray();

                        errorString = seializer.Serialize(error);

                        if (errorString.Length > 0)
                        {
                            return Json(new { Error = errorString });
                        }
                    }

                    course.AcademicTerm = model.AcademicTerm;
                    course.InstructorName = model.CourseUserName;
                    course.Title = model.Title;
                    course.CourseNumber = model.CourseNumber;
                    course.SectionNumber = model.SectionNumber;
                    course.CourseTimeZone = model.CourseTimeZone;
                    course.OfficeHours = model.OfficeHours;
                    course.CourseProductName = model.CourseProductName;
                    course.LmsIdLabel = string.IsNullOrWhiteSpace(model.LmsIdLabel) ? _defaultLmsMessage : model.LmsIdLabel;
                    course.LmsIdPrompt = model.LmsIdPrompt;
                    course.LmsIdRequired = model.LmsIdRequired;

                    // updates contact details
                    var types = from s in collection.AllKeys
                                where s.StartsWith("contactInfoType_")
                                select s;

                    Regex rgx;
                    bool isValid = true;
                    List<object> contactErrors = new List<object>();

                    course.ContactInformation = new List<ContactInfo>();

                    if (types.Count() > 0)
                    {
                        foreach (var t in types)
                        {
                            var contactType = collection[t];
                            var contactValue = collection[string.Format("contactInfoValue_{0}", t.Replace("contactInfoType_", string.Empty))];

                            switch (contactType)
                            {
                                case "Phone":
                                case "Fax":
                                    rgx = new Regex(@"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$");
                                    isValid = rgx.IsMatch(contactValue);
                                    break;

                                case "Email":
                                    rgx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                                    isValid = rgx.IsMatch(contactValue);
                                    break;
                            }

                            if (!isValid)
                            {
                                contactErrors.Add(new { id = t.Replace("Type", "Value"), messages = "Please specify valid contact info" });
                            }

                            course.ContactInformation.Add(new ContactInfo() { ContactType = contactType, ContactValue = contactValue });
                        }
                    }

                    seializer = new JavaScriptSerializer();
                    errorString = seializer.Serialize(contactErrors);

                    if (contactErrors.Count() > 0)
                    {
                        return Json(new { Error = errorString });
                    }

                    // updates syllabus info
                    if (collection["SyllabusType"] == "Url")
                    {
                        course.SyllabusType = collection["SyllabusType"];

                        var url = collection["SyllabusURL"];
                        if (url.Trim().Length > 0)
                        {
                            url = url.StartsWith("http://") || url.StartsWith("https://") ? url : String.Format("http://{0}", url);

                            rgx = new Regex(@"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$");
                            isValid = rgx.IsMatch(url);

                            if (!isValid)
                            {
                                return Json(new { Error = new { id = "url", Message = "Please specify valid syllabus url" } });
                            }
                        }

                        course.Syllabus = url;
                    }
                    else
                    {
                        if (Boolean.Parse(collection["isUploadValid"]))
                        {
                            var syllabusType = collection["SyllabusType"];
                            var syllabus = collection["RefUrlFilePath"];

                            if (syllabus.Trim().Length > 0)
                            {
                                course.SyllabusType = syllabusType;
                                course.Syllabus = syllabus;
                            }
                        }
                    }

                    break;

                case "navigation":
                    course.IsLoadStartOnInit = collection["isLoadStartOnInit"] == "on" ? true : false;

                    //if (!course.IsLoadStartOnInit)
                    //{
                    //    var showWelcomeReturn = false;

                    //    if (collection["isLoadStartOnInit"] != null)
                    //    {
                    //        showWelcomeReturn = true;
                    //    }

                    //    var widget = (this.PageActions.GetInstructorConsoleSettings("PX_Instructor_Console_Widget"));
                    //    widget.ShowWelcomeReturn = showWelcomeReturn;
                    //    this.PageActions.UpdateWidget(widget);
                    //}

                    break;
            }

            course = this.CourseActions.UpdateCourse(course);
            
            /* Update cache during Dashboard Updates */
            var productCourseId = Context.CourseIsProductCourse ? Context.CourseId : Context.ProductCourseId;
            Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username, productCourseId);
            Context.CacheProvider.InvalidateUserEnrollmentList(Context.CurrentUser.Username);
            Context.CacheProvider.InvalidateCourseList(Context.CurrentUser.Username);
            
            return null;
        }

        /// <summary>
        /// default
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            ViewData["ViewType"] = Context.AccessLevel;
            return View();
        }

        /// <summary>
        /// Main view 
        /// </summary>
        /// <returns></returns>
        public PartialViewResult View(Models.Widget model)
        {
            ViewData["ViewType"] = Context.AccessLevel;
            ViewData["IsSandboxCourse"] = Context.Course.IsSandboxCourse;
            ViewData["IsLoadStartOnInit"] = Context.Course.IsLoadStartOnInit;

            return PartialView();
        }

        public PartialViewResult GradebookPreferences()
        {
            var model = Context.Course.ToCourse();

            return PartialView(model);
        }

        public PartialViewResult GradebookCategoriesList(GradebookPreferences model)
        {
            if (model.GradeBookWeights == null)
            {
                model.UseWeightedCategories = Context.Course.UseWeightedCategories;
                model.GradeBookWeights = GradeActions.GetGradeBookWeights(Context.CourseId).ToGradeBookWeights();

                if (Context.Course.GradeBookCategoryList != null)
                {
                    model.GradeBookWeights.GradeWeightCategories.ForEach(c =>
                    {
                        var cat = Context.Course.GradeBookCategoryList.FirstOrDefault(o => o.Id.Equals(c.Id));
                        if (cat != null)
                        {
                            c.DropLowest = cat.DropLowest;
                            c.Sequence = cat.Sequence;
                        }
                    });
                }

                var itemList = new List<Bfw.PX.PXPub.Models.ContentItem>();
                model.GradeBookWeights.GradeWeightCategories.ForEach(o =>
                {
                    itemList.AddRange(o.Items);
                });

                var items = ContentActions.GetItems(Context.CourseId, (from s in itemList select s.Id).ToList()).ToList();

                model.GradeBookWeights.GradeWeightCategories.ForEach(o =>
                {
                    o.Items.ForEach(i =>
                    {
                        var item = items.Find(s => s.Id.Equals(i.Id));
                        i.CategorySequence = item.AssignmentSettings.CategorySequence;
                        i.Type = item.Subtype;
                        i.DueDate = item.AssignmentSettings.DueDate;
                    });

                    o.Items = o.Items.
                        Where(l => !l.DueDate.ToShortDateString().Equals(DateTime.MinValue.ToShortDateString())).
                        Where(l => !l.Type.ToLower().Equals("pxunit")).
                        OrderBy(l => l.CategorySequence).ThenBy(l => l.Title).ToList();
                });
            }

            return PartialView(model);
        }

        public ActionResult MoveGradebookCategory(GradebookPreferencesChangeState state)
        {
            var newSequence = Context.Sequence(state.AboveSequence, state.BelowSequence);

            var gbbWeights = GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();
            var gbbCategoryItem = gbbWeights.GradeWeightCategories.FirstOrDefault(o => o.Id.Equals(state.EntityId));

            if (gbbCategoryItem != null)
            {
                gbbCategoryItem.Sequence = newSequence;

                UpdateGradebookCategories(state.EntityId, gbbWeights, false, true);
            }

            return RedirectToAction("GradebookCategoriesList");
        }

        public ActionResult MoveGradebookItem(GradebookPreferencesChangeState state)
        {
            var newSequence = Context.Sequence(state.AboveSequence, state.BelowSequence);

            var list = new List<string>();
            list.Add(state.EntityId);

            var item = ContentActions.GetItems(Context.CourseId, list).FirstOrDefault();

            if (!state.ItemIdList.IsNullOrEmpty())
            {
                var changed = new List<BizDC.ContentItem>();
                var resortedItems = ResortCategory(state.ItemIdList, out changed);

                if (changed.Count > 0)
                {
                    ContentActions.StoreContents(changed);

                    if (state.AboveId.IsNullOrEmpty())
                    {
                        state.AboveSequence = string.Empty;
                    }
                    else
                    {
                        state.AboveSequence = resortedItems.Find(o => o.Id.Equals(state.AboveId)).CategorySequence;
                    }

                    if (state.BelowId.IsNullOrEmpty())
                    {
                        state.BelowSequence = string.Empty;
                    }
                    else
                    {
                        state.BelowSequence = resortedItems.Find(o => o.Id.Equals(state.BelowId)).CategorySequence;
                    }

                    newSequence = Context.Sequence(state.AboveSequence, state.BelowSequence);
                }
            }            

            if (item != null)
            {
                item.AssignmentSettings.CategorySequence = newSequence;

                ContentActions.StoreContent(item);
            }

            return RedirectToAction("GradebookCategoriesList");
        }

        public ActionResult RemoveGradebookCategory(GradebookPreferencesChangeState state)
        {
            var gbbWeights = GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();
            var gbbCategoryItem = gbbWeights.GradeWeightCategories.FirstOrDefault(o => o.Id.Equals(state.EntityId));

            if (gbbCategoryItem != null)
            {
                string uncategorizedId = string.Empty;
                var uncategorized = gbbWeights.GradeWeightCategories.FirstOrDefault(o => o.Id.Equals("0"));

                if (uncategorized == null)
                {
                    AssignmentCenterHelper helper = new AssignmentCenterHelper(ContentActions, Context, null, null, CourseActions, GradeActions, null, null);
                    uncategorizedId = helper.AddGradeBookCategoryToCourse(newCategory: "Uncategorized");
                }
                else
                {
                    uncategorizedId = uncategorized.Id;
                }

                var idList = new List<string>();
                gbbCategoryItem.Items.ForEach(c => idList.Add(c.Id));
                var items = ContentActions.GetItems(Context.CourseId, idList).ToList();

                items.ForEach(o => o.AssignmentSettings.Category = uncategorizedId);

                var ciList = items.Map(o => o.ToAssignedItem()).Map(i => i.ToContentItem(Context.CourseId, ContentActions)).ToList();

                if (ciList.Count > 0)
                {
                    ContentActions.StoreContents(ciList);
                }

                gbbWeights.GradeWeightCategories = (from s in gbbWeights.GradeWeightCategories
                                                    where s.Id != state.EntityId
                                                    select s).ToList();

                UpdateGradebookCategories(state.EntityId, gbbWeights, false, false);
            }

            return RedirectToAction("GradebookCategoriesList");
        }

        public JsonResult UpdatePassingScore(string passingScore)
        {
            try
            {
                var course = Context.Course;
                course.PassingScore = Double.Parse(passingScore);

                CourseActions.UpdateCourse(course);
            }
            catch
            {
                return Json(new { status = "fail", message = "Passing Score update failed!" });
            }

            return Json(new { status = "success", message = "" });
        }

        public JsonResult RenameGradebookCategory(string categoryId, string newValue)
        {
            try
            {
                var gbbWeights = GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();
                var gbbCategoryItem = gbbWeights.GradeWeightCategories.FirstOrDefault(o => o.Id.Equals(categoryId));

                if (gbbCategoryItem != null)
                {
                    gbbCategoryItem.Text = newValue;

                    UpdateGradebookCategories(categoryId, gbbWeights, false, false);
                }
                else
                {
                    return Json(new { status = "fail", message = "Category was not found!" });
                }
            }
            catch
            {
                return Json(new { status = "fail", message = "Renaming category failed!" });
            }

            return Json(new { status = "success", message = "" });
        }

        /// <summary>
        /// Update a category weight in gradebook
        /// </summary>
        /// <param name="state">state.EntityId = categoryid, state.NewValue = new weight</param>
        /// <returns></returns>
        public ActionResult UpdateGradebookCategoryWeight(GradebookPreferencesChangeState state)
        {
            var gbbWeights = GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();
            var gbbCategoryItem = gbbWeights.GradeWeightCategories.FirstOrDefault(o => o.Id.Equals(state.EntityId));

            if (gbbCategoryItem != null)
            {
                gbbCategoryItem.Weight = state.NewValue;

                UpdateGradebookCategories(state.EntityId, gbbWeights, false, false);
            }

            return RedirectToAction("GradebookCategoriesList");
        }

        public JsonResult UpdateGradebookCategoryDropLowest(string categoryId, string newValue)
        {
            try
            {
                var gbbWeights = GradeActions.GetGradeBookWeights(Context.EntityId).ToGradeBookWeights();
                var gbbCategoryItem = gbbWeights.GradeWeightCategories.FirstOrDefault(o => o.Id.Equals(categoryId));

                if (gbbCategoryItem != null)
                {
                    gbbCategoryItem.DropLowest = newValue;

                    UpdateGradebookCategories(categoryId, gbbWeights, true, false);
                }
                else
                {
                    return Json(new { status = "fail", message = "Category was not found!" });
                }
            }
            catch
            {
                return Json(new { status = "fail", message = "Updating category drop lowest failed!" });
            }

            return Json(new { status = "success", message = "" });
        }

        /// <summary>
        /// Exports & downloads content of gradebook into a csv file
        /// </summary>
        /// <returns></returns>
        public FileResult ExportGradebook()
        {
            FileStreamResult result = null;            

            var enrollments = EnrollmentActions.GetEntityEnrollmentsWithGrades(Context.EntityId);
            var csv = GradebookExportHelper.GetCsvString(enrollments);

            if (!csv.IsNullOrEmpty())
            {
                MemoryStream ms = new MemoryStream();
                ms.Write(System.Text.Encoding.Unicode.GetBytes(csv), 0, System.Text.Encoding.Unicode.GetBytes(csv).Length);
                ms.Position = 0;

                result = new FileStreamResult(ms, "application/csv") { FileDownloadName = string.Format("gradebook_{0}.csv", Context.EntityId) };
            }

            return result;
        }

        /// <summary>
        /// Identifies repeated sequence within category and fixes order 
        /// </summary>
        /// <param name="itemIdList">List of item ids within the category</param>
        /// <param name="changed">List of items with fixed category sequence to be stored</param>
        /// <returns>Sorted by category sequence list of items for the category</returns>
        private List<Bfw.PX.PXPub.Models.ContentItem> ResortCategory(string[] itemIdList, out List<BizDC.ContentItem> changed)
        {
            var result = new List<Bfw.PX.PXPub.Models.ContentItem>();
            changed = new List<BizDC.ContentItem>();

            if (itemIdList.Length > 0)
            {
                var catItems = ContentActions.GetItems(Context.CourseId, itemIdList.ToList()).ToList();
                var changes = new List<BizDC.ContentItem>();

                catItems.ForEach(i =>
                {
                    var q = from s in catItems
                            where s.AssignmentSettings.CategorySequence.Equals(i.AssignmentSettings.CategorySequence) && !s.Id.Equals(i.Id)
                            select s;

                    if (q.Count() > 0)
                    {
                        var aboveSequence = i.AssignmentSettings.CategorySequence;
                        var belowSequence = string.Empty;

                        var lastIndex = catItems.FindLastIndex(p => p.Id.Equals(i.Id));

                        if (lastIndex > -1)
                        {
                            if (catItems.Count >= lastIndex + 1)
                            {
                                var nextItem = catItems[lastIndex + 1];
                                belowSequence = nextItem.AssignmentSettings.CategorySequence;
                            }
                        }

                        foreach (var item in q)
                        {
                            item.AssignmentSettings.CategorySequence = Context.Sequence(aboveSequence, belowSequence == null ? string.Empty : belowSequence);
                            aboveSequence = item.AssignmentSettings.CategorySequence;

                            changes.Add(item);
                        }
                    }
                });

                changed = changes;              
                result = catItems.Map(o => o.ToContentItem(ContentActions)).ToList();
            }

            return result;
        }

        private void UpdateGradebookCategories(string categoryId, Bfw.PX.PXPub.Models.GradeBookWeights gbbWeights, bool isDropLowestUpdate, bool isSequenceUpdate)
        {
            if (isDropLowestUpdate)
            {
                gbbWeights.GradeWeightCategories.Where(i => !i.Id.Equals(categoryId)).ToList().ForEach(c => c.DropLowest = Context.Course.GradeBookCategoryList.Find(o => o.Id.Equals(c.Id)).DropLowest);
            }
            else
            {
                gbbWeights.GradeWeightCategories.ForEach(c => c.DropLowest = Context.Course.GradeBookCategoryList.Find(o => o.Id.Equals(c.Id)).DropLowest);
            }

            if (isSequenceUpdate)
            {
                gbbWeights.GradeWeightCategories.Where(i => !i.Id.Equals(categoryId)).ToList().ForEach(c => c.Sequence = Context.Course.GradeBookCategoryList.Find(o => o.Id.Equals(c.Id)).Sequence);
            }
            else
            {
                gbbWeights.GradeWeightCategories.ForEach(c => c.Sequence = Context.Course.GradeBookCategoryList.Find(o => o.Id.Equals(c.Id)).Sequence);
            }

            List<Bfw.PX.PXPub.Models.GradeBookWeightCategory> listCategories = gbbWeights.GradeWeightCategories;
            Context.Course.GradeBookCategoryList = new List<Biz.DataContracts.GradeBookWeightCategory>();

            foreach (var gradeBookWeightCategory in listCategories)
            {
                Context.Course.GradeBookCategoryList.Add(new BizDC.GradeBookWeightCategory()
                {
                    Id = gradeBookWeightCategory.Id,
                    Text = gradeBookWeightCategory.Text,
                    Weight = (gradeBookWeightCategory.Weight == null) ? "0" : gradeBookWeightCategory.Weight,
                    DropLowest = (gradeBookWeightCategory.DropLowest == null) ? "0" : gradeBookWeightCategory.DropLowest,
                    Sequence = (gradeBookWeightCategory.Sequence == null) ? "0" : gradeBookWeightCategory.Sequence
                });
            }

            var course = Context.Course;

            CourseActions.UpdateCourse(course);
        }

        internal List<BizDC.ContentItem> GetDefaultLaunchPadItems(string containerId, string toc = "syllabusfilter")
        {
            //Make sure we get subcontainers from old data schema and new (old == no value new == pxmultipartleson)
            var unitList = ContentActions.GetContainerItems(Context.EntityId, containerId, string.Empty, toc).ToList();
            unitList.AddRange(ContentActions.GetContainerItems(Context.EntityId, containerId, "PX_MULTIPART_LESSONS", toc));

            var items = (from c in unitList
                         where c.Categories.Count(o => o.Id.Equals(System.Configuration.ConfigurationManager.AppSettings["MyMaterials"])) == 0
                         select c).ToList();

            return items;
        }

        public ActionResult UpdateUseWeightedCategories(GradebookPreferencesChangeState state)
        {
            var course = Context.Course;

            course.UseWeightedCategories = state.UseWeighted;
            CourseActions.UpdateCourse(course);

            return RedirectToAction("GradebookCategoriesList");
        }

        public JsonResult GetBatchDueDateItemCount(string fromDate, string toDate)
        {
            var displayFromDate = DateTime.MinValue;
            var displayToDate = DateTime.MinValue;
            var allItems = GetItemsToUpdate(fromDate, toDate, false, ref displayFromDate, ref displayToDate);

            return Json(new { itemCount = allItems.Count(o => !o.Subtype.Equals("PxUnit")), fromDate = displayFromDate.ToString("D", CultureInfo.CreateSpecificCulture("en-US")), toDate = displayToDate.AddDays(-1).ToString("D", CultureInfo.CreateSpecificCulture("en-US")) });
        }

        public JsonResult BatchDueDateUpdate(string fromDate, string toDate, bool updateRestrictedDates, DateTime newDate, string toc = "syllabusfilter")
        {
            var displayFromDate = DateTime.MinValue;
            var displayToDate = DateTime.MinValue;
            var allItems = GetItemsToUpdate(fromDate, toDate, updateRestrictedDates, ref displayFromDate, ref displayToDate);
            var days = newDate.Subtract(displayFromDate).Days;

            if (allItems.Count() > 0)
            {
                // update due dates
                foreach (BizDC.ContentItem ci in allItems.Where(o => !o.Subtype.Equals("PxUnit") &&
                    !o.AssignmentSettings.DueDate.Year.Equals(DateTime.MinValue.Year)))
                {
                    ci.AssignmentSettings.DueDate = ci.AssignmentSettings.DueDate.AddDays(days);
                    AssignmentCenterHelper.ProcessAssignment(ci.Id, ci.ToAssignment(ContentActions).ToAssignmentCenterItem(), false, toc, true);
                }

                // update restricted by date if required
                if (updateRestrictedDates)
                {
                    foreach (BizDC.ContentItem ci in allItems.Where(o => o.ToContentItem(ContentActions).RestrictedByDate()))
                    {
                        var item = ci.ToContentItem(ContentActions);
                        DateTime date = DateTime.Parse(item.RestrictedDate());

                        if (date.InRange(displayFromDate, displayToDate))
                        {
                            ci.SetVisibility("restrictedbydate", date.AddDays(days));
                            ContentActions.StoreContent(ci);
                        }
                    }
                }

                return Json(new { status = "success", message = "Batch due date updated successfully." });
            }
            else
            {
                return Json(new { status = "fail", message = "There is no assignment to update." });
            }
        }

        private IEnumerable<BizDC.ContentItem> GetItemsToUpdate(string fromDate, string toDate, bool updateRestrictedDates, ref DateTime displayFromDate, ref DateTime displayToDate)
        {
            var fromArr = fromDate.Split('/');
            var toArr = toDate.Split('/');

            displayFromDate = new DateTime(Convert.ToInt32(fromArr[2]), Convert.ToInt32(fromArr[0]), Convert.ToInt32(fromArr[1]));
            displayToDate = new DateTime(Convert.ToInt32(toArr[2]), Convert.ToInt32(toArr[0]), Convert.ToInt32(toArr[1]), 23, 59, 0).AddDays(1);

            fromDate = displayFromDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ");
            toDate = displayToDate.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ssZ");

            if (updateRestrictedDates)
            {
                return ContentActions.FindContentItems(new Agilix.DataContracts.ItemSearch()
                    {
                        EntityId = Context.EntityId,
                        Query = "/bfw_properties/bfw_property@name='bfw_visibility' and /meta-containers/meta-container='Launchpad' and /bfw_type<>'PxUnit'"
                    });
            }
            else
            {
                return ContentActions.ListContentWithDueDatesWithinRange(Context.EntityId, fromDate, toDate).ToList();
            }
        }

        #region Implementation

        /// <summary>
        /// Gets values for a specific faceted field
        /// </summary>
        /// <param name="facetName"></param>
        /// <returns></returns>
        private FacetedSearchResults GetFacetValues(string facetName)
        {

            //get search controller instance
            SearchController searchController =
            ServiceLocator.Current.GetInstance(typeof(SearchController)) as SearchController;
            //do a faceted search for units(chapters)
            Bfw.PX.PXPub.Models.SearchQuery query = new Bfw.PX.PXPub.Models.SearchQuery()
            {
                IsFaceted = true,
                FacetedQuery = new Bfw.PX.PXPub.Models.FacetedSearchQuery
                {
                    Fields =
                                {
                                    facetName
                                },
                    Limit = -1,
                    MinCount = 1,
                }
            };

            if (_doProductSearch)
                query.EntityId = Context.Course.ProductCourseId;

            FacetedSearchResults searchResults = searchController.DoFacetedSearch(query);


            return searchResults;
        }

        /// <summary>
        /// Creates and returns an object representing the settings for Instructor Console
        /// </summary>
        /// <returns></returns>
        private Bfw.PX.PXPub.Models.LaunchPadSettings GetInstructorConsoleLaunchPadSettings()
        {
            var settingsDC = this.PageActions.GetInstructorConsoleLaunchPadSettings();
            var model = new Models.LaunchPadSettings();
            model.ToModelItem(settingsDC);
            return model;
        }

        #endregion Implementation
    }
}
