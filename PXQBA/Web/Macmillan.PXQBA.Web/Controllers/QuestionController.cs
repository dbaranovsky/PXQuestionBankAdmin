using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Bfw.Common;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Business.Services;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Helpers;
using Macmillan.PXQBA.Web.ViewModels;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;
        private readonly IQuestionMetadataService questionMetadataService;
    
        public QuestionController(IQuestionManagementService questionManagementService,   IQuestionMetadataService questionMetadataService)
        {
            this.questionManagementService = questionManagementService;
            this.questionMetadataService = questionMetadataService;
        }

        [HttpPost]
        public ActionResult UpdateMetadataField(string questionId, string fieldName, string fieldValue, bool isSharedField = false)
        {
            bool success;
            if (isSharedField)
            {
                success = questionManagementService.UpdateSharedQuestionField(questionId,fieldName, fieldValue);
            }
            else
            {
                success = questionManagementService.UpdateQuestionField(CourseHelper.CurrentCourse, questionId, fieldName, fieldValue);
            }
           
            return JsonCamel(new { isError = !success });
        
        }

        public ActionResult CreateQuestion(int questionType, string bank, string chapter)
        {
            var question = questionManagementService.CreateQuestion(CourseHelper.CurrentCourse, (QuestionType) questionType, bank, chapter);
            return JsonCamel(CreateQuestionViewModelForEditing(question));
            
        }

        [HttpPost]
        public ActionResult DuplicateQuestion(string questionId)
        {
            var question = questionManagementService.DuplicateQuestion(CourseHelper.CurrentCourse, questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(question));

        }

        public ActionResult GetAvailibleMetadata()
        {
            return JsonCamel(questionMetadataService.GetAvailableFields(CourseHelper.CurrentCourse).Select(MetadataFieldsHelper.Convert).ToList());
        }

        public ActionResult UpdateQuestion(Question question)
        {
            questionManagementService.UpdateQuestion(CourseHelper.CurrentCourse, QuestionHelper.QuestionIdToEdit, question);
            return JsonCamel(new { isError = false });
        }

        public ActionResult GetQuestion(string questionId)
        {
            var question = questionManagementService.GetQuestion(questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(question));
        }

        private QuestionViewModel CreateQuestionViewModelForEditing(Question question)
        {
            QuestionHelper.QuestionIdToEdit = question.Id;
            var tempQuestion = questionManagementService.CreateTemporaryQuestion(CourseHelper.CurrentCourse, question.Id);
            //TODO: need to create question view model from the temp question when moved to real API
            var questionViewModel = Mapper.Map<Question, QuestionViewModel>(question);
            questionViewModel.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), tempQuestion.EntityId, tempQuestion.QuizId);
            questionViewModel.EditorUrl = String.Format(ConfigurationHelper.GetEditorUrlTemplate(), tempQuestion.EntityId, tempQuestion.QuizId, tempQuestion.Id);
            return questionViewModel;
        }

        /// <summary>
        ///  Flag a question, in order to indicate to other editors that the question needs to be reviewed and possibly revised
        /// </summary>
        /// <param name="questionId"></param>
        /// <param name="isFlagged"></param>
        /// <returns></returns>
        public ActionResult FlagQuestion(string questionId, bool isFlagged)
        {
            // todo: imlement service call
            return JsonCamel(new { isError = false });
        }

	}
}