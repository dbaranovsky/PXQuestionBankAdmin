using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class BulkOperationController : MasterController
    {
        private readonly IBulkOperationService bulkOperationService;

        public BulkOperationController(IBulkOperationService bulkOperationService)
        {
            this.bulkOperationService = bulkOperationService;
        }

        [HttpPost]
        public ActionResult SetStatus(string[] questionsId, QuestionStatus newQuestionStatus)
        {
            bool isSuccess = bulkOperationService.SetStatus(questionsId, newQuestionStatus);
            return JsonCamel(new { isError = !isSuccess });
        }


        [HttpPost]
        public ActionResult RemoveFromTitle(string[] questionsId)
        {   //Todo: implement remove questions from title
           // bool isSuccess = bulkOperationService.RemoveFromTitle(questionsId);
            return JsonCamel(new { isError = false });
        }
	}
}