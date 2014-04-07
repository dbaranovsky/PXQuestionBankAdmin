﻿using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
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
            var result = questionManagementService.UpdateQuestionField(questionId, fieldName, fieldValue);
            return JsonCamel(new { isError = !result });
        
        }

        public ActionResult GetNewQuestionTemplate()
        {
            return JsonCamel(questionManagementService.GetNewQuestionTemplate());
        }

        [HttpPost]
        public ActionResult GetDuplicateQuestionTemplate(string questionId)
        {
            return JsonCamel(questionManagementService.GetDuplicateQuestionTemplate(questionId));
        }

        public ActionResult GetAvailibleMetadata()
        {
            return JsonCamel(questionMetadataService.GetAvailableFields());
        
        }

        public void CreateQuestion(string courseId, Question question)
        {
            questionManagementService.CreateQuestion(courseId, question);
        }
	}
}