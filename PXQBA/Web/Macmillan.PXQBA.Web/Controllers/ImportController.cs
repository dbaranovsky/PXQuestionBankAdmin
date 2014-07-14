using System.Web.Mvc;
using Macmillan.PXQBA.Web.ViewModels.Import;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class ImportController : MasterController
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ImportFromFile(int id)
        {
            var model = new ImportFromFileViewModel
                        {
                              FileId = id,
                              IsImported = false
                        };
            return View(model);
        }

        [HttpPost]
        public ActionResult ImportFromFile(int fileId, string courseId)
        {
          
            return JsonCamel(new {IsSuccess = true});
        }


	}
}