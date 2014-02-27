using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Custom;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Controllers.Contracts;
using Bfw.PX.PXPub.Controllers.Helpers;
using Bfw.PX.PXPub.Controllers.Mappers;
using Bfw.PX.PXPub.Models;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class UploadController : Controller
    {
        /// <summary>
        /// Gets or sets the grade actions.
        /// </summary>
        /// <value>
        /// The grade actions.
        /// </value>
        protected IGradeActions GradeActions
        {
            get;
            set;
        }

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

        protected IContentActions ContentActions { get; set; }
        /// <summary>
        /// Gets or sets the resource map actions.
        /// </summary>
        /// <value>
        /// The resource map actions.
        /// </value>
        protected IResourceMapActions ResourceMapActions
        {
            get;
            set;
        }

        protected IDocumentConverter DocumentConverter { get; set; }

        /// <summary>
        /// Gets or sets the enrollment actions.
        /// </summary>
        /// <value>
        /// The enrollment actions.
        /// </value>
        protected IEnrollmentActions EnrollmentActions
        {
            get
            {
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEnrollmentActions>();
            }
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected IBusinessContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// A const for the upload Max Size.
        /// </summary>
        private const int UploadMaxSize = 26214400; // 25 Megabytes 

        /// <summary>
        /// This will convert objects to JSON strings
        /// </summary>
        JavaScriptSerializer JsonSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="helper">The helper.</param>
        /// <param name="gradeActions">The grade actions.</param>
        /// <param name="resourceMapActions">The resource map actions.</param>
        public UploadController(IBusinessContext context, IContentHelper helper, IGradeActions gradeActions, IResourceMapActions resourceMapActions,
                                 IContentActions contentActions, IDocumentConverter documentConverter)
        {
            Context = context;
            GradeActions = gradeActions;
            ContentHelper = helper;
            ResourceMapActions = resourceMapActions;
            ContentActions = contentActions;
            DocumentConverter = documentConverter;            
            JsonSerializer = new JavaScriptSerializer();
        }

        public ActionResult UploadForm(string parentId,
                                       string onCompleteScript,
                                       Dictionary<string, string> uploadCustomParam,
                                       UploadType uploadType,
                                       UploadFileType uploadFileType,
                                       string onSuccessActionUrl,
                                       string uploadFilePath,
                                       bool addToResourceMap = false,
                                       bool isIncludeDownloadOnlyTypes = false)
        {
            string downloadOnlyDocumentTypes = string.Empty;
            if (isIncludeDownloadOnlyTypes)
            {
                downloadOnlyDocumentTypes = Context.Course.DownloadOnlyDocumentTypes;
            }

            var content = new Upload
            {
                ParentId = parentId,
                OnCompleteScript = onCompleteScript,
                CustomParams = uploadCustomParam,
                UploadType = uploadType,
                UploadFileType = uploadFileType,
                OnSuccessActionUrl = onSuccessActionUrl,
                UploadFilePath = uploadFilePath,
                AddToResourceMap = addToResourceMap,
                DownloadOnlyDocumentTypes = downloadOnlyDocumentTypes

            };

            if (onSuccessActionUrl.Contains("DocumentCollection"))
            {
                ViewData["isScriptLoaded"] = !Request.IsAjaxRequest();

                return View("~/Views/Shared/UploadDocCol.ascx", content);
            }
            else
            {
                return View("~/Views/Shared/Upload.ascx", content);
            }
        }

        public ActionResult UploadAndSubmitForm(string parentId,
                              Dictionary<string, string> uploadCustomParam,
                              string onCompleteScript = "PxAssignment.OnUploadAndSubmitComplete",
                              UploadType uploadType = UploadType.Aspose,
                              UploadFileType uploadFileType = UploadFileType.Restricted,
                              string onSuccessActionUrl = "",
                              string uploadFilePath = "Assets/",
                              string uploadFileName = "",
                              bool addToResourceMap = false,
                              bool isIncludeDownloadOnlyTypes = false,
                              string submitedDocComment = "",
                              bool retainOriginalFile = false)
        {
            string downloadOnlyDocumentTypes = string.Empty;
            if (isIncludeDownloadOnlyTypes)
            {
                downloadOnlyDocumentTypes = Context.Course.DownloadOnlyDocumentTypes;
            }
            var resource = ResourceMapActions.GetResourcesForItem(parentId).LastOrDefault();
            if (resource != null)
            {
                resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.Comment.ToString(), out submitedDocComment);
                resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.FileName.ToString(), out uploadFileName);
            }
            if (!string.IsNullOrEmpty(uploadFileName))
            {
                retainOriginalFile = true;
            }
            var content = new Upload
            {
                ParentId = parentId,
                OnCompleteScript = onCompleteScript,
                CustomParams = uploadCustomParam,
                UploadType = uploadType,
                UploadFileType = uploadFileType,
                OnSuccessActionUrl = onSuccessActionUrl,
                UploadFilePath = uploadFilePath,
                AddToResourceMap = addToResourceMap,
                DownloadOnlyDocumentTypes = downloadOnlyDocumentTypes,
                UploadComment = submitedDocComment,
                RetainOriginalFile = retainOriginalFile,
            };
            ViewData["uploadFileName"] = uploadFileName;
            return View("~/Views/Shared/UploadAndSubmit.ascx", content);
        }


        public ActionResult UploadImage(string parentId,
                               string onCompleteScript,
                               Dictionary<string, string> uploadCustomParam,
                               UploadType uploadType,
                               UploadFileType uploadFileType,
                               string onSuccessActionUrl)
        {
            var content = new Upload
            {
                ParentId = parentId,
                OnCompleteScript = onCompleteScript,
                CustomParams = uploadCustomParam,
                UploadType = uploadType,
                UploadFileType = uploadFileType,
                OnSuccessActionUrl = onSuccessActionUrl
            };
            return View("~/Views/Shared/UploadImage.ascx", content);
        }

        /// <summary>
        /// Get the upload document form for tinymce plugin
        /// </summary>
        /// <returns>upload document form</returns>
        public ActionResult TinyMceUploadDocumentForm()
        {
            return View("~/Views/Shared/TinyMceUploadDocumentForm.ascx", new Upload());
        }
        
        /// <summary>
        /// Update parent and sequence in case of parent id not folder
        /// </summary>
        /// <param name="content"></param>
        /// <param name="enrollmentId"></param>
        /// <returns></returns>
        private Upload updateContent(Upload content, string enrollmentId)
        {
            string parent = content.ParentId;
            Bfw.PX.Biz.DataContracts.ContentItem ci = ContentActions.GetContent(Context.EntityId, parent);
            if (ci.Subtype.ToLower() != "eportfolio")
            {
                content.ParentId = ci.DefaultCategoryParentId;
            }
            return content;
        }



        /// <summary>
        /// Uploads the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public ActionResult Upload(Upload content)
        {
            string sTitle = "";
            string req = Request.Form["uploadFilePath"];
            string fTitle = Request.Form["UploadTitle"];
            string domain = Request.Form["Domain"];
            string filename = content.UploadFile.FileName;
            string response = "";

            //Set domain
            if (!string.IsNullOrEmpty(domain))
            {
                ViewData["Domain"] = domain;
            }

            if (filename.IndexOf('\\') > 0)
            {
                var arrName = filename.Split('\\');
                filename = arrName[arrName.Length - 1].ToString();
            }

            if (!string.IsNullOrEmpty(req))
            {
                content.UploadFilePath = req;
            }

            if (content.UploadFile.ContentLength > UploadMaxSize)
            {
                response = JsonSerializer.Serialize(new UploadResponse
                {
                    Status = "error",
                    ErrorMessage = string.Format(Messages.UPLOAD_SIZE, UploadMaxSize / 1048576)
                });
            }
            else
            {
                //default title
                sTitle = content.UploadTitle == null ? fTitle.Replace(",", "") : content.UploadTitle;
                var hfTitle = fTitle.TrimStart(',');

                try
                {
                    var resourceId = "";
                    switch (content.UploadType)
                    {                        
                        case UploadType.Assignment:
                            var docCollectionId = content.CustomParams["docCollectionId"];
                            ContentHelper.StoreItemDocument(content.ParentId, docCollectionId, sTitle, content.UploadFile, Context.EntityId);
                            break;                        
                        default:
                            ContentHelper.StoreDocument(content.ParentId, sTitle, content.UploadFile, Context.EntityId);
                            break;
                    }

                    response = JsonSerializer.Serialize(new UploadResponse
                    {
                        Status = "success",
                        ParentId = content.ParentId,
                        ResourceId = resourceId,
                        OnSuccessActionUrl = content.OnSuccessActionUrl
                    });
                }
                catch (Exception)
                {
                    response = JsonSerializer.Serialize(new UploadResponse()
                    {
                        Status = "error",
                        ErrorMessage = Messages.UPLOAD_FAILURE,
                        ParentId = content.ParentId,
                    });
                }
            }
            return PartialView("EasyxdmIFrameContainer", response);
        }        

        private void UpdateContentItemsDocumentProperties(Biz.DataContracts.ContentItem ci, string filename, string docId, long fileSize)
        {
            PropertyValue propDate = new PropertyValue();
            propDate.Value = DateTime.Now.GetCourseDateTime().ToString();
            ci.Properties.Add("CreationDate", propDate);

            PropertyValue propName = new PropertyValue();
            propName.Value = filename;
            ci.Properties.Add("FileName", propName);

            PropertyValue propSize = new PropertyValue();
            propSize.Value = fileSize;
            ci.Properties.Add("FileSize", propSize);

            PropertyValue propDocId = new PropertyValue();
            propDocId.Value = docId;
            ci.Properties.Add("DocId", propDocId);
        }

        /// <summary>
        /// Saves the resource as HTML using aspose.
        /// </summary>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <param name="ci">The ci.</param>
        /// <param name="filename">The filename.</param>
        private void SaveResourceAsHtmlUsingAspose(string enrollmentId, Biz.DataContracts.ContentItem ci, string filename, Upload content)
        {
            Resource eportfolioConvertedResource = new Resource()
            {
                ContentType = "text/html",
                EntityId = enrollmentId,
                Url = ci.Href,
                Name = filename
            };

            var stream = DocumentConverter.ConvertDocument(new Biz.DataContracts.DocumentConversion()
            {
                DataStream = content.UploadFile.InputStream,
                FileName = filename,
                OutputType = Biz.DataContracts.DocumentOutputType.Html
            });

            var eportfoliohtmlResourceStream = eportfolioConvertedResource.GetStream();
            stream.CopyTo(eportfoliohtmlResourceStream);
            eportfoliohtmlResourceStream.Flush();

            ContentActions.StoreResources(new List<Resource> { eportfolioConvertedResource });
        }


        /// <summary>
        /// Maps the document collection to content item.
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="enrollmentId">The enrollment id.</param>
        /// <returns></returns>
        private Biz.DataContracts.ContentItem MapDocumentCollectionToContentItem(DocumentCollection dc, string enrollmentId)
        {
            Bfw.PX.Biz.DataContracts.ContentItem ci = dc.ToContentItem(enrollmentId);
            ContentHelper.SetDefaultParent(dc, ci);
            ci.Subtype = "UploadOrCompose";

            var documentCreatedDate = new MetadataValue() { Value = System.DateTime.Now.GetCourseDateTime() };
            ci.Metadata.Add("DOCUMENT_CREATED_DATE", documentCreatedDate);
            return ci;
        }

        /// <summary>
        /// Converts the uploaded content to document collection.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="fTitle">The f title.</param>
        /// <returns></returns>
        private DocumentCollection MapUploadedContentToDocumentCollection(Upload content, string fTitle)
        {
            DocumentCollection dc = ContentHelper.ToDocumentCollection(content);
            dc.ParentId = content.ParentId;
            dc.Title = fTitle;
            dc.DocumentCollectionSubType = "UploadOrCompose";
            if (!string.IsNullOrEmpty(dc.Title))
            {
                if (dc.Title.IndexOf(",") > -1)
                    dc.Title = dc.Title.Substring(1);
            }

            if (string.IsNullOrEmpty(dc.Id))
            {
                dc.Id = Guid.NewGuid().ToString("N");
            }
            return dc;
        }

        /// <summary>
        /// Gets the domain id.
        /// </summary>
        /// <returns></returns>
        private string GetDomainId()
        {
            var domainid = System.Configuration.ConfigurationManager.AppSettings["BfwUsersDomainId"];
            if (Context.Course.Domain != null &&
            !String.IsNullOrEmpty(Context.Course.Domain.Id))
                domainid = Context.Course.Domain.Id;
            return domainid;
        }


        /// <summary>
        /// Uploads the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult UploadAndSubmit(Upload content)
        {
            if (ValidateDueDateForDropbox(content.ParentId))
            {
                List<String> notAllowedFiles = new List<string> { "html", "htm", "js", "jsb", "mhtml", "mht", "xhtml", "xht", "php", "phtml", "php3", "php4", "php5", "phps", "shtml", "jhtml", "pl", "py", "cgi", "exe", "scr", "dll", "msi", "vbs", "bat", "com", "pif", "cmd", "vxd", "cpl" };

                string sTitle = "";
                string req = Request.Form["uploadFilePath"];
                string fTitle = string.Empty;
                string filename = string.Empty;

                if (content.UploadFile != null)
                {
                    fTitle = content.UploadFile.FileName.Substring(0, content.UploadFile.FileName.IndexOf("."));
                    filename = content.UploadFile.FileName;
                }
                if (filename.IndexOf('\\') > 0)
                {
                    var arrName = filename.Split('\\');
                    filename = arrName[arrName.Length - 1].ToString();
                }

                if (!string.IsNullOrEmpty(req))
                {
                    content.UploadFilePath = req;
                }

                if (content.UploadFile != null && content.UploadFile.ContentLength > UploadMaxSize)
                {
                    return Json(new UploadResponse()
                    {
                        Status = "error",
                        ErrorMessage = string.Format("Upload cannot be completed, file size cannot exceed {0} Megabytes", UploadMaxSize / 1048576)
                    });
                }


                string fileExtn = filename.Substring(filename.LastIndexOf(".") + 1).ToLowerInvariant();
                if (content.UploadFile != null && notAllowedFiles.Contains(fileExtn))
                {
                    return Json(new UploadResponse()
                    {
                        Status = "error",
                        ErrorMessage = string.Format("Upload cannot be completed, {0} files are not allowed to upload...", fileExtn)
                    });
                }

                //default title
                sTitle = content.UploadTitle == null ? fTitle.Replace(",", "") : content.UploadTitle;
                var sComment = content.UploadComment;
                var hfTitle = fTitle.TrimStart(',');

                try
                {
                    var resourceId = "";
                    var db = new Dropbox();
                    db.AssignmentStatus = AssignmentStatus.New;
                    if (content.UploadFile != null)
                    {
                        byte[] binaryData;
                        binaryData = new Byte[content.UploadFile.InputStream.Length];
                        long bytesRead = content.UploadFile.InputStream.Read(binaryData, 0, (int)content.UploadFile.InputStream.Length);
                        string base64String = System.Convert.ToBase64String(binaryData, 0, binaryData.Length);
                        content.UploadFile.InputStream.Seek(0, SeekOrigin.Begin);
                        db.Submission = new Models.Submission() { Body = base64String, StreamData = content.UploadFile.InputStream, Name = fTitle.Replace(",", ""), Notes = sComment, SubmittedFileName = filename };
                        db.StudentSubmittedFileSize = Convert.ToInt32(content.UploadFile.InputStream.Length) / 1000 + "K";
                        db.StudentSubmittedFilename = filename;
                    }
                    else if (!content.RetainOriginalFile && content.UploadFile == null)
                    {
                        db.Submission = new Models.Submission() { Notes = sComment };
                    }
                    else if (content.RetainOriginalFile)
                    {
                        var resource = ResourceMapActions.GetResourcesForItem(content.ParentId).LastOrDefault();
                        var submission = GradeActions.GetStudentSubmission(Context.EnrollmentId, content.ParentId);
                        string resouceFilename = "", resouceFilesize = "";
                        resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.FileName.ToString(), out resouceFilename);
                        resource.ExtendedProperties.TryGetValue(ResourceExtendedProperty.FileSize.ToString(), out resouceFilesize);
                        if (submission.StreamData != null)
                        {
                            submission.StreamData.Seek(0, SeekOrigin.Begin);
                        }
                        db.Submission = new Models.Submission() { Body = submission.Body, StreamData = submission.StreamData, Name = resouceFilename.Substring(0, resouceFilename.IndexOf(".")), Notes = sComment, SubmittedFileName = resouceFilename };
                        db.StudentSubmittedFileSize = resouceFilesize;
                        db.StudentSubmittedFilename = resouceFilename;
                    }

                    db.Id = content.ParentId;
                    StoreDropboxSubmission(db, "Submit");

                    return Json(new UploadResponse()
                    {
                        Status = "success",
                        ParentId = content.ParentId,
                        ResourceId = resourceId,
                        OnSuccessActionUrl = content.OnSuccessActionUrl
                    });
                }
                catch (Exception e)
                {
                    return Json(new UploadResponse()
                    {
                        Status = "error",
                        ErrorMessage = e.Message,
                        ParentId = content.ParentId,
                    });
                }
            }
            else
            {
                return Json(new UploadResponse()
                {
                    Status = "error",
                    ErrorMessage = "Not able to submit this dropbox because duedate has passed...",
                    ParentId = content.ParentId,
                });
            }
        }

        /// <summary>
        /// Validate DueDate For Dropbox
        /// </summary>
        /// <returns></returns>
        private bool ValidateDueDateForDropbox(string id)
        {
            bool result = false;
            var curItem = ContentActions.GetContent(String.IsNullOrEmpty(Context.EnrollmentId) ? Context.EntityId : Context.EnrollmentId, id);

            if (curItem.AssignmentSettings != null)
            {
                DateTime dueDate = curItem.AssignmentSettings.DueDate;

                if (curItem.AssignmentSettings.AllowLateSubmission && !curItem.AssignmentSettings.IsAllowLateGracePeriod)
                {
                    result = true;
                }
                else if (curItem.AssignmentSettings.AllowLateSubmission && curItem.AssignmentSettings.IsAllowLateGracePeriod)
                {
                    dueDate = AssignmentHelper.GetGraceDueDate(curItem.AssignmentSettings);
                    
                    if (dueDate > DateTime.Now)
                    {
                        result = true;
                    }
                }
                else if (dueDate > DateTime.Now.GetCourseDateTime())
                {
                    result = true;
                }
            }

            return result;
        }

        public static XElement BuildNode(string data, XName tagName, Int32 lineLength)
        {
            StringBuilder sb = new StringBuilder(data);

            Int32 position = 0;

            while (position < sb.Length)
            {
                sb.Insert(position, Environment.NewLine);
                position += lineLength + Environment.NewLine.Length;
            }

            sb.AppendLine();

            return new XElement(tagName, sb.ToString());
        }

        /// <summary>
        /// Saves Assignment Submission
        /// </summary>
        /// <param name="content">The content</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>A json object of the submission</returns>
        [ValidateInput(false)]
        public JsonResult StoreAssignmentSubmission(Assignment content, string behavior)
        {
            var returnData = new Dictionary<string, string>();
            var status = "";
            string url;

            if (string.IsNullOrEmpty(content.Submission.ResourcePath))
            {
                var resId = Guid.NewGuid().ToString("N");
                url = string.Format("Templates/Data/XmlResources/Documents/Assignments/{0}.pxres", resId);
            }
            else
            {
                url = content.Submission.ResourcePath;
            }

            switch (behavior.ToLowerInvariant())
            {
                case "submit":
                    status = "submitted";
                    var submission = new Bfw.PX.Biz.DataContracts.Submission
                    {
                        ItemId = content.Id,
                        Body = content.Submission.Body,
                        SubmissionType = SubmissionType.Assignment,
                        SubmittedDate = DateTime.Today
                    };


                    GradeActions.AddStudentSubmission(Context.EntityId, submission);
                    SaveSubmission(content, url, status);
                    break;
                case "save":
                case "save as":
                    status = "saved";
                    SaveSubmission(content, url, status);
                    break;
            }

            returnData.Add("status", status);
            returnData.Add("path", url);
            return Json(returnData);
        }

        /// <summary>
        /// Saves Dropbox Submission
        /// </summary>
        /// <param name="content">The content</param>
        /// <param name="behavior">The behavior.</param>
        /// <returns>A json object of the submission</returns>
        [ValidateInput(false)]
        public JsonResult StoreDropboxSubmission(Dropbox content, string behavior)
        {
            var returnData = new Dictionary<string, string>();
            var status = "";
            string url;

            if (string.IsNullOrEmpty(content.Submission.ResourcePath))
            {
                var resId = Guid.NewGuid().ToString("N");
                url = string.Format("Templates/Data/XmlResources/Documents/Assignments/{0}.pxres", resId);
            }
            else
            {
                url = content.Submission.ResourcePath;
            }

            switch (behavior.ToLowerInvariant())
            {
                case "submit":
                    status = "submitted";
                    var submission = new Bfw.PX.Biz.DataContracts.Submission
                    {
                        ItemId = content.Id,
                        Body = content.Submission.Body,
                        SubmissionType = SubmissionType.Assignment,
                        SubmittedDate = DateTime.Today,
                        StreamData = content.Submission.StreamData,
                        Notes = content.Submission.Notes,
                        SubmittedFileName = content.Submission.SubmittedFileName
                    };


                    GradeActions.AddStudentSubmission(Context.EntityId, submission);
                    SaveDropboxSubmission(content, url, status);
                    break;
                case "save":
                case "save as":
                    status = "saved";
                    SaveSubmission(content, url, status);
                    break;
            }

            returnData.Add("status", status);
            returnData.Add("path", url);
            return Json(returnData);
        }


        /// <summary>
        /// Saves the submission.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="url">The URL.</param>
        /// <param name="status">The status.</param>
        private void SaveSubmission(Assignment content, string url, string status)
        {
            var res = ResourceMapActions.GetResourcesForItem(content.Id);
            var submittedResource = ResourceMapActions.GetResourcesForItem(content.Id).SingleOrDefault(x => x.ExtendedProperties[ResourceExtendedProperty.Status.ToString()] == "submitted");

            if (submittedResource != null)
            {
                submittedResource.ExtendedProperties[ResourceExtendedProperty.Status.ToString()] = "unsubmitted";
                XmlResource xResource = new XmlResource() { EntityId = Context.EnrollmentId, Status = submittedResource.Status, Url = submittedResource.Url, Title = submittedResource.Name };
                xResource.ExtendedProperties.Add(ResourceExtendedProperty.AssignmentId.ToString(), submittedResource.ExtendedProperties[ResourceExtendedProperty.AssignmentId.ToString()]);
                xResource.ExtendedProperties.Add(ResourceExtendedProperty.Status.ToString(), submittedResource.ExtendedProperties[ResourceExtendedProperty.Status.ToString()]);
                xResource.ExtendedProperties.Add(ResourceExtendedProperty.WordCount.ToString(), submittedResource.ExtendedProperties[ResourceExtendedProperty.WordCount.ToString()]);
                ContentActions.StoreResources(new List<Resource> { xResource });
            }

            var resource = new XmlResource
            {
                Status = ResourceStatus.Normal,
                Url = url,
                EntityId = Context.EnrollmentId,
                Title = content.Submission.Name,
                Body = content.Submission.Body
            };

            resource.ExtendedProperties.Add(ResourceExtendedProperty.AssignmentId.ToString(), content.Id);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.Status.ToString(), status);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.WordCount.ToString(), content.Submission.WordCount);
            ContentActions.StoreResources(new List<Resource> { resource });
            ResourceMapActions.AddResourceMap(resource, content.Id, "Assignment");
        }


        /// <summary>
        /// Saves the dropbox submission.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="url">The URL.</param>
        /// <param name="status">The status.</param>
        private void SaveDropboxSubmission(Dropbox content, string url, string status)
        {
            var resource = new XmlResource
            {
                Status = ResourceStatus.Normal,
                Url = url,
                EntityId = Context.EnrollmentId,
                Title = content.Submission.Name
            };

            resource.ExtendedProperties.Add(ResourceExtendedProperty.AssignmentId.ToString(), content.Id);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.Status.ToString(), status);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.WordCount.ToString(), content.Submission.WordCount);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.Comment.ToString(), content.Submission.Notes);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.FileName.ToString(), content.StudentSubmittedFilename);
            resource.ExtendedProperties.Add(ResourceExtendedProperty.FileSize.ToString(), content.StudentSubmittedFileSize);
            ContentActions.StoreResources(new List<Resource> { resource });
            ResourceMapActions.AddResourceMap(resource, content.Id, "Assignment");
        }

        /// <summary>
        /// Uploads the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public ActionResult ImageUpload(Upload content)
        {
            string domain = Request.Form["Domain"];
            //Set domain
            if (!string.IsNullOrEmpty(domain))
            {
                ViewData["Domain"] = domain;
            }

            if (content.UploadFile.ContentLength > UploadMaxSize)
            {
                String response = JsonSerializer.Serialize(new UploadResponse
                {
                    Status = "error",
                    ErrorMessage = string.Format("Upload cannot be completed, file size cannot exceed {0} Megabytes", UploadMaxSize / 1048576)
                });

                return PartialView("EasyxdmIFrameContainer", response);
            }

            var enrollmentId = String.IsNullOrEmpty(content.EnrollmentId) ? Context.EnrollmentId : content.EnrollmentId;

            var parentId = content.ParentId;
            if (parentId == null)
            {
                parentId = Context.CourseId;
            }

            try
            {
                var resourceId = ContentHelper.StoreDocument(parentId, enrollmentId + "_" + System.Guid.NewGuid(), content.UploadFile, Context.EntityId);

                String response = JsonSerializer.Serialize(new UploadResponse
                {
                    Status = "success",
                    ParentId = parentId,
                    ResourceId = resourceId,
                    OnSuccessActionUrl = content.OnSuccessActionUrl,
                    FileName = content.UploadFile.FileName
                });

                return PartialView("EasyxdmIFrameContainer", response);
            }
            catch (Exception e)
            {
                String response = JsonSerializer.Serialize(new UploadResponse
                {
                    Status = "error",
                    ErrorMessage = e.Message,
                    ParentId = parentId,
                });

                return PartialView("EasyxdmIFrameContainer", response);
            }
        }       

    }


    /// <summary>
    /// 
    /// </summary>
    public class UploadResponse
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parent id.
        /// </summary>
        /// <value>
        /// The parent id.
        /// </value>
        public string ParentId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the resource id.
        /// </summary>
        /// <value>
        /// The resource id.
        /// </value>
        public string ResourceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the on success action URL.
        /// </summary>
        /// <value>
        /// The on success action URL.
        /// </value>
        public string OnSuccessActionUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets uploaded file name
        /// </summary>
        public string FileName
        {
            get;
            set;
        }
    }
}
