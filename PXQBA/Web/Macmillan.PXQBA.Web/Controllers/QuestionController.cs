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
            bool success = questionManagementService.UpdateQuestionField(CourseHelper.CurrentCourse, questionId, fieldName, fieldValue, isSharedField);
            return JsonCamel(new { isError = !success });
        
        }

        public ActionResult CreateQuestion(string questionType, string bank, string chapter)
        {
            var type = (QuestionType)EnumHelper.GetItemByDescription(typeof(QuestionType), questionType);
            var question = questionManagementService.CreateQuestion(CourseHelper.CurrentCourse, type, bank, chapter);
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
            var question = questionManagementService.GetQuestion(CourseHelper.CurrentCourse, questionId);
            return JsonCamel(CreateQuestionViewModelForEditing(question));
        }

        private QuestionViewModel CreateQuestionViewModelForEditing(Question question)
        {
            QuestionHelper.QuestionIdToEdit = question.Id;
            var tempQuestion = questionManagementService.CreateTemporaryQuestion(CourseHelper.CurrentCourse, question.Id);
            var questionViewModel = Mapper.Map<Question, QuestionViewModel>(tempQuestion, opt => opt.Items.Add(CourseHelper.CurrentCourse.ProductCourseId, CourseHelper.CurrentCourse.ProductCourseId));
            questionViewModel.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), questionViewModel.EntityId, questionViewModel.QuizId);
            questionViewModel.EditorUrl = String.Format(ConfigurationHelper.GetEditorUrlTemplate(), questionViewModel.EntityId, questionViewModel.QuizId, questionViewModel.Id);
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