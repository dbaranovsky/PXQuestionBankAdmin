using System.Web.Mvc;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class BulkOperationController : MasterController
    {
        [HttpPost]
        public ActionResult SetStatus(string[] questionsId, QuestionStatus newQuestionStatus)
        {
            bool success = true;
            return JsonCamel(new { isError = !success });
        }
	}
}