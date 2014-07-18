using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels.Import;
using Macmillan.PXQBA.Web.ViewModels.Pages;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class ImportController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IUserManagementService userManagementService;
        private readonly IProductCourseManagementService productCourseManagementService;

        public ImportController(IQuestionManagementService questionManagementService, IProductCourseManagementService productCourseManagementService, IUserManagementService userManagementService)
            : base(productCourseManagementService, userManagementService)
        {
            this.questionManagementService = questionManagementService;
            this.userManagementService = userManagementService;
            this.productCourseManagementService = productCourseManagementService;
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
            questionManagementService.ImportFile(fileId, courseId);
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
            if (string.Equals(fileExt, EnumHelper.GetEnumDescription(QuestionFileType.QTI), StringComparison.CurrentCultureIgnoreCase) && UserCapabilitiesHelper.Capabilities.Contains(Capability.ImportQuestionfromQTI))
            {
                return true;
            }

            if (string.Equals(fileExt, EnumHelper.GetEnumDescription(QuestionFileType.QML), StringComparison.CurrentCultureIgnoreCase) && UserCapabilitiesHelper.Capabilities.Contains(Capability.ImportQuestionfromQML))
            {
                return true;
            }

            if (string.Equals(fileExt, EnumHelper.GetEnumDescription(QuestionFileType.Respondus), StringComparison.CurrentCultureIgnoreCase) && UserCapabilitiesHelper.Capabilities.Contains(Capability.ImportQuestionfromRespondus))
            {
                return true;
            }

            return false;
        }


        public ActionResult FormTitleStep1()
        {
            return View();
        }

        public ActionResult FromTitleStep2(string courseId)
        {
            QuestionListViewModel viewModel = new QuestionListViewModel()
            {
                CourseId = courseId
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult SaveQuestionsForImport(string[] questionsId)
        {
            ImportQuestionsHelper.QuestionsForImport = new QuestionForImportContainer()
                                                       {
                                                           CourseId = CourseHelper.CurrentCourse.ProductCourseId,
                                                           QuestionsId = questionsId
                                                       };
            return JsonCamel(new { IsError = false });
        }

        public ActionResult FromTitleStep3()
        {
            var viewModel = new ImportFromTitleStep3ViewModel()
                            {
                                CourseId = ImportQuestionsHelper.QuestionsForImport.CourseId
                            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ImportQuestionsTo(string toCourseId)
        {
            var questionsForImport = ImportQuestionsHelper.QuestionsForImport;
            var capabilities = userManagementService.GetUserCapabilities(toCourseId);

            if (!capabilities.Contains(Capability.ImportQuestionFromTitle))
            {
                return JsonCamel(new ImportQuestionsToTitleResult
                {
                    IsError = true,
                    ErrorMessage = "You have no capabilities for import to this title."
                });
            }

            Course sourceCourse = productCourseManagementService.GetProductCourse(questionsForImport.CourseId, true);
            Course targetCourse = productCourseManagementService.GetProductCourse(toCourseId, true);
            var success = questionManagementService.ImportQuestions(sourceCourse, questionsForImport.QuestionsId, targetCourse);

            return JsonCamel(new ImportQuestionsToTitleResult
                             {
                                 IsError = !success,
                                 QuestionImportedCount = questionsForImport.QuestionsId.Count()
                             });
        }
	}
}