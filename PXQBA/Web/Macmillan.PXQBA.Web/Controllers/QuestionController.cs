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
        public ActionResult Edit(string questionId, string fieldName, string fieldValue)
        {
            bool success = false;
            if (fieldName.Equals(MetadataFieldNames.Sequence))
            {
                questionManagementService.UpdateQuestionSequence(CourseHelper.CurrentCourse, questionId, int.Parse(fieldValue));
                success = true;
            }
            else
            {
                success = questionManagementService.UpdateQuestionField(questionId, fieldName, fieldValue);
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
            //TODO: need to create question view model from temp question when moved to real API
            var questionViewModel = Mapper.Map<Question, QuestionViewModel>(question);
            questionViewModel.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrlTemplate(), tempQuestion.EntityId, tempQuestion.QuizId);
            questionViewModel.EditorUrl = String.Format(ConfigurationHelper.GetEditorUrlTemplate(), tempQuestion.EntityId, tempQuestion.QuizId, tempQuestion.Id);
            return questionViewModel;
        }
	}
}