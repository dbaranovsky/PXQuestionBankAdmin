using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bfw.Common.Collections;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.QuestionParserModule.DataContracts;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Common.Models;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels.Import;
using Macmillan.PXQBA.Web.ViewModels.Pages;
using ValidationResult = Macmillan.PXQBA.Business.Models.ValidationResult;

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



        public ActionResult ImportToTitle()
        {
            var validationResult = (ValidationResult) TempData["ValidationResult"];
            if (validationResult == null)
            {
                return View(new ImportFromFileViewModel
                            {
                               IsNothingToImport = true
                            });
            }
            var model = new ImportFromFileViewModel
                        {
                              FileId = validationResult.FileValidationResults.First().Id,
                              IsImported = false,
                              QuestionToImport = validationResult.FileValidationResults.First().QuestionParsed,
                              QuestionSkipped = validationResult.FileValidationResults.First().QuestionSkipped,
                              ParsingErrorMessage = string.Join("<br/>",validationResult.FileValidationResults.First().ValidationErrors)
                        };
            return View(model);
        }


        public ActionResult ImportFromFile(int fileId, string courseId)
        {
            ParsedFile file =  questionManagementService.GetValidatedFile(fileId);
            if(!IsAllowed(Path.GetExtension(file.FileName), userManagementService.GetUserCapabilities(courseId)))
            {
                 return JsonCamel(new { NotAllowed = true });
            }
            int questionCount = questionManagementService.ImportFile(fileId, courseId);
            return JsonCamel(new { TitleId = courseId, QuestionCount = questionCount});


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

            byte[] binData = StreamHelper.ReadFully(file.InputStream);
           
            var result = questionManagementService.ValidateFile(file.FileName, binData);
            TempData["ValidationResult"] = result;
            return JsonCamel(new {isValidated = true, fileId = result.FileValidationResults.First().Id, questionCount = result.FileValidationResults.First().QuestionParsed});
            
        }


        private bool IsAllowed(string fileExt, IEnumerable<Capability> capabilities)
        {
            if (string.Equals(fileExt, EnumHelper.GetEnumDescription(QuestionFileType.QTI), StringComparison.CurrentCultureIgnoreCase) && 
                capabilities.Contains(Capability.ImportQuestionfromQTI))
            {
                return true;
            }

            if (string.Equals(fileExt, EnumHelper.GetEnumDescription(QuestionFileType.QML), StringComparison.CurrentCultureIgnoreCase) && 
                capabilities.Contains(Capability.ImportQuestionfromQML))
            {
                return true;
            }

            if (string.Equals(fileExt, EnumHelper.GetEnumDescription(QuestionFileType.Respondus), StringComparison.CurrentCultureIgnoreCase) && 
                capabilities.Contains(Capability.ImportQuestionfromRespondus))
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