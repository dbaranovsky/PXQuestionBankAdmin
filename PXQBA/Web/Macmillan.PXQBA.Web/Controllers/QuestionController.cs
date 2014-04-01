using System.Web.Mvc;
using NLog.Config;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class QuestionController : MasterController
    {
        
        [HttpPost]
        public ActionResult Edit(string questionId, string fieldName, string fieldValue)
        {
            //ToDo: Execute api here
            return JsonCamel(new { result = "success"});
        }
	}
}