using System.Web.Mvc;
using Macmillan.PXQBA.Business.Contracts;

namespace Macmillan.PXQBA.Web.Controllers
{
    public class TitleListController : MasterController
    {
        private readonly IProductCourseManagementService productCourseManagementService;

        public TitleListController(IProductCourseManagementService productCourseManagementService)
        {
            this.productCourseManagementService = productCourseManagementService;
        }
        //
        // GET: /TitleList/
        public ActionResult Index()
        {
            productCourseManagementService.GetAvailableCourses();
            return View();
        }
	}
}