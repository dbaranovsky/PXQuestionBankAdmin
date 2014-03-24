using Macmillan.PXQBA.Business.Contracts;
using System.Web.Mvc;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionMetadataController : MasterController
    {
        private readonly IQuestionMetadataService questionMetadataService;

        public QuestionMetadataController(IQuestionMetadataService questionMetadataService)
        {
            this.questionMetadataService = questionMetadataService;
        }

        public ActionResult GetAvailableFields()
        {
            return JsonCamel(questionMetadataService.GetAvailableFields());
        }
	}
}