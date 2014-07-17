using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels.Import;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class ImportController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;

        public ImportController(IQuestionManagementService questionManagementService, IProductCourseManagementService productCourseManagementService, IUserManagementService userManagementService)
            : base(productCourseManagementService, userManagementService)
        {
            this.questionManagementService = questionManagementService;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ImportFromFile(int id)
        {
            var model = new ImportFromFileViewModel
                        {
                              FileId = id,
                              IsImported = false
                        };
            return View(model);
        }

        [HttpPost]
        public ActionResult ImportFromFile(int fileId, string courseId)
        {
            //TODO: check capabilities before importing
            return JsonCamel(new { TitleId = courseId, QuestionCount = 5 });


            //var isAllowed = IsAllowed(fileExt.ToLower());
            //if (!isAllowed)
            //{
            //    return JsonCamel(new { NotAllowed = true });
            //}
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {

            if (file == null || file.ContentLength == 0)
            {
                return JsonCamel(new {isValidated = false});
            }

            var fileExt = Path.GetExtension(file.FileName);

            if (string.IsNullOrEmpty(fileExt))
            {
                return JsonCamel(new { isValidated = false });
            }

            byte[] binData;
            using (var reader = new BinaryReader(file.InputStream))
            {
                binData = reader.ReadBytes((int) file.InputStream.Length);
            }
            
            var result = questionManagementService.ValidateFile(file.FileName, binData);

            if (!result.IsValidated)
            {
                return JsonCamel(new { isValidated = false});
            }

            return JsonCamel(new {isValidated = true, fileId = result.FileValidationResults.First().Id});
        }


        private bool IsAllowed(string fileExt, string courseId)
        {
            if (string.Equals(fileExt, ".qti", StringComparison.OrdinalIgnoreCase) && UserCapabilitiesHelper.Capabilities.Contains(Capability.ImportQuestionfromQTI))
            {
                return true;
            }

            if (string.Equals(fileExt, ".qml", StringComparison.OrdinalIgnoreCase) && UserCapabilitiesHelper.Capabilities.Contains(Capability.ImportQuestionfromQML))
            {
                return true;
            }

            if (string.Equals(fileExt, ".txt", StringComparison.OrdinalIgnoreCase) && UserCapabilitiesHelper.Capabilities.Contains(Capability.ImportQuestionfromRespondus))
            {
                return true;
            }

            return false;
        }


        public ActionResult FormTitleStep1()
        {
            return View();
        }
	}
}