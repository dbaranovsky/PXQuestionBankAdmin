using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using AutoMapper;
using Bfw.Common;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels;
using Macmillan.PXQBA.Web.ViewModels.Versions;
using Newtonsoft.Json;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IQuestionMetadataService questionMetadataService;
        private readonly IProductCourseManagementService productCourseManagementService;
    
        public QuestionController(IQuestionManagementService questionManagementService,   IQuestionMetadataService questionMetadataService, IProductCourseManagementService productCourseManagementService)
        {
            this.questionManagementService = questionManagementService;
            this.questionMetadataService = questionMetadataService;
            this.productCourseManagementService = productCourseManagementService;
        }

        [HttpPost]
        public ActionResult UpdateMetadataField(string questionId, string fieldName, string fieldValue)
        {
            bool success = questionManagementService.UpdateQuestionField(CourseHelper.CurrentCourse, questionId, fieldName, fieldValue);
            return JsonCamel(new { isError = !success });
        
        }

        [HttpPost]
        public ActionResult UpdateSharedMetadataField(string questionId, string fieldName, List<string> fieldValues)
        {
            bool success = questionManagementService.UpdateSharedQuestionField(CourseHelper.CurrentCourse, questionId, fieldName, fieldValues);
            return JsonCamel(new { isError = !success });

        }

        [HttpPost]
        public ActionResult BulkUpdateMetadataField(string[] questionIds, string fieldName, string fieldValue, bool isSharedField = false)
        {
            bool success = questionManagementService.BulklUpdateQuestionField(CourseHelper.CurrentCourse, questionIds, fieldName, fieldValue, isSharedField);
            return JsonCamel(new { isError = !success });

        }

        public ActionResult CreateQuestion(string questionType, string bank, string chapter)
        {
            var question = questionManagementService.CreateQuestion(CourseHelper.CurrentCourse, questionType, bank, chapter);
            return JsonCamel(CreateQuestionViewModelForEditing(question));
            
        }

        [HttpPost]
        public ActionResult DuplicateQuestion(string questionId, string version = null)
        {
            var question = questionManagementService.DuplicateQuestion(CourseHelper.CurrentCourse, questionId, version);
            return JsonCamel(CreateQuestionViewModelForEditing(question));

        }

        public ActionResult GetAvailibleMetadata()
        {
            return JsonCamel(questionMetadataService.GetAvailableFields(CourseHelper.CurrentCourse).Select(MetadataFieldsHelper.Convert).ToList());
        }

        public ActionResult GetAvailibleMetadataByCourseId(string courseId)
        {
            var course = productCourseManagementService.GetProductCourse(courseId);
            if (course != null)
            {
                return JsonCamel(questionMetadataService.GetAvailableFields(course).Select(MetadataFieldsHelper.Convert).ToList());
            }
            return JsonCamel(questionMetadataService.GetAvailableFields(CourseHelper.CurrentCourse).Select(MetadataFieldsHelper.Convert).ToList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateQuestion(string questionJsonString)
        {
            // manual JSON deserialize is nessessary becauese of invalid model mapping on controller
            var questionViewModel = JsonConvert.DeserializeObject<QuestionViewModel>(questionJsonString);
            var question = Mapper.Map<Question>(questionViewModel);

            try
            {
                questionManagementService.UpdateQuestion(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit, question);
            }
            catch (Exception ex)
            {
                return JsonCamel(new { isError = true });
            }
            return JsonCamel(new { isError = false });
        }

        public ActionResult GetQuestion(string questionId)
        {
            var question = questionManagementService.GetQuestion(CourseHelper.CurrentCourse, questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(question));
        }

        private QuestionViewModel CreateQuestionViewModelForEditing(Question question)
        {
            QuestionHelper.QuestionIdToEdit = question.Id;
            var tempQuestion = questionManagementService.CreateTemporaryQuestion(CourseHelper.CurrentCourse, question.Id);
            var questionViewModel = Mapper.Map<Question, QuestionViewModel>(tempQuestion, opt => opt.Items.Add(CourseHelper.CurrentCourse.ProductCourseId, CourseHelper.CurrentCourse));
            questionViewModel.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), questionViewModel.EntityId, questionViewModel.QuizId);

            questionViewModel.QuestionType = question.CustomUrl;
            questionViewModel.EditorUrl = CustomQuestionHelper.GetEditorUrl(questionViewModel.QuestionType,
                                                                            questionViewModel.Id,
                                                                            questionViewModel.EntityId,
                                                                            questionViewModel.QuizId);          
           

            questionViewModel.GraphEditorHtml = CustomQuestionHelper.GetGraphEditor(question.InteractionData,
                                                                                    questionViewModel.Id,
                                                                                    question.CustomUrl);
           

            return questionViewModel;
        }


        /// <summary>
        /// Remove shared questions from current title 
        /// </summary>
        /// <param name="questionsId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFromTitle(string[] questionsId)
        {
            bool isSuccess = questionManagementService.RemoveFromTitle(questionsId, CourseHelper.CurrentCourse);
            return JsonCamel(new { isError = !isSuccess });
        }

        /// <summary>
        /// Share questions with selected  title, bank and chapter
        /// </summary>
        /// <param name="questionsId"></param>
        /// <param name="courseId"></param>
        /// <param name="bank"></param>
        /// <param name="chapter"></param>
        /// <returns></returns>
        public ActionResult PublishToTitle(string[] questionsId, int courseId, string bank, string chapter)
        {
            bool isSuccess = questionManagementService.PublishToTitle(questionsId, courseId, bank, chapter, CourseHelper.CurrentCourse);
            return JsonCamel(new { isError = !isSuccess });
        }


        /// <summary>
        /// Returns current question's vesrion history 
        /// </summary>
        /// <returns></returns>
        public ActionResult GetQuestionVersions()
        {
            var versionHistory = Mapper.Map<QuestionHistoryViewModel>(questionManagementService.GetVersionHistory(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit), opt => opt.Items.Add(CourseHelper.CurrentCourse.ProductCourseId, CourseHelper.CurrentCourse.ProductCourseId));
            return JsonCamel(versionHistory);
        }


        public ActionResult GetVersionPreviewLink(string version)
        {
            var tempVersion = questionManagementService.GetTemporaryQuestionVersion(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit, version);
            return JsonCamel(new { Url = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), tempVersion.EntityId, tempVersion.QuizId) });
        }

        public ActionResult PublishDraftToOriginal(string draftQuestionId)
        {
            var success = questionManagementService.PublishDraftToOriginal(CourseHelper.CurrentCourse, draftQuestionId);
            return JsonCamel(new {isError = !success});
        }

        public ActionResult CreateDraft(string questionId, string version = null)
        {
            var question = questionManagementService.CreateDraft(CourseHelper.CurrentCourse, questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(question));

        }
	}
  
}