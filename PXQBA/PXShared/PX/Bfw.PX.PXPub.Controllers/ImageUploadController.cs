using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bfw.PX.PXPub.Components;
using Bfw.PX.PXPub.Models;
using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using BizDC = Bfw.PX.Biz.DataContracts;
using Bfw.Common.JqGridHelper;
using Bfw.PX.PXPub.Controllers.Mappers;
using System.Configuration;
using System.Web.Script.Serialization;
using Bfw.PX.Biz.ServiceContracts;
using Bfw.PX.PXPub.Controllers.Contracts;

namespace Bfw.PX.PXPub.Controllers
{
    [PerfTraceFilter]
    public class ImageUploadController : Controller
    {
        /// <summary>
        /// Contains business layer context information
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        protected BizSC.IBusinessContext Context { get; set; }
        /// <summary>
        /// Gets or sets the content actions.
        /// </summary>
        /// <value>
        /// The content actions.
        /// </value>
        protected BizSC.IContentActions ContentActions { get; set; }
        /// <summary>
        /// Gets or sets the resource map actions.
        /// </summary>
        /// <value>
        /// The resource map actions.
        /// </value>
        protected BizSC.IResourceMapActions ResourceMapActions { get; set; }

        /// <summary>
        /// Gets or sets the content helper.
        /// </summary>
        /// <value>
        /// The content helper.
        /// </value>
        protected IContentHelper ContentHelper { get; set; }

        private const int MaxJqGridRows = 1000;

        /// <summary>
        /// A const for the upload Max Size.
        /// </summary>
        private const int UploadMaxSize = 26214400; // 25 Megabytes 

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageUploadController"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="contentActions">The content actions.</param>
        /// <param name="resourceMapActions">The resource map actions.</param>
        public ImageUploadController(BizSC.IBusinessContext context, BizSC.IContentActions contentActions, BizSC.IResourceMapActions resourceMapActions, IContentHelper contentHelper)
        {
            Context = context;
            ContentActions = contentActions;
            ResourceMapActions = resourceMapActions;
            ContentHelper = contentHelper;
        }

        /// <summary>
        /// Searches for all resource images for the current course.
        /// </summary>
        /// <param name="sidx">The sidx.</param>
        /// <param name="sord">The sord.</param>
        /// <param name="page">The page.</param>
        /// <param name="uId">The u id.</param>
        /// <returns>
        /// Resource collection in a jqGrid compatible json format
        /// </returns>
        public ActionResult ImageGridData(string sidx, string sord, int page, int? uId)
        {            
            string resourcePath = Context.ExternalResourceBaseUrl + "/" + Context.EnrollmentId + "/";
            page = 1;
            var imageSettings = ConfigurationManager.AppSettings["AllowImageType"];
            var allowFileTypes = new List<string>();
            if (!imageSettings.IsNullOrEmpty())
            {
                allowFileTypes = imageSettings.Split(',').ToList();
            }
            else
            {
                allowFileTypes.Add("jpg");
            }
            //Load up all assignment resources
            var allResources = new List<Resource>();
            foreach (string type in allowFileTypes)
            {
                var resources = ContentActions.ListResources(Context.EnrollmentId, "Assets/*." + type, "").ToList();
                allResources.AddRange(resources);

            }
            var queryableResources = allResources.AsQueryable();
            //initialize jsonData with empty json object
            var jsonData = queryableResources.ToJqGridData(page, MaxJqGridRows, null, "",
                new[] {
                    "ResourceId", "Image", "DateModified"
                });

            if (queryableResources.Any())
            {
                var model = from resource in queryableResources
                    select new
                    {
                        ResourceId = ResourceMapActions.GetResourceId(resource),
                        Image = "<a id='" + resourcePath + resource.Url + "' rel=\"jmenu\" class=\"jmenua\"><img id=\"lvFileList_" + System.Guid.NewGuid() + "_imgList \" src=\"" + resourcePath + resource.Url + "\" style=\"height:300px;width:300px;border-width:0px;padding:5px;\" /></a>",
                        DateModified = resource.ModifiedDate.ToString()
                    };
                jsonData = model.ToJqGridData(page, MaxJqGridRows, null, "",
                    new[] {
                        "ResourceId", "Image", "DateModified"
                    });
            }

            var gridModel = new JqGridModel { Data = jsonData };

            const string colNamesString = @"['ResourceId', '', 'Last Modified']";
            const string colModelString = @"[
            { name: 'ResourceId', index: 'ResourceId', hidden: true, key: true, width: 200, align: 'center' },
            { name: 'Image', index: 'Title', width: 300, align: 'left' },                   
            { name: 'DateModified', index: 'DateModified', hidden: true, width: 200, align: 'center' }]";

            var serializer = new JavaScriptSerializer();
            var colModel = serializer.DeserializeObject(colModelString);
            var colNames = serializer.DeserializeObject(colNamesString);

            gridModel.ColModel = colModel;
            gridModel.ColNames = colNames;
            return Json(gridModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadImageForm(string parentId,
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
                OnSuccessActionUrl = onSuccessActionUrl,
                EnrollmentId = Context.EnrollmentId
            };
            return View(content);
        }

        /// <summary>
        /// Uploads the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public ActionResult ImageUpload(Upload content)
        {
            var jsonSerializer = new JavaScriptSerializer();
            string domain = Request.Form["Domain"];
            //Set domain
            if (!string.IsNullOrEmpty(domain))
            {
                ViewData["Domain"] = domain;
            }

            if (content.UploadFile.ContentLength > UploadMaxSize)
            {
                String response = jsonSerializer.Serialize(new UploadResponse
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
                var resourceId = ContentHelper.StoreDocument(parentId, enrollmentId + "_" + System.Guid.NewGuid(), content.UploadFile, Context.EnrollmentId);

                String response = jsonSerializer.Serialize(new UploadResponse
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
                String response = jsonSerializer.Serialize(new UploadResponse
                {
                    Status = "error",
                    ErrorMessage = e.Message,
                    ParentId = parentId,
                });

                return PartialView("EasyxdmIFrameContainer", response);
            }
        }

        /// <summary>
        /// Returns the ImageUpload view for this Controller.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

    }
}