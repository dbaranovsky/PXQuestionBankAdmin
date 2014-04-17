using System;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Common.Helpers;
using Macmillan.PXQBA.Web.Helpers;
using NLog.Config;

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
            var question = Mapper.Map<Question, QuestionViewModel>(questionManagementService.CreateQuestion(CourseHelper.CurrentCourse, (QuestionType)questionType, bank, chapter));
            question.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrl(), "200117", "AHWDG");
            return JsonCamel(question);
            
        }

        [HttpPost]
        public ActionResult DuplicateQuestion(string questionId)
        {

            var question = Mapper.Map<Question, QuestionViewModel>(questionManagementService.DuplicateQuestion(CourseHelper.CurrentCourse, questionId));
            question.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrl(), "200117", "AHWDG");
            return JsonCamel(question);

        }

        public ActionResult GetAvailibleMetadata()
        {
            return JsonCamel(questionMetadataService.GetAvailableFields().Select(MetadataFieldsHelper.Convert).ToList());
        }

        public ActionResult UpdateQuestion(Question question)
        {
            questionManagementService.UpdateQuestion(question);
            return JsonCamel(new { isError = false });
        }

        public ActionResult GetQuestion(string questionId)
        {
            var question = Mapper.Map<Question, QuestionViewModel>(questionManagementService.GetQuestion(questionId));
            question.ActionPlayerUrl = String.Format(ConfigurationHelper.GetActionPlayerUrl(), "200117", "AHWDG");
            return JsonCamel(question);
        }
	}
}