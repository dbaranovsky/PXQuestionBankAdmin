using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Bfw.PX.Abstractions;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;
using Microsoft.Practices.ServiceLocation;
using Widget = Bfw.PX.PXPub.Models.Widget;

namespace Bfw.PX.PXPub.Controllers.Widgets
{
    [PerfTraceFilter]
    public class AboutCourseWidgetController : Controller, IPXWidget
    {

       
        /// <summary>
        /// Implementing interfaces to utilize functionality
        /// </summary>
        protected IPageActions PageActions { get; set; }
        protected IBusinessContext Context { get; set; }
        protected ContentHelper ContentHelper { get; set; }
        protected IContentActions ContentActions { get; set; }
        public ICourseActions CourseActions { get; set; }
        public IUserActions UserActions { get; set; }
        protected IUrlHelperWrapper UrlHelper { get; set; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context">Conext interface</param>
        /// <param name="contentActions">Content Actions Interface</param>
        /// <param name="pageActions">Page Actions Interface</param>
        /// <param name="helper">Content Helper Interface</param>
        public AboutCourseWidgetController(IBusinessContext context, IContentActions contentActions,
                                           IPageActions pageActions, ContentHelper helper, 
                                           ICourseActions courseActions, IUserActions userActions, IUrlHelperWrapper urlHelperWrapper)
        {
            Context = context;
            ContentHelper = helper;
            PageActions = pageActions;
            ContentActions = contentActions;
            CourseActions = courseActions;
            UserActions = userActions;
            UrlHelper = urlHelperWrapper;
        }
        
        /// <summary>
        /// Don't know why this is required
        /// </summary>
        /// <returns>View("Index")</returns>
        public ActionResult Summary(Models.Widget widget)
        {
            return View("Index");
        }

        /// <summary>
        /// Don't know why this is required.
        /// </summary>
        /// <returns>View("Index")</returns>
        public ActionResult ViewAll(Models.Widget model)
        {
            return View("Index");
        }

        /// <summary>
        /// Don't know why this is required.
        /// </summary>
        /// <param name="model">Bfw.PX.PXPub.Models.Widget</param>
        /// <returns>PartialView()</returns>
        public ActionResult Edit( Widget model ) {
            return PartialView();
        }

        /// <summary>
        /// I suppose this is the default view of a widget in the none edit state
        /// </summary>
        /// <param name="model">Bfw.PX.PXPub.Models.Widget</param>
        /// <returns>PartialView(model)</returns>
        public ActionResult Index( Widget model ) {
            return PartialView( model );
        }

        /// <summary>
        /// Loads update/create widget screen if the person is an instructor.
        /// There is no way to check to see if the widget already exists besides checking the values
        /// We check to see if the instructor name is not empty we then assume that there is already a version
        /// so we load the general view. If the user wants to edit it they must click the widget to enter edit mode
        /// if they have the proper creds
        /// </summary>
        /// <param name="model">Bfw.PX.PXPub.Models.Widget</param>
        /// <returns>PartialView("Edit", aboutCourse)</returns>
        public ActionResult OnBeforeAdd(Widget model)
        {
            var course = this.CourseActions.GetCourseByCourseId(Context.CourseId);
            AboutCourse aboutCourse = new AboutCourse();

            aboutCourse.CourseTitle = course.Title;
            aboutCourse.InstructorName = course.InstructorName;
            aboutCourse.OfficeHours = course.OfficeHours;
            aboutCourse.SyllabusLinkType = course.SyllabusType == "Url" ? AboutCourseLinkType.Url : AboutCourseLinkType.File;
            aboutCourse.SyllabusUrl = course.SyllabusUrl;
            aboutCourse.SyllabusFileName = course.SyllabusFileName;

            aboutCourse.ContactInfo = (from c in course.ContactInformation
                                       select new AboutCourseContactInfo() { Type = c.ContactType, Info = c.ContactValue }).ToList();

            if (model.Properties.ContainsKey("show_course_title"))
            {
                aboutCourse.ShowCourseTitle = Convert.ToBoolean(model.Properties["show_course_title"].Value.ToString());
            }

            if (model.Properties.ContainsKey("show_instructor_name"))
            {
                aboutCourse.ShowInstructorName = Convert.ToBoolean(model.Properties["show_instructor_name"].Value.ToString());
            }

            if (model.Properties.ContainsKey("show_office_hours"))
            {
                aboutCourse.ShowOfficeHours = Convert.ToBoolean(model.Properties["show_office_hours"].Value.ToString());
            }

            if (model.Properties.ContainsKey("show_email"))
            {
                aboutCourse.ShowEmail = Convert.ToBoolean(model.Properties["show_email"].Value.ToString());
            }

            if (model.Properties.ContainsKey("show_phone_number"))
            {
                aboutCourse.ShowPhoneNumber = Convert.ToBoolean(model.Properties["show_phone_number"].Value.ToString());
            }

            if (model.Properties.ContainsKey("show_download_syllabus"))
            {
                aboutCourse.ShowSyllabusUrl = Convert.ToBoolean(model.Properties["show_download_syllabus"].Value.ToString());
            }

            if (model.Properties.ContainsKey("show_widget_editlink"))
            {
                aboutCourse.ShowEditLink = Convert.ToBoolean(model.Properties["show_widget_editlink"].Value.ToString());
            }

            if (Context.AccessLevel != AccessLevel.Instructor)
            {
                return PartialView("Index", model);
            }

            return PartialView("Edit", aboutCourse);
        }

        /// <summary>
        /// Saves Widget intance
        /// </summary>
        /// <param name="pageName">Name of the target page</param>
        /// <param name="parentId">Parent ID</param>
        /// <param name="widgetZoneId">Targeted zone</param>
        /// <param name="widgetTemplateId">Name of template ID</param>
        /// <param name="widgetId">New GUID for widget</param>
        /// <param name="prevSeq">Previous Sequence</param>
        /// <param name="nextSeq">Next Sequence</param>
        /// <param name="instructorName">Instructor Name</param>
        /// <param name="officeHours">Office Hours</param>
        /// <param name="syllabusUrl">Syllabus URI</param>
        /// <param name="RefUrlFilePath">File path for uploaded file from hidden field</param>
        /// <param name="contactInfo">AboutCourseContactInfo Array</param>
        /// <returns>Widget with GUID</returns>
        public ActionResult Save(string pageName,
                                 string parentId,
                                 string widgetZoneId,
                                 string widgetTemplateId,
                                 string widgetId,
                                 string prevSeq,
                                 string nextSeq,
                                 string coursename,
                                 string instructorName,
                                 string officeHours,
                                 string syllabusUrl,
                                 string RefUrlFilePath,
                                 string RefUrlFileName,
                                 string aboutCourseLinkType,
                                 AboutCourseContactInfo[] contactInfo)
        {
            var course = CourseActions.GetCourseByCourseId(Context.CourseId);

            if (!(syllabusUrl.IndexOf("http://") > -1) && !(syllabusUrl.IndexOf("https://") > -1))
            {
                syllabusUrl = "http://" + syllabusUrl;
            }

            if ( !string.IsNullOrEmpty( instructorName ) || !string.IsNullOrEmpty( officeHours ) )
            {
                Dictionary<string, PropertyValue> properties = new Dictionary<string, PropertyValue>();

                if (!string.IsNullOrEmpty(instructorName))
                {
                    course.InstructorName = instructorName;
                }

                if (!string.IsNullOrEmpty(officeHours))
                {
                    course.OfficeHours = officeHours;
                }

                if (!string.IsNullOrEmpty(coursename))
                {
                    course.Title = coursename;
                }

                string result;
                string mode = string.Empty;
                string errorMsg = string.Empty;
                
                if (aboutCourseLinkType == AboutCourseLinkType.Url.ToString() && !string.IsNullOrEmpty(syllabusUrl))
                {
                    course.SyllabusType = AboutCourseLinkType.Url.ToString();
                    course.Syllabus = syllabusUrl;
                }
                else if (aboutCourseLinkType == AboutCourseLinkType.File.ToString() && !string.IsNullOrEmpty(RefUrlFilePath))
                {
                    course.SyllabusType = AboutCourseLinkType.File.ToString();
                    course.Syllabus = RefUrlFilePath;
                }

                // add a sets of Contact Info
                course.ContactInformation = new List<ContactInfo>();
                foreach (var c in contactInfo)
                {
                    if (c.Info != null)
                    {
                        course.ContactInformation.Add(new ContactInfo() { ContactType = c.Type, ContactValue = c.Info });
                    }
                }

                CourseActions.UpdateCourse(course);

                if (widgetId.IsNullOrEmpty())
                {
                    try
                    {
                        mode = "ADD";
                        var aboutCourseWidget = PageActions.AddWidget(pageName, widgetZoneId,
                                                                      widgetTemplateId,
                                                                      prevSeq,
                                                                      nextSeq, "",
                                                                      properties);
                        if (aboutCourseWidget != null)
                        {
                            result = "Success";
                            widgetId = aboutCourseWidget.Id;
                        }
                        else
                        {
                            result = "Fail";
                        }
                    }
                    catch (Exception ex)
                    {
                        result = "Failed with exception.";
                        errorMsg = ex.Message;
                    }
                }
                else
                {
                    try
                    {
                        result = "Success";
                        mode = "EDIT";
                        PageActions.UpdateWidget(pageName, widgetId, null, properties);
                    }
                    catch (Exception ex)
                    {
                        result = "Update failed with exception.";
                        errorMsg = ex.Message;
                    }
                }

                AboutCourse aboutCourse = new AboutCourse();

                if (!string.IsNullOrEmpty(instructorName))
                {
                    aboutCourse.InstructorName = instructorName;
                }
                if (!string.IsNullOrEmpty(officeHours))
                {
                    aboutCourse.OfficeHours = officeHours;
                }
                if (!string.IsNullOrEmpty(syllabusUrl))
                {
                    aboutCourse.SyllabusUrl = syllabusUrl;
                }
                if (!string.IsNullOrEmpty(RefUrlFilePath))
                {
                    aboutCourse.SyllabusUrl = RefUrlFilePath;
                }

                if (contactInfo.Count() > 0)
                {
                    foreach (var info in contactInfo)
                    {
                        aboutCourse.ContactInfo.Add(info);
                    }
                }

                return
                    Json(
                        new
                            {
                                Result = result,
                                Mode = mode,
                                OldWidgetID = widgetId,
                                ErrorMes = errorMsg,
                                ZoneId = widgetZoneId,
                                WidgetTemplateID = widgetTemplateId,
                                WidgetId = widgetId
                            });
            }
            return Json( new
            {
                Result = "Fail",
                Mode = "EDIT",
                OldWidgetID = widgetId,
                ErrorMes = "empty form",
                ZoneId = widgetZoneId,
                WidgetTemplateID = widgetTemplateId,
                WidgetId = widgetId
            } );
        }

        /// <summary>
        /// Refreshes the widget from instructor console
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RefreshWidget(string id)
        {
            return RedirectToAction("View", new { id = id });
        }

        /// <summary>
        /// Bind data in the home zone (general view)
        /// </summary>
        /// <returns>PartialView("Index", aboutCourse)</returns>
        public ActionResult View(Widget model)
        {
            var newWidget = Request.QueryString["id"] != null ? PageActions.GetWidget(Request.QueryString["id"]) : PageActions.GetWidget(model.Id);
            var course = this.CourseActions.GetCourseByCourseId(Context.CourseId);
            AboutCourse aboutCourse = new AboutCourse();

            List<AboutCourseContactInfo> list = new List<AboutCourseContactInfo>();

            aboutCourse.CourseTitle = course.Title;
            aboutCourse.CourseNumber = course.CourseNumber;
            aboutCourse.SectionNumber = course.SectionNumber;
            aboutCourse.InstructorName = course.InstructorName;
            aboutCourse.OfficeHours = course.OfficeHours;
            aboutCourse.ContactInfo = (from c in course.ContactInformation
                                      select new AboutCourseContactInfo() { Type = c.ContactType, Info = c.ContactValue }).ToList();

            aboutCourse.CourseType = Context.Course.CourseType;

            if (newWidget.Properties.ContainsKey("show_course_title"))
            {
                aboutCourse.ShowCourseTitle = Convert.ToBoolean(newWidget.Properties["show_course_title"].Value.ToString());
            }

            if (newWidget.Properties.ContainsKey("show_instructor_name"))
            {
                aboutCourse.ShowInstructorName = Convert.ToBoolean(newWidget.Properties["show_instructor_name"].Value.ToString());
            }

            if (newWidget.Properties.ContainsKey("show_office_hours"))
            {
                aboutCourse.ShowOfficeHours = Convert.ToBoolean(newWidget.Properties["show_office_hours"].Value.ToString());
            }

            if (newWidget.Properties.ContainsKey("show_email"))
            {
                aboutCourse.ShowEmail = Convert.ToBoolean(newWidget.Properties["show_email"].Value.ToString());
            }

            if (newWidget.Properties.ContainsKey("show_phone_number"))
            {
                aboutCourse.ShowPhoneNumber = Convert.ToBoolean(newWidget.Properties["show_phone_number"].Value.ToString());
            }

            if (newWidget.Properties.ContainsKey("show_download_syllabus"))
            {
                aboutCourse.ShowSyllabusUrl = Convert.ToBoolean(newWidget.Properties["show_download_syllabus"].Value.ToString());
            }

            if (newWidget.Properties.ContainsKey("show_widget_editlink"))
            {
                aboutCourse.ShowEditLink = Convert.ToBoolean(newWidget.Properties["show_widget_editlink"].Value.ToString());
            }

            if (course.SyllabusType == "File")
            {
                aboutCourse.SyllabusDisplayText = "Download Syllabus";
                aboutCourse.SyllabusUrl = course.Syllabus;
            }
            else
            {
                aboutCourse.SyllabusDisplayText = "View Syllabus";
                aboutCourse.SyllabusUrl = course.SyllabusUrl;
            }

            if (aboutCourse.SyllabusUrl.Replace("http://", string.Empty).Replace("https://", string.Empty).Length == 0)
            {
                aboutCourse.ShowSyllabusUrl = false;
            }
            else
            {
                aboutCourse.SyllabusUrl = aboutCourse.SyllabusUrl.IsNullOrEmpty() ? aboutCourse.SyllabusUrl : (IsUrl(aboutCourse.SyllabusUrl) ? aboutCourse.SyllabusUrl : UrlHelper.Action("DownloadEnrollmentFile", "Download", new { resourcePath = aboutCourse.SyllabusUrl, entityId = Context.CourseId }));
            }

            
            aboutCourse.AccessLevel = Context.AccessLevel;
            aboutCourse.CampusLmsId = Context.CurrentUser.ReferenceId;
            aboutCourse.LmsIdEnabled = Context.Course.LmsIdRequired;

            return PartialView("Index", aboutCourse);
        }

        /// <summary>
        /// Uploads Syllabus file as a resource onto the DLAP server and creates reference in DLAP Database
        /// </summary>
        /// <param name="model">Bfw.PX.PXPub.Models.Widget</param>
        /// <returns>Json of Bfw.PX.PXPub.Models.FileUploadResults</returns>
        [ValidateInput(false)]
        public JsonResult UploadSyllabus(Widget model)
        {
            List<Resource> resources = new List<Resource>();
            Resource r = new Resource();
            FileUploadResults resultMessage = new FileUploadResults();            
            try
            {
                if (Request.Files[0] != null)
                {
                    string[] acceptedFileTypes = { ".doc", ".txt", ".pdf", ".docx"};
                    var uploadFile = Request.Files[0];
                    var length = uploadFile.ContentLength;

                    Byte[] bStream;
                    using (var binaryReader = new BinaryReader(uploadFile.InputStream))
                    {
                        bStream = binaryReader.ReadBytes(uploadFile.ContentLength);
                        binaryReader.Close();
                    }

                    var filenameVal = System.IO.Path.GetFileName(uploadFile.FileName);
                    var contenttypeVal = uploadFile.ContentType;
                    var cleanCourseID = Context.CourseId.Split(',').FirstOrDefault();
                    var resourceStream = r.GetStream();
                    resourceStream.Write(bStream, 0, length);

                    r.EntityId = model.EntityID;
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
                        return Json(new { resultMessage });
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
            return Json( new { resultMessage } );
        }

        /// <summary>
        /// Doing check to see if the string is a URI
        /// </summary>
        /// <param name="input">string URI</param>
        /// <returns>bool</returns>
        public bool IsUrl(string input)
        {
            Regex expression = new Regex( @"(https?|http)\://|www\.[A-Za-z0-9\.\-]+(/[A-Za-z0-9\?\&\=;\+!'\(\)\*\-\._~%]*)*" );
            return expression.IsMatch(input);
        }

        public JsonResult UpdateLMSID(string lmsId)
        {
            if (!lmsId.IsNullOrEmpty())
            {
                Context.CurrentUser.ReferenceId = lmsId;

                if (UserActions.UpdateUser(Context.CurrentUser))
                {
                    return Json(new
                    {
                        success = true,
                        LmsId = lmsId,
                        userId = Context.CurrentUser.Id
                    });
                }
            }
            return Json(new
            {
                success = false
            });
        }

    }
}