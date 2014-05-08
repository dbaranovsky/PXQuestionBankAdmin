using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Business.Models;
using Macmillan.PXQBA.Web.Helpers;

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
        {
            bool isSuccess = bulkOperationService.RemoveFromTitle(questionsId, CourseHelper.CurrentCourse);
            return JsonCamel(new { isError = !isSuccess });
        }

        public ActionResult PublishToTitle(string[] questionsId, int courseId, string bank, string chapter)
        {
            bool isSuccess = bulkOperationService.PublishToTitle(questionsId, courseId, bank, chapter);
            return JsonCamel(new {isError = !isSuccess});
        }
	}
}