using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using NLog.Config;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;

        public QuestionController(IQuestionManagementService questionManagementService)
        {
            this.questionManagementService = questionManagementService;
        }

        [HttpPost]
        public ActionResult Edit(string questionId, string fieldName, string fieldValue)
        {
            var result = questionManagementService.UpdateQuestionField(questionId, fieldName, fieldValue);
            return JsonCamel(new { result = "success"});
        }
	}
}