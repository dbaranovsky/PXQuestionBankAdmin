using System.IO;
using System.Web;
using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;
using Macmillan.PXQBA.Web.ViewModels.Import;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class ImportController : MasterController
    {
        private readonly IQuestionManagementService questionManagementService;

        public ImportController(IQuestionManagementService questionManagementService, IProductCourseManagementService productCourseManagementService, IUserManagementService userManagementService)
            : base(productCourseManagementService, userManagementService)
        {
            this.questionManagementService = questionManagementService;
 
        }

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
          
            return JsonCamel(new { TitleId = courseId, QuestionCount = 5 });
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes((int)file.InputStream.Length);
            string result = System.Text.Encoding.UTF8.GetString(binData);

            return JsonCamel(new {isSuccess = true});
        }


	}
}